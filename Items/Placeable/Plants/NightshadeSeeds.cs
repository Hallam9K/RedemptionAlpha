using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Plants;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Plants
{
    public class NightshadeSeeds : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 25;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NightshadeTile>(), 0);
			Item.maxStack = 9999;
			Item.width = 12;
			Item.height = 14;
			Item.value = 80;
			Item.rare = ItemRarityID.Blue;
		}
	}
}