using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedCrimstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<IrradiatedCrimstoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 1, 0);
        }
    }
}
