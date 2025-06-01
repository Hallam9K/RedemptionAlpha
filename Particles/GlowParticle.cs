using ParticleLibrary.Core.V3.Particles;
using static Redemption.Particles.ParticleBehaviors;

namespace Redemption.Particles
{
    public class GlowParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/SoftGlow";
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
}