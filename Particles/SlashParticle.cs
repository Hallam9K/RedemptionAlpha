using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Terraria;
using Terraria.GameContent;

namespace Redemption.Particles
{
    public class SlashParticle : Particle
    {
        public override string Texture => "Terraria/Images/Projectile_536";
        public float TimerMax;
        public SlashParticle() : this(6) { }
        public SlashParticle(float timerMax)
        {
            TimerMax = timerMax;
        }
        public float timer;
        private Vector2 startPos;
        private Vector2 endPos;
        private float progress;
        public override void Spawn()
        {
            TimeLeft = 18;
            timer = TimerMax;
            TileCollide = false;
            Rotation = Velocity.ToRotation();
            startPos = Position + Velocity;
            endPos = Position - Velocity;
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
            Vector2 scale = new Vector2(0.15f, 1) * Scale;
            Color color = Color.Multiply(Color with { A = 0 }, 1);
            for (float num7 = 0f; num7 <= 1f; num7 += 0.1f)
            {
                float x = (num7 - progress + 0.4f) * 8;
                float opacity = 1 / (1 + x * x);
                Vector2 drawPos = Vector2.Lerp(startPos, endPos, num7);
                spriteBatch.Draw(texture, drawPos - Main.screenPosition, null, color * opacity * 1f, Rotation + MathHelper.PiOver2, origin, scale, 0, 0);
                spriteBatch.Draw(texture, drawPos - Main.screenPosition, null, color * opacity * 1f, Rotation + MathHelper.PiOver2, origin, scale, 0, 0);
            }
        }
    }
}
