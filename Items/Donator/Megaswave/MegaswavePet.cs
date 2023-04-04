using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Megaswave
{
    public class MegaswavePet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Miniswave");
            Main.projFrames[Projectile.type] = 15;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 8, 5)
                .WithOffset(2, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 30;
            Projectile.height = 38;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            AIType = ProjectileID.BabyDino;
        }
        private int timer;
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;
            if (Projectile.velocity.Y >= -0.1f && Projectile.velocity.Y <= 0.1f && Projectile.velocity.X == 0)
            {
                timer++;
                if (timer >= 300)
                {
                    Projectile.rotation = 0;
                    if (frameY < 9)
                        frameY = 9;

                    if (frameCounter++ >= 20)
                    {
                        frameCounter = 0;
                        if (++frameY >= 10)
                            frameY = 9;
                    }
                    return true;
                }
            }
            else
                timer = 0;

            if (Projectile.ai[0] == 1)
            {
                Projectile.rotation = Projectile.velocity.X * 0.1f;
                if (frameY < 11)
                    frameY = 11;
                if (frameCounter++ >= 6)
                {
                    frameCounter = 0;
                    if (++frameY >= 14)
                        frameY = 11;
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
                        if (++frameY >= 8)
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
            Texture2D cloak = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Cloak").Value;
            int height = texture.Height / 15;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.FlameAndBlackDye);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            int heightC = cloak.Height / 15;
            int yC = heightC * frameY;
            Rectangle rectC = new(0, yC, cloak.Width, heightC);
            Main.EntitySpriteDraw(cloak, Projectile.Center - Main.screenPosition, new Rectangle?(rectC), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin - new Vector2(36, 0), Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<MegaswavePetBuff>()))
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