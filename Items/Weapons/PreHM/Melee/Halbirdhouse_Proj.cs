using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class Halbirdhouse_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Halbirdhouse");
        }
        private Vector2 startVector;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.alpha = 255;
            Projectile.Redemption().TechnicallyMelee = true;
        }

        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public float Rot;
        public int HitCount;
        public ref float Timer => ref Projectile.localAI[1];
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Projectile.spriteDirection = player.direction;

            player.SetCompositeArmFront(true, Length >= 40 ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            if (Timer++ == 0)
            {
                if (HitCount >= 3 && Projectile.owner == Main.myPlayer)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 4, ModContent.ProjectileType<ChickenEgg_Proj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                    Main.projectile[p].DamageType = DamageClass.Melee;
                    Main.projectile[p].netUpdate = true;
                    HitCount = 0;
                }
                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation());
                Rot = MathHelper.ToRadians(Main.rand.NextFloat(-10, 10));
                Length = 30;
            }

            if (Timer < 7)
                Length *= 1.2f;
            else
                Length *= 0.2f;
            Length = MathHelper.Clamp(Length, 36, 60);

            vector = startVector.RotatedBy(Rot) * Length;
            if (Timer >= 9)
            {
                if (player.channel)
                    Timer = 0;
                else
                    Projectile.Kill();
            }

            if (Timer > 1)
                Projectile.alpha = 0;

            Projectile.Center = player.MountedCenter + vector;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCount++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 v = Projectile.Center - RedeHelper.PolarVector(28, (Projectile.Center - player.Center).ToRotation());

            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}