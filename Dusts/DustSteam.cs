using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Dusts
{
	public class DustSteam : ModDust
    {
        public struct Data
        {
            public float Time;
        }
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle(0, 0, 32, 32);
            dust.scale = 0.01f;
            dust.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0f);
            dust.shader = GameShaders.Armor.GetShaderFromItemId(ItemID.LimeDye);
        }
        public override bool Update(Dust dust)
        {
            dust.rotation += 0.5f / 60f * (dust.velocity.X > 0f ? 1f : -1f);
            dust.position += dust.velocity;
            if (dust.customData is not Data data)
            {
                data = default;
            }
            data.Time += 0.75f / 60f;
            dust.alpha = (int)MathHelper.Lerp(160f, 255f, Math.Abs(data.Time - 1f));
            dust.scale += 0.8f / 60f;
            dust.velocity.Y -= 0.25f / 60f;
            if (dust.alpha >= byte.MaxValue - 1)
            {
                dust.active = false;
            }
            dust.customData = data;
            return false;
        }
    }
}