using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodSofa : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodSofaTile>(), 0);
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = 9999;
			Item.value = 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 5)
				.AddIngredient(ItemID.Silk, 2)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}