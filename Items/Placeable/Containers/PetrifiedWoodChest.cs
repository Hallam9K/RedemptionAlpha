using Redemption.Tiles.Containers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Containers
{
    public class PetrifiedWoodChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodChestTile>(), 0);
			Item.width = 32;
			Item.height = 30;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 8)
				.AddRecipeGroup(RecipeGroupID.IronBar, 2)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}