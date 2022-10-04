using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
	public class SoIRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seed of Infection Relic");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RelicTile>(), 2);
			Item.width = 34;
			Item.height = 46;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.buyPrice(0, 5);
		}
	}
}