using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Effects.Trails;
using Redemption.Effects.Trails.Tips;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Projectiles.Ranged;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara_Sword : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukonvasara");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 92;
            Projectile.height = 92;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.scale = 1.5f;
            Projectile.extraUpdates = 4;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;

            InitializeTrail();
        }
        public Player Owner => Main.player[Projectile.owner];
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[2];
        public ref float SwingAngle => ref Projectile.ai[0];
        public ref float LockedDir => ref Projectile.ai[1];
        public float[] oldrot = new float[8];
        public Vector2[] oldPos = new Vector2[8];
        public Vector2 startVector;
        public Vector2 positionVector;
        public float SwingSpeed;
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

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(-Owner.direction * 4, -4);

            Owner.direction = (int)LockedDir;
            Projectile.spriteDirection = Owner.direction;

            if (--pauseTimer <= 0)
            {
                progress = Timer / (maxTime * 6 * Projectile.MaxUpdates);
                if (Timer++ == 0)
                {
                    Projectile.scale = 1.5f * Projectile.ai[2];
                    Length = 70 * Projectile.scale;
                    startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                }
                if (Timer == (int)(maxTime * 3 * Projectile.MaxUpdates))
                {
                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Pitch = -.6f }, Owner.position);
                        SoundEngine.PlaySound(CustomSounds.Swing1 with { Pitch = -.6f }, Owner.position);
                    }
                }
                if (progress < 1f)
                {
                    Rot = MathHelper.ToRadians(60 + 100f * MathF.Atan(6.5f * MathHelper.Pi * (progress - 0.5f))) * Projectile.spriteDirection;
                    positionVector = startVector.RotatedBy(Rot) * Length;
                }
                else
                    Projectile.Kill();
            }

            Projectile.rotation = positionVector.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = armCenter + positionVector;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, positionVector.ToRotation() + MathHelper.PiOver2);

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = Projectile.rotation;
            oldPos[0] = positionVector;

            if (Main.netMode != NetmodeID.Server && --pauseTimer <= 0)
            {
                TrailSetUp();
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (progress > 0.43f && progress < 0.75f)
                return null;
            return false;
        }
        public bool strike;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            if (!strike)
            {
                strike = true;
                SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = .2f }, Projectile.position);
                Owner.RedemptionScreen().ScreenShakeIntensity += 8;
                pauseTimer = maxTime / 10 * Projectile.MaxUpdates;
            }
            Vector2 directionTo = target.DirectionTo(Owner.Center);
            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(target.Center + directionTo * 5 + new Vector2(0, 70) + Owner.velocity, DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f) + 3.14f) * Main.rand.NextFloat(4f, 5f) + (Owner.velocity / 2), 0, Color.White * .8f, 3f);

        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            Vector2 drawPos = armCenter + positionVector * 0.8f;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos2 = armCenter + oldPos[k] * 0.8f;
                Color color = Color.LightCyan with { A = 0 } * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos2 - Main.screenPosition, null, color * 3, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, drawPos - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.LightYellow), Projectile.rotation, origin, Projectile.scale, spriteEffects);
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }       
        #region draw trail
        private Vector2[] oldDirVector = new Vector2[60];
        private List<Vector2> directionVectorCache = new();
        private List<Vector2> positionCache = new();
        private DanTrail trail;
        public Color baseColor = Color.LightYellow * .7f;
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
                    oldDirVector.SetValue(positionVector, i);
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
            oldDirVector[0] = positionVector;

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageSwordTrailPosition(oldDirVector.Length, armCenter, oldDirVector, ref directionVectorCache, ref positionCache, 1.2f);
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
                if (mult < 1f)
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
    public class Ukonvasara_Sword_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Melee/Ukonvasara_Sword";
        public override void SetStaticDefaults()
        {
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.Redemption().TechnicallyMelee = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Player p = Main.player[Projectile.owner];
            p.HeldItem.noMelee = true;
            p.HeldItem.noUseGraphic = true;
            p.HeldItem.useStyle = ItemUseStyleID.Shoot;
            p.itemTime = 2;
            p.itemAnimation = 2;
            if (drawDelay++ > 1)
                draw = true;

            if (!Owner.channel && thrown)
                Teleport();

            BoomerangAI();
        }

        public int shootDelay;
        public Vector2 mousePos;
        public Vector2 mouseWorld;
        public bool thrown;
        public bool throwing;
        public int throwTimer;
        public int drawDelay;
        public bool draw;
        public ref float ShootDelay => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        private void BoomerangAI()
        {
            if (Main.myPlayer == Projectile.owner && mousePos == Vector2.Zero)
                mousePos = Owner.DirectionTo(Main.MouseWorld);

            Vector2 armPos = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            armPos += Utils.SafeNormalize(Projectile.velocity, Vector2.UnitX) * 30f;

            if (!thrown)
            {
                throwTimer++;
                if (Main.myPlayer == Owner.whoAmI)
                    Owner.ChangeDir(Main.MouseWorld.X < Owner.Center.X ? -1 : 1);

                Owner.heldProj = Projectile.whoAmI;
                Owner.itemTime = 2;
                Owner.itemAnimation = 2;

                Projectile.timeLeft = 2;
                Projectile.rotation = Utils.ToRotation(Projectile.velocity);
                Owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);

                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

                Projectile.Center = armPos - new Vector2(25 * Owner.direction, 25);
            }

            if (throwTimer < 10)
            {
                float progress = EaseFunction.EaseCircularInOut.Ease(throwTimer / 10f);
                Projectile.velocity = 2f * Vector2.One.RotatedBy(mousePos.ToRotation() + MathHelper.Lerp(0f, -1.85f, progress) * Owner.direction - 1 * MathHelper.PiOver4);
            }
            else if (throwTimer < 20)
            {
                float progress = EaseFunction.EaseCircularInOut.Ease((throwTimer - 10f) / 10f);
                Projectile.velocity = 2f * Vector2.One.RotatedBy(mousePos.ToRotation() + MathHelper.Lerp(-1.85f, 0.35f, progress) * Owner.direction - 1 * MathHelper.PiOver4);
            }
            else
            {
                if (!thrown)
                {
                    Projectile.friendly = true;
                    SoundEngine.PlaySound(CustomSounds.ElectricSlash2, Owner.position);

                    if (Projectile.owner == Main.myPlayer)
                        Projectile.velocity = Owner.DirectionTo(Main.MouseWorld) * 25f;

                    offset = Projectile.velocity * 0.6f;
                    Projectile.timeLeft = 2400;
                    Projectile.width = Projectile.height = 10;
                    Projectile.ignoreWater = false;
                    Projectile.tileCollide = true;
                    ShootDelay = 0;
                    thrown = true;
                }

                if (++ShootDelay >= 15)
                {
                    Owner.heldProj = Projectile.whoAmI;
                    ClampVelocity();
                }

                if (ShootDelay > 0 && ShootDelay < 50)
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (Main.rand.NextBool(5) && !teleport)
                {
                    for (int i = 0; i < 2; i++)
                        DustHelper.DrawParticleElectricity(Projectile.Center - new Vector2(20 * Projectile.direction, 0), Projectile.Center - new Vector2(20 * Projectile.direction, 0) + RedeHelper.PolarVector(Main.rand.Next(70, 121), RedeHelper.RandomRotation()), .8f, 10, 0.2f);
                }

                Owner.ChangeDir(Projectile.Center.X < Owner.Center.X ? -1 : 1);
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.Center.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);
            }
        }

        public bool travel;
        public bool stopped;
        public float stationaryTime;
        public float thrownTime;
        public float adjust = 0.1f;
        private void ClampVelocity()
        {
            Vector2 pos = Projectile.Center;

            float betweenX = Owner.Center.X - pos.X;
            float betweenY = Owner.Center.Y - pos.Y;

            float distance = (float)Math.Sqrt(betweenX * betweenX + betweenY * betweenY);
            float pureDistance = distance;
            float speed = 20f;

            if (distance > 3000f)
                Projectile.Kill();

            distance = speed / distance;
            betweenX *= distance;
            betweenY *= distance;

            thrownTime++;

            if (pureDistance > 700 && !travel)
            {
                travel = true;
                stopped = true;
            }

            if (thrownTime > 15 && !travel)
            {
                travel = true;
                stopped = true;
            }

            if (stationaryTime >= 60)
            {
                stopped = false;
                travel = true;
                adjust = 1f;
                if (Projectile.tileCollide)
                    Projectile.tileCollide = false;
            }

            if (stopped && travel)
            {
                Projectile.velocity *= 0.8f;
                stationaryTime++;
            }

            Projectile.rotation += 0.3f * MathF.Pow(stationaryTime / 60, 2);

            if ((!travel && !stopped) || (travel && !stopped))
            {
                if (Projectile.velocity.X < betweenX)
                {
                    Projectile.velocity.X += adjust;

                    if (Projectile.velocity.X < 0f && betweenX > 0f)
                        Projectile.velocity.X += adjust;
                }
                else if (Projectile.velocity.X > betweenX)
                {
                    Projectile.velocity.X -= adjust;

                    if (Projectile.velocity.X > 0f && betweenX < 0f)
                        Projectile.velocity.X -= adjust;
                }
                if (Projectile.velocity.Y < betweenY)
                {
                    Projectile.velocity.Y += adjust;

                    if (Projectile.velocity.Y < 0f && betweenY > 0f)
                        Projectile.velocity.Y += adjust;
                }
                else if (Projectile.velocity.Y > betweenY)
                {
                    Projectile.velocity.Y -= adjust;

                    if (Projectile.velocity.Y > 0f && betweenY < 0f)
                        Projectile.velocity.Y -= adjust;
                }
            }

            if (Vector2.Distance(Projectile.Center, Owner.Center) < 20f && travel && !stopped && !teleport)
                Projectile.Kill();
        }

        public float teleportTimer;
        public Vector2 originalPos;
        public Vector2 teleportPos;
        public List<Vector2> teleportPositions = new();
        public bool teleport;
        public float playerDir;
        public float swingAngle;
        public void Teleport()
        {
            Projectile.alpha = 255;
            teleport = true;
            if (teleportTimer++ == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    int q = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + new Vector2(10, 0), Vector2.Zero, ProjectileType<UkonArrowStrike>(), Projectile.damage * 3, Projectile.knockBack, Main.myPlayer, 0, 1);
                    Main.projectile[q].DamageType = DamageClass.Melee;
                    Main.projectile[q].localAI[0] = 35;
                    Main.projectile[q].alpha = 0;
                    Main.projectile[q].position.Y -= 540;
                    Main.projectile[q].frame = 12;
                    Main.projectile[q].netUpdate = true;
                    playerDir = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
                    swingAngle = Main.MouseWorld.DirectionFrom(Owner.Center).ToRotation();
                }
                originalPos = Owner.Center;
                teleportPos = Projectile.Center;
            }
            if (teleportTimer == 1)
            {
                DoTeleport(teleportPos);

                Tile tile = Main.tile[(int)Owner.Bottom.X / 16, (int)Owner.Bottom.Y / 16];
                if (tile.HasTile && Main.tileSolid[tile.TileType] && !TileID.Sets.Platforms[tile.TileType] && !tile.IsActuated)
                    Owner.position -= new Vector2(0f, Owner.height);

                tile = Main.tile[(int)Owner.Top.X / 16, (int)Owner.Top.Y / 16];
                if (tile.HasTile && Main.tileSolid[tile.TileType] && !TileID.Sets.Platforms[tile.TileType] && !tile.IsActuated)
                    Owner.position += new Vector2(0f, Owner.height);

                teleportPos = Owner.position;
                float step = 0.1f;
                for (int i = 0; i < 1 / step; i++)
                {
                    var stepPos = Vector2.Lerp(originalPos, Owner.Center, step * i);
                    teleportPositions.Add(stepPos);
                }
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = 0.1f }, Projectile.position);
                //Main.NewLightning();
                if (Main.myPlayer == Projectile.owner)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + new Vector2(10, 0), Vector2.Zero, ProjectileType<UkonArrowStrike>(), Projectile.damage * 3, Projectile.knockBack, Main.myPlayer, 0, 1);
                    Main.projectile[p].DamageType = DamageClass.Melee;
                    Main.projectile[p].localAI[0] = 35;
                    Main.projectile[p].alpha = 0;
                    Main.projectile[p].position.Y -= 540;
                    Main.projectile[p].frame = 12;
                    Main.projectile[p].netUpdate = true;
                }
                Owner.RedemptionScreen().ScreenShakeIntensity += 5;
            }
            if (teleportTimer == 2 && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), teleportPos, RedeHelper.PolarVector(5, swingAngle), ProjectileType<Ukonvasara_Sword>(), Projectile.damage * 6, Projectile.knockBack, Projectile.owner, swingAngle, playerDir, Projectile.ai[2]);

            if (teleportTimer == 3)
                Projectile.friendly = false;
            if (teleportTimer >= 60)
                Projectile.Kill();
        }
        public void DoTeleport(Vector2 targetPosition)
        {
            int num = 150;
            Vector2 vector = Owner.position;
            Vector2 vector2 = Owner.velocity;
            for (int i = 0; i < num; i++)
            {
                vector2 = (vector + Owner.Size / 2f).DirectionTo(targetPosition).SafeNormalize(Vector2.Zero) * 12f;
                Vector2 vector3 = Collision.TileCollision(vector, vector2, Owner.width, Owner.height, fallThrough: true, fall2: true, (int)Owner.gravDir);
                vector += vector3;
            }
            Owner.Teleport(vector, -1);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Owner.whoAmI, vector.X, vector.Y, -1);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (thrown)
            {
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;

                Projectile.velocity.Y -= 2.5f;
                Projectile.velocity *= 0.8f;
            }
            return false;
        }

        public Vector2 offset;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Helper.CheckCircularCollision(Projectile.Center - offset, 40, targetHitbox))
                return true;

            for (int i = 0; i < teleportPositions.Count; i++)
            {
                if (Vector2.Distance(teleportPositions[i], targetHitbox.Center.ToVector2()) < 50f)
                    return true;
            }

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            target.AddBuff(BuffType<ElectrifiedDebuff>(), 180);
            Projectile.localNPCImmunity[target.whoAmI] = 8;
            target.immune[Projectile.owner] = 0;
        }

        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(30, (Projectile.Center - Player.Center).ToRotation());
            float num = !thrown && Player.direction == -1 ? MathHelper.Pi : 0;
            if (!teleport)
            {
                RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - v - Main.screenPosition,
                    new Rectangle?(rect), Projectile.GetAlpha(Color.LightYellow), Projectile.rotation + num, origin, Projectile.scale, spriteEffects);
                Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition,
                    new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation + num, origin, Projectile.scale, spriteEffects, 0);
            }

            //Dash effect
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D Dash = Request<Texture2D>("Redemption/Textures/Trails/Lightning2").Value;
            Vector2 DashCenter = 0.5f * (originalPos + teleportPos);
            Rectangle Dashrectangle = Dash.Frame();
            Vector2 Dashorigin = Dashrectangle.Size() / 2f;
            float DashRot = (originalPos - teleportPos).ToRotation() + 0.075f * Player.direction;
            float DashLength = (originalPos - teleportPos).Length();
            float opacity = 1 - teleportTimer / 60;
            Vector2 DashScale = new(DashLength * 0.004f, 0.5f);
            if (teleportTimer >= 2)
                Main.EntitySpriteDraw(Dash, DashCenter - Main.screenPosition, new Rectangle?(Dashrectangle), Color.White * 0.8f * opacity, DashRot, Dashorigin, DashScale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
            return false;
        }
    }
}
