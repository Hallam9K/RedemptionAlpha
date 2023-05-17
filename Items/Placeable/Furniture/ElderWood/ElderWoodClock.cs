using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodClock : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("'Strange... It has only 8 numbers'");

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodClockTile>(), 0);
			Item.width = 18;
			Item.height = 40;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 10)
				.AddRecipeGroup(RecipeGroupID.IronBar, 3)
				.AddIngredient(ItemID.Glass, 6)
				.AddTile(TileID.Sawmill)
				.Register();
        }
	}
}
