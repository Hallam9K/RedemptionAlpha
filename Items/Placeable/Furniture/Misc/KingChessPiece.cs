using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Redemption.Tiles.Furniture.Misc;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class KingChessPiece : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Marble King Chess Piece");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<KingChessPieceTile>(), 0);
            Item.width = 20;
            Item.height = 46;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 8, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Marble, 16)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}