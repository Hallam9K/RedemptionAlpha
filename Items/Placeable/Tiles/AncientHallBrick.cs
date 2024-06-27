using Redemption.Globals;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientHallBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientHallBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
    public class AncientHallBrickSafe : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/AncientHallBrick";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientHallBrickTileSafe>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RedeRecipe.GathicStoneRecipeGroup, 2)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
            CreateRecipe()
                .AddIngredient<AncientHallPillarWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}