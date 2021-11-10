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
using Terraria.Graphics.Shaders;
using Redemption.Projectiles.Melee;
using Redemption.Base;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class ForgottenSword_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/ForgottenSword";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sword of the Forgotten");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        float oldRotation = 0f;
        int directionLock = 0;
        private float SwingSpeed;

        public override void AI()
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

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
                    swordRotation = MathHelper.ToRadians(130f * player.direction - 90f);

                    Projectile.ai[0] = 1;
                    oldRotation = swordRotation;
                    directionLock = player.direction;
                }
                if (Projectile.ai[0] >= 1)
                {
                    if (Projectile.ai[0] > 4)
                        Projectile.alpha = 0;
                    player.direction = directionLock;

                    Projectile.ai[0]++;

                    float timer = Projectile.ai[0] - 1;

                    swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(180f * player.direction - 90f), -timer / 9f / SwingSpeed);

                    if (Projectile.ai[0] >= 30 * SwingSpeed)
                        Projectile.Kill();
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
            }
            else
                player.itemRotation = (player.Center - Projectile.Center).ToRotation() + (player.direction == 1 ? MathHelper.Pi : 0);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.knockBackResist > 0)
                target.velocity.Y = -10 * target.knockBackResist;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color), oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}