using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class BlueEmberParticle : Particle
    {
        public BlueEmberParticle() : this(0, 0) { }
        public BlueEmberParticle(int aiTimer, byte aiType)
        {
            AITimer = aiTimer;
            AIType = aiType;
        }
        public override string Texture => "Redemption/Particles/EmberParticle";

        public int timer = Main.rand.Next(50, 100);
        public float speedX = Main.rand.NextFloat(4f, 9f);
        public float mult = Main.rand.NextFloat(10f, 31f) / 200f;
        public int timeLeftMax;
        public float size = 0f;
        public int AITimer;
        public byte AIType;

        public override void Spawn()
        {
            TimeLeft = Main.rand.Next(90, 121);
            TileCollide = false;
            if (AIType == 1)
                TimeLeft /= 7;
            timeLeftMax = TimeLeft;
            size = Main.rand.NextFloat(5f, 11f) / 10f;
        }

        public override void Update()
        {
            if (AITimer <= 0)
            {
                if (TimeLeft <= timeLeftMax / 2f)
                    Opacity = MathHelper.Lerp(1f, 0f, (float)(timeLeftMax / 2f - TimeLeft) / (timeLeftMax / 2f));

                if (AIType is 1)
                    return;

                if (AIType is 2)
                {
                    Velocity *= 0.9f;
                }
                else
                {
                    float sineX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX);

                    // Makes the particle change directions or speeds.
                    // Timer is used for keeping track of the current cycle
                    if (timer == 0)
                        NewMovementCycle();

                    // Adds the wind velocity to the particle.
                    // It adds less the faster it is already going.
                    Velocity += new Vector2(Main.windSpeedCurrent * (Main.windPhysicsStrength * 3f) * MathHelper.Lerp(1f, 0.1f, Math.Abs(Velocity.X) / 6f), 0f);
                    // Add the sine component to the velocity.
                    // This is scaled by the mult, which changes every cycle.
                    Velocity += new Vector2(sineX * mult, -Main.rand.NextFloat(1f, 2f) / 100f);

                    // Clamp the velocity so the particle doesnt go too fast.
                    Utils.Clamp(Velocity.X, -6f, 6f);
                    Utils.Clamp(Velocity.Y, -6f, 6f);
                }
                // Decrement the timer
                timer--;
            }
            AITimer--;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Color bright = Color.Multiply(new Color(30, 182, 228, 0), Opacity);
            Color mid = Color.Multiply(new Color(20, 91, 183, 0), Opacity);
            Color dark = Color.Multiply(new Color(13, 26, 139, 0), Opacity);

            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - TimeLeft) / timeLeftMax), Opacity);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - TimeLeft) / timeLeftMax), 1f);

            float pixelRatio = 1f / 64f;
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value, location, new Rectangle(0, 0, 64, 64), glowColor, Rotation, new Vector2(32f, 32f), 1f * size * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/Circle").Value, location - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor, Rotation, Vector2.Zero, 1f * pixelRatio * 3f * size * Scale, SpriteEffects.None, 0f);
            if (AIType < 1)
                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Particles/EmberParticle").Value, location, new Rectangle(0, 0, 3, 3), Color, Rotation, new Vector2(1.5f, 1.5f), 1f * Scale, SpriteEffects.None, 0f);
        }
        private void NewMovementCycle()
        {
            timer = Main.rand.Next(50, 100);
            speedX = Main.rand.NextFloat(4f, 9f);
            mult = Main.rand.NextFloat(10f, 31f) / 200f;
        }
    }
}