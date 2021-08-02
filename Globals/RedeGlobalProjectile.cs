using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Redemption
{
    public class ProjectileProperties : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public bool TechnicallyMelee;
        public override void SetDefaults(Projectile projectile)
        {
            if (ProjectileLists.IsTechnicallyMelee.Contains(projectile.type))
                TechnicallyMelee = true;
        }
    }
}