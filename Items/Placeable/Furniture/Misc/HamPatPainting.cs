using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class HamPatPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ham Pat");
            // Tooltip.SetDefault("'O. Tomato'");
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<HamPatPaintingTile>(), 0);
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 10, 0, 0);
		}
	}
}