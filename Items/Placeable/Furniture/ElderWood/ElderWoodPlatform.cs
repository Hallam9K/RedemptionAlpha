using Terraria.ModLoader;
using Terraria;
using Redemption.Tiles.Furniture.ElderWood;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodPlatform : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 200;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodPlatformTile>(), 0);
			Item.width = 24;
			Item.height = 14;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void AddRecipes()
		{
			CreateRecipe(2)
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>())
				.Register();
        }
	}
}
