using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Effects.Trails;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Spellsong_Proj : TrueMeleeProjectile
    {
        public float[] oldrot = new float[20];
        public Vector2[] oldPos = new Vector2[20];

        public override string Texture => "Redemption/Items/Weapons/HM/Melee/Spellsong";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spellsong, Core of the West");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            trailVector = new Vector2[trailLength];
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
        }
        private Player Owner => Main.player[Projectile.owner];
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        private Vector2 startVector;
        private Vector2 positionVector;
        private Vector2 mouseOrig;
        public float Timer;
        public float progress;
        public int maxTime;
        public override void OnSpawn(IEntitySource source)
        {
            maxTime = SetUseTime(Owner.HeldItem.useTime);
            Projectile.scale *= Owner.GetAdjustedItemScale(Owner.HeldItem);
            Length = 110 * Projectile.scale;
        }
        public override void AI()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();

            maxTime = SetUseTime(Owner.HeldItem.useTime);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Projectile.spriteDirection = Owner.direction;

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            switch (Projectile.ai[0])
            {
                case 0:
                    progress = Timer / (maxTime * Projectile.MaxUpdates);
                    if (Timer++ == 0)
                    {
                        if (Projectile.owner == Main.myPlayer)
                            mouseOrig = Main.MouseWorld;
                        SoundEngine.PlaySound(SoundID.Item71, Owner.position);
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                    }
                    if (Timer == (int)(maxTime / 6 * Projectile.MaxUpdates))
                    {
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center,
                            RedeHelper.PolarVector(55, (mouseOrig - armCenter).ToRotation()),
                            ProjectileType<SpellsongSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    if (progress < 1)
                    {
                        Rot = MathHelper.ToRadians(50 + 170f * MathF.Atan(3f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                        positionVector = startVector.RotatedBy(Rot) * Length;
                    }
                    if (progress >= 1)
                    {
                        if (!Owner.channel)
                        {
                            Projectile.Kill();
                            return;
                        }
                        if (Projectile.owner == Main.myPlayer)
                        {
                            if (Main.MouseWorld.X < Owner.Center.X)
                                Owner.direction = -1;
                            else
                                Owner.direction = 1;
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - armCenter).ToRotation() - ((MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.spriteDirection));
                            mouseOrig = Main.MouseWorld;
                        }
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                        Projectile.ai[0]++;
                        Timer = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 1:
                    progress = Timer / (maxTime * Projectile.MaxUpdates);
                    if (Timer++ == 0)
                    {
                        trailVector = new Vector2[trailLength];
                    }
                    if (Timer == (int)(maxTime / 6 * Projectile.MaxUpdates))
                    {
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center,
                            RedeHelper.PolarVector(55, (mouseOrig - armCenter).ToRotation()),
                            ProjectileType<SpellsongSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                        }
                    }
                    if (progress < 1)
                    {
                        Rot = -MathHelper.ToRadians(190 + 170f * MathF.Atan(3f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                        positionVector = startVector.RotatedBy(Rot) * Length;
                    }
                    if (progress >= 1)
                    {
                        if (!Owner.channel)
                        {
                            Projectile.Kill();
                            return;
                        }
                        if (Projectile.owner == Main.myPlayer)
                            mouseOrig = Main.MouseWorld;
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                        Projectile.ai[0]++;
                        Timer = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2:
                    progress = Timer / (maxTime * 1.5f * Projectile.MaxUpdates);
                    if (Timer++ == 0)
                    {
                        trailVector = new Vector2[trailLength];
                    }
                    if (progress < 1)
                    {
                        Rot = BaseUtility.MultiLerp(EaseFunction.EaseQuinticOut.Ease(progress), MathHelper.ToRadians(0), MathHelper.ToRadians(360 + 90));
                        positionVector = startVector.RotatedBy(Rot * Projectile.spriteDirection) * Length;
                    }
                    if (progress >= 1)
                        Projectile.Kill();

                    if (Timer == (int)(maxTime / 6 * Projectile.MaxUpdates))
                    {
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center,
                            RedeHelper.PolarVector(55, (mouseOrig - armCenter).ToRotation()),
                            ProjectileType<SpellsongSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    if (Timer == (int)(maxTime * 1.25f * Projectile.MaxUpdates) && Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center - RedeHelper.PolarVector(4, Projectile.rotation), RedeHelper.PolarVector(1, (Projectile.Center - armCenter).ToRotation()), ProjectileType<Spellsong_Beam>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    break;
            }
            Projectile.Center = armCenter + positionVector;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            TrailSetUp();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return (progress < 0.4f && Projectile.ai[0] == 2) || (progress <= 0.18f && Projectile.ai[0] < 2) ? null : false;
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
        #region draw trail
        public Color baseColor = Color.LightPink;
        public Color endColor = Color.Purple;

        private Ellipse trail;
        private List<Vector2> trailCache = new();
        private Vector2[] trailVector;
        private readonly int trailLength = 20;
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
            trailVector[0] = positionVector;

            trailCache = new List<Vector2>();
            for (int i = 0; i < trailVector.Length; i++)
            {
                if (trailVector[i] != Vector2.Zero)
                {
                    trailCache.Add(armCenter + trailVector[i] * 1);
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
                Color color = BaseUtility.MultiLerpColor(factor.X, baseColor, endColor);
                float opacity = MathHelper.Clamp(2 - 2f * progress, 0, 1);
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
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();

            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            Vector2 drawPos = armCenter + positionVector.SafeNormalize(default) * 60 * Projectile.scale;

            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, drawPos - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public void DrawSlash()
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D Swing = Request<Texture2D>("Redemption/Textures/BladeSwing").Value;
            float radialOffset = Projectile.spriteDirection == -1 ? 5 * MathHelper.PiOver4 : -1 * MathHelper.PiOver4;
            float opacityAlt = Projectile.ai[0] == 2 ? 100f : 20f;
            float rotation = Rot + startVector.ToRotation() + radialOffset;

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);
            Rectangle rectangle = Swing.Frame(1, 4);
            Vector2 origin2 = rectangle.Size() / 2f;
            float opacity = 1 / (1 + opacityAlt * ((progress - 0.1f) * (progress - 0.1f)));

            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 0), RedeColor.NebColour * opacity * 0.5f, rotation + Owner.direction * 0.1f, origin2, Projectile.scale * 1.3f, spriteEffects, 0f);
            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 1), RedeColor.NebColour * opacity * 0.3f, rotation + Owner.direction * 0.01f, origin2, Projectile.scale * 1.3f, spriteEffects, 0f);
            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 2), RedeColor.NebColour * opacity * 0.7f, rotation + Owner.direction * 0.1f, origin2, Projectile.scale * 1.32f, spriteEffects, 0f);
            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 3), new Color(204, 153, 255) * 0.7f * opacity, rotation + Owner.direction * 0.01f, origin2, Projectile.scale * 1.3f, spriteEffects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
    }
}