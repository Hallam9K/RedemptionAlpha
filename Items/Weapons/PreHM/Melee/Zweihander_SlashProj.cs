using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class Zweihander_SlashProj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zweihander");
            Main.projFrames[Projectile.type] = 10;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 222;
            Projectile.height = 156;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => false;

        public float SwingSpeed;
        int directionLock = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;

            SwingSpeed = SetSwingSpeed(30);

            Rectangle projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 70 : Projectile.Center.X), (int)(Projectile.Center.Y - 70), 70, 136);

            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                    player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    if (!player.channel)
                    {
                        Projectile.ai[0] = 1;
                        directionLock = player.direction;
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;
                    Projectile.ai[0]++;
                    if (Projectile.frame > 3)
                        player.itemRotation -= MathHelper.ToRadians(-20f * player.direction);
                    else
                        player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    if (++Projectile.frameCounter >= SwingSpeed / 10)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame is 4)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            player.velocity.X += 2 * player.direction;
                        }
                        if (Projectile.frame >= 4 && Projectile.frame <= 5)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile target = Main.projectile[i];
                                if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 100)
                                    continue;

                                if (target.velocity.Length() == 0 || !projHitbox.Intersects(target.Hitbox) ||
                                    target.Redemption().Unparryable || ProjectileTags.Unparryable.Has(target.type))
                                    continue;

                                SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
                                DustHelper.DrawCircle(target.Center, DustID.SilverCoin, 1, 4, 4, nogravity: true);
                                if (target.hostile || target.friendly)
                                {
                                    target.hostile = false;
                                    target.friendly = true;
                                }
                                target.damage *= 4;
                                target.velocity.X = -target.velocity.X * 0.8f;
                            }
                        }
                        if (Projectile.frame > 9)
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }

            Projectile.spriteDirection = player.direction;

            Projectile.Center = player.Center;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[Projectile.owner];
            float tipBonus;
            tipBonus = player.Distance(target.Center) / 3;
            tipBonus = MathHelper.Clamp(tipBonus, 0, 20);

            damage += (int)tipBonus;

            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 10;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int offset = Projectile.frame > 3 ? 14 : 0;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(38 * player.direction, 18 - offset) + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Texture2D slash = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PreHM/Melee/Zweihander_SlashProj2").Value;
            int height2 = slash.Height / 4;
            int y2 = height2 * (Projectile.frame - 3);
            Rectangle rect2 = new(0, y2, slash.Width, height2);
            Vector2 drawOrigin2 = new(slash.Width / 2, slash.Height / 2);

            if (Projectile.frame >= 3 && Projectile.frame <= 6)
                Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition - new Vector2(11 * player.direction, -192 - offset) + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 70 : Projectile.Center.X), (int)(Projectile.Center.Y - 70), 70, 136);
            return Projectile.frame is 4 && projHitbox.Intersects(targetHitbox);
        }
    }
}