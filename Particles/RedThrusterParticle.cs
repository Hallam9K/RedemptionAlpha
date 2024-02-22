using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class RedThrusterParticle : Particle
    {
        public override string Texture => "Redemption/Particles/EmberParticle";
        public int timeLeftMax;
        public float size = 0f;
        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = Main.rand.Next(10, 30);
            tileCollide = false;
            oldPos = new Vector2[3];
            SpawnAction = Spawn;
        }

        public override void AI()
        {
            velocity *= 0.9f;
            scale *= 0.9f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Color bright = Color.Multiply(new(235, 255, 255, 0), opacity);
            Color mid = Color.Multiply(new(255, 146, 135, 0), opacity);
            Color dark = Color.Multiply(new(150, 20, 54, 0), opacity);

            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), opacity);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), 1f);

            float pixelRatio = 1f / 64f;
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value, VisualPosition, new Rectangle(0, 0, 64, 64), glowColor, rotation, new Vector2(32f, 32f), 1f * size * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/Circle").Value, VisualPosition - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor, rotation, Vector2.Zero, 1f * pixelRatio * 3f * size * scale, SpriteEffects.None, 0f);
            return false;
        }
        private void Spawn()
        {
            timeLeftMax = timeLeft;
            size = Main.rand.NextFloat(5f, 11f) / 10f;
        }
    }
}
