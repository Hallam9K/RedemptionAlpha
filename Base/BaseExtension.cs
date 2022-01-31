using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Globals.Player;
using Terraria;

namespace Redemption.BaseExtension
{
    public static class BaseExtension
    {
        /// <summary>References the RedePlayer instance.</summary>
        public static RedePlayer Redemption(this Player player) => player.GetModPlayer<RedePlayer>();
        /// <summary>References the ScreenPlayer instance.</summary>
        public static ScreenPlayer RedemptionScreen(this Player player) => player.GetModPlayer<ScreenPlayer>();
        /// <summary>References the Radiation instance.</summary>
        public static Radiation RedemptionRad(this Player player) => player.GetModPlayer<Radiation>();
        /// <summary>References the BuffPlayer instance.</summary>
        public static BuffPlayer RedemptionPlayerBuff(this Player player) => player.GetModPlayer<BuffPlayer>();
        /// <summary>References the RedeNPC instance.</summary>
        public static RedeNPC Redemption(this NPC npc) => npc.GetGlobalNPC<RedeNPC>(true);
        /// <summary>References the BuffNPC instance.</summary>
        public static BuffNPC RedemptionNPCBuff(this NPC npc) => npc.GetGlobalNPC<BuffNPC>(true);
        /// <summary>References the GuardNPC instance.</summary>
        public static GuardNPC RedemptionGuard(this NPC npc) => npc.GetGlobalNPC<GuardNPC>(true);
        /// <summary>References the RedeItem instance.</summary>
        public static RedeItem Redemption(this Item item) => item.GetGlobalItem<RedeItem>();
        /// <summary>References the RedeProjectile instance.</summary>
        public static RedeProjectile Redemption(this Projectile proj) => proj.GetGlobalProjectile<RedeProjectile>();
    }
}
