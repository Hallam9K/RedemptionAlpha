using ParticleLibrary.Core.V3.Particles;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using System;
using Terraria;
using SystemVector2 = System.Numerics.Vector2;

namespace Redemption.Particles
{
    public static class ParticleBehaviors
    {
        [Flags]
        public enum ParticleFlags
        {
            Basic = 1,
            Fading = 2,
            Quad = 4,
            Ember = 8,
            EmberBurst = 16,
            Charge = 32,
            Spirit = 64,
            Rainbow = 128,
            SimpleStar = 256,
            Custom = 512,
            ColorTransition = 1024,
            Speed = 2048,
            // So on, in powers of 2
        }

        [Flags]
        public enum ParticleFlags2
        {
            DaggerSlash = 1,
            EmeraldCutter = 2,
            Slash = 4,
            DevilsPact = 8,
            // So on, in powers of 2
        }

        public static void FadingGlowBehavior(ref ParticleInfo info)
        {
            info.Velocity *= 0.96f;
            info.Position += info.Velocity;

            float opacity = info.Time / (info.Duration / 2f);
            if (info.Time > info.Duration / 2f)
                opacity = MathHelper.Clamp((info.Data[1] + .05f) * (info.Duration - info.Time), 0f, 1f);

            Color endColor = info.InitialColor * 0.8f;
            info.Color = Color.Lerp(info.InitialColor, endColor, info.Duration / (float)info.Time) * opacity;

            info.Time--;
        }

        public static void BasicBehavior(ref ParticleInfo info)
        {
            info.Velocity *= info.Data[1];
            info.Position += info.Velocity;

            float mult = info.Time / (float)info.Duration;

            info.Color = info.InitialColor * mult;
            info.Scale = info.InitialScale * mult;

            info.Rotation = info.Velocity.ToRotation();

            info.Time--;
        }

        public static void CustomBehavior(ref ParticleInfo info)
        {
            info.Velocity *= info.Data[2];
            info.Position += info.Velocity;

            info.Color = info.InitialColor * info.Data[3];
            info.Scale = info.InitialScale * info.Data[4];

            info.Rotation = info.Data[5];

            info.Time--;
        }

        public static void QuadBehavior(ref ParticleInfo info)
        {
            info.Velocity *= info.Data[2];
            info.Position += info.Velocity;

            uint packed = BitConverter.SingleToUInt32Bits(info.Data[1]);
            Color endColor = new(
              r: (byte)(packed & 0xFF),
              g: (byte)((packed >> 8) & 0xFF),
              b: (byte)((packed >> 16) & 0xFF),
              alpha: (byte)((packed >> 24) & 0xFF)
            );
            float mult = info.Time / (float)info.Duration;
            info.Color = Color.Lerp(info.InitialColor, endColor, mult) * mult;

            info.Scale *= info.Data[3];

            info.Time--;
        }

