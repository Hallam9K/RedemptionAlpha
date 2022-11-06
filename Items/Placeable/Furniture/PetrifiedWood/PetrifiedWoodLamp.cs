using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodLamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodLampTile>());
			Item.width = 10;
			Item.height = 34;
			Item.maxStack = 9999;
			Item.value = 100;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Torch)
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 3)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}