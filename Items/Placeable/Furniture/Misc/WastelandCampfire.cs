using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class WastelandCampfire : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Life regen is increased when near a campfire\n" +
				"'Now that's a hot wheel'"); */
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<WastelandCampfireTile>(), 0);
			Item.width = 30;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 0, 0);
        }
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.Wood, 10)
				.AddIngredient(ModContent.ItemType<WastelandTorch>(), 5)
				.Register();
		}
	}
}