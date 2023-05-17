using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Terraria;
using Terraria.GameContent;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedLivingWoodWall : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.LivingWoodWall, 1);
            Item.ResearchUnlockCount = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableWall((ushort)ModContent.WallType<IrradiatedLivingWoodWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
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