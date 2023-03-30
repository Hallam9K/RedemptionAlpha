using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class PonderingTreesPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pondering Trees");
            // Tooltip.SetDefault("'A. Tied'");
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PonderingTreesPaintingTile>(), 0);
			Item.width = 36;
			Item.height = 36;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 50, 0);
		}
	}
}