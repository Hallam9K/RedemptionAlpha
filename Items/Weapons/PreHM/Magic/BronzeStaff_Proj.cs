using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;

namespace Redemption.Items.Weapons.PreHM.Magic
{
	public class BronzeStaff_Proj : ModProjectile
	{
		protected virtual float HoldoutRangeMin => 50f;
		protected virtual float HoldoutRangeMax => 86f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bronze Staff");
		}

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 46;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;		
		}
		public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();
			
			if (Main.myPlayer == Projectile.owner)
            {
				Projectile.ai[0]++;
                if (Projectile.ai[0] >= 5)
                {
					Projectile.NewProjectile(source, Projectile.Center, 0, ModContent.ProjectileType<BlueOrb>(), 20, 3f, Main.myPlayer, Projectile.whoAmI);
                }
            }
		}
	}
}