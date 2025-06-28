using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
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
        public ref float Case => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float Timer2 => ref Projectile.localAI[1];
        public ref float Charge => ref Projectile.localAI[2];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            RedeProjectile.HoldOutProjBasics(Projectile, player, vector);
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

            offset -= 1;
            switch (Case)
            {
                case 0:
                    if (++Projectile.frameCounter >= 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 2)
                            Projectile.frame = 0;
                    }

                    Charge++;
                    if (Charge >= (int)(30 / player.GetAttackSpeed(DamageClass.Ranged)))
                        shake += 0.01f;
                    if (Charge % (int)(30 / player.GetAttackSpeed(DamageClass.Ranged)) == 0 && !Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.WindUp, Projectile.position);
                    if (Charge >= (int)(90 / player.GetAttackSpeed(DamageClass.Ranged)))
                    {
                        Case = 1;
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.ShootChange, Projectile.position);
                    }
                    Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));

                    if (!player.channel)
                        Case = 1;
                    break;
                case 1:
                    if (Timer++ == 0)
                    {
                        if (++Projectile.frameCounter >= 2)
                        {
                            Projectile.frameCounter = 0;
                            if (++Projectile.frame >= 4)
                                Projectile.frame = 2;
                        }
                        if (player.PickAmmo(player.HeldItem, out bullet, out float shootSpeed, out int weaponDamage, out float weaponKnockback, out int usedAmmoId, !Main.rand.NextBool(3)))
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

                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f)), bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                        }
                    }
                    if (Timer >= 2 && Charge >= 1)
                    {
                        Charge -= 1;
                        Timer = 0;
                    }
                    if (Timer >= 10)
                        Projectile.Kill();

                    if (player.controlUseItem)
                    {
                        if (Timer2++ % (int)(30 / player.GetAttackSpeed(DamageClass.Ranged)) == 0)
                            SoundEngine.PlaySound(CustomSounds.WindUp, Projectile.position);
                        player.velocity.X *= 0.1f;
                        Charge++;
                    }
                    else
                        Timer2 = 0;

                    Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
                    break;
            }
            shake = MathHelper.Min(shake, 0.8f);
            Charge = MathHelper.Min(Charge, 90);
            offset = MathHelper.Clamp(offset, 0, 20);
            if (Projectile.ai[1]++ > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-30 + offset, Projectile.velocity.ToRotation());
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - v - Main.screenPosition,
                new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
