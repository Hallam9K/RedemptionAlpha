using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Effects.Trails;
using Redemption.Effects.Trails.Tips;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles.Melee;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

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
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = false;
            Projectile.penetrate = 6;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 4;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

            InitializeTrail();
        }
        public Player Owner => Main.player[Projectile.owner];
        public float startRotation;
        public Vector2 positionVector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public int pauseTimer;
        public float progress;
        public float SwingSpeed;
        public int maxTime;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Owner.direction;
            maxTime = SetUseTime(Owner.HeldItem.useTime);
            Projectile.scale *= Owner.GetAdjustedItemScale(Owner.HeldItem);
            Projectile.width = (int)(70 * Projectile.scale);
            Projectile.height = (int)(70 * Projectile.scale);
        }
        public override void AI()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            if (--pauseTimer <= 0)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        progress = Timer / (maxTime * Projectile.MaxUpdates);
                    Projectile.friendly = progress >= 0.3f && progress <= 0.7f;
                        if (Timer++ == 0)
                        {
                            startRotation = Owner.direction == 1 ? Projectile.velocity.ToRotation() : -Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (Timer == (int)(maxTime / 2 * Projectile.MaxUpdates))
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1 with { Pitch = -.6f }, Owner.position);
                        }

                        if (progress < 1f)
                        {
                            float swingProgress = EaseFunction.EaseQuinticInOut.Ease(progress);
                            float initialRotation = -0.375f;
                            float angle = (swingProgress * 0.75f + initialRotation) * MathHelper.TwoPi;

                            Vector2 ellipse = new(90 * MathF.Cos(angle), 90 * MathF.Sin(angle));
                            ellipse = ellipse.RotatedBy(startRotation);
                            ellipse.X *= Owner.direction;
                            positionVector = ellipse * Projectile.scale;
                        }
                        else
                            Projectile.Kill();
                        break;
                    case 1:
                        progress = Timer / (maxTime * Projectile.MaxUpdates);
                        if (Timer++ == 0)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swoosh1 with { Pitch = -.6f }, Owner.position);
                            Length = 50 * Projectile.scale;
                            positionVector = RedeHelper.PolarVector(Length, Projectile.velocity.ToRotation());
                        }
                        if (Timer == 20)
                        {
                            if(Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.MountedCenter + positionVector, Main.MouseWorld.DirectionFrom(Owner.Center).SafeNormalize(Vector2.One), ProjectileType<DragonCleaverSkull_Proj>(), Projectile.damage * 3, Projectile.knockBack, Owner.whoAmI);
                        }
                        if (progress > 1f)
                            Projectile.Kill();
                        break;
                }
            }
            Projectile.Center = armCenter + positionVector;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            if (progress > 0.3f && progress < 0.7f)
            {
                BlockProj();
                if (Projectile.ai[0] == 0)
                    MakeDust(70 * Projectile.scale);
            }
            if (!Main.dedServ && --pauseTimer <= 0)
            {
                TrailSetUp();
            }
        }
        public void MakeDust(float pos)
        {
            float rotation = (Projectile.Center - Owner.Center).ToRotation();
            Dust dust8 = Dust.NewDustPerfect(Owner.Center + RedeHelper.PolarVector(pos, rotation) + new Vector2(Main.rand.NextFloat(-20 * Projectile.scale, 20 * Projectile.scale)), DustID.SolarFlare, RedeHelper.PolarVector(6, rotation), 0);
            dust8.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
            dust8.noGravity = true;
        }
        private void BlockProj()
        {
            foreach (Projectile target in Main.ActiveProjectiles)
            {
                if (!ProjReflect.FriendlyReflectCheck(Projectile, target, 500) || ProjReflect.ProjBlockBlacklist(target, true))
                    continue;

                if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !target.HasElement(ElementID.Fire))
                    continue;

                RedeDraw.SpawnExplosion(target.Center, Color.Orange, shakeAmount: 0, scale: .5f, noDust: true, rot: RedeHelper.RandomRotation(), tex: "Redemption/Textures/SwordClash");

                if (Owner.HeldItem.ModItem is DragonCleaver host)
                {
                    host.Count++;
                    if (host.Count == 6 && Projectile.ai[1] == 0)
                    {
                        Projectile.ai[1] = 1;
                        SoundEngine.PlaySound(SoundID.Item88, Owner.Center);
                        RedeDraw.SpawnRing(Owner.Center, new Color(255, 65, 65), 0.12f, 0.86f, 4);
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
                Owner.RedemptionScreen().ScreenShakeIntensity += 5;
                strike = true;
                pauseTimer = maxTime / 10 * Projectile.MaxUpdates;
            }
            if (Owner.HeldItem.ModItem is DragonCleaver host)
            {
                host.Count++;
                if (host.Count == 6 && Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] = 1;
                    SoundEngine.PlaySound(SoundID.Item88, Owner.Center);
                    RedeDraw.SpawnRing(Owner.Center, new Color(255, 65, 65), 0.12f, 0.86f, 4);
                }
            }
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            if (Owner.RedemptionPlayerBuff().dragonLeadBonus)
                target.AddBuff(BuffType<DragonblazeDebuff>(), 300);

            Vector2 dir = Projectile.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            RedeParticleManager.CreateSlashParticle(drawPos, dir.RotateRandom(1f) * 60, .75f, Color.Salmon, 10);
            RedeParticleManager.CreateDevilsPactParticle(drawPos, Vector2.Zero, 2f, Color.Salmon.WithAlpha(0), 127);
            for (int i = 0; i < 4; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                float randomVel = Main.rand.NextFloat(2f, 4f);
                Vector2 direction = target.DirectionFrom(Owner.Center);
                Vector2 position = target.Center - direction * 10;
                RedeParticleManager.CreateSpeedParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, 0.8f, Color.Salmon.WithAlpha(0));
            }
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = new(DelegateMethods.CutTiles);
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 lineEnd = Projectile.Center;
            float height = Projectile.height * Projectile.scale;
            Utils.PlotTileLine(lineStart, lineEnd, height, cut);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 lineEnd = Projectile.Center;
            float height = Projectile.height * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, height, ref point))
                return true;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = Projectile.spriteDirection == 1 ? new(texture.Height / 2, texture.Height / 2) : new(texture.Width - texture.Height / 2, texture.Height / 2);
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            Vector2 drawPos = armCenter + positionVector.SafeNormalize(default) * 50f * Projectile.scale;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
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
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true) + new Vector2(-Owner.direction * 4, -4);
            if (Timer == 2)
            {
                Projectile.alpha = 0;
                for (int i = 0; i < oldDirVector.Length; i++)
                    oldDirVector.SetValue(positionVector * 0.75f, i);
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
            oldDirVector[0] = positionVector * 0.75f;

            if (Projectile.ai[0] == 0 && Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageSwordTrailPosition(oldDirVector.Length, armCenter, oldDirVector, ref directionVectorCache, ref positionCache, 1);
                ManageTrail();
            }
        }

        public void InitializeTrail()
        {
            trail = new DanTrail(RedeGraphics.Instance.Primitives, new NoTip(),
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
        }

        public void ManageTrail()
        {
            trail.SetPositions(positionCache.ToArray(), Projectile.Center);
        }
        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Effect effect = Request<Effect>("Redemption/Effects/GlowTrailShader").Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0);
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            bool flip = Owner.direction == -1;
            Texture2D texture = Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5").Value;
            if (flip)
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


    }
}
