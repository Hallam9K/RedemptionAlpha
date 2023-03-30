using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class MiniSpaceship_Laser : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Laser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 800;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, Scale: 2);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].noGravity = true;
        }
    }
}
