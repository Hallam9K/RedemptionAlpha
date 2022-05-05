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
using ReLogic.Content;
using Redemption.Base;
using Terraria.ID;
using Redemption.Tiles.Furniture.Shade;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using System.Linq;
using Terraria.DataStructures;
using Redemption.NPCs.Soulless;
using Redemption.Tiles.Containers;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Furniture.Shade;
using Terraria.Utilities;
using Redemption.Items.Usable;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Globals;
using Redemption.Items.Usable.Potions;

namespace Redemption.WorldGeneration.Soulless
{
    public class SoullessSub : Subworld
    {
        public override int Width => 2200;
        public override int Height => 1800;
        public override bool NormalUpdates => false;
        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => false;
        public override List<GenPass> Tasks => new()
        {
            new SoullessPass1("Loading", 1),
            new SoullessPass2("Furnishing Caverns", 0.3f),
            new SoullessPass3("Sprinkling Spooky Pots", 0.1f),
            new SoullessPass4("Growing Cysts", 0.1f),
            new SoullessPass5("Sprinkling Spooky Objects", 0.1f),
            new SoullessPass6("Here, Have a Fungus", 0.01f),
            new SoullessPass7("Smoothing Tiles", 0.01f)
        };
        public override void OnLoad()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC(new EntitySource_WorldGen(), 474 * 16, 759 * 16, ModContent.NPCType<LostLight>());
            }

            Main.cloudAlpha = 0;
            Main.numClouds = 0;
            Main.rainTime = 0;
            Main.raining = false;
            Main.maxRaining = 0f;
            Main.slimeRain = false;

