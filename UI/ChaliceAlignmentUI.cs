using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class ChaliceAlignmentUI : UIState
    {

        //This dialogue system is adapted from my title system and as such works very similarly - Seraph
        private string Text;
        private string Title = null;
        private int FadeTimer = 0;
        private int MaxFadeTime = 0;
        private int DisplayTimer = 0;
        private int MaxDisplayTime = 0;
        private float FontScale = 1;

        private float Shake = 0;
        public int ID = 0;
        private readonly int TextFont = 0;
        public Vector2? TextPos = null;
        public Color? TextColor = null;
        public Color? ShadowColor = null;
        public static bool Visible = false;

        public void DisplayDialogue(string text, int displayTime = 30, int fadeTime = 12, float shakestrength = 0, Color? textColor = null, Color? shadowColor = null, Vector2? textPosition = null, int font = 0, int id = 0)
        {
            if (!Main.dedServ)
            {
                Text = text;
                Title = Language.GetTextValue("Mods.Redemption.UI.Chalice.Name") + ":";
                FadeTimer = 0;
                DisplayTimer = 0;
                MaxDisplayTime = displayTime;
                MaxFadeTime = fadeTime;
                FontScale = 0.6f;
                TextColor = textColor;
                ShadowColor = shadowColor;
                Shake = shakestrength;
                TextPos = textPosition;
                ID = id;
                Visible = true;
            }
        }
        public void HandleTimer()
        {
            if (Visible)
            {
                if (DisplayTimer < MaxDisplayTime)
                {
                    if (FadeTimer < MaxFadeTime)
                    {
                        ++FadeTimer;
                    }
                    else
                    {
                        ++DisplayTimer;
                    }
                }
                else
                {
                    if (FadeTimer > 0)
                    {
                        --FadeTimer;
                    }
                    else
                    {
                        Visible = false;
                    }
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
            spriteBatch.DrawString(font, Text, textpos, textColor * opacity, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
            if (Title != null)
            {
                Vector2 titlepos = new(titleDrawX - (titleLength / 2f), titleDrawY - ((float)titleHeight));

                spriteBatch.DrawString(font, Title, titlepos + new Vector2(1, 1), textShadowColor * opacity, 0, new Vector2(0, 0), FontScale * 0.7f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, Title, titlepos, textColor * opacity, 0, new Vector2(0, 0), FontScale * 0.7f, SpriteEffects.None, 0);

            }
            //font, SubtitleText, new Vector2(subtitleDrawX - ((float)subtitleLength / 2f), subtitleDrawY - ((float)subtitleHeight / 2f)), textColor * opacity
            HandleTimer();
        }
    }
}
