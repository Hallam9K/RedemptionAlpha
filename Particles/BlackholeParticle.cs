using ParticleLibrary.Core.V3.Particles;
using ParticleLibrary.Utilities;
using System;
using Terraria;

namespace Redemption.Particles
{
    public class BlackholeParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/WhiteGlow";
        public override void Update(ref ParticleInfo info)
        {
            int HostWhoAmI = (int)info.Data[1];
            float Opacity = info.Data[2];
            info.Position += info.Velocity;

            if (info.Time > info.Duration - 10)
                Opacity += .2f;

            float ScaleX = info.InitialScale.X * MathHelper.Clamp(info.Velocity.Length() / 100, 0, 5);
            float ScaleY = info.InitialScale.Y * MathHelper.Clamp(1 - info.Velocity.Length() / 500, 0.25f, 1);
            info.Scale = new(ScaleX, ScaleY);

            info.Rotation = info.Velocity.ToRotation() + 1.57f;

            if (HostWhoAmI >= 0 && Main.projectile[HostWhoAmI].active && Main.projectile[HostWhoAmI].ai[1] is 0)
            {
                Move(info, Main.projectile[HostWhoAmI].Center.ToNumerics(), 20, 5);
                if (System.Numerics.Vector2.Distance(Main.projectile[HostWhoAmI].Center.ToNumerics(), info.Position) < 50)
                    FadeOut(ref info, ref Opacity);
            }
            else
                FadeOut(ref info, ref Opacity);

            info.Color = info.InitialColor * Opacity;

            info.Time--;
        }
        private static void Move(ParticleInfo info, System.Numerics.Vector2 endPos, float speed, float turnResistance = 10f)
        {
            System.Numerics.Vector2 moveTo = endPos;
            System.Numerics.Vector2 move = moveTo - info.Position;
            float magnitude = MathF.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
                move *= speed / magnitude;

            move = (info.Velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = MathF.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
                move *= speed / magnitude;

            info.Velocity = move;
        }
        private static void FadeOut(ref ParticleInfo info, ref float opacity)
        {
            opacity -= .2f;
            if (opacity <= 0)
                info.Time = 0;
        }
    }
}