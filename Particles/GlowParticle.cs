using ParticleLibrary.Core.V3.Particles;
using System;
using static Redemption.Particles.ParticleBehaviors;

namespace Redemption.Particles
{
    public class GlowParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/SoftGlow";
        public override void Initialize(ref ParticleInfo info)
        {
            ParticleFlags behavior = (ParticleFlags)((int?)(info.Data.Length > 0 ? info.Data[0] : 0) ?? 0);
            if (behavior.HasFlag(ParticleFlags.Fading))
            {
                float opacity = info.Time / (info.Duration / 2f);
                if (info.Time > info.Duration / 2f)
                    opacity = MathHelper.Clamp((info.Data[1] + .05f) * (info.Duration - info.Time), 0f, 1f);

                Color endColor = info.InitialColor * 0.8f;
                info.Color = Color.Lerp(info.InitialColor, endColor, info.Duration / (float)info.Time) * opacity;
            }
            if (behavior.HasFlag(ParticleFlags.Ember))
            {
                float half = info.Duration / 2f;
                float mult = info.Time > half ? (half - (info.Time - half)) / half : info.Time / half;

                ColorTransitionBehaviour(ref info, 2);

                info.Scale = info.InitialScale * mult;
            }
            if (behavior.HasFlag(ParticleFlags.EmberBurst))
            {
                float opacity = 1;
                if (info.Time <= (info.Duration / 2f))
                    opacity = info.Time / (info.Duration / 2f);

                ColorTransitionBehaviour(ref info);
                info.Color *= opacity;
            }
            if (behavior.HasFlag(ParticleFlags.Spirit))
            {
                var opacityScale = info.Data[6];

                float opacity = info.Time / (info.Duration / 2f);
                if (info.Time > info.Duration / 2f)
                    opacity = MathHelper.Clamp(.05f * (info.Duration - info.Time), 0f, 1f);

                ColorTransitionBehaviour(ref info, 5);
                info.Color *= opacity * opacityScale;
            }
            //if (behavior.HasFlag(ParticleFlags.Rainbow))
            //    RainbowBehavior(ref info);
            //if (behavior.HasFlag(ParticleFlags.SimpleStar))
            //    SimpleStarBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.ColorTransition))
                ColorTransitionBehaviour(ref info);
        }
        public override void Update(ref ParticleInfo info)
        {
            ParticleFlags behavior = (ParticleFlags)((int?)(info.Data.Length > 0 ? info.Data[0] : 0) ?? 0);
            if (behavior.HasFlag(ParticleFlags.Basic))
                BasicBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.Fading))
                FadingGlowBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.Quad))
                QuadBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.Ember))
                EmberBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.EmberBurst))
                EmberBurstBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.Charge))
                ChargeBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.Spirit))
                SpiritBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.Rainbow))
                RainbowBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.SimpleStar))
                SimpleStarBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.Custom))
                CustomBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags.ColorTransition))
                ColorTransitionBehaviour(ref info);
        }
    }
    public class GlowParticle2Behavior : GlowParticleBehavior
    {
        public override string Texture { get; } = "Redemption/Textures/Circle";
    }
    public class GlowParticle3Behavior : GlowParticleBehavior
    {
        public override string Texture { get; } = "Redemption/Particles/Star";
    }
    public class BigFlareParticleBehavior : GlowParticleBehavior
    {
        public override string Texture { get; } = "Redemption/Textures/BigFlare";
    }
    public class GlowParticle4Behavior : GlowParticleBehavior
    {
        public override string Texture { get; } = "Redemption/Particles/GlowParticle";
    }
    public class WhiteFlareParticleBehavior : GlowParticleBehavior
    {
        public override string Texture { get; } = "Redemption/Textures/WhiteFlare";
    }
    public class WhiteGlowParticleBehavior : GlowParticleBehavior
    {
        public override string Texture { get; } = "Redemption/Textures/WhiteGlow";
    }
}