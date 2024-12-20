using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Redemption.Globals.RedeNet;

namespace Redemption.UI
{
    public class TitleCard : UIState // Code by Seraph
    {
        public static TitleCard Instance => RedeSystem.Instance.TitleCardUIElement;

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

        /// <summary>
        /// Displays title card locally
        /// </summary>
        public static void DisplayTitle(string text, int displayTime = 1, int fadeTime = 120, float fontScale = 1, Color? altColor = null, string subtitle = null)
        {
            if (!RedeConfigClient.Instance.NoBossIntroText && !Main.dedServ)
            {  
                Instance.Text = text;
                Instance.SubtitleText = subtitle;
                Instance.FadeTimer = 0;
                Instance.DisplayTimer = 0;
                Instance.MaxDisplayTime = displayTime;
                Instance.MaxFadeTime = fadeTime;
                Instance.FontScale = fontScale;
                Instance.TextColor = altColor;
                Showing = true;
            }
        }

        /// <summary>
        /// Triggers title card display for all the players. Doesn't do anything on MP clients, only use on servers and SP.
        /// </summary>
        public static void BroadcastTitle(NetworkText text, int displayTime = 1, int fadeTime = 120, float fontScale = 1, Color? altColor = null, NetworkText subtitle = null)
        {
            if(Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Redemption.Instance.GetPacket();
                packet.Write((byte)ModMessageType.TitleCardFromServer);
                text.Serialize(packet);
                packet.Write((ushort)displayTime);
                packet.Write((ushort)fadeTime);
                packet.Write(fontScale);

                packet.Write(altColor.HasValue);
                if (altColor.HasValue) packet.Write(altColor.Value.PackedValue);

                packet.Write(subtitle != null);
                if (subtitle is not null) subtitle.Serialize(packet);

                packet.Send();
            }
            else if(Main.netMode == NetmodeID.SinglePlayer)
            {
                DisplayTitle(text.ToString(), displayTime, fadeTime, fontScale, altColor, subtitle?.ToString());
            }
        }

        public static void ReceiveTitleCardFromServer(BinaryReader reader, int sender)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            string text = NetworkText.Deserialize(reader).ToString();
            int displayTime = reader.ReadUInt16();
            int fadeTime = reader.ReadUInt16();
            float fontScale = reader.ReadSingle();

            Color? altColor = null;
            if (reader.ReadBoolean()) altColor = new() { PackedValue = reader.ReadUInt32() };

            string subtitle = null;
            if (reader.ReadBoolean()) subtitle = NetworkText.Deserialize(reader).ToString();

            if (Main.netMode == NetmodeID.MultiplayerClient)
                DisplayTitle(text, displayTime, fadeTime, fontScale, altColor, subtitle);
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

            float opacity = FadeTimer / (float)MaxFadeTime;
            Color drawColor = new(255, 255, 255);
            Color shadowColor = new(25, 25, 25);
            int totalLength = textLength;

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
            }

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend);

            //draw line
            for (int i = centerX - (totalLength / 2) + (totalLength / 2 % 38); i <= centerX + (totalLength / 2) - (totalLength / 2 % 38); i += 38)
            {
                Vector2 lineDrawPos = new(i, centerY);
                spriteBatch.Draw(lineTexture, lineDrawPos, lineTexture.Bounds, drawColor * opacity, 0, RedeHelper.GetOrigin(lineTexture), FontScale, SpriteEffects.None, 0);
            }
            //draw line endings
            Vector2 leftEndPos = new(centerX - (totalLength / 2) + (totalLength / 2 % 38) - 23, centerY);
            spriteBatch.Draw(leftEndTexture, leftEndPos, leftEndTexture.Bounds, drawColor * opacity, 0, RedeHelper.GetOrigin(leftEndTexture), FontScale, SpriteEffects.None, 0);

            Vector2 rightEndPos = new(centerX + (totalLength / 2) - (totalLength / 2 % 38) + 23, centerY);
            spriteBatch.Draw(rightEndTexture, rightEndPos, rightEndTexture.Bounds, drawColor * opacity, 0, RedeHelper.GetOrigin(rightEndTexture), FontScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend);
            Vector2 textpos = new(centerX - (textLength / 2f), centerY - (textHeight / 2f));
            spriteBatch.DrawString(font, Text, textpos + new Vector2(2, 2), shadowColor * opacity, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, Text, textpos, textColor * opacity, 0, Vector2.Zero, new Vector2(FontScale));

            if (SubtitleText != null)
            {
                Vector2 subtitlepos = new(subtitleDrawX - (subtitleLength / 2f), subtitleDrawY - (subtitleHeight / 2f));

                spriteBatch.DrawString(font, SubtitleText, subtitlepos + new Vector2(1, 1), shadowColor * opacity, 0, new Vector2(0, 0), FontScale * 0.5f, SpriteEffects.None, 0);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, SubtitleText, subtitlepos, textColor * opacity, 0, Vector2.Zero, new Vector2(FontScale) * 0.5f);

            }
            //font, SubtitleText, new Vector2(subtitleDrawX - ((float)subtitleLength / 2f), subtitleDrawY - ((float)subtitleHeight / 2f)), textColor * opacity
            HandleTimer();
        }
    }
}