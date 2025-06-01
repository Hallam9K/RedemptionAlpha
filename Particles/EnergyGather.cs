using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.Effects;
using Redemption.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Redemption.Particles
{
    public class AnglonPortal_EnergyGather : Particle
    {
        public AnglonPortal_EnergyGather() : this(Vector2.Zero) { }
        public AnglonPortal_EnergyGather(Vector2 seekEntityPos)
        {
            SeekEntityPos = seekEntityPos;
        }
        public override string Texture => Redemption.EMPTY_TEXTURE;
        private readonly int NUMPOINTS = 30;

        public Color baseColor = new(1, 1, 1);
        public Color endColor = Color.DarkOliveGreen;
        public Color edgeColor = new(14, 25, 13);

        public Entity seekEntity = null;
        public Vector2 seekEntityOffset;
        public Vector2 SeekEntityPos;

        public float killRange = 32;


        public List<Vector2> cache;
        public List<Vector2> cache2;
        public DanTrail trail;
        public DanTrail trail2;

        private readonly float thickness = 1f;

        private float thicknessModifier = 1f;
        public float velRotation;
        public float aiTimer;
        public override void Spawn()
        {
            TimeLeft = 999999;
            TileCollide = false;
            if (velRotation == -1)
            {
                velRotation = Velocity.ToRotation();
                Velocity = Vector2.Zero;
            }
        }

        public override void Update()
        {
            Vector2 powerCenter = SeekEntityPos;

            if (seekEntity != null && seekEntity is NPC or Player or Projectile)
                powerCenter = seekEntity.Center;

            if (aiTimer == 0)
            {
                const float DESIRED_FLY_SPEED_IN_PIXELS_PER_FRAME = 40;
                float AMOUNT_OF_FRAMES_TO_LERP_BY = MathHelper.Lerp(15f, 1f, velRotation / 120f);

                Vector2 desiredVelocity = Vector2.Normalize(powerCenter - Position) * DESIRED_FLY_SPEED_IN_PIXELS_PER_FRAME;
                Velocity = Vector2.Lerp(Velocity, desiredVelocity, 1f / AMOUNT_OF_FRAMES_TO_LERP_BY);
                Velocity.Normalize();
                Velocity *= 15f;

                float dist = Vector2.Distance(Position, powerCenter);

                if (dist < killRange)
                    aiTimer = 1;
                if (velRotation < 120f)
                    velRotation++;
            }
            if (aiTimer >= 1 && aiTimer <= 61)
            {
                Velocity = Vector2.Zero;

                float timer = aiTimer - 1;

                thicknessModifier = MathHelper.Lerp(1f, 0f, timer / 60f);

                aiTimer++;
            }
            if (aiTimer >= 61)
                Kill();


            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < NUMPOINTS; i++)
                {
                    cache.Add(Position);
                }
            }

            cache.Add(Position);

            while (cache.Count > NUMPOINTS)
            {
                cache.RemoveAt(0);
            }

            cache2 = new List<Vector2>();
            for (int i = 0; i < cache.Count; i++)
            {
                Vector2 point = cache[i];
                Vector2 nextPoint = i == cache.Count - 1 ? Position : cache[i + 1];
                Vector2 dir = Vector2.Normalize(nextPoint - point);
                if (i > cache.Count - 3 || dir == Vector2.Zero)
                    cache2.Add(point);
                else
                    cache2.Add(point + (dir * Main.rand.NextFloat(5)));
            }
        }

        public void ManageTrail()
        {
            trail ??= new DanTrail(Main.instance.GraphicsDevice, NUMPOINTS, new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                float toSin = MathHelper.Lerp(0f, 180f, 1f - mult);
                float sinMod = (float)Math.Sin(MathHelper.ToRadians(toSin));

                return thickness * 6 * sinMod * thicknessModifier;
            },
            factor =>
            {
                if (factor.X > 0.99f)
                    return Color.Transparent;

                return edgeColor * 0.1f * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Position;
            trail2 ??= new DanTrail(Main.instance.GraphicsDevice, NUMPOINTS, new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                float toSin = MathHelper.Lerp(0f, 180f, 1f - mult);
                float sinMod = (float)Math.Sin(MathHelper.ToRadians(toSin));

                return thickness * 3 * sinMod * thicknessModifier;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress);
            });

            trail2.Positions = cache2.ToArray();
            trail2.NextPosition = Position;
        }
    }
    public class GathuramPortal_EnergyGather : AnglonPortal_EnergyGather
    {
        public GathuramPortal_EnergyGather() : this(Vector2.Zero) { }
        public GathuramPortal_EnergyGather(Vector2 seekEntityPos)
        {
            SeekEntityPos = seekEntityPos;
        }
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void Spawn()
        {
            base.Spawn();
            endColor = Color.CornflowerBlue;
            edgeColor = new(14, 13, 25);
        }
    }
    public class Blackhole_EnergyGather : AnglonPortal_EnergyGather
    {
        public Blackhole_EnergyGather() : this(Vector2.Zero) { }
        public Blackhole_EnergyGather(Vector2 seekEntityPos)
        {
            SeekEntityPos = seekEntityPos;
        }
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void Spawn()
        {
            base.Spawn();
            endColor = RedeColor.NebColour;
            edgeColor = new(14, 13, 25);
        }
    }
    public class Fool_EnergyGather : AnglonPortal_EnergyGather
    {
        public Fool_EnergyGather() : this(Vector2.Zero) { }
        public Fool_EnergyGather(Vector2 seekEntityPos)
        {
            SeekEntityPos = seekEntityPos;
        }
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void Spawn()
        {
            base.Spawn();
            endColor = Color.White with { A = 0 };
            edgeColor = Color.LightGreen with { A = 0 };
        }
    }
}