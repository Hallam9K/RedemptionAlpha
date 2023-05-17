using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodBarrel : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodBarrelTile>(), 0);
            Item.width = 24;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
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
                .AddIngredient<SilverwoodBarrel2>()
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}
