using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.WorldGeneration.Space;
using SubworldLibrary;

namespace Redemption.Backgrounds.Skies
{
    public class SpaceSky : CustomSky
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
        private float sunAlpha = 0;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (SubworldSystem.IsActive<SpaceSub>())
            {
                Main.sunModY = 300;
                Main.moonModY = 300;
                if (maxDepth >= 3E+38f && minDepth < 3E+38f)
                {
                    if (!Main.dayTime)
                        sunAlpha = 0;
                    if (Main.dayTime)
                    {
                        if (sunAlpha < 1f)
                            sunAlpha += 0.05f;
                        else if (sunAlpha > 1f)
                            sunAlpha = 1f;

                        float cloudAlpha = 1f;
                        cloudAlpha -= Main.cloudAlpha * 1.5f;
                        if (cloudAlpha < 0f)
                            cloudAlpha = 0f;

                        int x = (int)(Main.time / 54000.0 * (Main.screenWidth + TextureAssets.Sun.Value.Width * 2)) - TextureAssets.Sun.Value.Width;
                        float rotation = (float)(Main.time / 54000.0) * 2f - 7.3f;
                        double bgTop = (-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0;
                        double y2;
                        int y;
                        if (Main.time < 27000.0)
                        {
                            y2 = Math.Pow(1.0 - Main.time / 54000.0 * 2.0, 2.0);
                            y = (int)(bgTop + y2 * 250.0 + 180.0);
                        }
                        else
                        {
                            y2 = Math.Pow((Main.time / 54000.0 - 0.5) * 2.0, 2.0);
                            y = (int)(bgTop + y2 * 250.0 + 180.0);
                        }
                        float scale = (float)(1.2 - y2 * 0.4);
                        Color color = new((byte)(255f * cloudAlpha), (byte)(Color.White.G * cloudAlpha), (byte)(Color.White.B * cloudAlpha), (byte)(255f * cloudAlpha));

                        Texture2D sunTexture = ModContent.Request<Texture2D>("Redemption/Textures/Sun3").Value;
                        spriteBatch.Draw(sunTexture, new Vector2(x, y + Main.sunModY), new Rectangle?(new Rectangle(0, 0, sunTexture.Width, sunTexture.Height)), color * sunAlpha, rotation, new Vector2(sunTexture.Width / 2, sunTexture.Height / 2), scale + 0.5f, SpriteEffects.None, 0f);
                        Texture2D sun2Texture = ModContent.Request<Texture2D>("Redemption/Textures/Sun2").Value;
                        spriteBatch.Draw(sun2Texture, new Vector2(x, y + Main.sunModY) + new Vector2(60, 10), new Rectangle?(new Rectangle(0, 0, sunTexture.Width, sunTexture.Height)), color * sunAlpha, rotation, new Vector2(sunTexture.Width / 2, sunTexture.Height / 2), scale - 0.25f, SpriteEffects.None, 0f);
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