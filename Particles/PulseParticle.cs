using ParticleLibrary.Core.V3.Particles;
using System;

namespace Redemption.Particles
{
    public class PulseParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/Shockwave";
        public override void Update(ref ParticleInfo info)
        {
            uint packed = BitConverter.SingleToUInt32Bits(info.Data[1]);
            Color c = new(
              r: (byte)(packed & 0xFF),
              g: (byte)((packed >> 8) & 0xFF),
              b: (byte)((packed >> 16) & 0xFF),
              alpha: (byte)((packed >> 24) & 0xFF)
            );

            float ScaleSpeed = info.Data[0];

            float progress = 1 - info.Time / (float)info.Duration;
            float Opacity = 1 - progress;
            info.Scale = info.InitialScale * MathF.Pow(progress, ScaleSpeed);
            info.Color = c * Opacity;
            info.Position += info.Velocity;
            info.Time--;
        }
    }
}