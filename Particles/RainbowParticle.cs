using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Textures;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Particles
{
    public class RainbowParticle : Particle
    {
        public override string Texture => "Redemption/Particles/RainbowParticle1";
        int frameCount;
        int frameTick;

        public override void SetDefaults()
        {
            width = 34;
            height = 34;
            scale = Vector2.One;
            timeLeft = 50;
            oldPos = new Vector2[10];
            oldRot = new float[10];
        }
        public override void AI()
        {
            for (int i = oldPos.Length - 1; i > 0; i--)
            {
                oldPos[i] = oldPos[i - 1];
            }
            oldPos[0] = position;
            for (int i = oldRot.Length - 1; i > 0; i--)
            {
                oldRot[i] = oldRot[i - 1];
            }
            oldRot[0] = rotation;
            if (ai[0] == 0)
            {
                if (ai[5] == 0)
                    ai[5] = 1;

                ai[1] = Main.rand.NextFloat(2f, 8f) / 10f;
                ai[2] = Main.rand.Next(0, 2);
                ai[3] = Main.rand.NextFloat(0f, 360f);
                timeLeft = (int)ai[4] > 0 ? (int)ai[4] : timeLeft;
            }
            ai[0]++;
            rotation += Utils.Clamp(velocity.X * 0.025f, -ai[1], ai[1]);
            velocity *= 0.98f;
            color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), (360f - timeLeft) / 360f);

            if (scale.Length() <= 0)
                active = false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            opacity = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            Color color2 = Color.Multiply(new(hsl.R, hsl.G, hsl.B, 0), opacity);
            spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, VisualPosition, null, color2 * ai[5], ai[3].InRadians().AngleLerp((ai[3] + 90f).InRadians(), (120f - timeLeft) / 120f), CommonTextures.RainbowParticle2.Size() / 2, 0.75f * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, VisualPosition, null, color2 * ai[5], ai[3].InRadians().AngleLerp((ai[3] + 90f).InRadians(), (120f - timeLeft) / 120f) + MathHelper.PiOver2, CommonTextures.RainbowParticle2.Size() / 2, 0.75f * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(CommonTextures.WhiteFlare.Value, VisualPosition, null, color2 * ai[5], 0f, CommonTextures.WhiteFlare.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(CommonTextures.GlowParticle.Value, VisualPosition, null, color2 * .5f, rotation, CommonTextures.GlowParticle.Size() / 2, Scale * .3f * ai[5], SpriteEffects.None, 0f);
            return false;
        }
    }
}
