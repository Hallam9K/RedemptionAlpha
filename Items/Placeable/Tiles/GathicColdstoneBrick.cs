using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicColdstoneBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GathicColdstoneBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicColdstone>(), 2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
