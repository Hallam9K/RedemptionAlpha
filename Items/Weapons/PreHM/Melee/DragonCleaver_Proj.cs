using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Gores.Misc;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class DragonCleaver_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/DragonCleaver";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Cleaver");
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] >= 1;

        float oldRotation = 0f;
        int directionLock = 0;
        private float SwingSpeed;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
            {
                Projectile.Kill();
            }

            SwingSpeed = SetSwingSpeed(1);

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);
                    if (!player.channel)
                    {
                        Projectile.ai[0] = 1;
                        oldRotation = swordRotation;
                        directionLock = player.direction;
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;

                    Projectile.ai[0]++;

                    float timer = Projectile.ai[0] - 1;

                    swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / 20f / SwingSpeed);

                    if (Projectile.ai[0] >= 21 * SwingSpeed)
                        Projectile.Kill();

                    foreach (Projectile target in Main.projectile)
                    {
                        if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 100)
                            continue;

                        if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !ProjectileTags.Fire.Has(target.type) || ProjectileLists.IsTechnicallyMelee.Contains(target.type))
                            continue;

                        DustHelper.DrawCircle(target.Center, DustID.Torch, 1, 4, 4, nogravity: true);
                        target.Kill();
                    }
                }
            }

            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.Center = playerCenter + Projectile.velocity * 40f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.ai[0] == 0)
            {
                player.itemRotation = MathHelper.ToRadians(-90f);
                player.bodyFrame.Y = 5 * player.bodyFrame.Height;
            }
            else
                player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (NPCTags.Dragonlike.Has(target.type))
                damage *= 4;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<BuffPlayer>().dragonLeadBonus)
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}