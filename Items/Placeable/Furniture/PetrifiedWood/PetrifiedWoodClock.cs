using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodClock : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodClockTile>(), 0);
			Item.width = 20;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 10)
				.AddRecipeGroup(RecipeGroupID.IronBar, 3)
				.AddIngredient(ItemID.Glass, 6)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}