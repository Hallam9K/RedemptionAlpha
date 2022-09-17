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
            DisplayName.SetDefault("Life Fruit Bio-Container");
            Tooltip.SetDefault("Naturally grows Life Fruits");
        }
        public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BiocontainerTile>(), 0);
			Item.width = 32;
			Item.height = 30;
			Item.maxStack = 99;
			Item.value = Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Cyan;
		}
	}
}