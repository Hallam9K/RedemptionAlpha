using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static Redemption.Effects.RenderTargets.FlameLayer;

namespace Redemption.Particles
{
    public class EmberParticle : Particle, IFlameSprite
    {
        public int timer = Main.rand.Next(50, 100);
        public int timeLeftMax;
        public float sineX;
        public float sineY;
        public float speedX = 6f;
        public float speedY = 3f;
        public float mult = 0.1f;
        public float opacity = 1f;
        public float size = 0f;
        public float sizeLiftime = 0f;

        public bool Active { get => active; set => active = value; }

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = Main.rand.Next(90, 121);
            timeLeftMax = timeLeft;
            tileCollide = false;
            oldPos = new Vector2[3];
            scale = 1f;
            timer = Main.rand.Next(50, 100);
            speedX = Main.rand.NextFloat(4f, 9f);
            speedX = Main.rand.NextFloat(4f, 9f);
            mult = Main.rand.NextFloat(10f, 31f) / 100f;
            mult = Main.rand.NextFloat(10f, 31f) / 100f;
            velocity = new Vector2((Main.windSpeedCurrent * Main.windPhysicsStrength + (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX)) * mult, 0f);
            DeathAction = Death;
        }

        public override void AI()
        {
            if (timeLeft == timeLeftMax)
            {
                size = Main.rand.NextFloat(5f, 11f) / 10f;
                Redemption.Targets.FlameLayer.Sprites.Add(this);
            }
            sineX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX);
            sineY = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedY);
            if (timer == 0)
            {
                timer = Main.rand.Next(50, 100);
                speedX = Main.rand.NextFloat(4f, 9f);
                speedX = Main.rand.NextFloat(4f, 9f);
                mult = Main.rand.NextFloat(10f, 31f) / 100f;
                mult = Main.rand.NextFloat(10f, 31f) / 100f;
            }
            velocity += new Vector2(Main.windSpeedCurrent * (Main.windPhysicsStrength * 3f) * MathHelper.Lerp(1f, 0.1f, Math.Abs(velocity.X) / 6f), 0f);
            velocity += new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX) * mult * 0.5f, -Main.rand.NextFloat(1f, 2f) / 100f);
            Utils.Clamp(velocity.X, -6f, 6f);
            Utils.Clamp(velocity.Y, -6f, 6f);
            timer--;
            if (timeLeft <= timeLeftMax / 2f)
            {
                opacity = MathHelper.Lerp(0f, 1f, timeLeft / (timeLeftMax / 2f));
                sizeLiftime = MathHelper.Lerp(2f, 0.5f, timeLeft / (timeLeftMax / 2f));
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D circle = ModContent.Request<Texture2D>("Embers/Textures/Circle").Value;
            Texture2D ember = ModContent.Request<Texture2D>("Embers/Particles/EmberParticle").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Embers/Textures/SoftGlow").Value;
            float c = 1f / 255f;
            //Color bright = new(240 * c * opacity, 149 * c * opacity, 46 * c * opacity, 0);
            //Color mid = new(187 * c * opacity, 63 * c * opacity, 25 * c * opacity, 0);
            //Color dark = new(131 * c * opacity, 23 * c * opacity, 37 * c * opacity, 0);
            Color bright = new(0.5f, 0.5f, 0.5f, 0f);
            Color mid = new(0.375f, 0.375f, 0.375f, 0f);
            Color dark = new(0.25f, 0.25f, 0.25f, 0f);


            //for (int i = 0; i < oldPos.Length; i++)
            //{
            //  Color trail = Color.Lerp(bright, dark, i / 3f);
            //  float scale = 1f - 0.9f / 3f * i * size;
            //  trail.R = (byte)(opacity * trail.R);
            //  trail.G = (byte)(opacity * trail.G);
            //  trail.B = (byte)(opacity * trail.B);
            //  trail.A = (byte)(opacity * trail.A);
            //  //spriteBatch.Draw(glow, oldPos[i] - Main.screenPosition - new Vector2(8f * scale, 8f * scale), new Rectangle(0, 0, 64, 64), trail, rotation, Vector2.Zero, 0.25f * scale, SpriteEffects.None, 0f);
            //  spriteBatch.Draw(ember, oldPos[i] - Main.screenPosition - new Vector2(32f * 0.0625f * size, 32f * 0.0625f * size), new Rectangle(0, 0, 64, 64), trail, rotation, Vector2.Zero, scale * 0.0625f * size, SpriteEffects.None, 0f);
            //}
            Color color = Color.Lerp(bright, dark, (float)timeLeft / timeLeftMax);
            Color color2 = Color.Lerp(mid, dark, (float)timeLeft / timeLeftMax);
            color.R = (byte)(opacity * color.R);
            color.G = (byte)(opacity * color.G);
            color.B = (byte)(opacity * color.B);
            color.A = (byte)(opacity * color.A);
            // * (2f - (opacity * 1.25f))
            float c2 = 1f / 64f;
            spriteBatch.Draw(glow, position - Main.screenPosition - new Vector2(32f, 32f), new Rectangle(0, 0, 64, 64), Color.Multiply(color2, 0.25f), rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(circle, position - Main.screenPosition - new Vector2(32f * (c2 * 1.5f), 32f * (c2 * 1.5f)), new Rectangle(0, 0, 64, 64), color, rotation, Vector2.Zero, 1f * c2 * 3f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(ember, position - Main.screenPosition - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 3, 3), color, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        private void Death()
        {
            Redemption.Targets.FlameLayer.Sprites.Remove(this);
            active = false;
        }
    }
}
