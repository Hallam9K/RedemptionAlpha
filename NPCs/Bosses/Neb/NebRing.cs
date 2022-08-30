using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class NebRing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Ring");
        }
        public override void SetDefaults()
        {
            Projectile.width = 408;
            Projectile.height = 408;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 254;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.Center = npc.Center;
            Projectile.velocity = Vector2.Zero;

            Projectile.localAI[0]++;
            Projectile.rotation += 0.06f;
            if (Projectile.localAI[0] <= 120)
                Projectile.alpha -= 2;
            if (Projectile.localAI[0] >= 200)
                Projectile.alpha += 4;

            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}