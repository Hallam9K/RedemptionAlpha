using ParticleLibrary.Core.V3.Particles;
using static Redemption.Particles.ParticleBehaviors;

namespace Redemption.Particles
{
    public class SlashParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Particles/RainbowParticle2";
        public override void Update(ref ParticleInfo info)
        {
            ParticleFlags2 behavior = (ParticleFlags2)((int?)(info.Data.Length > 0 ? info.Data[0] : 0) ?? 0);
            if (behavior.HasFlag(ParticleFlags2.DaggerSlash))
                DaggerSlashBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags2.EmeraldCutter))
                EmeraldCutterBehavior(ref info);
        }
    }
    public class SlashAltParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Terraria/Images/Extra_98";
        public override void Update(ref ParticleInfo info)
        {
            ParticleFlags2 behavior = (ParticleFlags2)((int?)(info.Data.Length > 0 ? info.Data[0] : 0) ?? 0);
            if (behavior.HasFlag(ParticleFlags2.Slash))
                SlashBehavior(ref info);
            if (behavior.HasFlag(ParticleFlags2.DevilsPact))
                DevilsPactBehavior(ref info);
        }
    }
}