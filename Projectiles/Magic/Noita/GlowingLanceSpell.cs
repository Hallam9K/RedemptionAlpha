using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic.Noita
{
    public class GlowingLanceSpell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Glowing Lance");
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = 4;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 4;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            int dust = Dust.NewDust(Projectile.Center - Vector2.One, 1, 1, ModContent.DustType<GlowingLanceDust>());
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0f;
            Projectile.velocity.Y += 0.02f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 25);
        }
    }
}