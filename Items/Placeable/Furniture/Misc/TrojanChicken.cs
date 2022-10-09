using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class TrojanChicken : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Trojan Chicken Replica");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<TrojanChickenTile>(), 0);
			Item.width = 38;
			Item.height = 26;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 50, 0);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.Wood, 30)
				.AddIngredient(ModContent.ItemType<ChickenEgg>(), 10)
				.AddTile(TileID.Sawmill)
				.AddCondition(Recipe.Condition.InGraveyardBiome)
				.Register();
		}
	}
}