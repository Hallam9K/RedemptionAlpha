using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class DoppelsSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Steel Sword Fragment");
        }

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<DoppelsSwordTile>(), 0);
			Item.width = 46;
			Item.height = 50;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(0, 0, 50, 0);
		}
	}
}