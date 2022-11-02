using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Projectiles.Hostile
{
    public class Synthesizer_Proj : ModProjectile
	{
        public override string Texture => Redemption.EMPTY_TEXTURE;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Synthesizer");
		}
		public override void SetDefaults()
		{
			Projectile.hostile = false;
            Projectile.friendly = false;
			Projectile.tileCollide = false;
            Projectile.width = 20;
			Projectile.height = 20;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 60;
		}
        public override void AI()
        {

		}
	}
}