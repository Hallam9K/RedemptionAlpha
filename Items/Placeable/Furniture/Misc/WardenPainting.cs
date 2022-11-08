using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class WardenPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Depressed But Still Handsome");
            Tooltip.SetDefault("'Anti M.'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<WardenPaintingTile>(), 0);
			Item.width = 54;
			Item.height = 38;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 2, 0, 0);
		}
	}
}