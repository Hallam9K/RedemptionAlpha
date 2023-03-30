using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Redemption.Base;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Redemption.Tiles.Furniture.Misc;
using System.Linq;

namespace Redemption.WorldGeneration
{
    public class GatewayIslandClear : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GoldenGateway/GatewayIsland_Clear", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            return true;
        }
    }
    public class GatewayIsland : MicroBiome
    {
        private readonly int WIDTH = 144;
        private readonly int HEIGHT = 67;
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = TileID.MarbleBlock,
                [new Color(150, 0, 0)] = TileID.PearlstoneBrick,
                [new Color(0, 0, 255)] = TileID.MarbleColumn,
                [new Color(0, 255, 0)] = TileID.Grass,
                [new Color(0, 255, 255)] = TileID.Cloud,
                [new Color(0, 150, 255)] = TileID.RainCloud,
                [new Color(255, 150, 0)] = TileID.Dirt,
                [new Color(255, 255, 0)] = TileID.GoldBrick,
                [new Color(255, 0, 255)] = TileID.YellowStucco,
                [new Color(100, 0, 0)] = TileID.RedStucco,
                [new Color(0, 100, 0)] = TileID.GreenStucco,
                [new Color(80, 0, 0)] = TileID.GrayStucco,
                [new Color(0, 80, 0)] = TileID.TeamBlockBlue,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(255, 0, 0)] = WallID.MarbleBlock,
                [new Color(150, 0, 0)] = WallID.MarbleUnsafe,
                [new Color(0, 255, 0)] = WallID.LivingLeaf,
                [new Color(0, 255, 255)] = WallID.Cloud,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GoldenGateway/GatewayIsland", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GoldenGateway/GatewayIsland_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GoldenGateway/GatewayIsland_Slopes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texLiquids = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GoldenGateway/GatewayIsland_Liquids", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, texLiquids, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            GenUtils.ObjectPlace(origin.X + 74, origin.Y + 30, ModContent.TileType<GoldenGatewayTile>());
            GenUtils.ObjectPlace(origin.X + 48, origin.Y + 33, TileID.Lamps, 30);
            GenUtils.ObjectPlace(origin.X + 101, origin.Y + 33, TileID.Lamps, 30);
            GenUtils.ObjectPlace(origin.X + 63, origin.Y + 32, TileID.Statues, 36);
            GenUtils.ObjectPlace(origin.X + 113, origin.Y + 33, TileID.Statues, 36);
            GenUtils.ObjectPlace(origin.X + 67, origin.Y + 30, TileID.WorkBenches, 30);
            GenUtils.ObjectPlace(origin.X + 81, origin.Y + 30, TileID.WorkBenches, 30);

            for (int i = origin.X; i < origin.X + WIDTH; i++)
            {
                for (int j = origin.Y; j < origin.Y + HEIGHT; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.RedStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 29);
                            WorldGen.SlopeTile(i, j, 1);
                            break;
                        case TileID.GreenStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 29);
                            WorldGen.SlopeTile(i, j, 2);
                            break;
                        case TileID.GrayStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 31);
                            WorldGen.SlopeTile(i, j, 1);
                            break;
                        case TileID.TeamBlockBlue:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 31);
                            WorldGen.SlopeTile(i, j, 2);
                            break;
                    }
                    if (Framing.GetTileSafely(i, j).TileType == TileID.YellowStucco)
                    {
                        Framing.GetTileSafely(i, j).ClearTile();
                        WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 29);
                    }

                    if ((Framing.GetTileSafely(i, j).WallType == WallID.MarbleBlock || Framing.GetTileSafely(i, j).WallType == WallID.MarbleUnsafe) && WorldGen.InWorld(i, j))
                        Framing.GetTileSafely(i, j).WallColor = PaintID.BlackPaint;
                }
            }
            for (int i = origin.X + 38; i < origin.X + 47; i++)
            {
                for (int j = origin.Y + 10; j < origin.Y + 34; j++)
                {
                    if (WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X + 53; i < origin.X + 62; i++)
            {
                for (int j = origin.Y + 6; j < origin.Y + 33; j++)
                {
                    if (WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X + 88; i < origin.X + 97; i++)
            {
                for (int j = origin.Y + 6; j < origin.Y + 33; j++)
                {
                    if (WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X + 103; i < origin.X + 112; i++)
            {
                for (int j = origin.Y + 10; j < origin.Y + 34; j++)
                {
                    if (WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X; i < origin.X + WIDTH; i++)
            {
                for (int j = origin.Y + 29; j < origin.Y + HEIGHT; j++)
                {
                    if (WorldGen.genRand.NextBool(10))
                        WorldGen.PlacePot(i, j - 1, 28, Main.rand.Next(31, 34));
                }
            }
            int[] TileArray = { TileID.Dirt,
                TileID.Grass,
                TileID.Cloud,
                TileID.RainCloud };
            for (int i = origin.X; i < origin.X + WIDTH; i++)
            {
                for (int j = origin.Y; j < origin.Y + HEIGHT; j++)
                {
                    if (TileArray.Contains(Framing.GetTileSafely(i, j).TileType) && WorldGen.InWorld(i, j))
                        BaseWorldGen.SmoothTiles(i, j, i + 1, j + 1);
                }
            }
            GenVars.structures.AddProtectedStructure(new Rectangle(origin.X, origin.Y, WIDTH, HEIGHT));
            return true;
        }
    }
}