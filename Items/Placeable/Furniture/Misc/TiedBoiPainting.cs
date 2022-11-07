using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class TiedBoiPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Boi");
            Tooltip.SetDefault("'O. Tomato'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<TiedBoiPaintingTile>(), 0);
			Item.width = 38;
			Item.height = 32;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 2, 0, 0);
		}
	}
}