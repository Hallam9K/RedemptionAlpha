using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class MoRDialogueUI : UIState
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
        public Vector2? PointPos = null;
        public Color? TextColor = null;
        public Color? ShadowColor = null;
        public static bool Visible = false;

        public void DisplayDialogue(string text, int displayTime = 30, int fadeTime = 12, float fontScale = 1, string whosespeaking = null, float shakestrength = 0, Color? textColor = null, Color? shadowColor = null, Vector2? textPosition = null, Vector2? speakerPosition = null, int font = 0, int id = 0, bool sound = false)
        {
            if (!Main.dedServ)
            {
                //Most of the arguments should be self explanetory but here's an overview regardless
                /*
                 * PRIVATE = can only be set by this method
                 * PUBLIC = can be set outside this method
                 * 
                 * 
                 * text - The dialogue - PRIVATE
                 * displayTime - how long the dialogue shows up (must be positive or things will break) - PRIVATE
                 * fadetime - how long the dialogue takes to fade in and out (total time visible is displayTime + 2*Fadetime) set to 1 if you don't want fade - PRIVATE
                 * fontScale - relative text size to baseline - PRIVATE
                 * whosespeaking - what the "X" above the dialogue says, colon (:) symbol has to be added manually, won't display if the text is null - PRIVATE
                 * shakestrength - how much the dialogue shakes (added on request) - PRIVATE
                 * textColor - color of the text, defaults to white - PUBLIC can be changedd externally if you want rainbow text or something cool like that
                 * shadowColor - color of the shadow text, defaults to black - PUBLIC can be changedd externally if you want rainbow text or something cool like that
                 * textPosition - where (relative to the top left of the screen) the dialogue draws, defaults to the center and 1/3rd down from the top - PUBLIC 
                 * spwakerPosition - where the speech bubble arrow points to (not relative to the screen, relative to the world instead) - PUBLIC
                 */
                if (sound)
                    SoundEngine.PlaySound(SoundID.MenuTick);

                Text = text;
                Title = whosespeaking;
                FadeTimer = 0;
                DisplayTimer = 0;
                MaxDisplayTime = displayTime;
                MaxFadeTime = fadeTime;
                FontScale = fontScale;
                PointPos = speakerPosition;
                TextColor = textColor;
                ShadowColor = shadowColor;
                Shake = shakestrength;
                TextPos = textPosition;
                ID = id;
                Visible = true;
            }
        }
        public Vector2 lastScreenSize;
        public Vector2 screenPos;
        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            screenPos = new(Main.screenWidth / 2, Main.screenHeight / 3);
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;
            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                screenPos = new(Main.screenWidth / 2, Main.screenHeight / 3);
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

            Texture2D arrowTexture = ModContent.Request<Texture2D>("Redemption/Textures/MoRDialogueArrow").Value;
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
                titleLength = (int)(font.MeasureString(Title).X * (FontScale * 0.7f));
                titleHeight = (int)(font.MeasureString(Title).Y * (FontScale * 0.7f));
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
            if (PointPos != null)
            {
                Vector2 arrowOffset = new Vector2(1.41421356f, 0).RotatedBy(((Vector2)(PointPos - Main.screenPosition) - CenterPosition).ToRotation());
                if (Math.Abs(arrowOffset.X) > 1)
                {
                    bool a = arrowOffset.X >= 0;
                    arrowOffset.X = a.ToDirectionInt();
                }
                if (Math.Abs(arrowOffset.Y) > 1)
                {
                    bool a = arrowOffset.Y >= 0;
                    arrowOffset.Y = a.ToDirectionInt();
                }
                arrowOffset.X *= (totalLength / 2) + 30;
                arrowOffset.Y *= (totalHeight / 2) + 30;

                float rot = ((Vector2)(PointPos - Main.screenPosition) - (CenterPosition + arrowOffset)).ToRotation();

                spriteBatch.Draw(arrowTexture, CenterPosition + arrowOffset, arrowTexture.Bounds, Color.White * opacity * 0.5f, rot, new Vector2(arrowTexture.Width / 2, arrowTexture.Height / 2), 1, SpriteEffects.None, 0);
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
