using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Exceptions;

namespace Redemption.Particles
{
	public class Particle
	{
		public Particle particle;
		/// <summary>
		/// Initializes a new particle instance.
		/// </summary>
		public Particle()
		{
			particle = this;
			SetDefaults();
			SetPrivateDefaults();
		}
		/// <summary>
		/// The position of the particle.
		/// </summary>
		public Vector2 position = new(0f, 0f);
		/// <summary>
		/// The velocity of the particle.
		/// </summary>
		public Vector2 velocity = new(0f, 0f);
		/// <summary>
		/// The old positions of the particle.
		/// </summary>
		public Vector2[] oldPos;
		/// <summary>
		/// The old centers of the particle.
		/// </summary>
		public Vector2[] oldCen;
		/// <summary>
		/// The old rotations of the particle.
		/// </summary>
		public float[] oldRot;
		/// <summary>
		/// The old velocities of the particle.
		/// </summary>
		public Vector2[] oldVel;
		/// <summary>
		/// Method to run on the spawning of the particle.
		/// </summary>
		public Action SpawnAction;
		/// <summary>
		/// Method to run on the death of the particle.
		/// </summary>
		public Action DeathAction;
		/// <summary>
		/// Color to apply or colors to shift between.
		/// </summary>
		public Color color;
		/// <summary>
		/// The hitbox for this particle.
		/// </summary>
		public Rectangle frame;
		/// <summary>
		/// The texture of this particle.
		/// </summary>
		public Texture2D texture;
		/// <summary>
		/// The ID of this particle instance.
		/// </summary>
		public int whoAmI;
		/// <summary>
		/// Whether this particle is active or not.
		/// </summary>
		public bool active;
		/// <summary>
		/// The direction of this particle on the previous tick.
		/// </summary>
		public int oldDirection;
		/// <summary>
		/// The current direction of this particle.
		/// </summary>
		public int direction;
		/// <summary>
		/// The width of this particle.
		/// </summary>
		public int width;
		/// <summary>
		/// The height of this particle.
		/// </summary>
		public int height;
		/// <summary>
		/// The size of this particle.
		/// </summary>
		public float scale;
		/// <summary>
		/// The rotation of this particle.
		/// </summary>
		public float rotation;
		/// <summary>
		/// The type of this particle.
		/// </summary>
		public int type;
		/// <summary>
		/// The alpha of this particle.
		/// </summary>
		public float alpha = 1f;
		/// <summary>
		/// The strength of the gravity applied to this projectile.
		/// </summary>
		public float gravity;
		/// <summary>
		/// The duration of the lifetime of this particle, in frames.
		/// </summary>
		public int timeLeft;
		/// <summary>
		/// Length of the old position cache;
		/// </summary>
		public int oldPosLength;
		/// <summary>
		/// Length of the old rotation cache;
		/// </summary>
		public int oldRotLength;
		/// <summary>
		/// Length of the old center cache;
		/// </summary>
		public int oldCenLength;
		/// <summary>
		/// Length of the old velocity cache;
		/// </summary>
		public int oldVelLength;
		/// <summary>
		/// AI array, useful for passing data into the particle on spawn.
		/// </summary>
		public float[] ai;
		/// <summary>
		/// Whether or not this particle should interact with tiles.
		/// </summary>
		public bool tileCollide;

		public virtual void SetDefaults()
		{
		}
		private void SetPrivateDefaults()
		{
			if (oldPosLength > 0)
				oldPos = new Vector2[oldPosLength];
			if (oldRotLength > 0)
				oldRot = new float[oldRotLength];
			if (oldCenLength > 0)
				oldCen = new Vector2[oldCenLength];
			if (oldVelLength > 0)
				oldVel = new Vector2[oldVelLength];
			if (texture == null)
			{
				string filePath = GetType().Namespace.Replace(".", "/") + "/" + GetType().Name;
				texture = ModContent.Request<Texture2D>(filePath).Value;
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
		/// Drawing code to run before the particle manager draw call. Return false to keep the particle manager from drawing your particle.
		/// </summary>
		/// <returns>bool</returns>
		public virtual bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			return true;
		}
		/// <summary>
		/// This drawing method runs if PreDraw returns true.
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			spriteBatch.Draw(texture, position - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), color != default ? color : lightColor, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
		}
		/// <summary>
		/// Drawing code to run after the particle manager draw call.
		/// </summary>
		public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
		}
		/// <summary>
		/// Kills this particle.
		/// </summary>
		//public void Kill(this Particle particle) => particle.Active = false;
	}
}
