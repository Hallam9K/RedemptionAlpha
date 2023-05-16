using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodPlatform : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 200;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodPlatformTile>());
            Item.width = 24;
            Item.height = 16;
            Item.maxStack = 9999;
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