using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Redemption.Globals;

namespace Redemption.Projectiles.Misc
{
    public class ProjDeath : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("death");
        }
        public override void SetDefaults()
        {
            Projectile.width = 2000;
            Projectile.height = 2000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 6;
        }
        public override void AI()
        {
            foreach (Projectile target in Main.projectile)
            {
                if (Projectile == target || !target.active || target.damage <= 0 || target.hostile || target.GetGlobalProjectile<RedeGlobalProjectile>().TechnicallyMelee)
                    continue;

                target.Kill();
            }
        }
    }
}