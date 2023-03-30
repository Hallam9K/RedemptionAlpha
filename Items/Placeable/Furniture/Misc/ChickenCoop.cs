using Redemption.Globals;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class ChickenCoop : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("Occasionally spawns chicken eggs");

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ChickenCoopTile>(), 0);
			Item.width = 44;
			Item.height = 42;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 15, 0);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RedeRecipe.ChickenRecipeGroup, 2)
				.AddRecipeGroup(RecipeGroupID.Wood, 30)
				.AddIngredient(ItemID.Hay, 10)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}