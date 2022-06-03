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
            new SoullessPass1("Loading", 1)
        };

        public override void Load()
        {
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
        public override void Unload()
        {
        }
    }
    public class SoullessPass1 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Loading";
            WorldGen.noTileActions = true;
            Main.spawnTileY = 827;
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
                [new Color(22, 26, 35)] = ModContent.TileType<ShadestoneMossyTile>(),
                [new Color(0, 255, 255)] = ModContent.TileType<ShadestoneSlabTile>(),
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
            bool genned = false;
            bool placed = false;
            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall);
                    gen.Generate(0, 0, true, true);

                    genned = true;
                });

                placed = true;
            }
        }
        public SoullessPass1(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
}