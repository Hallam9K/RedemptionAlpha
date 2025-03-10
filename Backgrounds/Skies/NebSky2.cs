using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Bosses.Neb;
using System;
using Terraria;
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
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTex = Request<Texture2D>("Redemption/Backgrounds/Skies/NebSky2").Value;
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
                    skyColor = Color.Cyan;
                    break;
                }
                else if (proj.type == ProjectileType<Neb_Meteor_Tele>())
                {
                    skyColor = Color.Orange;
                    beamActive = true;
                    MoonbeamIntensity += 0.05f;
                    break;
                }
                else if (proj.type == ProjectileType<Neb_Lightning_Tele>())
                {
                    skyColor = new(0, 242, 170);
                    beamActive = true;
                    MoonbeamIntensity += 0.01f;
                    break;
                }
            }
            if (!beamActive)
                MoonbeamIntensity -= 0.01f;
            MoonbeamIntensity = MathHelper.Clamp(MoonbeamIntensity, 0, 1);

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                Rotation -= .001f;
                if (!Main.dayTime)
                {
                    Vector2 SkyPos = new(Main.screenWidth / 2, Main.screenHeight / 2);
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