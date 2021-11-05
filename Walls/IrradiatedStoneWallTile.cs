using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedStoneWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = ModContent.DustType<IrradiatedStoneDust>();
			ItemDrop = ModContent.ItemType<IrradiatedStoneWall>();
			AddMapEntry(new Color(20, 20, 20));
		}
	}
}