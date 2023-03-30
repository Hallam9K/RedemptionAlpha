using Redemption.Tiles.Furniture.Shade;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadesteelHangingCell : ModItem
	{
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hanging Shadesteel Cell");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadesteelHangingCellTile>(), 0);
			Item.width = 24;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 10, 0);
		}
	}
}