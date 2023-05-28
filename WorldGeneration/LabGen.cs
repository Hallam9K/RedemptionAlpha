using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Walls;
using Redemption.Tiles.Containers;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Ores;
using Redemption.Tiles.Tiles;
using Terraria.WorldBuilding;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Redemption.Items.Lore;
using Redemption.Items.Usable;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Tools.PostML;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.PostML.Melee;
using System.Linq;
using Redemption.Items.Weapons.PostML.Magic;
using Redemption.Items.Weapons.PostML.Summon;

namespace Redemption.WorldGeneration
{
    public class LabClear : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AbandonedLabClear", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            return true;
        }
    }

    public class AbandonedLab : MicroBiome
    {
        public static List<int> labMainLoot;
        public static List<int> labMainLoot2;
        public static List<int> labDatalogLoot;
        public static List<int> labDatalogLoot2;
        public static int labDatalogRand;
        public static int labDatalogRand2;
        public override bool Place(Point origin, StructureMap structures)
        {
            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(0, 255, 0)] = ModContent.TileType<LabPlatingTileUnsafe>(),
                [new Color(0, 100, 0)] = ModContent.TileType<OvergrownLabPlatingTile>(),
                [new Color(0, 255, 100)] = ModContent.TileType<LabPlatingTile>(),
                [new Color(100, 100, 100)] = ModContent.TileType<MetalSupportBeamTile>(),
                [new Color(0, 255, 255)] = TileID.Chain,
                [new Color(50, 50, 50)] = TileID.Stone,
                [new Color(50, 0, 50)] = TileID.Silt,
                [new Color(90, 60, 30)] = TileID.Asphalt,
                [new Color(0, 0, 255)] = ModContent.TileType<XenomiteShardTile>(),
                [new Color(255, 255, 200)] = ModContent.TileType<UraniumTile>(),
                [new Color(255, 200, 255)] = ModContent.TileType<PlutoniumTile>(),
                [new Color(255, 100, 0)] = ModContent.TileType<SolidCoriumTile>(),
                [new Color(255, 255, 0)] = ModContent.TileType<HardenedSludgeTile>(),
                [new Color(17, 54, 17)] = ModContent.TileType<BlackHardenedSludgeTile>(),
                [new Color(120, 255, 255)] = ModContent.TileType<LabTubeTile>(),
                [new Color(255, 120, 255)] = ModContent.TileType<LabTankTile>(),
                [new Color(220, 255, 255)] = ModContent.TileType<HalogenLampTile>(),
                [new Color(255, 100, 100)] = ModContent.TileType<RedLaserTile>(),
                [new Color(100, 255, 100)] = ModContent.TileType<GreenLaserTile>(),
                [new Color(100, 0, 100)] = ModContent.TileType<ElectricHazardTile>(),
                //[new Color(200, 200, 70)] = ModContent.TileType<DangerTapeTile>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(0, 255, 0)] = ModContent.WallType<LabPlatingWallTileUnsafe>(),
                [new Color(255, 255, 0)] = ModContent.WallType<VentWallTile>(),
                [new Color(0, 0, 255)] = ModContent.WallType<HardenedSludgeWallTile>(),
                [new Color(100, 0, 0)] = ModContent.WallType<BlackHardenedSludgeWallTile>(),
                [new Color(0, 255, 255)] = ModContent.WallType<MossyLabPlatingWallTile>(),
                [new Color(255, 0, 255)] = ModContent.WallType<MossyLabWallTile>(),
                [new Color(100, 100, 0)] = ModContent.WallType<DangerTapeWallTile>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AbandonedLab", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AbandonedLabWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texLiquids = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AbandonedLabLiquids", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, texLiquids);
                gen.Generate(origin.X, origin.Y, true, true);
            });

            // Doors
            GenUtils.ObjectPlace(origin.X + 135, origin.Y + 19, (ushort)ModContent.TileType<LabDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 143, origin.Y + 19, (ushort)ModContent.TileType<LabDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 169, origin.Y + 35, (ushort)ModContent.TileType<LabDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 109, origin.Y + 35, (ushort)ModContent.TileType<LabDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 62, origin.Y + 72, (ushort)ModContent.TileType<LabDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 62, origin.Y + 84, (ushort)ModContent.TileType<LabDoorClosed>());
            // Keycard Doors
            GenUtils.ObjectPlace(origin.X + 143, origin.Y + 34, (ushort)ModContent.TileType<LabKeycardDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 196, origin.Y + 28, (ushort)ModContent.TileType<LabKeycardDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 29, origin.Y + 71, (ushort)ModContent.TileType<LabKeycardDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 29, origin.Y + 83, (ushort)ModContent.TileType<LabKeycardDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 227, origin.Y + 84, (ushort)ModContent.TileType<LabKeycardDoorClosed>());
            GenUtils.ObjectPlace(origin.X + 194, origin.Y + 130, (ushort)ModContent.TileType<LabKeycardDoorClosed>());

            GenUtils.ObjectPlace(origin.X + 156, origin.Y + 7, (ushort)ModContent.TileType<LabBossDoorTileH>());
            GenUtils.ObjectPlace(origin.X + 200, origin.Y + 56, (ushort)ModContent.TileType<LabBossDoorTile>());
            GenUtils.ObjectPlace(origin.X + 223, origin.Y + 179, (ushort)ModContent.TileType<LabBossDoorTile>());
            GenUtils.ObjectPlace(origin.X + 128, origin.Y + 103, (ushort)ModContent.TileType<LabBossDoorTile>());
            GenUtils.ObjectPlace(origin.X + 104, origin.Y + 165, (ushort)ModContent.TileType<LabBossDoorTile>());
            GenUtils.ObjectPlace(origin.X + 42, origin.Y + 165, (ushort)ModContent.TileType<LabBossDoorTile>());
            GenUtils.ObjectPlace(origin.X + 118, origin.Y + 173, (ushort)ModContent.TileType<LabBossDoorTile>());
            // Signs
            GenUtils.ObjectPlace(origin.X + 188, origin.Y + 16, (ushort)ModContent.TileType<SkullSignTile>());
            GenUtils.ObjectPlace(origin.X + 61, origin.Y + 56, (ushort)ModContent.TileType<ElectricitySignTile>());
            GenUtils.ObjectPlace(origin.X + 206, origin.Y + 55, (ushort)ModContent.TileType<BiohazardSignTile>());
            GenUtils.ObjectPlace(origin.X + 203, origin.Y + 55, (ushort)ModContent.TileType<SkullSignTile>());
            GenUtils.ObjectPlace(origin.X + 210, origin.Y + 178, (ushort)ModContent.TileType<SkullSignTile>());
            GenUtils.ObjectPlace(origin.X + 204, origin.Y + 178, (ushort)ModContent.TileType<BiohazardSignTile>());
            GenUtils.ObjectPlace(origin.X + 75, origin.Y + 137, (ushort)ModContent.TileType<ElectricitySignTile>());
            GenUtils.ObjectPlace(origin.X + 107, origin.Y + 164, (ushort)ModContent.TileType<SkullSignTile>());
            GenUtils.ObjectPlace(origin.X + 108, origin.Y + 173, (ushort)ModContent.TileType<BiohazardSignTile>());
            GenUtils.ObjectPlace(origin.X + 226, origin.Y + 128, (ushort)ModContent.TileType<SkullSignTile>());
            GenUtils.ObjectPlace(origin.X + 220, origin.Y + 117, (ushort)ModContent.TileType<RadioactiveSignTile>());
            GenUtils.ObjectPlace(origin.X + 252, origin.Y + 114, (ushort)ModContent.TileType<RadioactiveSignTile>());
            // Summons
            GenUtils.ObjectPlace(origin.X + 171, origin.Y + 21, (ushort)ModContent.TileType<JanitorEquipmentTile>());
            GenUtils.ObjectPlace(origin.X + 86, origin.Y + 121, (ushort)ModContent.TileType<WideLabConsoleTile>());
            GenUtils.ObjectPlace(origin.X + 143, origin.Y + 211, (ushort)ModContent.TileType<KariBedTile>());
            GenUtils.ObjectPlace(origin.X + 183, origin.Y + 101, (ushort)ModContent.TileType<JanitorEquipmentTile>());
            // Deco
            // Starting Rooms/Elevator
            GenUtils.ObjectPlace(origin.X + 118, origin.Y + 14, (ushort)ModContent.TileType<LabIntercomTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 121, origin.Y + 14, (ushort)ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 189, origin.Y + 13, (ushort)ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 122, origin.Y + 21, (ushort)ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 125, origin.Y + 21, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 129, origin.Y + 21, (ushort)ModContent.TileType<LabReceptionDeskTile>());
            GenUtils.ObjectPlace(origin.X + 129, origin.Y + 14, (ushort)ModContent.TileType<LabReceptionMonitorsTile>());
            GenUtils.ObjectPlace(origin.X + 139, origin.Y + 36, (ushort)ModContent.TileType<InfectedCorpse1Tile>());
            GenUtils.ObjectPlace(origin.X + 148, origin.Y + 86, (ushort)ModContent.TileType<LabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 148, origin.Y + 74, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            // Miniboss Room 1
            GenUtils.ObjectPlace(origin.X + 188, origin.Y + 21, (ushort)ModContent.TileType<LabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 151, origin.Y + 9, (ushort)ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 184, origin.Y + 9, (ushort)ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 153, origin.Y + 21, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 158, origin.Y + 21, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 182, origin.Y + 21, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 177, origin.Y + 21, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 155, origin.Y + 21, (ushort)ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 179, origin.Y + 21, (ushort)ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 145, origin.Y + 14, (ushort)ModContent.TileType<LabIntercomTile>(), 0, 1);
            // Office Room
            GenUtils.ObjectPlace(origin.X + 151, origin.Y + 37, (ushort)ModContent.TileType<LabChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 153, origin.Y + 37, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 152, origin.Y + 35, (ushort)ModContent.TileType<RadiationPillTile>());
            GenUtils.ObjectPlace(origin.X + 153, origin.Y + 35, (ushort)ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 156, origin.Y + 37, (ushort)ModContent.TileType<LabChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 158, origin.Y + 37, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 158, origin.Y + 35, (ushort)ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 161, origin.Y + 37, (ushort)ModContent.TileType<LabChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 163, origin.Y + 37, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 163, origin.Y + 35, (ushort)ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 166, origin.Y + 25, (ushort)ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 188, origin.Y + 31, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());

            GenUtils.ObjectPlace(origin.X + 166, origin.Y + 37, (ushort)ModContent.TileType<LabPhotoTile>());
            // Large Room 1
            GenUtils.ObjectPlace(origin.X + 114, origin.Y + 33, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 131, origin.Y + 32, (ushort)ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 102, origin.Y + 32, (ushort)ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 124, origin.Y + 37, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 119, origin.Y + 37, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 114, origin.Y + 37, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 96, origin.Y + 44, (ushort)ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 116, origin.Y + 37, (ushort)ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 89, origin.Y + 44, (ushort)ModContent.TileType<LabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 85, origin.Y + 44, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 82, origin.Y + 44, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 82, origin.Y + 34, (ushort)ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 78, origin.Y + 34, (ushort)ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 69, origin.Y + 34, (ushort)ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 65, origin.Y + 34, (ushort)ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 59, origin.Y + 44, (ushort)ModContent.TileType<LabChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 61, origin.Y + 44, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 60, origin.Y + 42, (ushort)ModContent.TileType<RadiationPillTile>());
            GenUtils.ObjectPlace(origin.X + 63, origin.Y + 44, (ushort)ModContent.TileType<LabChairTile>());
            GenUtils.ObjectPlace(origin.X + 66, origin.Y + 44, (ushort)ModContent.TileType<LabChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 68, origin.Y + 44, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 70, origin.Y + 44, (ushort)ModContent.TileType<LabChairTile>());
            GenUtils.ObjectPlace(origin.X + 73, origin.Y + 44, (ushort)ModContent.TileType<LabChairTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 75, origin.Y + 44, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 77, origin.Y + 44, (ushort)ModContent.TileType<LabChairTile>());
            GenUtils.ObjectPlace(origin.X + 53, origin.Y + 44, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 50, origin.Y + 34, (ushort)ModContent.TileType<LabIntercomTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 82, origin.Y + 42, (ushort)ModContent.TileType<RadiationPillTile>());
            // Server Room
            GenUtils.ObjectPlace(origin.X + 44, origin.Y + 58, (ushort)ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 44, origin.Y + 57, (ushort)ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 35, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 37, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 40, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 42, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 47, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 49, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 52, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 54, origin.Y + 58, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 45, origin.Y + 49, (ushort)ModContent.TileType<LabWallFanTile>());
            // Large Room 2
            GenUtils.ObjectPlace(origin.X + 77, origin.Y + 53, (ushort)ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 84, origin.Y + 53, (ushort)ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 80, origin.Y + 54, (ushort)ModContent.TileType<LabReceptionMonitorsTile>());
            GenUtils.ObjectPlace(origin.X + 80, origin.Y + 67, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 103, origin.Y + 64, (ushort)ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            // Stage 3 Infected Scientist Arena/Side Room
            GenUtils.ObjectPlace(origin.X + 37, origin.Y + 82, (ushort)ModContent.TileType<Stage3CorpseTile>());
            GenUtils.ObjectPlace(origin.X + 67, origin.Y + 86, (ushort)ModContent.TileType<XeniumRefineryTile>());
            GenUtils.ObjectPlace(origin.X + 74, origin.Y + 86, (ushort)ModContent.TileType<XeniumRefineryTile>());
            GenUtils.ObjectPlace(origin.X + 81, origin.Y + 86, (ushort)ModContent.TileType<XeniumRefineryTile>());
            GenUtils.ObjectPlace(origin.X + 127, origin.Y + 83, (ushort)ModContent.TileType<OpenVentTile>());
            // Behemoth Arena
            GenUtils.ObjectPlace(origin.X + 163, origin.Y + 55, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 187, origin.Y + 53, (ushort)ModContent.TileType<OpenVentTile>());
            // Overgrown Rooms
            GenUtils.ObjectPlace(origin.X + 243, origin.Y + 55, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 236, origin.Y + 101, (ushort)ModContent.TileType<InfectedCorpse3Tile>());
            //GenUtils.ObjectPlace(origin.X + 244, origin.Y + 87, (ushort)ModContent.TileType<BotanistStationTile>());
            GenUtils.ObjectPlace(origin.X + 247, origin.Y + 82, (ushort)ModContent.TileType<OpenVentTile>());
            GenUtils.ObjectPlace(origin.X + 249, origin.Y + 58, (ushort)ModContent.TileType<MossyLabTableTile>());
            GenUtils.ObjectPlace(origin.X + 251, origin.Y + 58, (ushort)ModContent.TileType<LabChairTile>());
            GenUtils.ObjectPlace(origin.X + 252, origin.Y + 87, (ushort)ModContent.TileType<MossTubeTile>());
            GenUtils.ObjectPlace(origin.X + 279, origin.Y + 87, (ushort)ModContent.TileType<MossTubeTile>(), 1);
            GenUtils.ObjectPlace(origin.X + 241, origin.Y + 73, (ushort)ModContent.TileType<MossTubeTile>(), 1);
            // Uranium Rooms
            GenUtils.ObjectPlace(origin.X + 188, origin.Y + 70, (ushort)ModContent.TileType<HazmatCorpseTile>());
            GenUtils.ObjectPlace(origin.X + 154, origin.Y + 100, (ushort)ModContent.TileType<XeniumSmelterTile>());
            GenUtils.ObjectPlace(origin.X + 183, origin.Y + 117, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 183, origin.Y + 126, (ushort)ModContent.TileType<LabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 188, origin.Y + 114, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 179, origin.Y + 117, (ushort)ModContent.TileType<GirusCorruptorTile>());
            // Reactor Room
            GenUtils.ObjectPlace(origin.X + 235, origin.Y + 117, (ushort)ModContent.TileType<LabReactorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 270, origin.Y + 117, (ushort)ModContent.TileType<LabReactorTile>());
            GenUtils.ObjectPlace(origin.X + 233, origin.Y + 126, (ushort)ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 236, origin.Y + 126, (ushort)ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 267, origin.Y + 126, (ushort)ModContent.TileType<LabWallFanTile>());
            GenUtils.ObjectPlace(origin.X + 270, origin.Y + 126, (ushort)ModContent.TileType<LabWallFanTile>());
            // Underwater Rooms
            GenUtils.ObjectPlace(origin.X + 167, origin.Y + 139, (ushort)ModContent.TileType<SewerHoleTile>());
            GenUtils.ObjectPlace(origin.X + 232, origin.Y + 131, (ushort)ModContent.TileType<SewerHoleTile>());
            GenUtils.ObjectPlace(origin.X + 233, origin.Y + 159, (ushort)ModContent.TileType<SewerHoleTile>());
            GenUtils.ObjectPlace(origin.X + 238, origin.Y + 173, (ushort)ModContent.TileType<SewerHoleTile>());
            // Blisterface Arena
            GenUtils.ObjectPlace(origin.X + 194, origin.Y + 180, (ushort)ModContent.TileType<SmallVentTile>());
            GenUtils.ObjectPlace(origin.X + 203, origin.Y + 180, (ushort)ModContent.TileType<SmallVentTile>());
            GenUtils.ObjectPlace(origin.X + 211, origin.Y + 180, (ushort)ModContent.TileType<SmallVentTile>());
            GenUtils.ObjectPlace(origin.X + 220, origin.Y + 180, (ushort)ModContent.TileType<SmallVentTile>());
            // Broken Elevator
            GenUtils.ObjectPlace(origin.X + 19, origin.Y + 74, (ushort)ModContent.TileType<InfectedCorpse2Tile>());
            GenUtils.ObjectPlace(origin.X + 4, origin.Y + 84, (ushort)ModContent.TileType<InfectedCorpse1Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 25, origin.Y + 86, (ushort)ModContent.TileType<LabReceptionCouchTile>());
            GenUtils.ObjectPlace(origin.X + 6, origin.Y + 105, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 21, origin.Y + 99, (ushort)ModContent.TileType<LabIntercomTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 23, origin.Y + 105, (ushort)ModContent.TileType<InfectedCorpse3Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 27, origin.Y + 102, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 31, origin.Y + 102, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 38, origin.Y + 105, (ushort)ModContent.TileType<HospitalBedTile>());
            GenUtils.ObjectPlace(origin.X + 40, origin.Y + 105, (ushort)ModContent.TileType<LabWorkbenchTile>());
            GenUtils.ObjectPlace(origin.X + 41, origin.Y + 104, (ushort)ModContent.TileType<RadiationPillTile>());
            GenUtils.ObjectPlace(origin.X + 6, origin.Y + 121, (ushort)ModContent.TileType<LabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 6, origin.Y + 140, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 16, origin.Y + 150, (ushort)ModContent.TileType<InfectedCorpse1Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 20, origin.Y + 151, (ushort)ModContent.TileType<InfectedCorpse3Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 24, origin.Y + 137, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 46, origin.Y + 140, (ushort)ModContent.TileType<InfectedCorpse1Tile>());
            GenUtils.ObjectPlace(origin.X + 63, origin.Y + 140, (ushort)ModContent.TileType<InfectedCorpse2Tile>());
            GenUtils.ObjectPlace(origin.X + 87, origin.Y + 140, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 89, origin.Y + 140, (ushort)ModContent.TileType<ServerCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 105, origin.Y + 140, (ushort)ModContent.TileType<InfectedCorpse3Tile>());
            GenUtils.ObjectPlace(origin.X + 126, origin.Y + 135, (ushort)ModContent.TileType<LabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 122, origin.Y + 149, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 139, origin.Y + 159, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 122, origin.Y + 167, (ushort)ModContent.TileType<BrokenLabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 27, origin.Y + 117, (ushort)ModContent.TileType<OpenVentTile>());
            // Volt Arena
            GenUtils.ObjectPlace(origin.X + 52, origin.Y + 92, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 55, origin.Y + 92, (ushort)ModContent.TileType<BotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 60, origin.Y + 92, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 65, origin.Y + 94, (ushort)ModContent.TileType<BotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 70, origin.Y + 94, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 80, origin.Y + 91, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 84, origin.Y + 91, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 87, origin.Y + 91, (ushort)ModContent.TileType<BotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 92, origin.Y + 91, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 102, origin.Y + 94, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 105, origin.Y + 94, (ushort)ModContent.TileType<BotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 113, origin.Y + 92, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 117, origin.Y + 92, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 120, origin.Y + 92, (ushort)ModContent.TileType<BotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 66, origin.Y + 109, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 70, origin.Y + 109, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 101, origin.Y + 109, (ushort)ModContent.TileType<BotHangerTile>());
            GenUtils.ObjectPlace(origin.X + 106, origin.Y + 109, (ushort)ModContent.TileType<EmptyBotHangerTile>());
            // MACE Arena
            GenUtils.ObjectPlace(origin.X + 108, origin.Y + 167, (ushort)ModContent.TileType<InfectedCorpse2Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 38, origin.Y + 165, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 113, origin.Y + 175, (ushort)ModContent.TileType<LabTableTile>());
            GenUtils.ObjectPlace(origin.X + 112, origin.Y + 173, (ushort)ModContent.TileType<RadiationPillTile>());
            GenUtils.ObjectPlace(origin.X + 113, origin.Y + 173, (ushort)ModContent.TileType<LabComputerTile>());
            GenUtils.ObjectPlace(origin.X + 43, origin.Y + 161, (ushort)ModContent.TileType<BigMaceTurretTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 103, origin.Y + 161, (ushort)ModContent.TileType<BigMaceTurretTile>());

            GenUtils.ObjectPlace(origin.X + 106, origin.Y + 156, (ushort)ModContent.TileType<LabReceptionDeskTile>());
            GenUtils.ObjectPlace(origin.X + 109, origin.Y + 156, (ushort)ModContent.TileType<LabChairTile>());
            GenUtils.ObjectPlace(origin.X + 113, origin.Y + 156, (ushort)ModContent.TileType<LabBackDoorTile>());
            GenUtils.ObjectPlace(origin.X + 105, origin.Y + 151, (ushort)ModContent.TileType<LabCeilingMonitorTile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 109, origin.Y + 151, (ushort)ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 102, origin.Y + 151, (ushort)ModContent.TileType<LabIntercomTile>(), 0, 1);
            // Patient Zero Arena
            GenUtils.ObjectPlace(origin.X + 117, origin.Y + 179, (ushort)ModContent.TileType<LabCeilingMonitorTile>());
            GenUtils.ObjectPlace(origin.X + 114, origin.Y + 187, (ushort)ModContent.TileType<HospitalBedTile>());
            GenUtils.ObjectPlace(origin.X + 173, origin.Y + 184, (ushort)ModContent.TileType<LabCabinetTile>());
            GenUtils.ObjectPlace(origin.X + 118, origin.Y + 209, (ushort)ModContent.TileType<InfectedCorpse2Tile>());
            GenUtils.ObjectPlace(origin.X + 156, origin.Y + 208, (ushort)ModContent.TileType<InfectedCorpse2Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 163, origin.Y + 209, (ushort)ModContent.TileType<InfectedCorpse2Tile>());
            GenUtils.ObjectPlace(origin.X + 127, origin.Y + 210, (ushort)ModContent.TileType<InfectedCorpse3Tile>());
            GenUtils.ObjectPlace(origin.X + 138, origin.Y + 207, (ushort)ModContent.TileType<InfectedCorpse3Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 148, origin.Y + 210, (ushort)ModContent.TileType<InfectedCorpse3Tile>(), 0, 1);
            GenUtils.ObjectPlace(origin.X + 168, origin.Y + 210, (ushort)ModContent.TileType<InfectedCorpse3Tile>());

            labMainLoot = new List<int> {
                ModContent.ItemType<GasMask>(), ModContent.ItemType<Holoshield>(), ModContent.ItemType<PrototypeAtomRifle>(), ModContent.ItemType<MiniWarhead>(), ModContent.ItemType<GravityHammer>(), ModContent.ItemType<TeslaGenerator>(), ModContent.ItemType<LightningRod>()
            };
            labMainLoot2 = new List<int> {
                ModContent.ItemType<HazmatSuit>(), ModContent.ItemType<MysteriousXenomiteFragment>(),  ModContent.ItemType<EmptyMutagen>(), ModContent.ItemType<Hacksaw>(), ModContent.ItemType<DepletedCrossbow>(), ModContent.ItemType<TeslaCoil>()
            };
            labDatalogLoot = new List<int> {
                ModContent.ItemType<FloppyDisk1>(),
                ModContent.ItemType<FloppyDisk3>(),
                ModContent.ItemType<FloppyDisk3_1>()
            };
            labDatalogLoot2 = new List<int> {
                ModContent.ItemType<FloppyDisk5>(),
                ModContent.ItemType<FloppyDisk5_1>(),
                ModContent.ItemType<FloppyDisk5_2>(),
                ModContent.ItemType<FloppyDisk5_3>()
            };
            labDatalogRand = Main.rand.Next(6);
            labDatalogRand2 = Main.rand.Next(26);
            LabChest(origin.X + 183, origin.Y + 31);
            LabChest(origin.X + 204, origin.Y + 31, 1);
            LabChest(origin.X + 199, origin.Y + 27, 2);
            LabChest(origin.X + 204, origin.Y + 27, 3);
            LabChest(origin.X + 24, origin.Y + 49, 4);
            LabChest(origin.X + 145, origin.Y + 86, 5);
            LabChest(origin.X + 167, origin.Y + 131, 6);
            LabChest(origin.X + 172, origin.Y + 131, 7);
            LabChest(origin.X + 180, origin.Y + 126, 8);
            LabChest(origin.X + 145, origin.Y + 74, 9);
            LabChest(origin.X + 77, origin.Y + 75, 10);
            LabChest(origin.X + 82, origin.Y + 75, 11);
            LabChest(origin.X + 11, origin.Y + 74, 12);
            LabChest(origin.X + 14, origin.Y + 74, 13);
            LabChest(origin.X + 9, origin.Y + 105, 14);
            LabChest(origin.X + 34, origin.Y + 105, 15);
            LabChest(origin.X + 9, origin.Y + 140, 16);
            LabChest(origin.X + 23, origin.Y + 151, 17);
            LabChest(origin.X + 67, origin.Y + 140, 18);
            LabChest(origin.X + 24, origin.Y + 163, 19);
            LabChest(origin.X + 27, origin.Y + 74, 20);
            LabChest(origin.X + 30, origin.Y + 74, 21);
            LabChest(origin.X + 243, origin.Y + 73, 22);
            LabChest(origin.X + 231, origin.Y + 101, 23);
            LabChest(origin.X + 159, origin.Y + 157, 24);
            LabChest(origin.X + 156, origin.Y + 157, 25);
            LabChest(origin.X + 184, origin.Y + 181, 26);
            LabChest(origin.X + 187, origin.Y + 181, 27);
            LabChest(origin.X + 276, origin.Y + 164, 28);
            LabChest(origin.X + 279, origin.Y + 164, 29);

            SpecialLabChest(origin.X + 151, origin.Y + 154);
            PZLabChest(origin.X + 76, origin.Y + 194);

            LabLocker(origin.X + 130, origin.Y + 37);
            LabLocker(origin.X + 27, origin.Y + 49, 1);
            LabLocker(origin.X + 91, origin.Y + 86, 2);
            LabLocker(origin.X + 100, origin.Y + 86, 3);
            LabLocker(origin.X + 122, origin.Y + 86, 4);
            LabLocker(origin.X + 122, origin.Y + 74, 5);
            LabLocker(origin.X + 167, origin.Y + 87, 6);
            LabLocker(origin.X + 198, origin.Y + 86, 7);
            LabLocker(origin.X + 202, origin.Y + 87, 8);

            int[] TileArray = { ModContent.TileType<HardenedSludgeTile>(),
                ModContent.TileType<BlackHardenedSludgeTile>(),
                TileID.Stone,
                TileID.Silt,
                TileID.Asphalt,
                ModContent.TileType<XenomiteShardTile>(),
                ModContent.TileType<UraniumTile>(),
                ModContent.TileType<PlutoniumTile>(),
                ModContent.TileType<SolidCoriumTile>(),
                ModContent.TileType<SolidCoriumTile>() };
            for (int i = origin.X; i < origin.X + 289; i++)
            {
                for (int j = origin.Y; j < origin.Y + 217; j++)
                {
                    if (TileArray.Contains(Framing.GetTileSafely(i, j).TileType) && WorldGen.InWorld(i, j))
                        BaseWorldGen.SmoothTiles(i, j, i + 1, j + 1);
                }
            }
            GenVars.structures.AddProtectedStructure(new Rectangle(origin.X, origin.Y, 289, 217));
            return true;
        }

        public static void LabChest(int x, int y, int id = 0)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<LabChestTileLocked>(), false, 1);

            int[] LabChestLoot = new int[]
            {
                ModContent.ItemType<HazmatSuit>(), ModContent.ItemType<MysteriousXenomiteFragment>(),  ModContent.ItemType<EmptyMutagen>(), ModContent.ItemType<Hacksaw>(), ModContent.ItemType<DepletedCrossbow>(), ModContent.ItemType<TeslaCoil>()
            };
            int[] LabChestLoot2 = new int[]
            {
                ModContent.ItemType<ScrapMetal>(),
                ModContent.ItemType<AIChip>(),
                ModContent.ItemType<Capacitor>(),
                ModContent.ItemType<Plating>(),
                ModContent.ItemType<RawXenium>()
            };
            int[] LabChestLoot3 = new int[]
            {
                ModContent.ItemType<XenomiteShard>(),
                ItemID.LunarOre
            };
            int[] LabChestLoot4 = new int[]
            {
                ModContent.ItemType<Uranium>(),
                ModContent.ItemType<Plutonium>()
            };
            /*int[] LabChestLoot4 = new int[]
            {
                ModContent.ItemType<TerraBombaPart1>(),
                ModContent.ItemType<TerraBombaPart2>(),
                ModContent.ItemType<TerraBombaPart3>()
            };*/
            if (PlacementSuccess >= 0)
            {
                int slot = 0;
                Chest chest = Main.chest[PlacementSuccess];

                if (labMainLoot2 == null || labMainLoot2.Count == 0)
                    chest.item[slot++].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot), false);
                else
                {
                    int ID = labMainLoot2[Main.rand.Next(0, labMainLoot2.Count)];
                    chest.item[slot++].SetDefaults(ID, false);
                    labMainLoot2.Remove(ID);
                }

                chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot2));
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 3);

                chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot3));
                chest.item[slot++].stack = WorldGen.genRand.Next(8, 18);

                if (WorldGen.genRand.NextBool(4))
                {
                    chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot4));
                    chest.item[slot++].stack = WorldGen.genRand.Next(3, 12);
                }

                if (WorldGen.genRand.NextBool(2))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<CrystalSerum>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(2, 6);
                }
                if (id >= labDatalogRand2 && labDatalogLoot2 != null && labDatalogLoot2.Count != 0)
                {
                    int ID = labDatalogLoot2[Main.rand.Next(0, labDatalogLoot2.Count)];
                    chest.item[slot++].SetDefaults(ID, false);
                    labDatalogLoot2.Remove(ID);
                }
                /*if (WorldGen.genRand.Next(4) == 0)
                {
                    chest.item[4].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot4));
                }*/
            }
        }
        public static void LabLocker(int x, int y, int id = 0)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<StandardLabLockerTile>(), false);

            int[] LabChestLoot = new int[]
            {
                ModContent.ItemType<GasMask>(), ModContent.ItemType<Holoshield>(), ModContent.ItemType<PrototypeAtomRifle>(), ModContent.ItemType<MiniWarhead>(), ModContent.ItemType<GravityHammer>(), ModContent.ItemType<TeslaGenerator>(), ModContent.ItemType<LightningRod>()
            };
            int[] LabChestLoot2 = new int[]
            {
                ModContent.ItemType<ScrapMetal>(),
                ModContent.ItemType<AIChip>(),
                ModContent.ItemType<Capacitor>(),
                ModContent.ItemType<Plating>()
            };
            int[] LabChestLoot3 = new int[]
            {
                ModContent.ItemType<CrystalSerum>(),
                ModContent.ItemType<CarbonMyofibre>(),
                ModContent.ItemType<XenomiteShard>()
            };
            if (PlacementSuccess >= 0)
            {
                int slot = 0;
                Chest chest = Main.chest[PlacementSuccess];

                if (labMainLoot == null || labMainLoot.Count == 0)
                    chest.item[slot++].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot), false);
                else
                {
                    int ID = labMainLoot[Main.rand.Next(0, labMainLoot.Count)];
                    chest.item[slot++].SetDefaults(ID, false);
                    labMainLoot.Remove(ID);
                }

                chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot2));
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 3);

                chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, LabChestLoot3));
                chest.item[slot++].stack = WorldGen.genRand.Next(8, 12);

                if (id >= labDatalogRand && labDatalogLoot != null && labDatalogLoot.Count != 0)
                {
                    int ID = labDatalogLoot[Main.rand.Next(0, labDatalogLoot.Count)];
                    chest.item[slot++].SetDefaults(ID, false);
                    labDatalogLoot.Remove(ID);
                }

                if (WorldGen.genRand.NextBool(4))
                {
                    chest.item[slot].SetDefaults(ItemID.GoldCoin);
                    chest.item[slot++].stack = WorldGen.genRand.Next(2, 5);
                }
            }
        }
        public static void SpecialLabChest(int x, int y)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<LabChestTileLocked2>(), false, 1);
            if (PlacementSuccess >= 0)
            {
                int slot = 0;
                Chest chest = Main.chest[PlacementSuccess];

                chest.item[slot++].SetDefaults(ModContent.ItemType<NanoPickaxe>());

                chest.item[slot].SetDefaults(ModContent.ItemType<RawXenium>());
                chest.item[slot++].stack = WorldGen.genRand.Next(68, 92);
                chest.item[slot].SetDefaults(ItemID.LunarOre);
                chest.item[slot++].stack = WorldGen.genRand.Next(40, 50);
            }
        }
        public static void PZLabChest(int x, int y)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<LabChestTileLocked2>(), false, 1);
            if (PlacementSuccess >= 0)
            {
                int slot = 0;
                Chest chest = Main.chest[PlacementSuccess];

                chest.item[slot++].SetDefaults(ModContent.ItemType<PZGauntlet>());
                chest.item[slot++].SetDefaults(ModContent.ItemType<SwarmerCannon>());
                chest.item[slot++].SetDefaults(ModContent.ItemType<Petridish>());
                chest.item[slot++].SetDefaults(ModContent.ItemType<PortableHoloProjector>());

                chest.item[slot].SetDefaults(ModContent.ItemType<RawXenium>());
                chest.item[slot++].stack = WorldGen.genRand.Next(140, 160);
                chest.item[slot].SetDefaults(ItemID.LunarOre);
                chest.item[slot++].stack = WorldGen.genRand.Next(140, 160);
            }
        }
    }
}
