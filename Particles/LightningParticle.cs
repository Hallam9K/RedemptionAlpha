using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class LightningParticle : Particle
    {
        public override string Texture => "Redemption/Particles/EmberParticle";
        public int timer = Main.rand.Next(6, 7);
        public int timeLeftMax;
        public float size = 0f;

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = Main.rand.Next(5, 7);
            tileCollide = false;
            oldPos = new Vector2[3];
            SpawnAction = Spawn;
        }

        public override void AI()
        {
            // Decrement the timer
            timer--;

            // Halfway through, start fading.
            if (timeLeft <= timeLeftMax / 2f)
                opacity = MathHelper.Lerp(1f, 0f, (float)(timeLeftMax / 2f - timeLeft) / (timeLeftMax / 2f));
        }
        private void Spawn()
        {
            timeLeftMax = timeLeft;
            size = Main.rand.NextFloat(2f, 4f) / 10f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D circle = ModContent.Request<Texture2D>("Redemption/Textures/Circle").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value;

            Color bright = Color.Multiply(new(255, 255, 255, 0), opacity);
            Color mid = Color.Multiply(new(161, 255, 253, 0), opacity);
            Color dark = Color.Multiply(new(40, 186, 242, 0), opacity);
            switch (ai[0])
            {
                case 1:
                    bright = Color.Multiply(new(255, 255, 255, 0), opacity);
                    mid = Color.Multiply(new(255, 255, 174, 0), opacity);
                    dark = Color.Multiply(new(255, 189, 69, 0), opacity);
                    break;
                case 2:
                    bright = Color.Multiply(new(255, 146, 135, 0), opacity);
                    mid = Color.Multiply(new(223, 62, 55, 0), opacity);
                    dark = Color.Multiply(new(150, 20, 54, 0), opacity);
                    break;
            }
            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), opacity);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), 1f);

            float pixelRatio = 1f / 64f;
            spriteBatch.Draw(glow, VisualPosition, new Rectangle(0, 0, 64, 64), glowColor, rotation, new Vector2(32f, 32f), 1f * size * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(circle, VisualPosition - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor, rotation, Vector2.Zero, 1f * pixelRatio * 3f * size * scale, SpriteEffects.None, 0f);
            return false;
        }

    }
}
