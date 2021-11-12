using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Redemption.Tiles.Trees;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCorruptGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedCorruptGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCrimsonGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedCrimsonGrassTile>()][Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;
            TileID.Sets.Conversion.Grass[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(117, 99, 86));
            MinPick = 10;
            MineResist = 1f;
            DustType = DustID.Ash;
            ItemDrop = ModContent.ItemType<IrradiatedDirt>();
        }
	}
}

