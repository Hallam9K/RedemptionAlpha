using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Mounts;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class JyrinaMount_Horse : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jyrina");
            Main.projFrames[Projectile.type] = 17;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 64;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            if (Math.Abs(player.velocity.X) > 10)
            {
                Vector2 position = Projectile.Center + (Vector2.Normalize(player.velocity) * 30f);
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado)];
                dust.position = position;
                dust.velocity = (player.velocity.RotatedBy(1.57) * 0.33f) + (player.velocity / 4f);
                dust.position += player.velocity.RotatedBy(1.57);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
                dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado)];
                dust.position = position;
                dust.velocity = (player.velocity.RotatedBy(-1.57) * 0.33f) + (player.velocity / 4f);
                dust.position += player.velocity.RotatedBy(-1.57);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;

                if (Projectile.frame < 11)
                    Projectile.frame = 11;
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 16)
                        Projectile.frame = 11;
                }
            }
            else
            {
                if (player.velocity.X > -1 && player.velocity.X < 1 && Math.Abs(player.velocity.Y) <= 1)
                {
                    if (Projectile.frameCounter++ >= 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 2)
                            Projectile.frame = 0;
                    }
                }
                else
                {
                    if (Projectile.frame < 3)
                        Projectile.frame = 3;

                    if (Projectile.frameCounter++ >= 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 10)
                            Projectile.frame = 3;
                    }
                }
            }
            Projectile.spriteDirection = player.direction;
            Projectile.Center = player.Center + new Vector2(60 * player.direction, -2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 17;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightGoldenrodYellow, Color.LightYellow * 0.7f, Color.LightGoldenrodYellow);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Main.EntitySpriteDraw(texture, oldPos + Projectile.Size / 2f - Main.screenPosition, rect, Projectile.GetAlpha(shaderColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<JyrinaMountBuff>()))
                Projectile.timeLeft = 2;
            else
                Projectile.active = false;
        }
    }
}