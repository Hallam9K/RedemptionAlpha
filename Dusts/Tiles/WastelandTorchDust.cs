using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts.Tiles
{
	public class WastelandTorchDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.4f;
			dust.noGravity = false;
			dust.noLight = false;
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.99f;

			Lighting.AddLight(dust.position, 0.35f * dust.scale, 0.3f * dust.scale, 0.3f * dust.scale);

			if (dust.scale < 0.5f)
				dust.active = false;

			return false;
		}
	}
}