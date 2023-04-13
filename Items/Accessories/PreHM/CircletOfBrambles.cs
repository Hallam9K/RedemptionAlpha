using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria.Localization;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Face)]
    public class CircletOfBrambles : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.NatureS);
        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Circlet of Brambles");
            /* Tooltip.SetDefault("Every 5th use of a magic weapon shoots a spread of stingers" +
                "\nIncreased life regeneration while in the Jungle\n" +
                "6% increased " + ElementID.NatureS + " elemental critical strike chance"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 36;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.RedemptionPlayerBuff().thornCirclet = true;
            if (player.ZoneJungle)
            {
                player.lifeRegen += 2;
            }
        }
	}
}
