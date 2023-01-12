using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Redemption.Backgrounds.Skies
{
    public class NebSky : CustomSky
    {
        public bool Active;
        public float Intensity;
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }

        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.5f));
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/NebSky").Value;

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                if (!Main.dayTime)
                {
                    if (Main.screenWidth > SkyTex.Width || Main.screenHeight > SkyTex.Height)
                    {
                        Rectangle rect = new(0, 0, Main.screenWidth, Main.screenHeight);
                        spriteBatch.Draw(SkyTex, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        Vector2 SkyPos = new(Main.screenWidth / 2, Main.screenHeight / 2);
                        spriteBatch.Draw(SkyTex, SkyPos, null, Color.White, 0f, new Vector2(SkyTex.Width >> 1, SkyTex.Height >> 1), 1f, SpriteEffects.None, 1f);
                    }
                }
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - Intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            Intensity = 0.002f;
            Active = true;
        }

        public override void Deactivate(params object[] args)
        {
            Active = false;
        }

        public override void Reset()
        {
            Active = false;
        }

        public override bool IsActive()
        {
            return Active || Intensity > 0.001f;
        }
    }
}