using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class StormSummonerPro2 : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukko's Lightning");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 31;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ >= 30)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<UkkoStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                Projectile.Kill();
            }
        }
    }
}