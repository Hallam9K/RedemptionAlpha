using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class JungleStatuette : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<JungleStatuetteTile>(), 0);
            Item.width = 28;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 80);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RichMahogany, 6)
                .AddIngredient(ItemID.MudBlock, 2)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}