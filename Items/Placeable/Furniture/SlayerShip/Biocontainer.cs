using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class Biocontainer : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bio-Container");
            Tooltip.SetDefault("Naturally grows Life Fruits\n" +
                "'A small container created by King Slayer in order to grow plants in space.\n" +
                "It simulates specific environments depending on the settings, this one seems ideal for growing rare jungle plant-life'");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BiocontainerTile>(), 0);
			Item.width = 32;
			Item.height = 30;
			Item.maxStack = 9999;
			Item.value = Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Cyan;
		}
	}
}