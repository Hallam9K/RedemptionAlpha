using Microsoft.Xna.Framework;
using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public bool TechnicallyMelee;
        public bool IsHammer;
        public bool IsAxe;
        public bool Unparryable;
        public override void SetDefaults(Projectile projectile)
        {
            if (ProjectileLists.IsTechnicallyMelee.Contains(projectile.type))
                TechnicallyMelee = true;
        }
        public override void ModifyHitNPC(Projectile projectile, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsAxe && crit)
                damage += damage / 2;
        }
        public static void Decapitation(Terraria.NPC target, ref int damage, ref bool crit, int chance = 200)
        {
            if (target.life < target.lifeMax && NPCLists.SkeletonHumanoid.Contains(target.type))
            {
                if (Main.rand.NextBool(chance))
                {
                    CombatText.NewText(target.getRect(), Color.Orange, "Decapitated!");
                    target.GetGlobalNPC<RedeNPC>().decapitated = true;
                    damage = damage < target.life ? target.life : damage;
                    crit = true;
                }
            }
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
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
            Projectile.GetGlobalProjectile<RedeProjectile>().TechnicallyMelee = true;
        }
    }
}