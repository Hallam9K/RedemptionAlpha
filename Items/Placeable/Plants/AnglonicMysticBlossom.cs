using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.Tiles.Plants;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Plants
{
    public class AnglonicMysticBlossom : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'An exceptionally rare flower with an eternal lifetime.'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AnglonicMysticBlossomTile>(), 0);
            Item.width = 34;
            Item.height = 30;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 7, 5, 0);
            Item.rare = ItemRarityID.Pink;
        }
    }
}
