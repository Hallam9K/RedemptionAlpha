using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace Redemption.Textures
{
    public class CirclePulse_Visual : ModProjectile
    {
        public override string Texture => "Redemption/Textures/DreamsongLight_Visual";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pulse");
        }
        public override void SetDefaults()
        {
            Projectile.width = 600;
            Projectile.height = 600;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public Entity entityTarget;
        public override void AI()
        {
            if (entityTarget != null)
            {
                if (entityTarget.active)
                    Projectile.Center = entityTarget.Center;
            }

            Projectile.timeLeft = 10;
            Projectile.velocity *= 0;
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 60)
            {
                if (Projectile.localAI[0] < 30)
                    Projectile.alpha -= 5;
                else
                    Projectile.alpha += 5;
                Projectile.scale += 0.003f;
            }
            else
            {
                Projectile.alpha = 255;
                Projectile.scale = 1;
                Projectile.Kill();
            }
        }
        public Color color;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, origin, Projectile.scale * Projectile.ai[0], SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}