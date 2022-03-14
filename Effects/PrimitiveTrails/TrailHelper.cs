using Redemption.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Redemption.PrimitiveTrails.Trail;
using Redemption.Globals;

namespace Redemption.PrimitiveTrails
{
	// Code by Boffin (From Spirit Mod)
	public class TrailManager : ModSystem
	{
		public Dictionary<ITrailObject, Trail> trails;
		public Effect effect;
		public BasicEffect basicEffect;
		public Matrix worldViewProjection;

		public override void OnModLoad()
		{
			Redemption.Trails = this;
			trails = new Dictionary<ITrailObject, Trail>();
			effect = ModContent.Request<Effect>("Embers/Effects/trailShaders", AssetRequestMode.ImmediateLoad).Value;
			Main.QueueMainThreadAction(() =>
			{
				basicEffect = new BasicEffect(Main.graphics.GraphicsDevice)
				{
					VertexColorEnabled = true
				};
			});
			int width = Main.graphics.GraphicsDevice.Viewport.Width;
			int height = Main.graphics.GraphicsDevice.Viewport.Height;
			Vector2 zoom = Main.GameViewMatrix.Zoom;
			Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
			Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
			worldViewProjection = view * projection;
		}
		public override void Unload()
		{
			trails = null;
			effect = null;
			basicEffect = null;
		}
		public void CreateTrail(ITrailObject trail, ITrailColor trailType, ITrailCap trailCap, ITrailPosition trailPosition, float widthAtFront, float maxLength, ITrailShader shader = null)
		{
			Trail newTrail = new(trail, trailCap, trailType, trailPosition, shader ?? new DefaultShader(), widthAtFront, maxLength);
			newTrail.Update();
			trails.TryAdd(trail, newTrail);
		}
		public void UpdateTrails()
		{
			for (int i = 0; i < trails.Count; i++)
			{
				var trail = trails.ElementAt(i);

				trail.Value.Update();
				if (trail.Value.Dead)
				{
					trails.Remove(trail.Key);
					i--;
				}
			}
		}
		public void DrawTrails(SpriteBatch spriteBatch)
		{
			foreach (var trail in trails)
			{
				trail.Value.Draw(effect, spriteBatch.GraphicsDevice);
			}
		}
		public void ClearAllTrails() => trails.Clear();
		public static void TryTrailKill(ITrailObject trail)
		{
			Redemption.Trails.TryEndTrail(trail);
		}
		public void TryEndTrail(ITrailObject trailObject)
		{
			trails.TryGetValue(trailObject, out Trail trail);
			float dissolveSpeed = Math.Max(15f, trail.velocity.Length() * 3f);
			trail.StartDissolve(dissolveSpeed);
		}
		public override void PreUpdateProjectiles()
		{
			if (Main.netMode != NetmodeID.Server)
				Redemption.Trails.UpdateTrails();
		}
		public static void GetObjectValues(Trail trail, out (bool active, Vector2 position, Vector2 center, Vector2 velocity, float width, float height, float opacity) values)
		{
			if (trail.trailOject is ModNPC npc)
			{
				values.active = npc.NPC.active;
				values.position = npc.NPC.position;
				values.center = npc.NPC.Center;
				values.velocity = npc.NPC.velocity;
				values.width = TextureAssets.Npc[npc.NPC.type].Value.Width;
				values.height = TextureAssets.Npc[npc.NPC.type].Value.Height;
				values.opacity = npc.NPC.Opacity;
			}
			else if (trail.trailOject is ModProjectile projectile)
			{
				values.active = projectile.Projectile.active;
				values.position = projectile.Projectile.position;
				values.center = projectile.Projectile.Center;
				values.velocity = projectile.Projectile.velocity;
				values.width = TextureAssets.Projectile[projectile.Projectile.type].Value.Width;
				values.height = TextureAssets.Projectile[projectile.Projectile.type].Value.Height;
				values.opacity = projectile.Projectile.Opacity;
			}
			else if (trail.trailOject is Particle particle)
			{
				values.active = particle.active;
				values.position = particle.position;
				values.center = particle.Center;
				values.velocity = particle.velocity;
				values.width = particle.width;
				values.height = particle.height;
				values.opacity = particle.opacity;
			}
			else
			{
				values.active = false;
				values.position = Vector2.Zero;
				values.center = Vector2.Zero;
				values.velocity = Vector2.Zero;
				values.width = 0f;
				values.height = 0f;
				values.opacity = 0f;
			}
		}
	}

