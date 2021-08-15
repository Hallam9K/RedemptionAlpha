using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Plants;

namespace Redemption.Items.Placeable.Plants
{
    public class NightshadeSeeds : ModItem
    {
        public override void SetDefaults()
		{
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.placeStyle = 0;
			Item.width = 12;
			Item.height = 14;
			Item.value = 80;
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<NightshadeTile>();
		}
	}
}