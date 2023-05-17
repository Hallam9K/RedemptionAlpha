using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Walls
{
    public class PetrifiedWoodWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
			AddMapEntry(new Color(48, 44, 42));
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}