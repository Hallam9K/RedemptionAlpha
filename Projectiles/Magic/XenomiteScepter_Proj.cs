using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class XenomiteScepter_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Helix Bolt");
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ElementID.ProjArcane[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;

        }
        public float vectorOffset = 0f;
        public Vector2 originalVelocity;

        public override void AI()
        {
            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;
            if (Projectile.ai[0] == 0)
            {
                vectorOffset -= 0.4f;
                if (vectorOffset <= -1.3f)
                {
                    vectorOffset = -1.3f;
                    Projectile.ai[0] = 1;
                }
            }
            else
            {
                vectorOffset += 0.4f;
                if (vectorOffset >= 1.3f)
                {
                    vectorOffset = 1.3f;
                    Projectile.ai[0] = 0;
                }
            }
            float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
            Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                ManageTrail();
            }
            if (fakeTimer > 0 || Projectile.timeLeft < 2)
                FakeKill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 300);
            else if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 150);
            FakeKill();
        }

        private ref float fakeTimer => ref Projectile.ai[1];
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                int pieCut = 20;
                for (int m = 0; m < pieCut; m++)
                {
                    RedeParticleManager.CreateGlowParticle(Projectile.Center, BaseUtility.RotateVector(default, new Vector2(2f, 0f), m / (float)pieCut * 6.28f), 0.6f, Color.Green, Main.rand.Next(50, 60));
                    RedeParticleManager.CreateSpeedParticle(Projectile.Center, BaseUtility.RotateVector(default, new Vector2(15f, 0f), m / (float)pieCut * 6.28f), 1f, Color.Green.WithAlpha(0));
                }
                for (int m = 0; m < pieCut; m++)
                {
                    RedeParticleManager.CreateGlowParticle(Projectile.Center, BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)pieCut * 6.28f), 0.6f, Color.Green, Main.rand.Next(50, 60));
                }
                SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
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
        public Color baseColor = Color.Green;
        public Color endColor = RedeColor.GreenPulse;
        public Color edgeColor = Color.DarkGreen;
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

                return thickness * 6 * mult;
            },
            factor =>
            {
                if (factor.X > 0.99f)
                    return Color.Transparent;

                return edgeColor * 0.1f * factor.X;
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

                return thickness * 3 * mult;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress);
            });

            trail2.Positions = cache2.ToArray();
            trail2.NextPosition = Projectile.Center;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Vector2 drawOrigin = new(ModContent.Request<Texture2D>("Redemption/Textures/WhiteOrb").Value.Width / 2, ModContent.Request<Texture2D>("Redemption/Textures/WhiteOrb").Value.Height / 2);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Redemption/Textures/WhiteOrb").Value, Projectile.Center - (Projectile.velocity / 20) - Main.screenPosition, null, Color.Green, Projectile.rotation, drawOrigin, 1 * Projectile.Opacity, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            return true;
        }
    }
}
