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
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BorealStatuetteTile>(), 0);
            Item.width = 22;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
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