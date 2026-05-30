using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.Effects.Trails;
using Redemption.Effects.Trails.Tips;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;

namespace Redemption.Particles
{
    public class ElectricParticle : Particle
    {
        public float OffsetLimit;

        public float Thickness;

        public float NoiseResolution;

        public int MaxTime;

        private Vector2 oldVelocity;

        private List<Vector2> cache = new();

        private DanTrail trail;

        private float Randnomness;
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public ElectricParticle(int maxTime, float offsetLimit, float thickness = 0.2f)
        {
            MaxTime = maxTime;
            OffsetLimit = offsetLimit;
            Thickness = thickness;
            InitializeTrail();
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
        private readonly int NumPoints = 20;
        public void ManageCache()
        {
            cache = new List<Vector2>();
            for (float i = 0; i <= NumPoints; i++)
            {
                cache.Add(Position + oldVelocity * (i / NumPoints));
            }
        }

        public void InitializeTrail()
        {
            trail = new DanTrail(RedeGraphics.Instance.Primitives, new NoTip(),
            factor =>
            {
                return OffsetLimit;
            },
            factor =>
            {
                return Color.White;
            });
        }

        public void ManageTrail()
        {
            trail.SetPositions(cache.ToArray(), Position);
        }
        public void DrawTrail()
        {
            float progress = TimeLeft / (float)MaxTime;
            float modifiedProgress = EaseFunction.Linear.Ease(progress);
            float modifiedProgress2 = EaseFunction.EaseQuinticIn.Ease(progress);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Effect effect = Request<Effect>("Redemption/Effects/Electric").Value;

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
            effect.Parameters["noiseResolution"].SetValue(new Vector2(NoiseResolution, Thickness * modifiedProgress2));

            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}
