using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
	public class EmberParticle : Particle
	{
		public int timer = Main.rand.Next(50, 100);
		public float speedX = Main.rand.NextFloat(4f, 9f);
		public float mult = Main.rand.NextFloat(10f, 31f) / 200f;
		public int timeLeftMax;
		public float size = 0f;

		public override void SetDefaults()
		{
			width = 1;
			height = 1;
			timeLeft = Main.rand.Next(90, 121);
			tileCollide = false;
			oldPos = new Vector2[3];
			SpawnAction = Spawn;
		}

		public override void AI()
		{
			if (ai[0] <= 0)
			{
				float sineX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX);

				// Makes the particle change directions or speeds.
				// Timer is used for keeping track of the current cycle
				if (timer == 0)
					NewMovementCycle();

				// Adds the wind velocity to the particle.
				// It adds less the faster it is already going.
				velocity += new Vector2(Main.windSpeedCurrent * (Main.windPhysicsStrength * 3f) * MathHelper.Lerp(1f, 0.1f, Math.Abs(velocity.X) / 6f), 0f);
				// Add the sine component to the velocity.
				// This is scaled by the mult, which changes every cycle.
				velocity += new Vector2(sineX * mult, -Main.rand.NextFloat(1f, 2f) / 100f);

				// Clamp the velocity so the particle doesnt go too fast.
				Utils.Clamp(velocity.X, -6f, 6f);
				Utils.Clamp(velocity.Y, -6f, 6f);

				// Decrement the timer
				timer--;

				// Halfway through, start fading.
				if (timeLeft <= timeLeftMax / 2f)
					opacity = MathHelper.Lerp(1f, 0f, (float)(timeLeftMax / 2f - timeLeft) / (timeLeftMax / 2f));
			}
			ai[0]--;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			Texture2D circle = ModContent.Request<Texture2D>("Redemption/Textures/Circle").Value;
			Texture2D ember = ModContent.Request<Texture2D>("Redemption/Particles/EmberParticle").Value;
			Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value;

			Color bright = Color.Multiply(new(240, 149, 46, 0), opacity);
			Color mid = Color.Multiply(new(187, 63, 25, 0), opacity);
			Color dark = Color.Multiply(new(131, 23, 37, 0), opacity);

			Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), opacity);
			Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), 1f);

			float pixelRatio = 1f / 64f;
			spriteBatch.Draw(glow, VisualPosition, new Rectangle(0, 0, 64, 64), glowColor, rotation, new Vector2(32f, 32f), 1f * size * scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(circle, VisualPosition - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor, rotation, Vector2.Zero, 1f * pixelRatio * 3f * size * scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(ember, VisualPosition, new Rectangle(0, 0, 3, 3), color, rotation, new Vector2(1.5f, 1.5f), 1f * scale, SpriteEffects.None, 0f);
			return false;
		}
		private void Spawn()
		{
			timeLeftMax = timeLeft;
			size = Main.rand.NextFloat(5f, 11f) / 10f;
		}
		private void NewMovementCycle()
		{
			timer = Main.rand.Next(50, 100);
			speedX = Main.rand.NextFloat(4f, 9f);
			mult = Main.rand.NextFloat(10f, 31f) / 200f;
		}
	}
}
