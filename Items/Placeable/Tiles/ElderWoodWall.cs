using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Tiles
{
    public class ElderWoodWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 400;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<ElderWoodWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<ElderWood>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}