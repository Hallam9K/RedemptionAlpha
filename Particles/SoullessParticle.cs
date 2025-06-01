using ParticleLibrary.Core.V3.Particles;

namespace Redemption.Particles
{
    public class SoullessParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/WhiteOrb";
        public override void Update(ref ParticleInfo info)
        {
            float Opacity = info.Data[1];

            if (info.Data[0] <= 0)
            {
                info.Velocity *= .9f;
                // Halfway through, start fading.
                if (info.Time <= info.Duration / 2f)
                    Opacity = MathHelper.Lerp(1f, 0f, (float)(info.Duration / 2f - info.Time) / (info.Duration / 2f));
            }
            info.Data[0]--;

            info.Color = info.InitialColor * Opacity;

            info.Position += info.Velocity;
            info.Time--;
        }
    }
}