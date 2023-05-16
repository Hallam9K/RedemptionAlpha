using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodCandelabra : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodCandelabraTile>());
            Item.width = 26;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = 300;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(5)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}