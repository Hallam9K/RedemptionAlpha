using Microsoft.Xna.Framework;
using Redemption.Globals;
using System;
using System.Collections.Generic;
using Terraria;

namespace Redemption.Effects
{
    public static class TrailHelper
    {
        public static Vector3 Vec3(this Vector2 vector) => new Vector3(vector.X, vector.Y, 0);

        public static T[] FastUnion<T>(this T[] front, T[] back)
        {
            T[] combined = new T[front.Length + back.Length];

            Array.Copy(front, combined, front.Length);
            Array.Copy(back, 0, combined, front.Length, back.Length);

            return combined;
        }
        public static void ManageBasicCaches(ref List<Vector2> cache, ref List<Vector2> cache2, int numPoints, Vector2 pos)
        {
            if (cache == null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < numPoints; i++)
                {
                    cache.Add(pos);
                }
            }
            cache.Add(pos);
            while (cache.Count > numPoints)
            {
                cache.RemoveAt(0);
            }
            cache2 = new List<Vector2>();
            for (int i = 0; i < cache.Count; i++)
            {
                Vector2 point = cache[i];
                Vector2 nextPoint = i == cache.Count - 1 ? pos : cache[i + 1];
                Vector2 dir = Vector2.Normalize(nextPoint - point);
                if (i > cache.Count - 3 || dir == Vector2.Zero)
                    cache2.Add(point);
                else
                    cache2.Add(point + (dir * Main.rand.NextFloat(5)));
            }
        }

        public static void ManageBasicTrail(ref List<Vector2> cache, ref List<Vector2> cache2, ref DanTrail trail, ref DanTrail trail2, int numPoints, Vector2 pos, Color baseColor, Color endColor, Color edgeColor, float thickness, bool roundTip = true)
        {
            trail ??= new DanTrail(Main.instance.GraphicsDevice, numPoints, roundTip ? new RoundedTip(4) : new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                return thickness * 6 * mult;
            },
            factor =>
            {
                if (factor.X > 0.99f)
                    return Color.Transparent;

                return edgeColor * 0.1f * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = pos;
            trail2 ??= new DanTrail(Main.instance.GraphicsDevice, numPoints, roundTip ? new RoundedTip(4) : new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                return thickness * 3 * mult;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress);
            });

            trail2.Positions = cache2.ToArray();
            trail2.NextPosition = pos;
        }
        public static void ManageSwordTrailPosition(Projectile Projectile, Player Player, float[] oldrot, ref List<float> RotationCache, ref List<float> RotationCache2, ref List<float> RotationCacheLate, ref List<Vector2> positionCache, ref List<Vector2> positionCacheLate, float drawPos = 15f, int dirType = 0)
        {
            //storing rotation info
            float dir = 0;
            switch (dirType)
            {
                case 0:
                    dir = Player.direction == 1 ? MathHelper.PiOver4 : 3 * MathHelper.PiOver4;
                    break;
                case 1:
                    dir = Player.direction == -1 ? MathHelper.PiOver4 : 3 * MathHelper.PiOver4;
                    break;
            }

            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                RotationCache.Add(oldrot[i] + dir);
            }
            while (RotationCache.Count > Projectile.oldPos.Length)
            {
                RotationCache.RemoveAt(0);
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                RotationCache2.Add(i == 0 ? RotationCache[i] : MathHelper.Lerp(RotationCache[i - 1], RotationCache[i], i / Projectile.oldPos.Length));
            }
            while (RotationCache2.Count > Projectile.oldPos.Length)
            {
                RotationCache2.RemoveAt(0);
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                RotationCacheLate.Add(i == 0 || i == 1 ? RotationCache[i] : MathHelper.Lerp(RotationCache[i - 2], RotationCache[i - 1], i / Projectile.oldPos.Length));
            }
            while (RotationCacheLate.Count > Projectile.oldPos.Length)
            {
                RotationCacheLate.RemoveAt(0);
            }

            //storing drawing position info
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                positionCache.Add(Player.Center - RotationCache2[i].ToRotationVector2() * drawPos);
            }
            while (positionCache.Count > Projectile.oldPos.Length)
            {
                positionCache.RemoveAt(0);
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                positionCacheLate.Add(Player.Center - RotationCacheLate[i].ToRotationVector2() * drawPos);
            }
            while (positionCacheLate.Count > Projectile.oldPos.Length)
            {
                positionCacheLate.RemoveAt(0);
            }
        }
        public static void ManageSwordTrailPosition(Projectile Projectile, Vector2 SpawnPoint, float[] oldrot, ref List<float> RotationCache, ref List<float> RotationCache2, ref List<float> RotationCacheLate, ref List<Vector2> positionCache, ref List<Vector2> positionCacheLate, float drawPos = 15f, int dirType = 0)
        {
            //storing rotation info
            float dir = 0;
            switch (dirType)
            {
                case 0:
                    dir = 0;
                    break;
                case 1:
                    dir = MathHelper.Pi;
                    break;
            }

            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                RotationCache.Add(oldrot[i] + dir);
            }
            while (RotationCache.Count > Projectile.oldPos.Length)
            {
                RotationCache.RemoveAt(0);
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                RotationCache2.Add(i == 0 ? RotationCache[i] : MathHelper.Lerp(RotationCache[i - 1], RotationCache[i], i / Projectile.oldPos.Length));
            }
            while (RotationCache2.Count > Projectile.oldPos.Length)
            {
                RotationCache2.RemoveAt(0);
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                RotationCacheLate.Add(i == 0 || i == 1 ? RotationCache[i] : MathHelper.Lerp(RotationCache[i - 2], RotationCache[i - 1], i / Projectile.oldPos.Length));
            }
            while (RotationCacheLate.Count > Projectile.oldPos.Length)
            {
                RotationCacheLate.RemoveAt(0);
            }

            //storing drawing position info
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                positionCache.Add(SpawnPoint - RotationCache2[i].ToRotationVector2() * drawPos);
            }
            while (positionCache.Count > Projectile.oldPos.Length)
            {
                positionCache.RemoveAt(0);
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                positionCacheLate.Add(SpawnPoint - RotationCacheLate[i].ToRotationVector2() * drawPos);
            }
            while (positionCacheLate.Count > Projectile.oldPos.Length)
            {
                positionCacheLate.RemoveAt(0);
            }
        }
        public static void ManageSwordTrailPosition(int TrailLength, Vector2 drawCenter, Vector2[] oldDirectionVector, ref List<Vector2> directionVectorCache, ref List<Vector2> positionCache, float extension = 1)
        {
            for (int i = TrailLength - 1; i >= 0; i--)
            {
                directionVectorCache.Add(oldDirectionVector[i] * extension);
            }
            while (directionVectorCache.Count > TrailLength)
            {
                directionVectorCache.RemoveAt(0);
            }

            for (int i = 0; i < TrailLength; i++)
            {
                positionCache.Add(drawCenter + directionVectorCache[i]);
            }
            while (positionCache.Count > TrailLength)
            {
                positionCache.RemoveAt(0);
            }
        }
    }
}