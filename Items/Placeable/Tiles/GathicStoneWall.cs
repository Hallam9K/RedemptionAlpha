using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicStoneWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<GathicStoneWallTileSafe>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<GathicStone>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}