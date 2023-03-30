using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicStoneBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GathicStoneBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicStoneBrickWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicStone>(), 2)
                .AddTile(TileID.Furnaces)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AncientHallPillarWall>(), 4)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
