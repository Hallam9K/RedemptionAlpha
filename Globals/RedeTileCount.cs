using System;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeTileCount : ModSystem
    {
        public int LabTileCount;
        public int SlayerShipTileCount;
        public int WastelandTileCount;
        public int WastelandSnowTileCount;
        public int WastelandDesertTileCount;
        public int WastelandCorruptTileCount;
        public int WastelandCrimsonTileCount;

        // TODO: Find how to make tiles count to vanilla biomes in tmodloader source
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) // TODO: look through tileid.sets for the wasteland tiles
        {
            LabTileCount = tileCounts[ModContent.TileType<LabTileUnsafe>()];
            SlayerShipTileCount = tileCounts[ModContent.TileType<SlayerShipPanelTile>()];
            WastelandTileCount = tileCounts[ModContent.TileType<IrradiatedStoneTile>()] + tileCounts[ModContent.TileType<IrradiatedGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedSandTile>()] + tileCounts[ModContent.TileType<IrradiatedSandstoneTile>()] + tileCounts[ModContent.TileType<IrradiatedIceTile>()] + tileCounts[ModContent.TileType<IrradiatedCorruptGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedCrimsonGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedCrimstoneTile>()] + tileCounts[ModContent.TileType<IrradiatedEbonstoneTile>()] + tileCounts[ModContent.TileType<IrradiatedSnowTile>()];
            WastelandSnowTileCount = tileCounts[ModContent.TileType<IrradiatedIceTile>()] + tileCounts[ModContent.TileType<IrradiatedSnowTile>()];
            WastelandDesertTileCount = tileCounts[ModContent.TileType<IrradiatedSandTile>()] + tileCounts[ModContent.TileType<IrradiatedSandstoneTile>()];
            WastelandCorruptTileCount = tileCounts[ModContent.TileType<IrradiatedCorruptGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedEbonstoneTile>()];
            WastelandCrimsonTileCount = tileCounts[ModContent.TileType<IrradiatedCrimsonGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedCrimstoneTile>()];
        }
    }
}