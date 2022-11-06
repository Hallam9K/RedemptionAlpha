using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodFence : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 400;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<PetrifiedWoodFenceTile>());
			Item.width = 32;
			Item.height = 28;
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}