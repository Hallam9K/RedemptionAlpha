using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Exit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III");
            Main.projFrames[Projectile.type] = 14;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 106;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 14)
                    Projectile.frame = 12;
            }
            if (Projectile.frame >= 10)
                Projectile.velocity.Y -= 1f;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}