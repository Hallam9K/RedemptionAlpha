using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Walls;
using Redemption.Tiles.Tiles;
using Redemption.Base;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Redemption.Tiles.Bars;
using Redemption.Tiles.Furniture.ElderWood;

namespace Redemption.WorldGeneration
{
    public class AncientDecalGen
    {
        private static readonly Dictionary<Color, int> colorToTile = new()
        {
            [new Color(255, 0, 0)] = ModContent.TileType<GathicStoneBrickTile>(),
            [new Color(200, 0, 0)] = ModContent.TileType<GathicGladestoneBrickTile>(),
            [new Color(0, 255, 0)] = ModContent.TileType<GathicStoneTile>(),
            [new Color(0, 200, 0)] = ModContent.TileType<GathicGladestoneTile>(),
            [new Color(255, 0, 255)] = ModContent.TileType<ElderWoodTile>(),
            [new Color(100, 90, 70)] = ModContent.TileType<AncientDirtTile>(),
            [new Color(200, 200, 50)] = ModContent.TileType<AncientGoldCoinPileTile>(),
            [new Color(180, 180, 150)] = TileID.AmberGemspark,
            [new Color(0, 0, 255)] = TileID.TeamBlockBlue,
            [new Color(150, 150, 150)] = -2,
            [Color.Black] = -1
        };
        private static readonly Dictionary<Color, int> colorToWall = new()
        {
            [new Color(0, 0, 255)] = ModContent.WallType<GathicStoneBrickWallTileUnsafe>(),
            [new Color(0, 0, 200)] = ModContent.WallType<GathicGladestoneBrickWallTileUnsafe>(),
            [new Color(255, 0, 0)] = ModContent.WallType<GathicStoneWallTileUnsafe>(),
            [new Color(200, 0, 0)] = ModContent.WallType<GathicGladestoneWallTileUnsafe>(),
            [new Color(150, 150, 150)] = -2,
            [Color.Black] = -1
        };

