using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Cooldowns;
using Redemption.Effects.Trails;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles.Melee;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class XeniumLance_Proj : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float SwingType => ref Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[1];

        private Vector2 positionVector;

        private float progress;

        private float swingProgress;

        private float startRotation;
        public override bool ShouldUpdatePosition() => false;
        public override void SetStaticDefaults()
        {
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.Redemption().TechnicallyMelee = true;

            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            trailVector = new Vector2[trailLength];
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            Utils.WriteVector2(writer, positionVector);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            positionVector = Utils.ReadVector2(reader);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Owner.direction;
            Projectile.scale *= Projectile.ai[2];
        }
        public override void AI()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter);

            switch (SwingType)
            {
                case 0:
                    progress = Timer / (Owner.HeldItem.useTime / Owner.GetAttackSpeed(DamageClass.Melee) * Projectile.MaxUpdates);
                    if (Timer == 0)
                    {
                        Owner.ChangeDir(Projectile.velocity.X >= 0 ? 1 : -1);
                        startRotation = Owner.direction == 1 ? Projectile.velocity.ToRotation() : -Projectile.velocity.ToRotation() + MathHelper.Pi;
                    }
                    if (progress <= 1)
                    {
                        swingProgress = EaseFunction.EaseQuadInOut.Ease(progress);
                        float angle = (0.9f * swingProgress + -0.45f) * MathHelper.TwoPi;
                        float x = MathF.Cos(angle) * (30 + 90 * BaseUtility.MultiLerp(swingProgress, 0, 1, 1, 1, 0));
                        float y = MathF.Sin(angle) * 90;
                        Vector2 path = new(x, y);
                        path = path.RotatedBy(startRotation);
                        path.X *= Owner.direction;

                        positionVector = path * Projectile.scale;
                    }
                    if (progress > 1)
                    {
                        Projectile.Kill();
                    }
                    Projectile.friendly = swingProgress < 0.98f;
                    TrailSetUp();
                    break;
                case 1:
                    progress = Timer / (Owner.HeldItem.useTime / Owner.GetAttackSpeed(DamageClass.Melee) * Projectile.MaxUpdates);
                    if (progress == 0)
                    {
                        Projectile.velocity = playerCenter.DirectionTo(Main.MouseWorld);
                        Owner.ChangeDir(Projectile.velocity.X >= 0 ? 1 : -1);
                        startRotation = Owner.direction == 1 ? Projectile.velocity.ToRotation() : -Projectile.velocity.ToRotation() + MathHelper.Pi;
                    }
                    if (progress <= 1)
                    {
                        swingProgress = EaseFunction.EaseQuadInOut.Ease(progress);
                        float angle = (1.45f * swingProgress + -0.45f) * -MathHelper.TwoPi;
                        float x = MathF.Cos(angle) * (30 + 90 * BaseUtility.MultiLerp(swingProgress, 0, 1, 1, 1, 1, 1, 0));
                        float y = MathF.Sin(angle) * 90;
                        Vector2 path = new(x, y);
                        path = path.RotatedBy(startRotation);
                        path.X *= Owner.direction;

                        positionVector = path * Projectile.scale;
                    }
                    if (progress > 1)
                    {
                        Projectile.Kill();
                    }
                    Projectile.friendly = swingProgress < 0.98f;
                    TrailSetUp();
                    break;
                case 2:
                    progress = Timer / (Owner.HeldItem.useTime / Owner.GetAttackSpeed(DamageClass.Melee) * Projectile.MaxUpdates);
                    if (progress == 0)
                    {
                        Owner.ChangeDir(Projectile.velocity.X >= 0 ? 1 : -1);
                        startRotation = Owner.direction == 1 ? Projectile.velocity.ToRotation() : -Projectile.velocity.ToRotation() + MathHelper.Pi;

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Swoosh1.WithPitchOffset(.2f).WithVolumeScale(.5f), Owner.position);
                    }
                    if (progress <= 1)
                    {
                        swingProgress = EaseFunction.EaseCubicInOut.Ease(progress);
                        float x = MathF.Cos(0) * (30 + 150 * BaseUtility.MultiLerp(swingProgress, 0, 1, 0));
                        float y = MathF.Sin(0) * 120;
                        Vector2 path = new(x, y);
                        path = path.RotatedBy(startRotation);
                        path.X *= Owner.direction;

                        positionVector = path * Projectile.scale;
                    }
                    if (progress > 1)
                    {
                        Projectile.Kill();
                    }
                    Projectile.friendly = swingProgress < 0.98f;
                    break;
                case 3:
                    progress = Timer / (10 * Projectile.MaxUpdates);
                    if (Timer == 0)
                    {
                        Projectile.friendly = true;
                        Projectile.tileCollide = true;
                        Projectile.velocity = Projectile.velocity.SafeNormalize(default) * 15f;
                        startRotation = Owner.direction == 1 ? Projectile.velocity.ToRotation() : -Projectile.velocity.ToRotation() + MathHelper.Pi;
                    }
                    if (progress <= 1)
                    {
                        swingProgress = EaseFunction.EaseCubicInOut.Ease(progress);
                        float x = MathF.Cos(0) * (30 + 120 * BaseUtility.MultiLerp(swingProgress, 0, 1, 1, 0));
                        float y = MathF.Sin(0) * 60;
                        Vector2 path = new(x, y);
                        path = path.RotatedBy(startRotation);
                        path.X *= Owner.direction;

                        positionVector = path * Projectile.scale;
                        Owner.velocity = Projectile.velocity * 0.5f;
                        Owner.position += Projectile.velocity * 0.5f;

                        Color c = new Color(100, 255, 100);

                        for (int i = -1; i < 2; i += 2)
                        {
                            Vector2 pos = Projectile.Center + Projectile.velocity * 2 + Projectile.velocity.RotatedBy(1.57f * i);
                            Vector2 vel = Projectile.velocity.RotatedBy(i * 2.85f);
                            RedeParticleManager.CreateAdditiveGlowParticle(pos, vel, new Vector2(1.6f, 0.2f) * 1, c, 12, 0.91f);
                        }
                    }
                    if (progress >= 1)
                    {
                        Projectile.Kill();
                    }
                    if (Timer >= 5 * Projectile.MaxUpdates)
                    {
                        if (Timer % 6 == 0)
                            RedeParticleManager.CreateSpeedParticle(Main.rand.NextVector2FromRectangle(Owner.Hitbox), -Projectile.velocity * 2, .75f, Color.LightSeaGreen.WithAlpha(0));
                    }
                    if (Timer >= 10 * Projectile.MaxUpdates)
                    {
                        Owner.Redemption().contactImmune = true;
                    }
                    break;

            }
            Projectile.rotation = positionVector.ToRotation();
            Projectile.Center = playerCenter + positionVector;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathHelper.ToRadians(0));
            Timer++;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 8;
            return true;
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = new(DelegateMethods.CutTiles);
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 lineEnd = Projectile.Center + positionVector * 0.1f;
            float height = Projectile.height * Projectile.scale;
            Utils.PlotTileLine(lineStart, lineEnd, height, cut);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 lineEnd = Projectile.Center + positionVector * 0.1f;
            float height = Projectile.height * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, height, ref point))
                return true;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ProjectileType<XeniumLanceSpark_Proj>(), Projectile.damage / 5, 0, Projectile.owner);

            SoundEngine.PlaySound(CustomSounds.Gun2KS with { Volume = 1f }, Projectile.position);
            Vector2 drawPos = Vector2.Lerp(target.Center, Projectile.Center, 0.01f) + Main.rand.NextVector2Square(-10, 0);
            for (int i = 0; i < 6; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.3f, 0.3f);
                float randomVel = Main.rand.NextFloat(2f, 3f) * 20;
                Vector2 direction = target.DirectionFrom(Owner.Center);
                Vector2 drawPos2 = drawPos - direction * 30;
                RedeParticleManager.CreateSpeedParticle(drawPos2, direction.RotatedBy(randomRotation) * randomVel, .1f, Color.LightGreen.WithAlpha(0), extension: 100);
                RedeParticleManager.CreateSpeedParticle(drawPos2, direction.RotatedBy(randomRotation) * randomVel, .5f, Color.LightSeaGreen.WithAlpha(0), extension: 100);
            }
            if(SwingType is 3)
            {
                Owner.ClearBuff(BuffType<XeniumLanceCooldown>());

                Owner.immune = true;
                Owner.immuneTime = (int)MathHelper.Max(Owner.immuneTime, 20);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (SwingType == 3 && Timer >= 10 * Projectile.MaxUpdates)
                modifiers.FlatBonusDamage += Timer * 100;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            DrawSword(lightColor);
            return false;
        }
        private void DrawSword(Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 drawAnchor = Owner.RotatedRelativePoint(Owner.MountedCenter) - Main.screenPosition;
            Vector2 drawPos = drawAnchor + positionVector + positionVector.SafeNormalize(default) * -50;

            float rotation = Projectile.spriteDirection == 1 ? Projectile.rotation + 1 * MathHelper.PiOver4 : Projectile.rotation + 3 * MathHelper.PiOver4;
            Main.EntitySpriteDraw(texture, drawPos, rect, Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, spriteEffects, 0);
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
            trailVector[0] = positionVector + positionVector * 0.1f;

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
                Color color = new(50, 205, 50, 0);
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

            Texture2D texture = Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_2").Value;
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(texture);
            effect.Parameters["horizontalFlip"].SetValue(false);
            effect.Parameters["brightTip"].SetValue(true);
            effect.Parameters["minimumDistanceFromCenter"].SetValue(3);
            effect.Parameters["squishToEdgeFactor"].SetValue(0);
            effect.Parameters["squishPowerInverse"].SetValue(0.9f);
            effect.Parameters["tipColor"].SetValue(new Vector4(1, 1, 1, 1));
            effect.Parameters["interpolantStart"].SetValue(0.955f);
            effect.Parameters["interpolantEnd"].SetValue(1f);
            effect.Parameters["intensity"].SetValue(1);
            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
        #endregion
    }
}
