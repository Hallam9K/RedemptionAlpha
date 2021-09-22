using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.StructureHelper;
using Redemption.StructureHelper.ChestHelper.GUI;
using Redemption.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption
{
    public class Redemption : Mod
    {
        public override void AddRecipes() => RedeRecipe.Load(this);

        public static Redemption Instance { get; private set; }

        public const string Abbreviation = "MoR";
        public const string EMPTY_TEXTURE = "Redemption/Empty";
        public Vector2 cameraOffset;

        public static int AntiqueDorulCurrencyId;

        public Redemption()
        {
            Instance = this;
        }

        public override void Load()
        {
            AntiqueDorulCurrencyId = CustomCurrencyManager.RegisterCurrency(new AntiqueDorulCurrency(ModContent.ItemType<AncientGoldCoin>(), 999L, "Antique Doruls"));
        }

        public override void Unload()
        {
            RedeRecipe.Unload();
        }
    }
    public class RedeSystem : ModSystem
    {
        public static RedeSystem Instance { get; private set; }
        public RedeSystem()
        {
            Instance = this;
        }

        public static bool Silence;

        public override void PostUpdatePlayers()
        {
            Silence = false;
        }

        UserInterface GeneratorMenuUI;
        internal ManualGeneratorMenu GeneratorMenu;

        UserInterface ChestMenuUI;
        internal ChestCustomizerState ChestCustomizer;

        public UserInterface DialogueUILayer;
        public MoRDialogueUI DialogueUIElement;
        public UserInterface ChaliceUILayer;
        public ChaliceAlignmentUI ChaliceUIElement;

        public UserInterface TitleUILayer;
        public TitleCard TitleCardUIElement;

        public static TrailManager TrailManager;
        public bool Initialized;

        public override void Load()
        {
            RedeDetours.Initialize();
            if (!Main.dedServ)
            {
                On.Terraria.Main.Update += LoadTrailManager;

                TitleUILayer = new UserInterface();
                TitleCardUIElement = new TitleCard();
                TitleUILayer.SetState(TitleCardUIElement);

                DialogueUILayer = new UserInterface();
                DialogueUIElement = new MoRDialogueUI();
                DialogueUILayer.SetState(DialogueUIElement);

                ChaliceUILayer = new UserInterface();
                ChaliceUIElement = new ChaliceAlignmentUI();
                ChaliceUILayer.SetState(ChaliceUIElement);

                GeneratorMenuUI = new UserInterface();
                GeneratorMenu = new ManualGeneratorMenu();
                GeneratorMenuUI.SetState(GeneratorMenu);

                ChestMenuUI = new UserInterface();
                ChestCustomizer = new ChestCustomizerState();
                ChestMenuUI.SetState(ChestCustomizer);
            }
        }
        private void LoadTrailManager(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
        {
            if (!Initialized)
            {
                TrailManager = new TrailManager(Redemption.Instance);
                Initialized = true;
            }

            orig(self, gameTime);
        }
        public override void Unload()
        {
            On.Terraria.Main.Update -= LoadTrailManager;
        }

        public override void PreUpdateProjectiles()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                TrailManager.UpdateTrails();
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (RedeWorld.SkeletonInvasion)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
                if (index >= 0)
                {
                    LegacyGameInterfaceLayer SkeleUI = new("Redemption: SkeleInvasion",
                        delegate
                        {
                            DrawSkeletonInvasionUI(Main.spriteBatch);
                            return true;
                        },
                        InterfaceScaleType.UI);
                    layers.Insert(index, SkeleUI);
                }
            }
            layers.Insert(layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text")), new LegacyGameInterfaceLayer("GUI Menus",
                delegate
                {
                    if (ManualGeneratorMenu.Visible)
                    {
                        GeneratorMenuUI.Update(Main._drawInterfaceGameTime);
                        GeneratorMenu.Draw(Main.spriteBatch);
                    }

                    if (ChestCustomizerState.Visible)
                    {
                        ChestMenuUI.Update(Main._drawInterfaceGameTime);
                        ChestCustomizer.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                AddInterfaceLayer(layers, ChaliceUILayer, ChaliceUIElement, MouseTextIndex, ChaliceAlignmentUI.Visible, "Chalice");
                AddInterfaceLayer(layers, DialogueUILayer, DialogueUIElement, MouseTextIndex + 1, MoRDialogueUI.Visible, "Dialogue");
                AddInterfaceLayer(layers, TitleUILayer, TitleCardUIElement, MouseTextIndex + 2, TitleCard.Showing, "Title Card");
            }
        }

        public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null) //Code created by Scalie
        {
            string name;
            if (customName == null)
            {
                name = state.ToString();
            }
            else
            {
                name = customName;
            }
            layers.Insert(index, new LegacyGameInterfaceLayer("Redemption: " + name,
                delegate
                {
                    if (visible)
                    {
                        userInterface.Update(Main._drawInterfaceGameTime);
                        state.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI));
        }

        #region Skele Invasion UI
        public static void DrawSkeletonInvasionUI(SpriteBatch spriteBatch)
        {
            if (RedeWorld.SkeletonInvasion)
            {
                float alpha = 0.5f;
                Texture2D backGround1 = TextureAssets.ColorBar.Value;
                Texture2D progressColor = TextureAssets.ColorBar.Value;
                Texture2D InvIcon = ModContent.Request<Texture2D>("Redemption/Items/Armor/Vanity/EpidotrianSkull").Value;
                float scmp = 0.5f + 0.75f * 0.5f;
                Color descColor = new(77, 39, 135);
                Color waveColor = new(255, 241, 51);
                const int offsetX = 20;
                const int offsetY = 20;
                int width = (int)(200f * scmp);
                int height = (int)(46f * scmp);
                Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 23f), new Vector2(width, height));
                Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);
                float cleared = (float)Main.time / 16200;
                string waveText = "Cleared " + Math.Round(100 * cleared) + "%";
                Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y + 5), Color.White, scmp * 0.8f, 0.5f, -0.1f);
                Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
                Rectangle waveProgressAmount = new(0, 0, (int)(progressColor.Width * MathHelper.Clamp(cleared, 0f, 1f)), progressColor.Height);
                Vector2 offset = new((waveProgressBar.Width - (int)(waveProgressBar.Width * scmp)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scmp)) * 0.5f);
                spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
                spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
                const int internalOffset = 6;
                Vector2 descSize = new Vector2(154, 40) * scmp;
                Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 19f), new Vector2(width, height));
                Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize * .8f);
                Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);
                int descOffset = (descBackground.Height - (int)(32f * scmp)) / 2;
                Rectangle icon = new(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * scmp), (int)(32 * scmp));
                spriteBatch.Draw(InvIcon, icon, Color.White);
                Utils.DrawBorderString(spriteBatch, "Bone Party!", new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
            }
        }
        #endregion

        #region StructureHelper Draw
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem.ModItem is CopyWand)
            {
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                Texture2D tex = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/corner");
                Texture2D tex2 = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/box1");
                Point16 TopLeft = (Main.LocalPlayer.HeldItem.ModItem as CopyWand).TopLeft;
                int Width = (Main.LocalPlayer.HeldItem.ModItem as CopyWand).Width;
                int Height = (Main.LocalPlayer.HeldItem.ModItem as CopyWand).Height;

                float tileScale = 16 * Main.GameViewMatrix.Zoom.Length() * 0.707106688737f;
                Vector2 pos = (Main.MouseWorld / tileScale).ToPoint16().ToVector2() * tileScale - Main.screenPosition;
                pos = Vector2.Transform(pos, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
                pos = Vector2.Transform(pos, Main.UIScaleMatrix);

                spriteBatch.Draw(tex, pos, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);

                if (Width != 0 && TopLeft != null)
                {
                    spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.15f);
                    spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Red, 0, tex.Frame().Size() / 2, 1, 0, 0);
                }
                if (TopLeft != null) spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            }

            if (Main.LocalPlayer.HeldItem.ModItem is MultiWand)
            {
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                Texture2D tex = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/corner");
                Texture2D tex2 = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/box1");
                Point16 TopLeft = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).TopLeft;
                int Width = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).Width;
                int Height = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).Height;
                int count = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).StructureCache.Count;

                float tileScale = 16 * Main.GameViewMatrix.Zoom.Length() * 0.707106688737f;
                Vector2 pos = (Main.MouseWorld / tileScale).ToPoint16().ToVector2() * tileScale - Main.screenPosition;
                pos = Vector2.Transform(pos, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
                pos = Vector2.Transform(pos, Main.UIScaleMatrix);

                spriteBatch.Draw(tex, pos, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);

                if (Width != 0 && TopLeft != null)
                {
                    spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.15f);
                    spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Yellow, 0, tex.Frame().Size() / 2, 1, 0, 0);
                }
                if (TopLeft != null) spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.LimeGreen, 0, tex.Frame().Size() / 2, 1, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin();
                Utils.DrawBorderString(spriteBatch, "Structures to save: " + count, Main.MouseScreen + new Vector2(0, 30), Color.White);

                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            }
        }
        #endregion
    }
}