	public interface ITrailShader
	{
		string ShaderPass { get; }
		void ApplyShader(Effect effect, Trail trail, List<Vector2> positions);
	}
	#region Different Trail Shaders
	public class DefaultShader : ITrailShader
	{
		public string ShaderPass => "DefaultPass";
		public void ApplyShader(Effect effect, Trail trail, List<Vector2> positions)
		{
			effect.CurrentTechnique.Passes[0].Apply();
		}
	}
	public class ImageShader : ITrailShader
	{
		public string ShaderPass => "BasicImagePass";

		protected Vector2 _coordMult;
		protected float _xOffset;
		protected float _yAnimSpeed;
		protected float _strength;
		private readonly Texture2D _texture;

		public ImageShader(Texture2D image, Vector2 coordinateMultiplier, float strength = 1f, float yAnimSpeed = 0f)
		{
			_coordMult = coordinateMultiplier;
			_strength = strength;
			_yAnimSpeed = yAnimSpeed;
			_texture = image;
		}

		public ImageShader(Texture2D image, float xCoordinateMultiplier, float yCoordinateMultiplier, float strength = 1f, float yAnimSpeed = 0f) : this(image, new Vector2(xCoordinateMultiplier, yCoordinateMultiplier), strength, yAnimSpeed)
		{
		}

		public void ApplyShader(Effect effect, Trail trail, List<Vector2> positions)
		{
			_xOffset -= _coordMult.X;
			effect.Parameters["imageTexture"].SetValue(_texture);
			effect.Parameters["coordOffset"].SetValue(new Vector2(_xOffset, Main.GlobalTimeWrappedHourly * _yAnimSpeed));
			effect.Parameters["coordMultiplier"].SetValue(_coordMult);
			effect.Parameters["strength"].SetValue(_strength);
			effect.CurrentTechnique.Passes[ShaderPass].Apply();
		}
	}
	#endregion
	public interface ITrailPosition
	{
		Vector2 GetNextTrailPosition(ITrailObject trail);
	}
	#region Different Trail Positions
	public class DefaultTrailPosition : ITrailPosition
	{
		public Vector2 GetNextTrailPosition(ITrailObject trailObject)
		{
			Redemption.Trails.trails.TryGetValue(trailObject, out Trail trail);
			return trail == null ? Vector2.Zero : trail.center;
		}
	}
	public class OriginTrailPosition : ITrailPosition
	{
		public Vector2 GetNextTrailPosition(ITrailObject trailObject)
		{
			Redemption.Trails.trails.TryGetValue(trailObject, out Trail trail);
			Vector2 drawOrigin = new(trail.width * 0.5f, trail.height * 0.5f);
			return trail.position + drawOrigin + Vector2.UnitY;
		}
	}
	public class ArrowGlowPosition : ITrailPosition
	{
		public Vector2 GetNextTrailPosition(ITrailObject trailObject)
		{
			Redemption.Trails.trails.TryGetValue(trailObject, out Trail trail);
			return trail.center + trail.velocity + Vector2.UnitY;
		}
	}
	public class ZigZagTrailPosition : ITrailPosition
	{
		private int zigType;
		private int zigMove;
		private readonly float strength;

		public ZigZagTrailPosition(float strength)
		{
			this.strength = strength;
			zigType = 0;
			zigMove = 1;
		}

		public Vector2 GetNextTrailPosition(ITrailObject trailObject)
		{
			Redemption.Trails.trails.TryGetValue(trailObject, out Trail trail);
			Vector2 offset = Vector2.Zero;
			if (zigType == -1) offset = trail.velocity.TurnLeft();
			else if (zigType == 1) offset = trail.velocity.TurnRight();
			if (zigType != 0) offset.Normalize();

			zigType += zigMove;
			if (zigType == 2)
			{
				zigType = 0;
				zigMove = -1;
			}
			else if (zigType == -2)
			{
				zigType = 0;
				zigMove = 1;
			}

			return trail.center + offset * strength;
		}
	}
	public class WaveTrailPos : ITrailPosition
	{
		private float counter;
		private readonly float strength;
		public WaveTrailPos(float strength)
		{
			this.strength = strength;
		}

