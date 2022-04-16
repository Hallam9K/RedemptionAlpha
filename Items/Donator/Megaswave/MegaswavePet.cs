using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
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
            DisplayName.SetDefault("Miniswave");
            Main.projFrames[Projectile.type] = 15;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 38;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, player, true, 6, 8, 120, 1000, 2000, 0.1f, 6, 10);

            if (Projectile.velocity.Y >= -0.1f && Projectile.velocity.Y <= 0.1f && Projectile.velocity.X == 0)
            {
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] >= 300)
                {
                    Projectile.rotation = 0;
                    if (Projectile.frame < 9)
                        Projectile.frame = 9;

                    if (Projectile.frameCounter++ >= 20)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 10)
                            Projectile.frame = 9;
                    }
                    return;
                }
            }
            else
                Projectile.localAI[1] = 0;

            if (Projectile.ai[0] != 0 && Projectile.ai[0] == 1)
            {
                if (Projectile.ai[0] == 1)
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                else
                    Projectile.rotation = Projectile.velocity.X * 0.05f;
                if (Projectile.frame < 11)
                    Projectile.frame = 11;
                if (Projectile.frameCounter++ >= 6)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 14)
                        Projectile.frame = 11;
                }
            }
            else
            {
                Projectile.rotation = 0;

                if (Projectile.velocity.X == 0)
                    Projectile.frame = 0;
                else
                {
                    Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X * 0.5f) + 1;
                    if (Projectile.frameCounter >= 6)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 8)
                            Projectile.frame = 1;
                    }
                }
            }
            Projectile.LookByVelocity();

            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D cloak = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Cloak").Value;
            int height = texture.Height / 15;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.FlameAndBlackDye);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            int heightC = cloak.Height / 15;
            int yC = heightC * Projectile.frame;
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