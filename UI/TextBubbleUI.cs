using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.ModLoader;
using System.Linq;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using System;

namespace Redemption.UI
{
	public class TextBubbleUI : UIState
	{
		public static List<Dialogue> Dialogue;

		public Texture2D LidenTex;
		public Texture2D EpidotraTex;
		public Texture2D BoxTex;

		public static bool Visible = true;

		public override void OnInitialize()
		{
			Dialogue = new();

			LidenTex = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Liden", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			EpidotraTex = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		}
		public static void AddDialogue(Dialogue dialogue)
		{
			Dialogue.Add(dialogue);
		}
		public static void RemoveDialogue(Dialogue dialogue)
		{
			Dialogue.Remove(dialogue);
		}
		public static void ClearDialogue()
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
				Dialogue dialogue = Dialogue[i];

				if (dialogue.leader != null)
					continue;

				// Measure our progress via a modulo between our char time and
				// our timer, allowing us to decide how many chars to display
				if (dialogue.pauseTime <= 0 && dialogue.displayingText.Length != dialogue.text.Length && dialogue.timer % dialogue.charTime == 0)
					dialogue.displayingText += dialogue.text[dialogue.displayingText.Length];

				InterpretSymbols(ref dialogue);

				if (dialogue.displayingText.Length == dialogue.text.Length)
					dialogue.textFinished = true;

				if (dialogue.textFinished && dialogue.endPauseTime <= 0 && dialogue.fadeTime <= 0)
				{
					for (int k = 0; k < Dialogue.Count; k++)
						if (Dialogue[k].leader == dialogue)
							Dialogue[k].leader = null;
					Dialogue.Remove(dialogue);
				}
				
				dialogue.pauseTime--;
				if(dialogue.pauseTime <= 0)
					dialogue.timer++;
				if (dialogue.textFinished)
					dialogue.endPauseTime--;
				if (dialogue.endPauseTime <= 0)
					dialogue.fadeTime--;
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible || Dialogue.Count == 0)
				return;

			for (int i = 0; i < Dialogue.Count; i++)
			{
				Dialogue dialogue = Dialogue[i];

				if (dialogue.leader != null)
					continue;

				string[] drawnText = FormatText(dialogue.displayingText, dialogue.font, out int width, out int height);
				Vector2 pos = dialogue.npc != null ? dialogue.npc.Center - Main.screenPosition - new Vector2((width + 68f) / 2f, -dialogue.npc.height) : new Vector2(Main.screenWidth / 2f - width / 2f, Main.screenHeight * 0.8f - height / 2f);

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
		public static void InterpretSymbols(ref Dialogue dialogue)
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
				int.TryParse(numbers, out int result);

				dialogue.pauseTime = result;
				dialogue.text = dialogue.text.Replace(text, "");
				dialogue.displayingText = dialogue.displayingText[0..^1];
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
			}
		}
		public static void DrawStringEightWay(SpriteBatch spriteBatch, string text, int thickness, Vector2 position, Color textColor, Color shadowColor)
		{
			for (int i = -thickness; i <= thickness; i++)
			{
				for (int k = -thickness; k <= thickness; k++)
				{
					if (i == 0 && k == 0) continue;
					float alpha = MathHelper.Lerp(1f, 0f, Math.Abs((i + k) / 2f));
					spriteBatch.DrawString(Terraria.GameContent.FontAssets.MouseText.Value, text, position + new Vector2(i, k), Color.Multiply(shadowColor, alpha));
				}
			}
			spriteBatch.DrawString(Terraria.GameContent.FontAssets.MouseText.Value, text, position, textColor);
		}
	}
	public class Dialogue
	{
		public NPC npc;
		public Vector2 position;
		public Texture2D icon;
		public Texture2D bubble;
		public DynamicSpriteFont font;
		public Color textColor;
		public Color shadowColor;

		public Dialogue leader;

		public string text;
		public int timer;
		public int charTime;
		public int endPauseTime;
		public int fadeTime;
		public bool boxFade;

		public string displayingText;
		public bool textFinished;
		public int fadeTimeMax;
		public int pauseTime;

		public Dialogue(string text)
		{
			npc = null;
			icon = null;
			bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Liden").Value;
			font = Terraria.GameContent.FontAssets.MouseText.Value;
			textColor = Color.White;
			shadowColor = Color.Black;
			leader = null;
			this.text = text ?? "";
			displayingText ??= "";
			charTime = 6;
			endPauseTime = 60;
			fadeTime = 60;
			fadeTimeMax = fadeTime;
			boxFade = true;
		}
		public Dialogue(Dialogue dialogue)
		{
			npc = dialogue.npc;
			icon = dialogue.icon;
			bubble = dialogue.bubble ?? ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Liden").Value;
			font = dialogue.font ?? Terraria.GameContent.FontAssets.MouseText.Value;
			textColor = dialogue.textColor;
			shadowColor = dialogue.shadowColor;
			leader = dialogue.leader;
			displayingText ??= "";
			charTime = dialogue.charTime;
			endPauseTime = dialogue.endPauseTime;
			fadeTime = dialogue.fadeTime;
			fadeTimeMax = dialogue.fadeTime;
			boxFade = dialogue.boxFade;
		}
		public Dialogue(NPC npc, Texture2D icon, Texture2D bubble, DynamicSpriteFont font, Color textColor, Color shadowColor, Dialogue leader, string text, int charTime, int pauseTime, int fadeTime, bool boxFade)
		{
			this.npc = npc;
			this.icon = icon;
			this.bubble = bubble ?? ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Liden").Value;
			this.font = font ?? Terraria.GameContent.FontAssets.MouseText.Value;
			this.textColor = textColor;
			this.shadowColor = shadowColor;
			this.leader = leader;
			this.text = text ?? "";
			displayingText ??= "";
			this.charTime = charTime;
			this.endPauseTime = pauseTime;
			this.fadeTime = fadeTime;
			fadeTimeMax = fadeTime;
			this.boxFade = boxFade;
		}
	}
}
