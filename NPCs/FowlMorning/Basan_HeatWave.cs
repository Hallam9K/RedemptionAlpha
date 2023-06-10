using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Terraria;
using Terraria.ID;

namespace Redemption.NPCs.FowlMorning
{
    public class Basan_HeatWave : FireSlash_Proj
    {
        public override string Texture => "Redemption/Projectiles/Melee/FireSlash_Proj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Wave");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjFire[Type] = true;
            ElementID.ProjWind[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
        public override void AI()
        {
            Projectile.LookByVelocity();
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.velocity *= .98f;
            if (Projectile.ai[0] == 1)
                squish -= 0.02f;
            else
                squish += 0.02f;
            Projectile.alpha += 8;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override bool? CanHitNPC(NPC target) => target.friendly && Projectile.alpha <= 200 ? null : false;
    }
}
