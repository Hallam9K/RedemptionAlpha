using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Base;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class ShadowBolt : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Bolt");
            ElementID.ProjShadow[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 2;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.alpha = 20;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(94, 53, 104), Color.Black), new RoundCap(), new DefaultTrailPosition(), 22f, 250f);
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(117, 10, 47), Color.Black), new RoundCap(), new DefaultTrailPosition(), 22f, 200f);
        }
        public override void AI()
        {
            if (Projectile.velocity.Length() < 6)
                Projectile.velocity *= 1.02f;
            if (Projectile.localAI[0] == 0f)
            {
                for (int m = 0; m < 8; m++)
                {
                    int dustID = Dust.NewDust(Projectile.Center - new Vector2(-1, -1), 2, 2, DustID.ShadowbeamStaff, 0f, 0f, 100, Color.White, 2f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)8 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
                Projectile.localAI[0] = 1f;
            }
            if (Main.rand.NextBool(2))
                ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new GlowParticle2(), new Color(94, 53, 104), 1f, .45f, Main.rand.Next(10, 20));
            if (Main.rand.NextBool(2))
                ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new GlowParticle2(), new Color(117, 10, 47), 1f, .45f, Main.rand.Next(10, 20));

            flareScale += Main.rand.NextFloat(-.02f, .02f);
            flareScale = MathHelper.Clamp(flareScale, .1f, .3f);
            flareOpacity += Main.rand.NextFloat(-.1f, .1f);
            flareOpacity = MathHelper.Clamp(flareOpacity, 0.6f, 0.8f);
        }
        private float flareScale;
        private float flareOpacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/PurpleEyeFlare").Value;
            Rectangle rect2 = new(0, 0, flare.Width, flare.Height);
            Vector2 origin2 = new(flare.Width / 2, flare.Height / 2);

            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * flareOpacity, 0, origin2, Projectile.scale + flareScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * flareOpacity * 0.75f, 0, origin2, (Projectile.scale + flareScale) * 1.1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * flareOpacity * 0.5f, 0, origin2, (Projectile.scale + flareScale) * 1.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            Projectile.penetrate--;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int m = 0; m < 8; m++)
            {
                int dustID = Dust.NewDust(Projectile.Center - new Vector2(-1, -1), 2, 2, DustID.ShadowbeamStaff, 0f, 0f, 100, Color.White, 2f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)8 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}