using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class NirinCogMedium : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NirinCogMediumTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 30, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NiricPipe>(6)
                .AddIngredient(ItemID.Cog, 4)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}