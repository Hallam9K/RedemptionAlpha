using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Graphics;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Redemption.Globals.RedeNet;

namespace Redemption.UI
{
    public class ChaliceAlignmentUI : UIState //This dialogue system is adapted from my title system and as such works very similarly - Seraph
    {
        public static ChaliceAlignmentUI Instance => RedeSystem.Instance.ChaliceUIElement;

        private string Text;
        private string Title = null;
        private float FadeTimer = 0;
        private int MaxFadeTime = 0;
        private float DisplayTimer = 0;
        private int MaxDisplayTime = 0;
        private float FontScale = 1;

        private float Shake = 0;
        private readonly int TextFont = 0;
        public Vector2? TextPos = null;
        public Color? TextColor = null;
        public Color? ShadowColor = null;
        public static bool Visible = false;

        /// <summary>
        /// Displays Chalice dialogue locally
        /// </summary>
        public static void DisplayDialogue(string text, int displayTime = 30, int fadeTime = 12, float shakeStrength = 0, Color? textColor = null, Color? shadowColor = null, Vector2? textPosition = null)
        {
            if(!RedeWorld.alignmentGiven)
                return;

            if (!Main.dedServ)
            {
                Instance.Text = text;
                Instance.Title = Language.GetTextValue("Mods.Redemption.UI.Chalice.Name") + ":";
                Instance.FadeTimer = 0;
                Instance.DisplayTimer = 0;
                Instance.MaxDisplayTime = displayTime;
                Instance.MaxFadeTime = fadeTime;
                Instance.FontScale = 0.6f;
                Instance.TextColor = textColor;
                Instance.ShadowColor = shadowColor;
                Instance.Shake = shakeStrength;
                Instance.TextPos = textPosition;
                Visible = true;   
            }
        }

        /// <summary>
        /// Displays Chalice dialogue to all the players. Can be called on clients or servers, you must make sure that only one of them calls it.
        /// </summary>
        public static void BroadcastDialogue(NetworkText text, int displayTime = 30, int fadeTime = 12, float shakeStrength = 0, Color? textColor = null, Color? shadowColor = null, Vector2? textPosition = null, int toClient = -1, int ignoreClient = -1)
        {
            if (!RedeWorld.alignmentGiven)
                return;

            if (Main.netMode != NetmodeID.Server)
            {
                DisplayDialogue(text.ToString(), displayTime, fadeTime, shakeStrength, textColor, shadowColor, textPosition);
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Redemption.Instance.GetPacket();
                packet.Write((byte)ModMessageType.SyncChaliceDialogue);
                text.Serialize(packet);
                packet.Write((ushort)displayTime);
                packet.Write((ushort)fadeTime);
                packet.Write(shakeStrength);

                packet.Write(textColor.HasValue);
                if (textColor.HasValue) packet.Write(textColor.Value.PackedValue);

                packet.Write(shadowColor.HasValue);
                if (shadowColor.HasValue) packet.Write(shadowColor.Value.PackedValue);

                packet.Write(textPosition.HasValue);
                if (textPosition.HasValue) packet.WriteVector2(textPosition.Value);

                packet.Send(toClient, ignoreClient);
            }
        }

        public static void ReceiveSyncChaliceDialogue(BinaryReader reader, int sender)
        {
            NetworkText text = NetworkText.Deserialize(reader);
            int displayTime = reader.ReadUInt16();
            int fadeTime = reader.ReadUInt16();
            float shakeStrength = reader.ReadSingle();

            Color? textColor = null;
            if (reader.ReadBoolean()) textColor = new() { PackedValue = reader.ReadUInt32() };

            Color? shadowColor = null;
            if (reader.ReadBoolean()) shadowColor = new() { PackedValue = reader.ReadUInt32() };

            Vector2? textPosition = null;
            if (reader.ReadBoolean()) textPosition = reader.ReadVector2();

            if (Main.netMode == NetmodeID.MultiplayerClient)
                DisplayDialogue(text.ToString(), displayTime, fadeTime, shakeStrength, textColor, shadowColor, textPosition);
            else
                BroadcastDialogue(text, displayTime, fadeTime, shakeStrength, textColor, shadowColor, textPosition, ignoreClient: sender);
        }

        public override void Update(GameTime gameTime)
        {
            float passedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Main.FrameSkipMode == FrameSkipMode.Subtle)
                passedTime = 1f / 60f;

