using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class NozaCageSmall : ModItem
	{
        public override string Texture => "Redemption/Items/Placeable/Furniture/Misc/NozaCage";
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Small Bastion Cage");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NozaCageSmallTile>(), 0);
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 25, 0);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.IronBar, 6)
				.AddIngredient(ItemID.Obsidian, 4)
				.AddTile(TileID.HeavyWorkBench)
				.Register();
		}
	}
	public class NozaCage : ModItem
	{
		public override string Texture => "Redemption/Items/Placeable/Furniture/Misc/NozaCage";
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bastion Cage");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NozaCageTile>(), 0);
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 50, 0);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.IronBar, 9)
				.AddIngredient(ItemID.Obsidian, 6)
				.AddTile(TileID.HeavyWorkBench)
				.Register();
		}
	}
	public class NozaCageLarge : ModItem
	{
		public override string Texture => "Redemption/Items/Placeable/Furniture/Misc/NozaCage";
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Large Bastion Cage");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NozaCageLargeTile>(), 0);
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 75, 0);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.IronBar, 14)
				.AddIngredient(ItemID.Obsidian, 8)
				.AddTile(TileID.HeavyWorkBench)
				.Register();
		}
	}
}
