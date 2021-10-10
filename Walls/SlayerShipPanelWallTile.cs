using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class SlayerShipPanelWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            ItemDrop = ModContent.ItemType<SlayerShipPanelWall>();
            AddMapEntry(new Color(35, 34, 40));
		}
        public override bool CanExplode(int i, int j) => false;
    }
    public class SlayerShipPanelWall : PlaceholderTile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slayer's Ship Wall Panel");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SlayerShipPanelWallTile>();
        }
    }
}