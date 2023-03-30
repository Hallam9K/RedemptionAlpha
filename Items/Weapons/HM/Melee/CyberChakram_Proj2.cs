using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class CyberChakram_Proj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyber Chakram");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 180;
            Projectile.light = 0.5f;
        }
        public override void AI()
        {
            Projectile.velocity *= 0;
            Projectile.alpha += 6;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Projectile.ai[0];
                Projectile.localAI[1] = .6f;
                Projectile.localAI[0] = 1;
            }
            Projectile.rotation += Projectile.localAI[1] * Projectile.ai[1];
            Projectile.localAI[1] *= .9f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * Projectile.Opacity;
        }
    }
}
