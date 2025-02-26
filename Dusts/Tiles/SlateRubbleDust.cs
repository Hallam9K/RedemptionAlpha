using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts.Tiles
{
    public class SlateRubbleDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(4) * 10, 10, 10);
            dust.fadeIn = 140;
        }
        public override bool MidUpdate(Dust dust)
        {
            dust.rotation = dust.velocity.X / 60f;
            if (Collision.SolidCollision(dust.position, 6, 6))
            {
                dust.fadeIn = 0;
                dust.scale *= 0.9f;
                dust.velocity *= 0.10f;
            }
            return false;
        }
    }
}