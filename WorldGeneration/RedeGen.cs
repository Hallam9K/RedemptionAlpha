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

namespace Redemption.WorldGeneration
{
    public class RedeGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int ShiniesIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            int GuideIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));
            if (GuideIndex == -1)
                return;

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