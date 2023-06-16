using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic.Noita
{
    public class ConcentratedLightSpell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Concentrated Light");
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.penetrate = 4;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 800;
            Projectile.extraUpdates = 10;
        }
        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GreenSpellDust>());
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            Projectile.velocity *= 1.01f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}