		public Vector2 GetNextTrailPosition(ITrailObject trailObject)
		{
			Redemption.Trails.trails.TryGetValue(trailObject, out Trail trail);
			counter += 0.33f;
			Vector2 offset = Vector2.UnitX.RotatedBy((float)Math.Sin(MathHelper.PiOver4 * counter));
			return trail.center + offset.RotatedBy(trail.velocity.ToRotation()) * strength;
		}
	}
	#endregion
	public interface ITrailColor
	{
		Color GetColourAt(float distanceFromStart, float trailLength, List<Vector2> points);
	}
	#region Different Trail Color Types
	public class GradientTrail : ITrailColor
	{
		private Color _startColour;
		private Color _endColour;

		public GradientTrail(Color start, Color end)
		{
			_startColour = start;
			_endColour = end;
		}

		public Color GetColourAt(float distanceFromStart, float trailLength, List<Vector2> points)
		{
			float progress = distanceFromStart / trailLength;
			return Color.Lerp(_startColour, _endColour, progress) * (1f - progress);
		}
	}

	public class RainbowTrail : ITrailColor
	{
		private readonly float _saturation;
		private readonly float _lightness;
		private readonly float _speed;
		private readonly float _distanceMultiplier;

		public RainbowTrail(float animationSpeed = 5f, float distanceMultiplier = 0.01f, float saturation = 1f, float lightness = 0.5f)
		{
			_saturation = saturation;
			_lightness = lightness;
			_distanceMultiplier = distanceMultiplier;
			_speed = animationSpeed;
		}

		public Color GetColourAt(float distanceFromStart, float trailLength, List<Vector2> points)
		{
			float progress = distanceFromStart / trailLength;
			float hue = (Main.GlobalTimeWrappedHourly * _speed + distanceFromStart * _distanceMultiplier) % MathHelper.TwoPi;
			return ColorFromHSL(hue, _saturation, _lightness) * (1f - progress);
		}

		//Borrowed methods for converting HSL to RGB
		private Color ColorFromHSL(float h, float s, float l)
		{
			h /= MathHelper.TwoPi;

			float r = 0, g = 0, b = 0;
			if (l != 0)
			{
				if (s == 0)
					r = g = b = l;
				else
				{
					float temp2;
					if (l < 0.5f)
						temp2 = l * (1f + s);
					else
						temp2 = l + s - l * s;

					float temp1 = 2f * l - temp2;

					r = GetColorComponent(temp1, temp2, h + 0.33333333f);
					g = GetColorComponent(temp1, temp2, h);
					b = GetColorComponent(temp1, temp2, h - 0.33333333f);
				}
			}
			return new Color(r, g, b);
		}
		private float GetColorComponent(float temp1, float temp2, float temp3)
		{
			if (temp3 < 0f)
				temp3 += 1f;
			else if (temp3 > 1f)
				temp3 -= 1f;

			if (temp3 < 0.166666667f)
				return temp1 + (temp2 - temp1) * 6f * temp3;
			else if (temp3 < 0.5f)
				return temp2;
			else if (temp3 < 0.66666666f)
				return temp1 + (temp2 - temp1) * (0.66666666f - temp3) * 6f;
			else
				return temp1;
		}
	}

	public class StandardColorTrail : ITrailColor
	{
		private Color _colour;

		public StandardColorTrail(Color colour)
		{
			_colour = colour;
		}

		public Color GetColourAt(float distanceFromStart, float trailLength, List<Vector2> points)
		{
			float progress = distanceFromStart / trailLength;
			return _colour * (1f - progress);
		}
	}

	public class OpacityUpdatingTrail : ITrailColor
	{
		private Color startColor;
		private Color endColor;
		private Trail trail;
		private float opacity = 1f;

		public OpacityUpdatingTrail(ITrailObject trailObject, Color color)
		{
			startColor = color;
			endColor = color;
			Redemption.Trails.trails.TryGetValue(trailObject, out Trail trail);
			this.trail = trail;
		}

