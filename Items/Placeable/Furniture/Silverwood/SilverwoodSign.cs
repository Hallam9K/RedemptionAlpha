using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodSign : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Silverwood Danger Sign");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodSignTile>());
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(8)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}