        public static void PlaceL(Point origin, int ID = 0)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientDecal/ADecalL" + ID, AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientDecal/ADecalL" + ID + "_Walls", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall);
                gen.Generate(origin.X, origin.Y, true, true);
            });

            switch (ID)
            {
                case 2:
                    GenUtils.ObjectPlace(origin.X + 7, origin.Y + 12, ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 9, origin.Y + 14, ModContent.TileType<ElderWoodWorkbenchTile>());
                    GenUtils.ObjectPlace(origin.X + 11, origin.Y + 9, ModContent.TileType<ElderWoodChandelierTile>(), 1);
                    WorldGen.PlaceTile(origin.X + 11, origin.Y + 14, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1);
                    WorldGen.SlopeTile(origin.X + 11, origin.Y + 14, 2);
                    GenUtils.ObjectPlace(origin.X + 13, origin.Y + 13, ModContent.TileType<ElderWoodClockTile>());
                    GenUtils.ObjectPlace(origin.X + 17, origin.Y + 13, ModContent.TileType<ElderWoodBedTile>());
                    if (WorldGen.genRand.NextBool(2))
                        RedeGen.ElderWoodChest(origin.X + 15, origin.Y + 9);
                    break;
                case 3:
                    GenUtils.ObjectPlace(origin.X + 6, origin.Y + 14, ModContent.TileType<ElderWoodLampTile>(), 1);
                    break;
            }

            for (int i = origin.X; i < origin.X + 25; i++)
            {
                for (int j = origin.Y; j < origin.Y + 25; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.AmberGemspark:
                            Framing.GetTileSafely(i, j).ClearTile();
                            GenUtils.ObjectPlace(i, j, (ushort)ModContent.TileType<GraveSteelAlloyTile>());
                            break;
                        case TileID.TeamBlockBlue:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1);
                            break;
                    }
                    if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ElderWoodPlatformTile>() && WorldGen.InWorld(i, j))
                        WorldGen.KillTile(i, j, true);
                }
            }
        }
        public static void PlaceM(Point origin, int ID = 0)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientDecal/ADecalM" + ID, AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientDecal/ADecalM" + ID + "_Walls", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall);
                gen.Generate(origin.X, origin.Y, true, true);
            });

            switch (ID)
            {
                case 1:
                    RedeGen.ElderWoodChest(origin.X + 12, origin.Y + 17);
                    break;
                case 2:
                    RedeGen.ElderWoodChest(origin.X + 10, origin.Y + 19);
                    for (int i = origin.X + 8; i < origin.X + 14; i++)
                    {
                        for (int j = origin.Y + 17; j < origin.Y + 20; j++)
                        {
                            if (!Framing.GetTileSafely(i, j).HasTile)
                                WorldGen.PlaceLiquid(i, j, LiquidID.Water, 255);
                        }
                    }
                    break;
                case 3:
                    GenUtils.ObjectPlace(origin.X + 4, origin.Y + 12, ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 19, origin.Y + 12, ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 8, origin.Y + 14, ModContent.TileType<ElderWoodBookcaseTile>());
                    GenUtils.ObjectPlace(origin.X + 12, origin.Y + 14, ModContent.TileType<ElderWoodChairTile>(), 0, 1);
                    GenUtils.ObjectPlace(origin.X + 14, origin.Y + 14, ModContent.TileType<ElderWoodTableTile>());
                    GenUtils.ObjectPlace(origin.X + 14, origin.Y + 12, ModContent.TileType<ElderWoodCandleTile>(), 1);
                    if (WorldGen.genRand.NextBool(2))
                        RedeGen.ElderWoodChest(origin.X + 10, origin.Y + 14);
                    break;
                case 0:
                    GenUtils.ObjectPlace(origin.X + 11, origin.Y + 4, ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 14, origin.Y + 18, ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 13, origin.Y + 14, ModContent.TileType<ElderWoodSofaTile>());
                    GenUtils.ObjectPlace(origin.X + 13, origin.Y + 6, ModContent.TileType<ElderWoodClockTile>());
                    GenUtils.ObjectPlace(origin.X + 15, origin.Y + 6, ModContent.TileType<ElderWoodSinkTile>());
                    GenUtils.ObjectPlace(origin.X + 19, origin.Y + 6, ModContent.TileType<ElderWoodBedTile>());
                    GenUtils.ObjectPlace(origin.X + 17, origin.Y + 3, ModContent.TileType<ElderWoodLanternTile>(), 1);
                    GenUtils.ObjectPlace(origin.X + 16, origin.Y + 20, ModContent.TileType<ElderWoodChairTile>(), 0, 1);
                    GenUtils.ObjectPlace(origin.X + 17, origin.Y + 20, ModContent.TileType<ElderWoodWorkbenchTile>());
                    GenUtils.ObjectPlace(origin.X + 17, origin.Y + 19, ModContent.TileType<ElderWoodCandelabraTile>(), 1);
                    if (WorldGen.genRand.NextBool(2))
                        RedeGen.ElderWoodChest(origin.X + 21, origin.Y + 18);
                    break;
            }

            for (int i = origin.X; i < origin.X + 25; i++)
            {
                for (int j = origin.Y; j < origin.Y + 25; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.AmberGemspark:
                            Framing.GetTileSafely(i, j).ClearTile();
                            GenUtils.ObjectPlace(i, j, (ushort)ModContent.TileType<GraveSteelAlloyTile>());
                            break;
                        case TileID.TeamBlockBlue:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1);
                            break;
                    }
                    if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ElderWoodPlatformTile>() && WorldGen.InWorld(i, j))
                        WorldGen.KillTile(i, j, true);
                }
            }
        }
        public static void PlaceR(Point origin, int ID = 0)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientDecal/ADecalR" + ID, AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientDecal/ADecalR" + ID + "_Walls", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall);
                gen.Generate(origin.X, origin.Y, true, true);
            });

            switch (ID)
            {
                case 2:
                    if (WorldGen.genRand.NextBool(2))
                        RedeGen.ElderWoodChest(origin.X + 5, origin.Y + 18);
                    for (int i = origin.X + 4; i < origin.X + 16; i++)
                    {
                        for (int j = origin.Y + 8; j < origin.Y + 20; j++)
                        {
                            WorldGen.PlaceLiquid(i, j, LiquidID.Water, 255);
                        }
                    }
                    break;
                case 3:
                    GenUtils.ObjectPlace(origin.X + 6, origin.Y + 5, ModContent.TileType<ElderWoodSofaTile>());
                    GenUtils.ObjectPlace(origin.X + 11, origin.Y + 3, ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 17, origin.Y + 3, ModContent.TileType<ElderWoodLanternTile>(), 1);
                    GenUtils.ObjectPlace(origin.X + 16, origin.Y + 7, ModContent.TileType<ElderWoodPianoTile>());
                    GenUtils.ObjectPlace(origin.X + 21, origin.Y + 7, ModContent.TileType<ElderWoodTableTile>());
                    GenUtils.ObjectPlace(origin.X + 19, origin.Y + 7, ModContent.TileType<ElderWoodChairTile>(), 0, 1);
                    GenUtils.ObjectPlace(origin.X + 22, origin.Y + 5, TileID.ClayPot);
                    RedeGen.ElderWoodChest(origin.X + 10, origin.Y + 21);
                    break;
            }

            for (int i = origin.X; i < origin.X + 25; i++)
            {
                for (int j = origin.Y; j < origin.Y + 25; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.AmberGemspark:
                            Framing.GetTileSafely(i, j).ClearTile();
                            GenUtils.ObjectPlace(i, j, (ushort)ModContent.TileType<GraveSteelAlloyTile>());
                            break;
                        case TileID.TeamBlockBlue:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1);
                            break;
                    }
                    if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ElderWoodPlatformTile>() && WorldGen.InWorld(i, j))
                        WorldGen.KillTile(i, j, true);
                }
            }
        }
    }
}