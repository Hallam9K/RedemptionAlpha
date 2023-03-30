using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodBed : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodBedTile>(), 0);
			Item.width = 34;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 2000;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 15)
				.AddIngredient(ItemID.Silk, 5)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}