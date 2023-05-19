using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Tiles;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Kingdom;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Kingdom
{
    public class SmallKeepBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SmallKeepBannerTile>(), 0);
            Item.width = 12;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
            Item.value = Item.sellPrice(0, 0, 10, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<EvergoldBar>()
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}