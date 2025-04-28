using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GloomMushroomBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<GloomMushroomTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<GloomMushroom>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
