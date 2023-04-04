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
using Redemption.Items.Weapons.HM.Ammo;
using Redemption.Projectiles.Ranged;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class SwarmerCannon_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Ranged/SwarmerCannon";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Swarmer Cannon");
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 36;
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
        private int hiveFrame;
        private float hiveScale = 0.01f;
        private bool hiveGrown;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++hiveFrame >= 4)
                    hiveFrame = 0;
            }
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

            offset -= 2;
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(44 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-2, Projectile.rotation + MathHelper.PiOver2);
                Vector2 gunSmokePos = Projectile.Center + RedeHelper.PolarVector(44 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-16, Projectile.rotation + MathHelper.PiOver2);

                if (!player.channel)
                {
                    if (hiveScale >= 1 && Projectile.localAI[1] == 1)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.position);
                        DustHelper.DrawCircle(gunPos, DustID.GreenFairy, 2, dustSize: 2, nogravity: true);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(30, (Main.MouseWorld - gunPos).ToRotation()), ModContent.ProjectileType<SwarmGrowth_Proj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                    }
                    if (Projectile.localAI[1]++ >= 2)
                        Projectile.Kill();
                }
                else
                {
                    if (Projectile.localAI[0]++ % 18 == 0)
                    {
                        int weaponDamage = Projectile.damage;
                        float weaponKnockback = Projectile.knockBack;
                        float shootSpeed = player.inventory[player.selectedItem].shootSpeed;
                        if (Projectile.UseAmmo(AmmoID.Bullet, ref bullet, ref shootSpeed, ref weaponDamage, ref weaponKnockback))
                        {
                            if (bullet == ProjectileID.Bullet)
                                bullet = ModContent.ProjectileType<BileBullet_Proj>();

                            for (int i = 0; i < 4; i++)
                            {
                                int num5 = Dust.NewDust(gunSmokePos, 4, 28, DustID.GreenBlood, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                                Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.08f, 0.08f));
                                Main.dust[num5].velocity *= 0.66f;
                                Main.dust[num5].noGravity = true;
                                num5 = Dust.NewDust(gunSmokePos, 4, 28, DustID.Clentaminator_Green, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                                Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.08f, 0.08f));
                                Main.dust[num5].velocity *= 3f;
                                Main.dust[num5].noGravity = true;
                            }
                            offset = 14;
                            player.RedemptionScreen().ScreenShakeIntensity += 1;
                            SoundEngine.PlaySound(SoundID.Item38, Projectile.position);
                            for (int i = 0; i < Main.rand.Next(3, 6); i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation() + Main.rand.NextFloat(-0.08f, 0.08f)), bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                            }
                        }
                    }
                    hiveScale += 0.01f;
                    if (hiveScale >= 0.5f)
                        shake += 0.04f;
                    if (hiveScale >= 1 && !hiveGrown)
                    {
                        DustHelper.DrawCircle(gunPos, DustID.GreenFairy, 2, dustSize: 2, nogravity: true);
                        SoundEngine.PlaySound(SoundID.NPCHit20, Projectile.position);
                        hiveGrown = true;
                    }
                    Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
                }
            }
            shake = MathHelper.Min(shake, 0.8f);
            offset = MathHelper.Clamp(offset, 0, 20);
            hiveScale = MathHelper.Clamp(hiveScale, 0.01f, 1f);
            if (Projectile.ai[1]++ > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Texture2D hive = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/SeedOfInfection/SeedGrowth").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            int height = hive.Height / 4;
            int y = height * hiveFrame;
            Rectangle hiveRect = new(0, y, hive.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 hiveOrigin = new(hive.Width / 2, height / 2);
            Vector2 v = RedeHelper.PolarVector(-20 + offset, Projectile.velocity.ToRotation());
            Vector2 hivePos = Projectile.Center + RedeHelper.PolarVector((44 - offset) * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-2, Projectile.rotation + MathHelper.PiOver2);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(hive, hivePos - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(hiveRect), Projectile.GetAlpha(lightColor), Projectile.rotation, hiveOrigin, Projectile.scale * hiveScale, spriteEffects, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}