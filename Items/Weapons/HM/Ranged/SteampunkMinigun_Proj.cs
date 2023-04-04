using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class SteampunkMinigun_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Steam-Cog Minigun");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 28;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        private float offset;
        private float shake;
        private int bullet = 1;
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
            Projectile.Center = player.MountedCenter;
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

            offset -= 1;
            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel)
                {
                    if (Projectile.localAI[0]++ == 0)
                    {
                        if (++Projectile.frameCounter >= 2)
                        {
                            Projectile.frameCounter = 0;
                            if (++Projectile.frame >= 4)
                                Projectile.frame = 2;
                        }
                        int weaponDamage = Projectile.damage;
                        float weaponKnockback = Projectile.knockBack;
                        float shootSpeed = player.inventory[player.selectedItem].shootSpeed;
                        if (Projectile.UseAmmo(AmmoID.Bullet, ref bullet, ref shootSpeed, ref weaponDamage, ref weaponKnockback, !Main.rand.NextBool(3)))
                        {
                            if (bullet == ProjectileID.Bullet)
                                bullet = ProjectileID.BulletHighVelocity;

                            Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(25 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(2, Projectile.rotation + MathHelper.PiOver2);
                            Vector2 gunSmokePos = Projectile.Center + RedeHelper.PolarVector(49 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-3, Projectile.rotation + MathHelper.PiOver2);
                            for (int i = 0; i < 3; i++)
                            {
                                int num5 = Dust.NewDust(gunSmokePos, 4, 10, DustID.Smoke, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                                Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[num5].velocity *= 0.66f;
                                Main.dust[num5].noGravity = true;
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                int num5 = Dust.NewDust(gunSmokePos, 4, 10, DustID.Wraith, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                                Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[num5].velocity *= 3f;
                                Main.dust[num5].noGravity = true;
                            }
                            offset = 4;
                            player.RedemptionScreen().ScreenShakeIntensity += 1;
                            SoundEngine.PlaySound(SoundID.Item40, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f)), bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                        }
                    }
                    if (Projectile.localAI[0] >= 2 && Projectile.localAI[1] >= 2)
                    {
                        Projectile.localAI[1] -= 2;
                        Projectile.localAI[0] = 0;
                    }
                    if (Projectile.localAI[0] >= 10)
                        Projectile.Kill();
                }
                else
                {
                    if (++Projectile.frameCounter >= 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 2)
                            Projectile.frame = 0;
                    }

                    Projectile.localAI[1]++;
                    if (Projectile.localAI[1] >= 30)
                        shake += 0.01f;
                    if (Projectile.localAI[1] % 30 == 0)
                        SoundEngine.PlaySound(CustomSounds.WindUp, Projectile.position);
                    if (Projectile.localAI[1] >= 300)
                    {
                        player.channel = false;
                        SoundEngine.PlaySound(CustomSounds.ShootChange, Projectile.position);
                    }
                    Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
                }
            }
            shake = MathHelper.Min(shake, 0.8f);
            Projectile.localAI[1] = MathHelper.Min(Projectile.localAI[1], 300);
            offset = MathHelper.Clamp(offset, 0, 20);
            if (Projectile.ai[1]++ > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-30 + offset, Projectile.velocity.ToRotation());
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}