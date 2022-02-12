using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Utilities;
using Terraria.ModLoader;

namespace Redemption.Backgrounds.Skies
{
    public class RuinedKingdomSky : CustomSky
    {
        public bool Active;
        public float Intensity;
        private struct LightPillar
        {
            public Vector2 Position;

            public float Depth;
        }

        private LightPillar[] _pillars;

        private readonly UnifiedRandom _random = new();

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
            Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/RuinedKingdomSky").Value;
            Texture2D BeamTexture = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/RuinedKingdomBeam").Value;

            if (maxDepth >= 3E+38f && minDepth < 3E+38f)
            {
                spriteBatch.Draw(SkyTex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            }
            int num = -1;
            int num2 = 0;
            for (int i = 0; i < _pillars.Length; i++)
            {
                float depth = _pillars[i].Depth;
                if (num == -1 && depth < maxDepth)
                {
                    num = i;
                }
                if (depth <= minDepth)
                {
                    break;
                }
                num2 = i;
            }
            if (num == -1)
            {
                return;
            }
            Vector2 value5 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle rectangle2 = new(-1000, -1000, 4000, 4000);
            float scale3 = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
            for (int j = num; j < num2; j++)
            {
                Vector2 value4 = new(1f / _pillars[j].Depth, 0.9f / _pillars[j].Depth);
                Vector2 vector = _pillars[j].Position;
                vector = (vector - value5) * value4 + value5 - Main.screenPosition;
                if (rectangle2.Contains((int)vector.X, (int)vector.Y))
                {
                    float num3 = value4.X * 450f;
                    spriteBatch.Draw(BeamTexture, vector, null, Color.White * 0.2f * scale3 * Intensity, 0f, Vector2.Zero, new Vector2(num3 / 70f, num3 / 45f), SpriteEffects.None, 0f);
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
            _pillars = new LightPillar[40];
            for (int i = 0; i < _pillars.Length; i++)
            {
                _pillars[i].Position.X = i / (float)_pillars.Length * (Main.maxTilesX * 16f + 20000f) + _random.NextFloat() * 40f - 20f - 20000f;
                _pillars[i].Position.Y = _random.NextFloat() * 200f - 2000f;
                _pillars[i].Depth = _random.NextFloat() * 8f + 7f;
            }
            Array.Sort(_pillars, new Comparison<LightPillar>(SortMethod));
        }

        private int SortMethod(LightPillar pillar1, LightPillar pillar2)
        {
            return pillar2.Depth.CompareTo(pillar1.Depth);
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