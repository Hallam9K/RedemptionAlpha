using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodBathtub : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodBathtubTile>(), 0);
			Item.width = 34;
			Item.height = 22;
			Item.maxStack = 9999;
			Item.value = 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 14)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}