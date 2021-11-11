using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedSandstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<IrradiatedSandstoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }
    }
}
