using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACE_Scrap : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Scrap");
            Main.projFrames[Projectile.type] = 5;
        }
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.SaucerScrap);
            AIType = ProjectileID.SaucerScrap;
			Projectile.hostile = true;
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 160;
		}
    }
}