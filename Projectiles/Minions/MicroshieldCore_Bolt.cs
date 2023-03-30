using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Redemption.Effects.PrimitiveTrails;
using Terraria.ID;

namespace Redemption.Projectiles.Minions
{
    public class MicroshieldCore_Bolt : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Bolt");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(207, 29, 29), new Color(106, 16, 16)), new RoundCap(), new ArrowGlowPosition(), 14f, 160f);
        }
        public override void AI()
        {
            Projectile.velocity *= 1.03f;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color colour = Color.Lerp(Color.White, new Color(207, 29, 29), 1f);

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour, Projectile.rotation, origin, 0.8f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.4f, Projectile.rotation, origin, 1.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}