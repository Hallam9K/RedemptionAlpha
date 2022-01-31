using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Redemption.Effects;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class KeeperSoulCharge : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Charge");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 0;
            Projectile.timeLeft = 200;
            Projectile.Redemption().Unparryable = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new StandardColorTrail(Color.GhostWhite), new RoundCap(), new ArrowGlowPosition(), 32f, 250f);
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, Projectile.Opacity, Projectile.Opacity);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 4.4f;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);
    }
}