using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodClockTile>());
            Item.width = 18;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientAlloy>(3)
                .AddIngredient(ItemID.Glass, 6)
                .AddIngredient<Tiles.Silverwood>(10)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}