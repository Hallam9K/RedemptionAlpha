using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedLivingWoodWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<IrradiatedLivingWoodWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<PetrifiedWood>())
				.AddTile(TileID.LivingLoom)
				.Register();
		}
	}
}