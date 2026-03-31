using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<GathicStoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddCustomShimmerResult(ItemType<EnergizedGathicStone>())
                .AddDecraftCondition(RedeConditions.DownedADD)
                .AddIngredient(ItemType<GathicStoneWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe(10)
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddIngredient<GraveSteelShards>()
                .AddTile(TileID.Furnaces)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemType<GathicGladestone>())
                .Register();
        }
    }
}
