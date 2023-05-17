using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodLongtable_End : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Longtable (End)");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodLongtableTile_End>());
            Item.width = 18;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.value = 500;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class SilverwoodLongtable_Mid : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Longtable (Middle)");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodLongtableTile_Mid>());
            Item.width = 32;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.value = 500;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class SilverwoodLongtable_Mid2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Longtable (Middle with Chair)");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodLongtableTile_Mid2>());
            Item.width = 32;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.value = 500;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(2)
                .AddIngredient<SilverwoodChair>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}