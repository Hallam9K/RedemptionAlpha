using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.GameContent;
using Redemption.Base;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class OOFingergun_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Magic/OOFingergun";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Finger Gun");
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        public float offset;
        public float rotOffset;
        private int firerate = 15;
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

            Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(19 * Projectile.spriteDirection, Projectile.rotation) + RedeHelper.PolarVector(-2, Projectile.rotation + MathHelper.PiOver2);
            offset -= 3;
            rotOffset += 0.05f;
            if (Main.myPlayer == Projectile.owner)
            {
                float shootSpeed = player.inventory[player.selectedItem].shootSpeed;
                int mana = player.inventory[player.selectedItem].mana;
                if (Projectile.localAI[1]++ >= firerate && player.channel)
                {
                    if (BasePlayer.ReduceMana(player, mana))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(shootSpeed, (Main.MouseWorld - gunPos).ToRotation()), ModContent.ProjectileType<OOFingergun_Fingerflash>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.whoAmI);
                        Projectile.localAI[1] = 0;
                    }
                }
                if (firerate > 5 && Projectile.localAI[0]++ >= 60 && Projectile.localAI[0] % 40 == 0)
                    firerate--;
                if ((!player.channel && Projectile.localAI[0] >= 10) || !player.CheckMana(mana))
                    Projectile.Kill();
            }
            offset = MathHelper.Clamp(offset, 0, 40);
            rotOffset = MathHelper.Clamp(rotOffset, -1, 0);
            if (Projectile.ai[1]++ > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-24 + offset, Projectile.velocity.ToRotation());

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}