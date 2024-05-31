using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class DevilsPactParticle : Particle
    {
        public override string Texture => "Redemption/Particles/RainbowParticle1";
        public int DustType;
        public DevilsPactParticle() : this(0) { }
        public DevilsPactParticle(int dustType)
        {
            DustType = dustType;
        }
        public override void Spawn()
        {
            TimeLeft = 10;
            TileCollide = false;
        }
        public void MakeDust()
        {
            for (float num4 = 0f; num4 < 3f; num4++)
            {
                Vector2 vector2 = (MathHelper.PiOver4 + MathHelper.PiOver4 * num4).ToRotationVector2() * 4f;
                Dust dust = Dust.NewDustPerfect(Position, DustType, vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat() * Scale, newColor: Color.White);
                dust.noGravity = true;
                dust = Dust.NewDustPerfect(Position, DustType, -vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat() * Scale, newColor: Color.White);
                dust.noGravity = true;
            }
        }
        public override void Update()
        {
            MakeDust();
            Opacity = 1f - 1f / 10f * (10 - TimeLeft);
            if (Opacity <= 0)
                Kill();
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Color color = Color.Multiply(Color, Opacity);
            for (int i = -1; i < 2; i++)
            {
                float ScaleX = (2f - i * i * 0.5f) * Scale;
                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Particles/RainbowParticle2").Value, location,
                    new Rectangle(0, 0, 142, 42), color * Opacity * 2, Rotation + MathHelper.PiOver2 + MathHelper.Pi / 4 * i, new Vector2(71f, 21f), new Vector2(1, ScaleX) * 0.3f, 0, 0f);
            }
        }
    }
}
