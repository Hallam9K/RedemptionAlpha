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
			// DisplayName.SetDefault("King Slayer King Slaying");
            // Tooltip.SetDefault("'Anti M.'");
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<KSPaintingTile>(), 0);
			Item.width = 52;
			Item.height = 36;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 2, 0, 0);
		}
	}
}