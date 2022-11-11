using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
	public class VoidFlame : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = true;
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X;
			dust.scale -= 0.1f;
			if (dust.scale < 0.5f)
				dust.active = false;
			return false;
		}
	}
}