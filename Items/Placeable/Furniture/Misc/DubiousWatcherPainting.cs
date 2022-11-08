using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class DubiousWatcherPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dubious Watcher");
            Tooltip.SetDefault("'[REDACTED]'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<DubiousWatcherPaintingTile>(), 0);
			Item.width = 18;
			Item.height = 26;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 50, 0);
		}
	}
}