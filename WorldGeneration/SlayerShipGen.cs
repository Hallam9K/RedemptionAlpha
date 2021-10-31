using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Walls;
using Terraria.Utilities;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Lore;
using Redemption.Items.Usable;
using Redemption.Tiles.Containers;
using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Tiles.Tiles;
using Terraria.WorldBuilding;
using Redemption.Base;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Redemption.WorldGeneration
{
    public class SlayerShipClear : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipClear", AssetRequestMode.ImmediateLoad).Value;
            Main.QueueMainThreadAction(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            return true;
        }
    }
    public class SlayerShip : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(0, 255, 255)] = ModContent.TileType<SlayerShipPanelTile>(),
                [new Color(255, 255, 0)] = ModContent.TileType<JunkMetalTile>(),
                [new Color(100, 100, 100)] = ModContent.TileType<MetalSupportBeamTile>(),
                [new Color(220, 255, 255)] = ModContent.TileType<HalogenLampTile>(),
                [new Color(255, 0, 255)] = ModContent.TileType<ShipGlassTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<ElectricHazardTile>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(0, 255, 0)] = ModContent.WallType<SlayerShipPanelWallTile>(),
                [new Color(255, 0, 0)] = ModContent.WallType<JunkMetalWall>(),
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShip", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipSlopes", AssetRequestMode.ImmediateLoad).Value;
            Main.QueueMainThreadAction(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, null, texSlopes);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            return true;
        }
    }
    public class SlayerShipDeco : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            WorldGen.PlaceObject(origin.X + 90, origin.Y + 23, (ushort)ModContent.TileType<SlayerChairTile>());
            NetMessage.SendObjectPlacment(-1, origin.X + 90, origin.Y + 23, (ushort)ModContent.TileType<SlayerChairTile>(), 0, 0, -1, -1);
            WorldGen.PlaceObject(origin.X + 84, origin.Y + 36, (ushort)ModContent.TileType<SlayerFabricatorTile>());
            NetMessage.SendObjectPlacment(-1, origin.X + 84, origin.Y + 36, (ushort)ModContent.TileType<SlayerFabricatorTile>(), 0, 0, -1, -1);
            ShipChest(origin.X + 45, origin.Y + 44);
            ShipChest(origin.X + 52, origin.Y + 48);
            ShipChest(origin.X + 81, origin.Y + 39);
            ShipChest(origin.X + 101, origin.Y + 39);
            ShipChest(origin.X + 53, origin.Y + 41);
            ShipChest(origin.X + 55, origin.Y + 41);
            ShipChest(origin.X + 58, origin.Y + 41);
            ShipChest(origin.X + 60, origin.Y + 41);
            ShipChest(origin.X + 108, origin.Y + 47);
            ShipChest(origin.X + 104, origin.Y + 47);
            ShipChest(origin.X + 102, origin.Y + 47);
            ShipChest(origin.X + 100, origin.Y + 47);
            return true;
        }

        public static void ShipChest(int x, int y)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<HolochestTile>(), false, 1);

            int[] HoloChestLoot = new int[]
                {   ModContent.ItemType<Datalog>(),
                    ModContent.ItemType<Datalog2>(),
                    ModContent.ItemType<Datalog3>(),
                    ModContent.ItemType<Datalog4>(),
                    ModContent.ItemType<Datalog5>(),
                    ModContent.ItemType<Datalog6>(),
                    ModContent.ItemType<Datalog7>(),
                    ModContent.ItemType<Datalog8>(),
                    ModContent.ItemType<Datalog9>(),
                    ModContent.ItemType<Datalog10>(),
                    ModContent.ItemType<Datalog11>(),
                    ModContent.ItemType<Datalog12>(),
                    ModContent.ItemType<Datalog13>(),
                    ModContent.ItemType<Datalog14>(),
                    ModContent.ItemType<Datalog15>(),
                    ModContent.ItemType<Datalog16>(),
                    ModContent.ItemType<Datalog17>(),
                    ModContent.ItemType<Datalog18>(),
                    ModContent.ItemType<Datalog19>(),
                    ModContent.ItemType<Datalog20>(),
                    ModContent.ItemType<Datalog21>(),
                    ModContent.ItemType<Datalog22>(),
                    ModContent.ItemType<Datalog24>(),
                    ModContent.ItemType<Datalog25>(),
                    ModContent.ItemType<Datalog26>(),
                    ModContent.ItemType<Datalog27>(),
                    ModContent.ItemType<Datalog28>()
            };
            int[] HoloChestLoot2 = new int[]
            {   //ModContent.ItemType<ScrapMetal>(),
                //ModContent.ItemType<AIChip>(),
                ModContent.ItemType<Capacitator>(),
                ModContent.ItemType<Plating>()
            };
            int[] HoloChestLoot3 = new int[]
            {   ModContent.ItemType<CarbonMyofibre>(),
                ModContent.ItemType<XenomiteShard>(),
            };
            if (PlacementSuccess >= 0)
            {
                Chest chest = Main.chest[PlacementSuccess];

                /*Item item0 = chest.item[0];
                UnifiedRandom genRand0 = WorldGen.genRand;
                int[] array0 = new int[]
                { ModContent.ItemType<SlayerBigRevolver>(), ModContent.ItemType<SlayerGravGun>(), ModContent.ItemType<AndroidMinion>(), ModContent.ItemType<SlayersChakram>(), ModContent.ItemType<MissileDroneCaller>() };
                item0.SetDefaults(Utils.Next(genRand0, array0), false);*/

                chest.item[1].SetDefaults(Utils.Next(WorldGen.genRand, HoloChestLoot2));
                chest.item[1].stack = WorldGen.genRand.Next(1, 3);

                chest.item[2].SetDefaults(Utils.Next(WorldGen.genRand, HoloChestLoot2));
                chest.item[2].stack = WorldGen.genRand.Next(1, 3);

                chest.item[3].SetDefaults(Utils.Next(WorldGen.genRand, HoloChestLoot3));
                chest.item[3].stack = WorldGen.genRand.Next(8, 12);

                chest.item[4].SetDefaults(Utils.Next(WorldGen.genRand, HoloChestLoot));
            }
        }
    }
}