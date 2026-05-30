using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class TinyCleaver_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/TinyCleaver_Proj1";
        private static Asset<Texture2D> chainTexture;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            chainTexture = Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/TinyCleaver_Chain");
        }
        public override void Unload()
        {
            chainTexture = null;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tiny Cleaver");
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Length = 34;
            Rot = MathHelper.ToRadians(2);
            Projectile.alpha = 255;
            Projectile.extraUpdates = 5;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly && Projectile.ai[0] != 0 ? null : false;
        }
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 startVector;
        private Vector2 positionVector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float speed;
        private float SwingSpeed;
        public int pauseTimer;
        public float progress;
        public int maxTime;
        public override void AI()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();

            maxTime = SetUseTime(Owner.HeldItem.useTime);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Projectile.spriteDirection = Owner.direction;

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);

            if (--pauseTimer <= 0)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Timer == 0)
                            strike = false;

                        SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.4f }, Projectile.position);

                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                        positionVector = startVector * Length;
                        Projectile.ai[0] = 1;
                        Projectile.netUpdate = true;
                        break;
                    case 1:
                        progress = Timer / (maxTime * 1.5f * Projectile.MaxUpdates);
                        if (Timer++ == 0)
                            strike = false;
                        if (progress < 1)
                        {
                            Rot = MathHelper.ToRadians(30 + 120f * MathF.Atan(2f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                            positionVector = startVector.RotatedBy(Rot) * (Length + extendoLength * 7) * Owner.GetAdjustedItemScale(Owner.HeldItem);
                        }
                        else
                        {
                            if (!Owner.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;
                            startVector = positionVector.SafeNormalize(default);
                            Projectile.ai[0] = 2;
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.4f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        progress = Timer / (maxTime * 1.25f * Projectile.MaxUpdates);
                        if (Timer++ == 0)
                            strike = false;
                        if (progress < 1)
                        {
                            Rot = -MathHelper.ToRadians(30 + 120f * MathF.Atan(2f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                            extendoLength += 2f / (maxTime * Projectile.MaxUpdates / 12f);
                            positionVector = startVector.RotatedBy(Rot) * (Length + extendoLength * 7) * Owner.GetAdjustedItemScale(Owner.HeldItem);
                        }
                        else
                        {
                            if (!Owner.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;

                            Projectile.ai[0] = 3;
                            startVector = positionVector.SafeNormalize(default);
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.4f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 3:
                        progress = Timer / (maxTime * 1.25f * Projectile.MaxUpdates);
                        if (Timer++ == 0)
                            strike = false;
                        if (progress < 0.5f)
                            extendoLength += 3f / (maxTime * Projectile.MaxUpdates / 12f);
                        else
                            extendoLength -= 4f / (maxTime * Projectile.MaxUpdates / 12f);

                        if (progress < 1)
                        {
                            Rot = MathHelper.ToRadians(30 + 120f * MathF.Atan(2f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                            positionVector = startVector.RotatedBy(Rot) * (Length + extendoLength * 7) * Owner.GetAdjustedItemScale(Owner.HeldItem);
                        }
                        else
                        {
                            if (!Owner.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;

                            Projectile.ai[0] = 4;
                            startVector = positionVector.SafeNormalize(default);
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.5f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 4:
                        progress = Timer / (maxTime * 1.25f * Projectile.MaxUpdates);
                        if (Timer++ == 0)
                            strike = false;
                        if (progress < 0.2f)
                            extendoLength += 5f / (maxTime * Projectile.MaxUpdates / 12f);
                        if (progress > 0.8f && progress < 1)
                            extendoLength -= 10f / (maxTime * Projectile.MaxUpdates / 12f);

                        if (progress < 1)
                        {
                            Rot = -MathHelper.ToRadians(30 + 90f * MathF.Atan(2f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                            positionVector = startVector.RotatedBy(Rot) * (Length + extendoLength * 7) * Owner.GetAdjustedItemScale(Owner.HeldItem);
                        }
                        else
                        {
                            if (!Owner.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;
                            speed = 10;

                            Projectile.ai[0] = 5;

                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.BallCreate with { Volume = 0.3f, Pitch = 0.1f }, Projectile.position);

                            if (Projectile.owner == Main.myPlayer)
                            {
                                startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - armCenter).ToRotation());
                                if (Main.MouseWorld.X < Owner.Center.X)
                                    Owner.direction = -1;
                                else
                                    Owner.direction = 1;
                            }
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 5:
                        if (Timer == 0)
                            strike = false;

                        positionVector = startVector * (Length + extendoLength * 7) * Owner.GetAdjustedItemScale(Owner.HeldItem);

                        if (Timer++ < maxTime * Projectile.MaxUpdates)
                        {
                            extendoLength += speed / (maxTime * Projectile.MaxUpdates / 12f);
                            speed *= 0.96f;
                        }
                        else
                        {
                            extendoLength -= speed / (maxTime * Projectile.MaxUpdates / 12f);
                            speed *= 1.1f;
                        }
                        if (Timer >= maxTime * 2 * Projectile.MaxUpdates)
                        {
                            Projectile.Kill();
                        }
                        break;

                }
            }
            extendoLength = MathHelper.Clamp(extendoLength, 10, 80);
            if (Timer > 1)
                Projectile.alpha = 0;
            Projectile.Center = armCenter + positionVector;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = new(DelegateMethods.CutTiles);
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 unit = lineStart.DirectionTo(Projectile.Center);
            Vector2 lineEnd = lineStart + unit * ((extendoLength + 5) * extendo[7]);
            float height = 32 * Projectile.scale;
            Utils.PlotTileLine(lineStart, lineEnd, height, cut);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 lineStart = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 unit = lineStart.DirectionTo(Projectile.Center);
            Vector2 lineEnd = lineStart + unit * ((extendoLength + 5) * extendo[7]);
            float height = 24 * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, height, ref point))
                return true;
            else
                return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 5)
            {
                modifiers.FinalDamage *= 1.5f;
                if (target.knockBackResist > 0)
                    modifiers.Knockback.Flat += 4;
            }

        }
        public bool strike;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = .8f }, Projectile.position);
            if (!strike)
            {
                Owner.RedemptionScreen().ScreenShakeIntensity += 2;
                strike = true;
                pauseTimer = maxTime / 4;
            }
            Vector2 directionTo = target.DirectionTo(Owner.Center);
            for (int i = 0; i < 16; i++)
                Dust.NewDustPerfect(target.Center + directionTo * 5 + new Vector2(0, 35) + Owner.velocity, DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f) + 3.14f) * Main.rand.NextFloat(3f, 5f) + (Owner.velocity / 2), 0, Color.Red * .8f, 1.6f);

            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            Projectile.localNPCImmunity[target.whoAmI] = (int)(maxTime * Projectile.MaxUpdates);
            target.immune[Projectile.owner] = 0;
        }
        public float[] extendo = new float[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        public float extendoLength = 10;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D segMid = Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/TinyCleaver_Proj2").Value;
            Texture2D segEnd = Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/TinyCleaver_Proj3").Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            Vector2 drawPos = armCenter + positionVector.SafeNormalize(default) * 32 * Projectile.scale;

            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            for (int i = 0; i < 8; i++)
            {
                Main.EntitySpriteDraw(i < 7 ? segMid : segEnd, drawPos + RedeHelper.PolarVector(extendoLength * extendo[i], (Projectile.Center - armCenter).ToRotation()) - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            return false;
        }
        public override bool PreDrawExtras()
        {
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);
            Vector2 handleCenter = armCenter + positionVector.SafeNormalize(default) * 32 * Projectile.scale;
            Vector2 center = handleCenter + RedeHelper.PolarVector(extendoLength * extendo[7], (Projectile.Center - armCenter).ToRotation());
            Vector2 directionToHandle = handleCenter - center;
            float chainRotation = directionToHandle.ToRotation() - MathHelper.PiOver2;
            float distanceToHandle = directionToHandle.Length();

            while (distanceToHandle > 20f && !float.IsNaN(distanceToHandle))
            {
                directionToHandle /= distanceToHandle; //get unit vector
                directionToHandle *= chainTexture.Height(); //multiply by chain link length

                center += directionToHandle; //update draw position
                directionToHandle = handleCenter - center; //update distance
                distanceToHandle = directionToHandle.Length();

                //Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, Projectile.GetAlpha(Color.White), chainRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}