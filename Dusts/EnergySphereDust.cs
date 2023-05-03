using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
    public class EnergySphereDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 6, 4, 6);
        }
        public override bool MidUpdate(Dust dust)
        {
            dust.rotation += dust.velocity.X / 3f;
            dust.alpha += 4;
            if (!dust.noLight)
            {
                float strength = dust.scale * 1.4f;
                if (strength > 1f)
                    strength = 1f;
                Lighting.AddLight(dust.position, 0.3f * strength, 0.3f * strength, 0.5f * strength);
            }

            if (Collision.SolidCollision(dust.position + dust.velocity, 10, 10) && dust.fadeIn == 0f)
            {
                dust.scale *= 0.9f;
                dust.velocity *= 0.10f;
            }
            return false;
        }
        public override bool Update(Dust dust)
        {
            bool flag5 = WorldGen.SolidTile(Framing.GetTileSafely((int)dust.position.X / 16, (int)dust.position.Y / 16));
            if (flag5)
                dust.noLight = true;
            return true;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, lightColor.B, 25);
        }
    }
}