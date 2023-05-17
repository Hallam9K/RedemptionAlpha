using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodTable : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodTableTile>());
            Item.width = 38;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(8)
                .AddIngredient<AncientAlloy>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}