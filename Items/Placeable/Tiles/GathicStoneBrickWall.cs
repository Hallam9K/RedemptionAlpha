using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicStoneBrickWall : ModItem
    {
        public override void SetStaticDefaults()
		{
			SacrificeTotal = 400;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<GathicStoneBrickWallTileSafe>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<GathicStoneBrick>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}