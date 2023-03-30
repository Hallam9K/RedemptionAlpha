using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicGladestone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GathicGladestoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicGladestoneWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe(10)
                .AddIngredient(ModContent.ItemType<GathicStone>(), 10)
                .AddIngredient(ItemID.GrassSeeds)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
