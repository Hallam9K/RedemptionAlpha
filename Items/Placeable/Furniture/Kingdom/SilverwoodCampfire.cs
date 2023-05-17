using Redemption.Rarities;
using Redemption.Tiles.Furniture.Kingdom;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Kingdom
{
    public class SilverwoodCampfire : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Life regen is increased when near a campfire");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodCampfireTile>(), 0);
			Item.width = 30;
			Item.height = 16;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
		{
			CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddIngredient<KingdomTorch>(5)
				.Register();
		}
	}
}