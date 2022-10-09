using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
	public class CleaverRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omega Cleaver Relic");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RelicTile>(), 5);
			Item.width = 32;
			Item.height = 46;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.buyPrice(0, 5);
		}
	}
}