using System;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeTileCount : ModSystem
    {
        public int LabTileCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            LabTileCount = tileCounts[ModContent.TileType<LabTileUnsafe>()];
        }
    }
}