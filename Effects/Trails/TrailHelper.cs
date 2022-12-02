using Microsoft.Xna.Framework;
using System;

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

    }
}

