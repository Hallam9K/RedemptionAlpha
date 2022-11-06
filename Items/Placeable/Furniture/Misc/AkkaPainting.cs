using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class AkkaPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Goddess of Nature");
            Tooltip.SetDefault("'D. Juice'");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AkkaPaintingTile>(), 0);
			Item.width = 50;
			Item.height = 34;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 2, 0, 0);
		}
	}
}