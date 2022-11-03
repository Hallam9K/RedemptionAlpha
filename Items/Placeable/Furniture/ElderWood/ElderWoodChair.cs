using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodChair : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodChairTile>(), 0);
			Item.width = 16;
			Item.height = 30;
			Item.maxStack = 9999;
			Item.value = 150;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 4)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}