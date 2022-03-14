using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
	public class ParticleManager : ModSystem
	{
		public static List<Particle> particles;
		private static Vector2 screenSize;
		public override void OnModLoad()
		{
			particles = new List<Particle>(6000);
			screenSize = Main.ScreenSize.ToVector2();
			On.Terraria.Main.DrawDust += DrawParticles;
			Main.OnResolutionChanged += UpdateSceenSize;
		}
		public override void Unload()
		{
			particles.Clear();
			particles = null;
		}
		public static void Dispose()
		{
			particles.Clear();
		}
		private void UpdateSceenSize(Vector2 obj)
		{
			screenSize = Main.ScreenSize.ToVector2();
		}
		private void DrawParticles(On.Terraria.Main.orig_DrawDust orig, Main self)
		{
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix); 
			PreUpdate(Main.spriteBatch);
			Update(Main.spriteBatch);
			PostUpdate(Main.spriteBatch);
			Main.spriteBatch.End();
			orig(self);
		}
		public static void PreUpdate(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < particles?.Count; i++)
				particles[i].PreAI();
		}
		public static void Update(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < particles?.Count; i++)
			{
				Particle particle = particles[i];

				particle.oldDirection = particle.direction;
				if ((particle.tileCollide && !Collision.SolidCollision(particles[i].position + (new Vector2(particles[i].width / 2f, particles[i].height / 2f) * particles[i].scale), 1, 1)) || !particle.tileCollide)
				{
					particle.velocity.Y += particles[i].gravity;
					particle.position += particles[i].velocity;
					UpdateOldPos(particle);
				}

				particle.direction = particle.velocity.X >= 0f ? 1 : -1;
				particle.lavaWet = Collision.LavaCollision(particle.position, particle.width, particle.height);
				particle.wet = Collision.WetCollision(particle.position, particle.width, particle.height);

				particle.AI();
				if (ShouldDraw(particle))
				{
					bool draw = particle.PreDraw(spriteBatch, particle.VisualPosition, Lighting.GetColor((int)(particles[i].position.X / 16), (int)(particles[i].position.Y / 16)));
					if (draw)
						particle.Draw(spriteBatch, particle.VisualPosition, Lighting.GetColor((int)(particles[i].position.X / 16), (int)(particles[i].position.Y / 16)));
				}
				if (particle.timeLeft-- == 0 || !particles[i].active)
				{
					particle.DeathAction?.Invoke();
					particles.RemoveAt(i);
				}
			}
		}
		public static void PostUpdate(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < particles?.Count; i++)
			{
				particles[i].PostAI();
				if (ShouldDraw(particles[i]))
					particles[i].PostDraw(spriteBatch, particles[i].VisualPosition, Lighting.GetColor((int)(particles[i].position.X / 16), (int)(particles[i].position.Y / 16)));
			}
		}
		public static void NewParticle(Vector2 Position, Vector2 Velocity, Particle Type, Color Color, float Scale, float AI0 = 0, float AI1 = 0, float AI2 = 0, float AI3 = 0, float AI4 = 0, float AI5 = 0, float AI6 = 0, float AI7 = 0)
		{
			if (Type.texture == null)
				throw new NullReferenceException($"Texture was null for {Type.GetType().Name}.");
			Type.position = Position;
			Type.velocity = Velocity;
			Type.color = Color;
			Type.scale = Scale;
			Type.active = true;
			Type.ai = new float[] { AI0, AI1, AI2, AI3, AI4, AI5, AI6, AI7 };
			if (particles?.Count > 6000)
				particles.TrimExcess();
			if (particles?.Count < 6000)
			{
				Type.SpawnAction?.Invoke();
				particles?.Add(Type);
			}
			RedeDetours.NewParticle(Position, Velocity, Type, Color, Scale, AI0, AI1, AI2, AI3, AI4, AI5, AI6, AI7);
		}
		public static void UpdateOldPos(Particle particle)
		{
			if (particle.oldPos != null)
			{
				for (int i = particle.oldPos.Length - 1; i >= 0; i--)
				{
					particle.oldPos[i] = i == 0 ? particle.position : particle.oldPos[i - 1];
					if (i == 0)
						break;
				}
			}
			if (particle.oldCen != null)
			{
				for (int i = particle.oldCen.Length - 1; i >= 0; i--)
				{
					particle.oldCen[i] = i == 0 ? particle.Center : particle.oldCen[i - 1];
					if (i == 0)
						break;
				}
			}
			if (particle.oldRot != null)
			{
				for (int i = particle.oldRot.Length - 1; i >= 0; i--)
				{
					particle.oldRot[i] = i == 0 ? particle.rotation : particle.oldRot[i - 1];
					if (i == 0)
						break;
				}
			}
			if (particle.oldVel != null)
			{
				for (int i = particle.oldVel.Length - 1; i >= 0; i--)
				{
					particle.oldVel[i] = i == 0 ? particle.velocity : particle.oldVel[i - 1];
					if (i == 0)
						break;
				}
			}
		}
		public static bool ShouldDraw(Particle particle)
		{
			if (particle.position.X < Main.screenPosition.X - 100f || particle.position.Y < Main.screenPosition.Y - 100f || particle.position.X > Main.screenPosition.X + screenSize.X + 100f || particle.position.Y > Main.screenPosition.Y + screenSize.Y + 100f)
				return false;
			return true;
		}
	}
}
