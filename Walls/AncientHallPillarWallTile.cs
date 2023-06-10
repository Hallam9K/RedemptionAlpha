using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class AncientHallPillarWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = ModContent.DustType<SlateDust>();
            AddMapEntry(new Color(49, 43, 39));
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}