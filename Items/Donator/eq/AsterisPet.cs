using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Weapons.PostML.Magic;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.eq
{
    public class AsterisPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 13;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(2, 6, 5)
                .WithOffset(2, 0).WithSpriteDirection(-1);
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 22;
            Projectile.height = 38;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            AIType = ProjectileID.BabyDino;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;

            if (Projectile.ai[0] == 1 || Projectile.velocity.Y != 0)
            {
                if (frameY < 7)
                    frameY = 7;
                if (frameCounter++ >= 5)
                {
                    frameCounter = 0;
                    if (++frameY > 12)
                        frameY = 7;
                }
            }
            else
            {
                Projectile.rotation = 0;

                if (Projectile.velocity.X == 0)
                    frameY = 0;
                else
                {
                    if (frameY < 1)
                        frameY = 1;
                    frameCounter += (int)Math.Abs(Projectile.velocity.X * 0.5f) + 1;
                    if (frameCounter >= 6)
                    {
                        frameCounter = 0;
                        if (++frameY > 6)
                            frameY = 1;
                    }
                }
            }
            return true;
        }
        bool unobtainLine;
        bool ribbonLine;
        bool xeniumStaffLine;
        bool skillIssued;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            if (!unobtainLine && player.HeldItem.ModItem is PlaceholderTile)
            {
                CombatText.NewText(Projectile.getRect(), Color.LightBlue, "I'm sure you shouldn't have it in your inventory...", true, false);
                unobtainLine = true;
            }
            if (!ribbonLine && player.HeldItem.type == ModContent.ItemType<GildedBonnet>())
            {
                CombatText.NewText(Projectile.getRect(), Color.LightBlue, "Oh, what a lovely ribbon you have over here.", true, false);
                ribbonLine = true;
            }
            if (!xeniumStaffLine && player.HeldItem.type == ModContent.ItemType<XeniumStaff>())
            {
                CombatText.NewText(Projectile.getRect(), Color.LightBlue, "Xenium Staff op, please nerf.", true, false);
                xeniumStaffLine = true;
            }
            if (player.dead && !skillIssued)
            {
                CombatText.NewText(Projectile.getRect(), Color.LightBlue, "skill issue smh", true, false);
                skillIssued = true;
            }
            else
                skillIssued = false;
        }
        private int frameY;
        private int frameCounter;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 13;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<AsterisPetBuff>()))
                Projectile.timeLeft = 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
    }
}