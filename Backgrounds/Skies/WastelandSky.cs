using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Redemption.Backgrounds.Skies
{
    public class WastelandSky : CustomSky
    {
        public bool Active;
        public float Intensity;
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity += 0.02f;
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity -= 0.02f;
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }
        public override Color OnTileColor(Color inColor) => inColor;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/SkyTex2").Value;
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), new Color(160, 198, 163) * .5f * Intensity);
            }
        }
        public override float GetCloudAlpha() => 1f - Intensity;
        public override void Activate(Vector2 position, params object[] args) => Active = true;
        public override void Deactivate(params object[] args) => Active = false;
        public override void Reset() => Active = false;
        public override bool IsActive() => Active || Intensity > 0.001f;
    }
    public class WastelandSnowSky : CustomSky
    {
        public bool Active;
        public float Intensity;
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity += 0.02f;
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity -= 0.02f;
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }
        public override Color OnTileColor(Color inColor) => inColor;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/SkyTex2").Value;
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), new Color(158, 211, 169) * .5f * Intensity);
            }
        }
        public override float GetCloudAlpha() => 1f - Intensity;
        public override void Activate(Vector2 position, params object[] args) => Active = true;
        public override void Deactivate(params object[] args) => Active = false;
        public override void Reset() => Active = false;
        public override bool IsActive() => Active || Intensity > 0.001f;
    }
    public class WastelandCorruptSky : CustomSky
    {
        public bool Active;
        public float Intensity;
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity += 0.02f;
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity -= 0.02f;
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }
        public override Color OnTileColor(Color inColor) => inColor;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/WastelandCorruptSkyTex").Value;
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Intensity);
            }
        }
        public override float GetCloudAlpha() => 1f - Intensity;
        public override void Activate(Vector2 position, params object[] args) => Active = true;
        public override void Deactivate(params object[] args) => Active = false;
        public override void Reset() => Active = false;
        public override bool IsActive() => Active || Intensity > 0.001f;
    }
    public class WastelandCrimsonSky : CustomSky
    {
        public bool Active;
        public float Intensity;
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity += 0.02f;
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity -= 0.02f;
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }
        public override Color OnTileColor(Color inColor) => inColor;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/WastelandCrimsonSkyTex").Value;
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Intensity);
            }
        }
        public override float GetCloudAlpha() => 1f - Intensity;
        public override void Activate(Vector2 position, params object[] args) => Active = true;
        public override void Deactivate(params object[] args) => Active = false;
        public override void Reset() => Active = false;
        public override bool IsActive() => Active || Intensity > 0.001f;
    }
}