        public static void EmberBehavior(ref ParticleInfo info)
        {
            if (info.Data[1] <= 0)
            {
                // o = offset
                // m = multiplier
                float o = MathF.Sin((Main.GlobalTimeWrappedHourly / 120f) * MathF.Tau) * 32768f;
                float m = 0.4f;

                float x = RedeParticleManager.Perlin.GetNoise(info.Position.X + o, info.Position.Y + o) * m;
                float y = RedeParticleManager.Perlin.GetNoise(info.Position.Y + o, info.Position.X + o) * m;

                info.Velocity += new SystemVector2(x + (Main.windSpeedCurrent * (Main.windPhysicsStrength * 2f)), y);
                info.Velocity = SystemVector2.Clamp(info.Velocity, new SystemVector2(-6f), new SystemVector2(6f));
                info.Velocity *= 0.99f;
                info.Position += info.Velocity;
            }

            float half = info.Duration / 2f;
            float mult = info.Time > half ? (half - (info.Time - half)) / half : info.Time / half;

            ColorTransitionBehaviour(ref info, 2);

            info.Scale = info.InitialScale * mult;
            info.Time--;
            info.Data[1]--;
        }
        #region Old Ember Behaviour
        /*public static void OldEmberBehavior(ref ParticleInfo info)
        {
            var speedX = info.Data[3];
            var multi = info.Data[4];

            float opacity = info.Time / (info.Duration / 2f);
            float colorMult = info.Time / (float)info.Duration;

            uint packed = BitConverter.SingleToUInt32Bits(info.Data[5]);
            Color otherColor = new(
              r: (byte)(packed & 0xFF),
              g: (byte)((packed >> 8) & 0xFF),
              b: (byte)((packed >> 16) & 0xFF),
              alpha: (byte)((packed >> 24) & 0xFF)
            );

            info.Color = Color.Lerp(info.InitialColor.WithAlpha(0), otherColor.WithAlpha(0), colorMult) * opacity;

            if (info.Data[1] <= 0)
            {
                float sineX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX);

                // Makes the particle change directions or speeds.
                // Timer is used for keeping track of the current cycle
                if (info.Data[2] == 0)
                {
                    info.Data[2] = Main.rand.Next(50, 100);
                    info.Data[3] = Main.rand.NextFloat(4f, 9f);
                    info.Data[4] = Main.rand.NextFloat(10f, 31f) / 200f;
                }

                // Adds the wind velocity to the particle.
                // It adds less the faster it is already going.
                info.Velocity += new System.Numerics.Vector2(Main.windSpeedCurrent * (Main.windPhysicsStrength * 3f) * MathHelper.Lerp(1f, 0.1f, Math.Abs(info.Velocity.X) / 6f), 0f);
                // Add the sine component to the velocity.
                // This is scaled by the mult, which changes every cycle.
                info.Velocity += new System.Numerics.Vector2(sineX * multi, -Main.rand.NextFloat(1f, 2f) / 100f);

                // Clamp the velocity so the particle doesnt go too fast.
                Utils.Clamp(info.Velocity.X, -6f, 6f);
                Utils.Clamp(info.Velocity.Y, -6f, 6f);
                // Decrement the timer
                info.Data[2]--;
            }
            info.Data[1]--;

            info.Position += info.Velocity;

            info.Time--;
        }*/
        #endregion
        public static void EmberBurstBehavior(ref ParticleInfo info)
        {
            float opacity = 1;
            if (info.Time <= (info.Duration / 2f))
                opacity = info.Time / (info.Duration / 2f);

            ColorTransitionBehaviour(ref info);
            info.Color *= opacity;

            info.Velocity *= info.Data[2];
            info.Position += info.Velocity;

            info.Time--;
        }

        public static void ColorTransitionBehaviour(ref ParticleInfo info, int dataID = 1)
        {
            uint packed = BitConverter.SingleToUInt32Bits(info.Data[dataID]);
            Color otherColor = new(
              r: (byte)(packed & 0xFF),
              g: (byte)((packed >> 8) & 0xFF),
              b: (byte)((packed >> 16) & 0xFF),
              alpha: (byte)((packed >> 24) & 0xFF)
            );

            info.Color = Color.Lerp(info.InitialColor.WithAlpha(0), otherColor.WithAlpha(0), 1f - (info.Time / (float)info.Duration));
        }

        public static void ChargeBehavior(ref ParticleInfo info)
        {
            int PlayerWhoAmI = (int)info.Data[2];
            int ProjWhoAmI = (int)info.Data[3];
            float StartDistance = info.Data[4];
            float EndDistance = info.Data[5];
            float Extension = info.Data[6];
            float StartRotationGap = info.Data[7];

            if (ProjWhoAmI < 0 || !Main.projectile[ProjWhoAmI].active)
            {
                info.Position += info.Velocity;
                info.Time--;
                return;
            }
            Projectile proj = Main.projectile[ProjWhoAmI];

            info.Data[1]++;

            float progress = info.Data[1] / 10;
            float modifiedProgress = MathF.Pow(progress, 3f);
            float ScaleX = info.InitialScale.X + Extension * progress;
            float ScaleY = info.InitialScale.X - info.InitialScale.X * progress;

            info.Scale = new SystemVector2(ScaleX, ScaleY) * 0.02f * 128f;

            if (PlayerWhoAmI >= 0 && Main.projectile[PlayerWhoAmI].active)
            {
                Player player = Main.player[PlayerWhoAmI];

                if (player.active && !player.dead)
                {
                    info.Rotation = proj.velocity.ToRotation() + StartRotationGap;
                    SystemVector2 setPosition = player.RotatedRelativePoint(player.MountedCenter, true).ToNumerics();
                    SystemVector2 movePos = info.Rotation.ToRotationVector2().ToNumerics() * MathHelper.Lerp(StartDistance, EndDistance, modifiedProgress);
                    info.Position = setPosition + movePos;
                }
            }
            else
            {
                info.Rotation = proj.velocity.ToRotation() + StartRotationGap;
                SystemVector2 setPosition = proj.Center.ToNumerics();
                SystemVector2 movePos = info.Rotation.ToRotationVector2().ToNumerics() * MathHelper.Lerp(StartDistance, EndDistance, modifiedProgress);
                info.Position = setPosition + movePos;
            }

            info.Position += info.Velocity;

            info.Time--;
        }

