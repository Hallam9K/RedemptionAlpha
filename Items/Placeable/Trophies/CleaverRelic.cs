using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
    public class CleaverRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Omega Cleaver Relic");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RelicTile>(), 5);
			Item.width = 32;
			Item.height = 46;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.buyPrice(0, 5);
		}
	}
}