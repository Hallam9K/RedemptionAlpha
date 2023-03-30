using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class EmeraldHeartPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Emerald Heart");
            // Tooltip.SetDefault("'D. Juice'");
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<EmeraldHeartPaintingTile>(), 0);
			Item.width = 42;
			Item.height = 30;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 3, 0, 0);
		}
	}
}