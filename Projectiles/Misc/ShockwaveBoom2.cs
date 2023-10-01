using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria;

namespace Redemption.Projectiles.Misc
{
    public class ShockwaveBoom2 : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave Boom");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (!Main.dedServ)
            {
                float progress = (180f - Projectile.timeLeft) / 60f;
                float pulseCount = 1;
                float rippleSize = 5;
                float speed = 15;
                Filters.Scene["MoR:Shockwave"].GetShader().UseProgress(progress).UseOpacity(100f * (1 - progress / 3f));
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] >= 0 && Projectile.localAI[1] <= 60)
                {
                    if (!Filters.Scene["MoR:Shockwave"].IsActive())
                    {                                                             //pulseCount rippleSize speed
                        Filters.Scene.Activate("MoR:Shockwave", Projectile.Center).GetShader().UseColor(pulseCount, rippleSize, speed).UseTargetPosition(Projectile.Center);
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                Filters.Scene["MoR:Shockwave"].Deactivate();
            }
        }
    }
    public class ShockwaveBoom3 : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave Boom");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (!Main.dedServ)
            {
                float progress = (180f - Projectile.timeLeft) / 60f;
                float pulseCount = 8;
                float rippleSize = 2;
                float speed = 18;
                Filters.Scene["MoR:Shockwave"].GetShader().UseProgress(progress).UseOpacity(100f * (1 - progress / 3f));
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] >= 0 && Projectile.localAI[1] <= 300)
                {
                    if (!Filters.Scene["MoR:Shockwave"].IsActive())
                    {                                                             //pulseCount rippleSize speed
                        Filters.Scene.Activate("MoR:Shockwave", Projectile.Center).GetShader().UseColor(pulseCount, rippleSize, speed).UseTargetPosition(Projectile.Center);
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                Filters.Scene["MoR:Shockwave"].Deactivate();
            }
        }
    }
}
