using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;


namespace Redemption.UI
{
    public class TitleCard : UIState // Code by Seraph
    {
        private string Text;
        private string SubtitleText = null;
        private int FadeTimer = 0;
        private int MaxFadeTime = 0;
        private int DisplayTimer = 0;
        private int MaxDisplayTime = 0;
        private float FontScale = 1;
        private readonly int TextFont = 0;
        private Color? TextColor = null;
        public static bool Showing = false;

        public void DisplayTitle(string text, int displayTime = 1, int fadeTime = 120, float fontScale = 1, int font = 0, Color? altColor = null, string subtitle = null)
        {
            if (RedeConfigClient.Instance.BossIntroText && !Main.dedServ)
            {
                Text = text;
                SubtitleText = subtitle;
                FadeTimer = 0;
                DisplayTimer = 0;
                MaxDisplayTime = displayTime;
                MaxFadeTime = fadeTime;
                FontScale = fontScale;
                TextColor = altColor;
                Showing = true;
            }
        }

        public void HandleTimer()
        {
            if (Showing)
            {
                if (DisplayTimer < MaxDisplayTime)
                {
                    if (FadeTimer < MaxFadeTime)
                        ++FadeTimer;
                    else
                        ++DisplayTimer;
                }
                else
                {
                    if (FadeTimer > 0)
                        --FadeTimer;
                    else
                        Showing = false;
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Showing)
                return;

            Texture2D lineTexture = ModContent.Request<Texture2D>("Redemption/Textures/TitleLine").Value;
            Texture2D leftEndTexture = ModContent.Request<Texture2D>("Redemption/Textures/TitleLineEndLeft").Value;
            Texture2D rightEndTexture = ModContent.Request<Texture2D>("Redemption/Textures/TitleLineEndRight").Value;
            DynamicSpriteFont font = TextFont switch
            {
                1 => FontAssets.ItemStack.Value,
                2 => FontAssets.MouseText.Value,
                _ => FontAssets.DeathText.Value,
            };
            // * Main.UIScale
            int centerX = (int)(Main.screenWidth * Main.UIScale) / 2;
            int centerY = (int)(Main.screenHeight * Main.UIScale) / 4;

            int textLength = (int)(font.MeasureString(Text).X * FontScale);
            int textHeight = (int)(font.MeasureString(Text).Y * FontScale);

            float opacity = (FadeTimer / (float)MaxFadeTime);
            Color drawColor = (new Color(255, 255, 255));
            Color shadowColor = (new Color(25, 25, 25));
            int totalLength = textLength;
            int totalHeight = textHeight;

            int subtitleDrawX = centerX;
            int subtitleDrawY = centerY - (int)(textHeight * 0.6f);

            int subtitleLength = 0;
            int subtitleHeight = 0;
            //* opacity
            Color textColor;
            if (TextColor == null)
            {
                textColor = drawColor;
            }
            else
            {
                textColor = (Color)TextColor;
            }

            if (SubtitleText != null)
            {
                subtitleLength = (int)(font.MeasureString(SubtitleText).X * (FontScale * 0.5f));
                subtitleHeight = (int)(font.MeasureString(SubtitleText).Y * (FontScale * 0.5f));

                totalLength = subtitleLength > textLength ? subtitleLength : textLength;

                totalHeight = (int)(centerY + (textHeight * 0.5f)) - (int)(subtitleDrawY + (subtitleHeight * 0.5f));
            }

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend);

            //draw line
            for (int i = centerX - (totalLength / 2) + (totalLength / 2 % 38); i <= centerX + (totalLength / 2) - ((totalLength / 2) % 38); i += 38)
            {
                Vector2 lineDrawPos = new(i, centerY);
                spriteBatch.Draw(lineTexture, lineDrawPos, lineTexture.Bounds, drawColor * opacity, 0, RedeHelper.GetOrigin(lineTexture), FontScale, SpriteEffects.None, 0);
            }
            //draw line endings
            Vector2 leftEndPos = new(centerX - (totalLength / 2) + ((totalLength / 2) % 38) - 23, centerY);
            spriteBatch.Draw(leftEndTexture, leftEndPos, leftEndTexture.Bounds, drawColor * opacity, 0, RedeHelper.GetOrigin(leftEndTexture), FontScale, SpriteEffects.None, 0);

            Vector2 rightEndPos = new(centerX + (totalLength / 2) - ((totalLength / 2) % 38) + 23, centerY);
            spriteBatch.Draw(rightEndTexture, rightEndPos, rightEndTexture.Bounds, drawColor * opacity, 0, RedeHelper.GetOrigin(rightEndTexture), FontScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend);
            Vector2 textpos = new(centerX - (textLength / 2f), centerY - (textHeight / 2f));
            spriteBatch.DrawString(font, Text, textpos + new Vector2(2, 2), shadowColor * opacity, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, Text, textpos, textColor * opacity, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);

            if (SubtitleText != null)
            {
                Vector2 subtitlepos = new(subtitleDrawX - (subtitleLength / 2f), subtitleDrawY - (subtitleHeight / 2f));

                spriteBatch.DrawString(font, SubtitleText, subtitlepos + new Vector2(1, 1), shadowColor * opacity, 0, new Vector2(0, 0), FontScale * 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, SubtitleText, subtitlepos, textColor * opacity, 0, new Vector2(0, 0), FontScale * 0.5f, SpriteEffects.None, 0);

            }
            //font, SubtitleText, new Vector2(subtitleDrawX - ((float)subtitleLength / 2f), subtitleDrawY - ((float)subtitleHeight / 2f)), textColor * opacity
            HandleTimer();
        }
    }
}