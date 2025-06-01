using ParticleLibrary.Core.V3.Particles;
using System;

namespace Redemption.Particles
{
    public class PulseParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/Shockwave";
        public override void Update(ref ParticleInfo info)
        {
            float ScaleSpeed = info.Data[0];

            float progress = 1 - info.Time / (float)info.Duration;
            float Opacity = 1 - progress;
            info.Scale = info.InitialScale * MathF.Pow(progress, ScaleSpeed);

            info.Color = info.InitialColor * Opacity;

            info.Position += info.Velocity;
            info.Time--;
        }
    }
}