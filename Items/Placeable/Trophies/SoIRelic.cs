using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
    public class SoIRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Seed of Infection Relic");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RelicTile>(), 2);
			Item.width = 34;
			Item.height = 46;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.buyPrice(0, 5);
		}
	}
}