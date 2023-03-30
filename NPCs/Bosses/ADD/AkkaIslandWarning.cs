using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaIslandWarning : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Warning");
        }
        public override void SetDefaults()
        {
            Projectile.width = 1696;
            Projectile.height = 320;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 190;
            Projectile.timeLeft = 600;
        }
    }
}