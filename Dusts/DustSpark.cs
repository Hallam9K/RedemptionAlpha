using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Redemption.Dusts
{
    public class DustSpark : ModDust
    {
        public override string Texture => "Redemption/Dusts/DustSpark";
        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;
        public override void OnSpawn(Dust dust)
        {
            dust.fadeIn = 0;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 5, 50);

            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Redemption.Instance.Assets.Request<Effect>("Effects/ShrinkingDust").Value), "ShrinkingDustPass");
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= new Vector2(2.5f, 25).RotatedBy(dust.rotation) * dust.scale;
                dust.customData = 1;
            }

            dust.frame.Y++;
            dust.frame.Height--;

            dust.rotation = dust.velocity.ToRotation() + 1.57f;
            dust.position += dust.velocity;

            dust.color.G -= 8;
            dust.color.A -= 5;

            dust.velocity.X *= 0.98f;
            dust.velocity.Y *= 0.95f;

            dust.velocity.Y += 0.15f;

            float mult = 1;

            if (dust.fadeIn < 5)
                mult = dust.fadeIn / 5f;

            dust.shader.UseSecondaryColor(new Color((int)(255 * (1 - dust.fadeIn / 20f)), 0, 0) * mult);
            dust.shader.UseColor(dust.color * mult);
            dust.fadeIn++;

            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.02f);

            if (dust.fadeIn > 30)
                dust.active = false;
            return false;
        }
    }
    public class DustSpark2 : DustSpark
    {
        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= new Vector2(2.5f, 25).RotatedBy(dust.rotation) * dust.scale;
                dust.customData = 1;
            }

            //dust.frame.Y++;
            //dust.frame.Height--;

            dust.rotation = dust.velocity.ToRotation() + 1.57f;
            dust.position += dust.velocity;

            //dust.color.G -= 8;
            dust.color.A -= 5;

            dust.velocity.X *= 0.98f;
            dust.velocity.Y *= 0.98f;

            float mult = 1;

            if (dust.fadeIn < 5)
                mult = dust.fadeIn / 5f;

            dust.shader.UseSecondaryColor(new Color((int)(255 * (1 - dust.fadeIn / 20f)), 0, 0) * mult);
            dust.shader.UseColor(dust.color * mult);
            dust.fadeIn++;

            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.02f);

            if (dust.fadeIn > 30)
                dust.active = false;
            return false;
        }
    }
}