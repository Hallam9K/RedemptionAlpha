using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Textures
{
    public class Ring_Visual : ModProjectile
    {
        public override string Texture => "Redemption/Textures/Ring1";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ring");
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 20;
            Projectile.scale = 0.1f;
        }
        public Color color;
        public override void AI()
        {
            Projectile.scale += Projectile.ai[0]; // 0.13f
            Projectile.scale *= Projectile.ai[1]; // 0.9f
            if (Projectile.timeLeft < 10)
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.timeLeft / 10f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
