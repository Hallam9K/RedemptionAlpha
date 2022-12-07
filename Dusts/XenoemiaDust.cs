using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
    public class XenoemiaDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 14, 12, 14);
            dust.noGravity = true;
        }
        public struct Data
        {
            public float time;
        }
        public override bool MidUpdate(Dust dust)
        {
            dust.rotation = dust.velocity.X / 20f;

            if (dust.customData is not Data data)
            {
                data = default;
            }

            data.time += 0.3f / 60f;
            if (!dust.noLight)
                Lighting.AddLight(dust.position, 0.1f, 0.4f, 0.1f);
            dust.alpha = (int)MathHelper.Lerp(60f, 255f, Math.Abs(data.time - 1f));
            if (dust.alpha >= byte.MaxValue - 1)
                dust.active = false;

            dust.customData = data;
            return false;
        }
    }
}