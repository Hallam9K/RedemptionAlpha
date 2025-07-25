using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBucket_Proj : ModProjectile
    {
        public Vector2[] oldPos = new Vector2[8];
        public float[] oldRot = new float[8];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 8;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 30;
            Projectile.velocity.Y += 0.1f;

            if (Main.rand.NextBool(4) && Projectile.timeLeft < 170)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(7, Projectile.rotation - MathHelper.PiOver2), ModContent.ProjectileType<BucketSplash>(), Projectile.damage / 2, 0, Main.myPlayer);

            for (int k = oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldRot[k] = oldRot[k - 1];
            }
            oldPos[0] = Projectile.Center;
            oldRot[0] = Projectile.rotation;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                Color color = Color.White * ((oldPos.Length - k) / (float)oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color.WithAlpha(0) * .4f, oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X > 4 || oldVelocity.X < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y > 4 || oldVelocity.Y < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity.Y *= 0.7f;
            Projectile.velocity.X *= 0.7f;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Iron, 0f, 0f, 100, default, 2.5f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
        }
    }
    public class BucketSplash : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water");
            ElementID.ProjWater[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Water, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Main.dust[dust].scale = 3f;
            Main.dust[dust].noGravity = true;

            Projectile.velocity.Y += 0.70f;

            if (Projectile.localAI[0]++ >= 10)
                Projectile.friendly = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => target.AddBuff(BuffID.Wet, 180);
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => target.AddBuff(BuffID.Wet, 180);
    }
}