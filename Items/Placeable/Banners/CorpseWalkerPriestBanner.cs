using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Banners;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Banners
{
    public class CorpseWalkerPriestBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpse-Walker Priest Banner");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CorpseWalkerPriestBannerTile>(), 0);
            Item.width = 12;
            Item.height = 28;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 10, 0);
        }
    }
}