using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class DangerTapeWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            RedeTileHelper.CannotTeleportInFront[Type] = true;
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(43, 43, 44));
        }
        public override bool CanExplode(int i, int j) => false;
        public override void KillWall(int i, int j, ref bool fail) => fail = true;
    }
    public class DangerTapeWall2Tile : ModWall
    {
        public override string Texture => "Redemption/Walls/DangerTapeWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(43, 43, 44));
        }
    }
}
