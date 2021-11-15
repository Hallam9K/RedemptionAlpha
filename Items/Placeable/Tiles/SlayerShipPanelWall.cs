using Redemption.Tiles.Tiles;
using Redemption.Walls;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class SlayerShipPanelWall : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slayer's Ship Panel Wall");
        }

		public override void SetDefaults()
		{
            Item.DefaultToPlacableWall((ushort)ModContent.WallType<SlayerShipPanelWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999;
		}
    }
}
