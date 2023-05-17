using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodCrateTNT : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Silverwood Crate");
            // Tooltip.SetDefault("Filled with dynamite");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodCratesTile>(), 2);
            Item.width = 28;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(11)
                .AddIngredient<AncientAlloy>()
                .AddIngredient(ItemID.Dynamite, 3)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}
