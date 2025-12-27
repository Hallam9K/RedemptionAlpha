using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Projectiles.Ranged;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class FlakCannon_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Ranged/FlakCannon";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flak Cannon");
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        private float offset;
        private float shake;
        bool firstShot;
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

            offset -= 6;
            if (!player.channel)
            {
                if (Projectile.localAI[0]++ == 0)
                {
                    if (player.PickAmmo(player.HeldItem, out int grenade, out float shootSpeed, out int weaponDamage, out float weaponKnockback, out int usedAmmoId, firstShot))
                    {
                        firstShot = true;
                        switch (grenade)
                        {
                            case ProjectileID.Grenade:
                                grenade = ProjectileType<FlakGrenade>();
                                break;
                            case ProjectileID.BouncyGrenade:
                                grenade = ProjectileType<FlakGrenade_Bouncy>();
                                break;
                            case ProjectileID.StickyGrenade:
                                grenade = ProjectileType<FlakGrenade_Sticky>();
                                break;
                            case ProjectileID.Beenade:
                                grenade = ProjectileType<FlakGrenade_Bee>();
                                break;
                        }
                        
                        Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(36 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-9, Projectile.rotation + MathHelper.PiOver2);
                        Vector2 gunSmokePos = Projectile.Center + RedeHelper.PolarVector(66 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-19, Projectile.rotation + MathHelper.PiOver2);
                        for (int i = 0; i < 5; i++)
                        {
                            int num5 = Dust.NewDust(gunSmokePos, 6, 22, DustID.Smoke, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                            Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                            Main.dust[num5].velocity *= 0.66f;
                            Main.dust[num5].noGravity = true;
                            Main.dust[num5].scale = 1.4f;
                        }
                        for (int i = 0; i < 15; i++)
                        {
                            int num5 = Dust.NewDust(gunSmokePos, 6, 22, DustID.Wraith, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, Scale: 2);
                            Main.dust[num5].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                            Main.dust[num5].velocity *= 3f;
                            Main.dust[num5].noGravity = true;
                        }
                        offset = 30;
                        SoundEngine.PlaySound(SoundID.Item61, Projectile.position);

                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, Projectile.velocity * 20, grenade, Projectile.damage, Projectile.knockBack, player.whoAmI);
                    }
                }

                if (Projectile.localAI[0] >= 4 && Projectile.localAI[1] >= 30)
                {
                    Projectile.localAI[1] -= 20;
                    Projectile.localAI[0] = 0;
                }

                if (Projectile.localAI[0] >= 33)
                    Projectile.Kill();
            }
            else
            {
                Projectile.localAI[1] += player.GetAttackSpeed(DamageClass.Ranged);
                if (Projectile.localAI[1] >= 30)
                    shake += 0.02f;
                if (Projectile.localAI[1] >= 140 / player.GetAttackSpeed(DamageClass.Ranged) && !Main.dedServ && !fullcharge)
                {
                    SoundEngine.PlaySound(CustomSounds.ShootChange, Projectile.position);
                    fullcharge = true;
                }
                Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
            }
            shake = MathHelper.Min(shake, 0.8f);
            Projectile.localAI[1] = MathHelper.Min(Projectile.localAI[1], 140);
            offset = MathHelper.Clamp(offset, 0, 40);
            if (Projectile.ai[1]++ > 1)
                Projectile.alpha = 0;
        }
        public bool fullcharge;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-36 + offset, Projectile.velocity.ToRotation());

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition,
                null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
