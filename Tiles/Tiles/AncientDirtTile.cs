using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class AncientDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientHallBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[Type][TileID.Mudstone] = true;
            Main.tileMerge[TileID.Mudstone][Type] = true;
            Main.tileBlockLight[Type] = true;
            ItemDrop = ModContent.ItemType<AncientDirt>();
			AddMapEntry(new Color(115, 88, 69));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}