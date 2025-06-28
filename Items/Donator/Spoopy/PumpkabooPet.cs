using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Spoopy
{
    public class PumpkabooPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pumpkaboo");
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 4, 5)
                .WithOffset(-4, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 32;
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

            Lighting.AddLight(Projectile.Center, .74f, .85f, .4f);

            if (Projectile.ai[0] == 1)
            {
                rotation = Projectile.velocity.X * 0.1f;
                caneRotation += Projectile.velocity.X * 0.1f;
                frameY = 0;
            }
            else
            {
                caneRotation.SlowRotation(0, (float)Math.PI / 20);
                rotation = 0;

                if (Projectile.velocity.X == 0)
                    frameY = 0;
                else
                {
                    frameCounter += (int)Math.Abs(Projectile.velocity.X * 0.5f) + 1;
                    if (frameCounter >= 6)
                    {
                        frameCounter = 0;
                        if (++frameY >= 4)
                            frameY = 0;
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
        private float rotation;
        private float caneRotation;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D cane = ModContent.Request<Texture2D>(Texture + "_Cane").Value;
            int height = texture.Height / 4;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Rectangle rectCane = new(0, y, cane.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 drawOriginCane = new(cane.Width / 2, Projectile.height / 2);
            Vector2 center = new(Projectile.Center.X, Projectile.Center.Y + 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(cane, center - new Vector2(18 * Projectile.spriteDirection, 0) - Main.screenPosition, new Rectangle?(rectCane), Projectile.GetAlpha(lightColor), caneRotation, drawOriginCane, Projectile.scale, effects, 0);
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<PumpkabooPetBuff>()))
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