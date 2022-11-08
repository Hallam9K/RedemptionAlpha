using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class AncientAutoPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ancient Technology");
            Tooltip.SetDefault("'D. Juice'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientAutoPaintingTile>(), 0);
			Item.width = 28;
			Item.height = 28;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 50, 0);
		}
	}
}