using Terraria.ModLoader;
using Terraria;
using Terraria.WorldBuilding;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ID;
using Terraria.DataStructures;
using Redemption.StructureHelper;
using Redemption.Globals;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Containers;
using Redemption.Tiles.Tiles;
using Redemption.Tiles.Plants;
using Microsoft.Xna.Framework;
using Redemption.Tiles.Ores;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Chat;
using Terraria.Localization;
using Redemption.Tiles.Furniture.Misc;
using Redemption.Tiles.Natural;

namespace Redemption.WorldGeneration
{
    public class RedeGen : ModSystem
    {
        public static bool dragonLeadSpawn;
        public static Point newbCavePoint;

        public override void OnWorldLoad()
        {
            if (NPC.downedBoss3)
                dragonLeadSpawn = true;
            else
                dragonLeadSpawn = false;
        }

        public override void OnWorldUnload()
        {
            dragonLeadSpawn = false;
        }

        public override void PostUpdateWorld()
        {
            if (NPC.downedBoss3 && !dragonLeadSpawn)
            {
                dragonLeadSpawn = true;
                if (RedeWorld.alignment >= 0)
                {
                    string status = "Crystals form in the icy caverns...";
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightBlue);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.LightBlue);

                    for (int i = 0; i < Main.maxTilesX; i++)
                    {
                        for (int j = 0; j < Main.maxTilesY; j++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile.type == ModContent.TileType<DragonLeadOre2Tile>())
                                tile.type = TileID.Stone;
                        }
                    }
                }
                else
                {
                    string status = "The caverns are heated with dragon bone...";
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Orange);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.Orange);

                    for (int i = 0; i < Main.maxTilesX; i++)
                    {
                        for (int j = 0; j < Main.maxTilesY; j++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile.type == ModContent.TileType<DragonLeadOre2Tile>())
                                tile.type = (ushort)ModContent.TileType<DragonLeadOreTile>();
                        }
                    }
                }
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }

        /// <summary>
        /// Checks if the given area is more or less flattish.
        /// Returns false if the average tile height variation is greater than the threshold.
        /// Expects that the first tile is solid, and traverses from there.
        /// Use the weight parameters to change the importance of up/down checks. - Spirit Mod
        /// </summary>
        public static bool CheckFlat(int startX, int startY, int width, float threshold, int goingDownWeight = 0, int goingUpWeight = 0)
        {
            // Fail if the tile at the other end of the check plane isn't also solid
            if (!WorldGen.SolidTile(startX + width, startY)) return false;

            float totalVariance = 0;
            for (int i = 0; i < width; i++)
            {
                if (startX + i >= Main.maxTilesX) return false;

                // Fail if there is a tile very closely above the check area
                for (int k = startY - 1; k > startY - 100; k--)
                {
                    if (WorldGen.SolidTile(startX + i, k)) return false;
                }

                // If the tile is solid, go up until we find air
                // If the tile is not, go down until we find a floor
                int offset = 0;
                bool goingUp = WorldGen.SolidTile(startX + i, startY);
                offset += goingUp ? goingUpWeight : goingDownWeight;
                while ((goingUp && WorldGen.SolidTile(startX + i, startY - offset))
                    || (!goingUp && !WorldGen.SolidTile(startX + i, startY + offset)))
                {
                    offset++;
                }
                if (goingUp) offset--; // account for going up counting the first tile
                totalVariance += offset;
            }
            return totalVariance / width <= threshold;
        }

        private static void SpawnThornSummon()
        {
            bool placed1 = false;
            int attempts = 0;
            int placed2 = 0;
            int placeX2 = 0;
            while (!placed1 && attempts++ < 100000)
            {
                int placeX = Main.spawnTileX + WorldGen.genRand.Next(-600, 600);

                int placeY = (int)Main.worldSurface - 200;

                if (placeX > Main.spawnTileX - 100 && placeX < Main.spawnTileX + 100)
                    continue;
                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(placeX, placeY) && placeY <= Main.worldSurface)
                {
                    placeY++;
                }
                // If we went under the world's surface, try again
                if (placeY > Main.worldSurface)
                    continue;
                Tile tile = Main.tile[placeX, placeY];
                if (tile.type != TileID.Grass)
                    continue;
                if (!CheckFlat(placeX, placeY, 2, 0))
                    continue;

                WorldGen.PlaceObject(placeX, placeY - 1, ModContent.TileType<HeartOfThornsTile>(), true);
                NetMessage.SendObjectPlacment(-1, placeX, placeY - 1, (ushort)ModContent.TileType<HeartOfThornsTile>(), 0, 0, -1, -1);
                if (Main.tile[placeX, placeY - 1].type != ModContent.TileType<HeartOfThornsTile>())
                    continue;
                placeX2 = placeX;
                attempts = 0;
                placed1 = true;
            }
            while (placed1 && placed2 < 30 && attempts++ < 100000)
            {
                int placeX3 = placeX2 + WorldGen.genRand.Next(-20, 20);

                int placeY = (int)Main.worldSurface - 200;
                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(placeX3, placeY) && placeY <= Main.worldSurface)
                    placeY++;
                // If we went under the world's surface, try again
                if (placeY > Main.worldSurface)
                    continue;
                Tile tile = Main.tile[placeX3, placeY];
                if (tile.type != TileID.Grass)
                    continue;
                switch (WorldGen.genRand.Next(2))
                {
                    case 0:
                        WorldGen.PlaceObject(placeX3, placeY - 1, ModContent.TileType<ThornsTile>(), true, WorldGen.genRand.Next(2));
                        NetMessage.SendObjectPlacment(-1, placeX3, placeY - 1, (ushort)ModContent.TileType<ThornsTile>(), WorldGen.genRand.Next(2), 0, -1, -1);
                        break;
                    case 1:
                        WorldGen.PlaceObject(placeX3, placeY - 1, ModContent.TileType<ThornsTile2>(), true, WorldGen.genRand.Next(2));
                        NetMessage.SendObjectPlacment(-1, placeX3, placeY - 1, (ushort)ModContent.TileType<ThornsTile>(), WorldGen.genRand.Next(2), 0, -1, -1);
                        break;
                }
                placed2++;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int ShiniesIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            int GuideIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));
            if (GuideIndex == -1)
                return;
            tasks.Insert(GuideIndex, new PassLegacy("Heart of Thorns", delegate (GenerationProgress progress, GameConfiguration configuration)
            {
                progress.Message = "Cursing the forest";
                SpawnThornSummon();
            }));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 2, new PassLegacy("Generating P L A N T", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    progress.Message = "Generating P L A N T";
                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
                    {
                        int x = Main.maxTilesX;
                        int y = Main.maxTilesY;
                        int tilesX = WorldGen.genRand.Next(0, x);
                        int tilesY = WorldGen.genRand.Next((int)(y * .05f), (int)(y * .3));
                        if (Main.tile[tilesX, tilesY].type == TileID.Dirt)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(3, 5), WorldGen.genRand.Next(4, 6), (ushort)ModContent.TileType<PlantMatterTile>());
                        }
                    }
                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 8E-05); k++)
                    {
                        int x = Main.maxTilesX;
                        int y = Main.maxTilesY;
                        int tilesX = WorldGen.genRand.Next(0, x);
                        int tilesY = WorldGen.genRand.Next((int)(y * .05f), (int)(y * .5));
                        if (Main.tile[tilesX, tilesY].type == TileID.Mud)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(3, 5), WorldGen.genRand.Next(4, 8), (ushort)ModContent.TileType<PlantMatterTile>());
                        }
                    }
                }));
                tasks.Insert(ShiniesIndex + 3, new PassLegacy("Generating Infested Stone", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    progress.Message = "Hiding Spiders";
                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 5E-05); k++)
                    {
                        int i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                        int j2 = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .8f));
                        WorldGen.OreRunner(i2, j2, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)ModContent.TileType<InfestedStoneTile>());
                    }
                }));
                tasks.Insert(ShiniesIndex + 4, new PassLegacy("Generating Dragon Fossils", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Dragon-Lead
                    progress.Message = "Generating dragon fossils";
                    Mod mod = Redemption.Instance;
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(255, 0, 0)] = ModContent.TileType<DragonLeadOre2Tile>(),
                        [new Color(150, 150, 150)] = -2, //turn into air
                        [Color.Black] = -1 //don't touch when genning
                    };
                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 2E-05); k++)
                    {
                        bool placed = false;
                        int attempts = 0;
                        while (!placed && attempts++ < 10000)
                        {
                            int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                            int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .65f), (int)(Main.maxTilesY * .8));
                            if (!WorldGen.InWorld(tilesX, tilesY))
                                continue;

                            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/DL" + (WorldGen.genRand.Next(11) + 1),
                                AssetRequestMode.ImmediateLoad).Value;

                            bool whitelist = false;
                            int stoneScore = 0;
                            int emptyScore = 0;
                            for (int x = 0; x < tex.Width; x++)
                            {
                                for (int y = 0; y < tex.Height; y++)
                                {
                                    if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.WhitelistTiles.Contains(Main.tile[tilesX + x, tilesY + y].type))
                                    {
                                        whitelist = true;
                                        break;
                                    }
                                    int type = Main.tile[tilesX + x, tilesY + y].type;
                                    if (type == TileID.Stone || type == TileID.Dirt)
                                        stoneScore++;
                                    else
                                        emptyScore++;
                                }
                            }
                            if (whitelist)
                                continue;
                            if (stoneScore < (int)(emptyScore * 1.5))
                                continue;

                            Point16 origin = new(tilesX, tilesY);
                            Main.QueueMainThreadAction(() =>
                            {
                                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                                gen.Generate(origin.X, origin.Y, true, true);
                            });
                            placed = true;
                        }
                    }
                    #endregion
                }));
                tasks.Insert(ShiniesIndex2 + 1, new PassLegacy("Portals", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Surface Portal
                    progress.Message = "Thinking with portals";
                    Mod mod = Redemption.Instance;
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(255, 0, 0)] = TileID.Dirt,
                        [new Color(0, 255, 0)] = TileID.Grass,
                        [new Color(0, 0, 255)] = TileID.Emerald,
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(0, 255, 0)] = WallID.DirtUnsafe3,
                        [new Color(0, 0, 255)] = WallID.DirtUnsafe1,
                        [new Color(0, 255, 255)] = WallID.GrassUnsafe,
                        [Color.Black] = -1
                    };

                    bool placed = false;
                    int attempts = 0;

                    while (!placed && attempts++ < 200000)
                    {
                        int placeX = WorldGen.genRand.Next(0, Main.maxTilesX);

                        int placeY = (int)Main.worldSurface - 200;

                        if (!WorldGen.InWorld(placeX, placeY) || (placeX > Main.spawnTileX - 200 && placeX < Main.spawnTileX + 200))
                            continue;
                        // We go down until we hit a solid tile or go under the world's surface
                        while (!WorldGen.SolidTile(placeX, placeY) && placeY <= Main.worldSurface)
                        {
                            placeY++;
                        }
                        // If we went under the world's surface, try again
                        if (placeY > Main.worldSurface)
                            continue;
                        Tile tile = Main.tile[placeX, placeY];
                        if (tile.type != TileID.Grass)
                            continue;
                        for (int i = -40; i <= 40; i++)
                        {
                            for (int j = -40; j <= 40; j++)
                            {
                                int type = Main.tile[placeX + i, placeY + j].type;
                                if (type == TileID.SnowBlock || type == TileID.Sand || type == ModContent.TileType<HeartOfThornsTile>())
                                    continue;
                            }
                        }
                        if (!CheckFlat(placeX, placeY, 14, 2))
                            continue;

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/NewbCave", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/NewbCaveWalls", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texClear = ModContent.Request<Texture2D>("Redemption/WorldGeneration/NewbCaveClear", AssetRequestMode.ImmediateLoad).Value;

                        Point origin = new(placeX - 34, placeY - 11);
                        Main.QueueMainThreadAction(() =>
                        {
                            TexGen genC = BaseWorldGenTex.GetTexGenerator(texClear, colorToTile);
                            genC.Generate(origin.X, origin.Y, true, true);

                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });

                        newbCavePoint = origin;
                        placed = true;
                    }
                }));
                tasks.Insert(ShiniesIndex2 + 3, new PassLegacy("Portals 2", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    progress.Message = "Thinking with portals";

                    WorldGen.PlaceObject(newbCavePoint.X + 34, newbCavePoint.Y + 10, (ushort)ModContent.TileType<AnglonPortalTile>(), true);
                    NetMessage.SendObjectPlacment(-1, newbCavePoint.X + 34, newbCavePoint.Y + 10, (ushort)ModContent.TileType<AnglonPortalTile>(), 0, 0, -1, -1);
                    WorldGen.PlaceObject(newbCavePoint.X + 34, newbCavePoint.Y + 64, (ushort)ModContent.TileType<NewbMound>(), true);
                    NetMessage.SendObjectPlacment(-1, newbCavePoint.X + 34, newbCavePoint.Y + 64, (ushort)ModContent.TileType<NewbMound>(), 0, 0, -1, -1);

                    BaseWorldGen.SmoothTiles(newbCavePoint.X, newbCavePoint.Y, newbCavePoint.X + 60, newbCavePoint.Y + 82);

                    for (int i = newbCavePoint.X; i < newbCavePoint.X + 60; i++)
                    {
                        for (int j = newbCavePoint.Y; j < newbCavePoint.Y + 30; j++)
                        {
                            WorldGen.SpreadGrass(i, j);
                        }
                    }
                    for (int i = newbCavePoint.X + 13; i < newbCavePoint.X + 53; i++)
                    {
                        for (int j = newbCavePoint.Y + 66; j < newbCavePoint.Y + 74; j++)
                        {
                            if (!Framing.GetTileSafely(i, j).IsActive)
                                WorldGen.PlaceLiquid(i, j, LiquidID.Water, 255);
                        }
                    }

                    for (int i = newbCavePoint.X; i < newbCavePoint.X + 60; i++)
                    {
                        for (int j = newbCavePoint.Y; j < newbCavePoint.Y + 82; j++)
                        {
                            WorldGen.GrowTree(i, j - 1);
                            if (Main.tile[i, j].type == TileID.Dirt && !Framing.GetTileSafely(i, j - 1).IsActive &&
                            Framing.GetTileSafely(i, j).IsActive)
                            {
                                if (WorldGen.genRand.NextBool(3))
                                {
                                    WorldGen.PlaceObject(i, j - 1, TileID.LargePiles2, true, WorldGen.genRand.Next(47, 50));
                                    NetMessage.SendObjectPlacment(-1, i, j - 1, TileID.LargePiles2, WorldGen.genRand.Next(47, 50), 0, -1, -1);
                                }
                            }
                            WorldGen.PlacePot(i, j - 1);
                        }
                    }
                    #endregion
                }));
            }

            #region Ancient Decal + Hall of Heroes
            /*tasks.Insert(ShiniesIndex + 4, new PassLegacy("Ancient House", delegate (GenerationProgress progress, GameConfiguration configuration)
            {
                AncientHouse();
            }));
            tasks.Insert(ShiniesIndex + 5, new PassLegacy("Ancient Decal", delegate (GenerationProgress progress, GameConfiguration configuration)
            {
                for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 3E-05); k++)
                {
                    bool placed = false;
                    int attempts = 0;
                    while (!placed && attempts++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .8));
                        if (!WorldGen.InWorld(tilesX + 2, tilesY + 4))
                            continue;

                        Tile tile = Framing.GetTileSafely(tilesX + 2, tilesY + 4);
                        if (tile.type != TileID.Stone && tile.type != TileID.Mud)
                            continue;

                        Point16 origin = new(tilesX, tilesY);
                        Generator.GenerateMultistructureRandom("WorldGeneration/AncientSRocksM", origin, Mod, false);
                        placed = true;
                    }
                }
                for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 3E-05); k++)
                {
                    bool placed = false;
                    int attempts = 0;
                    while (!placed && attempts++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .8));
                        if (!WorldGen.InWorld(tilesX + 2, tilesY + 7))
                            continue;

                        Tile tile = Framing.GetTileSafely(tilesX + 2, tilesY + 7);
                        if (tile.type != TileID.Stone && tile.type != TileID.Mud)
                            continue;

                        Point16 origin = new(tilesX, tilesY);
                        Generator.GenerateMultistructureRandom("WorldGeneration/AncientSPillarsM", origin, Mod, false);
                        placed = true;
                    }
                }
                for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 4E-06); k++)
                {
                    bool placed = false;
                    int attempts = 0;
                    while (!placed && attempts++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .8));
                        if (!WorldGen.InWorld(tilesX + 6, tilesY + 10))
                            continue;

                        Tile tile = Framing.GetTileSafely(tilesX + 6, tilesY + 10);
                        if (tile.type != TileID.Stone && tile.type != TileID.Mud)
                            continue;

                        Point16 dims = Point16.Zero;
                        Generator.GetDimensions("WorldGeneration/AncientSArch1", Mod, ref dims);
                        for (int x = 0; x < dims.X; x++)
                        {
                            for (int y = 0; y < dims.Y - 3; y++)
                            {
                                if (Main.tile[tilesX + x, tilesY + y].IsActive)
                                    continue;
                            }
                        }

                        Point16 origin = new(tilesX, tilesY);
                        Generator.GenerateStructure("WorldGeneration/AncientSArch1", origin, Mod, false);
                        placed = true;
                    }
                }
                for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 4E-06); k++)
                {
                    bool placed = false;
                    int attempts = 0;
                    while (!placed && attempts++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .8));
                        if (!WorldGen.InWorld(tilesX + 5, tilesY + 7))
                            continue;

                        Tile tile = Framing.GetTileSafely(tilesX + 5, tilesY + 7);
                        if (tile.type != TileID.Stone && tile.type != TileID.Mud)
                            continue;

                        Point16 dims = Point16.Zero;
                        Generator.GetDimensions("WorldGeneration/AncientSCoinPile1", Mod, ref dims);
                        for (int x = 0; x < dims.X; x++)
                        {
                            for (int y = 0; y < dims.Y - 6; y++)
                            {
                                if (Main.tile[tilesX + x, tilesY + y].IsActive)
                                    continue;
                            }
                        }

                        Point16 origin = new(tilesX, tilesY);
                        Generator.GenerateStructure("WorldGeneration/AncientSCoinPile1", origin, Mod, false);
                        placed = true;
                    }
                }
                for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 3E-06); k++)
                {
                    int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                    int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .3f), (int)(Main.maxTilesY * .8));
                    if (WorldGen.InWorld(tilesX, tilesY) && (Main.tile[tilesX, tilesY].type == TileID.Stone || Main.tile[tilesX, tilesY].type == TileID.Mud))
                    {
                        Point16 origin = new(tilesX, tilesY);
                        Generator.GenerateMultistructureRandom("WorldGeneration/AncientSHutM", origin, Mod, false);
                    }
                }
                for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 2E-06); k++)
                {
                    int tilesX = WorldGen.genRand.Next(0, Main.maxTilesX - 12);
                    int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .3f), (int)(Main.maxTilesY * .8));
                    if (WorldGen.InWorld(tilesX, tilesY) && (Main.tile[tilesX, tilesY].type == TileID.Stone || Main.tile[tilesX, tilesY].type == TileID.Mud))
                    {
                        Point16 origin = new(tilesX, tilesY);
                        Generator.GenerateStructure("WorldGeneration/AncientSBridge", origin, Mod, false);
                    }
                }
            }));
            tasks.Insert(ShiniesIndex + 10, new PassLegacy("Tied Lair", delegate (GenerationProgress progress, GameConfiguration configuration)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 100000)
                {
                    int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                    int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .3f), (int)(Main.maxTilesY * .8));
                    if (!WorldGen.InWorld(tilesX, tilesY))
                        continue;

                    Tile tile = Framing.GetTileSafely(tilesX, tilesY);
                    if (tile.type != TileID.Stone)
                        continue;

                    Point16 dims = Point16.Zero;
                    Generator.GetDimensions("WorldGeneration/TiedLair", Mod, ref dims);
                    for (int x = 0; x < dims.X; x++)
                    {
                        for (int y = 0; y < dims.Y; y++)
                        {
                            if (TileLists.WhitelistTiles.Contains(Main.tile[tilesX + x, tilesY + y].type))
                                continue;
                        }
                    }

                    Point16 origin = new(tilesX, tilesY);
                    Generator.GenerateStructure("WorldGeneration/TiedLair", origin, Mod, false);
                    placed = true;
                }
            }));
            tasks.Insert(ShiniesIndex2 + 4, new PassLegacy("???", delegate (GenerationProgress progress, GameConfiguration configuration)
            {
                HeroHall();
            }));*/
            #endregion
        }
        public static void AncientHouse()
        {
            Mod mod = Redemption.Instance;
            Point16 origin = new((int)(Main.maxTilesX * .07f), (int)(Main.maxTilesY * .45f));
            if (Main.dungeonX < Main.maxTilesX / 2)
            {
                origin = new Point16((int)(Main.maxTilesX * .93f), (int)(Main.maxTilesY * .45f));
            }
            Generator.GenerateStructure("WorldGeneration/AncientHut", origin, mod, false);
            AncientWoodChest(origin.X + 4, origin.Y + 14);
        }
        public static void AncientWoodChest(int x, int y)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<AncientWoodChestTile>(), false);

            //int[] ChestLoot2 = new int[]
            //{
            //ModContent.ItemType<AncientWoodStave>(), ModContent.ItemType<AncientWoodSword>(), ModContent.ItemType<AncientWoodBow>()
            //};
            int[] ChestLoot3 = new int[]
            {
                ModContent.ItemType<AncientWood>(),
                ModContent.ItemType<GathicStone>(),
                ModContent.ItemType<AncientDirt>()
            };
            if (PlacementSuccess >= 0)
            {
                Chest chest = Main.chest[PlacementSuccess];

                //chest.item[0].SetDefaults(ModContent.ItemType<Falcon>());
                //chest.item[0].stack = 1;

                //chest.item[1].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot2));
                //chest.item[1].stack = 1;

                chest.item[2].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot3));
                chest.item[2].stack = WorldGen.genRand.Next(20, 60);
            }
        }
        public static void HeroHall()
        {
            Mod mod = Redemption.Instance;
            Point16 origin = new((int)(Main.maxTilesX * 0.4f), (int)(Main.maxTilesY * 0.45f));
            Generator.GenerateStructure("WorldGeneration/HallOfHeroes", origin, mod, false);
        }
    }
}