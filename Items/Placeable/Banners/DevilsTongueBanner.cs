using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Banners;

namespace Redemption.Items.Placeable.Banners
{
    public class DevilsTongueBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Devil's Tongue Banner");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<DevilsTongueBannerTile>(), 0);
            Item.width = 12;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 10, 0);
        }
    }
}