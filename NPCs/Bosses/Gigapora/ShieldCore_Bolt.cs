using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class ShieldCore_Bolt : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Bolt");
            ElementID.ProjThunder[Type] = true;
            ElementID.ProjFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 236, 100, 100), new Color(0, 0, 0, 0)), new RoundCap(), new DefaultTrailPosition(), 10f, 300f);
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 29, 29, 0), new Color(106, 16, 16, 0)), new RoundCap(), new DefaultTrailPosition(), 20f, 200f);
        }
        public override void AI()
        {
            Projectile.velocity *= 1.02f;
            flareScale += Main.rand.NextFloat(-.02f, .02f);
            flareScale = MathHelper.Clamp(flareScale, .9f, 1.1f);
            flareOpacity += Main.rand.NextFloat(-.2f, .2f);
            flareOpacity = MathHelper.Clamp(flareOpacity, 0.6f, 1.1f);
        }
        public float flareScale;
        public float flareOpacity;
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/RedEyeFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Color.White * flareOpacity, Projectile.rotation, origin, 1f * flareScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Color.White * flareOpacity * 0.4f, Projectile.rotation, origin, 1.4f * flareScale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}