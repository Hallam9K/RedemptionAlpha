using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara_Proj2 : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Melee/Ukonvasara";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukonvasara");
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }

        public override void SetSafeDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Length = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().IsHammer = true;
        }
        public ref float Length => ref Projectile.localAI[0];
        private Vector2 startVector;
        private Vector2 vector;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<HammerBuff>(), 10);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.Redemption().contactImmune = true;
            Projectile.direction = player.direction;
            Projectile.spriteDirection = player.direction;
            Projectile.Center = player.MountedCenter + vector;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.localAI[1]++ == 0)
                {
                    SoundEngine.PlaySound(CustomSounds.ElectricSlash2, player.position);
                    startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation());
                    vector = startVector * Length;
                }
                if (Projectile.localAI[1] % 4 == 0 && Projectile.timeLeft > 25)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<UkonSpark_Proj>(), Projectile.damage / 3, 0, Main.myPlayer);
                }
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            }
            if (Projectile.timeLeft <= 25)
                player.velocity *= 0.9f;
            else
            {
                player.velocity = RedeHelper.PolarVector(30, Projectile.velocity.ToRotation());
                Vector2 position = player.Center + (Vector2.Normalize(player.velocity) * 30f);
                Dust dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.Electric)];
                dust.position = position;
                dust.velocity = (player.velocity.RotatedBy(1.57) * 0.33f) + (player.velocity / 4f);
                dust.position += player.velocity.RotatedBy(1.57);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
                dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.Electric)];
                dust.position = position;
                dust.velocity = (player.velocity.RotatedBy(-1.57) * 0.33f) + (player.velocity / 4f);
                dust.position += player.velocity.RotatedBy(-1.57);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
            }
            if ((!player.channel || player.velocity.Length() < 1) && Projectile.timeLeft > 25)
                Projectile.timeLeft = 25;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 60;
            target.immune[Projectile.owner] = 0;

            if (Main.myPlayer == Projectile.owner)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<UkonArrowStrike>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 1);
                Main.projectile[p].DamageType = DamageClass.Melee;
                Main.projectile[p].localAI[0] = 35;
                Main.projectile[p].alpha = 0;
                Main.projectile[p].position.Y -= 540;
                Main.projectile[p].frame = 12;
                Main.projectile[p].netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
