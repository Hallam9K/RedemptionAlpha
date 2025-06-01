using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Particles;
using Redemption.Textures;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class Twinklestar_TinyStar : ModProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteOrb";
        public override void SetStaticDefaults()
        {
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.alpha = 5;
            Projectile.scale = .4f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.rotation += 0.1f;
            Projectile.velocity *= 0.96f;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.ai[0] = Main.rand.NextFloat(0f, 360f);
                Projectile.scale = Main.rand.NextFloat(.3f, .5f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Projectile.Opacity = Projectile.timeLeft <= 20 ? 1f - 1f / 20f * (20 - Projectile.timeLeft) : 1f;
            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            Color color2 = Color.Multiply(new(hsl.R, hsl.G, hsl.B, 0), Projectile.Opacity);
            Main.spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, Projectile.Center - Main.screenPosition, null, color2, Projectile.ai[0].InRadians().AngleLerp((Projectile.ai[0] + 90f).InRadians(), (120f - Projectile.timeLeft) / 120f), new Vector2(71, 21), 0.75f * Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, Projectile.Center - Main.screenPosition, null, color2, Projectile.ai[0].InRadians().AngleLerp((Projectile.ai[0] + 90f).InRadians(), (120f - Projectile.timeLeft) / 120f) + MathHelper.PiOver2, new Vector2(71, 21), 0.75f * Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color2 * .5f, Projectile.ai[0].InRadians().AngleLerp((Projectile.ai[0] + 90f).InRadians(), (120f - Projectile.timeLeft) / 120f), new Vector2(114 / 2, 114 / 2), Projectile.scale + 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(CommonTextures.GlowParticle.Value, Projectile.Center - Main.screenPosition, null, color2, Projectile.rotation, new Vector2(64, 64), Projectile.scale * .3f, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class Twinklestar_ShootingStar : ModProjectile
    {
        public ref float Size => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[2];

        public float vectorOffset = 0f;

        public bool offsetLeft = false;

        public Vector2 originalVelocity = Vector2.Zero;

        public float Timer2;

        public float flag;
        public override string Texture => "Redemption/NPCs/Bosses/Neb/PNebula1";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }
        public override void AI()
        {
            Projectile.alpha += 2;
            Projectile.localAI[0]++;

            if (Main.rand.NextBool(3))
                RedeParticleManager.CreateRainbowParticle(Projectile.Center + Projectile.velocity, RedeHelper.Spread(1), Main.rand.NextFloat(.4f, .6f) * Projectile.Opacity, Projectile.Opacity, Main.rand.Next(20, 40));
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 2, 2, DustType<GlowDust>(), Scale: 2 * Projectile.Opacity)];
                dust.noGravity = true;
                dust.noLight = true;
                Color dustColor = new(RedeColor.NebColour.R, RedeColor.NebColour.G, RedeColor.NebColour.B) { A = 0 };
                dust.color = dustColor * .2f * Projectile.Opacity;
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                ManageTrail();
            }

            Timer++;
            if (Timer == 10 && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NebSound3 with { Pitch = .3f, Volume = .6f }, Projectile.position);

            Projectile.rotation += 0.1f;
            switch (Projectile.localAI[1])
            {
                case 0:
                    Projectile.scale = 0.01f;
                    Projectile.localAI[1] = 1;
                    break;
                case 1:
                    if (Projectile.alpha > 0)
                        Projectile.alpha -= 50;

                    Projectile.scale += 0.06f;
                    Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.01f, 0.5f);
                    break;
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || !(proj.type == ProjectileType<Twinklestar_TinyStar>()))
                    continue;

                if (proj.velocity.Length() > 10)
                    proj.friendly = true;
                else
                    proj.friendly = false;

                if (Helper.CheckCircularCollision(Projectile.Center, (int)Size * 6, proj.Hitbox) && Timer >= 10)
                {
                    proj.ai[0] = 1;
                    if (proj.ai[0] == 1)
                        proj.Move(Projectile.Center - 30f * Vector2.Normalize(Projectile.velocity), 200, 20);

                    if (proj.ai[0] == 1)
                    {
                        proj.ai[1] = 1;
                        proj.friendly = true;
                    }
                }
            }
            MakeDust(45, 0.25f);
            MakeDust(70, 0.5f);

            if (fakeTimer > 0)
                FakeKill();
        }
        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            for (int i = 0; i < 20; i++)
                RedeParticleManager.CreateGlowParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), 3 * Projectile.scale, new Color(117, 10, 47), Main.rand.Next(50, 60));
            for (int i = 0; i < 20; i++)
                RedeParticleManager.CreateGlowParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), 3 * Projectile.scale, new Color(94, 53, 104), Main.rand.Next(50, 60));
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<VoidFlame>(), Scale: 2);
                Main.dust[dust].velocity *= 2;
                Main.dust[dust].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(new Color(255, 255, 255, 0)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public void MakeDust(float pos, float spread)
        {
            Vector2 position = Projectile.Center + (Vector2.Normalize(Projectile.velocity) * pos);
            Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GiantCursedSkullBolt)];
            dust.position = position;
            dust.velocity = (Projectile.velocity.RotatedBy(1.57) * spread) + (Projectile.velocity / 4f);
            dust.position += Projectile.velocity.RotatedBy(1.57) * spread;
            dust.fadeIn = 0.5f;
            dust.noGravity = true;
            dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GiantCursedSkullBolt)];
            dust.position = position;
            dust.velocity = (Projectile.velocity.RotatedBy(-1.57) * spread) + (Projectile.velocity / 4f);
            dust.position += Projectile.velocity.RotatedBy(-1.57) * spread;
            dust.fadeIn = 0.5f;
            dust.noGravity = true;
        }

        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                for (int i = 0; i < 20; i++)
                    RedeParticleManager.CreateGlowParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), 3 * Projectile.scale, new Color(117, 10, 47), Main.rand.Next(50, 60));
                for (int i = 0; i < 20; i++)
                    RedeParticleManager.CreateGlowParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), 3 * Projectile.scale, new Color(94, 53, 104), Main.rand.Next(50, 60));
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<VoidFlame>(), Scale: 2);
                    Main.dust[dust].velocity *= 2;
                    Main.dust[dust].noGravity = true;
                }
            }
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer >= 60)
                Projectile.Kill();
        }
        private readonly int NUMPOINTS = 50;
        public Color baseColor = Color.Pink;
        public Color endColor = RedeColor.NebColour;
        public Color edgeColor = Color.Purple;
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 4f;
        public void ManageTrail()
        {
            trail ??= new DanTrail(Main.instance.GraphicsDevice, NUMPOINTS, new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                return thickness * 6 * mult * Projectile.Opacity;
            },
            factor =>
            {
                if (factor.X > 0.99f)
                    return Color.Transparent;

                return edgeColor * 0.1f * factor.X * Projectile.Opacity;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
            trail2 ??= new DanTrail(Main.instance.GraphicsDevice, NUMPOINTS, new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                return thickness * 3 * mult * Projectile.Opacity;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress) * Projectile.Opacity;
            });

            trail2.Positions = cache2.ToArray();
            trail2.NextPosition = Projectile.Center;
        }
    }
}
