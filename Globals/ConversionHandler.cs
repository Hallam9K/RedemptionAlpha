using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Tiles.Ores;
using Redemption.Tiles.Tiles;
using Redemption.Walls;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption
{
    class ConversionHandler
    {
        public static void ConvertWasteland(Vector2 Center, int radius = 287, bool crater = true)
        {
            if (crater)
            {
                Tile tile = Framing.GetTileSafely((int)Center.X / 16, BaseWorldGen.GetFirstTileFloor((int)Center.X / 16, (int)Center.Y / 16));
                int tileType = tile.type;

                Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Crater", AssetRequestMode.ImmediateLoad).Value;
                Texture2D texSnow = ModContent.Request<Texture2D>("Redemption/WorldGeneration/CraterSnow", AssetRequestMode.ImmediateLoad).Value;
                Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/CraterWalls", AssetRequestMode.ImmediateLoad).Value;

                if (tile.IsActive && (TileID.Sets.Conversion.Ice[tileType] || tileType == TileID.SnowBlock))
                {
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(0, 255, 255)] = ModContent.TileType<PlutoniumTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };
                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Main.QueueMainThreadAction(() =>
                    {
                        TexGen gen = BaseWorldGenTex.GetTexGenerator(texSnow, colorToTile, texWall, colorToWall);
                        gen.Generate((int)(Center.X / 16) - 50, (int)(Center.Y / 16) - 46, true, true);
                    });
                }
                else if (tile.IsActive && (TileID.Sets.Conversion.Sand[tileType] || TileID.Sets.Conversion.Sandstone[tileType] || TileID.Sets.Conversion.HardenedSand[tileType]))
                {
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(0, 255, 255)] = ModContent.TileType<PlutoniumTile>(),
                        [new Color(0, 0, 255)] = TileID.Sandstone,
                        [new Color(255, 0, 0)] = TileID.Glass,
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };
                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };
                    Main.QueueMainThreadAction(() =>
                    {
                        TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall);
                        gen.Generate((int)(Center.X / 16) - 50, (int)(Center.Y / 16) - 46, true, true);
                    });
                }
                else
                {
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(0, 255, 255)] = ModContent.TileType<PlutoniumTile>(),
                        [new Color(0, 0, 255)] = TileID.Dirt,
                        [new Color(255, 0, 0)] = TileID.Ash,
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };
                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };
                    Main.QueueMainThreadAction(() =>
                    {
                        TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall);
                        gen.Generate((int)(Center.X / 16) - 50, (int)(Center.Y / 16) - 46, true, true);
                    });
                }
            }

            BaseWorldGen.ReplaceTiles(Center, radius,
                new int[]
                {
                    // Stone
                    TileID.Stone,
                    TileID.BlueMoss,
                    TileID.BrownMoss,
                    TileID.GreenMoss,
                    TileID.LavaMoss,
                    TileID.PurpleMoss,
                    TileID.RedMoss,
                    TileID.RedMoss,
                    TileID.RedMoss,
                    TileID.RedMoss,
                    //
                    TileID.Ebonstone,
                    TileID.Crimstone,
                    TileID.Pearlstone,
                    // Grass
                    TileID.Grass,
                    TileID.CorruptGrass,
                    TileID.CrimsonGrass,
                    TileID.HallowedGrass,
                    // Ice
                    TileID.IceBlock,
                    TileID.CorruptIce,
                    TileID.FleshIce,
                    TileID.HallowedIce,
                    // Sand
                    TileID.Sand,
                    TileID.Ebonsand,
                    TileID.Crimsand,
                    TileID.Pearlsand,
                    // Hardened Sand
                    TileID.HardenedSand,
                    TileID.CorruptHardenedSand,
                    TileID.CrimsonHardenedSand,
                    TileID.HallowHardenedSand, 
                    // Sandstone
                    TileID.Sandstone,
                    TileID.CorruptSandstone,
                    TileID.CrimsonSandstone,
                    TileID.HallowSandstone,
                    //
                    TileID.LivingWood,
                    // Gems
                    TileID.Amethyst,
                    TileID.Topaz,
                    TileID.Sapphire,
                    TileID.Emerald,
                    TileID.Ruby,
                    TileID.Diamond
                },
                new int[]
                {
                    // Stone
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    //
                    ModContent.TileType<IrradiatedEbonstoneTile>(),
                    ModContent.TileType<IrradiatedCrimstoneTile>(),
                    ModContent.TileType<IrradiatedStoneTile>(),
                    // Grass
                    ModContent.TileType<DeadGrassTile>(),
                    ModContent.TileType<DeadGrassTileCorruption>(),
                    ModContent.TileType<DeadGrassTileCrimson>(),
                    ModContent.TileType<DeadGrassTile>(),
                    // Ice
                    ModContent.TileType<RadioactiveIceTile>(),
                    ModContent.TileType<RadioactiveIceTile>(),
                    ModContent.TileType<RadioactiveIceTile>(),
                    ModContent.TileType<RadioactiveIceTile>(),
                    // Sand
                    ModContent.TileType<RadioactiveSandTile>(),
                    ModContent.TileType<RadioactiveSandTile>(),
                    ModContent.TileType<RadioactiveSandTile>(),
                    ModContent.TileType<RadioactiveSandTile>(),
                    // Hardened Sand
                    ModContent.TileType<HardenedRadioactiveSandTile>(),
                    ModContent.TileType<HardenedRadioactiveSandTile>(),
                    ModContent.TileType<HardenedRadioactiveSandTile>(),
                    ModContent.TileType<HardenedRadioactiveSandTile>(),
                    // Sandstone
                    ModContent.TileType<RadioactiveSandstoneTile>(),
                    ModContent.TileType<RadioactiveSandstoneTile>(),
                    ModContent.TileType<RadioactiveSandstoneTile>(),
                    ModContent.TileType<RadioactiveSandstoneTile>(),
                    //
                    ModContent.TileType<LivingDeadWoodTile>(),
                    // Gems
                    ModContent.TileType<StarliteGemOreTile>(),
                    ModContent.TileType<StarliteGemOreTile>(),
                    ModContent.TileType<StarliteGemOreTile>(),
                    ModContent.TileType<StarliteGemOreTile>(),
                    ModContent.TileType<StarliteGemOreTile>(),
                    ModContent.TileType<StarliteGemOreTile>()
                }, true);
            BaseWorldGen.ReplaceWalls(Center, 287,
                new int[]
                {
                    WallID.GrassUnsafe,
                    WallID.CorruptGrassUnsafe,
                    WallID.CrimsonGrassUnsafe,
                    WallID.HallowedGrassUnsafe,
                    WallID.Stone,
                    WallID.EbonstoneUnsafe,
                    WallID.CrimstoneUnsafe,
                    WallID.PearlstoneBrickUnsafe,
                    //
                    WallID.HardenedSand,
                    WallID.CorruptHardenedSand,
                    WallID.CrimsonHardenedSand,
                    WallID.HallowHardenedSand,
                    //
                    WallID.Sandstone,
                    WallID.CorruptSandstone,
                    WallID.CrimsonSandstone,
                    WallID.HallowSandstone,
                    //
                    WallID.IceUnsafe,
                    WallID.LivingLeaf,
                    WallID.LivingWood
                },
                new int[]
                {
                    ModContent.WallType<DeadGrassWallTile>(),
                    ModContent.WallType<DeadGrassWallTile>(),
                    ModContent.WallType<DeadGrassWallTile>(),
                    ModContent.WallType<DeadGrassWallTile>(),
                    ModContent.WallType<IrradiatedStoneWallTile>(),
                    ModContent.WallType<IrradiatedStoneWallTile>(),
                    ModContent.WallType<IrradiatedStoneWallTile>(),
                    ModContent.WallType<IrradiatedStoneWallTile>(),
                    //
                    ModContent.WallType<HardenedRadioactiveSandWallTile>(),
                    ModContent.WallType<HardenedRadioactiveSandWallTile>(),
                    ModContent.WallType<HardenedRadioactiveSandWallTile>(),
                    ModContent.WallType<HardenedRadioactiveSandWallTile>(),
                    //
                    ModContent.WallType<RadioactiveSandstoneWallTile>(),
                    ModContent.WallType<RadioactiveSandstoneWallTile>(),
                    ModContent.WallType<RadioactiveSandstoneWallTile>(),
                    ModContent.WallType<RadioactiveSandstoneWallTile>(),
                    //
                    ModContent.WallType<RadioactiveIceWallTile>(),
                    ModContent.WallType<LivingDeadLeavesWallTile>(),
                    ModContent.WallType<LivingDeadWoodWallTile>()
                }, true);
            int radiusLeft = (int)(Center.X / 16f - radius);
            int radiusRight = (int)(Center.X / 16f + radius);
            int radiusUp = (int)(Center.Y / 16f - radius);
            int radiusDown = (int)(Center.Y / 16f + radius);
            if (radiusLeft < 0) { radiusLeft = 0; }
            if (radiusRight > Main.maxTilesX) { radiusRight = Main.maxTilesX; }
            if (radiusUp < 0) { radiusUp = 0; }
            if (radiusDown > Main.maxTilesY) { radiusDown = Main.maxTilesY; }

            float distRad = radius * 16f;
            for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
            {
                for (int y1 = radiusUp; y1 <= radiusDown; y1++)
                {
                    double dist = Vector2.Distance(new Vector2(x1 * 16f + 8f, y1 * 16f + 8f), Center);
                    if (!WorldGen.InWorld(x1, y1, 0)) continue;
                    if (dist < distRad && Main.tile[x1, y1] != null && Main.tile[x1, y1].IsActive && Main.tile[x1, y1].type == TileID.LeafBlock)
                    {
                        WorldGen.KillTile(x1, y1, false, false, true);
                    }
                }
            }
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, (int)(Center.X / 16f), (int)(Center.Y / 16f), (radius * 2) + 2);

        }
    }
}