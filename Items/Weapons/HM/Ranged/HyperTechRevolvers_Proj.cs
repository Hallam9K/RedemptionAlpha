using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Audio;
using System.Collections.Generic;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class HyperTechRevolvers_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hyper-Tech Revolver");
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.ai[0] == 1)
                behindNPCs.Add(index);
        }
        private float offset;
        private float rotOffset;
        private int bullet = 1;
        private bool swap;
        private bool reset;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1]++ == 0 && Projectile.ai[0] == 1)
                swap = true;

            bool playerBuff = player.HasBuff<RevolverTossBuff>() || player.HasBuff<RevolverTossBuff2>() || player.HasBuff<RevolverTossBuff3>();
            if (player.HasBuff<RevolverTossDebuff>() && !playerBuff)
            {
                if (Projectile.ai[0] == 1)
                {
                    Projectile.Kill();
                    return;
                }
            }
            if (player.HasBuff<RevolverTossDebuff>() && player.ownedProjectileCounts[ModContent.ProjectileType<HyperTechRevolvers_Proj>()] == 2)
            {
                Projectile.Kill();
                return;
            }
            if (!player.HasBuff<RevolverTossDebuff>() && player.ownedProjectileCounts[ModContent.ProjectileType<HyperTechRevolvers_Proj>()] == 1 && player.ownedProjectileCounts[ModContent.ProjectileType<HyperTechRevolvers_Proj2>()] == 0)
            {
                Projectile.Kill();
                return;
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<HyperTechRevolvers_Proj2>()] > 0)
            {
                if (playerBuff)
                    reset = true;
            }
            else if (reset)
            {
                Projectile.Kill();
                return;
            }

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

            Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(21 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-6, Projectile.rotation + MathHelper.PiOver2);
            if (Projectile.ai[0] == 1)
                gunPos = Projectile.Center + RedeHelper.PolarVector(15 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-12, Projectile.rotation + MathHelper.PiOver2);

            offset -= 5;
            rotOffset += 0.1f;
            if (Main.myPlayer == Projectile.owner)
            {
                int firerate = 10;
                if (player.HasBuff<RevolverTossBuff>())
                    firerate = 8;
                else if (player.HasBuff<RevolverTossBuff2>())
                    firerate = 6;
                else if (player.HasBuff<RevolverTossBuff3>())
                    firerate = 4;

                if (!player.channel)
                    Projectile.Kill();
                else
                {
                    if (++Projectile.localAI[1] % firerate == 0)
                    {
                        if (!swap)
                        {
                            int weaponDamage = Projectile.damage;
                            float weaponKnockback = Projectile.knockBack;
                            float shootSpeed = player.inventory[player.selectedItem].shootSpeed;
                            if (Projectile.UseAmmo(AmmoID.Bullet, ref bullet, ref shootSpeed, ref weaponDamage, ref weaponKnockback))
                            {
                                if (bullet == ProjectileID.Bullet)
                                    bullet = ProjectileID.NanoBullet;

                                offset = 15;
                                rotOffset = -0.6f;
                                SoundEngine.PlaySound(SoundID.Item41, Projectile.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation()), bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                            }
                        }
                        swap = !swap;
                    }
                }
            }
            offset = MathHelper.Clamp(offset, 0, 20);
            rotOffset = MathHelper.Clamp(rotOffset, -1, 0);
            if (Projectile.ai[1] > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-26 + offset, Projectile.velocity.ToRotation());
            Vector2 pos = Projectile.Center;
            if (Projectile.ai[0] == 1)
                pos = Projectile.Center - new Vector2(6 * Projectile.spriteDirection, 6);

            Main.EntitySpriteDraw(texture, pos - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, pos - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(Color.White), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