            Main.dayTime = true;
            Main.time = 40000;
        }
        //private double animationTimer = 0;
        /*public override void DrawMenu(GameTime gameTime)
        {
            Texture2D soullessBackground = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessSubworldTex").Value;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);
            Main.spriteBatch.Draw
            (
                soullessBackground,
                new Rectangle(Main.screenWidth - soullessBackground.Width, Main.screenHeight - soullessBackground.Height + 50 - (int)(animationTimer * 10), soullessBackground.Width, soullessBackground.Height),
                null,
                Color.White * (float)(animationTimer / 5) * 0.8f
            );
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 5)
                animationTimer = 5;
        }*/
        public override void OnUnload()
        {
        }
    }
    public class SoullessPass1 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Loading";
            WorldGen.noTileActions = true;
            Main.spawnTileY = 799;
            Main.spawnTileX = 432;
            Main.worldSurface = 635;
            Main.rockLayer = 635;

            Mod mod = Redemption.Instance;
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(0, 255, 0)] = ModContent.TileType<ShadestoneTile>(),
                [new Color(0, 0, 255)] = ModContent.TileType<ShadestoneBrickTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<ShadestoneRubbleTile>(),
                [new Color(255, 255, 255)] = ModContent.TileType<MasksTile>(),
                [new Color(20, 20, 20)] = ModContent.TileType<BedrockTile>(),
                [new Color(110, 115, 157)] = ModContent.TileType<PrisonBarsTile>(),
                [new Color(77, 81, 110)] = ModContent.TileType<PrisonBarsBeamTile>(),
                [new Color(22, 26, 35)] = ModContent.TileType<ShadestoneMossyTile>(),
                [new Color(0, 26, 35)] = ModContent.TileType<ShadestoneBrickMossyTile>(),
                [new Color(0, 255, 255)] = ModContent.TileType<ShadestoneSlabTile>(),
                [new Color(70, 70, 70)] = ModContent.TileType<ShadesteelChainTile>(),
                [new Color(200, 200, 100)] = ModContent.TileType<AncientAlloyBrickTile>(),
                [new Color(200, 100, 200)] = ModContent.TileType<AncientAlloyPipeTile>(),
                [new Color(100, 200, 200)] = ModContent.TileType<EvergoldBrickTile>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(0, 0, 255)] = ModContent.WallType<ShadestoneWallTile>(),
                [new Color(255, 0, 0)] = ModContent.WallType<ShadestoneBrickWallTile>(),
                [new Color(100, 100, 100)] = ModContent.WallType<LeadFenceBlackWall>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessCaverns", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessCavernsWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texSlopes = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessCavernsSlopes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texPlatforms = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessCavernsPlatforms", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texLiquids = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessCavernsLiquids", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall, texLiquids, texSlopes);
                gen.Generate(0, 0, true, true);
            });
        }
        public SoullessPass1(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SoullessPass2 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Furnishing Caverns";
            Mod mod = Redemption.Instance;

            #region Platforms
            Dictionary<Color, int> colorToTile2 = new()
            {
                [new Color(255, 0, 0)] = TileID.RedStucco,
                [new Color(0, 255, 0)] = TileID.YellowStucco,
                [new Color(255, 0, 255)] = TileID.GreenStucco,
                [Color.Black] = -1
            };
            Texture2D platTex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessCavernsPlatforms", AssetRequestMode.ImmediateLoad).Value;
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(platTex, colorToTile2);
                gen.Generate(0, 0, true, true);
            });
            for (int x = 0; x < 0 + platTex.Width; x++)
            {
                for (int y = 0; y < 0 + platTex.Height; y++)
                {
                    switch (Main.tile[x, y].TileType)
                    {
                        case TileID.RedStucco:
                            Main.tile[x, y].ClearTile();
                            WorldGen.PlaceTile(x, y, ModContent.TileType<ShadestonePlatformTile>(), true, false, -1, 0);
                            WorldGen.SlopeTile(x, y, 1);
                            break;
                        case TileID.YellowStucco:
                            Main.tile[x, y].ClearTile();
                            WorldGen.PlaceTile(x, y, ModContent.TileType<ShadestonePlatformTile>(), true, false, -1, 0);
                            WorldGen.SlopeTile(x, y, 2);
                            break;
                        case TileID.GreenStucco:
                            Main.tile[x, y].ClearTile();
                            WorldGen.PlaceTile(x, y, ModContent.TileType<ShadestonePlatformTile>(), true, false, -1, 0);
                            break;
                    }
                }
            }
            #endregion

            Texture2D ObjectTex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessCavernsObjects", AssetRequestMode.ImmediateLoad).Value;
            Dictionary<Color, int> colorToObj = new()
            {
                [new Color(255, 0, 0)] = TileID.AmberGemspark,
                [new Color(150, 0, 0)] = TileID.AmethystGemspark,
                [new Color(0, 0, 255)] = TileID.AmethystGemsparkOff,
                [new Color(100, 0, 0)] = TileID.DiamondGemspark,
                [new Color(0, 255, 0)] = TileID.EmeraldGemspark,
                [new Color(0, 150, 0)] = TileID.EmeraldGemsparkOff,
                [new Color(0, 100, 0)] = TileID.RubyGemspark,
                [new Color(255, 255, 0)] = TileID.RubyGemsparkOff,
                [new Color(255, 0, 255)] = TileID.SapphireGemspark,
                [new Color(0, 255, 255)] = TileID.TopazGemspark,
                [new Color(0, 100, 100)] = TileID.TopazGemsparkOff,
                [new Color(100, 0, 100)] = TileID.AmberGemsparkOff,
                [new Color(120, 120, 120)] = TileID.DiamondGemsparkOff,
                [new Color(180, 180, 180)] = TileID.SapphireGemsparkOff,
                [new Color(141, 134, 135)] = TileID.TeamBlockBlue,
                [new Color(247, 245, 213)] = TileID.TeamBlockGreen,
                [new Color(203, 185, 151)] = TileID.TeamBlockPink,
                [new Color(255, 66, 0)] = TileID.TeamBlockRed,
                [new Color(255, 66, 66)] = TileID.TeamBlockWhite,
                [new Color(255, 200, 66)] = TileID.TeamBlockYellow,
                [new Color(255, 66, 200)] = TileID.GrayStucco,
                [new Color(255, 120, 255)] = TileID.GreenStucco,
                [new Color(100, 120, 255)] = TileID.HayBlock,
                [new Color(100, 120, 200)] = TileID.AntiPortalBlock,
                [new Color(233, 120, 233)] = TileID.SandStoneSlab,
                [new Color(220, 0, 0)] = TileID.StoneSlab,
                [new Color(200, 0, 0)] = TileID.AccentSlab,
                [new Color(141, 132, 172)] = TileID.Palladium,
                [Color.Black] = -1
            };
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(ObjectTex, colorToObj);
                gen.Generate(0, 0, true, true);
            });

            #region Objects
            for (int x2 = 0; x2 < 0 + ObjectTex.Width; x2++)
            {
                for (int y2 = 0; y2 < 0 + ObjectTex.Height; y2++)
                {
                    switch (Main.tile[x2, y2].TileType)
                    {
                        case TileID.AmberGemspark:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneChairTile>(), 0, 1);
                            break;
                        case TileID.AccentSlab:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneChairTile>());
                            break;
                        case TileID.AmethystGemspark:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneTableTile>());
                            break;
                        case TileID.AmethystGemsparkOff:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneTable2Tile>());
                            break;
                        case TileID.DiamondGemspark:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneBookcaseTile>());
                            break;
                        case TileID.EmeraldGemspark:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneLanternTile>());
                            break;
                        case TileID.EmeraldGemsparkOff:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneChandelierTile>());
                            break;
                        case TileID.RubyGemspark:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneCandelabraTile>());
                            break;
                        case TileID.RubyGemsparkOff:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneLampTile>());
                            break;
                        case TileID.SapphireGemspark:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestonePillar1Tile>());
                            break;
                        case TileID.TopazGemspark:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneSofaTile>());
                            break;
                        case TileID.TopazGemsparkOff:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneWorkbenchTile>());
                            break;
                        case TileID.AmberGemsparkOff:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestonePillar2Tile>());
                            break;
                        case TileID.DiamondGemsparkOff:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneSinkTile>());
                            break;
                        case TileID.SapphireGemsparkOff:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneCandleTile>());
                            break;
                        case TileID.TeamBlockBlue:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneDoorClosed>());
                            break;
                        case TileID.TeamBlockGreen:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneDresserTile>());
                            break;
                        case TileID.TeamBlockPink:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadesteelHangingCell2Tile>());
                            break;
                        case TileID.TeamBlockRed:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneBathtubTile>());
                            break;
                        case TileID.TeamBlockWhite:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneBedTile>(), 0, 1);
                            break;
                        case TileID.StoneSlab:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneBedTile>());
                            break;
                        case TileID.TeamBlockYellow:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestoneClockTile>());
                            break;
                        case TileID.GrayStucco:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadestonePianoTile>());
                            break;
                        case TileID.GreenStucco:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadesteelHangingCellTile>(), Main.rand.Next(3));
                            break;
                        case TileID.HayBlock:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadesteelPortcullisClose>());
                            break;
                        case TileID.AntiPortalBlock:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadesteelGateClose>());
                            break;
                        case TileID.SandStoneSlab:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, TileID.Books);
                            break;
                        case TileID.Palladium:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<WaxCandlesTile>(), Main.rand.Next(5));
                            break;
                    }
                }
            }
            #endregion

            GenUtils.ObjectPlace(440, 797, ModContent.TileType<ShadestoneCandleTile>());
            GenUtils.ObjectPlace(240, 765, ModContent.TileType<ShadestoneCandleTile>());
            GenUtils.ObjectPlace(524, 874, ModContent.TileType<ShadestoneCandleTile>());
            GenUtils.ObjectPlace(554, 833, ModContent.TileType<ShadestoneCandelabraTile>());
            GenUtils.ObjectPlace(276, 853, ModContent.TileType<ShadestoneCandelabraTile>());
            GenUtils.ObjectPlace(300, 749, ModContent.TileType<GiantShadesteelChainTile>());
            GenUtils.ObjectPlace(291, 763, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(327, 786, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(285, 870, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(540, 843, ModContent.TileType<ShadestoneToiletTile>());

            GenUtils.ObjectPlace(352, 813, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(289, 870, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(335, 883, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(550, 835, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(398, 794, ModContent.TileType<SoullessRemainsTile>());
            //Chests
            SpookChest(265, 854);
            SpookChest(254, 880);
            SpookChest(289, 763);
            SpookChest(404, 789);
            SpookChest(277, 804);
            SpookChest(370, 871);
            SpookChest(559, 834);
            SpookChest(658, 799);

            for (int i = 0; i < 1800; i++)
            {
                for (int j = 0; j < 1800; j++)
                {
                    if (Framing.GetTileSafely(i, j).TileType == TileID.Books && WorldGen.InWorld(i, j))
                        Framing.GetTileSafely(i, j).TileColor = PaintID.WhitePaint;
                }
            }
        }
        public SoullessPass2(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        #region Chest Contents
        public static void SpookChest(int x, int y, int chestID = 0)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<ShadestoneChestTile>(), false);

            int[] ChestLoot2 = new int[] {
                ItemID.MiningPotion, ItemID.BattlePotion, ModContent.ItemType<LurkingKetredPotion>(), ItemID.InvisibilityPotion };
            int[] ChestLoot3 = new int[] {
                ItemID.EndurancePotion, ItemID.WrathPotion, ModContent.ItemType<InsulatiumPotion>(), ModContent.ItemType<ChakrogAnglerPotion>(), };

            if (PlacementSuccess >= 0)
            {
                int slot = 0;
                Chest chest = Main.chest[PlacementSuccess];

                int[] ChestLoot1 = new int[]
                { ModContent.ItemType<SoulScroll>(), ModContent.ItemType<MaskOfGrief>(),  ModContent.ItemType<StatuetteOfFaith>(),  ModContent.ItemType<ManiacsLantern>(), ModContent.ItemType<CageFlail>(), ModContent.ItemType<SoulCandles>() };

                chest.item[slot++].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot1));

                chest.item[slot].SetDefaults(ModContent.ItemType<Shadesoul>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 3);

                //if (WorldGen.genRand.NextBool(2))
                //{
                //    chest.item[slot].SetDefaults(ModContent.ItemType<ShadeKnife>());
                //    chest.item[slot++].stack = WorldGen.genRand.Next(100, 600);
                //}
                if (RedeHelper.GenChance(.66f))
                {
                    chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot2));
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 2);
                }
                if (RedeHelper.GenChance(.33f))
                {
                    chest.item[slot].SetDefaults(Utils.Next(WorldGen.genRand, ChestLoot3));
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 2);
                }
                if (WorldGen.genRand.NextBool(6))
                    chest.item[slot++].SetDefaults(ModContent.ItemType<BlackenedHeart>());
            }
        }
        #endregion
    }
    public class SoullessPass3 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Sprinkling Spooky Pots";
            #region Pots for Soulless Caverns
            for (int num = 0; num < 1800; num++)
            {
                int xAxis = WorldGen.genRand.Next(1800 - 45);
                int yAxis = WorldGen.genRand.Next(1800 - 45);
                for (int DecoX = xAxis; DecoX < xAxis + 45; DecoX++)
                {
                    for (int DecoY = yAxis; DecoY < yAxis + 45; DecoY++)
                    {
                        if (Framing.GetTileSafely(DecoX, DecoY).TileType == ModContent.TileType<ShadestoneBrickTile>() && !Framing.GetTileSafely(DecoX, DecoY - 1).HasTile)
                        {
                            if (WorldGen.genRand.NextBool(20))
                                WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadePots>(), true, Main.rand.Next(3));
                        }
                        if (Framing.GetTileSafely(DecoX, DecoY).TileType == ModContent.TileType<ShadestoneTile>() && !Framing.GetTileSafely(DecoX, DecoY - 1).HasTile)
                        {
                            if (WorldGen.genRand.NextBool(40))
                                WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadePots>(), true, Main.rand.Next(3));
                        }
                    }
                }
            }
            #endregion
        }
        public SoullessPass3(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SoullessPass4 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Growing Cysts";
            #region Ambient Tiles
            for (int i = 0; i < 1800; i++)
            {
                for (int j = 0; j < 1800; j++)
                {
                    ushort type = Framing.GetTileSafely(i, j).TileType;
                    if ((type == ModContent.TileType<ShadestoneTile>() || type == ModContent.TileType<ShadestoneMossyTile>()) && !Framing.GetTileSafely(i, j - 1).HasTile && WorldGen.InWorld(i, j))
                    {
                        if (WorldGen.genRand.NextBool(10))
                            WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ShadeCyst>(), true);
                    }
                }
            }
            #endregion
        }
        public SoullessPass4(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SoullessPass5 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Sprinkling Spooky Objects";
            #region Random Deco for Soulless Caverns
            for (int num = 0; num < 1800; num++)
            {
                int xAxis = WorldGen.genRand.Next(1800 - 45);
                int yAxis = WorldGen.genRand.Next(1800 - 45);
                for (int DecoX = xAxis; DecoX < xAxis + 45; DecoX++)
                {
                    for (int DecoY = yAxis; DecoY < yAxis + 45; DecoY++)
                    {
                        ushort type = Framing.GetTileSafely(DecoX, DecoY).TileType;
                        if ((type == ModContent.TileType<ShadestoneTile>() || type == ModContent.TileType<ShadestoneMossyTile>()) && !Framing.GetTileSafely(DecoX, DecoY - 1).HasTile)
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                switch (WorldGen.genRand.Next(9))
                                {
                                    case 0:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco1>(), true);
                                        break;
                                    case 1:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco2>(), true);
                                        break;
                                    case 2:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco3>(), true);
                                        break;
                                    case 3:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco4>(), true);
                                        break;
                                    case 4:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco6>(), true);
                                        break;
                                    case 5:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco7>(), true);
                                        break;
                                    case 6:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco8>(), true);
                                        break;
                                    case 7:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco9>(), true);
                                        break;
                                    case 8:
                                        WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco10>(), true);
                                        break;

                                }
                            }
                        }
                    }
                }
            }
            for (int num = 0; num < 1200; num++)
            {
                int xAxis = WorldGen.genRand.Next(1800 - 45);
                int yAxis = WorldGen.genRand.Next(1800 - 45);
                for (int DecoX = xAxis; DecoX < xAxis + 45; DecoX++)
                {
                    for (int DecoY = yAxis; DecoY < yAxis + 45; DecoY++)
                    {
                        if (Framing.GetTileSafely(DecoX, DecoY).WallType == ModContent.WallType<ShadestoneBrickWallTile>())
                        {
                            if (WorldGen.genRand.NextBool(1400))
                                WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadeDeco5>(), true, Main.rand.Next(3));
                        }
                    }
                }
            }
            #endregion
        }
        public SoullessPass5(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SoullessPass6 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Here, Have a Fungus";
            #region Let it Grow  
            //WorldGen.AddTrees();
            for (int num = 0; num < 1800; num++)
            {
                int xAxis = WorldGen.genRand.Next(1800 - 45);
                int yAxis = WorldGen.genRand.Next(1800 - 45);
                for (int DecoX = xAxis; DecoX < xAxis + 45; DecoX++)
                {
                    for (int DecoY = yAxis; DecoY < yAxis + 45; DecoY++)
                    {
                        ushort type = Framing.GetTileSafely(DecoX, DecoY).TileType;
                        if ((type == ModContent.TileType<ShadestoneMossyTile>() || type == ModContent.TileType<ShadestoneBrickMossyTile>()) && !Framing.GetTileSafely(DecoX, DecoY + 1).HasTile)
                        {
                            if (WorldGen.genRand.NextBool(15))
                                WorldGen.PlaceObject(DecoX, DecoY + 1, ModContent.TileType<Nooseroot_Large>(), true, Main.rand.Next(3));
                            if (WorldGen.genRand.NextBool(15))
                                WorldGen.PlaceObject(DecoX, DecoY + 1, ModContent.TileType<Nooseroot_Medium>(), true, Main.rand.Next(3));
                            if (WorldGen.genRand.NextBool(15))
                                WorldGen.PlaceObject(DecoX, DecoY + 1, ModContent.TileType<Nooseroot_Small>(), true, Main.rand.Next(3));
                        }
                    }
                }
            }
            #endregion
        }
        public SoullessPass6(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
    public class SoullessPass7 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Smoothing Tiles";
            int[] TileArray = { ModContent.TileType<ShadestoneTile>(),
                ModContent.TileType<MasksTile>(),
                ModContent.TileType<ShadestoneMossyTile>() };
            for (int i = 0; i < 1800; i++)
            {
                for (int j = 0; j < 1800; j++)
                {
                    if (TileArray.Contains(Framing.GetTileSafely(i, j).TileType) && WorldGen.InWorld(i, j))
                        BaseWorldGen.SmoothTiles(i, j, i + 1, j + 1);
                }
            }
            WorldGen.noTileActions = false;
        }
        public SoullessPass7(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
}