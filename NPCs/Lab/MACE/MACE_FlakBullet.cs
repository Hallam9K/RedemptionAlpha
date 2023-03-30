using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACE_FlakBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flak Bullet");
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.2f, Projectile.Opacity * 0.1f, 0f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return true;
        }
    }
}