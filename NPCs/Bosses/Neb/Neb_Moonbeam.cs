using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using ReLogic.Content;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class Neb_Moonbeam : LaserProjectile, IDrawAdditive
    {
        public override string Texture => "Redemption/NPCs/Bosses/Erhan/ScorchingRay";
        // >
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Ray of Guidance");
            ElementID.ProjArcane[Type] = true;
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }

        public override void SetSafeDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 380;
            Projectile.alpha = 255;
            LaserScale = 1;
            LaserSegmentLength = 120;
            LaserWidth = 78;
            LaserEndSegmentLength = 60;
            MaxLaserLength = 3800;
            StopsOnTiles = false;
        }

        public override bool CanHitPlayer(Player target) => AITimer >= 10;
        public override bool? CanHitNPC(NPC target) => target.friendly && AITimer >= 10 ? null : false;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.ManaSickness, 300);
        }
        private SlotId loop;
        private ActiveSound sound;
        private SlotId loop2;
        private ActiveSound sound2;
        public override void AI()
        {
            Projectile.rotation = MathHelper.PiOver2;
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(Projectile)];
            NPC neb = Main.npc[(int)Projectile.ai[0]];
            int speed = 8;
            if (neb.type == NPCType<Nebuleus2>() || neb.type == NPCType<Nebuleus2_Clone>())
                speed = 9;
            if (AITimer > 40)
                Projectile.MoveToVector2(player.Center + new Vector2(0, -1500), speed);
            Projectile.position.Y = MathHelper.Max(player.position.Y - 1500, 50);

            #region Beginning And End Effects
            if (AITimer == 15)
            {
                if (sound == null)
                    loop = SoundEngine.PlaySound(SoundID.Zombie104 with { Pitch = -1f }, new Vector2(Projectile.Center.X, player.Center.Y));
                if (sound2 == null && !Main.dedServ)
                    loop2 = SoundEngine.PlaySound(CustomSounds.NebBeam with { Pitch = -.5f }, new Vector2(Projectile.Center.X, player.Center.Y));
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.NebBeam, new Vector2(Projectile.Center.X, player.Center.Y));
            }
            if (AITimer >= 10)
            {
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = new Vector2(Projectile.Center.X, player.Center.Y);
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(40, Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity);
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
                if (Projectile.timeLeft > 30)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * MaxLaserLength - new Vector2(60, -40), 120, 40, DustType<GlowDust>(), 0, 0, Scale: 5);
                        Color dustColor = new(255, 255, 209) { A = 0 };
                        Main.dust[num5].velocity = -Projectile.velocity * Main.rand.NextFloat(1f, 2f);
                        Main.dust[num5].color = dustColor * Projectile.Opacity;
                        Main.dust[num5].noGravity = true;
                    }
                }
            }
            else
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
                LaserScale += .5f;
                LaserScale = MathHelper.Min(LaserScale, 6f);
            }
            if (Projectile.timeLeft < 30 || !neb.active || neb.ai[0] >= 7)
            {
                Projectile.hostile = false;
                if (sound != null)
                {
                    sound.Volume = MathHelper.Lerp(0, 2, Projectile.timeLeft / 30);
                    if (sound.Volume <= 0)
                    {
                        sound.Stop();
                        loop = SlotId.Invalid;
                    }
                }
                if (sound2 != null)
                {
                    sound2.Volume = MathHelper.Lerp(0, 2, Projectile.timeLeft / 30);
                    if (sound2.Volume <= 0)
                    {
                        sound2.Stop();
                        loop2 = SlotId.Invalid;
                    }
                }
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
                Projectile.alpha += 25;
            }
            #endregion

            LaserLength = MaxLaserLength;

            ++AITimer;
            SoundEngine.TryGetActiveSound(loop, out sound);
            if (sound != null)
                sound.Position = new Vector2(Projectile.Center.X, player.Center.Y);
            SoundEngine.TryGetActiveSound(loop2, out sound2);
            if (sound2 != null)
                sound2.Position = new Vector2(Projectile.Center.X, player.Center.Y);
        }
        public void AdditiveCall(SpriteBatch sB, Vector2 screenPos)
        {
            DrawTether(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * MaxLaserLength, screenPos, new Color(206, 255, 227, 0), new Color(103, 234, 200, 0), 800 * LaserScale, Projectile.Opacity);
        }
        public override void OnKill(int timeLeft)
        {
            if (sound != null)
            {
                sound.Stop();
                loop = SlotId.Invalid;
            }
            if (sound2 != null)
            {
                sound2.Stop();
                loop2 = SlotId.Invalid;
            }
        }
        public void DrawTether(Vector2 Target, Vector2 screenPos, Color color1, Color color2, float Size, float Strength)
        {
            Effect effect = Request<Effect>("Redemption/Effects/Beam2", AssetRequestMode.ImmediateLoad).Value;

            Texture2D TrailTex = Request<Texture2D>("Redemption/Textures/Trails/EnergyVertical", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TrailTex2 = Request<Texture2D>("Redemption/Textures/Trails/FlameVertical", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTexture"].SetValue(TrailTex);
            effect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly / 2);

            effect.Parameters["uColor"].SetValue(color1.ToVector4());
            effect.Parameters["uSecondaryColor"].SetValue(color2.ToVector4());

            effect.Parameters["uFadeHeight"].SetValue(Projectile.Opacity);
            effect.Parameters["TextureMod"].SetValue(1f);
            effect.Parameters["lerpCap"].SetValue(2f);
            effect.Parameters["strengthCap"].SetValue(2f);
            effect.Parameters["textureY"].SetValue(.003f);
            effect.Parameters["strengthScale"].SetValue(.001f);

            Effect effect2 = effect;
            effect.Parameters["uTexture"].SetValue(TrailTex2);
            effect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly / 2.5f);

            Vector2 dist = Target - Projectile.Center;
            Vector2 dist2 = Projectile.Center - Target;
            TrianglePrimitive tri = new()
            {
                TipPosition = Projectile.Center - screenPos,
                Rotation = dist.ToRotation(),
                Height = Size + 20 + dist.Length() * 1.5f,
                Color = Color.White * Strength,
                Width = (Size + Projectile.width) * Projectile.Opacity
            };
            PrimitiveRenderer.DrawPrimitiveShape(tri, effect);
            TrianglePrimitive tri2 = new()
            {
                TipPosition = Target - screenPos,
                Rotation = dist2.ToRotation(),
                Height = Size + 20 + dist2.Length() * 1.5f,
                Color = Color.White * Strength,
                Width = (Size + Projectile.width) * Projectile.Opacity
            };
            PrimitiveRenderer.DrawPrimitiveShape(tri2, effect2);
        }
        public override bool PreDraw(ref Color lightColor) => false;
        #region Collisions
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            float point = 0f;
            // Run an AABB versus Line check to look for collisions
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * LaserLength, 60 * LaserScale, ref point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
    public class Neb_Moonbeam_Tele : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
        }
        public float vectorOffset = 0f;
        public Vector2 originalVelocity = Vector2.Zero;

        private readonly int NUMPOINTS = 60;
        public Color baseColor = new(206, 255, 227, 0);
        public Color endColor = new(37, 202, 202, 0);
        public Color edgeColor = new(103, 234, 200, 0);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 9f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;

            if (Projectile.ai[0] != 0)
            {
                if (Projectile.ai[1] is 0)
                {
                    vectorOffset -= 0.2f;
                    if (vectorOffset <= -2f)
                    {
                        vectorOffset = -2f;
                        Projectile.ai[1] = 1;
                    }
                }
                else
                {
                    vectorOffset += 0.2f;
                    if (vectorOffset >= 2f)
                    {
                        vectorOffset = 2f;
                        Projectile.ai[1] = 0;
                    }
                }
                float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
                Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 2, 2, DustType<GlowDust>(), Scale: 3)];
                    dust.noGravity = true;
                    dust.noLight = true;
                    Color dustColor = new(206, 255, 227, 0) { A = 0 };
                    dust.color = dustColor * .2f * Projectile.Opacity;
                }
            }
            if (Projectile.ai[0] != 0 && Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                ManageTrail();
            }
            Projectile.alpha += 2;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
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
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] != 0)
            {
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
            return false;
        }
    }
}