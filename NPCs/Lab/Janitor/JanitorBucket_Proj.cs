using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBucket_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bucket");
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