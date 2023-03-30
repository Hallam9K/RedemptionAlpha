using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Walls;
using Terraria.GameContent;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedStoneWall : ModItem
    {
		public override void SetStaticDefaults()
        {
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.StoneWall, 1);
            Item.ResearchUnlockCount = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableWall((ushort)ModContent.WallType<IrradiatedStoneWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
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