using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
    public class SoullessScreenDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = false;
            dust.noGravity = true;

            dust.alpha = 100;
            dust.fadeIn = 1f;
            dust.scale = Main.rand.NextFloat(0.25f, 0.5f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 14, 10, 14);
            dust.velocity *= 0;

            dust.color = new Color(255, 255, 255);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            bool tile = WorldGen.SolidTile(Framing.GetTileSafely((int)dust.position.X / 16, (int)dust.position.Y / 16));
            if (tile)
            {
                dust.noLight = true;
            }
            else
            {
                dust.noLight = false;
            }
            if (!dust.noLight)
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.3f);
            }
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.1f;

            dust.velocity.X += (float)Math.Sin(Main.GameUpdateCount / 20) * 0.02f;
            dust.velocity.X = MathHelper.Clamp(dust.velocity.X, -1f, 1f);

            dust.velocity.Y -= 0.025f;
            if (dust.velocity.Y < 1f)
            {
                dust.velocity.Y = -1f;
            }
            if (dust.fadeIn > dust.scale)
            {
                dust.scale += 0.0015f;
            }
            else
            {
                dust.fadeIn = 0;

                if ((dust.scale -= 0.005f) < 0.1f)
                {
                    dust.active = false;
                }
            }
            return false;
        }
    }
}