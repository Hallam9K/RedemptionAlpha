using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class BlindJustice_Aura : ModProjectile
    {
        public override string Texture => "Redemption/Textures/HolyGlow2";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Aura");
        }
        public override void SetDefaults()
        {
            Projectile.width = 162;
            Projectile.height = 162;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 50;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.05f * player.direction;
            Projectile.Center = player.Center;

            for (int i = 0; i < 30; i++)
            {
                float distance = Main.rand.Next(40) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = (dustPosition - nextDustPosition + Projectile.velocity) * player.direction;
                if (Main.rand.NextBool(8))
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.GoldFlame, dustVelocity, 0, Scale: 1.5f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }
            if (Projectile.timeLeft % 3 == 0 && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, RedeHelper.PolarVector(10, Projectile.rotation * 8), ModContent.ProjectileType<Lightmass>(), Projectile.damage / 5, 0, Projectile.owner);
            }

            if (Projectile.timeLeft >= 40)
                Projectile.alpha -= 10;

            if (Projectile.timeLeft <= 20)
            {
                Projectile.alpha += 7;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Vector2 flareOrigin = new(flare.Width / 2, flare.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale * 1.6f, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), -Projectile.rotation, drawOrigin, Projectile.scale * 1.6f, effects, 0);
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightYellow) * 2f, Projectile.rotation, flareOrigin, Projectile.scale * 1.6f, effects, 0);
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightYellow) * 2f, -Projectile.rotation, flareOrigin, Projectile.scale * 1.6f, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
