using Redemption.UI;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.CrossMod;

public static class FablesHelper
{
    private static readonly Mod fables = CrossMod.Fables.Instance;

    public static bool DisplayFablesBossIntroCard(string bossName, string bossTitle, int duration, bool flipped, Color edgeColor, Color titleColor, Color nameColorChroma1, Color nameColorChroma2, string musicTitle = "", string composerName = "")
    {
        if (!CrossMod.Fables.Enabled)
            return false;

        if (string.IsNullOrEmpty(musicTitle))
            return (bool)fables.Call("vfx.displayBossIntroCard", Language.GetTextValue(bossName), Language.GetTextValue(bossTitle), duration, flipped, edgeColor, titleColor, nameColorChroma1, nameColorChroma2);
        else
            return (bool)fables.Call("vfx.displayBossIntroCard", Language.GetTextValue(bossName), Language.GetTextValue(bossTitle), duration, flipped, edgeColor, titleColor, nameColorChroma1, nameColorChroma2, musicTitle, composerName);
    }

    public static void DisplayBossIntroCard(string bossName, string bossTitle, int duration, bool flipped, Color edgeColor, Color titleColor, Color nameColorChroma1, Color nameColorChroma2, string musicTitle = "", string composerName = "")
    {
        if (CrossMod.Fables.Enabled && DisplayFablesBossIntroCard(bossName, bossTitle, duration, flipped, edgeColor, titleColor, nameColorChroma1, nameColorChroma2, musicTitle, composerName))
            return;

        TitleCard.BroadcastTitle(NetworkText.FromKey(bossName), 60, 90, 0.8f, titleColor, NetworkText.FromKey(bossTitle));
    }
}