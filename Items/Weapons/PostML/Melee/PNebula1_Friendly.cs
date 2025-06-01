using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Textures;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PNebula1_Friendly : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/PNebula1";
        public int proType = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Piercing Nebula");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
            ProjectileLists.ProjSpear[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
        }
        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;

        private readonly int NUMPOINTS = 50;
        public Color baseColor = Color.Pink;
        public Color endColor = RedeColor.NebColour;
        public Color edgeColor = Color.Purple;
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 4f;
        public override void OnSpawn(IEntitySource source)
        {
            if (proType == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ProjectileType<PNebula2_Friendly>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ProjectileType<PNebula3_Friendly>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
            }
        }
        public override void AI()
        {
            Projectile.alpha += 2;
            Projectile.localAI[0]++;

            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;
            if (proType != 0)
            {
                if (offsetLeft)
                {
                    vectorOffset -= 0.1f;
                    if (vectorOffset <= -1f)
                    {
                        vectorOffset = -1f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset += 0.1f;
                    if (vectorOffset >= 1f)
                    {
                        vectorOffset = 1f;
                        offsetLeft = true;
                    }
                }
                float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
                Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));
            }
            else
            {
                if (Main.rand.NextBool(3))
                    RedeParticleManager.CreateRainbowParticle(Projectile.Center + Projectile.velocity, RedeHelper.Spread(1), Main.rand.NextFloat(.4f, .6f) * Projectile.Opacity, Projectile.Opacity * .5f, Main.rand.Next(20, 40));
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 2, 2, DustType<GlowDust>(), Scale: 2 * Projectile.Opacity)];
                    dust.noGravity = true;
                    dust.noLight = true;
                    Color dustColor = new(RedeColor.NebColour.R, RedeColor.NebColour.G, RedeColor.NebColour.B) { A = 0 };
                    dust.color = dustColor * .2f * Projectile.Opacity;
                }
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (proType != 0 && Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                ManageTrail();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;

            if (proType != 0)
                return;
            Main.player[Projectile.owner].RedemptionScreen().ScreenShakeIntensity += 2;
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            RedeDraw.SpawnExplosion(Projectile.Center, Main.DiscoColor * 0.6f, 6, 0, 30, 2, 1 * Projectile.Opacity, true, "Redemption/NPCs/Bosses/Neb/GiantStar_Proj", RedeHelper.RandomRotation());
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.1f * Projectile.timeLeft / 120;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (proType == 0)
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
            else
            {
                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive();

                Vector2 drawOrigin = new(CommonTextures.WhiteOrb.Width() / 2, CommonTextures.WhiteOrb.Height() / 2);
                Main.EntitySpriteDraw(CommonTextures.WhiteOrb.Value, Projectile.Center - (Projectile.velocity / 20) - Main.screenPosition, null, Color.Pink, Projectile.rotation, drawOrigin, 1 * Projectile.Opacity, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();

                Main.spriteBatch.End();
                Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.ZoomMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("Redemption/Textures/Trails/Trail_4").Value);
                effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
                effect.Parameters["repeats"].SetValue(1f);

                trail?.Render(effect);
                trail2?.Render(effect);

                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
                return true;
            }
        }
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
    public class PNebula2_Friendly : PNebula1_Friendly
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 1;
            offsetLeft = false;
        }
    }
    public class PNebula3_Friendly : PNebula1_Friendly
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 2;
            offsetLeft = true;
        }
    }
}