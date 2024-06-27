using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class NirinCogBig : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NirinCogBigTile>(), 0);
            Item.width = 48;
            Item.height = 48;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 60, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NiricPipe>(10)
                .AddIngredient(ItemID.Cog, 8)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}