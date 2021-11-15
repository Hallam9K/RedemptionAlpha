using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts.Tiles
{
    public class LabPlatingDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            dust.noGravity = false;

            dust.velocity /= 2f;
            dust.scale = 1f;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X;

            dust.scale -= 0.05f;
            if (dust.scale < 0.2f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}