using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class GlowParticle2 : Particle
    {
        public override string Texture => "Redemption/Particles/EmberParticle";
        public int timer = Main.rand.Next(50, 100);
        public int timeLeftMax;
        public float size = 0f;

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 40;
            tileCollide = false;
            opacity = 0;
            oldPos = new Vector2[3];
            SpawnAction = Spawn;
        }

        public override void AI()
        {
            velocity *= 0.96f;
            // Halfway through, start fading.
            if (timeLeft <= timeLeftMax / 2f)
                opacity = MathHelper.Lerp(1f, 0f, (float)(timeLeftMax / 2f - timeLeft) / (timeLeftMax / 2f));
            else
            {
                opacity += ai[0] + .05f;
                opacity = MathHelper.Clamp(opacity, 0, 1);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D circle = ModContent.Request<Texture2D>("Redemption/Textures/Circle").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value;

            Color bright = Color.Multiply(new(255, 255, 255, 0), opacity);
            Color mid = Color.Multiply(new(color.R, color.G, color.B, 0), opacity);
            Color dark = Color.Multiply(new(color.R - 50, color.G - 50, color.B - 50, 0), opacity);

            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), opacity);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), 1f);

            float pixelRatio = 1f / 64f;
            spriteBatch.Draw(glow, VisualPosition, new Rectangle(0, 0, 64, 64), glowColor * opacity, rotation, new Vector2(32f, 32f), 1f * size * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(circle, VisualPosition - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor * opacity, rotation, Vector2.Zero, 1f * pixelRatio * 3f * size * scale, SpriteEffects.None, 0f);
            return false;
        }
        private void Spawn()
        {
            timeLeft = (int)ai[1];
            timeLeftMax = timeLeft;
            size = Main.rand.NextFloat(5f, 11f) / 10f;
        }
    }
}
