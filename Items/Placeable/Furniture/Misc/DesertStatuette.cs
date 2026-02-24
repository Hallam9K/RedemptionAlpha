using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class DesertStatuette : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<DesertStatuetteTile>(), 0);
            Item.width = 24;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 80);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sandstone, 6)
                .AddIngredient(ItemID.FossilOre, 2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}