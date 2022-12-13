using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Redemption.Tiles.Tiles;
using Redemption.Tiles.Ores;
using Redemption.Walls;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.ID;
using ReLogic.Content;
using Terraria.DataStructures;
using Redemption.Base;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.MusicBoxes;
using System.Linq;
using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Globals;
using Redemption.NPCs.Space;
using System;

namespace Redemption.WorldGeneration.Space
{
    public class SpaceSub : Subworld
    {
        public override int Width => 2400;
        public override int Height => 1200;
        public override bool NormalUpdates => false;
        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;
        public override List<GenPass> Tasks => new()
        {
            new SpacePass1("Loading", 1),
            new SpacePass2("Asteroid Bases", 0.5f),
            new SpacePass3("Asteroid Ores", 0.5f),
            new SpacePass4("Smoothing Asteroids", 0.3f),
        };
        public override void OnLoad()
        {
            SubworldSystem.hideUnderworld = true;
            Main.cloudAlpha = 0;
            Main.cloudBGAlpha = 0;
            Main.numClouds = 0;
            Main.rainTime = 0;
            Main.raining = false;
            Main.maxRaining = 0f;
            Main.slimeRain = false;
        }
        public override void OnUnload()
        {
            Main.sunModY = 0;
            Main.moonModY = 0;
        }
    }
    public class SpacePass1 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Loading";
            Main.spawnTileY = 612;
            Main.spawnTileX = 2400 / 2;
            Main.worldSurface = Main.maxTilesY - 42;
            Main.rockLayer = Main.maxTilesY + 42;
            for (int i = 0; i < 100; i++)
            {
                int X = WorldGen.genRand.Next(100, 2400 - 100);
                int Y = WorldGen.genRand.Next(100, 612);
                MakeMeteor(X, Y);
            }
            SlayerBase1();
        }
        private readonly int WIDTH = 168;
        private readonly int HEIGHT = 128;
        public void SlayerBase1()
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<AsteroidTile>(),
                [new Color(0, 255, 0)] = ModContent.TileType<SlayerShipPanelTile>(),
                [new Color(255, 255, 0)] = ModContent.TileType<HalogenLampTile>(),
                [new Color(0, 255, 255)] = ModContent.TileType<ShipGlassTile>(),
                [new Color(0, 0, 255)] = ModContent.TileType<MetalSupportBeamTile>(),
                [new Color(0, 255, 150)] = TileID.TinPlating,
                [new Color(0, 150, 255)] = TileID.CopperPlating,
                [new Color(255, 0, 150)] = TileID.TeamBlockPink,
                [new Color(150, 0, 255)] = TileID.TeamBlockGreen,
                [new Color(255, 0, 255)] = TileID.TeamBlockBlue,
                [new Color(150, 150, 150)] = -2, //turn into air
                [Color.Black] = -1 //don't touch when genning
            };
            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(255, 0, 0)] = ModContent.WallType<SlayerShipPanelWallTile>(),
                [new Color(0, 0, 255)] = ModContent.WallType<AsteroidWallTile>(),
                [new Color(0, 255, 255)] = WallID.Glass,
                [new Color(255, 255, 0)] = WallID.MartianConduit,
                [Color.Black] = -1
            };
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Space/SlayerBase1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Space/SlayerBase1_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Space/SlayerBase1_Slopes", AssetRequestMode.ImmediateLoad).Value;

            Point16 origin = new((2400 / 2) - 24, 510);
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, null, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });

            for (int i = origin.X + 114; i < origin.X + 127; i++)
            {
                if (Framing.GetTileSafely(i, origin.Y + 31).TileType == ModContent.TileType<SlayerShipPanelTile>() && WorldGen.InWorld(i, origin.Y + 31))
                    Wiring.ActuateForced(i, origin.Y + 31);
            }
            for (int i = origin.X + 97; i < origin.X + 103; i++)
            {
                for (int j = origin.Y + 23; j < origin.Y + 32; j++)
                {
                    if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<SlayerShipPanelTile>() && WorldGen.InWorld(i, j))
                        Wiring.ActuateForced(i, j);
                }
            }

            GenUtils.ObjectPlace(origin.X + 24, origin.Y + 102, ModContent.TileType<CyberTeleporterTile>());
            GenUtils.ObjectPlace(origin.X + 20, origin.Y + 90, ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 20, origin.Y + 89, ModContent.TileType<KSBoxTile>());
            GenUtils.ObjectPlace(origin.X + 24, origin.Y + 79, ModContent.TileType<LabCeilingLampTile>());
            GenUtils.ObjectPlace(origin.X + 106, origin.Y + 32, ModContent.TileType<CyberTableTile>());
            GenUtils.ObjectPlace(origin.X + 106, origin.Y + 30, ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 108, origin.Y + 32, ModContent.TileType<CyberChairTile>());
            GenUtils.ObjectPlace(origin.X + 113, origin.Y + 26, ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 127, origin.Y + 26, ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 134, origin.Y + 16, ModContent.TileType<LabCeilingLampTile>());
            GenUtils.ObjectPlace(origin.X + 132, origin.Y + 32, ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 132, origin.Y + 31, ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 131, origin.Y + 32, ModContent.TileType<CyberChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 89, origin.Y + 32, ModContent.TileType<ServerCabinetTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 91, origin.Y + 32, ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 83, origin.Y + 32, ModContent.TileType<ServerCabinetTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 81, origin.Y + 32, ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 92, origin.Y + 26, ModContent.TileType<CyberChairTile>());
            GenUtils.ObjectPlace(origin.X + 92, origin.Y + 26, ModContent.TileType<CyberChairTile>());
            GenUtils.ObjectPlace(origin.X + 90, origin.Y + 26, ModContent.TileType<CyberTableTile>());
            GenUtils.ObjectPlace(origin.X + 90, origin.Y + 24, ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 88, origin.Y + 26, ModContent.TileType<CyberChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 85, origin.Y + 26, ModContent.TileType<CyberChairTile>());
            GenUtils.ObjectPlace(origin.X + 83, origin.Y + 26, ModContent.TileType<CyberTableTile>());
            GenUtils.ObjectPlace(origin.X + 83, origin.Y + 24, ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 81, origin.Y + 26, ModContent.TileType<CyberChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 61, origin.Y + 32, ModContent.TileType<LabRailTile_L>());
            for (int i = 62; i < 77; i++)
                GenUtils.ObjectPlace(origin.X + i, origin.Y + 32, ModContent.TileType<LabRailTile_Mid>());
            GenUtils.ObjectPlace(origin.X + 77, origin.Y + 32, ModContent.TileType<LabRailTile_R>());
            GenUtils.ObjectPlace(origin.X + 29, origin.Y + 90, TileID.PottedPlants1);
            GenUtils.ObjectPlace(origin.X + 93, origin.Y + 70, TileID.PottedPlants1, 3);
            GenUtils.ObjectPlace(origin.X + 131, origin.Y + 23, TileID.PottedPlants2, 1);
            GenUtils.ObjectPlace(origin.X + 105, origin.Y + 30, TileID.ClayPot);
            GenUtils.ObjectPlace(origin.X + 105, origin.Y + 29, TileID.MatureHerbs);
            GenUtils.ObjectPlace(origin.X + 82, origin.Y + 24, TileID.LavaLamp);
            GenUtils.ObjectPlace(origin.X + 51, origin.Y + 27, TileID.PotsSuspended, 3);
            GenUtils.ObjectPlace(origin.X + 50, origin.Y + 32, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 53, origin.Y + 32, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 47, origin.Y + 16, ModContent.TileType<LabCeilingLampTile>());
            GenUtils.ObjectPlace(origin.X + 56, origin.Y + 16, ModContent.TileType<LabCeilingLampTile>());
            GenUtils.ObjectPlace(origin.X + 36, origin.Y + 16, ModContent.TileType<LabCeilingLampTile>());
            GenUtils.ObjectPlace(origin.X + 51, origin.Y + 16, ModContent.TileType<LabReceptionMonitorsTile>());
            GenUtils.ObjectPlace(origin.X + 48, origin.Y + 24, ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 54, origin.Y + 24, ModContent.TileType<ServerCabinetTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 51, origin.Y + 24, ModContent.TileType<LabReceptionDeskTile>());
            GenUtils.ObjectPlace(origin.X + 42, origin.Y + 32, TileID.PottedPlants1);
            GenUtils.ObjectPlace(origin.X + 34, origin.Y + 32, ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 36, origin.Y + 32, ModContent.TileType<ServerCabinetTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 38, origin.Y + 32, ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 44, origin.Y + 44, ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 59, origin.Y + 44, ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 59, origin.Y + 53, ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 59, origin.Y + 52, ModContent.TileType<SlayerWiringKitTile>());
            GenUtils.ObjectPlace(origin.X + 148, origin.Y + 31, ModContent.TileType<KSBattlestationTile>());
            GenUtils.ObjectPlace(origin.X + 19, origin.Y + 31, ModContent.TileType<KSBattlestationTile>());
            GenUtils.ObjectPlace(origin.X + 43, origin.Y + 53, ModContent.TileType<AndroidInactiveTile>());
            GenUtils.ObjectPlace(origin.X + 46, origin.Y + 54, ModContent.TileType<AndroidInactiveTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 56, origin.Y + 54, ModContent.TileType<AndroidInactiveTile>());
            GenUtils.ObjectPlace(origin.X + 23, origin.Y + 76, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 28, origin.Y + 76, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 50, origin.Y + 13, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 131, origin.Y + 13, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 136, origin.Y + 13, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 45, origin.Y + 48, ModContent.TileType<WallDatalogTile>());
            GenUtils.ObjectPlace(origin.X + 57, origin.Y + 48, ModContent.TileType<WallDatalogTile>(), 1);

            for (int i = origin.X; i < origin.X + WIDTH; i++)
            {
                for (int j = origin.Y; j < origin.Y + HEIGHT; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.TeamBlockPink:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, ModContent.TileType<LabPlatformTile>(), true);
                            WorldGen.SlopeTile(i, j, 1);
                            break;
                        case TileID.TeamBlockGreen:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, ModContent.TileType<LabPlatformTile>(), true);
                            WorldGen.SlopeTile(i, j, 2);
                            break;
                    }
                    if (Framing.GetTileSafely(i, j).TileType == TileID.TeamBlockBlue)
                    {
                        Framing.GetTileSafely(i, j).ClearTile();
                        WorldGen.PlaceTile(i, j, ModContent.TileType<LabPlatformTile>(), true);
                    }
                    if ((Framing.GetTileSafely(i, j).TileType == TileID.TinPlating || Framing.GetTileSafely(i, j).TileType == TileID.CopperPlating) && WorldGen.InWorld(i, j))
                        Framing.GetTileSafely(i, j).TileColor = PaintID.BlackPaint;
                }
            }
            for (int j = origin.Y + 33; j < origin.Y + 98; j++)
            {
                if (Framing.GetTileSafely(origin.X + 98, j).WallType == WallID.MartianConduit && WorldGen.InWorld(origin.X + 98, j))
                    Framing.GetTileSafely(origin.X + 98, j).WallColor = PaintID.BlackPaint;

                if (Framing.GetTileSafely(origin.X + 101, j).WallType == WallID.MartianConduit && WorldGen.InWorld(origin.X + 101, j))
                    Framing.GetTileSafely(origin.X + 101, j).WallColor = PaintID.BlackPaint;
            }
        }
        public static void MakeMeteor(int X, int Y)
        {
            Mod mod = Redemption.Instance;
            Point16 origin = new(X, Y);
            StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/Space/Asteriods", origin, mod);
        }
        public SpacePass1(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SpacePass2 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Asteroid Bases";
            AstBase1();
            AstBase2();
            AstBase2(4, WorldGen.genRand.NextBool());
            AstBase2(5, WorldGen.genRand.NextBool());
        }
        private readonly int WIDTH1 = 119;
        private readonly int HEIGHT1 = 75;
        public void AstBase1()
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<AsteroidTile>(),
                [new Color(0, 255, 0)] = ModContent.TileType<SlayerShipPanelTile>(),
                [new Color(255, 255, 0)] = ModContent.TileType<HalogenLampTile>(),
                [new Color(0, 0, 255)] = ModContent.TileType<MetalSupportBeamTile>(),
                [new Color(255, 0, 150)] = TileID.TeamBlockPink,
                [new Color(150, 0, 255)] = TileID.TeamBlockGreen,
                [new Color(255, 0, 255)] = TileID.TeamBlockBlue,
                [new Color(150, 150, 150)] = -2, //turn into air
                [Color.Black] = -1 //don't touch when genning
            };
            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(255, 0, 0)] = ModContent.WallType<SlayerShipPanelWallTile>(),
                [new Color(0, 0, 255)] = ModContent.WallType<AsteroidWallTile>(),
                [new Color(0, 255, 255)] = WallID.Glass,
                [Color.Black] = -1
            };
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Space/SlayerBase2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Space/SlayerBase2_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Space/SlayerBase2_Slopes", AssetRequestMode.ImmediateLoad).Value;

            Point16 origin = new(WorldGen.genRand.Next(2400 / 4, 2400 / 3), WorldGen.genRand.Next(100, 537));
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, null, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });

            GenUtils.ObjectPlace(origin.X + 49, origin.Y + 11, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 54, origin.Y + 11, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 59, origin.Y + 11, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 64, origin.Y + 11, ModContent.TileType<SolarPanelTile>());
            GenUtils.ObjectPlace(origin.X + 96, origin.Y + 41, ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 99, origin.Y + 41, ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 79, origin.Y + 36, ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 79, origin.Y + 39, ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 89, origin.Y + 36, ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 89, origin.Y + 39, ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 69, origin.Y + 36, TileID.PotsSuspended, 5);
            GenUtils.ObjectPlace(origin.X + 68, origin.Y + 41, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 71, origin.Y + 41, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 77, origin.Y + 33, TileID.PottedPlants1, 3);
            GenUtils.ObjectPlace(origin.X + 69, origin.Y + 33, ModContent.TileType<KSBattlestationTile>());
            GenUtils.ObjectPlace(origin.X + 11, origin.Y + 40, ModContent.TileType<AndroidInactiveTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 39, origin.Y + 40, ModContent.TileType<AndroidInactiveTile>());
            GenUtils.ObjectPlace(origin.X + 17, origin.Y + 47, ModContent.TileType<AndroidInactiveTile>());
            GenUtils.ObjectPlace(origin.X + 33, origin.Y + 47, ModContent.TileType<AndroidInactiveTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 21, origin.Y + 47, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 24, origin.Y + 47, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 27, origin.Y + 47, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 30, origin.Y + 47, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 22, origin.Y + 41, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 29, origin.Y + 41, ModContent.TileType<DroneShelfTile>());
            GenUtils.ObjectPlace(origin.X + 12, origin.Y + 47, ModContent.TileType<PrototypeSilverInactiveTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 39, origin.Y + 47, ModContent.TileType<PrototypeSilverInactiveTile>());
            GenUtils.ObjectPlace(origin.X + 36, origin.Y + 41, ModContent.TileType<PrototypeSilverInactiveTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 15, origin.Y + 41, ModContent.TileType<PrototypeSilverInactiveTile>());
            GenUtils.ObjectPlace(origin.X + 25, origin.Y + 38, ModContent.TileType<WallDatalogTile>(), 2);

            GenUtils.ObjectPlace(origin.X + 17, origin.Y + 41, ModContent.TileType<LabRailTile_L>());
            for (int i = 18; i < 34; i++)
                GenUtils.ObjectPlace(origin.X + i, origin.Y + 41, ModContent.TileType<LabRailTile_Mid>());
            GenUtils.ObjectPlace(origin.X + 34, origin.Y + 41, ModContent.TileType<LabRailTile_R>());

            for (int i = origin.X; i < origin.X + WIDTH1; i++)
            {
                for (int j = origin.Y; j < origin.Y + HEIGHT1; j++)
                {
                    switch (Framing.GetTileSafely(i, j).TileType)
                    {
                        case TileID.TeamBlockPink:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, ModContent.TileType<LabPlatformTile>(), true);
                            WorldGen.SlopeTile(i, j, 1);
                            break;
                        case TileID.TeamBlockGreen:
                            Framing.GetTileSafely(i, j).ClearTile();
                            WorldGen.PlaceTile(i, j, ModContent.TileType<LabPlatformTile>(), true);
                            WorldGen.SlopeTile(i, j, 2);
                            break;
                    }
                    if (Framing.GetTileSafely(i, j).TileType == TileID.TeamBlockBlue)
                    {
                        Framing.GetTileSafely(i, j).ClearTile();
                        WorldGen.PlaceTile(i, j, ModContent.TileType<LabPlatformTile>(), true);
                    }
                }
            }
        }
        public static void AstBase2(int ID = 3, bool left = false)
        {
            Mod mod = Redemption.Instance;
            Point16 dims = new();
            StructureHelper.Generator.GetDimensions("WorldGeneration/Space/SlayerBase" + ID, mod, ref dims);
            bool placed = false;
            Point16 origin = Point16.Zero;
            while (!placed)
            {
                if (left)
                    origin = new(WorldGen.genRand.Next((int)(2400 * 0.1f), (int)(2400 * 0.4f)), WorldGen.genRand.Next(100, 537));
                else
                    origin = new(WorldGen.genRand.Next((int)(2400 * 0.6f), (int)(2400 * 0.9f)), WorldGen.genRand.Next(100, 537));
                bool fail = false;
                for (int x = 0; x < dims.X; x++)
                {
                    for (int y = 0; y < dims.Y; y++)
                    {
                        int type = Framing.GetTileSafely(origin.X + x, origin.Y + y).TileType;
                        if (type == ModContent.TileType<ShipGlassTile>() || type == ModContent.TileType<SlayerShipPanelTile>())
                        {
                            fail = true;
                            break;
                        }
                    }
                }
                if (fail)
                    continue;
                placed = true;
            }
            StructureHelper.Generator.GenerateStructure("WorldGeneration/Space/SlayerBase" + ID, origin, mod);
            if (ID == 3)
                SpaceArea.base3Vector = origin.ToVector2();
        }
        public SpacePass2(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SpacePass3 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Asteroid Materials";

            for (int i = 0; i < 2400; i++)
            {
                for (int j = 0; j < 1200; j++)
                {
                    bool air = false;
                    bool tileUp = Framing.GetTileSafely(i, j - 1).TileType != ModContent.TileType<AsteroidTile>();
                    bool tileDown = Framing.GetTileSafely(i, j + 1).TileType != ModContent.TileType<AsteroidTile>();
                    bool tileLeft = Framing.GetTileSafely(i - 1, j).TileType != ModContent.TileType<AsteroidTile>();
                    bool tileRight = Framing.GetTileSafely(i + 1, j).TileType != ModContent.TileType<AsteroidTile>();
                    if (tileUp)
                        air = true;
                    else if (tileDown)
                        air = true;
                    else if (tileLeft)
                        air = true;
                    else if (tileRight)
                        air = true;

                    if (!air && Framing.GetTileSafely(i, j).TileType == ModContent.TileType<AsteroidTile>() && WorldGen.InWorld(i, j))
                        WorldGen.PlaceWall(i, j, ModContent.WallType<AsteroidWallTile>(), true);
                }
            }

            int[] OreArray = { TileID.BreakableIce,
                 TileID.Cobalt,
                 TileID.Iron,
                 TileID.Gold,
                 TileID.Platinum,
                 TileID.LunarOre };
            for (int k = 0; k < (int)(2400 * 1000 * 15E-04); k++)
            {
                int gemType = Utils.Next(WorldGen.genRand, OreArray);
                int tilesX = WorldGen.genRand.Next(0, 2400);
                int tilesY = WorldGen.genRand.Next(0, 612);
                if (Main.tile[tilesX, tilesY].TileType == ModContent.TileType<AsteroidTile>())
                {
                    new AsteroidOreGen(new Vector2(tilesX, tilesY), Vector2.Zero, new Point16(-7, 7), new Point16(-3, 3), WorldGen.genRand.Next(2, 12), WorldGen.genRand.Next(4, 14), (ushort)gemType, false, true).Start();
                }
            }
        }
        public SpacePass3(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SpacePass4 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Smoothing Tiles";
            int[] TileArray = { ModContent.TileType<AsteroidTile>() };
            for (int i = 0; i < 2400; i++)
            {
                for (int j = 0; j < 1200; j++)
                {
                    if (TileArray.Contains(Framing.GetTileSafely(i, j).TileType) && WorldGen.InWorld(i, j))
                        BaseWorldGen.SmoothTiles(i, j, i, j);

                    if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<LabPlatformTile>() && WorldGen.InWorld(i, j))
                        WorldGen.KillTile(i, j, true);
                }
            }
        }
        public SpacePass4(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class AsteroidOreGen : TileRunner
    {
        public AsteroidOreGen(Vector2 pos, Vector2 speed, Point16 hRange, Point16 vRange, double strength, int steps, ushort type, bool addTile, bool overRide) : base(pos, speed, hRange, vRange, strength, steps, type, addTile, overRide)
        {
        }
        public override bool ValidTile(Tile tile, int x, int y)
        {
            bool tileUp = !Framing.GetTileSafely(x, y - 1).HasTile || !Framing.GetTileSafely(x, y - 2).HasTile;
            bool tileDown = !Framing.GetTileSafely(x, y + 1).HasTile || !Framing.GetTileSafely(x, y + 2).HasTile;
            bool tileLeft = !Framing.GetTileSafely(x - 1, y).HasTile || !Framing.GetTileSafely(x - 2, y).HasTile;
            bool tileRight = !Framing.GetTileSafely(x + 1, y).HasTile || !Framing.GetTileSafely(x + 2, y).HasTile;
            if (tileUp)
                return false;
            else if (tileDown)
                return false;
            else if (tileLeft)
                return false;
            else if (tileRight)
                return false;

            return tile.TileType == ModContent.TileType<AsteroidTile>();
        }
    }
}
