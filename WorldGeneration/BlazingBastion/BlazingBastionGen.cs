using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Walls;
using Redemption.Tiles.Tiles;
using Terraria.WorldBuilding;
using Redemption.Base;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Redemption.Tiles.Furniture.Misc;

namespace Redemption.WorldGeneration
{
    public class BastionClear : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/BastionBase_Clear", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            return true;
        }
    }
    public class BlazingBastion : MicroBiome
    {
        private readonly int WIDTH = 295;
        private readonly int HEIGHT = 155;
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<ShinkiteBrickTile>(),
                [new Color(150, 0, 0)] = ModContent.TileType<ShinkiteBeamTile>(),
                [new Color(0, 255, 0)] = TileID.HellstoneBrick,
                [new Color(0, 0, 255)] = TileID.ObsidianBrick,
                [new Color(0, 255, 255)] = TileID.Ash,
                [new Color(0, 150, 255)] = TileID.Grate,
                [new Color(255, 0, 255)] = TileID.RedStucco,
                [new Color(255, 255, 0)] = TileID.YellowStucco,
                [new Color(255, 150, 0)] = TileID.GreenStucco,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(255, 0, 0)] = ModContent.WallType<ShinkiteBrickWallTile>(),
                [new Color(0, 255, 0)] = WallID.HellstoneBrickUnsafe,
                [new Color(0, 0, 255)] = WallID.ObsidianBrickUnsafe,
                [new Color(255, 0, 255)] = WallID.Lavafall,
                [new Color(0, 255, 255)] = WallID.RedStainedGlass,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/BastionBase", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/BastionBase_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/BastionBase_Slopes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texLiquids = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/BastionBase_Liquids", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, texLiquids, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            for (int i = origin.X + 25; i < origin.X + 142; i++)
            {
                for (int j = origin.Y + 46; j < origin.Y + 50; j++)
                {
                    if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ShinkiteBrickTile>() && WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X + 172; i < origin.X + 177; i++)
            {
                for (int j = origin.Y + 87; j < origin.Y + 107; j++)
                {
                    if ((Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ShinkiteBrickTile>() ||
                        Framing.GetTileSafely(i, j).TileType == TileID.ObsidianBrick ||
                        Framing.GetTileSafely(i, j).TileType == TileID.HellstoneBrick) && WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X + 193; i < origin.X + 198; i++)
            {
                for (int j = origin.Y + 87; j < origin.Y + 107; j++)
                {
                    if ((Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ShinkiteBrickTile>() ||
                        Framing.GetTileSafely(i, j).TileType == TileID.ObsidianBrick ||
                        Framing.GetTileSafely(i, j).TileType == TileID.HellstoneBrick) && WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X + 230; i < origin.X + 235; i++)
            {
                for (int j = origin.Y + 87; j < origin.Y + 107; j++)
                {
                    if ((Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ShinkiteBrickTile>() ||
                        Framing.GetTileSafely(i, j).TileType == TileID.ObsidianBrick ||
                        Framing.GetTileSafely(i, j).TileType == TileID.HellstoneBrick) && WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            for (int i = origin.X + 251; i < origin.X + 256; i++)
            {
                for (int j = origin.Y + 87; j < origin.Y + 107; j++)
                {
                    if ((Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ShinkiteBrickTile>() ||
                        Framing.GetTileSafely(i, j).TileType == TileID.ObsidianBrick ||
                        Framing.GetTileSafely(i, j).TileType == TileID.HellstoneBrick) && WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }
            // Tower 1
            GenUtils.ObjectPlace(origin.X + 151, origin.Y + 53, TileID.TallGateClosed);
            GenUtils.ObjectPlace(origin.X + 163, origin.Y + 53, TileID.TallGateClosed);
            GenUtils.ObjectPlace(origin.X + 157, origin.Y + 56, TileID.Bookcases, 4);
            GenUtils.ObjectPlace(origin.X + 155, origin.Y + 47, TileID.HangingLanterns, 32);
            GenUtils.ObjectPlace(origin.X + 159, origin.Y + 47, TileID.HangingLanterns, 32);
            GenUtils.ObjectPlace(origin.X + 155, origin.Y + 45, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 159, origin.Y + 45, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 152, origin.Y + 30, TileID.Chairs, 16);
            GenUtils.ObjectPlace(origin.X + 162, origin.Y + 30, TileID.Chairs, 16, 1);
            GenUtils.ObjectPlace(origin.X + 157, origin.Y + 25, TileID.Chandeliers, 32);
            WorldGen.AddBuriedChest(origin.X + 154, origin.Y + 30, 0, false, 4);
            // Tower 2
            GenUtils.ObjectPlace(origin.X + 264, origin.Y + 53, TileID.TallGateClosed);
            GenUtils.ObjectPlace(origin.X + 276, origin.Y + 53, TileID.TallGateClosed);
            GenUtils.ObjectPlace(origin.X + 270, origin.Y + 56, TileID.Bookcases, 4);
            GenUtils.ObjectPlace(origin.X + 268, origin.Y + 47, TileID.HangingLanterns, 32);
            GenUtils.ObjectPlace(origin.X + 272, origin.Y + 47, TileID.HangingLanterns, 32);
            GenUtils.ObjectPlace(origin.X + 268, origin.Y + 45, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 272, origin.Y + 45, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 265, origin.Y + 30, TileID.Chairs, 16);
            GenUtils.ObjectPlace(origin.X + 275, origin.Y + 30, TileID.Chairs, 16, 1);
            GenUtils.ObjectPlace(origin.X + 270, origin.Y + 25, TileID.Chandeliers, 32);
            WorldGen.AddBuriedChest(origin.X + 274, origin.Y + 30, 0, false, 4);

            // Bastion
            GenUtils.ObjectPlace(origin.X + 180, origin.Y + 59, TileID.TallGateClosed);
            GenUtils.ObjectPlace(origin.X + 247, origin.Y + 59, TileID.TallGateClosed);
            GenUtils.ObjectPlace(origin.X + 187, origin.Y + 63, TileID.Benches, 10);
            GenUtils.ObjectPlace(origin.X + 193, origin.Y + 63, TileID.Benches, 10);
            GenUtils.ObjectPlace(origin.X + 190, origin.Y + 63, TileID.Bookcases, 4);
            GenUtils.ObjectPlace(origin.X + 213, origin.Y + 63, TileID.GrandfatherClocks, 17);
            GenUtils.ObjectPlace(origin.X + 211, origin.Y + 63, TileID.Dressers, 9);
            GenUtils.ObjectPlace(origin.X + 216, origin.Y + 63, TileID.Dressers, 9);
            GenUtils.ObjectPlace(origin.X + 237, origin.Y + 63, TileID.Chairs, 16);
            GenUtils.ObjectPlace(origin.X + 232, origin.Y + 63, TileID.Chairs, 16, 1);
            GenUtils.ObjectPlace(origin.X + 235, origin.Y + 63, TileID.Tables, 13);
            GenUtils.ObjectPlace(origin.X + 235, origin.Y + 61, TileID.Candelabras, 25);
            GenUtils.ObjectPlace(origin.X + 211, origin.Y + 61, TileID.Candles, 25);
            GenUtils.ObjectPlace(origin.X + 216, origin.Y + 61, TileID.Candles, 25);
            GenUtils.ObjectPlace(origin.X + 182, origin.Y + 90, TileID.Chandeliers, 32);
            GenUtils.ObjectPlace(origin.X + 187, origin.Y + 90, TileID.Chandeliers, 32);
            GenUtils.ObjectPlace(origin.X + 205, origin.Y + 90, TileID.Chandeliers, 32);
            GenUtils.ObjectPlace(origin.X + 222, origin.Y + 90, TileID.Chandeliers, 32);
            GenUtils.ObjectPlace(origin.X + 240, origin.Y + 90, TileID.Chandeliers, 32);
            GenUtils.ObjectPlace(origin.X + 245, origin.Y + 90, TileID.Chandeliers, 32);

            GenUtils.ObjectPlace(origin.X + 180, origin.Y + 39, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 186, origin.Y + 39, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 241, origin.Y + 39, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 247, origin.Y + 39, TileID.Lamps, 23);

            GenUtils.ObjectPlace(origin.X + 177, origin.Y + 87, ModContent.TileType<NozaCageHangingTile>());
            GenUtils.ObjectPlace(origin.X + 200, origin.Y + 87, ModContent.TileType<NozaCageHangingTile>());
            GenUtils.ObjectPlace(origin.X + 217, origin.Y + 87, ModContent.TileType<NozaCageHangingTile>());
            GenUtils.ObjectPlace(origin.X + 249, origin.Y + 87, ModContent.TileType<NozaCageHangingTile>());
            GenUtils.ObjectPlace(origin.X + 184, origin.Y + 106, ModContent.TileType<NozaCageTile>());
            GenUtils.ObjectPlace(origin.X + 190, origin.Y + 106, ModContent.TileType<NozaCageLargeTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 198, origin.Y + 106, ModContent.TileType<NozaCageSmallTile>());
            GenUtils.ObjectPlace(origin.X + 221, origin.Y + 106, ModContent.TileType<NozaCageTile>());
            GenUtils.ObjectPlace(origin.X + 224, origin.Y + 106, ModContent.TileType<NozaCageSmallTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 242, origin.Y + 106, ModContent.TileType<NozaCageTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 248, origin.Y + 106, ModContent.TileType<NozaCageLargeTile>());
            GenUtils.ObjectPlace(origin.X + 190, origin.Y + 101, ModContent.TileType<NozaCageSmallTile>());
            GenUtils.ObjectPlace(origin.X + 242, origin.Y + 103, ModContent.TileType<NozaCageSmallTile>(), 0, 1);

            bool placedBigHouse = false;
            int attempts = 0;
            while (!placedBigHouse && attempts++ < 10000)
            {
                int placeX = WorldGen.genRand.Next(origin.X + 12, origin.X + 123);
                int placeY = origin.Y + 28;
                bool whitelist = false;
                for (int i = -2; i <= 26; i++)
                {
                    for (int j = 0; j <= 30; j++)
                    {
                        int type = Framing.GetTileSafely(placeX + i, placeY + j).TileType;
                        if (type == TileID.ObsidianBrick || type == TileID.HellstoneBrick)
                        {
                            whitelist = true;
                            break;
                        }
                    }
                }
                if (whitelist)
                    continue;

                Point bigHouseOrigin = new(placeX, placeY);
                BigHouse(bigHouseOrigin);
                placedBigHouse = true;
            }
            int placedHouse = 0;
            attempts = 0;
            while (placedHouse < 2 && attempts++ < 10000)
            {
                int placeX = WorldGen.genRand.Next(origin.X + 12, origin.X + 128);
                int placeY = origin.Y + 38;
                bool whitelist = false;
                for (int i = -2; i <= 21; i++)
                {
                    for (int j = 0; j <= 20; j++)
                    {
                        int type = Framing.GetTileSafely(placeX + i, placeY + j).TileType;
                        if (type == TileID.ObsidianBrick || type == TileID.HellstoneBrick)
                        {
                            whitelist = true;
                            break;
                        }
                    }
                }
                if (whitelist)
                    continue;

                Point houseOrigin = new(placeX, placeY);
                House(houseOrigin);
                placedHouse++;
            }
            int placedMiniTower = 0;
            attempts = 0;
            while (placedMiniTower < 3 && attempts++ < 10000)
            {
                int placeX = WorldGen.genRand.Next(origin.X + 12, origin.X + 135);
                int placeY = origin.Y + 29;
                bool whitelist = false;
                for (int i = -2; i <= 14; i++)
                {
                    for (int j = 0; j <= 29; j++)
                    {
                        int type = Framing.GetTileSafely(placeX + i, placeY + j).TileType;
                        if (type == TileID.ObsidianBrick || type == TileID.HellstoneBrick)
                        {
                            whitelist = true;
                            break;
                        }
                    }
                }
                if (whitelist)
                    continue;

                Point towerOrigin = new(placeX, placeY);
                MiniTower(towerOrigin);
                placedMiniTower++;
            }
            for (int i = origin.X; i < origin.X + WIDTH; i++)
            {
                for (int j = origin.Y; j < origin.Y + HEIGHT; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.GreenStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 13);
                            WorldGen.SlopeTile(i, j, 1);
                            break;
                        case TileID.YellowStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 13);
                            WorldGen.SlopeTile(i, j, 2);
                            break;
                        case TileID.RedStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 13);
                            break;
                    }
                }
            }
            for (int i = origin.X; i < origin.X + WIDTH; i++)
            {
                for (int j = origin.Y + 29; j < origin.Y + HEIGHT; j++)
                {
                    if (WorldGen.genRand.NextBool(20))
                        WorldGen.PlacePot(i, j - 1, 28, Main.rand.Next(13, 16));
                    if (WorldGen.genRand.NextBool(30))
                        GenUtils.ObjectPlace(i, j - 1, TileID.Statues, 49);
                    if (WorldGen.genRand.NextBool(30))
                    {
                        switch (WorldGen.genRand.Next(3))
                        {
                            case 0:
                                GenUtils.ObjectPlace(i, j - 1, TileID.PottedPlants2, WorldGen.genRand.Next(8, 10));
                                break;
                            case 1:
                                GenUtils.ObjectPlace(i, j - 1, TileID.PottedLavaPlants, WorldGen.genRand.NextBool(2) ? 2 : 0);
                                break;
                            case 2:
                                GenUtils.ObjectPlace(i, j - 1, TileID.PottedLavaPlantTendrils);
                                break;
                        }
                    }
                }
            }
            return true;
        }
        public static void MiniTower(Point origin)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<ShinkiteBrickTile>(),
                [new Color(0, 255, 0)] = TileID.HellstoneBrick,
                [new Color(0, 0, 255)] = TileID.ObsidianBrick,
                [new Color(255, 0, 255)] = TileID.RedStucco,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };
            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(0, 255, 0)] = WallID.HellstoneBrickUnsafe,
                [new Color(0, 0, 255)] = WallID.ObsidianBrickUnsafe,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/MiniTower1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/MiniTower1_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/MiniTower1_Slopes", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, null, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            for (int i = origin.X; i < origin.X + 12; i++)
            {
                for (int j = origin.Y; j < origin.Y + 30; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.RedStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 13);
                            break;
                    }
                }
            }
            GenUtils.ObjectPlace(origin.X + 1, origin.Y + 26, TileID.ClosedDoor, 19);
            GenUtils.ObjectPlace(origin.X + 10, origin.Y + 26, TileID.ClosedDoor, 19);
            GenUtils.ObjectPlace(origin.X + 3, origin.Y + 19, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 8, origin.Y + 19, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 2, origin.Y + 7, TileID.Chairs, 16);
            GenUtils.ObjectPlace(origin.X + 9, origin.Y + 7, TileID.Chairs, 16, 1);
            GenUtils.ObjectPlace(origin.X + 3, origin.Y + 4, TileID.Torches, 7);
            GenUtils.ObjectPlace(origin.X + 8, origin.Y + 4, TileID.Torches, 7);
            WorldGen.AddBuriedChest(origin.X + 6, origin.Y + 6, 0, false, 4);
        }
        public static void BigHouse(Point origin)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<ShinkiteBrickTile>(),
                [new Color(150, 0, 0)] = ModContent.TileType<ShinkiteBeamTile>(),
                [new Color(0, 255, 0)] = TileID.HellstoneBrick,
                [new Color(0, 0, 255)] = TileID.ObsidianBrick,
                [new Color(255, 0, 255)] = TileID.RedStucco,
                [new Color(255, 255, 0)] = TileID.YellowStucco,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };
            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(255, 0, 0)] = ModContent.WallType<ShinkiteBrickWallTile>(),
                [new Color(0, 255, 0)] = WallID.HellstoneBrickUnsafe,
                [new Color(0, 0, 255)] = WallID.ObsidianBrickUnsafe,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/House1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/House1_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/House1_Slopes", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, null, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            for (int i = origin.X; i < origin.X + 24; i++)
            {
                for (int j = origin.Y; j < origin.Y + 31; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.RedStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 13);
                            break;
                        case TileID.YellowStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 13);
                            WorldGen.SlopeTile(i, j, 2);
                            break;
                    }
                }
            }
            GenUtils.ObjectPlace(origin.X + 2, origin.Y + 27, TileID.ClosedDoor, 19);
            GenUtils.ObjectPlace(origin.X + 15, origin.Y + 27, TileID.ClosedDoor, 19);
            GenUtils.ObjectPlace(origin.X + 14, origin.Y + 17, TileID.ClosedDoor, 19);
            GenUtils.ObjectPlace(origin.X + 5, origin.Y + 29, TileID.Bookcases, 4);
            GenUtils.ObjectPlace(origin.X + 11, origin.Y + 29, TileID.WorkBenches, 14);
            GenUtils.ObjectPlace(origin.X + 10, origin.Y + 29, TileID.Chairs, 16, 1);
            GenUtils.ObjectPlace(origin.X + 8, origin.Y + 22, TileID.Candelabras, 25);
            GenUtils.ObjectPlace(origin.X + 19, origin.Y + 19, TileID.Beds, 8);
            GenUtils.ObjectPlace(origin.X + 19, origin.Y + 15, TileID.HangingLanterns, 32);
            GenUtils.ObjectPlace(origin.X + 7, origin.Y + 13, TileID.Pianos, 15);
            GenUtils.ObjectPlace(origin.X + 12, origin.Y + 12, TileID.Tables, 13);
            GenUtils.ObjectPlace(origin.X + 10, origin.Y + 12, TileID.Chairs, 16, 1);
            GenUtils.ObjectPlace(origin.X + 8, origin.Y + 5, TileID.PotsSuspended, 8);
            GenUtils.ObjectPlace(origin.X + 12, origin.Y + 10, TileID.Candles, 25);
            GenUtils.ObjectPlace(origin.X + 18, origin.Y + 29, TileID.Campfire, 2);
            GenUtils.ObjectPlace(origin.X + 11, origin.Y + 16, TileID.Banners, WorldGen.genRand.Next(14, 22));
            GenUtils.ObjectPlace(origin.X + 12, origin.Y + 25, TileID.Painting3X2, WorldGen.genRand.NextBool(2) ? 0 : WorldGen.genRand.Next(16, 18));
        }
        public static void House(Point origin)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<ShinkiteBrickTile>(),
                [new Color(0, 255, 0)] = TileID.HellstoneBrick,
                [new Color(0, 0, 255)] = TileID.ObsidianBrick,
                [new Color(255, 0, 255)] = TileID.RedStucco,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };
            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(255, 0, 0)] = ModContent.WallType<ShinkiteBrickWallTile>(),
                [new Color(0, 255, 0)] = WallID.HellstoneBrickUnsafe,
                [new Color(0, 0, 255)] = WallID.ObsidianBrickUnsafe,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/House2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/House2_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/BlazingBastion/House2_Slopes", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, null, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            for (int i = origin.X; i < origin.X + 19; i++)
            {
                for (int j = origin.Y; j < origin.Y + 21; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.RedStucco:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, TileID.Platforms, true, false, -1, 13);
                            break;
                    }
                }
            }
            GenUtils.ObjectPlace(origin.X + 2, origin.Y + 17, TileID.ClosedDoor, 19);
            GenUtils.ObjectPlace(origin.X + 16, origin.Y + 17, TileID.ClosedDoor, 19);
            GenUtils.ObjectPlace(origin.X + 9, origin.Y + 19, TileID.Tables, 13);
            GenUtils.ObjectPlace(origin.X + 7, origin.Y + 19, TileID.Chairs, 16, 1);
            GenUtils.ObjectPlace(origin.X + 11, origin.Y + 19, TileID.Chairs, 16);
            GenUtils.ObjectPlace(origin.X + 9, origin.Y + 17, TileID.Candles, 25);
            GenUtils.ObjectPlace(origin.X + 4, origin.Y + 12, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 14, origin.Y + 12, TileID.Lamps, 23);
            GenUtils.ObjectPlace(origin.X + 9, origin.Y + 5, TileID.Chandeliers, 32);
            GenUtils.ObjectPlace(origin.X + 5, origin.Y + 12, TileID.GrandfatherClocks, 17);
            GenUtils.ObjectPlace(origin.X + 12, origin.Y + 12, TileID.Benches, 10);
            GenUtils.ObjectPlace(origin.X + 5, origin.Y + 14, TileID.Banners, WorldGen.genRand.Next(14, 22));
            GenUtils.ObjectPlace(origin.X + 13, origin.Y + 14, TileID.Banners, WorldGen.genRand.Next(14, 22));
            if (WorldGen.genRand.NextBool(2))
                GenUtils.ObjectPlace(origin.X + 9, origin.Y + 8, TileID.Painting3X2, WorldGen.genRand.NextBool(2) ? 0 : WorldGen.genRand.Next(16, 18));
        }
    }
}