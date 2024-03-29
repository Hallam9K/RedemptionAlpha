using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
    public class ChickenFeatherDust1 : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 14, 12, 14);
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
    public class ChickenFeatherDust2 : ChickenFeatherDust1 { }
    public class ChickenFeatherDust3 : ChickenFeatherDust1 { }
    public class ChickenFeatherDust4 : ChickenFeatherDust1 { }
    public class ChickenFeatherDust5 : ChickenFeatherDust1 { }
    public class ChickenFeatherDust6 : ChickenFeatherDust1 { }
    public class ChickenFeatherDust7 : ChickenFeatherDust1 { }
}