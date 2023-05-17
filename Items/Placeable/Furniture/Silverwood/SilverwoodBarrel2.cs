using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodBarrel2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Barrel");
            Tooltip.SetDefault("'Looks slightly broken'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodBarrelTile>(), 1);
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(9)
                .AddIngredient<AncientAlloy>()
                .AddTile(TileID.Sawmill)
                .Register();
            CreateRecipe()
                .AddIngredient<SilverwoodBarrel>()
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}
