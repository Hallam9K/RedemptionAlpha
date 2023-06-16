using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class BlackHardenedSludgeWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            RedeTileHelper.CannotTeleportInFront[Type] = true;
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(12, 16, 19));
        }
        public override bool CanExplode(int i, int j) => false;
        public override void KillWall(int i, int j, ref bool fail) => fail = true;
    }
}