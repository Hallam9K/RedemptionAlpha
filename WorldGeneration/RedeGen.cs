using Terraria.ModLoader;
using Terraria;
using Terraria.WorldBuilding;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ID;
using Terraria.DataStructures;
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
using Terraria.ModLoader.IO;
using System.IO;
using Redemption.NPCs.Friendly;
using Redemption.Tiles.Furniture.ElderWood;
using Redemption.Walls;
using Redemption.Tiles.Bars;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Armor.Single;
using Redemption.Items.Usable.Potions;
using Redemption.Tiles.MusicBoxes;
using Redemption.Tiles.Furniture.Archcloth;
using Redemption.NPCs.Bosses.KSIII;
using System;
using System.Threading;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.NPCs.PreHM;
using Terraria.Audio;
using Redemption.Tiles.Furniture.Lab;
using Redemption.NPCs.Minibosses.Calavia;
using SubworldLibrary;

namespace Redemption.WorldGeneration
{
    public class RedeGen : ModSystem
    {
        public static bool dragonLeadSpawn;
        public static bool cryoCrystalSpawn;
        public static bool corpseCheck;
        public static Vector2 newbCaveVector = new(-1, -1);
        public static Vector2 gathicPortalVector = new(-1, -1);
        public static Vector2 slayerShipVector = new(-1, -1);
        public static Vector2 HallOfHeroesVector = new(-1, -1);
        public static Vector2 LabVector = new(-1, -1);
        public static Vector2 BastionVector = new(-1, -1);
        public static Vector2 GoldenGatewayVector = new(-1, -1);
        public static Point16 JoShrinePoint;
        public static Point16 SpiritAssassinPoint;
        public static Point16 SpiritCommonGuardPoint;
        public static Point16 SpiritOldManPoint;
        public static Point16 HangingTiedPoint;
        public static Point16 SpiritOldLadyPoint;
        public static Point16 SpiritDruidPoint;