        public static void SpiritBehavior(ref ParticleInfo info)
        {
            var speedX = info.Data[3];
            var opacityScale = info.Data[6];

            float opacity = info.Time / (info.Duration / 2f);
            if (info.Time > info.Duration / 2f)
                opacity = MathHelper.Clamp(.05f * (info.Duration - info.Time), 0f, 1f);

            ColorTransitionBehaviour(ref info, 5);
            info.Color *= opacity * opacityScale;

            if (info.Data[1] <= 0)
            {
                float sineX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * speedX);

                // Makes the particle change directions or speeds.
                // Timer is used for keeping track of the current cycle
                if (info.Data[2] == 0)
                {
                    info.Data[2] = Main.rand.Next(50, 100);
                    info.Data[3] = Main.rand.NextFloat(4f, 9f);
                    info.Data[4] = Main.rand.NextFloat(10f, 31f) / 200f;
                }

                info.Velocity += new SystemVector2(sineX * info.Data[4], -Main.rand.NextFloat(1f, 2f) / 100f);

                // Clamp the velocity so the particle doesnt go too fast.
                Utils.Clamp(info.Velocity.X, -6f, 6f);
                Utils.Clamp(info.Velocity.Y, -6f, 6f);
                // Decrement the timer
                info.Data[2]--;
            }
            info.Data[1]--;

            info.Position += info.Velocity;

            info.Time--;
        }

        public static void RainbowBehavior(ref ParticleInfo info)
        {
            float OpacityScale = info.Data[1];
            float RandomRotation = info.Data[2];
            float ExtraRotation = info.Data[3];

            if (RandomRotation != -1)
                info.Rotation = RandomRotation.InRadians().AngleLerp((RandomRotation + 90f).InRadians(), (120f - info.Time) / 120f) + ExtraRotation;

            info.Velocity *= 0.98f;

            float Opacity = info.Time <= 20 ? 1f - 1f / 20f * (20 - info.Time) : 1f;
            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            if (info.InitialColor != Color.White)
                hsl = info.InitialColor;
            Color color2 = Color.Multiply(new(hsl.R, hsl.G, hsl.B, 0), Opacity);

            info.Color = color2 * Opacity * OpacityScale;

            info.Position += info.Velocity;
            info.Time--;
        }

        public static void SimpleStarBehavior(ref ParticleInfo info)
        {
            float Deceleration = info.Data[1];
            float RandomRotation = info.Data[2];
            float ExtraRotation = info.Data[3];

            info.Rotation = RandomRotation.InRadians().AngleLerp((RandomRotation + 90f).InRadians(), (120f - info.Time) / 120f) + ExtraRotation;
            info.Velocity *= Deceleration;

            float Opacity = info.Time <= 20 ? 1f - 1f / 20f * (20 - info.Time) : 1f;
            Color c = Color.Multiply(info.InitialColor, Opacity);
            info.Color = c * Opacity;

            info.Position += info.Velocity;
            info.Time--;
        }



