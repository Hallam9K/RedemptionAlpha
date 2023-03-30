using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodWorkBench : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodWorkbenchTile>(), 0);
			Item.width = 32;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 150;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 10)
				.Register();
        }
	}
}