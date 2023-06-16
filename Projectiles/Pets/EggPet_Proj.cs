using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Pets;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class EggPet_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Egg");
            Main.projFrames[Projectile.type] = 28;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 10, 4).WithOffset(2, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 16;
            Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            AIType = ProjectileID.BabyDino;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;

            CheckActive(player);

            if (Projectile.ai[0] == 1)
            {
                if (frameY < 22)
                    frameY = 22;
                if (++frameCounter >= 5)
                {
                    frameCounter = 0;
                    if (++frameY >= 27)
                        frameY = 22;
                }
            }
            else
            {
                if (Projectile.velocity.X > -.3f && Projectile.velocity.X < .3f)
                {
                    if (frameCounter++ >= 4)
                    {
                        frameCounter = 0;
                        if (++frameY >= 9)
                            frameY = 0;
                    }
                }
                else
                {
                    if (frameY < 10)
                        frameY = 10;

                    frameCounter += (int)Projectile.velocity.X;
                    if (frameCounter >= 4 || frameCounter <= -4)
                    {
                        frameCounter = 0;
                        if (++frameY >= 21)
                            frameY = 10;
                    }
                }
            }
            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            return true;
        }
        private int frameY;
        private int frameCounter;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 28;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - new Vector2(0, 1) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<EggPetBuff>()))
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