using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;

namespace Redemption.Walls
{
    public class PetrifiedWoodFenceTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
			ItemDrop = ModContent.ItemType<PetrifiedWoodFence>();
			AddMapEntry(new Color(48, 44, 42));
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}