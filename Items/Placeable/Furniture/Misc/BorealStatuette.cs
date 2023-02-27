using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Redemption.Tiles.Furniture.Misc;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class BorealStatuette : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BorealStatuetteTile>(), 0);
            Item.width = 22;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 0, 80);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BorealWood, 6)
                .AddIngredient(ItemID.IceBlock, 2)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}