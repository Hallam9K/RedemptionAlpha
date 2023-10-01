using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Base;
using Redemption.Globals;

namespace Redemption.Projectiles.Magic
{
    public class NoidanNuoli : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Noidan Nuoli");
            ElementID.ProjArcane[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<NoidanSauvaDust>(), Projectile.velocity.X, Projectile.velocity.Y, 200, Scale: 1.5f);
                dust.velocity += Projectile.velocity * 0.3f;
                dust.velocity *= 0.2f;
                dust.noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            int dustType = ModContent.DustType<NoidanSauvaDust>();
            int pieCut = 16;
            for (int m = 0; m < pieCut; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType, 0f, 0f, 100, Color.White, 1.2f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)pieCut * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
        }
    }
}
