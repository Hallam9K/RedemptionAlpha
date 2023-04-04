using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lordfunnyman
{
    public class Lordfunnyman_Pet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lego Man");
            Main.projFrames[Projectile.type] = 13;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 8, 5)
                .WithOffset(2, 0).WithSpriteDirection(-1);
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 16;
            Projectile.height = 32;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            AIType = ProjectileID.BabyDino;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;

            if (Projectile.ai[0] == 1)
            {
                if (frameY < 8)
                    frameY = 8;
                if (frameCounter++ >= 3)
                {
                    frameCounter = 0;
                    if (++frameY > 12)
                        frameY = 8;
                }
            }
            else
            {
                Projectile.rotation = 0;

                if (Projectile.velocity.X == 0)
                    frameY = 0;
                else
                {
                    frameCounter += (int)Math.Abs(Projectile.velocity.X * 0.5f) + 1;
                    if (frameCounter >= 6)
                    {
                        frameCounter = 0;
                        if (++frameY > 7)
                            frameY = 1;
                    }
                }
            }
            return true;
        }
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

            Main.EntitySpriteDraw(texture, Projectile.Center - new Vector2(-4 * Projectile.spriteDirection, 8) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<LegoPetBuff>()))
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