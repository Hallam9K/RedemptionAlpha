using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.Player;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lantard
{
    public class LantardPatreon_Pet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ralsei");
            Main.projFrames[Projectile.type] = 10;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 46;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			CheckActive(player);

			if (Projectile.ai[0] != 0 && Projectile.ai[0] == 1)
			{
				if (Projectile.ai[0] == 1)
					Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
				else
					Projectile.rotation = Projectile.velocity.X * 0.05f;
				Projectile.frame = 9;
			}
			else
			{
				Projectile.rotation = 0;

				if (Projectile.velocity.X == 0)
					Projectile.frame = 0;
				else
				{
					Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X * 0.5f) + 1;
					if (Projectile.frameCounter >= 6)
					{
						Projectile.frameCounter = 0;
						if (++Projectile.frame >= 9)
							Projectile.frame = 1;
					}
				}
			}
			Projectile.LookByVelocity();

			if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
			{
				Projectile.position = player.Center;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}

			BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, player, true, 6, 8, 60, 1000, 2000, 0.1f, 6, 10);
		}
		private void CheckActive(Player player)
		{
			if (!player.dead && player.HasBuff(ModContent.BuffType<LantardPetBuff>()))
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