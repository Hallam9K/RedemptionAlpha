using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class ClayStatuette : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<ClayStatuetteTile>(), 0);
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 20);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ClayBlock, 8)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}