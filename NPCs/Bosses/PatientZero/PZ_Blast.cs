using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Projectiles.Hostile;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZ_Blast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenium Blast");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (Main.rand.NextBool(2))
            {
                int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenTorch);
                Main.dust[dust1].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, 0, Projectile.Opacity * 0.3f, 0);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.localAI[0] == 0)
            {
                DustHelper.DrawCircle(Projectile.Center, DustID.GreenTorch, 5, 1, 1, 1, 3, nogravity: true);
                Projectile.localAI[0] = 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY) - new Vector2(0, 20);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 40; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, Scale: 2);
                Main.dust[dustIndex].velocity *= 3f;
            }
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(10, MathHelper.ToRadians(90) * i), ModContent.ProjectileType<PZ_Miniblast>(), Projectile.damage / 2, 3, Main.myPlayer);
        }
    }
}