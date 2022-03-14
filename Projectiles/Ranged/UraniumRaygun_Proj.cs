using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;
using Redemption.Base;

namespace Redemption.Projectiles.Ranged
{
    public class UraniumRaygun_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Uranium Ring");
        }
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hostile = false;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.alpha = 50;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.Redemption().Unparryable = true;
        }
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;
            if (offsetLeft)
            {
                Projectile.scale -= 0.03f;
                if (Projectile.scale <= 0.7f)
                {
                    Projectile.scale = 0.7f;
                    offsetLeft = false;
                }
            }
            else
            {
                Projectile.scale += 0.03f;
                if (Projectile.scale >= 1.3f)
                {
                    Projectile.scale = 1.3f;
                    offsetLeft = true;
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5;
        }
        public override void Kill(int timeLeft)
        {
            for (int m = 0; m < 16; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.GreenFairy, 0f, 0f, 100, Color.White, 1.6f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)16 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
        }
    }
}