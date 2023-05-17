using Terraria.ModLoader;
using Terraria;
using Redemption.Tiles.Furniture.PetrifiedWood;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodPlatform : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 200;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodPlatformTile>(), 0);
			Item.width = 24;
			Item.height = 14;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void AddRecipes()
		{
			CreateRecipe(2)
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>())
				.Register();
        }
	}
}
