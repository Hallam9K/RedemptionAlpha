using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class Transition : ModProjectile
    {
        public override string Texture => "Redemption/Textures/TransitionTex";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Transition");
        }
        public override void SetDefaults()
        {
            Projectile.width = 1000;
            Projectile.height = 1000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 500;
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            Projectile.scale += 0.1f;
            if (Projectile.localAI[0] == 1f)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha <= 0)
                    Projectile.localAI[0] = 1f;
            }
        }
    }
}