using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class EvergoldChain : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evergold Chain");
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EvergoldChainTile>());
            Item.width = 24;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient<EvergoldBar>()
                .AddTile(TileID.Sawmill)
                .Register();
            CreateRecipe()
                .AddIngredient<EvergoldChainSolid>()
                .Register();
        }
    }
    public class EvergoldChainSolid : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/EvergoldChain";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evergold Chain (Solid)");
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EvergoldChainSolidTile>());
            Item.width = 24;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EvergoldChain>()
                .Register();
        }
    }
}
