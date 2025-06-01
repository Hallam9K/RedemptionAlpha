using ParticleLibrary.Core.V3.Particles;
using System;
using SystemVector2 = System.Numerics.Vector2;

namespace Redemption.Particles
{
    public class SpeedParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/WhiteGlow";
        public override void Update(ref ParticleInfo info)
        {
            float DeAcc = info.Data[0];
            float Extension = info.Data[2];

            float progress = (info.Time - 1) / (info.Duration - 1f);
            float ScaleX = ((info.InitialScale.X * 192f) + Extension) * progress;

            info.Scale = new SystemVector2(ScaleX, info.InitialScale.Y * 192f);

            info.Velocity *= DeAcc;
            float Opacity = MathF.Pow(progress, 2);
            info.Color = info.InitialColor * Opacity;

            if (info.Data[1] > 0)
            {
                info.Data[1]--;
                if (info.Data[1] <= 0)
                    info.Time = 0;
            }

            info.Position += info.Velocity;
            info.Time--;
        }
    }
}