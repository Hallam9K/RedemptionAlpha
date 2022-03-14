using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
	public class Particle : Entity
	{
		public Particle()
		{
			SetDefaults();
			SetPrivateDefaults();
		}
		public virtual string Texture { get; set; }
		public override Vector2 VisualPosition => position - Main.screenPosition;

		public Color color;
		public Rectangle frame;
		public Texture2D texture;
		public Action SpawnAction;
		public Action DeathAction;

		public bool tileCollide = false;
		public float scale = 1f;
		public float rotation = 0f;
		public float opacity = 1f;
		public float gravity = 0f;
		public int timeLeft = 0;

		public float[] ai;
		public Vector2[] oldPos;
		public float[] oldRot;
		public Vector2[] oldCen;
		public Vector2[] oldVel;

		public virtual void SetDefaults()
		{
		}
		private void SetPrivateDefaults()
		{
			if (texture == null)
			{
				string filePath = Texture == string.Empty || Texture == null ? GetType().Namespace.Replace(".", "/") + "/" + GetType().Name : Texture;
				texture = ModContent.Request<Texture2D>(filePath).Value;
				if (texture == null)
					throw new NullReferenceException($"Texture was null for {GetType().Name}.");
			}
		}
		/// <summary>
		/// Code to run pre-update.
		/// </summary>
		public virtual void PreAI()
		{
		}
		/// <summary>
		/// Code to run mid-update.
		/// </summary>
		public virtual void AI()
		{
		}
		/// <summary>
		/// Code to run post-update.
		/// </summary>
		public virtual void PostAI()
		{
		}
		/// <summary>
		/// This method runs before Draw. Return false to keep the Particle Manager from drawing your particle.
		/// </summary>
		/// <returns>bool</returns>
		public virtual bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
		{
			return true;
		}
		/// <summary>
		/// This method runs if PreDraw returns true.
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
		{
			spriteBatch.Draw(texture, position - drawPos, new Rectangle(0, 0, width, height), color != default ? color : lightColor, rotation, new Vector2(width, height) * 0.5f, scale, SpriteEffects.None, 0f);
		}
		/// <summary>
		/// This method runs after Draw is called.
		/// </summary>
		public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
		{
		}
		/// <summary>
		/// Kills a particle.
		/// </summary>
		public void Kill() => ParticleManager.particles.Remove(this);
	}
}