        public static void DaggerSlashBehavior(ref ParticleInfo info)
        {
            int DustType = (int)info.Data[1];

            Vector2 dustPosition = new(info.Position.X, info.Position.Y);
            for (float num4 = 0f; num4 < 3f; num4 += 2f)
            {
                Vector2 vector2 = (MathHelper.PiOver4 + MathHelper.PiOver4 * num4).ToRotationVector2() * 4f * new Vector2(info.Scale.X / 142, info.Scale.Y / 42);
                Dust dust = Dust.NewDustPerfect(dustPosition, DustType, vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat(), newColor: info.InitialColor);
                dust.noGravity = true;
                dust = Dust.NewDustPerfect(dustPosition, DustType, -vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat(), newColor: info.InitialColor);
                dust.noGravity = true;
            }

            float Opacity = info.Time * (6 - info.Time) / 5;

            info.Color = info.InitialColor * Opacity;
            info.Position += info.Velocity;

            if (Opacity < 0)
                info.Time = 0;

            info.Time--;
        }

        public static void EmeraldCutterBehavior(ref ParticleInfo info)
        {
            int DustType = (int)info.Data[1];

            if (DustType != -1)
            {
                Vector2 dustPosition = new(info.Position.X, info.Position.Y);
                for (float num4 = 0f; num4 < 3f; num4++)
                {
                    Vector2 vector2 = (MathHelper.PiOver4 + MathHelper.PiOver4 * num4).ToRotationVector2() * 4f;
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustType, vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat(), newColor: info.InitialColor);
                    dust.noGravity = true;
                    Dust dust2 = Dust.NewDustPerfect(dustPosition, DustType, -vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat(), newColor: info.InitialColor);
                    dust2.noGravity = true;
                }
            }

            float Opacity = info.Time * (6 - info.Time) / 5;

            Color color = Color.Multiply(info.InitialColor, Opacity);
            info.Color = color * Opacity;
            info.Position += info.Velocity;

            if (Opacity < 0)
                info.Time = 0;

            info.Time--;
        }

        public static void SlashBehavior(ref ParticleInfo info)
        {
            SystemVector2 leftEnd = info.Position + info.Velocity;
            SystemVector2 rightEnd = info.Position - info.Velocity;

            float progress = 1 - info.Time / (float)info.Duration;
            float reverseOpacity = 1 - info.InitialColor.A / 255f;

            if (progress < 0.66f)
            {
                float scaleMulti = info.InitialScale.Y;
                RedeParticleManager.CreateAdditiveGlowParticle(SystemVector2.Lerp(leftEnd, rightEnd, Main.rand.NextFloat(0.4f, 0.6f)), info.Velocity * Main.rand.NextFloatDirection() * 0.025f, scaleMulti * 0.075f, info.InitialColor.WithAlpha(1) * reverseOpacity, info.Duration + 10);
            }

            float fadeOut = MathF.Min(1, 2 - 2 * progress);
            SystemVector2 scale = new SystemVector2(0.2f, 0.5f) * 1.3f * (info.InitialScale * new SystemVector2(72)) * fadeOut;

            info.Color = info.InitialColor * fadeOut;
            info.Scale = scale;

            info.Time--;
        }

        public static void DevilsPactBehavior(ref ParticleInfo info)
        {
            int DustType = (int)info.Data[1];

            if (DustType != -1)
            {
                Vector2 dustPosition = new(info.Position.X, info.Position.Y);
                for (float num4 = 0f; num4 < 3f; num4++)
                {
                    Vector2 vector2 = (MathHelper.PiOver4 + MathHelper.PiOver4 * num4).ToRotationVector2() * 4f;
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustType, vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat() * new Vector2(info.Scale.X / 72, info.Scale.Y / 72), newColor: Color.White);
                    dust.noGravity = true;
                    dust = Dust.NewDustPerfect(dustPosition, DustType, -vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat() * new Vector2(info.Scale.X / 72, info.Scale.Y / 72), newColor: Color.White);
                    dust.noGravity = true;
                }
            }

            float Opacity = 1f - 1f / 10f * (10 - info.Time);

            Color color = Color.Multiply(info.InitialColor, Opacity);
            info.Color = color * Opacity;
            info.Position += info.Velocity;

            if (Opacity < 0)
                info.Time = 0;

            info.Time--;
        }
    }
}