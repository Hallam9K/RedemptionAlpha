using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Projectiles.Melee;
using Redemption.Projectiles.Ranged;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara_Axe : TrueMeleeProjectile, ITrailProjectile
    {
        public float[] oldrot = new float[6];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().IsAxe = true;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(117, 249, 253), new Color(180, 251, 253)), new RoundCap(), new DefaultTrailPosition(), 100f, 100f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_1", AssetRequestMode.ImmediateLoad).Value, 0.1f, 1f, 1f));
        }

        private Vector2 startVector;
        private Vector2 vector;
        public float Length;
        public float Rot;
        public float spinTimer;
        public float stopTimer;
        public float rollingTimer = 60f;
        public float OpacityTimer;

        public float acc;
        public float SwingSpeed;
        public float progress;
        public bool charged = false;
        public bool rotRight;
        private Player Owner => Main.player[Projectile.owner];
        public override void AI()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }
            if (!charged && !Owner.channel)
            {
                Projectile.Kill();
                return;
            }

            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Main.MouseWorld.X < Owner.Center.X ? -1 : 1);
            if (Main.MouseWorld.X < Owner.Center.X)
                rotRight = true;

            SwingSpeed = 1 / Owner.GetAttackSpeed(DamageClass.Melee);
            progress = acc / (120 * SwingSpeed);

            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver2;
            Projectile.Center = Owner.MountedCenter + vector;
            Projectile.timeLeft = 360;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Owner.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            if (Main.myPlayer == Projectile.owner)
            {
                acc++;
                if (spinTimer++ == 0)
                {
                    Projectile.scale *= Projectile.ai[2];
                    Length = 60 * Projectile.ai[2];
                    startVector = RedeHelper.PolarVector(1, MathHelper.PiOver2 * Projectile.spriteDirection);
                }

                spinTimer = MathHelper.Clamp(spinTimer, 0, 400);
                acc = MathHelper.Max(0, acc);
                Rot = MathHelper.ToRadians(spinTimer * MathHelper.Lerp(1f, 6f, progress)) * Projectile.spriteDirection;
                vector = startVector.RotatedBy(Rot) * Length;

                if (progress > 1f && !charged)
                {
                    DustHelper.DrawCircle(Owner.Center, DustID.Electric, 2, 2, 2, 1, 2, nogravity: true);
                    SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
                    charged = true;
                }
                if (charged && !Owner.channel && rollingTimer > 0)
                {
                    OpacityTimer++;
                    if (!Main.dedServ && rollingTimer == 60)
                        SoundEngine.PlaySound(CustomSounds.ElectricSlash2, Owner.position);

                    if (rollingTimer % 4 == 0)
                    {
                        int d = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<UkonSpark_Proj>(), Projectile.damage / 3, 0, Main.myPlayer);
                        Main.projectile[d].usesIDStaticNPCImmunity = false;
                        Main.projectile[d].usesLocalNPCImmunity = true;
                        Main.projectile[d].localNPCHitCooldown = 5;
                        Main.projectile[d].timeLeft = 60;
                        Main.projectile[d].noEnchantmentVisuals = true;
                    }

                    Owner.Redemption().contactImmune = true;
                    Owner.fullRotation -= rotRight ? 0.25f : -0.25f;
                    Owner.fullRotationOrigin = new Vector2(10, 20);

                    rollingTimer--;
                    Owner.AddBuff(ModContent.BuffType<HammerBuff2>(), 10);
                    Owner.Move(Main.MouseWorld, 50, 10);
                    Chop();

                    Vector2 position = Owner.Center + (Vector2.Normalize(Owner.velocity) * 30f);
                    Dust dust = Main.dust[Dust.NewDust(Owner.position, Owner.width, Owner.height, DustID.Electric)];
                    dust.position = position;
                    dust.velocity = (Owner.velocity.RotatedBy(1.57) * 0.2f) + (Owner.velocity / 8f);
                    dust.position += Owner.velocity.RotatedBy(1.57) * 0.5f;
                    dust.fadeIn = 0.5f;
                    dust.noGravity = true;
                    dust = Main.dust[Dust.NewDust(Owner.position, Owner.width, Owner.height, DustID.Electric)];
                    dust.position = position;
                    dust.velocity = (Owner.velocity.RotatedBy(-1.57) * 0.2f) + (Owner.velocity / 8f);
                    dust.position += Owner.velocity.RotatedBy(-1.57) * 0.5f;
                    dust.fadeIn = 0.5f;
                    dust.noGravity = true;
                }

                if (rollingTimer <= 1)
                    Owner.velocity *= 0.1f;
                if (rollingTimer <= 0)
                    Projectile.Kill();
            }

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }

        public bool chop;
        public void Chop()
        {
            if (Owner.controlDown && !chop)
            {
                rollingTimer += 60;
                chop = true;
            }

            if (chop)
            {
                Owner.velocity = new Vector2(0, 50);
                Tile tile = Main.tile[(int)Owner.Bottom.X / 16, (int)Owner.Bottom.Y / 16];
                if (tile.HasTile && Main.tileSolid[tile.TileType] || TileID.Sets.Platforms[tile.TileType] && Owner.velocity.Y > 70)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int k = -1; k < 2; k++)
                        {
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + new Vector2(k * 50, 0), Vector2.Zero, ModContent.ProjectileType<UkonArrowStrike>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 1);
                            Main.projectile[p].DamageType = DamageClass.Melee;
                            Main.projectile[p].localAI[0] = 35;
                            Main.projectile[p].alpha = 0;
                            Main.projectile[p].position.Y -= 540;
                            Main.projectile[p].frame = 12;
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = 0.6f }, Projectile.position);
                    Main.NewLightning();
                    rollingTimer = 0;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Helper.CheckCircularCollision(Owner.Center, 40, targetHitbox) || projHitbox.Intersects(targetHitbox))
                return true;
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (progress > 0.1f)
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 30;
            target.immune[Projectile.owner] = 0;
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 180);

            if (Main.myPlayer == Projectile.owner)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center + new Vector2(Main.rand.NextFloat(-2, 2)), Vector2.Zero, ModContent.ProjectileType<UkonArrowStrike>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 1);
                Main.projectile[p].DamageType = DamageClass.Melee;
                Main.projectile[p].localAI[0] = 35;
                Main.projectile[p].alpha = 0;
                Main.projectile[p].position.Y -= 540;
                Main.projectile[p].frame = 12;
                Main.projectile[p].netUpdate = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Owner.fullRotation = 0f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = Projectile.Center - RedeHelper.PolarVector(20, (Projectile.Center - Owner.Center).ToRotation());

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(20, 20);
                Color color = Projectile.GetAlpha(lightColor with { A = 0 }) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * 0.5f, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}