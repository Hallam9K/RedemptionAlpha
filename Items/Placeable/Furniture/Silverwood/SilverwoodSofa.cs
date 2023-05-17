using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodSofa : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodSofaTile>());
            Item.width = 40;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 60;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(5)
                .AddIngredient<AncientAlloy>(2)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}