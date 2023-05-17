using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodBathtub : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodBathtubTile>());
            Item.width = 30;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 60;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(4)
                .AddIngredient<AncientAlloy>(10)
                .AddIngredient<EvergoldBar>(1)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}