using System;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeTileCount : ModSystem
    {
        public int LabTileCount;
        public int SlayerShipTileCount;
        public int WastelandTileCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            LabTileCount = tileCounts[ModContent.TileType<LabTileUnsafe>()];
            SlayerShipTileCount = tileCounts[ModContent.TileType<SlayerShipPanelTile>()];
            WastelandTileCount = tileCounts[ModContent.TileType<IrradiatedStoneTile>()] + tileCounts[ModContent.TileType<DeadGrassTile>()] + tileCounts[ModContent.TileType<RadioactiveSandTile>()] + tileCounts[ModContent.TileType<RadioactiveSandstoneTile>()] + tileCounts[ModContent.TileType<RadioactiveIceTile>()];
        }
    }
}