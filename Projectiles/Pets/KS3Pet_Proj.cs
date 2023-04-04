using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Bosses.KSIII;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class KS3Pet_Proj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slayer Projector");
			Main.projFrames[Projectile.type] = 12;
			Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 1, 5)
				.WithOffset(2, -20f).WithSpriteDirection(-1)
                .WithCode(DelegateMethods.CharacterPreview.Float);
        }

        public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 46;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		private int HeadType;
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CheckActive(player);

			Projectile.rotation = Projectile.velocity.X * 0.05f;

			if (Projectile.frame < HeadType * 2)
				Projectile.frame = HeadType * 2;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > (HeadType * 2) + 1)
					Projectile.frame = HeadType * 2;
			}
			if (Projectile.velocity.X < -2 || Projectile.velocity.X > 2)
				Projectile.LookByVelocity();
			else
				Projectile.LookAtEntity(player);

			if (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus || NPC.AnyNPCs(ModContent.NPCType<KS3>()) || player.InModBiome<SlayerShipBiome>())
				HeadType = 5;
			else if (player.wellFed)
				HeadType = 1;
			else if (NPC.AnyNPCs(ModContent.NPCType<Keeper>()) || NPC.AnyNPCs(ModContent.NPCType<KeeperSpirit>()))
				HeadType = 2;
			else
				HeadType = 0;
			
			Projectile.Move(new Vector2(player.Center.X + (70 * -player.direction), player.Center.Y - 70), 10, 40);

			if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
			{
				Projectile.position = player.Center;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}
		}

		private void CheckActive(Player player)
		{
			if (!player.dead && player.HasBuff(ModContent.BuffType<KS3PetBuff>()))
			{
				Projectile.timeLeft = 2;
			}
		}
	}
}