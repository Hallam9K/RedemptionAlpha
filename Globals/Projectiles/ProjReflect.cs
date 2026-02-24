using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class ProjReflect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public float ReflectDamageIncrease;
        public override void ModifyHitNPC(Projectile projectile, Terraria.NPC npc, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (ReflectDamageIncrease is 0)
                return;
            modifiers.FinalDamage *= ReflectDamageIncrease;
        }
        public static bool ProjBlockBlacklist(Projectile proj, bool countHoming = false)
        {
            return proj.minion || proj.ownerHitCheck || proj.Redemption().TechnicallyMelee || proj.Redemption().ParryBlacklist || Main.projPet[proj.type] || proj.sentry || (countHoming && ProjectileID.Sets.CultistIsResistantTo[proj.type]);
        }
        public static bool FriendlyReflectCheck(Projectile proj, Projectile target, int dmgCap = 100)
        {
            return target.whoAmI != proj.whoAmI && target.hostile && target.damage <= NPCHelper.HostileProjDamage(dmgCap) && ProjectileLoader.CanHitPlayer(target, Main.LocalPlayer);
        }
        public static bool FriendlyReflectCheck(Projectile target, Terraria.Player player, int dmgCap = 100)
        {
            return target.hostile && target.damage <= NPCHelper.HostileProjDamage(dmgCap) && ProjectileLoader.CanHitPlayer(target, player);
        }
        public static bool FriendlyReflectCheck(Projectile proj, Projectile target, Terraria.NPC npc, int dmgCap = 100)
        {
            return target.whoAmI != proj.whoAmI && FriendlyReflectCheck(target, npc, dmgCap);
        }
        public static bool FriendlyReflectCheck(Projectile target, Terraria.NPC npc, int dmgCap = 100)
        {
            return target.hostile && target.damage <= NPCHelper.HostileProjDamage(dmgCap) && ProjectileLoader.CanHitNPC(target, npc) != false;
        }

        public static bool HostileReflectCheck(Projectile proj, Projectile target, Terraria.NPC npc, int dmgCap = 100)
        {
            return target.whoAmI != proj.whoAmI && HostileReflectCheck(target, npc, dmgCap);
        }
        public static bool HostileReflectCheck(Projectile target, Terraria.NPC npc, int dmgCap = 100)
        {
            return target.friendly && target.damage <= dmgCap && ProjectileLoader.CanHitNPC(target, npc) != false;
        }

        public static void FriendlyReflectEffect(Projectile target, bool xVelOnly = true, float velDecrease = 1, float dmgIncrease = 1f)
        {
            if (target.hostile || target.friendly)
            {
                target.hostile = false;
                target.friendly = true;
                target.Redemption().friendlyHostile = false;
            }
            target.RedemptionReflect().ReflectDamageIncrease = NPCHelper.HostileProjDamageMultiplier() * dmgIncrease;
            if (xVelOnly)
                target.velocity.X = -target.velocity.X * velDecrease;
            else
                target.velocity = -target.velocity * velDecrease;

            target.reflected = true;
        }
        public static void HostileReflectEffect(Projectile target, bool xVelOnly = true, float velDecrease = 1)
        {
            if (target.hostile || target.friendly)
            {
                target.hostile = true;
                target.friendly = false;
                target.Redemption().friendlyHostile = false;
            }
            target.damage /= NPCHelper.HostileProjDamageMultiplier();
            if (xVelOnly)
                target.velocity.X = -target.velocity.X * velDecrease;
            else
                target.velocity = -target.velocity * velDecrease;

            target.reflected = true;
        }
    }
}