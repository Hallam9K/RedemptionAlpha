using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Gigapora;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class GigapeiliBolt : ShieldCore_Bolt, ITrailProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 1;
        }
        public new void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 236, 100, 100), new Color(0, 0, 0, 0)), new RoundCap(), new DefaultTrailPosition(), 20f, 100f);
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 29, 29, 0), new Color(106, 16, 16, 0)), new RoundCap(), new DefaultTrailPosition(), 10f, 200f);
        }
        NPC target;
        bool targetted;
        public override bool PreAI()
        {
            float spread = .04f;
            if (Projectile.ai[0] is 1)
            {
                spread = .02f;
                if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center) && !targetted)
                {
                    targetted = true;
                    Projectile.velocity = Projectile.Center.DirectionTo(target.Center) * 20f;
                }
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-spread, spread));
            flareScale += Main.rand.NextFloat(-.02f, .02f);
            flareScale = MathHelper.Clamp(flareScale, .2f, .3f);
            flareOpacity += Main.rand.NextFloat(-.2f, .2f);
            flareOpacity = MathHelper.Clamp(flareOpacity, 0.6f, 1.1f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.Center - (Projectile.timeLeft > 0 ? Vector2.Zero : Projectile.velocity) - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, .5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0;
                Color dustColor = new(255, 176, 70) { A = 0 };
                Main.dust[dust].color = dustColor;
            }
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 20f)
                vector *= 16f / magnitude;
        }
    }
}
