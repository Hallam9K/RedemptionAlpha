using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
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
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class DragonCleaver_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/DragonCleaver";

        public float[] oldrot = new float[10];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjFire[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 4;
            Projectile.usesOwnerMeleeHitCD = true;
        }
        Player Player => Main.player[Projectile.owner];
        private float startRotation;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[2];
        public int pauseTimer;
        public float progress;
        public float SwingSpeed;
        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 2, -2);
            Projectile.Center = armCenter + vector;

            Projectile.spriteDirection = Player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            if (Main.myPlayer == Projectile.owner && --pauseTimer <= 0)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        progress = Timer / (200 * SwingSpeed);
                        if (Timer++ == 0)
                        {
                            Projectile.width = (int)(70 * Projectile.ai[2]);
                            Projectile.height = (int)(70 * Projectile.ai[2]);

                            Projectile.scale *= Projectile.ai[2];
                            startRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (Timer == (int)(75 * SwingSpeed))
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1 with { Pitch = -.6f }, Player.position);
                        }

                        if (progress < 1f)
                        {
                            float modifiedProgress = 0.05f + 1 / (1 + MathF.Pow(3, -14 * (progress - 0.5f)));
                            float x = 30 * MathF.Cos(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.8f);
                            float y = 30 * MathF.Sin(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.8f);
                            Vector2 ellipse = new(x, y);
                            vector = ellipse.RotatedBy(startRotation) * 2.5f * Projectile.scale;
                        }
                        else
                            Projectile.Kill();
                        break;
                    case 1:
                        progress = Timer / (200 * SwingSpeed);
                        if (Timer++ == 0)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swoosh1 with { Pitch = -.6f }, Player.position);
                            Projectile.friendly = false;
                            Projectile.width = (int)(70 * Projectile.ai[2]);
                            Projectile.height = (int)(70 * Projectile.ai[2]);
                            Length = 50 * Projectile.ai[2];
                            vector = RedeHelper.PolarVector(Length, Projectile.velocity.ToRotation());
                        }
                        if (Timer == 20)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.MountedCenter + vector, Main.MouseWorld.DirectionFrom(Player.Center).SafeNormalize(Vector2.One), ModContent.ProjectileType<DragonCleaverSkull_Proj>(), Projectile.damage * 3, Projectile.knockBack, Player.whoAmI);

                        if (progress > 1f)
                            Projectile.Kill();
                        break;
                }
            }

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            if (progress > 0.3f && progress < 0.7f)
            {
                BlockProj();

                Projectile.friendly = true;
                if (Projectile.ai[0] == 0)
                    MakeDust(70 * Projectile.scale);
            }
            else
                Projectile.friendly = false;

            if (Main.netMode != NetmodeID.Server && --pauseTimer <= 0)
            {
                TrailSetUp();
            }
        }
        public void MakeDust(float pos)
        {
            float rotation = (Projectile.Center - Player.Center).ToRotation();
            Dust dust8 = Dust.NewDustPerfect(Player.Center + RedeHelper.PolarVector(pos, rotation) + new Vector2(Main.rand.NextFloat(-20 * Projectile.scale, 20 * Projectile.scale)), 259, RedeHelper.PolarVector(6, rotation), 0);
            dust8.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
            dust8.noGravity = true;
        }
        private void BlockProj()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile target = Main.projectile[i];
                if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 200 / 4)
                    continue;

                if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !target.HasElement(ElementID.Fire) || target.ProjBlockBlacklist(true))
                    continue;

                RedeDraw.SpawnExplosion(target.Center, Color.Orange, shakeAmount: 0, scale: .5f, noDust: true, rot: RedeHelper.RandomRotation(), tex: ModContent.Request<Texture2D>("Redemption/Textures/SwordClash").Value);

                if (Player.HeldItem.ModItem is DragonCleaver host)
                {
                    host.Count++;
                    if (host.Count == 6 && Projectile.ai[1] == 0)
                    {
                        Projectile.ai[1] = 1;
                        SoundEngine.PlaySound(SoundID.Item88, Player.Center);
                        RedeDraw.SpawnRing(Player.Center, new Color(255, 65, 65), 0.12f, 0.86f, 4);
                    }
                }

                target.Kill();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dragonlike.Contains(target.type))
                modifiers.FinalDamage *= 4;
        }

        public int hitfury;
        public bool strike;
        private bool parried;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = .8f }, Projectile.position);
            if (!strike)
            {
                Player.RedemptionScreen().ScreenShakeIntensity += 5;
                strike = true;
                pauseTimer = Projectile.extraUpdates - 1;
            }
            if (Player.HeldItem.ModItem is DragonCleaver host)
            {
                host.Count++;
                if (host.Count == 6 && Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] = 1;
                    SoundEngine.PlaySound(SoundID.Item88, Player.Center);
                    RedeDraw.SpawnRing(Player.Center, new Color(255, 65, 65), 0.12f, 0.86f, 4);
                }
            }
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            if (Player.RedemptionPlayerBuff().dragonLeadBonus)
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);

            Vector2 dir = Projectile.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            ParticleSystem.NewParticle(drawPos, dir.RotateRandom(1f) * 60, new SlashParticleAlt(10, 1), Color.Salmon, .75f, layer: Layer.BeforePlayers);
            ParticleSystem.NewParticle(drawPos, Vector2.Zero, new DevilsPactParticle(127), Color.Salmon with { A = 0 }, 2f, layer: Layer.BeforePlayers);
            for (int i = 0; i < 4; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                float randomVel = Main.rand.NextFloat(2f, 4f);
                Vector2 direction = target.DirectionFrom(Player.Center);
                Vector2 position = target.Center - direction * 10;
                ParticleSystem.NewParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, new SpeedParticle(), Color.Salmon, 0.8f);
            }
        }
        #region draw trail
        private Vector2[] oldDirVector = new Vector2[60];
        private List<Vector2> directionVectorCache = new();
        private List<Vector2> positionCache = new();
        private DanTrail trail;
        public Color baseColor = Color.OrangeRed * .7f;
        public Color endColor = Color.Yellow * .2f;
        private float thickness = 32f;
        private float opacity = 0;
        public void TrailSetUp()
        {
            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 2, -2);
            if (Timer == 2)
            {
                Projectile.alpha = 0;
                for (int i = 0; i < oldDirVector.Length; i++)
                    oldDirVector.SetValue(vector, i);
            }

            float x = (progress - 0.55f) * 10;
            if (progress < 0.3f || progress > 0.85f)
                opacity = 0;
            else
                opacity = 1 / (1 + x * x);

            for (int k = oldDirVector.Length - 1; k > 0; k--)
            {
                oldDirVector[k] = oldDirVector[k - 1];
            }
            oldDirVector[0] = vector;

            if (Projectile.ai[0] == 0 && Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageSwordTrailPosition(oldDirVector.Length, armCenter, oldDirVector, ref directionVectorCache, ref positionCache, 1.1f);
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
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 2, -2);
            Vector2 dir = RedeHelper.PolarVector(1, (armCenter - Projectile.Center).ToRotation());
            Vector2 Center = Projectile.Center + dir * 20;
            Main.EntitySpriteDraw(texture, Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
