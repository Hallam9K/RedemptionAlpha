using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoHail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hail");
            ElementID.ProjIce[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.5f;
            Projectile.rotation += 0.08f;
            Projectile.alpha -= 10;
            for (int p = 0; p < 255; p++)
            {
                Player player = Main.player[p];
                if (player.active && !player.dead && Projectile.Hitbox.Intersects(player.Hitbox))
                    player.AddBuff(BuffID.Frozen, 60);
            }
        }
        public override void OnKill(int timeLeft)
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice);
            Main.dust[dustIndex].velocity *= 2;
        }
    }
}