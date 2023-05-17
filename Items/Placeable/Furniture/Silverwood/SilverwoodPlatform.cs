using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodPlatform : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodPlatformTile>());
            Item.width = 24;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<Tiles.Silverwood>()
                .Register();
        }
    }
}