using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Tiles.Ores;
using Redemption.Tiles.Tiles;
using Redemption.Walls;
using Redemption.WorldGeneration;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption
{
    class ConversionHandler
    {
        public static bool GenningWasteland;
        public static Vector2 WastelandCenter;
        public static int Radius;
        public static void ConvertWasteland(Vector2 Center, int radius = 287, bool crater = true)
        {
            if (crater)
            {
                Tile tile = Framing.GetTileSafely((int)Center.X / 16, BaseWorldGen.GetFirstTileFloor((int)Center.X / 16, (int)Center.Y / 16));
                int tileType = tile.TileType;

                Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Crater", AssetRequestMode.ImmediateLoad).Value;
                Texture2D texSnow = ModContent.Request<Texture2D>("Redemption/WorldGeneration/CraterSnow", AssetRequestMode.ImmediateLoad).Value;
                Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/CraterWalls", AssetRequestMode.ImmediateLoad).Value;

                if (tile.HasTile && (TileID.Sets.Conversion.Ice[tileType] || tileType == TileID.SnowBlock))
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

                    GenUtils.InvokeOnMainThread(() =>
                    {
                        TexGen gen = BaseWorldGenTex.GetTexGenerator(texSnow, colorToTile, texWall, colorToWall);
                        gen.Generate((int)(Center.X / 16) - 50, (int)(Center.Y / 16) - 46, true, true);
                    });
                }
                else if (tile.HasTile && (TileID.Sets.Conversion.Sand[tileType] || TileID.Sets.Conversion.Sandstone[tileType] || TileID.Sets.Conversion.HardenedSand[tileType]))
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
                    GenUtils.InvokeOnMainThread(() =>
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
                    GenUtils.InvokeOnMainThread(() =>
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
            if (radiusLeft < 15) { radiusLeft = 15; }
            if (radiusRight > Main.maxTilesX - 15) { radiusRight = Main.maxTilesX - 15; }
            if (radiusUp < 15) { radiusUp = 15; }
            if (radiusDown > Main.maxTilesY - 15) { radiusDown = Main.maxTilesY - 15; }

            GenningWasteland = true;
            WastelandCenter = Center;
            Radius = radius;
            RadiusUp = radiusUp;
        }
        private static int RadiusUp;
        public static void GenWasteland(int radiusLeft, int radiusRight, int radiusDown, Vector2 Center, int radius)
        {
            if (!GenningWasteland || RadiusUp > radiusDown)
            {
                if (RadiusUp > radiusDown)
                {
                    GenningWasteland = false;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendTileSquare(-1, (int)(Center.X / 16f), (int)(Center.Y / 16f), (radius * 2) + 2);
                }
                return;
            }

            RadiusUp++;
            for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
            {
                double dist = Vector2.Distance(new Vector2(x1 * 16f + 8f, RadiusUp * 16f + 8f), Center);
                if (!WorldGen.InWorld(x1, RadiusUp, 0))
                    continue;

                Tile tile = Framing.GetTileSafely(x1, RadiusUp);
                if (dist < radius * 16f && tile != null)
                    WastelandTileConversion(tile, x1, RadiusUp);
            }
        }
        public static void WastelandTileConversion(Tile tile, int x1, int y1)
        {
            #region Conversion
            if (tile.HasTile && tile.TileType == TileID.LeafBlock)
                WorldGen.KillTile(x1, y1, false, false, true);
            if (tile.WallType == WallID.LivingLeaf)
                WorldGen.KillWall(x1, y1, false);

            if ((TileID.Sets.Conversion.Stone[tile.TileType] && tile.TileType != TileID.Ebonstone && tile.TileType != TileID.Crimstone) ||
                TileID.Sets.Conversion.Moss[tile.TileType])
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedStoneTile>());
            else if (tile.TileType is TileID.Ebonstone)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedEbonstoneTile>());
            else if (tile.TileType is TileID.Crimstone)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedCrimstoneTile>());
            else if (TileID.Sets.Conversion.Grass[tile.TileType] && tile.TileType != TileID.CorruptGrass && tile.TileType != TileID.CrimsonGrass)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedGrassTile>());
            else if (tile.TileType is TileID.CorruptGrass)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedCorruptGrassTile>());
            else if (tile.TileType is TileID.CrimsonGrass)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedCrimsonGrassTile>());
            else if (tile.TileType is TileID.Dirt)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedDirtTile>());
            else if (TileID.Sets.Conversion.Ice[tile.TileType])
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedIceTile>());
            else if (tile.TileType is TileID.SnowBlock)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedSnowTile>());
            else if (TileID.Sets.Conversion.Sand[tile.TileType])
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedSandTile>());
            else if (TileID.Sets.Conversion.HardenedSand[tile.TileType])
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedHardenedSandTile>());
            else if (TileID.Sets.Conversion.Sandstone[tile.TileType])
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedSandstoneTile>());
            else if (tile.TileType is TileID.LivingWood)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<IrradiatedLivingWoodTile>());
            else if (tile.TileType is TileID.WoodBlock)
                ConvertTile(x1, y1, (ushort)ModContent.TileType<PetrifiedWoodTile>());

            if (WallID.Sets.Conversion.Stone[tile.WallType] && tile.WallType != WallID.EbonstoneUnsafe && tile.WallType != WallID.CrimstoneUnsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedStoneWallTile>());
            else if (tile.WallType is WallID.EbonstoneUnsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedEbonstoneWallTile>());
            else if (tile.WallType is WallID.CrimstoneUnsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedCrimstoneWallTile>());
            else if (WallID.Sets.Conversion.HardenedSand[tile.WallType])
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedHardenedSandWallTile>());
            else if (WallID.Sets.Conversion.Sandstone[tile.WallType])
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedSandstoneWallTile>());
            else if (tile.WallType is WallID.IceUnsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedIceWallTile>());
            else if (tile.WallType is WallID.SnowWallUnsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedSnowWallTile>());
            else if (tile.WallType is WallID.LivingWood or WallID.LivingWoodUnsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedLivingWoodWallTile>());
            else if (tile.WallType is WallID.DirtUnsafe or WallID.DirtUnsafe1 or WallID.DirtUnsafe2 or WallID.DirtUnsafe3 or WallID.DirtUnsafe4 or WallID.GrassUnsafe or WallID.FlowerUnsafe or WallID.CorruptGrassUnsafe or WallID.CrimsonGrassUnsafe or WallID.Cave6Unsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedDirtWallTile>());
            else if (tile.WallType is WallID.MudUnsafe)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<IrradiatedMudWallTile>());
            else if (tile.WallType is WallID.Wood)
                ConvertWall(x1, y1, (ushort)ModContent.WallType<PetrifiedWoodWallTile>());
            #endregion
        }
        public static void ConvertTile(int i, int j, ushort type)
        {
            if (Main.tile[i, j].TileType != type)
            {
                Main.tile[i, j].TileType = type;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }

        public static void ConvertWall(int i, int j, ushort type)
        {
            if (Main.tile[i, j].WallType != type)
            {
                Main.tile[i, j].WallType = type;
                WorldGen.SquareWallFrame(i, j);
                NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }
    }
}