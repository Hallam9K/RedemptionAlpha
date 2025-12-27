using Redemption.BaseExtension;
using Redemption.Globals.NPCs;
using Redemption.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles
{
    public abstract class ModRedeProjectile : ModProjectile
    {
        public virtual bool? SafeCanHitNPC(NPC target) => null;
        public override bool? CanHitNPC(NPC target)
        {
            if (SafeCanHitNPC(target) != null)
            {
                return SafeCanHitNPC(target);
            }

            if (target.Redemption().spiritSummon || !Projectile.Redemption().friendlyHostile)
                return null;
            if (target.type == NPCID.TargetDummy)
                return false;

            bool targetTarget = false;
            if (Host() != null)
                targetTarget = Host().HasNPCTarget && target.whoAmI == Host()?.TranslatedTargetIndex;
            return targetTarget || (Host()?.ModNPC is ModRedeNPC redeNPC && redeNPC.SpecialNPCTargets(target)) ? null : false;
        }

        public Entity HostAttacker()
        {
            if (Projectile.ai[2] < 0)
                return null;
            NPC host = Main.npc[(int)Projectile.ai[2]];
            if (!host.active || host.ModNPC is not ModRedeNPC)
                return null;
            return (host.ModNPC as ModRedeNPC).Attacker();
        }
        public NPC Host()
        {
            if (Projectile.ai[2] == -1)
                return null;
            NPC host = Main.npc[(int)Projectile.ai[2]];
            if (host.active)
                return host;
            return null;
        }
    }
}