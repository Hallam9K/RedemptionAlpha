using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Effects;
using Redemption.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class AnglonPortal_EnergyGather : Particle
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        private readonly int NUMPOINTS = 30;

        public Color baseColor = new(1, 1, 1);
        public Color endColor = Color.DarkOliveGreen;
        public Color edgeColor = new(14, 25, 13);

        public Entity seekEntity = null;
        public Vector2 seekEntityOffset = Vector2.Zero;

        public float killRange = 32;


        public List<Vector2> cache;
        public List<Vector2> cache2;
        public DanTrail trail;
        public DanTrail trail2;

        private readonly float thickness = 1f;

        private float thicknessModifier = 1f;

        public bool Active { get => active; set => active = value; }

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 999999;
            tileCollide = false;
            oldPos = new Vector2[3];
            SpawnAction = Spawn;
        }

        public override void AI()
        {
            Vector2 powerCenter = new(ai[0], ai[1]);

            if (seekEntity != null && seekEntity is NPC or Player or Projectile)
                powerCenter = seekEntity.Center;

            if (ai[2] == 0)
            {
                const float DESIRED_FLY_SPEED_IN_PIXELS_PER_FRAME = 40;
                float AMOUNT_OF_FRAMES_TO_LERP_BY = MathHelper.Lerp(15f, 1f, ai[3] / 120f);

                Vector2 desiredVelocity = Vector2.Normalize(powerCenter - Center) * DESIRED_FLY_SPEED_IN_PIXELS_PER_FRAME;
                velocity = Vector2.Lerp(velocity, desiredVelocity, 1f / AMOUNT_OF_FRAMES_TO_LERP_BY);
                velocity.Normalize();
                velocity *= 15f;

                float dist = Vector2.Distance(Center, powerCenter);

                if (dist < killRange)
                    ai[2] = 1;
                if (ai[3] < 120f)
                    ai[3]++;
            }
            if (ai[2] >= 1 && ai[2] <= 61)
            {
                velocity = Vector2.Zero;

                float timer = ai[2] - 1;

                thicknessModifier = MathHelper.Lerp(1f, 0f, timer / 60f);

                ai[2]++;
            }
            if (ai[2] >= 61)
                active = false;


            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

        private void Spawn()
        {
            if (ai[3] == -1)
            {
                ai[3] = velocity.ToRotation();
                velocity = Vector2.Zero;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < NUMPOINTS; i++)
                {
                    cache.Add(Center);
                }
            }

            cache.Add(Center);

            while (cache.Count > NUMPOINTS)
            {
                cache.RemoveAt(0);
            }

            cache2 = new List<Vector2>();
            for (int i = 0; i < cache.Count; i++)
            {
                Vector2 point = cache[i];
                Vector2 nextPoint = i == cache.Count - 1 ? Center : cache[i + 1];
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
            trail.NextPosition = Center;
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
            trail2.NextPosition = Center;
        }
    }
    public class GathuramPortal_EnergyGather : AnglonPortal_EnergyGather
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetDefaults()
        {
            base.SetDefaults();
            endColor = Color.CornflowerBlue;
            edgeColor = new(14, 13, 25);
        }
    }
}