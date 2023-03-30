using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodChair : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodChairTile>(), 0);
			Item.width = 16;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 150;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 4)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}