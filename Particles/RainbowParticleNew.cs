using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Textures;
using Terraria;

namespace Redemption.Particles
{
    public class RainbowParticleNew : Particle
    {
        public RainbowParticleNew() : this(0, 1) { }
        public RainbowParticleNew(int newTimeLeft = 0, float opacScale = 1)
        {
            NewTimeLeft = newTimeLeft;
            OpacScale = opacScale;
        }

        public override string Texture => "Redemption/Particles/RainbowParticle1";
        public float[] AITimer = new float[5];
        public int NewTimeLeft;
        public float OpacScale = 1;
        public override void Spawn()
        {
            TimeLeft = 50;
        }
        public override void Update()
        {
            if (AITimer[0] == 0)
            {
                AITimer[1] = Main.rand.NextFloat(2f, 8f) / 10f;
                AITimer[2] = Main.rand.Next(0, 2);
                AITimer[3] = Main.rand.NextFloat(0f, 360f);
                TimeLeft = NewTimeLeft > 0 ? NewTimeLeft : TimeLeft;
            }
            AITimer[0]++;
            Rotation += Utils.Clamp(Velocity.X * 0.025f, -AITimer[1], AITimer[1]);
            Velocity *= 0.98f;
            Color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), (360f - TimeLeft) / 360f);
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Opacity = TimeLeft <= 20 ? 1f - 1f / 20f * (20 - TimeLeft) : 1f;
            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            Color color2 = Color.Multiply(new(hsl.R, hsl.G, hsl.B, 0), Opacity);
            spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, location, null, color2 * OpacScale, AITimer[3].InRadians().AngleLerp((AITimer[3] + 90f).InRadians(), (120f - TimeLeft) / 120f), CommonTextures.RainbowParticle2.Size() / 2, 0.75f * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, location, null, color2 * OpacScale, AITimer[3].InRadians().AngleLerp((AITimer[3] + 90f).InRadians(), (120f - TimeLeft) / 120f) + MathHelper.PiOver2, CommonTextures.RainbowParticle2.Size() / 2, 0.75f * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(CommonTextures.WhiteFlare.Value, location, null, color2 * OpacScale, 0f, CommonTextures.WhiteFlare.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(CommonTextures.GlowParticle.Value, location, null, color2 * .5f, Rotation, CommonTextures.GlowParticle.Size() / 2, Scale * .3f * OpacScale, SpriteEffects.None, 0f);
        }
    }
}