        public override void ClearWorld()
        {
            cryoCrystalSpawn = false;
            dragonLeadSpawn = false;
            newbCaveVector = new Vector2(-1, -1);
            gathicPortalVector = new Vector2(-1, -1);
            slayerShipVector = new Vector2(-1, -1);
            HallOfHeroesVector = new Vector2(-1, -1);
            LabVector = new Vector2(-1, -1);
            BastionVector = new Vector2(-1, -1);
            GoldenGatewayVector = new Vector2(-1, -1);
            JoShrinePoint = Point16.Zero;
            SpiritAssassinPoint = Point16.Zero;
            SpiritCommonGuardPoint = Point16.Zero;
            SpiritOldManPoint = Point16.Zero;
            HangingTiedPoint = Point16.Zero;
            SpiritOldLadyPoint = Point16.Zero;
            SpiritDruidPoint = Point16.Zero;
            corpseCheck = false;
        }
        public override void PostWorldGen()
        {
            /*for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {
                                chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<BuddingBoline>());
                                break;
                            }
                        }
                    }
                }
            }*/
            bool seaEmblemPlaced = false;
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 17 * 36)
                {
                    if (!seaEmblemPlaced || WorldGen.genRand.NextBool(4))
                    {
                        chest.item[0].SetDefaults(ModContent.ItemType<GildedSeaEmblem>());
                        seaEmblemPlaced = true;
                    }
                }
            }
            #region The Funnies
            if (RedeConfigClient.Instance.FunniSpiders)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.TileType is TileID.Stone)
                            tile.TileType = (ushort)ModContent.TileType<InfestedStoneTile>();
                    }
                }
            }
            if (RedeConfigClient.Instance.FunniWasteland)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 100000)
                {
                    int placeX = WorldGen.genRand.Next((int)(Main.maxTilesX * .2f), (int)(Main.maxTilesX * .8f));

                    int placeY = (int)Main.worldSurface - 180;

                    if (placeX > Main.spawnTileX - 200 && placeX < Main.spawnTileX + 200)
                        continue;
                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(placeX, placeY) && placeY <= Main.worldSurface)
                    {
                        placeY++;
                    }
                    // If we went under the world's surface, try again
                    if (placeY > Main.worldSurface)
                        continue;
                    Tile tile = Framing.GetTileSafely(placeX, placeY);
                    if (!TileID.Sets.Conversion.Grass[tile.TileType] && !TileID.Sets.Conversion.Sand[tile.TileType] && !TileID.Sets.Conversion.Ice[tile.TileType] && tile.TileType != TileID.SnowBlock)
                        continue;
                    if (!CheckFlat(placeX, placeY, 2, 0))
                        continue;

                    Vector2 origin = new(placeX, placeY);

                    bool fail = false;
                    for (int x = -44; x <= 44; x++)
                    {
                        for (int y = -44; y <= 44; y++)
                        {
                            Point tileToWarhead = origin.ToPoint();
                            int type = Main.tile[tileToWarhead.X + x, tileToWarhead.Y + y].TileType;
                            if (Main.tile[tileToWarhead.X + x, tileToWarhead.Y + y] != null && Main.tile[tileToWarhead.X + x, tileToWarhead.Y + y].HasTile)
                            {
                                if (Main.tileDungeon[type] || type == 88 || type == 21 || type == 26 || type == 107 || type == 108 || type == 111 || type == 226 || type == 237 || type == 221 || type == 222 || type == 223 || type == 211)
                                    fail = true;
                                if (!TileLoader.CanExplode(tileToWarhead.X + x, tileToWarhead.Y + y))
                                    fail = true;
                            }
                        }
                    }
                    if (fail)
                        continue;

                    ConversionHandler.ConvertWasteland(origin * 16, 287);
                    SoundEngine.PlaySound(CustomSounds.NukeExplosion);
                    placed = true;
                }
                RedeBossDowned.nukeDropped = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            if (RedeConfigClient.Instance.FunniAllWasteland)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.HasTile || tile.WallType != 0)
                            ConversionHandler.WastelandTileConversion(tile, i, j);

                        if (tile.HasTile)
                        {
                            if (tile.TileType != ModContent.TileType<IrradiatedCorruptGrassTile>() && tile.TileType != ModContent.TileType<IrradiatedCrimsonGrassTile>())
                                WorldGen.KillTile(i, j, true);
                            else if (Framing.GetTileSafely(i, j - 1).TileType != ModContent.TileType<IrradiatedCorruptGrassTile>() && Framing.GetTileSafely(i, j - 1).TileType != ModContent.TileType<IrradiatedCrimsonGrassTile>())
                                WorldGen.KillTile(i, j - 1, true);
                            if (j < (int)(Main.maxTilesY * .4f))
                                WorldGen.SpreadGrass(i, j, ModContent.TileType<IrradiatedDirtTile>(), ModContent.TileType<IrradiatedGrassTile>(), true);
                            ModTile tile2 = TileLoader.GetTile(Main.tile[i, j].TileType);
                            if (tile2 != null)
                            {
                                for (int v = 0; v < 5; v++)
                                    tile2.RandomUpdate(i, j);
                            }
                        }
                    }
                }
                Point16 dims = Point16.Zero;
                StructureHelper.Generator.GetDimensions("WorldGeneration/AllWastelandHouse", Mod, ref dims);
                Point16 house = new(Main.spawnTileX - (dims.X / 2), Main.spawnTileY - 15);
                StructureHelper.Generator.GenerateStructure("WorldGeneration/AllWastelandHouse", house, Mod);
                for (int i = house.X - 1; i < house.X + dims.X + 1; i++)
                {
                    for (int j = house.Y - 1; j < house.Y + dims.Y; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.HasTile && tile.TileType is TileID.Trees or TileID.VanityTreeSakura or TileID.VanityTreeYellowWillow)
                            WorldGen.KillTile(i, j, noItem: true);
                    }
                }
                int guide = NPC.FindFirstNPC(NPCID.Guide);
                if (guide != -1)
                    Main.npc[guide].active = false;
                int num = NPC.NewNPC(new EntitySource_WorldGen(), (Main.spawnTileX + 5) * 16, Main.spawnTileY * 16, ModContent.NPCType<TBotUnconscious>());
                Main.npc[num].homeTileX = Main.spawnTileX + 5;
                Main.npc[num].homeTileY = Main.spawnTileY;
                Main.npc[num].direction = 1;
                Main.npc[num].homeless = true;

                RedeBossDowned.nukeDropped = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            else if (RedeConfigClient.Instance.FunniAncient)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.HasTile)
                        {
                            if (tile.TileType is TileID.Grass)
                                tile.TileType = (ushort)ModContent.TileType<AncientGrassTile>();
                            if (tile.TileType is TileID.Dirt)
                                tile.TileType = (ushort)ModContent.TileType<AncientDirtTile>();
                            if (tile.TileType is TileID.GoldCoinPile)
                                tile.TileType = (ushort)ModContent.TileType<AncientGoldCoinPileTile>();
                            if (tile.TileType is TileID.WoodBlock)
                                tile.TileType = (ushort)ModContent.TileType<ElderWoodTile>();
                            if (TileID.Sets.Conversion.Moss[tile.TileType])
                                tile.TileType = (ushort)ModContent.TileType<GathicGladestoneTile>();
                            if (tile.TileType == TileID.Stone || tile.TileType == ModContent.TileType<InfestedStoneTile>())
                                tile.TileType = (ushort)ModContent.TileType<GathicStoneTile>();
                            if (tile.TileType is TileID.GrayBrick)
                                tile.TileType = (ushort)ModContent.TileType<GathicStoneBrickTile>();

                            if (tile.TileType != TileID.CorruptGrass && tile.TileType != TileID.CrimsonGrass && tile.TileType != TileID.JungleGrass && tile.TileType != TileID.MushroomGrass)
                                WorldGen.KillTile(i, j, true);
                            WorldGen.SpreadGrass(i, j, ModContent.TileType<AncientDirtTile>(), ModContent.TileType<AncientGrassTile>(), true);
                            ModTile tile2 = TileLoader.GetTile(Main.tile[i, j].TileType);
                            if (tile2 != null)
                            {
                                for (int v = 0; v < 20; v++)
                                    tile2.RandomUpdate(i, j);
                            }
                            if (!Framing.GetTileSafely(i, j - 1).HasTile)
                            {
                                if (WorldGen.genRand.NextBool(30))
                                {
                                    switch (WorldGen.genRand.Next(7))
                                    {
                                        default:
                                            GenUtils.ObjectPlace(i, j - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                            break;
                                        case 1:
                                            GenUtils.ObjectPlace(i, j - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                            break;
                                        case 2:
                                            GenUtils.ObjectPlace(i, j - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                            break;
                                        case 3:
                                            GenUtils.ObjectPlace(i, j - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                            break;
                                        case 4:
                                            GenUtils.ObjectPlace(i, j - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                            break;
                                        case 5:
                                            GenUtils.ObjectPlace(i, j - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                            break;
                                        case 6:
                                            GenUtils.ObjectPlace(i, j - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                            break;
                                    }
                                }
                            }
                        }
                        if (tile.WallType is WallID.DirtUnsafe or WallID.DirtUnsafe1 or WallID.DirtUnsafe2 or WallID.DirtUnsafe3 or WallID.DirtUnsafe4 or WallID.Cave6Unsafe)
                            tile.WallType = (ushort)ModContent.WallType<AncientDirtWallTile>();
                        if (tile.WallType is WallID.Wood)
                            tile.WallType = (ushort)ModContent.WallType<ElderWoodWallTile>();
                        if (tile.WallType == WallID.Stone)
                            tile.WallType = (ushort)ModContent.WallType<GathicStoneWallTile>();
                        if (tile.WallType == WallID.GrayBrick)
                            tile.WallType = (ushort)ModContent.WallType<GathicStoneBrickWallTile>();
                    }
                }
                int guide = NPC.FindFirstNPC(NPCID.Guide);
                if (guide != -1)
                    Main.npc[guide].active = false;
                int num = NPC.NewNPC(new EntitySource_WorldGen(), (Main.spawnTileX + 5) * 16, Main.spawnTileY * 16, ModContent.NPCType<Fallen>());
                Main.npc[num].homeTileX = Main.spawnTileX + 5;
                Main.npc[num].homeTileY = Main.spawnTileY;
                Main.npc[num].direction = 1;
                Main.npc[num].homeless = true;
            }
            if (RedeConfigClient.Instance.FunniJanitor)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.HasTile)
                        {
                            if (tile.TileType == ModContent.TileType<LabPlatingTileUnsafe>())
                                tile.TileType = (ushort)ModContent.TileType<LabPlatingTile>();
                            if (tile.TileType == ModContent.TileType<XenomiteShardTile>() || tile.TileType == ModContent.TileType<HardenedSludgeTile>() || tile.TileType == ModContent.TileType<BlackHardenedSludgeTile>() || tile.TileType == ModContent.TileType<InfectedCorpse1Tile>() || tile.TileType == ModContent.TileType<InfectedCorpse2Tile>() || tile.TileType == ModContent.TileType<InfectedCorpse3Tile>())
                                WorldGen.KillTile(i, j, noItem: true);

                            if (tile.TileType == ModContent.TileType<OpenVentTile>())
                            {
                                WorldGen.KillTile(i, j, noItem: true);
                                WorldGen.PlaceTile(i + 1, j, ModContent.TileType<LargeVentTile>());
                            }
                            if (tile.TileType == ModContent.TileType<BrokenLabBackDoorTile>())
                            {
                                WorldGen.KillTile(i, j, noItem: true);
                                WorldGen.PlaceTile(i + 1, j + 3, ModContent.TileType<LabBackDoorTile>());
                            }
                        }
                        if (tile.WallType == ModContent.WallType<HardenedSludgeWallTile>() || tile.WallType == ModContent.WallType<BlackHardenedSludgeWallTile>())
                            tile.WallType = (ushort)ModContent.WallType<LabPlatingWallTileUnsafe>();

                        if (tile.WallType == ModContent.WallType<LabPlatingWallTileUnsafe>())
                            tile.WallType = (ushort)ModContent.WallType<LabPlatingWallTile>();
                        if (tile.WallType == ModContent.WallType<DangerTapeWallTile>())
                            tile.WallType = (ushort)ModContent.WallType<DangerTapeWall2Tile>();

                        if (tile.TileType == ModContent.TileType<LabTubeTile>() || tile.TileType == ModContent.TileType<LabTankTile>())
                            Framing.GetTileSafely(i, j).TileColor = PaintID.CyanPaint;
                    }
                }
            }
            #endregion
        }
        public override void PostUpdateWorld()
        {
            if (SubworldSystem.Current != null)
                return;

            if (NPC.downedBoss3 && !dragonLeadSpawn && !cryoCrystalSpawn)
            {
                if (RedeWorld.alignment >= 0)
                {
                    cryoCrystalSpawn = true;
                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.GoodSkeletron");
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightBlue);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.LightBlue);

                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 0.01f); k++)
                    {
                        int i2 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                        int j2 = WorldGen.genRand.Next((int)(Main.maxTilesY * .25f), (int)(Main.maxTilesY * .7f));
                        int tileUp = Framing.GetTileSafely(i2, j2 - 1).TileType;
                        int tileDown = Framing.GetTileSafely(i2, j2 + 1).TileType;
                        int tileLeft = Framing.GetTileSafely(i2 - 1, j2).TileType;
                        int tileRight = Framing.GetTileSafely(i2 + 1, j2).TileType;
                        if (!Framing.GetTileSafely(i2, j2).HasTile &&
                            (TileID.Sets.Conversion.Ice[tileUp] || TileID.Sets.Conversion.Ice[tileDown] || TileID.Sets.Conversion.Ice[tileLeft] || TileID.Sets.Conversion.Ice[tileRight]))
                        {
                            WorldGen.PlaceObject(i2, j2, ModContent.TileType<CryoCrystalTile>(), true);
                            NetMessage.SendObjectPlacement(-1, i2, j2, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                        }
                    }

                    for (int i = 0; i < Main.maxTilesX; i++)
                    {
                        for (int j = 0; j < Main.maxTilesY; j++)
                        {
                            Tile tile = Framing.GetTileSafely(i, j);
                            if (tile.TileType == ModContent.TileType<DragonLeadOre2Tile>())
                                tile.TileType = TileID.Stone;
                        }
                    }
                }
                else
                {
                    dragonLeadSpawn = true;
                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.BadSkeletron");
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Orange);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.Orange);

                    for (int i = 0; i < Main.maxTilesX; i++)
                    {
                        for (int j = 0; j < Main.maxTilesY; j++)
                        {
                            Tile tile = Framing.GetTileSafely(i, j);
                            if (tile.TileType == ModContent.TileType<DragonLeadOre2Tile>())
                                tile.TileType = (ushort)ModContent.TileType<DragonLeadOreTile>();
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

                int placeY = (int)Main.worldSurface - 180;

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
                Tile tile = Framing.GetTileSafely(placeX, placeY);
                if (tile.TileType != TileID.Grass)
                    continue;
                if (!CheckFlat(placeX, placeY, 2, 0))
                    continue;

                WorldGen.PlaceObject(placeX, placeY - 1, ModContent.TileType<HeartOfThornsTile>(), true);
                if (Framing.GetTileSafely(placeX, placeY - 1).TileType != ModContent.TileType<HeartOfThornsTile>())
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
                Tile tile = Framing.GetTileSafely(placeX3, placeY);
                if (tile.TileType != TileID.Grass)
                    continue;
                switch (WorldGen.genRand.Next(2))
                {
                    case 0:
                        WorldGen.PlaceObject(placeX3, placeY - 1, ModContent.TileType<ThornsTile>(), true, WorldGen.genRand.Next(2));
                        NetMessage.SendObjectPlacement(-1, placeX3, placeY - 1, (ushort)ModContent.TileType<ThornsTile>(), WorldGen.genRand.Next(2), 0, -1, -1);
                        break;
                    case 1:
                        WorldGen.PlaceObject(placeX3, placeY - 1, ModContent.TileType<ThornsTile2>(), true, WorldGen.genRand.Next(2));
                        NetMessage.SendObjectPlacement(-1, placeX3, placeY - 1, (ushort)ModContent.TileType<ThornsTile>(), WorldGen.genRand.Next(2), 0, -1, -1);
                        break;
                }
                placed2++;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int ShiniesIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));

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
                        if (Framing.GetTileSafely(tilesX, tilesY).TileType == TileID.Dirt)
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
                        if (Framing.GetTileSafely(tilesX, tilesY).TileType == TileID.Mud)
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
                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 2E-05); k++)
                    {
                        bool placed = false;
                        int attempts = 0;
                        while (!placed && attempts < 10000)
                        {
                            attempts++;
                            int tilesX = WorldGen.genRand.Next(12, Main.maxTilesX - 12);
                            int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .65f), (int)(Main.maxTilesY * .8));
                            if (!WorldGen.InWorld(tilesX, tilesY))
                                continue;

                            int index = WorldGen.genRand.Next(11);
                            Point16 dims = new();
                            StructureHelper.Generator.GetMultistructureDimensions("WorldGeneration/DragonLeadM", Mod, index, ref dims);

                            bool whitelist = false;
                            int stoneScore = 0;
                            int emptyScore = 0;
                            for (int x = 0; x < dims.X; x++)
                            {
                                for (int y = 0; y < dims.Y; y++)
                                {
                                    int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                    if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type))
                                    {
                                        whitelist = true;
                                        break;
                                    }
                                    if (Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile && (type == TileID.Stone || type == TileID.Dirt))
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
                            StructureHelper.Generator.GenerateMultistructureSpecific("WorldGeneration/DragonLeadM", origin, Mod, index);

                            for (int x = 0; x < dims.X; x++)
                            {
                                for (int y = 0; y < dims.Y; y++)
                                {
                                    if (WorldGen.InWorld(tilesX + x, tilesY + y) && WorldGen.genRand.NextBool(10) && Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<DragonLeadOre2Tile>())
                                        Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = TileID.Stone;
                                }
                            }
                            placed = true;
                        }
                    }
                    #endregion
                }));
                tasks.Insert(ShiniesIndex + 5, new PassLegacy("Generating Ancient Decal", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Ancient Decal
                    progress.Message = "Carving gathic caverns";
                    int multi = RedeConfigClient.Instance.FunniAncient ? 4 : 1;
                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * (4E-06 * multi)); k++)
                    {
                        bool placed = false;
                        int attempts = 0;
                        while (!placed && attempts++ < 10000)
                        {
                            bool funni = RedeConfigClient.Instance.FunniAncient;
                            int tilesX = WorldGen.genRand.Next(50, Main.maxTilesX - 250);
                            int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * (funni ? .1f : .4f)), (int)(Main.maxTilesY * .8f));
                            if (!WorldGen.InWorld(tilesX, tilesY))
                                continue;

                            int roomNum = Main.rand.Next(2, 5);
                            bool blacklist = false;
                            int stoneScore = 0;
                            int emptyScore = 0;
                            for (int x = 0; x < 25 * roomNum; x++)
                            {
                                for (int y = 0; y < 25; y++)
                                {
                                    int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                    if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type))
                                    {
                                        blacklist = true;
                                        break;
                                    }
                                    if (Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile && (type == TileID.Stone || type == TileID.Dirt))
                                        stoneScore++;
                                    else
                                        emptyScore++;
                                }
                            }
                            if (blacklist)
                                continue;
                            if (stoneScore < (int)(emptyScore * 1.5))
                                continue;

                            Vector2 origin = new(tilesX, tilesY);
                            StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/AncientDecalR", origin.ToPoint16(), Mod);
                            for (int i = 0; i < roomNum - 2; i++)
                            {
                                origin.X += 25;
                                StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/AncientDecalM", origin.ToPoint16(), Mod);
                            }
                            origin.X += 25;
                            StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/AncientDecalL", origin.ToPoint16(), Mod);

                            for (int x = 0; x < 25 * roomNum; x++)
                            {
                                for (int y = 0; y < 25; y++)
                                {
                                    if (WorldGen.InWorld(tilesX + x, tilesY + y))
                                    {
                                        if (!Framing.GetTileSafely(tilesX + x, tilesY + y - 1).HasTile && Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile)
                                        {
                                            if (WorldGen.genRand.NextBool(8))
                                            {
                                                switch (WorldGen.genRand.Next(7))
                                                {
                                                    default:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 1:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 2:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 3:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 4:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 5:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 6:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                }
                                            }
                                        }
                                        if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<ElderWoodPlatformTile>())
                                            WorldGen.KillTile(tilesX + x, tilesY + y, true);
                                        if (WorldGen.genRand.NextBool(5) && Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<AncientGoldCoinPileTile>() && Framing.GetTileSafely(tilesX + x, tilesY + y - 1).TileType != ModContent.TileType<AncientGoldCoinPileTile>())
                                            WorldGen.KillTile(tilesX + x, tilesY + y, noItem: true);
                                    }
                                }
                            }
                            placed = true;
                        }
                    }
                    #endregion
                }));
                tasks.Insert(ShiniesIndex + 6, new PassLegacy("Generating Icy Ancient Decal", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Frozen Ancient Decal
                    progress.Message = "Carving gathic caverns 2: Frozen Edition";
                    int multi = RedeConfigClient.Instance.FunniAncient ? 2 : 1;
                    for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * (16E-07 * multi)); k++)
                    {
                        bool placed = false;
                        int attempts = 0;
                        while (!placed && attempts++ < 10000)
                        {
                            bool funni = RedeConfigClient.Instance.FunniAncient;
                            int tilesX = WorldGen.genRand.Next(60, Main.maxTilesX - 250);
                            int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * (funni ? .1f : .3f)), (int)(Main.maxTilesY * .8f));
                            if (!WorldGen.InWorld(tilesX, tilesY))
                                continue;

                            int roomNum = 1;
                            bool blacklist = false;
                            int iceScore = 0;
                            int emptyScore = 0;
                            for (int x = 0; x < 30 * roomNum; x++)
                            {
                                for (int y = 0; y < 30; y++)
                                {
                                    int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                    if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type))
                                    {
                                        blacklist = true;
                                        break;
                                    }
                                    if (Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile && (type == TileID.IceBlock || type == TileID.SnowBlock))
                                        iceScore++;
                                    else
                                        emptyScore++;
                                }
                            }
                            if (blacklist)
                                continue;
                            if (iceScore < (int)(emptyScore * 1.5))
                                continue;

                            Vector2 origin = new(tilesX, tilesY);
                            StructureHelper.Generator.GenerateStructure("WorldGeneration/IceDecalR", origin.ToPoint16(), Mod);
                            for (int i = 0; i < roomNum; i++)
                            {
                                origin.X += 30;
                                StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/IceDecalM", origin.ToPoint16(), Mod);
                            }
                            origin.X += 30;
                            StructureHelper.Generator.GenerateStructure("WorldGeneration/IceDecalL", origin.ToPoint16(), Mod);

                            for (int x = 0; x < 30 * (roomNum + 2); x++)
                            {
                                for (int y = 0; y < 30; y++)
                                {
                                    if (WorldGen.InWorld(tilesX + x, tilesY + y))
                                    {
                                        List<int> GathicTileArray = new() { ModContent.TileType<GathicStoneTile>(), ModContent.TileType<GathicStoneBrickTile>() };
                                        for (int n = 1; n < 3; n++)
                                        {
                                            bool gathic = false;
                                            bool tileUp = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x, tilesY + y - n).TileType);
                                            bool tileDown = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x, tilesY + y + n).TileType);
                                            bool tileLeft = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x - n, tilesY + y).TileType);
                                            bool tileRight = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x + n, tilesY + y).TileType);
                                            if (tileUp)
                                                gathic = true;
                                            else if (tileDown)
                                                gathic = true;
                                            else if (tileLeft)
                                                gathic = true;
                                            else if (tileRight)
                                                gathic = true;

                                            if (gathic && Main.rand.NextBool(n))
                                            {
                                                if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneTile>())
                                                    Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = (ushort)ModContent.TileType<GathicColdstoneTile>();
                                                if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneBrickTile>())
                                                    Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = (ushort)ModContent.TileType<GathicColdstoneBrickTile>();
                                            }
                                        }
                                        if (WorldGen.genRand.NextBool(3))
                                        {
                                            if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneTile>())
                                                Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = TileID.IceBlock;
                                            if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneBrickTile>())
                                                Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = TileID.IceBrick;
                                            if (Framing.GetTileSafely(tilesX + x, tilesY + y).WallType == ModContent.WallType<GathicFroststoneWallTile>())
                                                Framing.GetTileSafely(tilesX + x, tilesY + y).WallType = WallID.IceUnsafe;
                                            if (Framing.GetTileSafely(tilesX + x, tilesY + y).WallType == ModContent.WallType<GathicFroststoneBrickWallTile>())
                                                Framing.GetTileSafely(tilesX + x, tilesY + y).WallType = WallID.IceBrick;
                                        }
                                        if (!Framing.GetTileSafely(tilesX + x, tilesY + y - 1).HasTile && Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile)
                                        {
                                            if (WorldGen.genRand.NextBool(8))
                                            {
                                                switch (WorldGen.genRand.Next(7))
                                                {
                                                    default:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 1:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 2:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 3:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 4:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 5:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                    case 6:
                                                        GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            placed = true;
                        }
                    }
                    #endregion
                }));
            }
            if (ShiniesIndex2 != -1)
            {
                tasks.Add(new PassLegacy("Abandoned Lab", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    WorldGen.noTileActions = true;
                    progress.Message = "Placing the Abandoned Lab in the island which is not\nactually canonically meant to be there but that'll change in 0.9";
                    Point16 origin = new((int)(Main.maxTilesX * 0.55f), (int)(Main.maxTilesY * 0.65f));
                    WorldUtils.Gen(origin.ToPoint(), new Shapes.Rectangle(289, 217), Actions.Chain(new GenAction[]
                    {
                        new Actions.SetLiquid(0, 0)
                    }));
                    LabVector = origin.ToVector2();

                    AbandonedLab biome = new();
                    LabClear delete = new();
                    delete.Place(origin.ToPoint(), GenVars.structures);
                    biome.Place(origin.ToPoint(), GenVars.structures);
                }));
                tasks.Add(new PassLegacy("Generating Ancient Decal", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Ancient Decal Special
                    progress.Message = "Putting spirits to rest";
                    bool placed = false;
                    int attempts = 0;
                    while (!placed && attempts++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next((int)(Main.maxTilesX * .3f), (int)(Main.maxTilesX * .7f));
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .8f));
                        if (!WorldGen.InWorld(tilesX, tilesY))
                            continue;

                        int roomNum = Main.rand.Next(4, 7);
                        bool blacklist = false;
                        int stoneScore = 0;
                        int emptyScore = 0;
                        for (int x = 0; x < 25 + (25 * roomNum); x++)
                        {
                            for (int y = 0; y < 39; y++)
                            {
                                int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type) || TileID.Sets.Conversion.Sandstone[type] || !GenVars.structures.CanPlace(new Rectangle(tilesX, tilesY, x, y)))
                                {
                                    blacklist = true;
                                    break;
                                }
                                if (Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile && (type == TileID.Stone || type == TileID.Dirt))
                                    stoneScore++;
                                else
                                    emptyScore++;
                            }
                        }
                        if (blacklist)
                            continue;
                        if (stoneScore < (emptyScore * 1.5f))
                            continue;

                        Vector2 origin = new(tilesX, tilesY);
                        StructureHelper.Generator.GenerateStructure("WorldGeneration/AncientDecalRSpirit1", origin.ToPoint16(), Mod);
                        origin.X += 25;
                        origin.Y += 14;
                        for (int i = 0; i < roomNum - 2; i++)
                        {
                            origin.X += 25;
                            StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/AncientDecalM", origin.ToPoint16(), Mod);
                        }
                        origin.X += 25;
                        StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/AncientDecalL", origin.ToPoint16(), Mod);

                        for (int x = 0; x < 25 + (25 * roomNum); x++)
                        {
                            for (int y = 0; y < 39; y++)
                            {
                                if (WorldGen.InWorld(tilesX + x, tilesY + y))
                                {
                                    if (!Framing.GetTileSafely(tilesX + x, tilesY + y - 1).HasTile && Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile)
                                    {
                                        if (WorldGen.genRand.NextBool(8))
                                        {
                                            switch (WorldGen.genRand.Next(7))
                                            {
                                                default:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 1:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 2:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 3:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 4:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 5:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 6:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                            }
                                        }
                                    }
                                    if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<ElderWoodPlatformTile>())
                                        WorldGen.KillTile(tilesX + x, tilesY + y, true);
                                    if (WorldGen.genRand.NextBool(5) && Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<AncientGoldCoinPileTile>() && Framing.GetTileSafely(tilesX + x, tilesY + y - 1).TileType != ModContent.TileType<AncientGoldCoinPileTile>())
                                        WorldGen.KillTile(tilesX + x, tilesY + y, noItem: true);
                                }
                            }
                        }
                        placed = true;
                    }
                    bool placed3 = false;
                    int attempts3 = 0;
                    while (!placed3 && attempts3++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next((int)(Main.maxTilesX * .3f), (int)(Main.maxTilesX * .7f));
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .8f));
                        if (!WorldGen.InWorld(tilesX, tilesY))
                            continue;

                        int roomNum = Main.rand.Next(6, 9);
                        bool blacklist = false;
                        int stoneScore = 0;
                        int emptyScore = 0;
                        for (int x = 0; x < 25 + (25 * roomNum); x++)
                        {
                            for (int y = 0; y < 44; y++)
                            {
                                int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type) || TileID.Sets.Conversion.Sandstone[type] || !GenVars.structures.CanPlace(new Rectangle(tilesX, tilesY, x, y)))
                                {
                                    blacklist = true;
                                    break;
                                }
                                if (Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile && (type == TileID.Stone || type == TileID.Dirt))
                                    stoneScore++;
                                else
                                    emptyScore++;
                            }
                        }
                        if (blacklist)
                            continue;
                        if (stoneScore < (emptyScore * 1.5f))
                            continue;

                        Vector2 origin = new(tilesX, tilesY);
                        StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/AncientDecalR", origin.ToPoint16(), Mod);
                        for (int i = 0; i < roomNum - 2; i++)
                        {
                            origin.X += 25;
                            StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/AncientDecalM", origin.ToPoint16(), Mod);
                        }
                        origin.X += 25;
                        origin.Y -= 19;
                        StructureHelper.Generator.GenerateStructure("WorldGeneration/AncientDecalLSpirit1", origin.ToPoint16(), Mod);

                        for (int x = 0; x < 25 + (25 * roomNum); x++)
                        {
                            for (int y = 0; y < 44; y++)
                            {
                                if (WorldGen.InWorld(tilesX + x, tilesY + y))
                                {
                                    if (!Framing.GetTileSafely(tilesX + x, tilesY + y - 1).HasTile && Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile)
                                    {
                                        if (WorldGen.genRand.NextBool(8))
                                        {
                                            switch (WorldGen.genRand.Next(7))
                                            {
                                                default:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 1:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 2:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 3:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 4:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 5:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 6:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                            }
                                        }
                                    }
                                    if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<ElderWoodPlatformTile>())
                                        WorldGen.KillTile(tilesX + x, tilesY + y, true);
                                    if (WorldGen.genRand.NextBool(5) && Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<AncientGoldCoinPileTile>() && Framing.GetTileSafely(tilesX + x, tilesY + y - 1).TileType != ModContent.TileType<AncientGoldCoinPileTile>())
                                        WorldGen.KillTile(tilesX + x, tilesY + y, noItem: true);
                                }
                            }
                        }
                        placed3 = true;
                    }
                    bool placed4 = false;
                    int attempts4 = 0;
                    while (!placed4 && attempts4++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next(60, Main.maxTilesX - 250);
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .3f), (int)(Main.maxTilesY * .8f));
                        if (!WorldGen.InWorld(tilesX, tilesY))
                            continue;

                        int roomNum = Main.rand.Next(2, 4);
                        int bigRoom = roomNum / 2;
                        bool blacklist = false;
                        int stoneScore = 0;
                        int emptyScore = 0;
                        for (int x = 0; x < 30 + (30 * roomNum); x++)
                        {
                            for (int y = 0; y < 50; y++)
                            {
                                int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type) || !GenVars.structures.CanPlace(new Rectangle(tilesX, tilesY, x, y)))
                                {
                                    blacklist = true;
                                    break;
                                }
                                if (type == TileID.IceBlock || type == TileID.SnowBlock)
                                    stoneScore++;
                                else
                                    emptyScore++;
                            }
                        }
                        if (blacklist)
                            continue;
                        if (stoneScore < (emptyScore * 1.5f))
                            continue;

                        Vector2 origin = new(tilesX, tilesY);
                        StructureHelper.Generator.GenerateStructure("WorldGeneration/IceDecalR", origin.ToPoint16(), Mod);
                        for (int i = 0; i < roomNum; i++)
                        {
                            origin.X += 30;
                            if (i == bigRoom)
                            {
                                origin.Y -= 6;
                                StructureHelper.Generator.GenerateStructure("WorldGeneration/IceDecalMSpecial1", origin.ToPoint16(), Mod);
                                origin.X += 30;
                                origin.Y += 6;
                            }
                            else
                                StructureHelper.Generator.GenerateMultistructureRandom("WorldGeneration/IceDecalM", origin.ToPoint16(), Mod);
                        }
                        origin.X += 30;
                        StructureHelper.Generator.GenerateStructure("WorldGeneration/IceDecalL", origin.ToPoint16(), Mod);

                        for (int x = 0; x < 30 + (30 * roomNum); x++)
                        {
                            for (int y = 0; y < 50; y++)
                            {
                                if (WorldGen.InWorld(tilesX + x, tilesY + y))
                                {
                                    List<int> GathicTileArray = new() { ModContent.TileType<GathicStoneTile>(), ModContent.TileType<GathicStoneBrickTile>() };
                                    for (int n = 1; n < 3; n++)
                                    {
                                        bool gathic = false;
                                        bool tileUp = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x, tilesY + y - n).TileType);
                                        bool tileDown = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x, tilesY + y + n).TileType);
                                        bool tileLeft = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x - n, tilesY + y).TileType);
                                        bool tileRight = GathicTileArray.Contains(Framing.GetTileSafely(tilesX + x + n, tilesY + y).TileType);
                                        if (tileUp)
                                            gathic = true;
                                        else if (tileDown)
                                            gathic = true;
                                        else if (tileLeft)
                                            gathic = true;
                                        else if (tileRight)
                                            gathic = true;

                                        if (gathic && Main.rand.NextBool(n))
                                        {
                                            if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneTile>())
                                                Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = (ushort)ModContent.TileType<GathicColdstoneTile>();
                                            if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneBrickTile>())
                                                Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = (ushort)ModContent.TileType<GathicColdstoneBrickTile>();
                                        }
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneTile>())
                                            Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = TileID.IceBlock;
                                        if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType == ModContent.TileType<GathicFroststoneBrickTile>())
                                            Framing.GetTileSafely(tilesX + x, tilesY + y).TileType = TileID.IceBrick;
                                        if (Framing.GetTileSafely(tilesX + x, tilesY + y).WallType == ModContent.WallType<GathicFroststoneWallTile>())
                                            Framing.GetTileSafely(tilesX + x, tilesY + y).WallType = WallID.IceUnsafe;
                                        if (Framing.GetTileSafely(tilesX + x, tilesY + y).WallType == ModContent.WallType<GathicFroststoneBrickWallTile>())
                                            Framing.GetTileSafely(tilesX + x, tilesY + y).WallType = WallID.IceBrick;
                                    }
                                    if (!Framing.GetTileSafely(tilesX + x, tilesY + y - 1).HasTile && Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile)
                                    {
                                        if (WorldGen.genRand.NextBool(8))
                                        {
                                            switch (WorldGen.genRand.Next(7))
                                            {
                                                default:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 1:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 2:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 3:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 4:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 5:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 6:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        placed4 = true;
                    }
                    bool placed2 = false;
                    int attempts2 = 0;
                    while (!placed2 && attempts2++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next((int)(Main.maxTilesX * .1f), (int)(Main.maxTilesX * .9f));
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .28f), (int)(Main.maxTilesY * .38f));
                        if (!WorldGen.InWorld(tilesX, tilesY))
                            continue;

                        bool blacklist = false;
                        int stoneScore = 0;
                        int emptyScore = 0;
                        for (int x = 0; x < 53; x++)
                        {
                            for (int y = 0; y < 33; y++)
                            {
                                int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type) || !GenVars.structures.CanPlace(new Rectangle(tilesX, tilesY, x, y)))
                                {
                                    blacklist = true;
                                    break;
                                }
                                if (Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile && (type == TileID.Stone || type == TileID.Dirt))
                                    stoneScore++;
                                else
                                    emptyScore++;
                            }
                        }
                        if (blacklist)
                            continue;
                        if (stoneScore < (int)(emptyScore * 1.5))
                            continue;

                        Vector2 origin = new(tilesX, tilesY);
                        StructureHelper.Generator.GenerateStructure("WorldGeneration/CommonGuardTomb", origin.ToPoint16(), Mod);
                        for (int x = 0; x < 53; x++)
                        {
                            for (int y = 0; y < 33; y++)
                            {
                                if (WorldGen.InWorld(tilesX + x, tilesY + y))
                                {
                                    if (!Framing.GetTileSafely(tilesX + x, tilesY + y - 1).HasTile && Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile)
                                    {
                                        if (WorldGen.genRand.NextBool(8))
                                        {
                                            switch (WorldGen.genRand.Next(7))
                                            {
                                                default:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 1:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 2:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 3:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 4:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 5:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 6:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                            }
                                        }
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                        WorldGen.PlacePot(tilesX + x, tilesY + y - 1, 28, 2);
                                    if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType is TileID.Stone or TileID.Dirt)
                                        BaseWorldGen.SmoothTiles(tilesX + x, tilesY + y, tilesY + x + 1, tilesY + y + 1);
                                }
                            }
                        }
                        placed2 = true;
                    }
                    bool placed5 = false;
                    int attempts5 = 0;
                    while (!placed5 && attempts5++ < 10000)
                    {
                        int tilesX = WorldGen.genRand.Next((int)(Main.maxTilesX * .1f), (int)(Main.maxTilesX * .9f));
                        int tilesY = WorldGen.genRand.Next((int)(Main.maxTilesY * .5f), (int)(Main.maxTilesY * .8f));
                        if (!WorldGen.InWorld(tilesX, tilesY))
                            continue;

                        Point16 dims = Point16.Zero;
                        StructureHelper.Generator.GetDimensions("WorldGeneration/JungleDecalSpecial1", Mod, ref dims);

                        bool blacklist = false;
                        int stoneScore = 0;
                        int emptyScore = 0;
                        for (int x = 0; x < dims.X; x++)
                        {
                            for (int y = 0; y < dims.Y; y++)
                            {
                                int type = Framing.GetTileSafely(tilesX + x, tilesY + y).TileType;
                                if (!WorldGen.InWorld(tilesX + x, tilesY + y) || TileLists.BlacklistTiles.Contains(type) || !GenVars.structures.CanPlace(new Rectangle(tilesX, tilesY, x, y)))
                                {
                                    blacklist = true;
                                    break;
                                }
                                if (Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile && (type == TileID.Mud || type == TileID.JungleGrass))
                                    stoneScore++;
                                else
                                    emptyScore++;
                            }
                        }
                        if (blacklist)
                            continue;
                        if (stoneScore < (int)(emptyScore * 1.5))
                            continue;

                        Vector2 origin = new(tilesX, tilesY);
                        StructureHelper.Generator.GenerateStructure("WorldGeneration/JungleDecalSpecial1", origin.ToPoint16(), Mod);
                        for (int x = 0; x < dims.X; x++)
                        {
                            for (int y = 0; y < dims.Y; y++)
                            {
                                if (WorldGen.InWorld(tilesX + x, tilesY + y))
                                {
                                    if (!Framing.GetTileSafely(tilesX + x, tilesY + y - 1).HasTile && Framing.GetTileSafely(tilesX + x, tilesY + y).HasTile)
                                    {
                                        if (WorldGen.genRand.NextBool(8))
                                        {
                                            switch (WorldGen.genRand.Next(7))
                                            {
                                                default:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile1>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 1:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile2>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 2:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile3>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 3:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile4>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 4:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile5>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 5:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile6>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                                case 6:
                                                    GenUtils.ObjectPlace(tilesX + x, tilesY + y - 1, ModContent.TileType<SkeletonRemainsTile7>(), 0, WorldGen.genRand.NextBool() ? -1 : 1);
                                                    break;
                                            }
                                        }
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                        WorldGen.PlacePot(tilesX + x, tilesY + y - 1, 28, Main.rand.Next(7, 10));
                                    if (Framing.GetTileSafely(tilesX + x, tilesY + y).TileType is TileID.Mud or TileID.JungleGrass)
                                        BaseWorldGen.SmoothTiles(tilesX + x, tilesY + y, tilesY + x + 1, tilesY + y + 1);
                                }
                            }
                        }
                        placed5 = true;
                    }
                    #endregion
                }));
                tasks.Add(new PassLegacy("Ancient Decal Chests", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    for (int i = 15; i < Main.maxTilesX - 15; i++)
                    {
                        for (int j = (int)(Main.maxTilesY * .3f); j < (int)(Main.maxTilesY * .9f); j++)
                        {
                            if (!WorldGen.InWorld(i, j))
                                continue;

                            if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<PetrifiedWoodTile>())
                            {
                                bool frozen = BaseTile.GetTileCount(new Vector2(i, j), new int[] { ModContent.TileType<GathicFroststoneBrickTile>(), ModContent.TileType<GathicFroststoneTile>() }, 15) > 0;

                                if (!frozen && Main.rand.NextBool(2))
                                    WorldGen.KillTile(i, j, noItem: true);
                                else
                                {
                                    Framing.GetTileSafely(i, j).ClearTile();
                                    ElderWoodChest(i, j, frozen ? 3 : 0);
                                }
                            }
                        }
                    }
                }));
                tasks.Add(new PassLegacy("Portals", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Surface Portal
                    progress.Message = "Thinking with portals";
                    Mod mod = Redemption.Instance;
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(255, 0, 0)] = TileID.Dirt,
                        [new Color(0, 255, 0)] = TileID.Grass,
                        [new Color(0, 0, 255)] = TileID.Emerald,
                        [new Color(0, 255, 255)] = ModContent.TileType<ElderWoodTile>(),
                        [new Color(255, 0, 255)] = ModContent.TileType<AncientHallBrickTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(0, 255, 0)] = WallID.DirtUnsafe3,
                        [new Color(0, 0, 255)] = WallID.DirtUnsafe1,
                        [new Color(0, 255, 255)] = WallID.GrassUnsafe,
                        [new Color(255, 0, 0)] = ModContent.WallType<AncientHallPillarWallTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    bool placed = false;
                    int liquidAttempts = 0;
                    while (!placed)
                    {
                        int placeX = WorldGen.genRand.Next(0, Main.maxTilesX);

                        int placeY = (int)Main.worldSurface - 160;

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
                        Tile tile = Framing.GetTileSafely(placeX, placeY);
                        if (tile.TileType != TileID.Grass)
                            continue;
                        if (!CheckFlat(placeX, placeY, 10, 2))
                            continue;

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/NewbCave", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/NewbCaveWalls", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texClear = ModContent.Request<Texture2D>("Redemption/WorldGeneration/NewbCaveClear", AssetRequestMode.ImmediateLoad).Value;

                        Vector2 origin = new(placeX - 34, placeY - 11);
                        int oldX = (int)origin.X;
                        int attempts = 0;
                        while (attempts < 50000 && !GenVars.structures.CanPlace(new Rectangle((int)origin.X, (int)origin.Y, 60, 82)))
                        {
                            origin.X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                        }
                        if (oldX != origin.X)
                        {
                            placeY = (int)Main.worldSurface - 160;
                            while (!WorldGen.SolidTile(placeX, placeY) && placeY <= Main.worldSurface)
                            {
                                placeY++;
                            }
                            if (placeY > Main.worldSurface)
                                continue;
                            tile = Framing.GetTileSafely(placeX, placeY);
                            if (tile.TileType != TileID.Grass)
                                continue;
                            if (!CheckFlat(placeX, placeY, 10, 2))
                                continue;

                            origin = new(placeX - 34, placeY - 11);
                        }
                        bool whitelist = false;
                        for (int i = 0; i <= 60; i++)
                        {
                            for (int j = 0; j <= 82; j++)
                            {
                                int type = Framing.GetTileSafely((int)origin.X + i, (int)origin.Y + j).TileType;
                                if (!WorldGen.InWorld((int)origin.X + i, (int)origin.Y + j) || TileLists.BlacklistTiles.Contains(type) || type == TileID.SnowBlock || type == TileID.Sand || type == ModContent.TileType<HeartOfThornsTile>())
                                {
                                    whitelist = true;
                                    break;
                                }
                            }
                        }
                        while (liquidAttempts++ < 1000)
                        {
                            for (int i = 0; i <= 60; i++)
                            {
                                for (int j = 0; j <= 20; j++)
                                {
                                    if (Framing.GetTileSafely((int)origin.X + i, (int)origin.Y + j).LiquidAmount >= 255)
                                    {
                                        whitelist = true;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                        if (whitelist)
                            continue;

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen genC = BaseWorldGenTex.GetTexGenerator(texClear, colorToTile);
                            genC.Generate((int)origin.X, (int)origin.Y, true, true);

                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall);
                            gen.Generate((int)origin.X, (int)origin.Y, true, true);
                        });

                        newbCaveVector = origin;
                        placed = true;
                    }

                    Point originPoint = newbCaveVector.ToPoint();
                    GenUtils.ObjectPlace(originPoint.X + 34, originPoint.Y + 10, (ushort)ModContent.TileType<AnglonPortalTile>());
                    GenUtils.ObjectPlace(originPoint.X + 22, originPoint.Y + 9, (ushort)ModContent.TileType<ElderWoodWorkbenchTile>());
                    GenUtils.ObjectPlace(originPoint.X + 23, originPoint.Y + 8, (ushort)ModContent.TileType<DemonScrollTile>());
                    GenUtils.ObjectPlace(originPoint.X + 34, originPoint.Y + 64, (ushort)ModContent.TileType<NewbMound>());

                    BaseWorldGen.SmoothTiles(originPoint.X, originPoint.Y, originPoint.X + 60, originPoint.Y + 82);
                    for (int i = originPoint.X; i < originPoint.X + 60; i++)
                    {
                        for (int j = originPoint.Y; j < originPoint.Y + 82; j++)
                            WorldGen.KillTile(i, j, true);
                    }
                    for (int i = originPoint.X; i < originPoint.X + 60; i++)
                    {
                        for (int j = originPoint.Y; j < originPoint.Y + 30; j++)
                        {
                            WorldGen.SpreadGrass(i, j);
                        }
                    }
                    WorldUtils.Gen(originPoint, new Shapes.Rectangle(60, 82), Actions.Chain(new GenAction[]
                    {
                        new Actions.SetLiquid(0, 0)
                    }));
                    for (int i = originPoint.X + 13; i < originPoint.X + 53; i++)
                    {
                        for (int j = originPoint.Y + 66; j < originPoint.Y + 74; j++)
                        {
                            if (!Framing.GetTileSafely(i, j).HasTile)
                                WorldGen.PlaceLiquid(i, j, (byte)LiquidID.Water, 255);
                        }
                    }

                    for (int i = originPoint.X; i < originPoint.X + 60; i++)
                    {
                        for (int j = originPoint.Y; j < originPoint.Y + 82; j++)
                        {
                            WorldGen.GrowTree(i, j - 1);
                            if (Framing.GetTileSafely(i, j).TileType == TileID.Dirt && !Framing.GetTileSafely(i, j - 1).HasTile &&
                                Framing.GetTileSafely(i, j).HasTile)
                            {
                                if (WorldGen.genRand.NextBool(3))
                                {
                                    WorldGen.PlaceObject(i, j - 1, TileID.LargePiles2, true, WorldGen.genRand.Next(47, 50));
                                    NetMessage.SendObjectPlacement(-1, i, j - 1, TileID.LargePiles2, WorldGen.genRand.Next(47, 50), 0, -1, -1);
                                }
                            }
                            if (WorldGen.genRand.NextBool(2))
                                WorldGen.PlacePot(i, j - 1);
                        }
                    }
                    GenVars.structures.AddProtectedStructure(new Rectangle(originPoint.X, originPoint.Y, 60, 82));
                    #endregion
                }));
                tasks.Add(new PassLegacy("Jo Shrine", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Jo Shrine
                    Point16 dims = new();
                    StructureHelper.Generator.GetDimensions("WorldGeneration/JShrine", Mod, ref dims);
                    bool placed = false;
                    while (!placed)
                    {
                        int placeX = WorldGen.genRand.Next(0, Main.maxTilesX);

                        int placeY = (int)Main.worldSurface - 160;

                        if (!WorldGen.InWorld(placeX, placeY) || (placeX > Main.spawnTileX - 100 && placeX < Main.spawnTileX + 100))
                            continue;
                        // We go down until we hit a solid tile or go under the world's surface
                        while (!WorldGen.SolidTile(placeX, placeY) && placeY <= Main.worldSurface)
                        {
                            placeY++;
                        }
                        // If we went under the world's surface, try again
                        if (placeY > Main.worldSurface)
                            continue;
                        Tile tile = Framing.GetTileSafely(placeX, placeY);
                        if (tile.TileType != TileID.Grass)
                            continue;
                        if (!CheckFlat(placeX + 4, placeY, 10, 2))
                            continue;

                        Vector2 origin = new(placeX - (dims.X / 2), placeY - 13);
                        int oldX = (int)origin.X;
                        int attempts = 0;
                        while (attempts < 50000 && !GenVars.structures.CanPlace(new Rectangle((int)origin.X, (int)origin.Y, dims.X, dims.Y)))
                        {
                            origin.X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                        }
                        if (oldX != origin.X)
                        {
                            placeY = (int)Main.worldSurface - 160;
                            while (!WorldGen.SolidTile(placeX, placeY) && placeY <= Main.worldSurface)
                            {
                                placeY++;
                            }
                            if (placeY > Main.worldSurface)
                                continue;
                            tile = Framing.GetTileSafely(placeX, placeY);
                            if (tile.TileType != TileID.Grass)
                                continue;
                            if (!CheckFlat(placeX + 4, placeY, 10, 2))
                                continue;

                            origin = new(placeX - (dims.X / 2), placeY - 13);
                        }
                        bool whitelist = false;
                        for (int i = 0; i <= dims.X; i++)
                        {
                            for (int j = 0; j <= dims.Y; j++)
                            {
                                int type = Framing.GetTileSafely((int)origin.X + i, (int)origin.Y + j).TileType;
                                if (!WorldGen.InWorld((int)origin.X + i, (int)origin.Y + j) || TileLists.BlacklistTiles.Contains(type) || type == TileID.SnowBlock || type == TileID.Sand || type == ModContent.TileType<HeartOfThornsTile>() || !GenVars.structures.CanPlace(new Rectangle((int)origin.X - 10, (int)origin.Y - 10, dims.X + 10, dims.Y + 10)))
                                {
                                    whitelist = true;
                                    break;
                                }
                                if (Framing.GetTileSafely((int)origin.X + i, (int)origin.Y + j).LiquidAmount >= 255)
                                {
                                    whitelist = true;
                                    break;
                                }
                            }
                        }
                        if (whitelist)
                            continue;

                        StructureHelper.Generator.GenerateStructure("WorldGeneration/JShrine", origin.ToPoint16(), Mod);

                        JoShrinePoint = origin.ToPoint16();
                        placed = true;
                    }

                    Point originPoint = JoShrinePoint.ToPoint();

                    for (int i = originPoint.X; i < originPoint.X + dims.X; i++)
                    {
                        for (int j = originPoint.Y; j < originPoint.Y + dims.Y; j++)
                            WorldGen.KillTile(i, j, true);
                    }
                    for (int i = originPoint.X; i < originPoint.X + dims.X; i++)
                    {
                        for (int j = originPoint.Y; j < originPoint.Y + dims.Y; j++)
                            WorldGen.SpreadGrass(i, j);
                    }
                    for (int i = originPoint.X; i < originPoint.X + dims.X; i++)
                        WorldGen.KillTile(i, originPoint.Y - 1, noItem: true);

                    BaseWorldGen.SmoothTiles(originPoint.X - 1, originPoint.Y - 1, originPoint.X + dims.X + 1, originPoint.Y + dims.Y + 1);
                    WorldUtils.Gen(originPoint, new Shapes.Rectangle(dims.X, dims.Y), Actions.Chain(new GenAction[]
                    {
                        new Actions.SetLiquid(0, 0)
                    }));
                    GenVars.structures.AddProtectedStructure(new Rectangle(originPoint.X, originPoint.Y, dims.X, dims.Y));
                    #endregion
                }));
                if (!ModLoader.TryGetMod("InfernumMode", out Mod infernum))
                {
                    tasks.Add(new PassLegacy("Blazing Bastion", delegate (GenerationProgress progress, GameConfiguration configuration)
                    {
                        progress.Message = "Building Blazing Bastions";
                        Point16 origin = new(Main.maxTilesX - 332, Main.maxTilesY - 192);
                        WorldUtils.Gen(new Point(origin.X, origin.Y - 60), new Shapes.Rectangle(332, 215), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetLiquid(0, 0)
                        }));
                        BastionVector = origin.ToVector2();

                        BlazingBastion biome = new();
                        BastionClear delete = new();
                        delete.Place(origin.ToPoint(), GenVars.structures);
                        biome.Place(origin.ToPoint(), GenVars.structures);
                        WorldUtils.Gen(origin.ToPoint(), new Shapes.Rectangle(332, 68), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetLiquid(0, 0)
                        }));
                    }));
                }
                tasks.Add(new PassLegacy("Golden Gateway", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    progress.Message = "Thinking harder with portals";
                    bool placed = false;
                    while (!placed)
                    {
                        int placeX = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                        int placeY = WorldGen.genRand.Next(50, 80);
                        if (!WorldGen.InWorld(placeX, placeY))
                            continue;

                        bool whitelist = false;
                        for (int i = 0; i <= 144; i++)
                        {
                            for (int j = 0; j <= 80; j++)
                            {
                                if (Main.tile[placeX + i, placeY + j].HasTile)
                                {
                                    whitelist = true;
                                    break;
                                }
                            }
                        }
                        if (whitelist)
                            continue;

                        Point16 origin = new(placeX, placeY);

                        GatewayIsland biome = new();
                        GatewayIslandClear delete = new();
                        delete.Place(origin.ToPoint(), GenVars.structures);
                        biome.Place(origin.ToPoint(), GenVars.structures);

                        GoldenGatewayVector = origin.ToVector2();
                        placed = true;
                    }
                }));
                tasks.Add(new PassLegacy("Portals 2", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Underground Portal
                    progress.Message = "Thinking with portals";
                    Mod mod = Redemption.Instance;
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(255, 0, 0)] = ModContent.TileType<GathicStoneBrickTile>(),
                        [new Color(200, 0, 0)] = ModContent.TileType<GathicGladestoneBrickTile>(),
                        [new Color(0, 255, 0)] = ModContent.TileType<GathicStoneTile>(),
                        [new Color(0, 200, 0)] = ModContent.TileType<GathicGladestoneTile>(),
                        [new Color(0, 0, 255)] = ModContent.TileType<AncientHallBrickTile>(),
                        [new Color(100, 90, 70)] = ModContent.TileType<AncientDirtTile>(),
                        [new Color(200, 200, 50)] = ModContent.TileType<AncientGoldCoinPileTile>(),
                        [new Color(180, 180, 150)] = TileID.AmberGemspark,
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(0, 0, 255)] = ModContent.WallType<GathicStoneBrickWallTile>(),
                        [new Color(0, 0, 200)] = ModContent.WallType<GathicGladestoneBrickWallTile>(),
                        [new Color(255, 0, 0)] = ModContent.WallType<GathicStoneWallTile>(),
                        [new Color(200, 0, 0)] = ModContent.WallType<GathicGladestoneWallTile>(),
                        [new Color(0, 255, 0)] = ModContent.WallType<AncientHallPillarWallTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    bool placed = false;

                    while (!placed)
                    {
                        int placeX = WorldGen.genRand.Next((int)(Main.maxTilesX * .45f), (int)(Main.maxTilesX * .55f));

                        int placeY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .7));

                        if (!WorldGen.InWorld(placeX, placeY))
                            continue;

                        Tile tile = Framing.GetTileSafely(placeX, placeY);
                        if (tile.TileType != TileID.Stone)
                            continue;

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GathicPortal", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GathicPortalWalls", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texSlope = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GathicPortalSlopes", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texClear = ModContent.Request<Texture2D>("Redemption/WorldGeneration/GathicPortalClear", AssetRequestMode.ImmediateLoad).Value;

                        Vector2 origin = new(placeX - 51, placeY - 23);

                        bool whitelist = false;
                        for (int i = 0; i <= 98; i++)
                        {
                            for (int j = 0; j <= 47; j++)
                            {
                                int type = Framing.GetTileSafely((int)origin.X + i, (int)origin.Y + j).TileType;
                                if (!WorldGen.InWorld((int)origin.X + i, (int)origin.Y + j) || type == TileID.SnowBlock || type == TileID.WoodBlock || type == TileID.Sandstone || TileLists.BlacklistTiles.Contains(type))
                                {
                                    whitelist = true;
                                    break;
                                }
                            }
                        }
                        if (whitelist)
                            continue;

                        WorldUtils.Gen(origin.ToPoint(), new Shapes.Rectangle(98, 47), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetLiquid(0, 0)
                        }));

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen genC = BaseWorldGenTex.GetTexGenerator(texClear, colorToTile);
                            genC.Generate((int)origin.X, (int)origin.Y, true, true);

                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall, null, texSlope);
                            gen.Generate((int)origin.X, (int)origin.Y, true, true);
                        });

                        gathicPortalVector = origin;
                        placed = true;
                    }

                    Point originPoint = gathicPortalVector.ToPoint();
                    GenUtils.ObjectPlace(originPoint.X + 50, originPoint.Y + 21, (ushort)ModContent.TileType<GathuramPortalTile>());
                    GenUtils.ObjectPlace(originPoint.X + 16, originPoint.Y + 22, (ushort)ModContent.TileType<ElderWoodTableTile>());
                    GenUtils.ObjectPlace(originPoint.X + 18, originPoint.Y + 22, (ushort)ModContent.TileType<ElderWoodChairTile>());
                    GenUtils.ObjectPlace(originPoint.X + 12, originPoint.Y + 22, (ushort)ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(originPoint.X + 21, originPoint.Y + 22, (ushort)ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(originPoint.X + 79, originPoint.Y + 22, (ushort)ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(originPoint.X + 88, originPoint.Y + 22, (ushort)ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(originPoint.X + 81, originPoint.Y + 22, (ushort)ModContent.TileType<ElderWoodClockTile>());
                    GenUtils.ObjectPlace(originPoint.X + 61, originPoint.Y + 21, (ushort)ModContent.TileType<SkeletonRemainsTile1_Special>());

                    ElderWoodChest(originPoint.X + 83, originPoint.Y + 22, 2);
                    ElderWoodChest(originPoint.X + 72, originPoint.Y + 36);

                    for (int i = originPoint.X; i < originPoint.X + 98; i++)
                    {
                        for (int j = originPoint.Y; j < originPoint.Y + 47; j++)
                        {
                            switch (Framing.GetTileSafely(i, j).TileType)
                            {
                                case TileID.AmberGemspark:
                                    Framing.GetTileSafely(i, j).ClearTile();
                                    WorldGen.PlaceObject(i, j, (ushort)ModContent.TileType<GraveSteelAlloyTile>(), true);
                                    NetMessage.SendObjectPlacement(-1, i, j, (ushort)ModContent.TileType<GraveSteelAlloyTile>(), 0, 0, -1, -1);
                                    break;
                            }
                            if (WorldGen.genRand.NextBool(3))
                                WorldGen.PlacePot(i, j - 1);
                        }
                    }
                    #endregion
                }));
                tasks.Add(new PassLegacy("Ancient Hut", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Ancient Hut
                    Mod mod = Redemption.Instance;
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(255, 0, 0)] = ModContent.TileType<GathicStoneBrickTile>(),
                        [new Color(200, 0, 0)] = ModContent.TileType<GathicGladestoneBrickTile>(),
                        [new Color(0, 255, 0)] = ModContent.TileType<GathicStoneTile>(),
                        [new Color(0, 200, 0)] = ModContent.TileType<GathicGladestoneTile>(),
                        [new Color(100, 80, 80)] = ModContent.TileType<ElderWoodTile>(),
                        [new Color(100, 90, 70)] = ModContent.TileType<AncientDirtTile>(),
                        [new Color(0, 255, 255)] = TileID.AmethystGemspark,
                        [new Color(0, 0, 255)] = TileID.DiamondGemspark,
                        [new Color(180, 180, 150)] = TileID.AmberGemspark,
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(0, 0, 255)] = ModContent.WallType<GathicStoneBrickWallTile>(),
                        [new Color(0, 0, 200)] = ModContent.WallType<GathicGladestoneBrickWallTile>(),
                        [new Color(255, 0, 0)] = ModContent.WallType<GathicStoneWallTile>(),
                        [new Color(200, 0, 0)] = ModContent.WallType<GathicGladestoneWallTile>(),
                        [new Color(0, 255, 0)] = ModContent.WallType<ElderWoodWallTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    bool placed = false;
                    Point origin = Point.Zero;

                    while (!placed)
                    {
                        int placeX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);

                        int placeY = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .7));

                        if (!WorldGen.InWorld(placeX, placeY))
                            continue;

                        Tile tile = Framing.GetTileSafely(placeX, placeY);
                        if (tile.TileType != TileID.Stone)
                            continue;

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientHutTiles", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientHutWalls", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texSlope = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientHutSlopes", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texClear = ModContent.Request<Texture2D>("Redemption/WorldGeneration/AncientHutClear", AssetRequestMode.ImmediateLoad).Value;

                        origin = new(placeX - 15, placeY - 11);

                        bool whitelist = false;
                        for (int i = 0; i <= 29; i++)
                        {
                            for (int j = 0; j <= 21; j++)
                            {
                                int type = Framing.GetTileSafely(origin.X + i, origin.Y + j).TileType;
                                if (!WorldGen.InWorld(origin.X + i, origin.Y + j) || type == TileID.SnowBlock || type == TileID.WoodBlock || type == TileID.Sandstone || TileLists.BlacklistTiles.Contains(type))
                                {
                                    whitelist = true;
                                    break;
                                }
                            }
                        }
                        if (whitelist)
                            continue;

                        WorldUtils.Gen(origin, new Shapes.Rectangle(29, 21), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetLiquid(0, 0)
                        }));

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen genC = BaseWorldGenTex.GetTexGenerator(texClear, colorToTile);
                            genC.Generate(origin.X, origin.Y, true, true);

                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall, null, texSlope);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });

                        placed = true;
                    }

                    GenUtils.ObjectPlace(origin.X + 17, origin.Y + 11, (ushort)ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 26, origin.Y + 12, (ushort)ModContent.TileType<ElderWoodDoorClosed>());
                    GenUtils.ObjectPlace(origin.X + 10, origin.Y + 3, (ushort)ModContent.TileType<ElderWoodBedTile>());
                    GenUtils.ObjectPlace(origin.X + 6, origin.Y + 13, (ushort)ModContent.TileType<ElderWoodClockTile>());
                    GenUtils.ObjectPlace(origin.X + 22, origin.Y + 12, (ushort)ModContent.TileType<ElderWoodTableTile>());
                    GenUtils.ObjectPlace(origin.X + 20, origin.Y + 12, (ushort)ModContent.TileType<ElderWoodChairTile>(), 0, 1);
                    GenUtils.ObjectPlace(origin.X + 14, origin.Y + 17, (ushort)ModContent.TileType<DoppelsSwordTile>());

                    ElderWoodChest(origin.X + 4, origin.Y + 13, 1);

                    for (int i = origin.X; i < origin.X + 88; i++)
                    {
                        for (int j = origin.Y; j < origin.Y + 47; j++)
                        {
                            switch (Framing.GetTileSafely(i, j).TileType)
                            {
                                case TileID.AmberGemspark:
                                    Framing.GetTileSafely(i, j).ClearTile();
                                    WorldGen.PlaceObject(i, j, (ushort)ModContent.TileType<GraveSteelAlloyTile>(), true);
                                    NetMessage.SendObjectPlacement(-1, i, j, (ushort)ModContent.TileType<GraveSteelAlloyTile>(), 0, 0, -1, -1);
                                    break;
                                case TileID.DiamondGemspark:
                                    Framing.GetTileSafely(i, j).ClearTile();
                                    WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1, 0);
                                    WorldGen.SlopeTile(i, j, 2);
                                    break;
                                case TileID.AmethystGemspark:
                                    Framing.GetTileSafely(i, j).ClearTile();
                                    WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1, 0);
                                    break;
                            }
                            if (WorldGen.genRand.NextBool(3))
                                WorldGen.PlacePot(i, j - 1);
                        }
                    }
                    #endregion
                }));
                tasks.Add(new PassLegacy("Hall of Heroes", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Hall of Heroes
                    progress.Message = "Unearthing Halls";
                    Mod mod = Redemption.Instance;
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(255, 0, 0)] = ModContent.TileType<GathicStoneBrickTile>(),
                        [new Color(200, 0, 0)] = ModContent.TileType<GathicGladestoneBrickTile>(),
                        [new Color(0, 0, 255)] = ModContent.TileType<AncientHallBrickTile>(),
                        [new Color(100, 80, 80)] = ModContent.TileType<ElderWoodTile>(),
                        [new Color(200, 200, 50)] = ModContent.TileType<AncientGoldCoinPileTile>(),
                        [new Color(200, 200, 200)] = TileID.Cobweb,
                        [new Color(0, 255, 0)] = TileID.AmberGemspark,
                        [new Color(255, 255, 0)] = TileID.AmethystGemspark,
                        [new Color(0, 255, 255)] = TileID.DiamondGemspark,
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(0, 255, 0)] = ModContent.WallType<AncientHallPillarWallTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };
                    bool placed = false;
                    while (!placed)
                    {
                        int placeX2 = WorldGen.genRand.Next((int)(Main.maxTilesX * .4f), (int)(Main.maxTilesX * .6f));
                        int placeY2 = WorldGen.genRand.Next((int)(Main.maxTilesY * .4f), (int)(Main.maxTilesY * .5f));

                        if (!WorldGen.InWorld(placeX2, placeY2))
                            continue;

                        Tile tile = Framing.GetTileSafely(placeX2, placeY2);
                        if (tile.TileType != TileID.Stone)
                            continue;

                        Vector2 origin2 = new(placeX2 - 40, placeY2 - 27);
                        bool blacklist = false;
                        for (int i = 0; i <= 88; i++)
                        {
                            for (int j = 0; j <= 47; j++)
                            {
                                int type = Framing.GetTileSafely((int)origin2.X + i, (int)origin2.Y + j).TileType;
                                if (TileLists.BlacklistTiles.Contains(type) || type == TileID.WoodBlock || !WorldGen.InWorld(placeX2, placeY2))
                                {
                                    blacklist = true;
                                    break;
                                }
                            }
                        }
                        if (blacklist)
                            continue;

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/HallOfHeroesTiles", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/HallOfHeroesWalls", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texSlope = ModContent.Request<Texture2D>("Redemption/WorldGeneration/HallOfHeroesSlopes", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texClear = ModContent.Request<Texture2D>("Redemption/WorldGeneration/HallOfHeroesClear", AssetRequestMode.ImmediateLoad).Value;
                        WorldUtils.Gen(origin2.ToPoint(), new Shapes.Rectangle(84, 43), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetLiquid(0, 0)
                        }));
                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen genC = BaseWorldGenTex.GetTexGenerator(texClear, colorToTile);
                            genC.Generate((int)origin2.X, (int)origin2.Y, true, true);

                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall, null, texSlope);
                            gen.Generate((int)origin2.X, (int)origin2.Y, true, true);
                        });
                        HallOfHeroesVector = origin2;
                        placed = true;
                    }
                    Point HallPoint = HallOfHeroesVector.ToPoint();
                    GenUtils.ObjectPlace(HallPoint.X + 24, HallPoint.Y + 24, (ushort)ModContent.TileType<KSStatueTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 54, HallPoint.Y + 24, (ushort)ModContent.TileType<NStatueTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 43, HallPoint.Y + 20, (ushort)ModContent.TileType<JStatueTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 35, HallPoint.Y + 20, (ushort)ModContent.TileType<HKStatueTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 39, HallPoint.Y + 16, (ushort)ModContent.TileType<HallOfHeroesBoxTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 39, HallPoint.Y + 27, (ushort)ModContent.TileType<AncientAltarTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 59, HallPoint.Y + 13, (ushort)ModContent.TileType<ArchclothBannerTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 20, HallPoint.Y + 13, (ushort)ModContent.TileType<ArchclothBannerTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 49, HallPoint.Y + 13, (ushort)ModContent.TileType<ArchclothBannerTile>());
                    GenUtils.ObjectPlace(HallPoint.X + 30, HallPoint.Y + 13, (ushort)ModContent.TileType<ArchclothBannerTile>());

                    ElderWoodChest(HallPoint.X + 2, HallPoint.Y + 30);
                    ElderWoodChest(HallPoint.X + 75, HallPoint.Y + 30);

                    for (int i = HallPoint.X; i < HallPoint.X + 88; i++)
                    {
                        for (int j = HallPoint.Y; j < HallPoint.Y + 47; j++)
                        {
                            switch (Main.tile[i, j].TileType)
                            {
                                case TileID.AmberGemspark:
                                    Main.tile[i, j].ClearTile();
                                    WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1, 0);
                                    WorldGen.SlopeTile(i, j, 2);
                                    break;
                                case TileID.AmethystGemspark:
                                    Main.tile[i, j].ClearTile();
                                    WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1, 0);
                                    WorldGen.SlopeTile(i, j, 1);
                                    break;
                                case TileID.DiamondGemspark:
                                    Main.tile[i, j].ClearTile();
                                    WorldGen.PlaceTile(i, j, ModContent.TileType<ElderWoodPlatformTile>(), true, false, -1, 0);
                                    break;
                            }
                            if (WorldGen.genRand.NextBool(3))
                                WorldGen.PlacePot(i, j - 1);
                        }
                    }
                    GenVars.structures.AddProtectedStructure(new Rectangle(HallPoint.X, HallPoint.Y, 84, 43));
                    #endregion
                }));
                tasks.Add(new PassLegacy("Tied Lair", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Tied Lair
                    Mod mod = Redemption.Instance;
                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(255, 0, 0)] = ModContent.TileType<GathicStoneBrickTile>(),
                        [new Color(200, 0, 0)] = ModContent.TileType<GathicGladestoneBrickTile>(),
                        [new Color(0, 255, 0)] = ModContent.TileType<GathicStoneTile>(),
                        [new Color(0, 200, 0)] = ModContent.TileType<GathicGladestoneTile>(),
                        [new Color(0, 0, 255)] = ModContent.TileType<AncientGoldCoinPileTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Dictionary<Color, int> colorToWall = new()
                    {
                        [new Color(0, 0, 255)] = ModContent.WallType<GathicStoneBrickWallTile>(),
                        [new Color(0, 0, 200)] = ModContent.WallType<GathicGladestoneBrickWallTile>(),
                        [new Color(255, 0, 0)] = ModContent.WallType<GathicStoneWallTile>(),
                        [new Color(200, 0, 0)] = ModContent.WallType<GathicGladestoneWallTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    bool placed = false;
                    Point origin = Point.Zero;

                    while (!placed)
                    {
                        int placeX = WorldGen.genRand.Next(50, Main.maxTilesX - 50);

                        int placeY = WorldGen.genRand.Next((int)(Main.maxTilesY * .5f), (int)(Main.maxTilesY * .7));

                        if (!WorldGen.InWorld(placeX, placeY))
                            continue;

                        Tile tile = Framing.GetTileSafely(placeX, placeY);
                        if (tile.TileType != TileID.Stone)
                            continue;

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/TiedLairTiles", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWall = ModContent.Request<Texture2D>("Redemption/WorldGeneration/TiedLairWalls", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texClear = ModContent.Request<Texture2D>("Redemption/WorldGeneration/TiedLairClear", AssetRequestMode.ImmediateLoad).Value;

                        origin = new(placeX - 10, placeY - 11);

                        bool whitelist = false;
                        for (int i = 0; i <= 19; i++)
                        {
                            for (int j = 0; j <= 16; j++)
                            {
                                int type = Framing.GetTileSafely(origin.X + i, origin.Y + j).TileType;
                                if (!WorldGen.InWorld(origin.X + i, origin.Y + j) || type == TileID.SnowBlock || type == TileID.WoodBlock || type == TileID.Sandstone || TileLists.BlacklistTiles.Contains(type))
                                {
                                    whitelist = true;
                                    break;
                                }
                            }
                        }
                        if (whitelist)
                            continue;

                        WorldUtils.Gen(origin, new Shapes.Rectangle(19, 16), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetLiquid(0, 0)
                        }));

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen genC = BaseWorldGenTex.GetTexGenerator(texClear, colorToTile);
                            genC.Generate(origin.X, origin.Y, true, true);

                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWall, colorToWall);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });

                        placed = true;
                    }

                    GenUtils.ObjectPlace(origin.X + 9, origin.Y + 10, TileID.Campfire, 7);
                    GenUtils.ObjectPlace(origin.X + 9, origin.Y + 5, (ushort)ModContent.TileType<HangingTiedTile>());
                    #endregion
                }));
                tasks.Add(new PassLegacy("Slayer Ship", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    progress.Message = "Crashing Spaceships";
                    Vector2 origin = new((int)(Main.maxTilesX * 0.65f), (int)Main.worldSurface - 160);
                    if (Main.dungeonX < Main.maxTilesX / 2)
                        origin = new Vector2((int)(Main.maxTilesX * 0.35f), (int)Main.worldSurface - 160);

                    origin.Y = GetTileFloorIgnoreTree((int)origin.X, (int)origin.Y, true);
                    origin.X -= 60;
                    int attempts = 0;
                    int checkType = 0;
                    bool inSpawn = false;
                    bool failed = false;
                    while ((attempts < 50000 && !GenVars.structures.CanPlace(new Rectangle((int)origin.X, (int)origin.Y, 133, 58))) || inSpawn)
                    {
                        switch (checkType)
                        {
                            case 0:
                                if (origin.X > Main.maxTilesX - 150)
                                {
                                    attempts = 0;
                                    checkType++;
                                }
                                origin.X++;
                                origin.Y = GetTileFloorIgnoreTree((int)origin.X + 60, (int)Main.worldSurface - 160, true);
                                inSpawn = false;
                                if (origin.X > Main.spawnTileX - 300 && origin.X < Main.spawnTileX + 300)
                                    inSpawn = true;
                                else
                                    attempts++;
                                break;
                            case 1:
                                if (origin.X < 150)
                                {
                                    attempts = 0;
                                    checkType++;
                                }
                                origin.X--;
                                origin.Y = GetTileFloorIgnoreTree((int)origin.X + 60, (int)Main.worldSurface - 160, true);
                                inSpawn = false;
                                if (origin.X > Main.spawnTileX - 300 && origin.X < Main.spawnTileX + 300)
                                    inSpawn = true;
                                else
                                    attempts++;
                                break;
                            case 2:
                                origin.X = WorldGen.genRand.Next(150, Main.maxTilesX - 150);
                                origin.Y = GetTileFloorIgnoreTree((int)origin.X + 60, (int)Main.worldSurface - 160, true);
                                origin.X -= 60;
                                inSpawn = false;
                                if (origin.X > Main.spawnTileX - 300 && origin.X < Main.spawnTileX + 300)
                                    inSpawn = true;
                                else
                                    attempts++;

                                if (attempts >= 49999)
                                    failed = true;
                                break;
                        }
                    }
                    if (failed)
                    {
                        origin = new((int)(Main.maxTilesX * 0.65f), (int)Main.worldSurface - 160);
                        if (Main.dungeonX < Main.maxTilesX / 2)
                            origin = new Vector2((int)(Main.maxTilesX * 0.35f), (int)Main.worldSurface - 160);

                        origin.Y = GetTileFloorIgnoreTree((int)origin.X, (int)origin.Y, true);
                        origin.X -= 60;
                    }
                    WorldUtils.Gen(origin.ToPoint(), new Shapes.Rectangle(80, 50), Actions.Chain(new GenAction[]
                    {
                        new Actions.SetLiquid(0, 0)
                    }));
                    slayerShipVector = origin;

                    SlayerShipClear delete = new();
                    SlayerShip biome = new();
                    delete.Place(origin.ToPoint(), GenVars.structures);
                    biome.Place(origin.ToPoint(), GenVars.structures);
                }));
                tasks.Add(new PassLegacy("Heart of Thorns", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    progress.Message = "Cursing the forest";
                    SpawnThornSummon();
                }));
                tasks.Add(new PassLegacy("Final Cleanup 2", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    #region Final Cleanup 2
                    Main.tileSolid[484] = false;
                    WorldGen.FillWallHolesInArea(new Rectangle(0, 0, Main.maxTilesX, (int)Main.worldSurface));
                    progress.Message = Lang.gen[86].Value;
                    for (int j = 0; j < Main.maxTilesX; j++)
                    {
                        progress.Set((float)j / (float)Main.maxTilesX);
                        for (int k = 0; k < Main.maxTilesY; k++)
                        {
                            if (Main.tile[j, k].HasTile && !WorldGen.SolidTile(j, k + 1) && (Main.tile[j, k].TileType == 53 || Main.tile[j, k].TileType == 112 || Main.tile[j, k].TileType == 234 || Main.tile[j, k].TileType == 224 || Main.tile[j, k].TileType == 123))
                            {
                                if ((double)k < Main.worldSurface + 10.0 && !Main.tile[j, k + 1].HasTile && Main.tile[j, k + 1].WallType != 191 && !WorldGen.oceanDepths(j, k))
                                {
                                    int num = 10;
                                    int num2 = k + 1;
                                    for (int l = num2; l < num2 + 10; l++)
                                    {
                                        if (Main.tile[j, l].HasTile && Main.tile[j, l].TileType == 314)
                                        {
                                            num = 0;
                                            break;
                                        }
                                    }
                                    while (!Main.tile[j, num2].HasTile && num > 0 && num2 < Main.maxTilesY - 50)
                                    {
                                        Tile tile = Main.tile[j, num2 - 1];
                                        Tile tile2 = Main.tile[j, num2];
                                        tile.Slope = 0;
                                        tile.IsHalfBlock = false;
                                        tile2.HasTile = true;
                                        tile2.TileType = Main.tile[j, k].TileType;
                                        tile2.Slope = 0;
                                        tile2.IsHalfBlock = false;
                                        num2++;
                                        num--;
                                    }
                                    if (num == 0 && !Main.tile[j, num2].HasTile)
                                    {
                                        Tile tile = Main.tile[j, num2];
                                        switch (Main.tile[j, k].TileType)
                                        {
                                            case 53:
                                                tile.TileType = 397;
                                                tile.HasTile = true;
                                                break;
                                            case 112:
                                                tile.TileType = 398;
                                                tile.HasTile = true;
                                                break;
                                            case 234:
                                                tile.TileType = 399;
                                                tile.HasTile = true;
                                                break;
                                            case 224:
                                                tile.TileType = 147;
                                                tile.HasTile = true;
                                                break;
                                            case 123:
                                                tile.TileType = 1;
                                                tile.HasTile = true;
                                                break;
                                        }
                                    }
                                    else if (Main.tile[j, num2].HasTile && Main.tileSolid[Main.tile[j, num2].TileType] && !Main.tileSolidTop[Main.tile[j, num2].TileType])
                                    {
                                        Tile tile = Main.tile[j, num2];
                                        tile.Slope = 0;
                                        tile.IsHalfBlock = false;
                                    }
                                }
                                else if (Main.tileSolid[Main.tile[j, k + 1].TileType] && !Main.tileSolidTop[Main.tile[j, k + 1].TileType] && (Main.tile[j, k + 1].TopSlope || Main.tile[j, k + 1].IsHalfBlock))
                                {
                                    Tile tile = Main.tile[j, k + 1];
                                    tile.Slope = 0;
                                    tile.IsHalfBlock = false;
                                }
                                else
                                {
                                    switch (Main.tile[j, k].TileType)
                                    {
                                        case 53:
                                            Main.tile[j, k].TileType = 397;
                                            break;
                                        case 112:
                                            Main.tile[j, k].TileType = 398;
                                            break;
                                        case 234:
                                            Main.tile[j, k].TileType = 399;
                                            break;
                                        case 224:
                                            Main.tile[j, k].TileType = 147;
                                            break;
                                        case 123:
                                            Main.tile[j, k].TileType = 1;
                                            break;
                                    }
                                }
                                if (Main.tile[j, k - 1].TileType == 323)
                                {
                                    WorldGen.TileFrame(j, k - 1);
                                }
                            }
                            if (Main.tile[j, k].TileType == 485 || Main.tile[j, k].TileType == 187 || Main.tile[j, k].TileType == 165)
                            {
                                WorldGen.TileFrame(j, k);
                            }
                            if (Main.tile[j, k].TileType == 28)
                            {
                                WorldGen.TileFrame(j, k);
                            }
                            if (Main.tile[j, k].TileType == 26)
                            {
                                WorldGen.TileFrame(j, k);
                            }
                            if (Main.tile[j, k].TileType == 137)
                            {
                                Tile tile = Main.tile[j, k];
                                tile.Slope = 0;
                                tile.IsHalfBlock = false;
                            }
                            if (Main.tile[j, k].HasTile && TileID.Sets.Boulders[Main.tile[j, k].TileType])
                            {
                                int num3 = Main.tile[j, k].TileFrameX / 18;
                                int num4 = j;
                                num4 -= num3;
                                int num5 = Main.tile[j, k].TileFrameY / 18;
                                int num6 = k;
                                num6 -= num5;
                                bool flag = false;
                                for (int m = 0; m < 2; m++)
                                {
                                    Tile tile = Main.tile[num4 + m, num6 - 1];
                                    if (tile != null && tile.HasTile && tile.TileType == 26)
                                    {
                                        flag = true;
                                        break;
                                    }
                                    for (int n = 0; n < 2; n++)
                                    {
                                        int num7 = num4 + m;
                                        int num8 = num6 + n;
                                        Tile tile2 = Main.tile[num7, num8];
                                        tile2.HasTile = true;
                                        tile2.Slope = SlopeType.Solid;
                                        tile2.IsHalfBlock = false;
                                        tile2.TileType = Main.tile[j, k].TileType;
                                        tile2.TileFrameX = (short)(m * 18);
                                        tile2.TileFrameY = (short)(n * 18);
                                    }
                                }
                                if (flag)
                                {
                                    ushort type = 0;
                                    if (Main.tile[j, k].TileType == 484)
                                    {
                                        type = 397;
                                    }
                                    for (int num9 = 0; num9 < 2; num9++)
                                    {
                                        for (int num10 = 0; num10 < 2; num10++)
                                        {
                                            int num11 = num4 + num9;
                                            int num12 = num6 + num10;
                                            Tile tile = Main.tile[num11, num12];
                                            tile.HasTile = true;
                                            tile.Slope = SlopeType.Solid;
                                            tile.IsHalfBlock = false;
                                            tile.TileType = type;
                                            tile.TileFrameX = 0;
                                            tile.TileFrameY = 0;
                                        }
                                    }
                                }
                            }
                            if (Main.tile[j, k].TileType == 323 && Main.tile[j, k].LiquidAmount > 0)
                            {
                                WorldGen.KillTile(j, k);
                            }
                            if (Main.tile[j, k].HasTile && Main.tile[j, k].TileType == 314)
                            {
                                int num13 = 15;
                                int num14 = 1;
                                int num15 = k;
                                while (k - num15 < num13)
                                {
                                    Main.tile[j, num15].LiquidAmount = 0;
                                    num15--;
                                }
                                for (num15 = k; num15 - k < num14; num15++)
                                {
                                    Main.tile[j, num15].LiquidAmount = 0;
                                }
                            }
                            if (Main.tile[j, k].HasTile && Main.tile[j, k].TileType == 332 && !Main.tile[j, k + 1].HasTile)
                            {
                                Main.tile[j, k + 1].ClearEverything();
                                Tile tile = Main.tile[j, k + 1];
                                tile.HasTile = true;
                                Main.tile[j, k + 1].TileType = 332;
                            }
                            if (j > WorldGen.beachDistance && j < Main.maxTilesX - WorldGen.beachDistance && (double)k < Main.worldSurface && Main.tile[j, k].LiquidAmount > 0 && Main.tile[j, k].LiquidAmount < byte.MaxValue && Main.tile[j - 1, k].LiquidAmount < byte.MaxValue && Main.tile[j + 1, k].LiquidAmount < byte.MaxValue && Main.tile[j, k + 1].LiquidAmount < byte.MaxValue && !TileID.Sets.Clouds[Main.tile[j - 1, k].TileType] && !TileID.Sets.Clouds[Main.tile[j + 1, k].TileType] && !TileID.Sets.Clouds[Main.tile[j, k + 1].TileType])
                            {
                                Main.tile[j, k].LiquidAmount = 0;
                            }
                        }
                    }
                    WorldGen.noTileActions = false;
                    WorldGen.gen = false;
                    #endregion
                }));
            }
        }
        private static int GetTileFloorIgnoreTree(int x, int startY, bool solid = true)
        {
            if (!WorldGen.InWorld(x, startY)) return startY;
            for (int y = startY; y < Main.maxTilesY - 10; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile is { HasTile: true } && (!solid || Main.tileSolid[tile.TileType]) && tile.TileType != TileID.LivingWood && tile.TileType != TileID.LeafBlock) { return y; }
            }
            return Main.maxTilesY - 10;
        }
        public static void ElderWoodChest(int x, int y, int ID = 0)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<ElderWoodChestTile>(), false);

            int tome = ModContent.ItemType<Earthbind>();
            if (ID == 3)
                tome = ModContent.ItemType<Mistfall>();

            int[] ChestLoot = new int[] {
                ModContent.ItemType<RopeHook>(), ModContent.ItemType<BeardedHatchet>(), ModContent.ItemType<WeddingRing>(), ModContent.ItemType<TrappedSoulBauble>(), ModContent.ItemType<EmptyCruxCard>(), tome };
            int[] ChestLoot2 = new int[] {
                ModContent.ItemType<ZweihanderFragment1>(), ModContent.ItemType<ZweihanderFragment2>() };
            int[] ChestLoot3 = new int[] {
                ItemID.MiningPotion, ItemID.BattlePotion, ItemID.BuilderPotion, ItemID.InvisibilityPotion };
            int[] ChestLoot4 = new int[] {
                ItemID.SpelunkerPotion, ItemID.StrangeBrew, ItemID.RecallPotion, ModContent.ItemType<VendettaPotion>(), };

            if (PlacementSuccess >= 0)
            {
                int slot = 0;
                Chest chest = Main.chest[PlacementSuccess];

                if (ID == 1)
                    chest.item[slot].SetDefaults(ModContent.ItemType<BronzeWand>());
                else if (ID == 2)
                    chest.item[slot].SetDefaults(ModContent.ItemType<DeadRinger>());
                else
                    chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot));
                chest.item[slot++].stack = 1;

                if (ID == 2)
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<EmptyCruxCard>());
                    chest.item[slot++].stack = 1;
                }
                if (RedeHelper.GenChance(.05f))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<Violin>());
                    chest.item[slot++].stack = 1;
                }
                if (RedeHelper.GenChance(.6f))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<GraveSteelAlloy>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(4, 10);
                }
                if (RedeHelper.GenChance(.6f))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<ElderWood>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(5, 15);
                }
                if (RedeHelper.GenChance(.6f))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<AncientGoldCoin>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(3, 16);
                }
                if (RedeHelper.GenChance(.2f))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<Archcloth>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 2);
                }
                if (RedeHelper.GenChance(.1f))
                {
                    chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot2));
                    chest.item[slot++].stack = 1;
                }
                if (RedeHelper.GenChance(.02f))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<JollyHelm>());
                    chest.item[slot++].stack = 1;
                }
                if (RedeHelper.GenChance(.66f))
                {
                    chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot3));
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 2);
                }
                if (RedeHelper.GenChance(.33f))
                {
                    chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot4));
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 2);
                }
                chest.item[slot].SetDefaults(ItemID.SilverCoin);
                chest.item[slot++].stack = WorldGen.genRand.Next(5, 10);
            }
        }

        public override void PreUpdateWorld()
        {
            if (newbCaveVector.X != -1 && !NPC.AnyNPCs(ModContent.NPCType<AnglonPortal>()))
            {
                Vector2 anglonPortalPos = new(((newbCaveVector.X + 35) * 16) - 8, ((newbCaveVector.Y + 12) * 16) - 4);
                LabArea.SpawnNPCInWorld(anglonPortalPos, ModContent.NPCType<AnglonPortal>());
            }
            if (gathicPortalVector.X != -1)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<GathuramPortal>()))
                {
                    Vector2 gathicPortalPos = new(((gathicPortalVector.X + 51) * 16) - 8, ((gathicPortalVector.Y + 23) * 16) - 4);
                    LabArea.SpawnNPCInWorld(gathicPortalPos, ModContent.NPCType<GathuramPortal>());
                }
                if ((RedeQuest.calaviaVar is 1 or 2) && !RedeBossDowned.downedCalavia && !NPC.AnyNPCs(ModContent.NPCType<Calavia_Intro>()) && !NPC.AnyNPCs(ModContent.NPCType<Calavia>()))
                {
                    Vector2 gathicPortalPos = new((gathicPortalVector.X + 47) * 16, (gathicPortalVector.Y + 22) * 16);
                    LabArea.SpawnNPCInWorld(gathicPortalPos, ModContent.NPCType<Calavia_Intro>());
                }
            }
            if (JoShrinePoint.X != 0 && !RedeBossDowned.downedTreebark && RedeWorld.DayNightCount < 4 && !NPC.AnyNPCs(ModContent.NPCType<TreebarkDryad>()))
            {
                Vector2 shrinePos = new((JoShrinePoint.X + 9) * 16, (JoShrinePoint.Y + 13) * 16);
                LabArea.SpawnNPCInWorld(shrinePos, ModContent.NPCType<TreebarkDryad>(), 0, 1, 0, 2);
            }
            if (slayerShipVector.X != -1 && RedeBossDowned.downedSlayer && !RedeBossDowned.downedOmega3 && !RedeBossDowned.downedNebuleus && !NPC.AnyNPCs(ModContent.NPCType<KS3Sitting>()) && !NPC.AnyNPCs(ModContent.NPCType<KS3>()))
            {
                Vector2 slayerSittingPos = new((slayerShipVector.X + 92) * 16, (slayerShipVector.Y + 28) * 16);
                LabArea.SpawnNPCInWorld(slayerSittingPos, ModContent.NPCType<KS3Sitting>());
            }
            if (!corpseCheck)
                CorpseChecks();
            SpawnSpiritAssassin();
            SpawnSpiritCommonGuard();
            SpawnSpiritOldMan();
            SpawnSpiritOldLady();
            SpawnSpiritDruid();
        }
        public static void CorpseChecks()
        {
            if (SpiritAssassinPoint.X == 0)
            {
                for (int x = (int)(Main.maxTilesX * .3f); x < (int)(Main.maxTilesX * .8f); x++)
                {
                    for (int y = (int)(Main.maxTilesY * .4f); y < (int)(Main.maxTilesY * .9f); y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<SkeletonRemainsTile3_Special>())
                            continue;

                        SpiritAssassinPoint = new Point16(x, y);
                        break;
                    }
                }
            }
            if (SpiritOldManPoint.X == 0)
            {
                for (int x = (int)(Main.maxTilesX * .3f); x < (int)(Main.maxTilesX * .9f); x++)
                {
                    for (int y = (int)(Main.maxTilesY * .4f); y < (int)(Main.maxTilesY * .9f); y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<SkeletonRemainsTile5_Special>())
                            continue;

                        SpiritOldManPoint = new Point16(x, y);
                        break;
                    }
                }
            }
            if (SpiritOldLadyPoint.X == 0)
            {
                for (int x = 250; x < Main.maxTilesX - 250; x++)
                {
                    for (int y = (int)(Main.maxTilesY * .3f); y < (int)(Main.maxTilesY * .9f); y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<NiricAutomatonRemainsTile>())
                            continue;

                        SpiritOldLadyPoint = new Point16(x, y - 18);
                        break;
                    }
                }
            }
            if (SpiritCommonGuardPoint.X == 0)
            {
                for (int x = (int)(Main.maxTilesX * .1f); x < Main.maxTilesX; x++)
                {
                    for (int y = (int)(Main.maxTilesY * .28f); y < (int)(Main.maxTilesY * .4f); y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<SkeletonRemainsTile4_Special>())
                            continue;

                        SpiritCommonGuardPoint = new Point16(x, y);
                        break;
                    }
                }
            }
            if (SpiritDruidPoint.X == 0)
            {
                for (int x = (int)(Main.maxTilesX * .1f); x < Main.maxTilesX; x++)
                {
                    for (int y = (int)(Main.maxTilesY * .5f); y < (int)(Main.maxTilesY * .9f); y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<SkeletonRemainsTile7_Special>())
                            continue;

                        SpiritDruidPoint = new Point16(x, y);
                        break;
                    }
                }
            }
            if (HangingTiedPoint.X == 0)
            {
                for (int x = 50; x < Main.maxTilesX - 50; x++)
                {
                    for (int y = (int)(Main.maxTilesY * .5f); y < (int)(Main.maxTilesY * .7f); y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (!tile.HasTile || tile.TileType != ModContent.TileType<HangingTiedTile>())
                            continue;

                        HangingTiedPoint = new Point16(x, y);
                        break;
                    }
                }
            }
            corpseCheck = true;
        }
        public static void SpawnSpiritAssassin()
        {
            if (RedeWorld.spawnCleared[0] || SpiritAssassinPoint.X == 0 || !Main.LocalPlayer.ZoneRockLayerHeight || Main.LocalPlayer.DistanceSQ(SpiritAssassinPoint.ToVector2() * 16) >= 500 * 500 || NPC.AnyNPCs(ModContent.NPCType<GathicTomb_Spawner>()))
                return;

            Vector2 pos = new((SpiritAssassinPoint.X + 14) * 16, SpiritAssassinPoint.Y * 16);
            LabArea.SpawnNPCInWorld(pos, ModContent.NPCType<GathicTomb_Spawner>());
        }
        public static void SpawnSpiritOldMan()
        {
            if (RedeWorld.spawnCleared[2] || SpiritOldManPoint.X == 0 || !Main.LocalPlayer.ZoneRockLayerHeight || Main.LocalPlayer.DistanceSQ(SpiritOldManPoint.ToVector2() * 16) >= 500 * 500 || NPC.AnyNPCs(ModContent.NPCType<GathicTomb_Spawner>()))
                return;

            Vector2 pos = new((SpiritOldManPoint.X + 1) * 16, (SpiritOldManPoint.Y + 10) * 16);
            LabArea.SpawnNPCInWorld(pos, ModContent.NPCType<GathicTomb_Spawner>(), 2);
        }
        public static void SpawnSpiritOldLady()
        {
            if (RedeWorld.spawnCleared[3] || SpiritOldLadyPoint.X == 0 || Main.LocalPlayer.DistanceSQ(SpiritOldLadyPoint.ToVector2() * 16) >= 500 * 500 || NPC.AnyNPCs(ModContent.NPCType<GathicTomb_Spawner>()))
                return;

            Vector2 pos = new(SpiritOldLadyPoint.X * 16, SpiritOldLadyPoint.Y * 16);
            LabArea.SpawnNPCInWorld(pos, ModContent.NPCType<GathicTomb_Spawner>(), 3);
        }
        public static void SpawnSpiritCommonGuard()
        {
            if (RedeWorld.spawnCleared[1] || SpiritCommonGuardPoint.X == 0 || Main.LocalPlayer.DistanceSQ(SpiritCommonGuardPoint.ToVector2() * 16) >= 500 * 500 || NPC.AnyNPCs(ModContent.NPCType<GathicTomb_Spawner>()))
                return;

            Vector2 pos = new(SpiritCommonGuardPoint.X * 16, SpiritCommonGuardPoint.Y * 16);
            LabArea.SpawnNPCInWorld(pos, ModContent.NPCType<GathicTomb_Spawner>(), 1);
        }
        public static void SpawnSpiritDruid()
        {
            if (RedeWorld.spawnCleared[4] || SpiritDruidPoint.X == 0 || Main.LocalPlayer.DistanceSQ(SpiritDruidPoint.ToVector2() * 16) >= 500 * 500 || NPC.AnyNPCs(ModContent.NPCType<GathicTomb_Spawner>()))
                return;

            Vector2 pos = new(SpiritDruidPoint.X * 16, SpiritDruidPoint.Y * 16);
            LabArea.SpawnNPCInWorld(pos, ModContent.NPCType<GathicTomb_Spawner>(), 4);
        }
        public override void SaveWorldData(TagCompound tag)
        {
            var lists = new List<string>();

            if (dragonLeadSpawn)
                lists.Add("DLeadSpawn");
            if (cryoCrystalSpawn)
                lists.Add("CCrystalSpawn");

            tag["lists"] = lists;
            tag["newbCaveVectorX"] = newbCaveVector.X;
            tag["newbCaveVectorY"] = newbCaveVector.Y;
            tag["gathicPortalVectorX"] = gathicPortalVector.X;
            tag["gathicPortalVectorY"] = gathicPortalVector.Y;
            tag["slayerShipVectorX"] = slayerShipVector.X;
            tag["slayerShipVectorY"] = slayerShipVector.Y;
            tag["HallOfHeroesVectorX"] = HallOfHeroesVector.X;
            tag["HallOfHeroesVectorY"] = HallOfHeroesVector.Y;
            tag["LabVectorX"] = LabVector.X;
            tag["LabVectorY"] = LabVector.Y;
            tag["BastionVectorX"] = BastionVector.X;
            tag["BastionVectorY"] = BastionVector.Y;
            tag["GoldenGatewayVectorX"] = GoldenGatewayVector.X;
            tag["GoldenGatewayVectorY"] = GoldenGatewayVector.Y;
            tag["JShrineX"] = JoShrinePoint.X;
            tag["JShrineY"] = JoShrinePoint.Y;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");
            dragonLeadSpawn = lists.Contains("DLeadSpawn");
            cryoCrystalSpawn = lists.Contains("CCrystalSpawn");

            newbCaveVector.X = tag.GetFloat("newbCaveVectorX");
            newbCaveVector.Y = tag.GetFloat("newbCaveVectorY");
            gathicPortalVector.X = tag.GetFloat("gathicPortalVectorX");
            gathicPortalVector.Y = tag.GetFloat("gathicPortalVectorY");
            slayerShipVector.X = tag.GetFloat("slayerShipVectorX");
            slayerShipVector.Y = tag.GetFloat("slayerShipVectorY");
            HallOfHeroesVector.X = tag.GetFloat("HallOfHeroesVectorX");
            HallOfHeroesVector.Y = tag.GetFloat("HallOfHeroesVectorY");
            LabVector.X = tag.GetFloat("LabVectorX");
            LabVector.Y = tag.GetFloat("LabVectorY");
            BastionVector.X = tag.GetFloat("BastionVectorX");
            BastionVector.Y = tag.GetFloat("BastionVectorY");
            GoldenGatewayVector.X = tag.GetFloat("GoldenGatewayVectorX");
            GoldenGatewayVector.Y = tag.GetFloat("GoldenGatewayVectorY");
            JoShrinePoint = new Point16(tag.Get<ushort>("JShrineX"), tag.Get<ushort>("JShrineY"));
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = dragonLeadSpawn;
            flags[1] = cryoCrystalSpawn;
            writer.Write(flags);

            writer.WritePackedVector2(newbCaveVector);
            writer.WritePackedVector2(gathicPortalVector);
            writer.WritePackedVector2(slayerShipVector);
            writer.WritePackedVector2(HallOfHeroesVector);
            writer.WritePackedVector2(LabVector);
            writer.WritePackedVector2(BastionVector);
            writer.WritePackedVector2(GoldenGatewayVector);
            writer.Write(JoShrinePoint.X);
            writer.Write(JoShrinePoint.Y);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            dragonLeadSpawn = flags[0];
            cryoCrystalSpawn = flags[1];

            newbCaveVector = reader.ReadPackedVector2();
            gathicPortalVector = reader.ReadPackedVector2();
            slayerShipVector = reader.ReadPackedVector2();
            HallOfHeroesVector = reader.ReadPackedVector2();
            LabVector = reader.ReadPackedVector2();
            BastionVector = reader.ReadPackedVector2();
            GoldenGatewayVector = reader.ReadPackedVector2();
            JoShrinePoint = new Point16(reader.ReadUInt16(), reader.ReadUInt16());
        }
    }
    public class GenUtils
    {
        public static void ObjectPlace(Point Origin, int x, int y, int TileType, int style = 0, int direction = -1)
        {
            WorldGen.PlaceObject(Origin.X + x, Origin.Y + y, TileType, true, style, 0, -1, direction);
            //NetMessage.SendObjectPlacment(-1, Origin.X + x, Origin.Y + y, TileType, style, 0, -1, direction);
        }
        public static void ObjectPlace(int x, int y, int TileType, int style = 0, int direction = -1)
        {
            WorldGen.PlaceObject(x, y, TileType, true, style, 0, -1, direction);
            //NetMessage.SendObjectPlacment(-1, x, y, TileType, style, 0, -1, direction);
        }
        public static void InvokeOnMainThread(Action action)
        {
            if (!AssetRepository.IsMainThread)
            {
                ManualResetEvent evt = new(false);

                Main.QueueMainThreadAction(() =>
                {
                    action();
                    evt.Set();
                });

                evt.WaitOne();
            }
            else
                action();
        }
    }
}
