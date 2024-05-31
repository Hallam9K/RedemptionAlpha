using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class PZGauntlet_Proj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infectious Fist");
            Main.projFrames[Projectile.type] = 7;
            ElementID.ProjPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (float k = 0; k < 6.28f; k += 0.3f)
            {
                float x = 2 * MathF.Cos(k);
                float y = 1 * MathF.Sin(k);
                Vector2 ellipse = new(x, y);
                Vector2 rotVec = ellipse.RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Green, rotVec * 2f);
                dust.noGravity = true;
            }
        }
        public override bool? CanHitNPC(NPC target) => Projectile.frame <= 3 ? null : false;
        public override void AI()
        {
            if (Projectile.timeLeft > 90)
            {
                if (++Projectile.frameCounter >= 2)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 3)
                        Projectile.frame = 2;
                }
            }
            else
            {
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 7)
                    {
                        Projectile.Kill();
                    }
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Pitch = 0.5f }, Projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Vector2 vector;
                double angle = Main.rand.NextDouble() * MathHelper.Pi * 2f;
                vector.X = (float)(Math.Sin(angle) * 2);
                vector.Y = (float)(Math.Cos(angle) * 2);
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center + vector, 4, 4, DustID.Clentaminator_Green)];
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * 0.2f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 7;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
