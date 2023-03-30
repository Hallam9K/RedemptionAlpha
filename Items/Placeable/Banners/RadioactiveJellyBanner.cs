using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Banners;

namespace Redemption.Items.Placeable.Banners
{
    public class RadioactiveJellyBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Radioactive Physalia Banner");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadioactiveJellyBannerTile>(), 0);
            Item.width = 12;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 10, 0);
        }
    }
}