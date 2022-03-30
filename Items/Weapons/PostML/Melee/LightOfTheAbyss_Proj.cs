using System;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class LightOfTheAbyss_Proj : ModProjectile
	{
        protected virtual float HoldoutRangeMin => 50f;
        protected virtual float HoldoutRangeMax => 106f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light of the Abyss");
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 34;
            Projectile.height = 34;
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
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[Projectile.owner] = 4;
		}
	}
}