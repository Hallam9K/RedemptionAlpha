using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public bool TechnicallyMelee;
        public bool Unparryable;
        public override void SetDefaults(Projectile projectile)
        {
            if (ProjectileLists.IsTechnicallyMelee.Contains(projectile.type))
                TechnicallyMelee = true;
        }

    }
    public abstract class TrueMeleeProjectile : ModProjectile
    {
        public float SetSwingSpeed(float speed)
        {
            Terraria.Player player = Main.player[Projectile.owner];
            return speed * player.meleeSpeed;
        }

        public virtual void SetSafeDefaults() { }

        public override void SetDefaults()
        {
            SetSafeDefaults();
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().TechnicallyMelee = true;
        }
    }
}