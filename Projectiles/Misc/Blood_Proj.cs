using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Projectiles.Hostile
{
    public class Blood_Proj : ModProjectile
	{
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blood Clusters");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.alpha = 255;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.timeLeft = 120;
		}

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			Main.dust[dust].scale = 3f;
			Main.dust[dust].noGravity = true;
			Projectile.velocity.Y += 0.7f;
		}
	}
}