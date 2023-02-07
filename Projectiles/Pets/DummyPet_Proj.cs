using Redemption.Buffs.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class DummyPet_Proj : ModProjectile
	{
		public override string Texture => Redemption.EMPTY_TEXTURE;
		public override void SetStaticDefaults()
		{
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CheckActive(player);
			Projectile.position = player.position;
		}

		private void CheckActive(Player player)
		{
			if (!player.dead && player.HasBuff(ModContent.BuffType<XenoemiaBuff>()))
				Projectile.timeLeft = 2;
		}
	}
}