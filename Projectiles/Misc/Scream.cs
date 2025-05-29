using Microsoft.Xna.Framework.Graphics;
using Redemption.Textures;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class Scream : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_873";
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 999999;
            Projectile.hide = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] == 0)
                return false;

            Asset<Texture2D> shineText = TextureAssets.Projectile[Type];
            Asset<Texture2D> shockwave = CommonTextures.Shockwave2;
            Color baseDrawColor = Projectile.GetAlpha(effectColor);
            baseDrawColor.A = 0;

            if (_gustSmall.Length > 0)
            {
                for (int i = 0; i < _gustSmall.Length; i++)
                {
                    if (_gustSmall[i].delay > 0)
                    {
                        continue;
                    }

                    Main.EntitySpriteDraw(shineText.Value, Projectile.Center + _gustSmall[i].Position - Main.screenPosition, null, baseDrawColor * _gustSmall[i].Alpha, _gustSmall[i].Position.ToRotation() + MathHelper.ToRadians(90), shineText.Size() / 2f, new Vector2(0.2f, 2f), SpriteEffects.None, 0);
                }
            }

            if (_gustBig.Length > 0)
            {
                for (int i = 0; i < _gustBig.Length; i++)
                {
                    if (_gustBig[i].delay > 0)
                    {
                        continue;
                    }

                    Main.EntitySpriteDraw(shineText.Value, Projectile.Center + _gustBig[i].Position - Main.screenPosition, null, baseDrawColor * _gustBig[i].Alpha, _gustBig[i].Position.ToRotation() + MathHelper.ToRadians(90), shineText.Size() / 2f, new Vector2(0.5f, 15f), SpriteEffects.None, 0);
                }
            }

            if (_windCircle.Length > 0)
            {
                for (int i = 0; i < _windCircle.Length; i++)
                {
                    if (_windCircle[i].delay > 0)
                    {
                        continue;
                    }

                    Main.EntitySpriteDraw(shockwave.Value, Projectile.Center - Main.screenPosition, null, baseDrawColor * _windCircle[i].Alpha, 0f, shockwave.Size() / 2f, _windCircle[i].scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }

        private struct GustSmall
        {
            public Vector2 Position;

            public Vector2 initPos;

            public float Alpha;

            public int delay;

            public int timer;
        }

        private GustSmall[] _gustSmall;

        private struct GustBig
        {
            public Vector2 Position;

            public Vector2 initPos;

            public float Alpha;

            public int delay;

            public int timer;
        }

        private GustBig[] _gustBig;

        private struct WindCircle
        {
            public float Alpha;

            public float scale;

            public int delay;

            public int timer;
        }

        private WindCircle[] _windCircle;

        public ref float Duration => ref Projectile.ai[0];
        public ref float Radius => ref Projectile.ai[1];

        public Color effectColor = Color.White;

        public int boundToNPC = -1;

        public int boundToProjectile = -1;

        public override void AI()
        {
            if (Duration <= 0 || Radius <= 0)
            {
                Projectile.Kill();
                return;
            }

            if (Duration > 0)
            {
                Duration--;
            }

            if (Duration <= 60)
            {
                float timer = 60 - Duration;

                Projectile.alpha = (int)MathHelper.Lerp(0, 255, timer / 60);
            }
            if (Projectile.localAI[0] == 0)
            {
                _gustSmall = new GustSmall[100];
                int delay = 0;
                for (int i = 0; i < _gustSmall.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        delay++;
                    }

                    _gustSmall[i].delay = delay;

                    _gustSmall[i].Alpha = 1f;

                    float dist = 10f + Main.rand.NextFloat(Radius - 10f);

                    Vector2 newPos = new Vector2(0f, dist).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360f)));

                    _gustSmall[i].Position = newPos;

                    //velocity
                }

                _gustBig = new GustBig[5];
                for (int i = 0; i < _gustBig.Length; i++)
                {
                    _gustBig[i].delay = Main.rand.Next(10, 21);

                    _gustBig[i].Alpha = 1f;

                    float dist = Radius / 2f + Main.rand.NextFloat(Radius / 2f);

                    Vector2 newPos = new Vector2(0f, dist).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360f)));

                    _gustBig[i].Position = newPos;
                }

                _windCircle = new WindCircle[3];
                delay = 0;
                for (int i = 0; i < _windCircle.Length; i++)
                {
                    delay += 10;

                    _windCircle[i].delay = delay;
                }

                Projectile.localAI[0] = 1;
            }

            if (_gustSmall.Length > 0)
            {
                for (int i = 0; i < _gustSmall.Length; i++)
                {
                    if (_gustSmall[i].delay > 0)
                    {
                        _gustSmall[i].delay--;
                        continue;
                    }

                    _gustSmall[i].Position *= 1.05f;
                    _gustSmall[i].Alpha *= 0.92f;

                    if (_gustSmall[i].Alpha <= 0.025f)
                    {
                        if (Duration < 60)
                        {
                            _gustSmall[i].Alpha = 0f;
                            continue;
                        }

                        _gustSmall[i].Alpha = 1f;

                        float dist = 10f + Main.rand.NextFloat(Radius - 10f);

                        Vector2 newPos = new Vector2(0f, dist).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360f)));

                        _gustSmall[i].Position = newPos;
                    }
                }
            }

            if (_gustBig.Length > 0)
            {
                for (int i = 0; i < _gustBig.Length; i++)
                {
                    if (_gustBig[i].delay > 0)
                    {
                        _gustBig[i].delay--;
                        continue;
                    }

                    if (_gustBig[i].Position.Length() <= Radius)
                    {
                        _gustBig[i].Position *= 1.05f;
                    }

                    _gustBig[i].Alpha *= 0.91f;

                    if (_gustBig[i].Alpha <= 0.025f)
                    {
                        if (Duration < 90)
                        {
                            _gustBig[i].Alpha = 0f;
                            continue;
                        }

                        _gustBig[i].delay = Main.rand.Next(10, 21);

                        _gustBig[i].Alpha = 1f;

                        float dist = Radius / 2f + Main.rand.NextFloat(Radius / 2f);

                        Vector2 newPos = new Vector2(0f, dist).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360f)));

                        _gustBig[i].Position = newPos;
                    }
                }
            }

            if (_windCircle.Length > 0)
            {
                for (int i = 0; i < _windCircle.Length; i++)
                {
                    if (_windCircle[i].delay > 0)
                    {
                        _windCircle[i].delay--;
                        continue;
                    }

                    float mod = MathHelper.Lerp(0f, 1f, _windCircle[i].timer / 30f);

                    _windCircle[i].scale = mod * (Radius / 300) * 2;

                    if (_windCircle[i].timer <= 5)
                    {
                        float mod2 = MathHelper.Lerp(0f, 1f, _windCircle[i].timer / 5f);

                        _windCircle[i].Alpha = mod2 * 0.2f;
                    }
                    else if (_windCircle[i].timer >= 5)
                    {
                        float mod2 = MathHelper.Lerp(1f, 0f, (_windCircle[i].timer - 5f) / 25f);

                        _windCircle[i].Alpha = mod2 * 0.2f;
                    }

                    if (_windCircle[i].timer < 30)
                    {
                        _windCircle[i].timer++;
                    }
                    else if (Duration >= 30)
                    {
                        _windCircle[i].timer = 0;
                    }
                }
            }
        }
    }
}