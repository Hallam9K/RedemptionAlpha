using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class GloomMushroomBig : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 33;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<GloomShroomFoliage2>());
            Item.width = 22;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GloomMushroom>(3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
