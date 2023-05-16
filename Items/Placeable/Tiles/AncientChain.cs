using Redemption.Globals;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientChain : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientChainTile>());
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient<AncientAlloy>()
                .AddTile(TileID.Sawmill)
                .Register();
            CreateRecipe()
                .AddIngredient<AncientChainSolid>()
                .Register();
        }
    }
    public class AncientChainSolid : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/AncientChain";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Chain (Solid)");
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientChainSolidTile>());
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientChain>()
                .Register();
        }
    }
}
