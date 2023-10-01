using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OO_Laser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Laser");
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                    Projectile.frame = 7;
            }
            Lighting.AddLight(Projectile.Center, 1f * Projectile.Opacity, 0.4f * Projectile.Opacity, 0.4f * Projectile.Opacity);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.velocity *= 1.02f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0f) * Projectile.Opacity;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, Scale: 1.3f);
                Main.dust[dustIndex].velocity *= 2f;
            }
        }
    }
}