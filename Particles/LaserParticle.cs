using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Terraria;
using Terraria.GameContent;

namespace Redemption.Particles
{
    public class LaserParticle  : Particle
    {
        public override string Texture => "Terraria/Images/Projectile_536";
        public float Squish;
        public LaserParticle() : this(5) { }
        public LaserParticle(float squish)
        {
            Squish = squish;
        }
        public float timer;
        private float progress;
        public override void Spawn()
        {
            TimeLeft = 18;
            timer = 8;
            TileCollide = false;
            Rotation = Velocity.ToRotation();
            Velocity *= 0;
        }
        public override void Update()
        {
            timer--;
            progress = 1 - timer / 8;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            Rectangle rect = texture.Frame(1, 1);
            Vector2 origin = rect.Size() / 2;

            float x = (progress - 0.5f) * Squish;
            float opacity = 1 / (1 + x * x);

            Vector2 scale = new Vector2(0.075f + 0.2f * opacity, 1) * Scale;
            Color color = Color.Multiply(Color with { A = 0 }, 1);

            Vector2 drawPos = Position;
            spriteBatch.Draw(texture, drawPos - Main.screenPosition, null, color * opacity * 1f, Rotation + MathHelper.PiOver2, origin, scale, 0, 0);
            spriteBatch.Draw(texture, drawPos - Main.screenPosition, null, color * opacity * 1f, Rotation + MathHelper.PiOver2, origin, scale, 0, 0);
        }
    }
}
