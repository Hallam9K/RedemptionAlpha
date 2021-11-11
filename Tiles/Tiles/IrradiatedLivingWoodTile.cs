using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedLivingWoodTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCorruptGrass>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedCorruptGrass>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCrimsonGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedCrimsonGrassTile>()][Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
			AddMapEntry(new Color(90, 90, 90));
            MineResist = 2.5f;
            ItemDrop = ModContent.ItemType<PetrifiedWood>();
		}
    }
}

