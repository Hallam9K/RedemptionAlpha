using Redemption.Items.Placeable.Furniture.Silverwood;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class Silverwood : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodTile>());
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SilverwoodPlatform>(), 2)
                .Register();
        }
    }
}
