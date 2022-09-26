using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using IL.Terraria.GameContent;

namespace Redemption.NPCs.Bosses.ADD
{
    public class EarthBarrier : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Floating Island");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 224;
            Projectile.height = 98;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 340;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity *= 0;
            Projectile.position = new Vector2(player.Center.X - 112, player.Center.Y - 250);

            Projectile.frame = (int)Projectile.ai[0];
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Projectile.alpha > 0 && Projectile.timeLeft >= 60)
                Projectile.alpha -= 10;

            var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
            foreach (var proj in list)
            {
                if (proj.active && Projectile != proj && proj.friendly && !proj.minion)
                {
                    proj.Kill();
                }
            }
        }
    }
}