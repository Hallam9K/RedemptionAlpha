using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class SkullDiggerPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tragic Protector");
            Tooltip.SetDefault("'BoRKman'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SkullDiggerPaintingTile>(), 0);
			Item.width = 38;
			Item.height = 38;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 50, 0);
		}
	}
}