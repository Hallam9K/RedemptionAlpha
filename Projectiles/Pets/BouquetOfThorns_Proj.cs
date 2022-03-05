using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class BouquetOfThorns_Proj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bouquet of Thorns");
			Main.projFrames[Projectile.type] = 6;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 38;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CheckActive(player);

			if (Projectile.velocity.Y == 0)
			{
				Projectile.rotation = 0;

				if (Projectile.velocity.X == 0)
					Projectile.frame = 0;
				else
				{
					Projectile.frameCounter++;
					if (Projectile.frameCounter >= 5)
					{
						Projectile.frameCounter = 0;
						Projectile.frame++;

						if (Projectile.frame >= Main.projFrames[Projectile.type])
							Projectile.frame = 0;
					}
				}
            }
            else
			{
				Projectile.rotation = Projectile.velocity.X * 0.05f;
				Projectile.frame = 3;
			}
			Projectile.LookByVelocity();

			if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
			{
				Projectile.position = player.Center;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}
			BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, player, true, 6, 8, 40, 1400, 2000, 0.1f, 5, 10);
		}

		private void CheckActive(Player player)
		{
			if (!player.dead && player.HasBuff(ModContent.BuffType<BouquetOfThornsBuff>()))
				Projectile.timeLeft = 2;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.penetrate == 0)
				Projectile.Kill();
			return false;
		}
	}
}