using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Textures;
using ReLogic.Content;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class FoldedShotgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 20;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CustomSounds.HLShotgun1;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<FoldedShotgun_Proj>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class FoldedShotgun_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Ranged/FoldedShotgun";
        public override void SetSafeDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }
        private float offset;
        private float rotOffset;
        private int bullet = 1;
        private bool flashEffect;
        Vector2 heldOffset;
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
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            Vector2 gunPos = Projectile.Center;
            Vector2 gunSmokePos = Projectile.Center + RedeHelper.OffsetWithRotation(Projectile, 51, -8);

            offset -= 5;
            rotOffset += 0.04f;

            float shootingSpeed = 1 / player.GetAttackSpeed(DamageClass.Ranged);

            if (Projectile.localAI[0]++ > 0 && Main.myPlayer == Projectile.owner)
            {
                if (!player.channel && Projectile.localAI[1] >= (int)(player.inventory[player.selectedItem].useTime * shootingSpeed))
                    Projectile.Kill();
                else
                {
                    if (Projectile.localAI[1]++ % (int)(player.inventory[player.selectedItem].useTime * shootingSpeed) == 0)
                    {
                        int weaponDamage = Projectile.damage;
                        float weaponKnockback = Projectile.knockBack;
                        float shootSpeed = player.inventory[player.selectedItem].shootSpeed;
                        if (Projectile.UseAmmo(AmmoID.Bullet, ref bullet, ref shootSpeed, ref weaponDamage, ref weaponKnockback))
                        {
                            flashEffect = true;
                            offset = 15;
                            rotOffset = -0.5f;

                            player.RedemptionScreen().ScreenShakeIntensity += 3;
                            player.velocity -= RedeHelper.PolarVector(2, (Main.MouseWorld - player.Center).ToRotation());

                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.HLShotgun1, Projectile.position);

                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 vel = RedeHelper.PolarVector(Main.rand.NextFloat(3, 7) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.4f, 0.4f));
                                int num5 = Dust.NewDust(gunSmokePos - new Vector2(6), 12, 12, DustID.Smoke, 0, 0, Scale: 1f);
                                Main.dust[num5].velocity = vel;
                                Main.dust[num5].velocity *= 0.66f;
                                Main.dust[num5].noGravity = true;

                                vel = RedeHelper.PolarVector(Main.rand.NextFloat(4, 16) * Projectile.spriteDirection, Projectile.rotation + Main.rand.NextFloat(-0.3f, 0.3f));
                                num5 = Dust.NewDust(gunSmokePos - new Vector2(6), 12, 12, DustID.Smoke, 0, 0, Scale: 2f);
                                Main.dust[num5].velocity = vel;
                                Main.dust[num5].velocity *= 0.66f;
                                Main.dust[num5].noGravity = true;

                                num5 = Dust.NewDust(gunSmokePos - new Vector2(6), 12, 12, DustID.Torch, 0, 0, Scale: 2f);
                                Main.dust[num5].velocity = vel;
                                Main.dust[num5].velocity *= 0.66f;
                                Main.dust[num5].noGravity = true;
                            }

                            int numberProjectiles = 4 + Main.rand.Next(3);
                            for (int i = 0; i < numberProjectiles; i++)
                            {
                                Vector2 perturbedSpeed = RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation()).RotatedByRandom(MathHelper.ToRadians(15));
                                float scale = 1f - (Main.rand.NextFloat() * .5f);
                                perturbedSpeed *= scale;
                                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, perturbedSpeed, bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                            }
                            RedeHelper.NPCRadiusDamage(Projectile.Center + RedeHelper.OffsetWithRotation(Projectile, 52, -5), 60, Projectile, Projectile.damage * 2, Projectile.knockBack);
                        }
                    }
                }
            }
            heldOffset = RedeHelper.PolarVector(offset, Projectile.velocity.ToRotation());
            Projectile.Center = vector;
            Projectile.spriteDirection = Projectile.direction;
            float num = 0;
            if (Projectile.spriteDirection == -1)
                num = MathHelper.Pi;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;

            offset = MathHelper.Clamp(offset, 0, 20);
            rotOffset = MathHelper.Clamp(rotOffset, -1, 0);

            if (flashEffect)
            {
                if (++Projectile.frameCounter >= 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 3)
                    {
                        flashEffect = false;
                        Projectile.frame = 0;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }
        Asset<Texture2D> flash;
        public override bool PreDraw(ref Color lightColor)
        {
            flash ??= ModContent.Request<Texture2D>(Texture + "_Flash");
            Rectangle flashRect = flash.Frame(1, 3, 0, Projectile.frame);
            Vector2 flashOrigin = new(flashRect.Width / 2 - (76 * Projectile.spriteDirection), flashRect.Height / 2 + 8);

            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Vector2 drawOrigin = new(texture.Width() / 2 - (19 * Projectile.spriteDirection), texture.Height() / 2 + 4);
            Vector2 pos = Projectile.Center - heldOffset;

            if (flashEffect)
                Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, flashRect, Projectile.GetAlpha(Color.White), Projectile.rotation, flashOrigin, Projectile.scale, spriteEffects, 0);

            Main.EntitySpriteDraw(texture.Value, pos - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}