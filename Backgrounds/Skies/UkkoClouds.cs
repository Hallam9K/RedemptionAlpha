using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Utilities;
using Terraria.ModLoader;

namespace Redemption.Backgrounds.Skies
{
    public class UkkoClouds : CustomSky
    {
        private readonly UnifiedRandom random = new();

        private struct Bolt
        {
            public Vector2 Position;

            public float Depth;

            public int Life;

            public bool IsAlive;
        }

        public bool Active;
        public float Intensity;
        private Bolt[] bolts;
        public int ticksUntilNextBolt;
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
                Intensity += 0.002f;
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity -= 0.002f;
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }

            if (ticksUntilNextBolt <= 0)
            {
                ticksUntilNextBolt = random.Next(5, 20);
                int num = 0;
                while (bolts[num].IsAlive && num != bolts.Length - 1)
                {
                    num++;
                }
                bolts[num].IsAlive = true;
                bolts[num].Position.X = random.NextFloat() * (Main.maxTilesX * 16f + 4000f) - 2000f;
                bolts[num].Position.Y = random.NextFloat() * 500f;
                bolts[num].Depth = random.NextFloat() * 8f + 2f;
                bolts[num].Life = 30;
            }
            ticksUntilNextBolt--;
            for (int i = 0; i < bolts.Length; i++)
            {
                if (bolts[i].IsAlive)
                {
                    Bolt[] expr168cp0 = bolts;
                    int expr168cp1 = i;
                    expr168cp0[expr168cp1].Life = expr168cp0[expr168cp1].Life - 1;
                    if (bolts[i].Life <= 0)
                    {
                        bolts[i].IsAlive = false;
                    }
                }
            }
        }

        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.5f));
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D CloudTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoClouds").Value;
            Texture2D boltTexture = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoSkyBolt").Value;
            Texture2D flashTexture = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoSkyFlash").Value;
            Texture2D BeamTexture = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoSkyBeam").Value;

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                Vector2 SkyPos = new(Main.screenWidth / 2, Main.screenHeight / 2);
                spriteBatch.Draw(CloudTex, SkyPos, null, new Color(200, 200, 200) * Intensity, 0f, new Vector2(CloudTex.Width >> 1, CloudTex.Height >> 1), 1f, SpriteEffects.None, 1f);
                Color white2 = Color.White;
                float num65 = 1f - Main.cloudAlpha * 1.5f;
                if (num65 < 0f)
                    num65 = 0f;

                white2.R = (byte)(white2.R * num65);
                white2.G = (byte)(white2.G * num65);
                white2.B = (byte)(white2.B * num65);
                white2.A = (byte)(white2.A * num65);
            }
            float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
            Vector2 value3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle rectangle = new(-1000, -1000, 4000, 4000);
            for (int i = 0; i < bolts.Length; i++)
            {
                if (bolts[i].IsAlive && bolts[i].Depth > minDepth && bolts[i].Depth < maxDepth)
                {
                    Vector2 value4 = new(1f / bolts[i].Depth, 0.9f / bolts[i].Depth);
                    Vector2 position = (bolts[i].Position - value3) * value4 + value3 - Main.screenPosition;
                    if (rectangle.Contains((int)position.X, (int)position.Y))
                    {
                        Texture2D texture = boltTexture;
                        int life = bolts[i].Life;
                        if (life > 26 && life % 2 == 0)
                        {
                            texture = flashTexture;
                        }
                        float scale2 = life / 30f;
                        spriteBatch.Draw(texture, position, null, Color.White * scale * scale2 * Intensity, 0f, Vector2.Zero, value4.X * 5f, SpriteEffects.None, 0f);
                    }
                }
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
            if (!Active)
            {
                Active = true;
                bolts = new Bolt[500];
                for (int i = 0; i < bolts.Length; i++)
                {
                    bolts[i].IsAlive = false;
                }

                _pillars = new LightPillar[40];
                for (int i = 0; i < _pillars.Length; i++)
                {
                    _pillars[i].Position.X = i / (float)_pillars.Length * (Main.maxTilesX * 16f + 20000f) + _random.NextFloat() * 40f - 20f - 20000f;
                    _pillars[i].Position.Y = _random.NextFloat() * 200f - 2000f;
                    _pillars[i].Depth = _random.NextFloat() * 8f + 7f;
                }
                Array.Sort(_pillars, new Comparison<LightPillar>(SortMethod));
            }
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