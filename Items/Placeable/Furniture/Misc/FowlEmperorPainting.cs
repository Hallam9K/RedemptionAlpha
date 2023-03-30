using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class FowlEmperorPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("The Legendary-er Being");
            // Tooltip.SetDefault("'A. Tied'");
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<FowlEmperorPaintingTile>(), 0);
			Item.width = 26;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 26, 0);
		}
	}
}