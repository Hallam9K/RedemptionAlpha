using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
    public class XenoemiaDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            if (dust.customData is not Data data)
            {
                data = default;
            }
            dust.rotation = Main.rand.NextFloat(0, MathHelper.PiOver4);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 14, 12, 14);
            dust.noGravity = true;
            data.time = RedeHelper.Spread(.4f);
            dust.customData = data;
        }
        public struct Data
        {
            public Vector2 time;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * ((255 - dust.alpha) / 255f);
        }
        public override bool MidUpdate(Dust dust)
        {
            dust.rotation = dust.velocity.X / 100f;
            if (dust.customData is not Data data)
            {
                data = default;
            }
            dust.velocity = data.time;
            dust.scale = 1;
            if (!dust.noLight)
                Lighting.AddLight(dust.position, 0.1f, 0.4f, 0.1f);
            dust.alpha++;
            if (dust.alpha >= byte.MaxValue - 1)
                dust.active = false;

            dust.customData = data;
            return false;
        }
    }
}