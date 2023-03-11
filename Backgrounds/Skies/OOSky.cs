using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Redemption.Backgrounds.Skies
{
    public class OOSky : CustomSky
    {
        public bool Active;
        public float Intensity;
        public float ScaleY = 1.3f;
        public int GrooveTimer;
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity += 0.5f;
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity -= 0.02f;
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }

        public override Color OnTileColor(Color inColor)
        {
            return inColor;
        }
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/SkyTex").Value;
            if (GrooveTimer == 0)
            {
                ScaleY -= 0.015f;
                if (ScaleY <= 1f)
                {
                    ScaleY = 1;
                    GrooveTimer = 1;
                }
            }
            else if (GrooveTimer == 1)
            {
                ScaleY += 0.005f;
                if (ScaleY > 1.3f)
                    GrooveTimer = 2;
            }
            else if (GrooveTimer++ >= 19)
                GrooveTimer = 0;

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, (int)(Main.screenHeight * ScaleY)), Color.Red * Intensity);
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - Intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
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