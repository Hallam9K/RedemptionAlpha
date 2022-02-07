using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Effects.PrimitiveTrails;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_EnergyBolt : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.Redemption().Unparryable = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(140, 255, 242), new Color(194, 255, 242)), new RoundCap(), new ArrowGlowPosition(), 10f, 200f);
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.localAI[0]++ == 0)
                Projectile.velocity *= 3;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color colour = Color.Lerp(Color.White, new Color(0, 242, 170), 1f);

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour, Projectile.rotation, origin, 0.6f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.4f, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}