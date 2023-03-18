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
using Terraria.GameInput;
using Terraria.UI.Chat;
using Terraria.GameContent;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Tiles.Furniture.Kingdom;

namespace Redemption.WorldGeneration.Soulless
{
    public class SoullessSub : Subworld
    {
        public override int Width => 2200;
        public override int Height => 1800;
        public override bool NormalUpdates => false;
        public override bool ShouldSave => true;
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
            if (SoullessArea.soullessBools[2])
            {
                Main.spawnTileY = 1024 + SoullessArea.Offset.Y;
                Main.spawnTileX = 509 + SoullessArea.Offset.X;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && SoullessArea.soullessInts[0] < 2)
            {
                if (SoullessArea.soullessInts[0] < 1)
                    NPC.NewNPC(new EntitySource_WorldGen(), (496 + SoullessArea.Offset.X) * 16, (802 + SoullessArea.Offset.Y) * 16, ModContent.NPCType<TwinfaceSoulless>());
                else
                    NPC.NewNPC(new EntitySource_WorldGen(), (534 + SoullessArea.Offset.X) * 16, (802 + SoullessArea.Offset.Y) * 16, ModContent.NPCType<TwinfaceSoulless>(), 0, 1, 0, Main.rand.Next(80, 280));
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
        private double animationTimer = 0;
        private double animationTimer2 = 0;
        public override void DrawSetup(GameTime gameTime)
        {
            PlayerInput.SetZoom_Unscaled();
            Main.instance.GraphicsDevice.Clear(Color.Black);
            DrawMenu(gameTime);
        }
        public override void DrawMenu(GameTime gameTime)
        {
            Texture2D soullessBackground = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/SoullessSubworldTex").Value;
            Texture2D portalTex = ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);
            Main.spriteBatch.Draw(portalTex, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * (float)(animationTimer / 5) * 0.4f, (float)animationTimer2 / 2, new Vector2(portalTex.Width / 2, portalTex.Height / 2), 20, 0, 0);
            Main.spriteBatch.Draw(portalTex, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * (float)(animationTimer / 5) * 0.3f, (float)animationTimer2 * 0.75f, new Vector2(portalTex.Width / 2, portalTex.Height / 2), 16, 0, 0);
            Main.spriteBatch.Draw(portalTex, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * (float)(animationTimer / 5) * 0.2f, (float)animationTimer2, new Vector2(portalTex.Width / 2, portalTex.Height / 2), 12, 0, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);
            Main.spriteBatch.Draw
            (
                soullessBackground,
                new Rectangle(Main.screenWidth - soullessBackground.Width, Main.screenHeight - soullessBackground.Height + 50 - (int)(animationTimer * 10), soullessBackground.Width, soullessBackground.Height),
                null,
                Color.White * (float)(animationTimer / 5) * 0.8f
            );
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, Main.statusText, new Vector2(Main.screenWidth, (float)Main.screenHeight) / 2f - FontAssets.DeathText.Value.MeasureString(Main.statusText) / 2f, Color.White, 0f, Vector2.Zero, Vector2.One);

            Main.spriteBatch.End();

