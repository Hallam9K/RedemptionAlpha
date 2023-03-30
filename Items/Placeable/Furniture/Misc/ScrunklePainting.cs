using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class ScrunklePainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Scrunkle");
            // Tooltip.SetDefault("'A. Tied'");
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ScrunklePaintingTile>(), 0);
			Item.width = 38;
			Item.height = 38;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 75, 0);
		}
	}
}