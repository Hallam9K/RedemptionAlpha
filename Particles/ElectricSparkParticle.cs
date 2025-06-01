using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.Effects;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace Redemption.Particles
{
    public class ElectricSparkParticle : Particle
    {
        public float OffsetLimit;

        public int NumPoints;

        public float Thickness;

        public float NoiseResolution;

        public int MaxTime;

        private Vector2 oldVelocity;

        private List<Vector2> cache = new();

        private DanTrail trail;

        private float Randnomness;
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public ElectricSparkParticle(int maxTime, float offsetLimit, float thickness = 0.2f, int numPoints = 6)
        {
            MaxTime = maxTime;
            OffsetLimit = offsetLimit;
            NumPoints = numPoints;
            Thickness = thickness;
        }
        public override void Spawn()
        {
            TimeLeft = MaxTime;
            Randnomness = Main.rand.NextFloat();
            NoiseResolution = Velocity.Length() * 0.001f;
            oldVelocity = Velocity;
            Velocity *= 0;
        }
        public override void Update()
        {
            if (!Main.dedServ)
            {
                ManageCache();

                if (cache.Count > 3)
                    ManageTrail();
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            DrawTrail();
        }
        public void ManageCache()
        {
            cache = new List<Vector2>();
            for(float i  = 0; i <= NumPoints; i++)
            {
                cache.Add(Position + oldVelocity * (i / NumPoints));
            }
        }
        public void ManageTrail()
        {
            trail = new DanTrail(Main.instance.GraphicsDevice, cache.Count, new NoTip(),
            factor =>
            {
                float sine =  0.5f * MathF.Sin(factor * MathHelper.Pi);
                float width = 10f + OffsetLimit * sine;
                return width;
            },
            factor =>
            {
                float progress = TimeLeft / (float)MaxTime;
                float opacity = progress > 0.9f ? 2 : BaseUtility.MultiLerp(factor.X - 1 + progress * 2, 0, 1);

                return Color.White * opacity;
            });
            trail.Positions = cache.ToArray();
            trail.NextPosition = Position;
        }
        public void DrawTrail()
        {
            float progress = TimeLeft / (float)MaxTime;
            float modifiedProgress = MathF.Pow(progress, 0.75f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Effect effect = Request<Effect>("Redemption/Effects/Electric", AssetRequestMode.ImmediateLoad).Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0);
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D texture = Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value;
            Texture2D noise0 = Request<Texture2D>("Redemption/Textures/Noise/noise").Value;
            Texture2D noise1 = Request<Texture2D>("Redemption/Textures/Noise/electricnoise").Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["tex0"].SetValue(texture);
            effect.Parameters["noiseTex0"].SetValue(noise0);
            effect.Parameters["noiseTex1"].SetValue(noise1);
            effect.Parameters["randomY"].SetValue(Randnomness);
            effect.Parameters["progress"].SetValue(modifiedProgress);
            effect.Parameters["uColor"].SetValue(Color.ToVector4());
            effect.Parameters["noiseResolution"].SetValue(new Vector2(NoiseResolution, Thickness));

            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}