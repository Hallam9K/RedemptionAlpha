using Redemption.Items.Usable.Potions;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.CrossMod;

public static class SpiritHelper
{
    private static readonly Mod spirit = CrossMod.Reforged.Instance;

    public static bool PlayerBotanist(Player player)
    {
        if (!CrossMod.Reforged.Enabled)
            return false;
        return (bool)spirit.Call("PlayerBotanist", player);
    }

    public static void AddUndead(int type, bool excludeDeathAnim = false)
    {
        if (!CrossMod.Reforged.Enabled)
            return;
        spirit.Call("AddUndead", type, excludeDeathAnim);
    }

    public static bool HasBackpack(Player player)
    {
        if (!CrossMod.Reforged.Enabled)
            return false;
        return (bool)spirit.Call("HasBackpack", player);
    }
}
internal class SpiritSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod) => CrossMod.Reforged.Enabled;

    public override void PostSetupContent()
    {
        Tuple<int, Color, bool>[] potionVatInfo =
        {
            Tuple.Create(ItemType<VendettaPotion>(), new Color(164, 77, 187), false),
            Tuple.Create(ItemType<CharismaPotion>(), new Color(230, 220, 110), false),

            Tuple.Create(ItemType<HydraCorrosionPotion>(), new Color(77, 255, 247), true),
            Tuple.Create(ItemType<SkirmishPotion>(), new Color(90, 191, 93), true),
            Tuple.Create(ItemType<VigourousPotion>(), new Color(218, 37, 109), true),
        };

        foreach (var potion in potionVatInfo)
            AddPotionVat(potion.Item1, potion.Item2, potion.Item3);
    }

    private static bool AddPotionVat(int itemType, Color color, bool isDecorative)
    {
        return (bool)CrossMod.Reforged.Instance.Call("AddPotionVat", itemType, color, isDecorative);
    }
}