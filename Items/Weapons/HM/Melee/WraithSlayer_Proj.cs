using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Redemption.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class WraithSlayer_Proj : TrueMeleeProjectile
    {
        public float[] oldrot = new float[8];
        public Vector2[] oldPos = new Vector2[8];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wraith Slayer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjArcane[Type] = true;
            ElementID.ProjShadow[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 94;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
        }
        private float startRotation;
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        private Player Player => Main.player[Projectile.owner];
        public float Timer;
        public int pauseTimer;
        public float progress;
        public float SwingSpeed;
        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            Projectile.Center = armCenter + vector;

            SwingSpeed = 1 / Player.GetAttackSpeed(DamageClass.Melee);
            progress = (Projectile.ai[0] == 0) ? Timer / (80 * SwingSpeed) : Timer / (100 * SwingSpeed);

            Projectile.spriteDirection = Player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4 - (Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver2);
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4 + (Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver2);
            if (Main.myPlayer == Projectile.owner && --pauseTimer <= 0)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 0)
                        {
                            Projectile.scale *= Projectile.ai[2];
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1, Player.position);
                            startRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (progress < 1f)
                        {
                            float modifiedProgress = 0.2f + 1 / (1 + MathF.Pow(3, -15 * (progress - 0.5f)));
                            float x = 35 * MathF.Cos(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.7f);
                            float y = 20 * MathF.Sin(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.7f);
                            Vector2 ellipse = new(x, y);
                            vector = ellipse.RotatedBy(startRotation) * 3f * Projectile.scale;
                        }
                        if (progress >= 1)
                        {
                            if (Main.MouseWorld.X < Player.Center.X)
                                Player.direction = -1;
                            else
                                Player.direction = 1;
                            Projectile.ai[0]++;
                            Projectile.velocity = armCenter.DirectionTo(Main.MouseWorld);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 0)
                        {
                            strike = false;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1, Player.position);
                            startRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (progress < 1f)
                        {
                            float modifiedProgress = -0.6f + 1 / (1 + MathF.Pow(3, -15 * (progress - 0.5f)));
                            float x = 20 * MathF.Cos((1 - modifiedProgress) * MathHelper.TwoPi * Projectile.spriteDirection * 0.5f);
                            float y = 30 * MathF.Sin((1 - modifiedProgress) * MathHelper.TwoPi * Projectile.spriteDirection * 0.5f);
                            Vector2 ellipse = new(x, y);
                            vector = ellipse.RotatedBy(startRotation) * 3.25f * Projectile.scale;
                        }
                        if (progress >= 1)
                            Projectile.Kill();
                        break;
                }
            }

            if (Timer >= 2)
                Projectile.alpha = 0;
            else
                Projectile.alpha = 255;

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = Projectile.rotation;
            oldPos[0] = Projectile.Center;

            if (Main.netMode != NetmodeID.Server && --pauseTimer <= 0)
            {
                TrailSetUp();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Spirit.Contains(target.type))
                modifiers.FinalDamage *= 2;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (!strike)
                target.immune[Player.heldProj] = 0;
            return !target.friendly && progress >= 0.2f && progress <= 0.8 && target.immune[Player.heldProj] == 0;
        }

        public bool strike;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!strike)
            {
                SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = -.2f }, Projectile.position);
                Player.RedemptionScreen().ScreenShakeIntensity += 4;
                strike = true;
                pauseTimer = 1;
            }
            target.immune[Player.heldProj] = 30;

            float Rot = Projectile.ai[0] == 0 ? 1 : -1;
            Vector2 dir = Projectile.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            ParticleSystem.NewParticle(drawPos, dir.RotatedBy(0.4f * Rot * Player.direction) * 80, new SlashParticleAlt(12, 1), Color.MediumPurple, 1f);
            for (int i = 0; i < 5; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.4f, 0.4f);
                float randomVel = Main.rand.NextFloat(2f, 3f);
                Vector2 direction = target.Center.DirectionFrom(Player.Center);
                Vector2 position = target.Center - direction * 30;
                ParticleSystem.NewParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, new SpeedParticle(), Color.MediumPurple, .8f);
            }

            if (target.life <= 0 && target.lifeMax >= 50 && (Main.rand.NextBool(6) || NPCLists.Spirit.Contains(target.type)) && NPC.CountNPCS(ModContent.NPCType<WraithSlayer_Samurai>()) < 4)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(target.position, target.width, target.height, DustID.Wraith);

                RedeHelper.SpawnNPC(Projectile.GetSource_FromThis(), (int)target.Center.X, (int)target.Center.Y, ModContent.NPCType<WraithSlayer_Samurai>(), ai3: Player.whoAmI);
            }
        }
        #region draw trail
        private Vector2[] oldDirVector = new Vector2[100];
        private List<Vector2> directionVectorCache = new();
        private List<Vector2> positionCache = new();
        private DanTrail trail;
        public Color baseColor = Color.MediumPurple;
        public Color endColor = Color.MediumPurple * .2f;
        private float thickness = 32f;
        private float opacity = 0;
        public void TrailSetUp()
        {
            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            if (Timer == 2)
            {
                for (int i = 0; i < oldDirVector.Length; i++)
                    oldDirVector.SetValue(vector, i);
            }

            float x = (progress - 0.5f) * 10;
            if (Projectile.ai[0] == 0)
            {
                if (progress < 0.3f)
                    opacity = 0;
                else
                    opacity = 1 / (1 + x * x);
            }
            else
            {
                if (progress > 0.75f)
                    opacity = 0;
                else
                    opacity = 1 / (1 + x * x);
            }

            for (int k = oldDirVector.Length - 1; k > 0; k--)
            {
                oldDirVector[k] = oldDirVector[k - 1];
            }
            oldDirVector[0] = vector;

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageSwordTrailPosition(oldDirVector.Length, armCenter, oldDirVector, ref directionVectorCache, ref positionCache, 1.25f);
                ManageTrail();
            }
        }
        public void ManageTrail()
        {
            trail ??= new DanTrail(Main.instance.GraphicsDevice, oldDirVector.Length, new NoTip(),
            factor =>
            {
                float mult = factor;
                float delay = 0;
                if (mult < 0.99f)
                    delay = 1;
                return thickness * MathF.Pow(mult, 2f) * Projectile.scale * delay;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress) * opacity;
            });
            trail.Positions = positionCache.ToArray();
            trail.NextPosition = Projectile.Center;
        }
        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Effect effect = ModContent.Request<Effect>("Redemption/Effects/GlowTrailShader", AssetRequestMode.ImmediateLoad).Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0);
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            bool flip = Player.direction == -1;
            Texture2D texture = ModContent.Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5").Value;
            if (flip)
                texture = ModContent.Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5_flipped2").Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(texture);
            effect.Parameters["time"].SetValue(1);
            effect.Parameters["repeats"].SetValue(-1);

            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects spriteEffects2 = Projectile.ai[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(15, (Projectile.Center - Player.Center).ToRotation());
            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects | spriteEffects2, 0);
            return false;
        }
    }
}
