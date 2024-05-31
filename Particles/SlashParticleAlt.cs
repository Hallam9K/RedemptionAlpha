using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using System;
using Terraria;
using Terraria.GameContent;

namespace Redemption.Particles
{
    public class SlashParticleAlt : Particle
    {
        public override string Texture => "Terraria/Images/Projectile_536";
        public float TimerMax;
        public float Squish;
        public SlashParticleAlt(float timerMax = 16, float squish = 1)
        {
            TimerMax = timerMax;
            Squish = squish;
        }
        public float timer;
        private Vector2 startPos;
        private Vector2 leftEnd;
        private Vector2 rightEnd;
        private float progress;
        public override void Spawn()
        {
            TimeLeft = (int)TimerMax;
            timer = TimerMax;
            TileCollide = false;
            Rotation = Velocity.ToRotation();
            startPos = Position;
            leftEnd = Position + Velocity;
            rightEnd = Position - Velocity;
        }
        public override void Update()
        {
            timer--;
            progress = 1 - timer / TimerMax;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            Rectangle rect = texture.Frame(1, 1);
            Vector2 origin = rect.Size() / 2;
            Color color = Color;
            for (float i = 0f; i <= 1f; i += 0.1f)
            {
                Vector2 drawPos1 = Vector2.Lerp(startPos, rightEnd, i);
                Vector2 drawPos2 = Vector2.Lerp(startPos, leftEnd, i);
                float x = i * Squish;
                float opacity = 1 / (1 + x * x);
                float y = 1 - progress;
                float fadeOut = MathF.Min(1, y);
                Vector2 scale = new Vector2(0.1f * opacity, 1) * 1.5f * Scale;

                spriteBatch.End();
                spriteBatch.BeginAdditive();

                spriteBatch.Draw(texture, drawPos1 - Main.screenPosition, null, color * fadeOut * 0.3f, Rotation + MathHelper.PiOver2, origin, new Vector2(scale.X * 8, scale.Y * 2), 0, 0);
                spriteBatch.Draw(texture, drawPos2 - Main.screenPosition, null, color * fadeOut * 0.3f, Rotation + MathHelper.PiOver2, origin, new Vector2(scale.X * 8, scale.Y * 2), 0, 0);
                spriteBatch.Draw(texture, drawPos1 - Main.screenPosition, null, color * opacity * fadeOut, Rotation + MathHelper.PiOver2, origin, scale, 0, 0);
                spriteBatch.Draw(texture, drawPos2 - Main.screenPosition, null, color * opacity * fadeOut, Rotation + MathHelper.PiOver2, origin, scale, 0, 0);

                spriteBatch.End();
                spriteBatch.BeginDefault(true);
            }
        }
    }
}
