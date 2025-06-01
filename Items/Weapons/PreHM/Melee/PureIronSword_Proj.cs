using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles.Melee;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class PureIronSword_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/PureIronSword";

        public float[] oldrot = new float[60];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Sword");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjIce[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        private Vector2 vector;
        private float startRotation;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        public float progress;
        private float SwingSpeed;
        private float glow;
        private bool parried;
        private Player Player => Main.player[Projectile.owner];
        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);

            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            Projectile.Center = armCenter + vector;

            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + 3 * MathHelper.PiOver4;

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        Projectile.scale *= Projectile.ai[2];
                        Projectile.ai[0] = 2;
                        Projectile.netUpdate = true;
                        break;
                    case 1: //down
                        BlockProj();
                        progress = Timer / (20 * (Projectile.extraUpdates + 1) * SwingSpeed);
                        if (Timer++ == 0)
                        {
                            Projectile.spriteDirection = Player.direction;
                            startRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (progress < 1)
                        {
                            float modifiedProgress = 0.05f + 1 / (1 + MathF.Pow(3, -15 * (progress - 0.5f)));
                            float x = 30 * MathF.Cos(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.9f);
                            float y = 20 * MathF.Sin(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.9f);
                            Vector2 ellipse = new(x, y);
                            vector = ellipse.RotatedBy(startRotation) * 2.25f * Projectile.scale;
                        }
                        if (Timer == (int)(10 * (Projectile.extraUpdates + 1) * SwingSpeed))
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        }
                        if (progress >= 1)
                        {
                            if (!Player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (glow >= 0.8f)
                            {
                                RedeDraw.SpawnRing(Player.Center, Color.LightCyan, 0.2f, 0.85f, 4);
                                RedeDraw.SpawnRing(Player.Center, Color.LightCyan, 0.2f);
                                SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center, Vector2.Zero, ProjectileType<ArcticWind_Proj>(), 0, 0, Projectile.owner);
                                Projectile.ai[0] = 3;
                            }
                            else
                            {
                                if (Main.MouseWorld.X < Player.Center.X)
                                    Player.direction = -1;
                                else
                                    Player.direction = 1;
                                Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - Player.Center).ToRotation());
                                Projectile.alpha = 255;
                                Projectile.ai[0]++;
                            }
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2: //up
                        BlockProj();
                        Projectile.spriteDirection = Player.direction;
                        progress = Timer / (20 * (Projectile.extraUpdates + 1) * SwingSpeed);
                        if (Timer++ == 0)
                        {
                            Projectile.spriteDirection = Player.direction;
                            startRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (progress < 1)
                        {
                            float modifiedProgress = -0.05f + 1 / (1 + MathF.Pow(3, -15 * (progress - 0.5f)));
                            float x = 30 * MathF.Cos((1 - modifiedProgress) * MathHelper.TwoPi * Projectile.spriteDirection * 0.9f);
                            float y = 20 * MathF.Sin((1 - modifiedProgress) * MathHelper.TwoPi * Projectile.spriteDirection * 0.9f);
                            Vector2 ellipse = new(x, y);
                            vector = ellipse.RotatedBy(startRotation) * 2.25f * Projectile.scale;
                        }
                        if (Timer == (int)(10 * (Projectile.extraUpdates + 1) * SwingSpeed))
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        }
                        if (progress >= 1)
                        {
                            if (!Player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (glow >= 0.8f)
                            {
                                RedeDraw.SpawnRing(Player.Center, Color.LightCyan, 0.2f, 0.85f, 4);
                                RedeDraw.SpawnRing(Player.Center, Color.LightCyan, 0.2f);
                                SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center, Vector2.Zero, ProjectileType<ArcticWind_Proj>(), 0, 0, Projectile.owner);
                                Projectile.ai[0] = 3;
                            }
                            else
                            {
                                if (Main.MouseWorld.X < Player.Center.X)
                                    Player.direction = -1;
                                else
                                    Player.direction = 1;
                                Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - Player.Center).ToRotation());
                                Projectile.alpha = 255;
                                Projectile.ai[0] = 1;
                            }
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 3:
                        BlockProj();
                        progress = Timer / (40 * (Projectile.extraUpdates + 1) * SwingSpeed);
                        thickness = 40;
                        if (Timer++ == 0)
                        {
                            Projectile.penetrate += 12;
                            Projectile.damage = Projectile.originalDamage;
                        }
                        if (progress < 1)
                        {
                            float x = 30 * MathF.Cos(4 * progress * MathHelper.TwoPi * Projectile.spriteDirection);
                            float y = 30 * MathF.Sin(4 * progress * MathHelper.TwoPi * Projectile.spriteDirection);
                            Vector2 ellipse = new(x, y);
                            vector = ellipse.RotatedBy(MathHelper.Pi) * 2.5f;

                            if (Timer % (int)(20 * SwingSpeed) == 0)
                                Player.direction *= -1;
                        }

                        if (progress > 1)
                        {
                            Projectile.Kill();
                        }
                        break;
                }
            }
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;


            if (progress > 0.3f && progress < 0.65f)
            {
                MakeDust(50 * Projectile.scale);
            }

            if (Main.netMode != NetmodeID.Server && --pauseTimer <= 0)
            {
                TrailSetUp();
            }
        }
        public void MakeDust(float pos)
        {
            float rotation = (Projectile.Center - Player.Center).ToRotation();
            Dust dust8 = Dust.NewDustPerfect(Player.Center + RedeHelper.PolarVector(pos, rotation) + new Vector2(Main.rand.NextFloat(-10 * Projectile.scale, 10 * Projectile.scale)), DustID.IceTorch, RedeHelper.PolarVector(5, rotation), 0, Scale: 2);
            dust8.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
            dust8.noGravity = true;
        }
        private void BlockProj()
        {
            foreach (Projectile target in Main.ActiveProjectiles)
            {
                if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 200 / 4)
                    continue;

                if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !target.HasElement(ElementID.Ice) || target.ProjBlockBlacklist(true))
                    continue;

                RedeDraw.SpawnExplosion(target.Center, new Color(214, 239, 243), shakeAmount: 0, scale: .5f, noDust: true, rot: RedeHelper.RandomRotation(), tex: "Redemption/Textures/SwordClash");
                target.Kill();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 3)
                modifiers.FinalDamage *= 2;
        }
        public int pauseTimer;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Player = Main.player[Projectile.owner];
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            if (glow < 0.8f)
                glow += 0.2f;
            if (Player.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(BuffType<PureChillDebuff>(), 300);

            Vector2 dir = Projectile.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            RedeParticleManager.CreateSlashParticle(drawPos, dir.RotateRandom(1f) * 60, .75f, Color.LightCyan, 10);
            RedeParticleManager.CreateDevilsPactParticle(drawPos, Vector2.Zero, 1f, Color.Salmon.WithAlpha(0), 307);
            for (int i = 0; i < 3; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                float randomVel = Main.rand.NextFloat(2f, 3f);
                Vector2 direction = target.Center.DirectionFrom(Player.Center);
                Vector2 position = target.Center - direction * 10;
                RedeParticleManager.CreateSpeedParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, .8f, Color.LightCyan.WithAlpha(0));
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] is 1 or 2)
            {
                return progress >= 0.45f && progress <= 0.55f ? null : false;
            }
            else if (Projectile.ai[0] is 3)
                return null;

            return false;
        }
        #region draw trail
        private Vector2[] oldDirVector = new Vector2[60];
        private List<Vector2> directionVectorCache = new();
        private List<Vector2> positionCache = new();
        private DanTrail trail;
        private Color baseColor = Color.LightSteelBlue;
        private Color endColor = Color.SteelBlue;
        private float thickness = 24f;
        private float opacity;
        public void TrailSetUp()
        {
            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            if (Timer == 2)
            {
                Projectile.alpha = 0;
                for (int i = 0; i < oldDirVector.Length; i++)
                    oldDirVector.SetValue(vector, i);
            }

            for (int k = oldDirVector.Length - 1; k > 0; k--)
            {
                oldDirVector[k] = oldDirVector[k - 1];
            }
            oldDirVector[0] = vector;

            if (Projectile.ai[0] == 3)
                opacity = MathHelper.Lerp(1, 0, progress);
            else
            {
                float x = (progress - 0.55f) * 10;
                if (progress < 0.3f || progress > 0.8f)
                    opacity = 0;
                else
                    opacity = 1 / (1 + x * x);
            }
            if (Projectile.ai[0] != 0 && Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageSwordTrailPosition(oldDirVector.Length, armCenter, oldDirVector, ref directionVectorCache, ref positionCache, 1.05f);
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
                if (mult < 0.98f)
                    delay = 1;
                return thickness * MathF.Pow(mult, 0.2f) * Projectile.scale * delay;
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

            Effect effect = Request<Effect>("Redemption/Effects/GlowTrailShader", AssetRequestMode.ImmediateLoad).Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0);
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            bool flip1 = Projectile.ai[0] == 1 && Player.direction == -1;
            bool flip2 = Projectile.ai[0] == 2 && Player.direction == 1;
            bool flip3 = Projectile.ai[0] == 3 && Projectile.spriteDirection == -1;
            Texture2D texture = Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5").Value;
            if (flip1 || flip2 || flip3)
                texture = Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5_flipped2").Value;

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
            Player Player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);
            Vector2 v = RedeHelper.PolarVector(10, (Projectile.Center - Player.Center).ToRotation());
            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
