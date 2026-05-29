using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Buffs.Minions;
using Redemption.Effects.Trails;
using Redemption.Effects.Trails.Tips;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Redemption.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

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
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            trailVector = new Vector2[trailLength];
        }
        private float startRotation;
        private Vector2 positionVector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        private Player Owner => Main.player[Projectile.owner];
        public float Timer;
        public int pauseTimer;
        public float progress;
        public int maxTime;
        public override void AI()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Projectile.spriteDirection = Owner.direction;

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);
            maxTime = SetUseTime(Owner.HeldItem.useTime);
            progress = (Projectile.ai[0] == 0) ? Timer / (maxTime * 0.8f * Projectile.MaxUpdates) : Timer / (maxTime * Projectile.MaxUpdates);
            if (--pauseTimer <= 0)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Timer++ == 0)
                        {
                            Projectile.scale *= Projectile.ai[2];
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1.WithVolumeScale(.7f), Owner.position);
                            startRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (progress < 1f)
                        {
                            float modifiedProgress = 0.2f + 1 / (1 + MathF.Pow(3, -15 * (progress - 0.5f)));
                            float x = 35 * MathF.Cos(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.7f);
                            float y = 20 * MathF.Sin(modifiedProgress * MathHelper.TwoPi * Projectile.spriteDirection * 0.7f);
                            Vector2 ellipse = new(x, y);
                            positionVector = ellipse.RotatedBy(startRotation) * 3f * Projectile.scale;
                        }
                        if (progress >= 1)
                        {
                            if (Projectile.owner == Main.myPlayer)
                            {
                                if (Main.MouseWorld.X < Owner.Center.X)
                                    Owner.direction = -1;
                                else
                                    Owner.direction = 1;

                                Projectile.velocity = armCenter.DirectionTo(Main.MouseWorld);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), armCenter, Projectile.velocity, ProjectileType<WraithSlayer_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                            }
                            Projectile.Kill();
                        }
                        break;
                    case 1:
                        if (Timer++ == 0)
                        {
                            strike = false;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundID.Item71, Owner.position);
                            startRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                        }
                        if (progress < 1f)
                        {
                            float modifiedProgress = -0.6f + 1 / (1 + MathF.Pow(3, -15 * (progress - 0.5f)));
                            float x = 20 * MathF.Cos((1 - modifiedProgress) * MathHelper.TwoPi * Projectile.spriteDirection * 0.5f);
                            float y = 30 * MathF.Sin((1 - modifiedProgress) * MathHelper.TwoPi * Projectile.spriteDirection * 0.5f);
                            Vector2 ellipse = new(x, y);
                            positionVector = ellipse.RotatedBy(startRotation) * 3.25f * Projectile.scale;
                        }
                        if (progress >= 1)
                            Projectile.Kill();
                        break;
                }
            }
            Projectile.Center = armCenter + positionVector;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4 - (Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver2);
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4 + (Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver2);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);

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
            return !target.friendly && progress >= 0.2f && progress <= 0.8 && target.immune[Projectile.owner] == 0 ? null : false;
        }
        public bool strike;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            if (!strike)
            {
                SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = -.2f }, Projectile.position);
                Owner.RedemptionScreen().ScreenShakeIntensity += 4;
                strike = true;
                pauseTimer = 1;
            }

            float Rot = Projectile.ai[0] == 0 ? 1 : -1;
            Vector2 dir = Projectile.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            RedeParticleManager.CreateSlashParticle(drawPos, dir.RotatedBy(0.4f * Rot * Owner.direction) * 80, 1, Color.MediumPurple, 8);
            for (int i = 0; i < 5; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.4f, 0.4f);
                float randomVel = Main.rand.NextFloat(2f, 3f);
                Vector2 direction = target.Center.DirectionFrom(Owner.Center);
                Vector2 position = target.Center - direction * 30;
                RedeParticleManager.CreateSpeedParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, .8f, Color.MediumPurple.WithAlpha(0));
            }
            if (target.life <= 0 && target.lifeMax >= 50 && (Main.rand.NextBool(6) || NPCLists.Spirit.Contains(target.type)) && NPC.CountNPCS(NPCType<WraithSlayer_Samurai>()) < 4)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(target.position, target.width, target.height, DustID.Wraith);

                Owner.AddBuff(BuffType<CursedSamuraiBuff>(), 2);
                RedeHelper.SpawnNPC(Projectile.GetSource_FromThis(), (int)target.Center.X, (int)target.Center.Y, NPCType<WraithSlayer_Samurai>(), ai3: Owner.whoAmI);
            }
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = new(DelegateMethods.CutTiles);
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 lineEnd = Projectile.Center + positionVector * 0.5f;
            float height = Projectile.height * Projectile.scale;
            Utils.PlotTileLine(lineStart, lineEnd, height, cut);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 lineEnd = Projectile.Center + positionVector * 0.5f;
            float height = Projectile.height * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, height, ref point))
                return true;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects spriteEffects2 = Projectile.ai[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            Vector2 drawPos = armCenter + positionVector * 0.8f;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects | spriteEffects2, 0);
            return false;
        }
        #region draw trail
        private Ellipse trail;
        private List<Vector2> trailCache = new();
        private Vector2[] trailVector;
        private int trailLength = 30;
        public void TrailSetUp()
        {
            if (!Main.dedServ)
            {
                ManageCache();
                if (trailCache.Count > 3)
                    ManageTrail();
            }
        }
        public void ManageCache()
        {
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);

            for (int i = trailVector.Length - 1; i > 0; i--)
            {
                trailVector[i] = trailVector[i - 1];
            }
            trailVector[0] = positionVector + positionVector * 0.5f;

            trailCache = new List<Vector2>();
            for (int i = 0; i < trailVector.Length; i++)
            {
                if (trailVector[i] != Vector2.Zero)
                {
                    trailCache.Add(armCenter + trailVector[i]);
                }
            }
        }
        public void ManageTrail()
        {
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);
            trail = new Ellipse(RedeGraphics.Instance.Primitives,
            factor =>
            {
                return 1;
            },
            factor =>
            {
                Color color = Color.Lerp(Color.MediumPurple, Color.MediumPurple * 0.2f, factor.X);
                float opacity = MathHelper.Clamp(1 - 1f * progress, 0, 1);
                return color * opacity;
            });
            trail.SetPositions(trailCache.ToArray(), armCenter);
        }
        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            Effect effect = Request<Effect>("Redemption/Effects/HikariteDaggerSlash").Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0);
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D texture = Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5").Value;
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(texture);
            effect.Parameters["horizontalFlip"].SetValue(false);
            effect.Parameters["brightTip"].SetValue(true);
            effect.Parameters["minimumDistanceFromCenter"].SetValue(1f);
            effect.Parameters["squishToEdgeFactor"].SetValue(1);
            effect.Parameters["squishPowerInverse"].SetValue(0.75f);
            effect.Parameters["tipColor"].SetValue(new Vector4(1, 1, 1, 1));
            effect.Parameters["interpolantStart"].SetValue(0);
            effect.Parameters["interpolantEnd"].SetValue(1f);
            effect.Parameters["intensity"].SetValue(5);
            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
        #endregion
    }
}
