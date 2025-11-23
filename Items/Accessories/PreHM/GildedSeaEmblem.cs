using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class GildedSeaEmblem : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.WaterS, ElementID.IceS, ElementID.FireS);
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 38;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.seaEmblem = true;
            modPlayer.ElementalDamage[ElementID.Water] += 0.15f;
            modPlayer.ElementalDamage[ElementID.Fire] -= 0.1f;

            modPlayer.ElementalResistance[ElementID.Water] += 0.15f;
            modPlayer.ElementalResistance[ElementID.Fire] -= 0.1f;

        }
    }
}
