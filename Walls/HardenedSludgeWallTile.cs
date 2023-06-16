using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class HardenedSludgeWallTile : ModWall
	{
		public override void SetStaticDefaults()
        {
            RedeTileHelper.CannotTeleportInFront[Type] = true;
            Main.wallHouse[Type] = false;
			AddMapEntry(new Color(14, 49, 15));
		}
		public override bool CanExplode(int i, int j) => false;
		public override void KillWall(int i, int j, ref bool fail) => fail = true;
	}
}