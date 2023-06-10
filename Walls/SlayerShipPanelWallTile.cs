using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class SlayerShipPanelWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            RedeTileHelper.CannotTeleportInFront[Type] = true;
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(35, 34, 40));
        }
        public override bool CanExplode(int i, int j) => false;
    }
}