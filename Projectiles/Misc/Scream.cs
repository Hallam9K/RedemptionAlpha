using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals.Projectiles;
using Redemption.Textures;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class Scream : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        private struct Gust
        {
            public Vector2 InitialPosition;
            public Vector2 Offset;
            public float Opacity;
            public int Delay;
            public int Timer;
        }

        private struct WindCircle
        {
            public float Opacity;
            public float Scale;
            public int Delay;
            public int Timer;
        }

        private static Dictionary<string, Asset<Texture2D>> _runsAssetsByPath;
        private static Asset<Texture2D> _screamAsset;
        private Gust[] _gustSmall;
        private Gust[] _gustBig;
        private WindCircle[] _windCircle;
        public Color effectColor = Color.White;

        public bool drawBehind = true;

        public ref float Duration => ref Projectile.ai[0];
        public ref float Radius => ref Projectile.ai[1];

        private bool Initialized
        {
            get => Projectile.localAI[0] != 0f;
            set => Projectile.localAI[0] = value.ToInt();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.hide = drawBehind;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void Load()
        {
            _screamAsset = Request<Texture2D>("Redemption/Textures/Shockwave2");
            _runsAssetsByPath = [];
        }

        public override void Unload()
        {
            _screamAsset = null;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 99999999;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Duration > 0f && Radius > 0f)
            {
                Projectile.timeLeft = 2;
            }

            Duration--;

            if (Duration <= 60f)
            {
                Projectile.Opacity = Duration / 60f;
            }

            InitializeEffects();
            UpdateEffects();
        }

        private void InitializeEffects()
        {
            if (Initialized)
            {
                return;
            }

            Initialized = true;

            if (Projectile.ShouldHandleNet())
            {
                Projectile.netUpdate = true;
            }

            if (Main.dedServ)
            {
                return;
            }

            _gustSmall = new Gust[100];
            for (int i = 0; i < _gustSmall.Length; i++)
            {
                float dist = 10f + Main.rand.NextFloat(Radius - 10f);
                _gustSmall[i].Offset = Main.rand.NextVector2Unit() * dist;
                _gustSmall[i].Delay = (i / 2) + 1;
                _gustSmall[i].Opacity = 1f;
                //velocity
            }

            _gustBig = new Gust[5];
            for (int i = 0; i < _gustBig.Length; i++)
            {
                float dist = Main.rand.NextFloat(Radius / 2f, Radius);
                _gustBig[i].Offset = Main.rand.NextVector2Unit() * dist;
                _gustBig[i].Delay = Main.rand.Next(10, 21);
                _gustBig[i].Opacity = 1f;
            }

            _windCircle = new WindCircle[3];
            for (int i = 0; i < _windCircle.Length; i++)
            {
                _windCircle[i].Delay = (i + 1) * 10;
            }
        }

        private void UpdateEffects()
        {
            for (int i = 0; i < _gustSmall?.Length; i++)
            {
                if (_gustSmall[i].Delay > 0)
                {
                    _gustSmall[i].Delay--;
                    continue;
                }

                _gustSmall[i].Offset *= 1.05f;
                _gustSmall[i].Opacity *= 0.92f;

                if (_gustSmall[i].Opacity <= 0.025f)
                {
                    if (Duration < 60)
                    {
                        _gustSmall[i].Opacity = 0f;
                        continue;
                    }

                    float dist = 10f + Main.rand.NextFloat(Radius - 10f);
                    _gustSmall[i].Offset = Main.rand.NextVector2Unit() * dist;
                    _gustSmall[i].Opacity = 1f;
                }
            }

            for (int i = 0; i < _gustBig?.Length; i++)
            {
                if (_gustBig[i].Delay > 0)
                {
                    _gustBig[i].Delay--;
                    continue;
                }

                if (_gustBig[i].Offset.LengthSquared() <= Radius * Radius)
                {
                    _gustBig[i].Offset *= 1.05f;
                }

                _gustBig[i].Opacity *= 0.91f;

                if (_gustBig[i].Opacity <= 0.025f)
                {
                    if (Duration < 90)
                    {
                        _gustBig[i].Opacity = 0f;
                        continue;
                    }

                    float dist = Main.rand.NextFloat(Radius / 2f, Radius);
                    _gustBig[i].Offset = Main.rand.NextVector2Unit() * dist;
                    _gustBig[i].Delay = Main.rand.Next(10, 21);
                    _gustBig[i].Opacity = 1f;
                }
            }

            for (int i = 0; i < _windCircle?.Length; i++)
            {
                if (_windCircle[i].Delay > 0)
                {
                    _windCircle[i].Delay--;
                    continue;
                }

                float mod = MathHelper.Lerp(0f, 1f, _windCircle[i].Timer / 30f);

                _windCircle[i].Scale = mod * (Radius / 300) * 2;

                if (_windCircle[i].Timer <= 5)
                {
                    float mod2 = MathHelper.Lerp(0f, 1f, _windCircle[i].Timer / 5f);

                    _windCircle[i].Opacity = mod2 * 0.2f;
                }
                else if (_windCircle[i].Timer >= 5)
                {
                    float mod2 = MathHelper.Lerp(1f, 0f, (_windCircle[i].Timer - 5f) / 25f);

                    _windCircle[i].Opacity = mod2 * 0.2f;
                }

                if (_windCircle[i].Timer < 30)
                {
                    _windCircle[i].Timer++;
                }
                else if (Duration >= 30)
                {
                    _windCircle[i].Timer = 0;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(drawBehind);
            writer.WriteRGB(effectColor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            drawBehind = reader.ReadBoolean();
            effectColor = reader.ReadRGB();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Initialized)
            {
                return false;
            }

            Texture2D screamTexture = _screamAsset.Value;
            Texture2D shineTexture = CommonTextures.Shine.Value;
            Vector2 drawPosition = Projectile.DrawPosition();
            Color baseDrawColor = Projectile.GetAlpha(effectColor) with { A = 0 };
            Vector2 shineOrigin = shineTexture.Size() * 0.5f;
            Vector2 screamOrigin = screamTexture.Size() * 0.5f;
            Vector2 gustSmallScale = new Vector2(0.2f, 2f) * Projectile.scale;
            Vector2 gustBigScale = new Vector2(0.5f, 15f) * Projectile.scale;

            for (int i = 0; i < _gustSmall?.Length; i++)
            {
                if (_gustSmall[i].Delay > 0)
                {
                    continue;
                }

                Main.EntitySpriteDraw(shineTexture, drawPosition + _gustSmall[i].Offset, null, baseDrawColor * _gustSmall[i].Opacity, _gustSmall[i].Offset.ToRotation() + MathHelper.PiOver2, shineOrigin, gustSmallScale, SpriteEffects.None);
            }

            for (int i = 0; i < _gustBig?.Length; i++)
            {
                if (_gustBig[i].Delay > 0)
                {
                    continue;
                }

                Main.EntitySpriteDraw(shineTexture, drawPosition + _gustBig[i].Offset, null, baseDrawColor * _gustBig[i].Opacity, _gustBig[i].Offset.ToRotation() + MathHelper.PiOver2, shineOrigin, gustBigScale, SpriteEffects.None);
            }

            for (int i = 0; i < _windCircle?.Length; i++)
            {
                if (_windCircle[i].Delay > 0)
                {
                    continue;
                }

                Main.EntitySpriteDraw(screamTexture, drawPosition, null, baseDrawColor * _windCircle[i].Opacity, 0f, screamOrigin, _windCircle[i].Scale * Projectile.scale, SpriteEffects.None);
            }
            return false;
        }
    }
}