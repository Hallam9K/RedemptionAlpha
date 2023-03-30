using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class DarkSteelArrow_Tendril : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Tendril");
            ElementID.ProjShadow[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 50;
            Projectile.timeLeft = 700;
            Projectile.penetrate = 8;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.localAI[0] = Main.rand.NextFloat(-0.01f, 0.01f);
            Projectile.localAI[1] = 20;
            decrease = Main.rand.Next(1, 3);
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        private int decrease;
        public override void AI()
        {
            if (Projectile.ai[0]++ % 20 == 0)
            {
                Projectile.localAI[1] -= decrease;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.localAI[0]);
            for (int i = 0; i < 1; i++)
            {
                int dust = Dust.NewDust(Projectile.position, (int)Projectile.localAI[1], (int)Projectile.localAI[1], ModContent.DustType<VoidFlame>(), Scale: Projectile.localAI[1] / 6);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (Projectile.localAI[1] <= 1)
                Projectile.Kill();
        }
    }
}
