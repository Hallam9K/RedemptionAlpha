using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodWorkBench : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodWorkBenchTile>(), 0);
            Item.width = 32;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 10)
                .Register();
        }
    }
}