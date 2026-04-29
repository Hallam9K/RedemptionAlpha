using ParticleLibrary.Core.V3.Particles;
using Terraria;

namespace Redemption.Particles
{
    public class RainbowParticleBehavior : GlowParticleBehavior
    {
        public override string Texture { get; } = "Redemption/Particles/RainbowParticle2";
        public override void Initialize(ref ParticleInfo info)
        {
            float OpacityScale = info.Data[1];
            float Opacity = info.Time <= 20 ? 1f - 1f / 20f * (20 - info.Time) : 1f;
            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            if (info.InitialColor != Color.White)
                hsl = info.InitialColor;
            Color color2 = Color.Multiply(new(hsl.R, hsl.G, hsl.B, 0), Opacity);

            info.Color = color2 * Opacity * OpacityScale;
        }
    }
}