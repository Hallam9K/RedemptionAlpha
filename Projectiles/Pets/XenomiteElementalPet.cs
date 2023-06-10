using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class XenomiteElementalPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pet Xenomite Elemental");
			Main.projFrames[Projectile.type] = 8;
			Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 5)
				.WithOffset(2, -20f).WithSpriteDirection(-1)
                .WithCode(DelegateMethods.CharacterPreview.Float);
        }

        public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 66;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CheckActive(player);

            Lighting.AddLight(Projectile.Center, .4f * Projectile.Opacity, Projectile.Opacity, .4f * Projectile.Opacity);
            Projectile.rotation = Projectile.velocity.X * 0.05f;

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.frame = 0;
			}
			if (Projectile.velocity.X < -2 || Projectile.velocity.X > 2)
				Projectile.LookByVelocity();
			else
				Projectile.LookAtEntity(player);

			Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 1f / 255f, (255 - Projectile.alpha) * 0.3f / 255f);

			Vector2 moveTo = new(player.Center.X + (30 * -player.direction), player.Center.Y - 100);

			if (Projectile.DistanceSQ(moveTo) < 100 * 100)
				Projectile.velocity *= 0.96f;
			else
				Projectile.Move(new Vector2(player.Center.X + (30 * -player.direction), player.Center.Y - 100), 10, 20);

			if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
			{
				Projectile.position = player.Center;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}
		}

		private void CheckActive(Player player)
		{
			if (!player.dead && player.HasBuff(ModContent.BuffType<XenomiteElementalPetBuff>()))
				Projectile.timeLeft = 2;
		}
	}
}