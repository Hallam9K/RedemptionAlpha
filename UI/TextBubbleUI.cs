using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.ModLoader;
using ReLogic.Graphics;
using System;
using Terraria.Audio;
using static Redemption.UI.Dialogue;
using IL.Terraria.DataStructures;
using Terraria.UI.Chat;

namespace Redemption.UI
{
    public class TextBubbleUI : UIState
    {
        public static List<IDialogue> Dialogue;

        public Texture2D LidenTex;
        public Texture2D EpidotraTex;
        public Texture2D KingdomTex;
        public Texture2D BoxTex;

        public static bool Visible = true;

        public override void OnInitialize()
        {
            Dialogue = new();

            LidenTex = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Liden", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            EpidotraTex = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            KingdomTex = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Kingdom", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public static void Add(IDialogue dialogue)
        {
            Dialogue.Add(dialogue);
        }
        public static void Remove(IDialogue dialogue)
        {
            Dialogue.Remove(dialogue);
        }
        public static void Clear()
        {
            Dialogue.Clear();
        }
        public override void Update(GameTime gameTime)
        {
            if (!Visible || Dialogue.Count == 0)
                return;

            base.Update(gameTime);

            for (int i = 0; i < Dialogue.Count; i++)
            {
                IDialogue dialogue = Dialogue[i];
                dialogue.Update();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || Dialogue.Count == 0)
                return;

            for (int i = 0; i < Dialogue.Count; i++)
            {
                Dialogue dialogue = Dialogue[i].Get();

                string[] drawnText = FormatText(dialogue.displayingText, dialogue.font, out int width, out int height);
                Vector2 pos = (dialogue.chain == null ? Vector2.Zero : dialogue.chain.modifier) + dialogue.modifier + (dialogue.entity != null ? dialogue.entity.Center - Main.screenPosition - new Vector2((width + 68f) / 2f, -dialogue.entity.height) : dialogue.chain.anchor != null ? dialogue.chain.anchor.VisualPosition : new Vector2(Main.screenWidth / 2f - width / 2f, Main.screenHeight * 0.8f - height / 2f));

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

                float alpha = 1f;
                if (dialogue.boxFade)
                {
                    float quotient = !dialogue.textFinished ? 0f : MathHelper.Lerp(1f, 0f, (float)dialogue.fadeTime / dialogue.fadeTimeMax);
                    alpha = MathHelper.Lerp(1f, 0f, quotient);
                }

                DrawPanel(spriteBatch, dialogue.bubble, pos, Color.Multiply(Color.White, alpha), width, height);

                Vector2 textPos = pos + new Vector2(17f, 17f);
                for (int k = 0; k < drawnText.Length; k++)
                {
                    string text = drawnText[k];
                    if (text == null)
                        continue;

                    DrawStringEightWay(spriteBatch, text, 1, textPos, Color.Multiply(dialogue.textColor, alpha), Color.Multiply(dialogue.shadowColor, alpha));

                    textPos.Y += dialogue.font.MeasureString(text).Y - 6;
                }
            }
        }
        public static void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, int width, int height)
        {
            // Top Left
            Vector2 topLeftPos = position;
            Rectangle topLeftRect = new(0, 0, 34, 34);
            spriteBatch.Draw(texture, topLeftPos, topLeftRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Top Middle
            Rectangle topMiddlePos = new((int)topLeftPos.X + topLeftRect.Width, (int)topLeftPos.Y, width, 34);
            Rectangle topMiddleRect = new(34, 0, 2, 34);
            spriteBatch.Draw(texture, topMiddlePos, topMiddleRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Top Right
            Vector2 topRightPos = topMiddlePos.TopRight();
            Rectangle topRightRect = new(36, 0, 34, 34);
            spriteBatch.Draw(texture, topRightPos, topRightRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


            // Middle Left
            Rectangle middleLeftDest = new((int)topLeftPos.X, (int)topLeftPos.Y + topLeftRect.Height, 34, height - 34);
            Rectangle middleLeftRect = new(0, 34, 34, 2);
            spriteBatch.Draw(texture, middleLeftDest, middleLeftRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Middle Middle
            Rectangle middleMiddleDest = new((int)topLeftPos.X + topLeftRect.Width, (int)topLeftPos.Y + topLeftRect.Height, width, height - 34);
            Rectangle middleMiddleRect = new(34, 34, 2, 2);
            spriteBatch.Draw(texture, middleMiddleDest, middleMiddleRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Middle Right
            Rectangle middleRightDest = new((int)topRightPos.X, (int)topRightPos.Y + topRightRect.Height, 34, height - 34);
            Rectangle middleRightRect = new(36, 34, 34, 2);
            spriteBatch.Draw(texture, middleRightDest, middleRightRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);


            // Bottom Left
            Vector2 bottomLeftPos = middleLeftDest.BottomLeft();
            Rectangle bottomLeftRect = new(0, 36, 34, 34);
            spriteBatch.Draw(texture, bottomLeftPos, bottomLeftRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Bottom Middle
            Rectangle bottomMiddlePos = new((int)bottomLeftPos.X + bottomLeftRect.Width, (int)bottomLeftPos.Y, width, 34);
            Rectangle bottomMiddleRect = new(34, 36, 2, 34);
            spriteBatch.Draw(texture, bottomMiddlePos, bottomMiddleRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Bottom Right
            Vector2 bottomRightPos = middleRightDest.BottomLeft();
            Rectangle bottomRightRect = new(36, 36, 34, 34);
            spriteBatch.Draw(texture, bottomRightPos, bottomRightRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        public static string[] FormatText(string text, DynamicSpriteFont font, out int width, out int height)
        {
            // Measure the width of the text so that we can stretch the bubble
            width = (int)font.MeasureString(text).X;
            if (width > 300)
                width = 300;

            height = 0;

            string[] displayingText = Utils.WordwrapString(text, font, width, 10, out int lines);

            int largestWidth = 0;
            for (int i = 0; i < displayingText.Length; i++)
            {
                if (displayingText?[i] == null)
                    continue;
                Vector2 stringSize = font.MeasureString(displayingText[i]);
                if (stringSize.X > largestWidth)
                    largestWidth = (int)stringSize.X;
                height += (int)stringSize.Y - 6;
            }

            width = largestWidth - 34;
            if (height < 22)
                height = 22;

            return displayingText;
        }
        public static void InterpretSymbols(Dialogue dialogue)
        {
            if (dialogue.displayingText.Length == 0)
                return;

            char trigger = dialogue.displayingText[^1];

            if (trigger == '[')
            {
                int index = dialogue.text.IndexOf(']', dialogue.displayingText.Length);
                int length = dialogue.displayingText.Length - 1;
                string text = dialogue.text[length..(index + 1)];
                string numbers = text[1..^1];

                if (numbers[0] == '@')
                {
                    dialogue.TriggerSymbol(numbers[1..]);
                    dialogue.chain.TriggerSymbol(numbers[1..]);
                    dialogue.text = dialogue.text.Replace(text, "");
                    dialogue.displayingText = dialogue.displayingText[0..^1];
                    return;
                }

                bool parsed = int.TryParse(numbers, out int result);
                if (!parsed) return;

                dialogue.pauseTime = result;
                dialogue.text = dialogue.text.Replace(text, "");
                dialogue.displayingText = dialogue.displayingText[0..^1];
                return;
            }

            if (trigger == '^')
            {
                int index = dialogue.text.IndexOf('^', dialogue.displayingText.Length);
                int length = dialogue.displayingText.Length - 1;
                string text = dialogue.text[length..(index + 1)];
                string numbers = text[1..^1];
                int.TryParse(numbers, out int result);

                dialogue.charTime = result;
                dialogue.text = dialogue.text.Replace(text, "");
                dialogue.displayingText = dialogue.displayingText[0..^1];
                return;
            }
        }
        public static void DrawStringEightWay(SpriteBatch spriteBatch, string text, int thickness, Vector2 position, Color textColor, Color shadowColor)
        {
            ChatManager.DrawColorCodedStringShadow(spriteBatch, Terraria.GameContent.FontAssets.MouseText.Value, text, position, shadowColor * .5f, 0, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.MouseText.Value, text, position, textColor, 0, Vector2.Zero, Vector2.One);
        }
    }

    public interface IDialogue
    {
        public void Update();
        public Dialogue Get();
    }

    public class DialogueChain : IDialogue
    {
        public delegate void SymbolTrigger(Dialogue dialogue, string signature);
        public event SymbolTrigger OnSymbolTrigger;
        public delegate void EndTrigger(Dialogue dialogue, int id);
        public event EndTrigger OnEndTrigger;
        public List<Dialogue> Dialogue;
        public Entity anchor;
        public Vector2 modifier;

        public DialogueChain(Entity anchor = null, Vector2 modifier = default)
        {
            Dialogue = new List<Dialogue>();
            this.anchor = anchor;
            this.modifier = modifier;
        }
        public void Update()
        {
            Dialogue[0].Update();
            if (Dialogue.Count == 0)
                TextBubbleUI.Dialogue.Remove(this);
        }
        public DialogueChain Add(Dialogue dialogue)
        {
            dialogue.chain = this;
            Dialogue.Add(dialogue);
            return this;
        }
        public Dialogue Get() => Dialogue[0];
        public void TriggerSymbol(string signature) => OnSymbolTrigger?.Invoke(Dialogue[0], signature);
        public void TriggerEnd(int ID) => OnEndTrigger?.Invoke(Dialogue[0], ID);
    }

    public class Dialogue : IDialogue
    {
        public DialogueChain chain;
        public delegate void SymbolTrigger(Dialogue dialogue, string signature);
        public event SymbolTrigger OnSymbolTrigger;
        public delegate void EndTrigger(Dialogue dialogue, int id);
        public event EndTrigger OnEndTrigger;

        public Entity entity;
        public Color textColor;
        public Color shadowColor;
        public SoundStyle? sound;

        public Texture2D icon;
        public Texture2D bubble;
        public DynamicSpriteFont font;
        public Vector2 modifier;

        public bool boxFade;
        public bool textFinished;

        public string text;
        public string displayingText;

        public int timer;
        public int charTime;
        public int pauseTime;
        public int preFadeTime;
        public int fadeTime;
        public int fadeTimeMax;
        public int endID;

        public Dialogue(Entity entity, string text, Color? textColor = null, Color? shadowColor = null, SoundStyle? sound = null, int charTime = 3, int preFadeTime = 100, int fadeTime = 30, bool boxFade = false, Texture2D icon = null, Texture2D bubble = null, DynamicSpriteFont font = null, Vector2 modifier = default, int endID = 0)
        {
            this.entity = entity;
            this.text = text ?? "";
            displayingText ??= "";

            this.textColor = textColor ?? Color.White;
            this.shadowColor = shadowColor ?? Color.Black;
            this.sound = sound ?? CustomSounds.Voice1;
            this.charTime = charTime - RedeConfigClient.Instance.DialogueSpeed;
            this.preFadeTime = preFadeTime + RedeConfigClient.Instance.DialogueWaitTime;
            this.fadeTime = fadeTime;
            fadeTimeMax = fadeTime;
            this.boxFade = boxFade;
            this.icon = icon;
            this.bubble = bubble ?? ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Liden").Value;
            this.font = font ?? Terraria.GameContent.FontAssets.MouseText.Value;
            this.modifier = modifier;
            this.endID = endID;
            if (this.charTime < 1)
                this.charTime = 1;
        }
        public void Update()
        {
            if (Main.gamePaused)
                return;
            // Measure our progress via a modulo between our char time and
            // our timer, allowing us to decide how many chars to display
            if (pauseTime <= 0 && displayingText.Length != text.Length && timer % charTime == 0)
            {
                SoundEngine.PlaySound((SoundStyle)sound, entity.position);
                displayingText += text[displayingText.Length];
            }

            TextBubbleUI.InterpretSymbols(this);

            if (displayingText.Length == text.Length)
                textFinished = true;

            if ((textFinished && preFadeTime <= 0 && fadeTime <= 0) || !entity.active)
            {
                if (endID > 0)
                {
                    TriggerEnd(endID);
                    chain.TriggerEnd(endID);
                }
                if (chain != null)
                    chain.Dialogue.Remove(this);
                else
                    TextBubbleUI.Dialogue.Remove(this);
            }

            pauseTime--;
            if (pauseTime <= 0)
                timer++;
            if (textFinished)
                preFadeTime--;
            if (preFadeTime <= 0)
                fadeTime--;
        }
        public Dialogue Get() => this;
        public void TriggerSymbol(string signature) => OnSymbolTrigger?.Invoke(this, signature);
        public void TriggerEnd(int ID) => OnEndTrigger?.Invoke(this, ID);
    }
}
