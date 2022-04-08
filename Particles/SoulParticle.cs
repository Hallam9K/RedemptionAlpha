using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class SoulParticle : EmberParticle
    {
        public override string Texture => "Redemption/Particles/EmberParticle";
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void AI()
        {
            // You can pass in a number to determine how long until it starts its ember movement.
            if (ai[0] <= 0)
            {
                float sineX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX);

                // Makes the particle change directions or speeds.
                // Timer is used for keeping track of the current cycle
                if (timer == 0)
                    NewMovementCycle();

                // Add the sine component to the velocity.
                // This is scaled by the mult, which changes every cycle.
                velocity += new Vector2(sineX * mult, -Main.rand.NextFloat(1f, 2f) / 100f);

                // Clamp the velocity so the particle doesnt go too fast.
                Utils.Clamp(velocity.X, -6f, 6f);
                Utils.Clamp(velocity.Y, -6f, 6f);

                // Decrement the timer
                timer--;

                // Halfway through, start fading.
                if (timeLeft <= timeLeftMax / 2f)
                    opacity = MathHelper.Lerp(1f, 0f, (float)(timeLeftMax / 2f - timeLeft) / (timeLeftMax / 2f));
            }
            ai[0]--;
        }
        public void Spawn()
        {
            timeLeftMax = timeLeft;
            size = Main.rand.NextFloat(5f, 11f) / 10f;
        }
        public void NewMovementCycle()
        {
            timer = Main.rand.Next(50, 100);
            speedX = Main.rand.NextFloat(4f, 9f);
            mult = Main.rand.NextFloat(10f, 31f) / 200f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D circle = ModContent.Request<Texture2D>("Redemption/Textures/Circle").Value;
            Texture2D ember = ModContent.Request<Texture2D>("Redemption/Particles/EmberParticle").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value;

            Color bright = Color.Multiply(new(255, 255, 255, 0), opacity);
            Color mid = Color.Multiply(new(238, 251, 255, 0), opacity);
            Color dark = Color.Multiply(new(195, 231, 244, 0), opacity);

            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), opacity);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - timeLeft) / timeLeftMax), 1f);

            float pixelRatio = 1f / 64f;
            spriteBatch.Draw(glow, VisualPosition, new Rectangle(0, 0, 64, 64), glowColor * (ai[1] + 1), rotation, new Vector2(32f, 32f), 1f * size * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(circle, VisualPosition - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor * (ai[1] + 1), rotation, Vector2.Zero, 1f * pixelRatio * 3f * size * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ember, VisualPosition, new Rectangle(0, 0, 3, 3), color, rotation, new Vector2(1.5f, 1.5f), scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
