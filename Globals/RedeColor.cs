using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria;

namespace Redemption
{
    public class RedeColor
    {
        public static Color NebColour => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(255, 0, 174), new Color(108, 0, 255), new Color(255, 0, 174));
        public static Color GirusTier => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(255, 0, 0), new Color(196, 70, 30), new Color(255, 0, 0));
        public static Color SoullessColour => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.DarkGray, Color.Black, Color.DarkGray);
        public static Color AncientColour => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(141, 134, 135), new Color(241, 165, 62), new Color(141, 134, 135));
        public static Color VlitchGlowColour => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.Red, Color.Crimson, Color.Red);
        public static Color FadeColour1 => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.White * 0f, Color.White * 0.4f, Color.White * 0f);
        public static Color HeatColour => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.Orange * 0f, Color.White * 0.4f, Color.Orange * 0f);
        public static Color COLOR_GLOWPULSE => Color.White * (Main.mouseTextColor / 255f);
        public static Color RedPulse => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.White, Color.Red * 0.6f, Color.White);
        public static Color EnergyPulse => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightBlue, Color.IndianRed, Color.LightGreen, Color.LightGoldenrodYellow, Color.LightBlue);
        public static Color GreenPulse => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightGreen, Color.Green, Color.LightGreen, Color.White, Color.LightGreen);
    }
}