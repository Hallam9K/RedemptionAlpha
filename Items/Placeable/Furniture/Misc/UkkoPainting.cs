using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class UkkoPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ukko Huutaa Pilvelle");
            Tooltip.SetDefault("'A. Tied'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<UkkoPaintingTile>(), 0);
			Item.width = 36;
			Item.height = 36;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 1, 0, 0);
		}
	}
}