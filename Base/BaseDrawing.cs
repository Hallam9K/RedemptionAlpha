using Microsoft.Xna.Framework;

using Terraria;

namespace Redemption
{
    public class BaseDrawing
    {
        /*
         * Convenience method for getting lighting color using an npc or projectile position.
         */
        public static Color GetLightColor(Vector2 position)
        {
            return Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));
        }

        /*
         * Convenience method for adding lighting using an npc or projectile position, using a Color instance for color.
         */
        public static void AddLight(Vector2 position, Color color, float brightnessDivider = 1F)
        {
            AddLight(position, color.R / 255F, color.G / 255F, color.B / 255F, brightnessDivider);
        }
        /*
         * Convenience method for adding lighting using an npc or projectile position with 0F - 1F color values.
         */
        public static void AddLight(Vector2 position, float colorR, float colorG, float colorB, float brightnessDivider = 1F)
        {
            Lighting.AddLight((int)(position.X / 16f), (int)(position.Y / 16f), colorR / brightnessDivider, colorG / brightnessDivider, colorB / brightnessDivider);
        }
    }
}