            if (Visible)
            {
                if (DisplayTimer < MaxDisplayTime)
                {
                    if (FadeTimer < MaxFadeTime)
                        FadeTimer += passedTime * 60;
                    else
                        DisplayTimer += passedTime * 60;
                }
                else
                {
                    if (FadeTimer > 0)
                        FadeTimer -= passedTime * 60;
                    else
                        Visible = false;
                }
            }
        }
        public Vector2 lastScreenSize;
        public Vector2 screenPos;
        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            screenPos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2.3f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                screenPos = new(Main.screenWidth / 2, Main.screenHeight / 2.3f);
            }
            DynamicSpriteFont font = TextFont switch
            {
                1 => FontAssets.ItemStack.Value,
                2 => FontAssets.MouseText.Value,
                _ => FontAssets.DeathText.Value,
            };
            Vector2 CenterPosition;
            if (TextPos != null)
            {
                CenterPosition = (Vector2)TextPos;
            }
            else
            {
                CenterPosition = screenPos * Main.UIScale;
            }
            // * Main.UIScale
            int centerX = (int)CenterPosition.X;
            int centerY = (int)CenterPosition.Y;

            int textLength = (int)(font.MeasureString(Text).X * FontScale);
            int textHeight = (int)(font.MeasureString(Text).Y * FontScale);

            float opacity = FadeTimer / (float)MaxFadeTime;
            Color drawColor = new(255, 255, 255);
            Color shadowColor = new(25, 25, 25);

            Texture2D darkTexture = ModContent.Request<Texture2D>("Redemption/Textures/BlackSquare").Value;

            int titleDrawX = centerX - (textLength / 2);
            int titleDrawY = centerY - (int)(textHeight * 0.6f);

            int totalLength = textLength;
            int totalHeight = textHeight;

            int titleLength = 0;
            int titleHeight = 0;
            //* opacity
            Color textColor;
            Color textShadowColor;
            if (TextColor == null)
            {
                textColor = drawColor;
            }
            else
            {
                textColor = (Color)TextColor;
            }

            if (ShadowColor == null)
            {
                textShadowColor = shadowColor;
            }
            else
            {
                textShadowColor = (Color)ShadowColor;
            }

            if (Title != null)
            {
                titleLength = (int)(font.MeasureString(Title).X * (FontScale * 0.8f));
                titleHeight = (int)(font.MeasureString(Title).Y * (FontScale * 0.8f));
                totalLength = textLength + titleLength;
                totalHeight = textHeight + (titleHeight * 2);
                // totalLength = titleLength > textLength ? titleLength : textLength;

                // totalHeight = (int)(centerY + (float)(textHeight * 0.5f)) - (int)(titleDrawY + (float)(titleHeight * 0.5f));
            }

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.Additive);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend);

            Vector2 topleft = new(CenterPosition.X - (totalLength / 2), CenterPosition.Y - (totalHeight / 2));

            for (int i = 0; i < 20; ++i)
            {
                Vector2 fakeGaussianBlurEffect = new Vector2(20, 0).RotatedBy(MathHelper.TwoPi / 20f * i);
                Vector2 actualdrawposition = topleft + fakeGaussianBlurEffect;
                Rectangle rect = new((int)actualdrawposition.X, (int)actualdrawposition.Y, totalLength, totalHeight);

                spriteBatch.Draw(darkTexture, rect, darkTexture.Bounds, new Color(0, 0, 0) * (opacity * 0.02f));
            }
            Vector2 textpos = new Vector2(centerX - (textLength / 2f), centerY - (textHeight / 2f)) + new Vector2(Main.rand.NextFloat(0, Shake)).RotatedByRandom(MathHelper.TwoPi);
            spriteBatch.DrawString(font, Text, textpos + new Vector2(2, 2), textShadowColor * opacity, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, Text, textpos, textColor * opacity, 0, Vector2.Zero, new Vector2(FontScale));
            if (Title != null)
            {
                Vector2 titlepos = new(titleDrawX - (titleLength / 2f), titleDrawY - ((float)titleHeight));

                spriteBatch.DrawString(font, Title, titlepos + new Vector2(1, 1), textShadowColor * opacity, 0, new Vector2(0, 0), FontScale * 0.7f, SpriteEffects.None, 0);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, Title, titlepos, textColor * opacity, 0, Vector2.Zero, new Vector2(FontScale) * 0.7f);

            }
            //font, SubtitleText, new Vector2(subtitleDrawX - ((float)subtitleLength / 2f), subtitleDrawY - ((float)subtitleHeight / 2f)), textColor * opacity
        }
    }
}
