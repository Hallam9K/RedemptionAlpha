using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class NoblesHalberd_Proj : ModProjectile
	{
		protected virtual float HoldoutRangeMin => 50f;
		protected virtual float HoldoutRangeMax => 86f;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Noble's Halberd");
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Spear);
			Projectile.Redemption().TechnicallyMelee = true;
		}

		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];
			int duration = player.itemAnimationMax;

			player.heldProj = Projectile.whoAmI;

			if (Projectile.timeLeft > duration)
				Projectile.timeLeft = duration;

			Projectile.velocity = Vector2.Normalize(Projectile.velocity);
			float halfDuration = duration * 0.5f;
			float progress;

			if (Projectile.timeLeft < halfDuration)
				progress = Projectile.timeLeft / halfDuration;
			else
				progress = (duration - Projectile.timeLeft) / halfDuration;

			Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

			if (Projectile.spriteDirection == -1)
				Projectile.rotation += MathHelper.ToRadians(45f);
			else
				Projectile.rotation += MathHelper.ToRadians(135f);

			return false;
		}
	}
}