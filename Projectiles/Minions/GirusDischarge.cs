using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;

namespace Redemption.Projectiles.Minions
{
    public class GirusDischarge : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Discharge");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 90;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 1f)
            {
                for (int num = 0; num < 4; num++)
                {
                    Vector2 vector = Projectile.position;
                    vector -= Projectile.velocity * (num * 0.25f);
                    int d = Dust.NewDust(vector, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 200);
                    Main.dust[d].position = vector;
                    Main.dust[d].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].noGravity = true;
                }
            }
        }
    }
}
