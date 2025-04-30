using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Pets;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class WeirdBloomPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, Main.projFrames[Projectile.type], 5)
                .WithOffset(2, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 38;
            Projectile.height = 50;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            AIType = ProjectileID.BabyDino;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;

            if (Projectile.velocity.Y == 0)
            {
                Projectile.rotation = 0;
                if (Projectile.velocity.X > -1 && Projectile.velocity.X < 1)
                    frameY = 0;
                else
                {
                    if (frameY < 1)
                        frameY = 1;

                    frameCounter += (int)(Projectile.velocity.X * 0.5f);
                    if (frameCounter >= 2 || frameCounter <= -2)
                    {
                        frameCounter = 0;
                        frameY++;

                        if (frameY >= Main.projFrames[Projectile.type])
                            frameY = 0;
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                frameY = 3;
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
            int height = texture.Height / 5;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(BuffType<WeirdBloomBuff>()))
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