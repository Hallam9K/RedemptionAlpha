using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientDirtWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableWall((ushort)ModContent.WallType<AncientDirtWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<AncientDirt>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}