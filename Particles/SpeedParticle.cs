using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class SpeedParticle : Particle
    {
        public override string Texture => "Redemption/Particles/EmberParticle";
        public float timer;
        public float extension;
        public float ScaleX;
        public float ScaleY;
        public float DeAcc;
        public bool IsAdditive;
        public int TimerMax;
        public SpeedParticle(float deAcc = 0.91f, bool isAdditive = true, int timerMax = 21)
        {
            DeAcc = deAcc;
            IsAdditive = isAdditive;
            TimerMax = timerMax;
        }
        public override void Spawn()
        {
            Velocity *= 0.5f;
            TimeLeft = TimerMax;
            timer = TimerMax - 1;

            TileCollide = false;
            extension = Main.rand.NextFloat(28, 32);
            Rotation = Velocity.ToRotation();
        }
        public override void Update()
        {
            timer--;
            float progress = timer / ((float)TimerMax - 1);
            ScaleX = Scale + extension * progress;
            ScaleY = Scale - Scale * progress;
            Velocity *= DeAcc;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Color a = IsAdditive ? Color with { A = 0 } : Color;
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value, location, new Rectangle(0, 0, 192, 192), a, Rotation, new Vector2(96, 96), new Vector2(ScaleX, Scale) * 0.02f, 0, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value, location, new Rectangle(0, 0, 192, 192), a, Rotation, new Vector2(96, 96), new Vector2(ScaleX, Scale) * 0.025f, 0, 0f);
        }
    }
}
