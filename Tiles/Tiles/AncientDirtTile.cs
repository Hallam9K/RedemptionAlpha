using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Trees;
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
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientHallBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            Main.tileBlockLight[Type] = true;
            ItemDrop = ModContent.ItemType<AncientDirt>();
			AddMapEntry(new Color(115, 88, 69));
			SetModTree(new AncientTree());
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<AncientSapling>();
        }
    }
}