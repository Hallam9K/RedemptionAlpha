using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodWorkBench : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodWorkBenchTile>());
            Item.width = 32;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 150;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(10)
                .Register();
        }
    }
}