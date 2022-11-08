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
			DisplayName.SetDefault("Emerald Heart");
            Tooltip.SetDefault("'D. Juice'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<EmeraldHeartPaintingTile>(), 0);
			Item.width = 42;
			Item.height = 30;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 3, 0, 0);
		}
	}
}