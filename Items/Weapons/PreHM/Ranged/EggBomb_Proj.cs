using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class EggBomb_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Ranged/EggBomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Bomb");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            if (Projectile.localAI[0] < 180)
                Projectile.velocity.Y += 0.2f;

            if (Projectile.localAI[0]++ == 180)
            {
                Projectile.velocity *= 0;
                Projectile.alpha = 255;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                SoundEngine.PlaySound(SoundID.NPCDeath11 with { Volume = .5f }, Projectile.position);
                for (int i = 0; i < 8; i++)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MothronEgg, Projectile.velocity.X * 0.5f,
                        Projectile.velocity.Y * 0.5f, Scale: 2);

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 3;
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 6;
                    Main.dust[dust].noGravity = true;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int g = 0; g < 3; g++)
                    {
                        int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                        Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                        Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                    }
                }
                Rectangle boom = new((int)Projectile.Center.X - 40, (int)Projectile.Center.Y - 40, 80, 80);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                        continue;

                    target.immune[Projectile.whoAmI] = 20;
                    int hitDirection = Projectile.Center.X > target.Center.X ? -1 : 1;
                    BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                }
            }
            if (Projectile.localAI[0] == 182)
                Projectile.friendly = false;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.whoAmI] = 20;

            if (Projectile.localAI[0] < 180)
                Projectile.localAI[0] = 180;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 + 6);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity.Y *= 0.6f;
            Projectile.velocity.X *= 0.6f;
            return false;
        }
    }
}