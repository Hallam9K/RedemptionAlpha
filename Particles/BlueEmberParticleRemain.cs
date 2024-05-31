using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class BlueEmberParticleRemain : Particle
    {
        public BlueEmberParticleRemain() : this(false, 0) { }
        public BlueEmberParticleRemain(bool shortLived, float fadeAdd)
        {
            ShortLived = shortLived;
            FadeAdd = fadeAdd;
        }
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public int timer = Main.rand.Next(50, 100);
        public float speedX = Main.rand.NextFloat(4f, 9f);
        public float mult = Main.rand.NextFloat(10f, 31f) / 200f;
        public int timeLeftMax;
        public float size = 0f;
        public bool ShortLived;
        public float FadeAdd;

        public override void Spawn()
        {
            TimeLeft = Main.rand.Next(90, 121);
            TileCollide = false;
            if (ShortLived)
                TimeLeft /= 7;
            timeLeftMax = TimeLeft;
            size = Main.rand.NextFloat(2f, 11f) / 10f;
        }

        public override void Update()
        {
            Velocity *= 0.96f;
            // Halfway through, start fading.
            if (TimeLeft <= timeLeftMax / 2f)
                Opacity = MathHelper.Lerp(1f, 0f, (float)(timeLeftMax / 2f - TimeLeft) / (timeLeftMax / 2f));
            else
            {
                Opacity += FadeAdd + .05f;
                Opacity = MathHelper.Clamp(Opacity, 0, 1);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Color bright = Color.Multiply(new(30, 182, 228, 0), Opacity);
            Color mid = Color.Multiply(new(20, 91, 183, 0), Opacity);
            Color dark = Color.Multiply(new(13, 26, 139, 0), Opacity);

            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - TimeLeft) / timeLeftMax), Opacity);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - TimeLeft) / timeLeftMax), 1f);

            float pixelRatio = 1f / 64f;
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow").Value, location, new Rectangle(0, 0, 64, 64), glowColor, Rotation, new Vector2(32f, 32f), 1f * size * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/Circle").Value, location - new Vector2(1.5f, 1.5f), new Rectangle(0, 0, 64, 64), emberColor, Rotation, Vector2.Zero, 1f * pixelRatio * 3f * size * Scale, SpriteEffects.None, 0f);
            if (!ShortLived)
                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Particles/EmberParticle").Value, location, new Rectangle(0, 0, 3, 3), Color, Rotation, new Vector2(1.5f, 1.5f), 1f * Scale, SpriteEffects.None, 0f);
        }
    }
}