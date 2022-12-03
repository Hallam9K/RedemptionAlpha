using Redemption.DamageClasses;
using Redemption.Globals.Player;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Face)]
    public class SpiritMonocle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("The increase is based on 3x your current Spirit Level");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 0, 20, 40);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance<RitualistClass>() += (player.GetModPlayer<RitualistPlayer>().SpiritLevel + 1) * 3;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int critAmt = (Main.LocalPlayer.GetModPlayer<RitualistPlayer>().SpiritLevel + 1) * 3;
            TooltipLine tooltip1Line = new(Mod, "Tooltip1", critAmt + "% increased ritual critical strike chance");
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            if (tooltipLocation != -1)
                tooltips.Insert(tooltipLocation, tooltip1Line);
        }
    }
}