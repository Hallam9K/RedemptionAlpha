using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class PetrifiedWoodWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<PetrifiedWoodWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<PetrifiedWood>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}