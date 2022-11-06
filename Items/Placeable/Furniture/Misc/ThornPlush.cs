using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class ThornPlush : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thorn Plushie");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ThornPlushTile>(), 0);
			Item.width = 38;
			Item.height = 40;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 1, 0, 0);
		}
	}
}