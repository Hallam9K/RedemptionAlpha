using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class KingChickenPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Legendary Being");
            Tooltip.SetDefault("'H. Ska'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<KingChickenPaintingTile>(), 0);
			Item.width = 26;
			Item.height = 26;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 25, 0);
		}
	}
}