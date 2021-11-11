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
            WastelandTileCount = tileCounts[ModContent.TileType<IrradiatedStoneTile>()] + tileCounts[ModContent.TileType<IrradiatedGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedSandTile>()] + tileCounts[ModContent.TileType<IrradiatedSandstoneTile>()] + tileCounts[ModContent.TileType<IrradiatedIceTile>()];
        }
    }
}