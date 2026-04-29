using ParticleLibrary.Core.V3.Particles;
using ParticleLibrary.Utilities;
using System;
using Terraria;
using SystemVector2 = System.Numerics.Vector2;

namespace Redemption.Particles
{
    public class BlackholeParticleBehavior : Behavior<ParticleInfo>
    {
        public override string Texture { get; } = "Redemption/Textures/WhiteGlow";
        public override void Update(ref ParticleInfo info)
        {
            uint packed = BitConverter.SingleToUInt32Bits(info.Data[1]);
            Color c = new(
              r: (byte)(packed & 0xFF),
              g: (byte)((packed >> 8) & 0xFF),
              b: (byte)((packed >> 16) & 0xFF),
              alpha: (byte)((packed >> 24) & 0xFF)
            );

            int HostWhoAmI = (int)info.Data[0];

            if (info.Time > info.Duration - 10)
                info.Data[2] += .2f;

            if (HostWhoAmI >= 0 && Main.projectile[HostWhoAmI].active && Main.projectile[HostWhoAmI].ai[1] is 0)
            {
                SystemVector2 hostCenter = Main.projectile[HostWhoAmI].Center.ToNumerics();
                info.Velocity = Move(info, hostCenter, 20, 2);
                if (SystemVector2.Distance(hostCenter, info.Position) < 50)
                    FadeOut(ref info, ref info.Data[2]);
            }
            else
                FadeOut(ref info, ref info.Data[2]);

            float ScaleX = info.InitialScale.X * MathHelper.Clamp(info.Velocity.Length() / 100f, 0, 5);
            float ScaleY = info.InitialScale.Y * MathHelper.Clamp(1 - info.Velocity.Length() / 500f, 0.15f, 1);
            info.Scale = new(ScaleX, ScaleY);

            info.Rotation = info.Velocity.ToRotation() + 1.57f;
            info.Color = c * info.Data[2];
            info.Position += info.Velocity;
            info.Time--;
        }
        private static SystemVector2 Move(ParticleInfo info, SystemVector2 endPos, float speed, float turnResistance)
        {
            SystemVector2 moveTo = endPos;
            SystemVector2 move = moveTo - info.Position;
            float magnitude = MathF.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            move = (info.Velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = MathF.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            return move;
        }

        private static void FadeOut(ref ParticleInfo info, ref float opacity)
        {
            opacity -= 1f;
            if (opacity <= 0)
                info.Time = 0;
        }
    }
}