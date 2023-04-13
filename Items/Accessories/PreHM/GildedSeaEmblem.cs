using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class GildedSeaEmblem : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.WaterS, ElementID.IceS, ElementID.FireS);
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Gilded Sea Emblem");
            Tooltip.SetDefault(ElementID.WaterS + " elemental weapons has a chance to soak enemies" +
                "\nSoaked enemies are more sluggish and prone to " + ElementID.IceS + " damage" +
                "\n15% increased " + ElementID.WaterS + " elemental damage and resistance" +
                "\n10% decreased " + ElementID.FireS + " elemental damage and resistance");*/
            ElementID.ItemWater[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
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