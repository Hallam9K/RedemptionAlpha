using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    public class InfectionDust : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infection");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / 180) * 1.02f;

            int dustType = 74;
            int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType, 0f, 0f, 100, Color.White, 2f);
            Main.dust[dustID].velocity *= 0f;
            Main.dust[dustID].noGravity = true;

        }
    }
}
