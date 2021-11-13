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
                        [new Color(0, 0, 255)] = ModContent.TileType<IrradiatedSandstoneTile>(),
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
                        [new Color(0, 0, 255)] = ModContent.TileType<IrradiatedDirtTile>(),
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
                    if (!WorldGen.InWorld(x1, y1, 0))
                        continue;

                    Tile tile = Main.tile[x1, y1];
                    if (dist < distRad && tile != null)
                    {
                        if (tile.IsActive && tile.type == TileID.LeafBlock)
                            WorldGen.KillTile(x1, y1, false, false, true);
                        if (tile.wall == WallID.LivingLeaf)
                            WorldGen.KillWall(x1, y1, false);

                        #region Conversion
                        if ((TileID.Sets.Conversion.Stone[tile.type] && tile.type != TileID.Ebonstone && tile.type != TileID.Crimstone) ||
                            TileID.Sets.Conversion.Moss[tile.type])
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedStoneTile>());
                        else if (tile.type == TileID.Ebonstone)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedEbonstoneTile>());
                        else if (tile.type == TileID.Crimstone)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedCrimstoneTile>());
                        else if (TileID.Sets.Conversion.Grass[tile.type] && tile.type != TileID.CorruptGrass && tile.type != TileID.CrimsonGrass)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedGrassTile>());
                        else if (tile.type == TileID.CorruptGrass)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedCorruptGrassTile>());
                        else if (tile.type == TileID.CrimsonGrass)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedCrimsonGrassTile>());
                        else if (tile.type == TileID.Dirt)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedDirtTile>());
                        else if (TileID.Sets.Conversion.Ice[tile.type])
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedIceTile>());
                        else if (tile.type == TileID.SnowBlock)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedSnowTile>());
                        else if (TileID.Sets.Conversion.Sand[tile.type])
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedSandTile>());
                        else if (TileID.Sets.Conversion.HardenedSand[tile.type])
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedHardenedSandTile>());
                        else if (TileID.Sets.Conversion.Sandstone[tile.type])
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedSandstoneTile>());
                        else if (tile.type == TileID.LivingWood)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedLivingWoodTile>());
                        else if (tile.type == TileID.WoodBlock)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<PetrifiedWoodTile>());
                        else if (tile.type == TileID.Amethyst || tile.type == TileID.Topaz || tile.type == TileID.Sapphire || tile.type == TileID.Emerald ||
                            tile.type == TileID.Ruby || tile.type == TileID.Diamond)
                            ConvertTile(x1, y1, (ushort)ModContent.TileType<StarliteGemOreTile>());

                        if (WallID.Sets.Conversion.Stone[tile.wall] && tile.wall != WallID.EbonstoneUnsafe && tile.wall != WallID.CrimstoneUnsafe)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedStoneWallTile>());
                        else if (tile.wall == WallID.EbonstoneUnsafe)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedEbonstoneWallTile>());
                        else if (tile.wall == WallID.CrimstoneUnsafe)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedCrimstoneWallTile>());
                        else if (WallID.Sets.Conversion.HardenedSand[tile.wall])
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedHardenedSandWallTile>());
                        else if (WallID.Sets.Conversion.Sandstone[tile.wall])
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedSandstoneWallTile>());
                        else if (tile.wall == WallID.IceUnsafe)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedIceWallTile>());
                        else if (tile.wall == WallID.SnowWallUnsafe)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedSnowWallTile>());
                        else if (tile.wall == WallID.LivingWood)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedLivingWoodWallTile>());
                        else if (tile.wall == WallID.DirtUnsafe || tile.wall == WallID.DirtUnsafe1 || tile.wall == WallID.GrassUnsafe || tile.wall == WallID.FlowerUnsafe ||
                            tile.wall == WallID.CorruptGrassUnsafe || tile.wall == WallID.CrimsonGrassUnsafe)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedDirtWallTile>());
                        else if (tile.wall == WallID.MudUnsafe)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedMudWallTile>());
                        else if (tile.wall == WallID.Wood)
                            ConvertWall(x1, y1, (ushort)ModContent.WallType<PetrifiedWoodWallTile>());
                        #endregion
                    }
                }
            }
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, (int)(Center.X / 16f), (int)(Center.Y / 16f), (radius * 2) + 2);

        }
        public static void ConvertTile(int i, int j, ushort type)
        {
            if (Main.tile[i, j].type != type)
            {
                Main.tile[i, j].type = type;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }

        public static void ConvertWall(int i, int j, ushort type)
        {
            if (Main.tile[i, j].wall != type)
            {
                Main.tile[i, j].wall = type;
                WorldGen.SquareWallFrame(i, j);
                NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }
    }
}