            animationTimer += 0.025;
            animationTimer2 += 0.025;
            if (animationTimer > 5)
                animationTimer = 5;
        }
        public override void OnExit()
        {
            animationTimer = 0;
            animationTimer2 = 0;
        }
        public override void OnEnter()
        {
            animationTimer = 0;
            animationTimer2 = 0;
        }
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
            Main.spawnTileY = 799 + SoullessArea.Offset.Y;
            Main.spawnTileX = 432 + SoullessArea.Offset.X;
            Main.worldSurface = 635 + SoullessArea.Offset.Y;
            Main.rockLayer = 635 + SoullessArea.Offset.Y;

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
                gen.Generate(SoullessArea.Offset.X, SoullessArea.Offset.Y, true, true);
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
            int offsetX = SoullessArea.Offset.X;
            int offsetY = SoullessArea.Offset.Y;

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
                gen.Generate(offsetX, offsetY, true, true);
            });
            for (int x = offsetX; x < offsetX + platTex.Width; x++)
            {
                for (int y = offsetY; y < offsetY + platTex.Height; y++)
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
                [new Color(10, 12, 255)] = TileID.BubblegumBlock,
                [Color.Black] = -1
            };
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(ObjectTex, colorToObj);
                gen.Generate(offsetX, offsetY, true, true);
            });

            #region Objects
            for (int x2 = offsetX; x2 < offsetX + ObjectTex.Width; x2++)
            {
                for (int y2 = offsetY; y2 < offsetY + ObjectTex.Height; y2++)
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
                        case TileID.BubblegumBlock:
                            Main.tile[x2, y2].ClearTile();
                            GenUtils.ObjectPlace(x2, y2, ModContent.TileType<ShadesteelPortcullis2Close>());
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

            GenUtils.ObjectPlace(440 + offsetX, 797 + offsetY, ModContent.TileType<ShadestoneCandleTile>());
            GenUtils.ObjectPlace(240 + offsetX, 765 + offsetY, ModContent.TileType<ShadestoneCandleTile>());
            GenUtils.ObjectPlace(524 + offsetX, 874 + offsetY, ModContent.TileType<ShadestoneCandleTile>());
            GenUtils.ObjectPlace(554 + offsetX, 833 + offsetY, ModContent.TileType<ShadestoneCandelabraTile>());
            GenUtils.ObjectPlace(276 + offsetX, 853 + offsetY, ModContent.TileType<ShadestoneCandelabraTile>());
            GenUtils.ObjectPlace(300 + offsetX, 749 + offsetY, ModContent.TileType<GiantShadesteelChainTile>());
            GenUtils.ObjectPlace(291 + offsetX, 763 + offsetY, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(285 + offsetX, 870 + offsetY, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(235 + offsetX, 979 + offsetY, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(366 + offsetX, 1079 + offsetY, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(339 + offsetX, 1060 + offsetY, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(693 + offsetX, 1071 + offsetY, ModContent.TileType<ShadestoneMirrorTile>());
            GenUtils.ObjectPlace(540 + offsetX, 843 + offsetY, ModContent.TileType<ShadestoneToiletTile>());
            GenUtils.ObjectPlace(208 + offsetX, 998 + offsetY, ModContent.TileType<ShadestoneCrateTile>());

            GenUtils.ObjectPlace(352 + offsetX, 813 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(289 + offsetX, 870 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(335 + offsetX, 883 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(550 + offsetX, 835 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(398 + offsetX, 794 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(505 + offsetX, 851 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(331 + offsetX, 1085 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(418 + offsetX, 1117 + offsetY, ModContent.TileType<SoullessRemainsTile2>());
            GenUtils.ObjectPlace(591 + offsetX, 1209 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(487 + offsetX, 1163 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(691 + offsetX, 1106 + offsetY, ModContent.TileType<SoullessRemainsTile>());
            GenUtils.ObjectPlace(500 + offsetX, 1210 + offsetY, ModContent.TileType<SoullessRemainsTile2>());

            GenUtils.ObjectPlace(378 + offsetX, 1054 + offsetY, ModContent.TileType<ShadestoneMonolith1Tile>());

            GenUtils.ObjectPlace(602 + offsetX, 820 + offsetY, ModContent.TileType<ShadesteelLeverTile>());
            GenUtils.ObjectPlace(612 + offsetX, 862 + offsetY, ModContent.TileType<ShadesteelLeverTile>());
            GenUtils.ObjectPlace(338 + offsetX, 761 + offsetY, ModContent.TileType<ShadesteelLeverTile>());
            GenUtils.ObjectPlace(328 + offsetX, 786 + offsetY, ModContent.TileType<ShadesteelLeverTile>());

            GenUtils.ObjectPlace(313 + offsetX, 1004 + offsetY, ModContent.TileType<StalkerGateTile>());

            GenUtils.ObjectPlace(734 + offsetX, 1055 + offsetY, ModContent.TileType<AngelStatue_SC>());
            //Chests
            SpookChest(265, 854);
            SpookChest(254, 880);
            SpookChest(289, 763);
            SpookChest(404, 789);
            SpookChest(277, 804);
            SpookChest(370, 871);
            SpookChest(559, 834);
            SpookChest(658, 799);
            SpookChest(462, 841);
            SpookChest(206, 995);
            SpookChest(206, 998);
            SpookChest(274, 1054);
            SpookChest(321, 1060);
            SpookChest(657, 1052);
            SpookChest(242, 924);
            SpookChest(666, 1080);
            SpookChest(485, 1168);

            for (int i = offsetX; i < 1800 + offsetX; i++)
            {
                for (int j = offsetY; j < 1800 + offsetY; j++)
                {
                    if (Framing.GetTileSafely(i, j).TileType == TileID.Books && WorldGen.InWorld(i, j))
                        Framing.GetTileSafely(i, j).TileColor = PaintID.WhitePaint;
                }
            }
            for (int i = 68 + offsetX; i < 82 + offsetX; i++)
            {
                for (int j = 837 + offsetY; j < 844 + offsetY; j++)
                {
                    if (Framing.GetTileSafely(i, j).WallType == ModContent.WallType<LeadFenceBlackWall>() && WorldGen.InWorld(i, j))
                        Framing.GetTileSafely(i, j).WallColor = PaintID.ShadowPaint;
                }
            }
            for (int i = 106 + offsetX; i < 120 + offsetX; i++)
            {
                for (int j = 837 + offsetY; j < 844 + offsetY; j++)
                {
                    if (Framing.GetTileSafely(i, j).WallType == ModContent.WallType<LeadFenceBlackWall>() && WorldGen.InWorld(i, j))
                        Framing.GetTileSafely(i, j).WallColor = PaintID.ShadowPaint;
                }
            }
        }
        public SoullessPass2(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        #region Chest Contents
        public static void SpookChest(int x, int y, int chestID = 0)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x + SoullessArea.Offset.X, y + SoullessArea.Offset.Y, (ushort)ModContent.TileType<ShadestoneChestTile>(), false);

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

                if (WorldGen.genRand.NextBool(2))
                {
                    chest.item[slot].SetDefaults(ModContent.ItemType<VesselDagger>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(200, 301);
                }
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
                int xAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.X - 45);
                int yAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.Y - 45);
                for (int DecoX = xAxis; DecoX < xAxis + 45; DecoX++)
                {
                    for (int DecoY = yAxis; DecoY < yAxis + 45; DecoY++)
                    {
                        if (Framing.GetTileSafely(DecoX, DecoY).TileType == ModContent.TileType<ShadestoneBrickTile>() && !Framing.GetTileSafely(DecoX, DecoY - 1).HasTile)
                        {
                            if (WorldGen.genRand.NextBool(15))
                                WorldGen.PlaceObject(DecoX, DecoY - 1, ModContent.TileType<ShadePots>(), true, Main.rand.Next(3));
                        }
                        if (Framing.GetTileSafely(DecoX, DecoY).TileType == ModContent.TileType<ShadestoneTile>() && !Framing.GetTileSafely(DecoX, DecoY - 1).HasTile)
                        {
                            if (WorldGen.genRand.NextBool(30))
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
            for (int i = SoullessArea.Offset.X; i < 1800 + SoullessArea.Offset.X; i++)
            {
                for (int j = SoullessArea.Offset.Y; j < 1800 + SoullessArea.Offset.Y; j++)
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
                int xAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.X - 45);
                int yAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.Y - 45);
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
                int xAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.X - 45);
                int yAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.Y - 45);
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
                int xAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.X - 45);
                int yAxis = WorldGen.genRand.Next(1800 + SoullessArea.Offset.Y - 45);
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
            for (int i = SoullessArea.Offset.X; i < 1800 + SoullessArea.Offset.X; i++)
            {
                for (int j = SoullessArea.Offset.Y; j < 1800 + SoullessArea.Offset.Y; j++)
                {
                    if (TileArray.Contains(Framing.GetTileSafely(i, j).TileType) && WorldGen.InWorld(i, j))
                        BaseWorldGen.SmoothTiles(i, j, i + 1, j + 1);

                    if (Framing.GetTileSafely(i, j).TileType == ModContent.TileType<ShadestonePlatformTile>() && WorldGen.InWorld(i, j))
                        WorldGen.KillTile(i, j, true);
                }
            }
            WorldGen.noTileActions = false;
        }
        public SoullessPass7(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
}
