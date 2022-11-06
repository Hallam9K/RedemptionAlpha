using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class EaglecrestGolemPlush : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eaglecrest Golem Plushie");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<EaglecrestGolemPlushTile>(), 0);
			Item.width = 46;
			Item.height = 44;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 1, 50, 0);
		}
	}
}