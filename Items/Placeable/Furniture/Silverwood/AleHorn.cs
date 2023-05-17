using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class AleHorn : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Ale Horn");
            Tooltip.SetDefault("Makes you tipsy");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AleHornTile>(), 0);
            Item.width = 36;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 250;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(3)
                .AddIngredient<AncientAlloy>()
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}