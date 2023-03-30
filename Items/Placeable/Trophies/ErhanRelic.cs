using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
    public class ErhanRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Erhan Relic");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RelicTile>(), 0);
			Item.width = 30;
			Item.height = 52;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.buyPrice(0, 5);
		}
	}
}