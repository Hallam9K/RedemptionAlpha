using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class SlayerShipPanel : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slayer's Ship Panel");
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

		public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<SlayerShipPanelTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
		}
    }
}
