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

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            Main.SceneMetrics.SandTileCount += tileCounts[ModContent.TileType<IrradiatedSandTile>()] + tileCounts[ModContent.TileType<IrradiatedSandstoneTile>()];
            Main.SceneMetrics.SnowTileCount += tileCounts[ModContent.TileType<IrradiatedIceTile>()] + tileCounts[ModContent.TileType<IrradiatedSnowTile>()];
            Main.SceneMetrics.EvilTileCount += tileCounts[ModContent.TileType<IrradiatedCorruptGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedEbonstoneTile>()];
            Main.SceneMetrics.BloodTileCount += tileCounts[ModContent.TileType<IrradiatedCrimsonGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedCrimstoneTile>()];

            LabTileCount = tileCounts[ModContent.TileType<LabPlatingTileUnsafe>()];
            SlayerShipTileCount = tileCounts[ModContent.TileType<SlayerShipPanelTile>()];
            WastelandTileCount = tileCounts[ModContent.TileType<IrradiatedStoneTile>()] + tileCounts[ModContent.TileType<IrradiatedGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedSandTile>()] + tileCounts[ModContent.TileType<IrradiatedSandstoneTile>()] + tileCounts[ModContent.TileType<IrradiatedIceTile>()] + tileCounts[ModContent.TileType<IrradiatedCorruptGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedCrimsonGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedCrimstoneTile>()] + tileCounts[ModContent.TileType<IrradiatedEbonstoneTile>()] + tileCounts[ModContent.TileType<IrradiatedSnowTile>()];
            WastelandSnowTileCount = tileCounts[ModContent.TileType<IrradiatedIceTile>()] + tileCounts[ModContent.TileType<IrradiatedSnowTile>()];
            WastelandDesertTileCount = tileCounts[ModContent.TileType<IrradiatedSandTile>()] + tileCounts[ModContent.TileType<IrradiatedSandstoneTile>()];
            WastelandCorruptTileCount = tileCounts[ModContent.TileType<IrradiatedCorruptGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedEbonstoneTile>()];
            WastelandCrimsonTileCount = tileCounts[ModContent.TileType<IrradiatedCrimsonGrassTile>()] + tileCounts[ModContent.TileType<IrradiatedCrimstoneTile>()];
        }
    }
}