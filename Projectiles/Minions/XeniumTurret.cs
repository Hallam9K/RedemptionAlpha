using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class XeniumTurret : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Xenium Autoturret");
			Main.projPet[Projectile.type] = true;

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}
		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 34;
			Projectile.tileCollide = false;

			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minionSlots = 1f;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
		}

		public override bool? CanCutTiles() => false;

		private NPC target;
		Vector2 AttackPos;
		private int attackPosition;
		private int attackPositionY;

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (!CheckActive(owner))
				return;
			OverlapCheck();

			if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center, false, owner.MinionAttackTargetNPC))
            {
				int bulletID = -1;
				float shootSpeed = 10f;
				int shootDamage = Projectile.damage;
				float shootKnockback = Projectile.knockBack;

				if (Projectile.ai[0] % 120 == 0)
				{
					attackPosition = Main.rand.Next(target.width + 40, target.width + 120);
					attackPositionY = Main.rand.Next(target.width - 25, target.width);
					AttackPos = new(Projectile.Center.X > target.Center.X ? attackPosition : -attackPosition, -attackPositionY) ;
				}

				Projectile.Move(target.Center + AttackPos, 15, 10);
				Projectile.LookAtEntity(target);

				if (++Projectile.ai[0] % 15 == 0)
                {
					if (Projectile.UseAmmo(AmmoID.Bullet, ref bulletID, ref shootSpeed, ref shootDamage, ref shootKnockback, Main.rand.Next(5) > 0))
                    {
						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(shootSpeed, (target.Center - Projectile.Center).ToRotation()), bulletID, shootDamage, Projectile.knockBack, Main.myPlayer);
					}
				}
			}
			else
            {
				Projectile.LookByVelocity();
				Projectile.Move(new Vector2(owner.Center.X + (20 + Projectile.minionPos * 50) * -owner.direction, owner.Center.Y - 62), 10, 0);
			}
			if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
			{
				Projectile.position = owner.Center;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}
		}

		private bool CheckActive(Player owner)
		{
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<XeniumTurretBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<XeniumTurretBuff>()))
				Projectile.timeLeft = 2;

			return true;
		}

		private void OverlapCheck()
		{
			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile other = Main.projectile[i];

				if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					if (Projectile.position.X < other.position.X)
					{
						Projectile.velocity.X -= overlapVelocity;
					}
					else
					{
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y)
					{
						Projectile.velocity.Y -= overlapVelocity;
					}
					else
					{
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}
	}
}