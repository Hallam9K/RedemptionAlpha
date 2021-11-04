using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
    public class XenoWaterDust : ModDust
	{
		public struct Data
		{
			public float time;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = true;
			dust.scale = 0.01f;
            dust.frame = new Rectangle(0, 0, 24, 24);
            dust.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0f);
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData is not Data data)
			{
				data = default;
			}

			data.time += 0.75f / 60f;

			dust.alpha = (int)MathHelper.Lerp(160f, 255f, Math.Abs(data.time - 1f));
			dust.scale += 0.5f / 60f;
			dust.velocity.Y -= 0.25f / 60f;
			dust.rotation += 0.5f / 60f * (dust.velocity.X > 0f ? 1f : -1f);
			dust.position += dust.velocity;

			if (dust.alpha >= byte.MaxValue - 1)
			{
				dust.active = false;
			}

			dust.customData = data;

			return false;
		}
	}
}