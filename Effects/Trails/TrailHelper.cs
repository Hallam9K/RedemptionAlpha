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
    }
}

