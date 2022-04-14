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
            Texture2D ember = ModContent.Request<Texture2D>("Redemption/Particles/EmberParticle").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value;

            Color bright = Color.Multiply(new(255, 255, 255, 0), opacity);
            Color mid = Color.Multiply(new(161, 255, 253, 0), opacity);
            Color dark = Color.Multiply(new(40, 186, 242, 0), opacity);

            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), opacity);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), 1f);

            float pixelRatio = 1f / 64f;
            spriteBatch.Draw(glow, VisualPosition, new Rectangle(0, 0, 64, 64), glowColor, rotation, new Vector2(32f, 32f), 1f * size, SpriteEffects.None, 0f);
            spriteBatch.Draw(circle, VisualPosition - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor, rotation, Vector2.Zero, 1f * pixelRatio * 3f * size, SpriteEffects.None, 0f);
            //spriteBatch.Draw(ember, VisualPosition, new Rectangle(0, 0, 3, 3), color, rotation, new Vector2(1.5f, 1.5f), 1f, SpriteEffects.None, 0f);
            return false;
        }

    }
}
