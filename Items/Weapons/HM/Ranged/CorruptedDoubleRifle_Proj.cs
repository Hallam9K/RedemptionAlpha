using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Projectiles.Ranged;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class CorruptedDoubleRifle_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Ranged/CorruptedDoubleRifle";
        public override void SetSafeDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }
        private float offset;
        private float rotOffset;
        private float heatOpacity;
        private int bullet = 1;
        bool firstShot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            RedeProjectile.HoldOutProjBasics(Projectile, player, vector);
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(21 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-6, Projectile.rotation + MathHelper.PiOver2);

            offset -= 5;
            rotOffset += 0.1f;
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.HeldItem.ModItem is CorruptedDoubleRifle rifle)
                    heatOpacity = rifle.Count / 40f;

                if (!player.channel && Projectile.ai[0] is 0)
                    Projectile.Kill();
                else
                {
                    float shootingSpeed = 1 / player.GetAttackSpeed(DamageClass.Ranged);
                    if (++Projectile.localAI[1] % (int)(player.inventory[player.selectedItem].useTime * shootingSpeed) == 0)
                    {
                        if (player.PickAmmo(player.HeldItem, out bullet, out float shootSpeed, out int weaponDamage, out float weaponKnockback, out int usedAmmoId, !firstShot || Main.rand.NextBool(3)))
                        {
                            firstShot = true;
                            shootSpeed *= Projectile.ai[0] == 1 ? 1 : 0.1f;

                            if (bullet == ProjectileID.Bullet)
                                bullet = ProjectileID.BulletHighVelocity;

                            offset = 15;
                            rotOffset = -0.3f;
                            float increase = 1;
                            if (player.HeldItem.ModItem is CorruptedDoubleRifle rifle3)
                                increase = 1 + rifle3.Count * 0.01f;
                            if (Projectile.ai[0] is 1)
                            {
                                float Distance = (Main.MouseWorld - gunPos).Length();
                                Distance = MathF.Max(Distance, 16 * 3);
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PlasmaShot, player.position);
                                player.GetModPlayer<EnergyPlayer>().statEnergy -= 6;
                                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + RedeHelper.PolarVector(8, (player.Center - Main.MouseWorld).ToRotation() + MathHelper.PiOver2), RedeHelper.PolarVector(Distance / 120, (Main.MouseWorld - gunPos).ToRotation()), ProjectileType<CorruptedDoubleRifle_Beam>(), (int)(Projectile.damage * increase * 2), Projectile.knockBack, player.whoAmI, 1, Distance);
                                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + RedeHelper.PolarVector(2, (player.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2), RedeHelper.PolarVector(Distance / 120, (Main.MouseWorld - gunPos).ToRotation()), ProjectileType<CorruptedDoubleRifle_Beam>(), (int)(Projectile.damage * increase * 2), Projectile.knockBack, player.whoAmI, 1, Distance);
                                Projectile.ai[0] = 0;
                                if (player.HeldItem.ModItem is CorruptedDoubleRifle rifle2)
                                {
                                    rifle2.Count = 0;
                                    rifle2.Charged = false;
                                    rifle2.Ready = false;
                                }
                            }
                            else
                            {
                                SoundEngine.PlaySound(SoundID.Item36, Projectile.position);
                                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + RedeHelper.PolarVector(8, (player.Center - Main.MouseWorld).ToRotation() + MathHelper.PiOver2), RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation()), bullet, (int)(Projectile.damage * increase), Projectile.knockBack, player.whoAmI).GetGlobalProjectile<CorruptedDoubleRifleGlobal>().ShotFrom = true;
                                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + RedeHelper.PolarVector(2, (player.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2), RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation()), bullet, (int)(Projectile.damage * increase), Projectile.knockBack, player.whoAmI).GetGlobalProjectile<CorruptedDoubleRifleGlobal>().ShotFrom = true;
                            }
                        }
                    }
                }
            }
            Vector2 v = RedeHelper.PolarVector(-26 + offset, Projectile.velocity.ToRotation());
            if (heatOpacity >= .5f)
            {
                int dustIndex = Dust.NewDust(Projectile.position - v, Projectile.width, Projectile.height, DustID.Smoke);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity.X = 0f;
                Main.dust[dustIndex].velocity.Y = -2f;
            }
            if (heatOpacity >= 1f)
            {
                int dustIndex = Dust.NewDust(Projectile.position - v, Projectile.width, Projectile.height, DustID.Torch, Scale: 2);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity.X = 0f;
                Main.dust[dustIndex].velocity.Y = -2f;
            }
            Projectile.Center = vector;
            Projectile.spriteDirection = Projectile.direction;
            float num = 0;
            if (Projectile.spriteDirection == -1)
                num = MathHelper.Pi;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;

            offset = MathHelper.Clamp(offset, 0, 20);
            rotOffset = MathHelper.Clamp(rotOffset, -1, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 shake = RedeHelper.Spread(heatOpacity);
            Vector2 v = RedeHelper.PolarVector(-26 + offset, Projectile.velocity.ToRotation()) + shake;
            Vector2 pos = Projectile.Center;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            Main.EntitySpriteDraw(texture, pos - v - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, pos - v - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            Main.EntitySpriteDraw(texture, pos - v + RedeHelper.Spread(heatOpacity) - Main.screenPosition, null, Projectile.GetAlpha(Color.Orange) * (heatOpacity / 2), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
            return false;
        }
    }
}
