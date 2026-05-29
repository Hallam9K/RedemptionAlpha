using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
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
            Projectile.friendly = false;
            Projectile.penetrate = 4;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

            InitializeTrail();
        }
        public Vector2 positionVector;
        public float startRotation;
        public float Timer;
        public float progress;
        public float SwingSpeed;
        public float glow;
        private Player Owner => Main.player[Projectile.owner];
        public int maxTime;
        public int origDamage;
        public override void OnSpawn(IEntitySource source)
        {
            maxTime = SetUseTime(Owner.HeldItem.useTime);
            Projectile.scale *= Owner.GetAdjustedItemScale(Owner.HeldItem);
            origDamage = Projectile.damage;
        }
        public override void AI()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.scale *= Projectile.ai[2];
                    Projectile.ai[0] = 2;
                    Projectile.netUpdate = true;
                    break;
                case 1: //down
                    BlockProj();
                    progress = Timer / (maxTime * 2 * Projectile.MaxUpdates);
                    Projectile.friendly = progress >= 0.4f && progress <= 0.6f;
                    if (Timer++ == 0)
                    {
                        Projectile.spriteDirection = Owner.direction;
                        startRotation = Owner.direction == 1 ? Projectile.velocity.ToRotation() : -Projectile.velocity.ToRotation() + MathHelper.Pi;
                    }
                    if (progress < 1)
                    {
                        float swingProgress = EaseFunction.EaseQuinticInOut.Ease(progress);
                        float initialRotation = -0.375f;
                        float angle = (swingProgress * 0.75f + initialRotation) * MathHelper.TwoPi;

                        Vector2 ellipse = new(90 * MathF.Cos(angle), 90 * MathF.Sin(angle));
                        ellipse = ellipse.RotatedBy(startRotation);
                        ellipse.X *= Owner.direction;
                        positionVector = ellipse * Projectile.scale;
                    }
                    if (Timer == (int)(maxTime * Projectile.MaxUpdates))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                    }
                    if (progress >= 1)
                    {
                        if (!Owner.channel)
                        {
                            Projectile.Kill();
                            return;
                        }
                        if (glow >= 0.8f)
                        {
                            RedeDraw.SpawnRing(Owner.Center, Color.LightCyan, 0.2f, 0.85f, 4);
                            RedeDraw.SpawnRing(Owner.Center, Color.LightCyan, 0.2f);
                            SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);

                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<ArcticWind_Proj>(), 0, 0, Projectile.owner);
                            Projectile.ai[0] = 3;
                        }
                        else
                        {
                            if (Projectile.owner == Main.myPlayer)
                            {
                                if (Main.MouseWorld.X < Owner.Center.X)
                                    Owner.direction = -1;
                                else
                                    Owner.direction = 1;
                                Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - Owner.Center).ToRotation());
                            }
                            Projectile.alpha = 255;
                            Projectile.ai[0]++;
                        }
                        Timer = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2: //up
                    BlockProj();
                    progress = Timer / (maxTime * 2 * Projectile.MaxUpdates);
                    Projectile.friendly = progress >= 0.4f && progress <= 0.6f;
                    if (Timer++ == 0)
                    {
                        Projectile.spriteDirection = Owner.direction;
                        startRotation = Owner.direction == 1 ? Projectile.velocity.ToRotation() : -Projectile.velocity.ToRotation() + MathHelper.Pi;
                    }
                    if (progress < 1)
                    {
                        float swingProgress = EaseFunction.EaseQuinticInOut.Ease(progress);
                        float initialRotation = 0.375f;
                        float angle = (swingProgress * -0.75f + initialRotation) * MathHelper.TwoPi;

                        Vector2 ellipse = new(90 * MathF.Cos(angle), 90 * MathF.Sin(angle));
                        ellipse = ellipse.RotatedBy(startRotation);
                        ellipse.X *= Owner.direction;
                        positionVector = ellipse * Projectile.scale;
                    }
                    if (Timer == (int)(maxTime * Projectile.MaxUpdates))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                    }
                    if (progress >= 1)
                    {
                        if (!Owner.channel)
                        {
                            Projectile.Kill();
                            return;
                        }
                        if (glow >= 0.8f)
                        {
                            RedeDraw.SpawnRing(Owner.Center, Color.LightCyan, 0.2f, 0.85f, 4);
                            RedeDraw.SpawnRing(Owner.Center, Color.LightCyan, 0.2f);
                            SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);

                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<ArcticWind_Proj>(), 0, 0, Projectile.owner);
                            Projectile.ai[0] = 3;
                        }
                        else
                        {
                            if (Projectile.owner == Main.myPlayer)
                            {
                                if (Main.MouseWorld.X < Owner.Center.X)
                                    Owner.direction = -1;
                                else
                                    Owner.direction = 1;
                                Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - Owner.Center).ToRotation());
                            }
                            Projectile.alpha = 255;
                            Projectile.ai[0] = 1;
                        }
                        Timer = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 3:
                    BlockProj();
                    progress = Timer / (maxTime * 4 * Projectile.MaxUpdates);
                    Projectile.friendly = true;
                    if (Timer++ == 0)
                    {
                        Projectile.spriteDirection = Owner.direction;
                        Projectile.localNPCHitCooldown = maxTime * Projectile.MaxUpdates;
                        Projectile.penetrate += 12;
                        Projectile.damage = origDamage;
                        Projectile.scale *= 1.25f;
                    }
                    if (progress < 1)
                    {
                        float swingProgress = EaseFunction.Linear.Ease(progress);
                        float initialRotation = 0.375f;
                        float angle = (swingProgress * 4f + initialRotation) * MathHelper.TwoPi;

                        Vector2 ellipse = new(90 * MathF.Cos(angle), 90 * MathF.Sin(angle));
                        ellipse = ellipse.RotatedBy(startRotation);
                        ellipse.X *= Projectile.spriteDirection;
                        positionVector = ellipse * Projectile.scale;

                        if (Timer % (int)(maxTime / 2 * Projectile.MaxUpdates) == 0)
                            Owner.direction *= -1;
                    }
                    if (progress > 1)
                    {
                        Projectile.Kill();
                    }
                    thickness = 36;
                    break;
            }
            Projectile.Center = armCenter + positionVector;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + 3 * MathHelper.PiOver4;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            if (progress > 0.3f && progress < 0.65f)
            {
                MakeDust(50 * Projectile.scale);
            }
            if (Main.netMode != NetmodeID.Server)
            {
                TrailSetUp();
            }
        }
        public void MakeDust(float pos)
        {
            float rotation = (Projectile.Center - Owner.Center).ToRotation();
            Dust dust8 = Dust.NewDustPerfect(Owner.Center + RedeHelper.PolarVector(pos, rotation) + new Vector2(Main.rand.NextFloat(-10 * Projectile.scale, 10 * Projectile.scale)), DustID.IceTorch, RedeHelper.PolarVector(5, rotation), 0, Scale: 2);
            dust8.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
            dust8.noGravity = true;
        }
        private void BlockProj()
        {
            foreach (Projectile target in Main.ActiveProjectiles)
            {
                if (!ProjReflect.FriendlyReflectCheck(Projectile, target, 500) || ProjReflect.ProjBlockBlacklist(target, true))
                    continue;

                if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !target.HasElement(ElementID.Ice))
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            if (glow < 0.8f)
                glow += 0.2f;
            if (Owner.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(BuffType<PureChillDebuff>(), 300);

            Vector2 dir = Projectile.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            RedeParticleManager.CreateSlashParticle(drawPos, dir.RotateRandom(1f) * 60, .75f, Color.LightCyan, 10);
            RedeParticleManager.CreateDevilsPactParticle(drawPos, Vector2.Zero, 1f, Color.Salmon.WithAlpha(0), 307);
            for (int i = 0; i < 3; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                float randomVel = Main.rand.NextFloat(2f, 3f);
                Vector2 direction = target.Center.DirectionFrom(Owner.Center);
                Vector2 position = target.Center - direction * 10;
                RedeParticleManager.CreateSpeedParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, .8f, Color.LightCyan.WithAlpha(0));
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
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            Vector2 drawPos = armCenter + positionVector.SafeNormalize(default) * 50f * Projectile.scale;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
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
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);
            if (Timer == 2)
            {
                Projectile.alpha = 0;
                for (int i = 0; i < oldDirVector.Length; i++)
                    oldDirVector.SetValue(positionVector * 0.75f, i);
            }
            for (int k = oldDirVector.Length - 1; k > 0; k--)
            {
                oldDirVector[k] = oldDirVector[k - 1];
            }
            oldDirVector[0] = positionVector * 0.75f;

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

            bool flip1 = Projectile.ai[0] == 1 && Owner.direction == -1;
            bool flip2 = Projectile.ai[0] == 2 && Owner.direction == 1;
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
    }
}