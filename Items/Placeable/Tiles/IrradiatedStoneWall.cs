using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedStoneWall : ModItem
    {
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<IrradiatedStoneWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
			Item.value = Item.buyPrice(0, 0, 1, 0);
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<IrradiatedStone>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}