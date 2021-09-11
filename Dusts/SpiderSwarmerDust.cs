using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
    public class SpiderSwarmerDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 16, 18);
        }

        public override bool MidUpdate(Dust dust)
        {
            dust.rotation = dust.velocity.X / 60f;

            if (Collision.SolidCollision(dust.position + dust.velocity, 10, 10) && dust.fadeIn == 0f)
            {
                dust.scale *= 0.9f;
                dust.velocity *= 0.10f;
            }
            return false;
        }
    }
}