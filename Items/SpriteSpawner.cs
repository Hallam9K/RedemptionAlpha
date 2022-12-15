using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Tiles.Tiles;
using Redemption.UI;
using Redemption.Walls;
using Redemption.WorldGeneration;
using Redemption.WorldGeneration.Space;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;
using static Redemption.Effects.RenderTargets.BasicLayer;

namespace Redemption.Items
{
    public class SpriteSpawner : ModItem, IBasicSprite
    {
        public static int x;
        public static int y;
        public int divisions;
        public Vector2 offset = new(0f, 0f);
        public bool active = x != 0 && y != 0;
        public bool Active { get => Item.active; set => Item.active = value; }
        public float progress;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sprite Spawner");
            Tooltip.SetDefault("Spawns sprites at the cursor and continuously draws them at that location.\n" +
                               "Can be used to test shaders. You should use Edit and Continue to do this.");
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Purple;
        }
        public override bool AltFunctionUse(Player player) => true;
        public int ge;
        public override bool? UseItem(Player player)
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

            Point16 origin = new((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
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
            GenUtils.ObjectPlace(origin.X + 69, origin.Y + 33, ModContent.TileType<KSBattlestationTile>(), 2);
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

            for (int i = origin.X; i < origin.X + 119; i++)
            {
                for (int j = origin.Y; j < origin.Y + 75; j++)
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
            /*NPC npc = NPC.NewNPCDirect(Item.GetSource_FromThis(), player.Center, NPCID.GreenSlime);
            npc.position = player.Center;

            DialogueChain chain = new();
            chain.modifier = new(0f, 128f);
            chain.Add(new(npc, "Hey there, don't mind me, I'm just testing this UI. I'll be out of your^20^...[90]uh^20^...[90]^6^hair[30] in no time.", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "It's such a lovely day out! I hope nothing bad happens to me...", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "What could go wrong anyway? Seems pretty safe out here.", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "Plus! I could just run away at any time! I'm SUPER[@BOO!] good at jumping.", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "[@Gotcha!]Apparently there's a slime out there that can jump a bajillion feet into the air! Something like the King of all slimes...", Color.LightGreen, Color.DarkCyan, boxFade: true));
            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;

            TextBubbleUI.Visible = true;
            TextBubbleUI.Add(chain);
            //Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<AdamPortal>(), 0, 0f);

            if (player.altFunctionUse == 2)
            {
                x = 0;
                y = 0;
                progress = 0f;
                Talk("Coordinates cleared.", new Color(218, 70, 70));
                if (Redemption.Targets.BasicLayer.Sprites.Contains(this))
                    Redemption.Targets.BasicLayer.Sprites.Remove(this);
                return true;
            }
            x = (int)(Main.MouseWorld.X / 16);
            y = (int)(Main.MouseWorld.Y / 16);
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, new Color(218, 70, 70), null);
            Talk($"Drawing sprites at [{x}, {y}]. Right-click to discard.", new Color(218, 70, 70));
            if (!Redemption.Targets.BasicLayer.Sprites.Contains(this))
                Redemption.Targets.BasicLayer.Sprites.Add(this);*/
            return true;
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            Talk(signature, Color.LightGreen);
        }
        public static void Talk(string message, Color color) => Main.NewText(message, color.R, color.G, color.B);
        public void Draw(SpriteBatch spriteBatch)
        {
            //float sin = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) + 1f / 4f + 1f;

            //progress += 1f;

            //if(progress % Main.rand.Next(18, 20) == 0)
            //    ParticleManager.NewParticle(new Vector2(x * 16, y * 16), Vector2.Zero, new EmberParticle(), Color.White, 1f);

            // Draw code here.
            //Effect effect = ModContent.Request<Effect>("Redemption/Effects/Circle").Value;
            //effect.Parameters["uColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            //effect.Parameters["uOpacity"].SetValue(1f);
            //effect.Parameters["uProgress"].SetValue(progress / 100f);
            //effect.CurrentTechnique.Passes[0].Apply();

            //Texture2D circle = ModContent.Request<Texture2D>("Redemption/Textures/EmptyPixel").Value;

            //spriteBatch.Draw(circle, new Vector2(x * 16, y * 16) - Main.screenPosition, Color.White);
            //spriteBatch.Draw(circle, new Vector2(x * 16, y * 16) - Main.screenPosition - new Vector2(progress * 0.5f * sin, progress * 0.5f * sin), new Rectangle(0, 0, 1, 1), Color.White, 0f, Vector2.Zero, progress * sin, SpriteEffects.None, 0f);
        }
    }
}
