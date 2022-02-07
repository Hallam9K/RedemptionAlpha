using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class EmberParticle : Particle
    {
        public Color[] colors = new Color[] { new Color(187, 63, 25, 0), new Color(131, 23, 37, 0) };
        public int timer = Main.rand.Next(50, 100);
        public int colorTimer = 0;
        public int colorPosition = 0;
        public float sineX;
        public float sineY;
        public float speedX = 6f;
        public float speedY = 3f;
        public float multX = 0.1f;
        public float multY = 0.1f;
        public float opacity = 1f;
        public override void SetDefaults()
        {
            particle.width = 1;
            particle.height = 1;
            particle.timeLeft = 240;
            particle.tileCollide = false;
            particle.oldPosLength = 20;
            particle.scale = 1f;
            timer = Main.rand.Next(50, 100);
            speedX = Main.rand.NextFloat(4f, 9f);
            speedX = Main.rand.NextFloat(4f, 9f);
            multX = Main.rand.NextFloat(10f, 31f) / 100f;
            multX = Main.rand.NextFloat(10f, 31f) / 100f;
            particle.velocity = new Vector2((Main.windSpeedCurrent * Main.windPhysicsStrength + (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX)) * multX, 0f);
        }
        public override void AI()
        {
            sineX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX);
            sineY = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedY);
            if (timer == 0)
            {
                timer = Main.rand.Next(50, 100);
                speedX = Main.rand.NextFloat(4f, 9f);
                speedX = Main.rand.NextFloat(4f, 9f);
                multX = Main.rand.NextFloat(10f, 31f) / 100f;
                multX = Main.rand.NextFloat(10f, 31f) / 100f;
            }
            particle.velocity += new Vector2(Main.windSpeedCurrent * (Main.windPhysicsStrength * 3f) * MathHelper.Lerp(1f, 0.1f, Math.Abs(particle.velocity.X) / 6f), 0f);
            particle.velocity += new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX) * multX * 0.5f, -Main.rand.NextFloat(1f, 2f) / 100f);
            Utils.Clamp(particle.velocity.X, -6f, 6f);
            Utils.Clamp(particle.velocity.Y, -6f, 6f);
            timer--;
            if (particle.timeLeft <= 120)
            {
                opacity = MathHelper.Lerp(0f, 1f, particle.timeLeft / 120f);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D ember = ModContent.Request<Texture2D>("Redemption/Particles/EmberParticle").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value;
            float c = 1f / 255f;
            Color bright = new(187 * c * opacity, 63 * c * opacity, 25 * c * opacity, 0);
            Color dark = new(131 * c * opacity, 23 * c * opacity, 37 * c * opacity, 0);

            for (int i = 0; i < particle.oldPos.Length; i++)
            {
                Color trail = Color.Lerp(bright, dark, i / 20f);
                float scale = 1f - 0.9f / 20f * i;
                trail.R = (byte)(opacity * trail.R);
                trail.G = (byte)(opacity * trail.G);
                trail.B = (byte)(opacity * trail.B);
                trail.A = (byte)(opacity * trail.A);
                spriteBatch.Draw(glow, particle.oldPos[i] - screenPos - new Vector2(8f * scale, 8f * scale), new Rectangle(0, 0, 64, 64), trail, particle.rotation, Vector2.Zero, 0.25f * scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(ember, particle.oldPos[i] - screenPos, new Rectangle(0, 0, 3, 3), trail, particle.rotation, new Vector2(1.5f, 1.5f), scale, SpriteEffects.None, 0f);
            }
            Color color = Color.Lerp(bright, dark, timeLeft / 240);
            color.R = (byte)(opacity * color.R);
            color.G = (byte)(opacity * color.G);
            color.B = (byte)(opacity * color.B);
            color.A = (byte)(opacity * color.A);
            spriteBatch.Draw(glow, particle.position - screenPos - new Vector2(8f, 8f), new Rectangle(0, 0, 64, 64), color, particle.rotation, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(ember, particle.position - screenPos, new Rectangle(0, 0, 3, 3), color, particle.rotation, new Vector2(1.5f, 1.5f), particle.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
