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
			// DisplayName.SetDefault("Thorn Plushie");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ThornPlushTile>(), 0);
			Item.width = 46;
			Item.height = 42;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 1, 0, 0);
		}
	}
}