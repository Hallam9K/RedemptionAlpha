using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class LabPlatingWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			AddMapEntry(new Color(100, 99, 107));
		}
        public override bool CanExplode(int i, int j) => false;
    }
}