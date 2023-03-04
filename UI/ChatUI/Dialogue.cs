using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;

namespace Redemption.UI.ChatUI
{
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

		public float timer;
		public float charTime;
		public float pauseTime;
		public float preFadeTime;
		public float fadeTime;
		public float fadeTimeMax;
		public int endID;

		public Dialogue(Entity entity, string text, Color? textColor = null, Color? shadowColor = null, SoundStyle? sound = null, float charTime = 0.05F, float preFadeTime = 2.5f, float fadeTime = 0.5f, bool boxFade = false, Texture2D icon = null, Texture2D bubble = null, DynamicSpriteFont font = null, Vector2 modifier = default, int endID = 0)
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
			if (this.charTime < 0)
				this.charTime = 0.01f;
		}

		public void Update(GameTime gameTime)
		{
			if (Main.gamePaused)
				return;

			// Measure our progress via a modulo between our char time and
			// our timer, allowing us to decide how many chars to display
			if (pauseTime <= 0 && displayingText.Length != text.Length && timer >= charTime)
			{
				SoundEngine.PlaySound((SoundStyle)sound, entity.position);
				displayingText += text[displayingText.Length];
				timer = 0;
			}

			ChatUI.InterpretSymbols(this);

			if (displayingText.Length == text.Length)
				textFinished = true;

			if (textFinished && preFadeTime <= 0 && fadeTime <= 0 || !entity.active)
			{
				if (endID > 0)
				{
					TriggerEnd(endID);
					chain.TriggerEnd(endID);
				}
				if (chain != null)
					chain.Dialogue.Remove(this);
				else
					ChatUI.Dialogue.Remove(this);
			}

			float passedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (Main.FrameSkipMode == FrameSkipMode.Subtle)
				passedTime = 1f / 60f;

			pauseTime -= passedTime;
			if (pauseTime <= 0)
				timer += passedTime;
			if (textFinished)
				preFadeTime -= passedTime;
			if (preFadeTime <= 0)
				fadeTime -= passedTime;
		}
		public Dialogue Get() => this;
		public void TriggerSymbol(string signature) => OnSymbolTrigger?.Invoke(this, signature);
		public void TriggerEnd(int ID) => OnEndTrigger?.Invoke(this, ID);
	}
}