		public OpacityUpdatingTrail(ITrailObject trailObject, Color startColor, Color endColor)
		{
			this.startColor = startColor;
			this.endColor = endColor;
			Redemption.Trails.trails.TryGetValue(trailObject, out Trail trail);
			this.trail = trail;
		}

		public Color GetColourAt(float distanceFromStart, float trailLength, List<Vector2> points)
		{
			float progress = distanceFromStart / trailLength;
			if (trail != null && trail.active)
				opacity = trail.opacity;

			return Color.Lerp(startColor, endColor, progress) * (1f - progress) * opacity;
		}
	}
	#endregion
	public interface ITrailCap
	{
		int ExtraTris { get; }
		void AddCap(VertexPositionColorTexture[] array, ref int currentIndex, Color colour, Vector2 position, Vector2 startNormal, float width);
	}
	#region Different Trail Caps
	public class RoundCap : ITrailCap
	{
		public int ExtraTris => 20;

		public void AddCap(VertexPositionColorTexture[] array, ref int currentIndex, Color colour, Vector2 position, Vector2 startNormal, float width)
		{
			//initial info
			float halfWidth = width * 0.5f;
			float arcStart = startNormal.ToRotation();
			float arcAmount = MathHelper.Pi;
			//int segments = (int)Math.Ceiling(6 * Math.Sqrt(halfWidth) * (arcAmount / MathHelper.TwoPi));
			int segments = ExtraTris;
			float theta = arcAmount / segments;
			float cos = (float)Math.Cos(theta);
			float sin = (float)Math.Sin(theta);
			float t;
			float x = (float)Math.Cos(arcStart) * halfWidth;
			float y = (float)Math.Sin(arcStart) * halfWidth;

			position -= Main.screenPosition;

			//create initial vertices
			VertexPositionColorTexture center = new VertexPositionColorTexture(new Vector3(position.X, position.Y, 0f), colour, Vector2.One * 0.5f);
			VertexPositionColorTexture prev = new VertexPositionColorTexture(new Vector3(position.X + x, position.Y + y, 0f), colour, Vector2.One);

			for (int i = 0; i < segments; i++)
			{
				//apply matrix transformation
				t = x;
				x = cos * x - sin * y;
				y = sin * t + cos * y;

				VertexPositionColorTexture next = new VertexPositionColorTexture(new Vector3(position.X + x, position.Y + y, 0f), colour, Vector2.One);

				//Add triangle vertices
				array[currentIndex++] = center;
				array[currentIndex++] = prev;
				array[currentIndex++] = next;

				prev = next;
			}
		}
	}
	public class TriangleCap : ITrailCap
	{
		public int ExtraTris => 1;

		private readonly float _widthmod;
		private readonly float _length;
		public TriangleCap(float width = 1f, float length = 1f)
		{
			_widthmod = width;
			_length = length;
		}

		public void AddCap(VertexPositionColorTexture[] array, ref int currentIndex, Color colour, Vector2 position, Vector2 startNormal, float width)
		{
			width *= _widthmod;
			float rotation = startNormal.ToRotation();
			float halfwidth = width / 2;
			Vector2 TipPos = position + Vector2.UnitY.RotatedBy(rotation) * width * _length - Main.screenPosition;
			Vector2 LeftBasePos = position + Vector2.UnitY.RotatedBy(rotation + MathHelper.PiOver2) * halfwidth - Main.screenPosition;
			Vector2 RightBasePos = position + Vector2.UnitY.RotatedBy(rotation - MathHelper.PiOver2) * halfwidth - Main.screenPosition;

			array[currentIndex++] = new VertexPositionColorTexture(new Vector3(LeftBasePos, 0), colour, Vector2.Zero);
			array[currentIndex++] = new VertexPositionColorTexture(new Vector3(RightBasePos, 0), colour, Vector2.One);
			array[currentIndex++] = new VertexPositionColorTexture(new Vector3(TipPos, 0), colour, new Vector2(0.5f, 1f));
		}
	}
	public class NoCap : ITrailCap
	{
		public int ExtraTris => 0;

		public void AddCap(VertexPositionColorTexture[] array, ref int currentIndex, Color colour, Vector2 position, Vector2 startNormal, float width)
		{

		}
	}
	#endregion
}