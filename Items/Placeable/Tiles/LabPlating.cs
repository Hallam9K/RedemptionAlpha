using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class LabPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Panel (Unsafe)");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabTileUnsafe>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(0, 0, 2, 0);
        }
    }
}