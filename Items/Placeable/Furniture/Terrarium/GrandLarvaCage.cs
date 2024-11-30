using Redemption.Items.Critters;
using Redemption.Tiles.Furniture.Terrarium;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Terrarium
{
    public class GrandLarvaCage : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GrandLarvaCageTile>(), 0);
            Item.width = 34;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Terrarium)
                .AddIngredient<GrandLarvaBait>()
                .Register();
        }
    }
}