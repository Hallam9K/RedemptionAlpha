using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Walls
{
    public class ElderWoodWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
            DustType = DustID.t_BorealWood;
			AddMapEntry(new Color(55, 44, 39));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}