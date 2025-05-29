using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Bosses.Neb;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;

namespace Redemption.Backgrounds.Skies
{
    public class NebSky2 : CustomSky
    {
        public bool Active;
        public float Intensity;
        public float MoonbeamIntensity;
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
        private Color skyColor;
        public float Rotation = 0;
        float uTime, speed;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = GetInstance<RedeConfigClient>().LegacyNebSky ? Request<Texture2D>("Redemption/Backgrounds/Skies/NebSky2").Value : TextureAssets.MagicPixel.Value;
            Texture2D SkyTex3 = Request<Texture2D>("Redemption/Backgrounds/Skies/SkyTex3").Value;

            bool beamActive = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active)
                    continue;

                if (proj.type == ProjectileType<Neb_Moonbeam>() && proj.localAI[0] >= 20)
                {
                    beamActive = true;
                    MoonbeamIntensity = proj.Opacity;
                    speed = MathHelper.Lerp(speed, 50, 0.1f);
                    skyColor = Color.Cyan;
                    break;
                }
                else if (proj.type == ProjectileType<Neb_Meteor_Tele>())
                {
                    skyColor = Color.Orange;
                    beamActive = true;
                    speed = MathHelper.Lerp(speed, 10, 0.1f);
                    MoonbeamIntensity += 0.05f;
                    break;
                }
                else if (proj.type == ProjectileType<Neb_Lightning_Tele>())
                {
                    skyColor = new(0, 242, 170);
                    beamActive = true;
                    speed = MathHelper.Lerp(speed, 20, 0.1f);
                    MoonbeamIntensity += 0.01f;
                    break;
                }
            }
            if (!beamActive)
            {
                speed = MathHelper.Lerp(speed, 1, 0.03f);
                MoonbeamIntensity -= 0.01f;
            }
            MoonbeamIntensity = MathHelper.Clamp(MoonbeamIntensity, 0, 1);

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                Rotation -= .001f;
                if (!Main.dayTime)
                {
                    if (!GetInstance<RedeConfigClient>().LegacyNebSky)
                    {

                        uTime += speed * 0.01f;
                        DrawHelper.SpriteBatchState s = spriteBatch.SaveState();
                        Texture2D starMap = Request<Texture2D>("Redemption/Textures/Noise/starNoise").Value;
                        Texture2D seamlessNoise = Request<Texture2D>("Redemption/Textures/Noise/seamlessNoise").Value;
                        Texture2D waterNoise = Request<Texture2D>("Redemption/Textures/Noise/waterNoise").Value;
                        Texture2D maskano = Request<Texture2D>("Redemption/Textures/Noise/smearNoise").Value;

                        spriteBatch.End();
                        spriteBatch.Begin(s.SpriteSortMode, s.BlendState, Redemption.nebSkyEffect, s);

                        float alpha = 1 + MoonbeamIntensity;
                        Redemption.nebSkyEffect.Parameters["skyColor1"].SetValue(new Vector4(1, 0.15f, 1.5f, Intensity) * alpha);
                        Redemption.nebSkyEffect.Parameters["skyColor2"].SetValue(new Vector4(1, 0.5f, 1.5f, Intensity) * alpha);
                        Redemption.nebSkyEffect.Parameters["bgColor1"].SetValue(new Vector4(.1f, 0, 1.7f, Intensity) * 0.8f * alpha);
                        Redemption.nebSkyEffect.Parameters["bgColor2"].SetValue(new Vector4(2, 0.15f, 1.5f, Intensity) * 0.5f * alpha);
                        Redemption.nebSkyEffect.Parameters["gamma"].SetValue(1.5f);
                        Redemption.nebSkyEffect.Parameters["starScale"].SetValue(2.5f);
                        Redemption.nebSkyEffect.Parameters["gradientScale"].SetValue(2.5f);
                        Redemption.nebSkyEffect.Parameters["gradientMax"].SetValue(2f);
                        Redemption.nebSkyEffect.Parameters["gradientMin"].SetValue(0f);
                        Redemption.nebSkyEffect.Parameters["uTime"].SetValue(uTime);
                        Redemption.nebSkyEffect.Parameters["_stars"].SetValue(starMap);
                        Redemption.nebSkyEffect.Parameters["_displace"].SetValue(seamlessNoise);
                        Redemption.nebSkyEffect.Parameters["_mask"].SetValue(waterNoise);
                        Redemption.nebSkyEffect.Parameters["_maskano"].SetValue(maskano);

                        int size = Main.screenHeight > Main.screenWidth ? Main.screenHeight : Main.screenWidth;
                        spriteBatch.Draw(SkyTex, new Rectangle(0, 0, size, size), Color.White);

                        spriteBatch.End();
                        spriteBatch.Begin(s);
                    }

                    Vector2 SkyPos = new(Main.screenWidth / 2, Main.screenHeight / 2);
                    if (GetInstance<RedeConfigClient>().LegacyNebSky)
                        spriteBatch.Draw(SkyTex, SkyPos, null, Color.White * .9f, Rotation, new Vector2(SkyTex.Width >> 1, SkyTex.Height >> 1), 2f, SpriteEffects.None, 1f);

                    if (MoonbeamIntensity > 0f)
                    {
                        float flicker = 1;
                        if (skyColor == Color.Cyan)
                            flicker = Main.rand.NextFloat(.9f, 1.1f);
                        spriteBatch.Draw(SkyTex3, SkyPos, new Rectangle(0, 0, Main.screenWidth, SkyTex3.Height), skyColor with { A = 0 } * MoonbeamIntensity * flicker, 0f, new Vector2(1920 >> 1, 1200 >> 1), 1f, SpriteEffects.None, 1f);
                        if (skyColor == Color.Cyan)
                            spriteBatch.Draw(SkyTex3, SkyPos, new Rectangle(0, 0, Main.screenWidth, SkyTex3.Height), Color.White with { A = 0 } * MoonbeamIntensity * flicker, 0f, new Vector2(1920 >> 1, 1200 >> 1), 1f, SpriteEffects.None, 1f);
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
            Intensity = 0.006f;
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