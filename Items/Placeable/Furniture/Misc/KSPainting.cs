using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class KSPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("King Slayer King Slaying");
            Tooltip.SetDefault("'Anti M.'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<KSPaintingTile>(), 0);
			Item.width = 52;
			Item.height = 36;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 2, 0, 0);
		}
	}
}