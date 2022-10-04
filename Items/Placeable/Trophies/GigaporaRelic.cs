using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
	public class GigaporaRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omega Gigapora Relic");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RelicTile>(), 6);
			Item.width = 30;
			Item.height = 44;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.buyPrice(0, 5);
		}
	}
}