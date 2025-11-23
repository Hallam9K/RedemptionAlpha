using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Redemption.Particles;
using Redemption.Projectiles.Ranged;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class DAN_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Ranged/DAN";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("D.A.N");
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 44;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        private float offset;
        private float rotOffset;
        private float spinRot;
        private float spinSpeed = 0.01f;
        private int bullet = 1;
        private float shake;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                float scaleFactor6 = 1f;
                if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;

                Vector2 vector13 = Main.MouseWorld - vector;
                if ((Projectile.localAI[1] == 0 && Projectile.localAI[0] > 80) || (Projectile.localAI[1] == 1 && Projectile.localAI[0] >= 120))
                    vector13 = RedeHelper.PolarVector(1, spinRot);

                vector13.Normalize();
                if (vector13.HasNaNs())
                    vector13 = Vector2.UnitX * player.direction;

                vector13 *= scaleFactor6;
                if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;

                Projectile.velocity = vector13;
                if (player.noItems || player.CCed || player.dead || !player.active)
                    Projectile.Kill();

                Projectile.netUpdate = true;
            }
            Projectile.Center = vector;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            float num = 0;
            if (Projectile.spriteDirection == -1)
                num = MathHelper.Pi;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;

            Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(45 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-5, Projectile.rotation + MathHelper.PiOver2);
            Vector2 gunSmokePos = Projectile.Center + RedeHelper.PolarVector(45 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-12, Projectile.rotation + MathHelper.PiOver2);
            offset -= 6;
            rotOffset += 0.1f;
            if (Projectile.localAI[0]++ == 2 || Projectile.localAI[0] == (int)(30 / player.GetAttackSpeed(DamageClass.Ranged)))
            {
                if (player.PickAmmo(player.HeldItem, out bullet, out float shootSpeed, out int weaponDamage, out float weaponKnockback, out int usedAmmoId, !Main.rand.NextBool(3)))
                {
                    int shotNum = 1;
                    for (int i = 0; i < Main.rand.Next(6, 8); i++)
                    {
                        if (player.PickAmmo(player.HeldItem, out bullet, out shootSpeed, out weaponDamage, out weaponKnockback, out usedAmmoId, !Main.rand.NextBool(3)))

                            shotNum++;
                    }
                    offset = 30;
                    rotOffset = -0.5f;
                    SoundEngine.PlaySound(SoundID.Item38, Projectile.position);
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.DANShot, Projectile.position);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        for (int i = 0; i < shotNum; i++)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(player.inventory[player.selectedItem].shootSpeed + Main.rand.Next(-4, 5), (Main.MouseWorld - gunPos).ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ProjectileType<DAN_Bullet>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int num5 = Dust.NewDust(gunSmokePos, 8, 20, DustID.Smoke, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, Scale: 2);
                        Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                        Main.dust[num5].velocity *= 1.5f;
                        Main.dust[num5].noGravity = true;
                        Main.dust[num5].scale = 1.4f;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        int num5 = Dust.NewDust(gunSmokePos, 8, 20, DustID.Wraith, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, Scale: 2);
                        Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                        Main.dust[num5].velocity *= 2f;
                        Main.dust[num5].noGravity = true;
                    }
                    player.RedemptionScreen().ScreenShakeIntensity += 6;

                    if (Projectile.owner == Main.myPlayer)
                        player.velocity -= RedeHelper.PolarVector(3, (Main.MouseWorld - player.Center).ToRotation());
                }
            }
            if (Projectile.localAI[0] == (int)(80 / player.GetAttackSpeed(DamageClass.Ranged)))
            {
                if (player.channel && player.GetModPlayer<EnergyPlayer>().statEnergy >= 15 && (Projectile.rotation < MathHelper.Pi - 0.8f + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0) && Projectile.rotation > 0.8f + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0)))
                {
                    Projectile.localAI[1] = 1;
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.ShieldActivate, Projectile.position);
                }
                spinRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);
            }
            if (Projectile.localAI[0] >= 80 / player.GetAttackSpeed(DamageClass.Ranged))
            {
                if (!player.channel && (Projectile.localAI[1] == 0 || Projectile.localAI[0] < 120))
                {
                    Projectile.Kill();
                    return;
                }
                if (Projectile.localAI[1] == 0)
                {
                    spinRot += spinSpeed;
                    spinSpeed *= 1.03f;

                    int speed = 8;
                    if (Projectile.localAI[0] >= 140 / player.GetAttackSpeed(DamageClass.Ranged))
                        speed = 6;
                    if (Projectile.localAI[0] >= 160 / player.GetAttackSpeed(DamageClass.Ranged))
                        speed = 4;
                    if (Projectile.localAI[0] >= 180 / player.GetAttackSpeed(DamageClass.Ranged))
                        speed = 2;
                    if (Projectile.localAI[0] >= 100 / player.GetAttackSpeed(DamageClass.Ranged) && Projectile.localAI[0] % speed == 0)
                    {
                        if (player.PickAmmo(player.HeldItem, out bullet, out float shootSpeed, out int weaponDamage, out float weaponKnockback, out int usedAmmoId, !Main.rand.NextBool(10)))

                        {
                            offset = 12;
                            SoundEngine.PlaySound(SoundID.Item40, Projectile.position);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.DANShot with { Volume = 0.5f, Pitch = 0.2f }, Projectile.position);

                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(player.inventory[player.selectedItem].shootSpeed, Projectile.velocity.ToRotation()), ProjectileType<DAN_Bullet>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);

                            for (int i = 0; i < 5; i++)
                            {
                                int num5 = Dust.NewDust(gunSmokePos, 8, 20, DustID.Smoke, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                                Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[num5].velocity *= 0.66f;
                                Main.dust[num5].noGravity = true;
                                Main.dust[num5].scale = 1.4f;
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                int num5 = Dust.NewDust(gunSmokePos, 8, 20, DustID.Wraith, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, Scale: 2);
                                Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[num5].noGravity = true;
                            }
                            player.RedemptionScreen().ScreenShakeIntensity += 1;
                        }
                    }
                }
                else
                {
                    if (Projectile.localAI[0] < (int)(120 / player.GetAttackSpeed(DamageClass.Ranged)))
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Vector2 dustPos = Projectile.Center + RedeHelper.PolarVector(65 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-5, Projectile.rotation + MathHelper.PiOver2);
                            Vector2 dVector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            dVector.X = (float)(Math.Sin(angle) * 40);
                            dVector.Y = (float)(Math.Cos(angle) * 40);
                            RedeParticleManager.CreateLightningParticle(dustPos + dVector, (dustPos + dVector).DirectionTo(dustPos) * 10f, 3, RedeParticleManager.purpleColors);
                            RedeParticleManager.CreateLightningParticle(dustPos + dVector, (dustPos + dVector).DirectionTo(dustPos) * 10f, 2, RedeParticleManager.goldColors);
                        }
                        shake += 0.3f;
                        Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
                    }
                    if (Projectile.localAI[0] == (int)(120 / player.GetAttackSpeed(DamageClass.Ranged)))
                    {
                        offset = 30;
                        spinRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);
                        player.RedemptionScreen().ScreenShakeIntensity += 20;
                        player.GetModPlayer<EnergyPlayer>().statEnergy -= 15;
                        if (!Main.dedServ)
                        {
                            SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.MissileExplosion, Projectile.position);
                        }
                        if (Projectile.owner == Main.myPlayer)
                        {
                            player.velocity -= RedeHelper.PolarVector(6, (Main.MouseWorld - player.Center).ToRotation());

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(3, Projectile.velocity.ToRotation()), ProjectileType<DAN_Laser>(), Projectile.damage * 5, Projectile.knockBack, player.whoAmI, Projectile.whoAmI);
                        }
                        for (int i = 0; i < 15; i++)
                        {
                            int num5 = Dust.NewDust(gunSmokePos, 8, 20, DustID.Smoke, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                            Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                            Main.dust[num5].velocity *= 0.66f;
                            Main.dust[num5].noGravity = true;
                            Main.dust[num5].scale = 1.4f;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            int num5 = Dust.NewDust(gunSmokePos, 8, 20, DustID.Wraith, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, Scale: 2);
                            Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                            Main.dust[num5].noGravity = true;
                        }
                    }
                    if (Projectile.localAI[0] >= 160 / player.GetAttackSpeed(DamageClass.Ranged))
                        Projectile.Kill();
                }
            }
            shake = MathHelper.Min(shake, 3f);
            Projectile.localAI[1] = MathHelper.Min(Projectile.localAI[1], 140);
            offset = MathHelper.Clamp(offset, 0, 40);
            rotOffset = MathHelper.Clamp(rotOffset, -1, 0);
            spinSpeed = MathHelper.Clamp(spinSpeed, 0, 0.2f);
            if (Projectile.ai[1]++ > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-16 + offset, Projectile.velocity.ToRotation());

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - v - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
