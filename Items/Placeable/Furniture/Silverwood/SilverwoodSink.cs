using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodSink : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodSinkTile>());
            Item.width = 34;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = 60;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(6)
                .AddIngredient<AncientAlloy>(2)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}