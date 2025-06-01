using ParticleLibrary.Core.V3.Particles;

namespace Redemption.Particles
{
    public class LaserParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Terraria/Images/Extra_98";
        public override void Update(ref ParticleInfo info)
        {
            float timer = info.Time - 10;
            info.Data[0] = 1 - timer / 8f;

            float x = (info.Data[0] - 0.5f) * info.Data[1];
            float opacity = 1 / (1 + x * x);

            System.Numerics.Vector2 scale = new System.Numerics.Vector2(0.075f + 0.2f * opacity, 1) * info.InitialScale.X;
            Color color = Color.Multiply(info.InitialColor with { A = 0 }, 1);

            info.Scale = scale * new System.Numerics.Vector2(72);
            info.Color = color * opacity;

            info.Position += info.Velocity;
            info.Time--;
        }
    }
}