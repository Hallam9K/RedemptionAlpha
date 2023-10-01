using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class CelestialStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Star");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.alpha = 50;
            Projectile.scale = 0.01f;
            Projectile.frame = Main.rand.Next(3);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            int npc = (int)Projectile.ai[0];
            if (npc < 0 || npc >= 200 || !Main.npc[npc].active)
            {
                Projectile.timeLeft = 400;
                Projectile.localAI[0] = 1;
            }
            Projectile.scale += 0.1f;
            Projectile.scale = MathHelper.Min(Projectile.scale, 1);
            if (Projectile.localAI[0] == 1)
            {
                Projectile.Move(player.Center, Projectile.timeLeft > 100 ? 30 : 60, 10);
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    SoundEngine.PlaySound(SoundID.MaxMana with { Volume = 0.5f }, player.position);
                    player.statLife += 2;
                    player.statMana++;
                    player.HealEffect(2);
                    player.ManaEffect(1);
                    Projectile.Kill();
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.1f, 1f, 1.1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(6, 6);
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale * scale, 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White, Projectile.rotation, drawOrigin, Projectile.scale * scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, Scale: 2f);
            Main.dust[dustIndex].velocity *= 0f;
        }
    }
}