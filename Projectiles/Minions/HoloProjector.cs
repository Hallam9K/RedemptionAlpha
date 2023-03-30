using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class HoloProjector : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hologram Projector");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (Projectile.timeLeft == 30)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 40), Vector2.Zero, ModContent.ProjectileType<HoloMinion>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
        }
    }
}