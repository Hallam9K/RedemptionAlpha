using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        /// <summary>References the AbilityPlayer instance.</summary>
        public static AbilityPlayer RedemptionAbility(this Player player) => player.GetModPlayer<AbilityPlayer>();
        /// <summary>References the RedeNPC instance.</summary>
        public static RedeNPC Redemption(this NPC npc) => npc.GetGlobalNPC<RedeNPC>();
        /// <summary>References the BuffNPC instance.</summary>
        public static BuffNPC RedemptionNPCBuff(this NPC npc) => npc.GetGlobalNPC<BuffNPC>();
        /// <summary>References the GuardNPC instance.</summary>
        public static GuardNPC RedemptionGuard(this NPC npc) => npc.GetGlobalNPC<GuardNPC>();
        /// <summary>References the RedeItem instance.</summary>
        public static RedeItem Redemption(this Item item) => item.GetGlobalItem<RedeItem>();
        /// <summary>References the </summary>
        public static ItemUseGlow RedemptionGlow(this Item item) => item.GetGlobalItem<ItemUseGlow>();
        /// <summary>References the RedeProjectile instance.</summary>
        public static RedeProjectile Redemption(this Projectile proj) => proj.GetGlobalProjectile<RedeProjectile>();

        /// <summary>Shorthand for converting degrees of rotation into a radians equivalent.</summary>
        public static float InRadians(this float degrees) => MathHelper.ToRadians(degrees);
        /// <summary>Shorthand for converting radians of rotation into a degrees equivalent.</summary>
        public static float InDegrees(this float radians) => MathHelper.ToDegrees(radians);
        public static Rectangle AnimationFrame(this Texture2D texture, ref int frame, ref int frameTick, int frameTime, int frameCount, bool frameTickIncrease, int overrideHeight = 0)
        {
            if (frameTick >= frameTime)
            {
                frameTick = -1;
                frame = frame == frameCount - 1 ? 0 : frame + 1;
            }
            if (frameTickIncrease)
                frameTick++;
            return new Rectangle(0, overrideHeight != 0 ? overrideHeight * frame : (texture.Height / frameCount) * frame, texture.Width, texture.Height / frameCount);
        }
    }
}
