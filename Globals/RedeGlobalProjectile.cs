using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public bool TechnicallyMelee;
        public bool Unparryable = false;
        public override void SetDefaults(Projectile projectile)
        {
            if (ProjectileLists.IsTechnicallyMelee.Contains(projectile.type))
                TechnicallyMelee = true;
        }
    }
}