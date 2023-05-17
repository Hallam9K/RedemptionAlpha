using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
    public class SpaceBgStyle : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }
        public override int ChooseFarTexture()
        {
            return -1;
        }
        public override int ChooseMiddleTexture()
        {
            return -1;
        }
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SpaceBG1");
        }
        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            float centerDist = 0;
            if (!Main.dayTime)
            {
                if (Main.time < 16200)
                    centerDist = MathHelper.Distance((float)Main.time, 0) * 6;
                else
                    centerDist = MathHelper.Distance((float)Main.time, 32400) * 6;
                centerDist /= 16200;
            }

            float c = 450f;
            float d = 100f;
            float bgParallax2 = 0.37f + 0.2f - 0.1f;
            int textureSlot2 = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/EpidotraPlanet");
            int textureSlot2_Brighter = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/EpidotraPlanet_Brighter");
            Main.instance.LoadBackground(textureSlot2);
            Main.instance.LoadBackground(textureSlot2_Brighter);
            float bgScale2 = 2f;
            float screenOff2 = typeof(Main).GetFieldValue<float>("screenOff", Main.instance);
            int bgW2 = (int)(Main.backgroundWidth[textureSlot2] * bgScale2);
            SkyManager.Instance.DrawToDepth(spriteBatch, 1f / bgParallax2);
            int bgStart2 = (int)(-Math.IEEERemainder(Main.screenPosition.X / 40 * bgParallax2, bgW2) - (bgW2 * 1.3f));
            int bgTop2 = (int)((-Main.screenPosition.Y + screenOff2 / 2f) / (Main.worldSurface * 16.0) * c + d);
            if (Main.gameMenu)
            {
                bgTop2 = 320;
            }
            Color backColor2 = typeof(Main).GetFieldValue<Color>("ColorOfSurfaceBackgroundsModified", Main.instance);
            if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
            {
                spriteBatch.Draw(TextureAssets.Background[textureSlot2].Value,
                    new Vector2(bgStart2 + bgW2, bgTop2),
                    new Rectangle(0, 0, Main.backgroundWidth[textureSlot2], Main.backgroundHeight[textureSlot2]),
                    backColor2 * (centerDist + 1), 0f, default, bgScale2, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureAssets.Background[textureSlot2_Brighter].Value,
                    new Vector2(bgStart2 + bgW2, bgTop2),
                    new Rectangle(0, 0, Main.backgroundWidth[textureSlot2], Main.backgroundHeight[textureSlot2]),
                    backColor2 * (centerDist + 1) * 1.1f, 0f, default, bgScale2, SpriteEffects.None, 0f);
            }

            float a = 4000f;
            float b = 3000f;
            int[] textureSlots = new int[] {
                BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SpaceBG1"),
            };
            int length = textureSlots.Length;
            for (int i = 0; i < textureSlots.Length; i++)
            {
                float bgParallax = 0.37f + 0.2f - (0.1f * (length - i));
                int textureSlot = textureSlots[i];
                Main.instance.LoadBackground(textureSlot);
                float bgScale = 2f;
                int bgW = (int)(Main.backgroundWidth[textureSlot] * bgScale);
                SkyManager.Instance.DrawToDepth(spriteBatch, 1f / bgParallax);
                float screenOff = typeof(Main).GetFieldValue<float>("screenOff", Main.instance);
                float scAdj = typeof(Main).GetFieldValue<float>("scAdj", Main.instance);
                int bgStart = (int)(-Math.IEEERemainder(Main.screenPosition.X * bgParallax, bgW) - (bgW / 2));
                int bgTop = (int)((-Main.screenPosition.Y + screenOff / 2f) / (Main.worldSurface * 16.0) * a + b) + (int)scAdj - (length);
                if (Main.gameMenu)
                {
                    bgTop = 320;
                }
                Color backColor = Color.White;
                int bgLoops = Main.screenWidth / bgW + 2;
                if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {
                    for (int k = 0; k < bgLoops; k++)
                    {
                        spriteBatch.Draw(TextureAssets.Background[textureSlot].Value,
                            new Vector2(bgStart + bgW * k, MathHelper.Clamp(bgTop, -600, 3000)),
                            new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
                            backColor, 0f, default, bgScale, SpriteEffects.None, 0f);
                    }
                }
            }
            return false;
        }
    }
}