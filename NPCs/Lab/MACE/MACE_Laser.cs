using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACE_Laser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Laser");
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 100;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                    Projectile.frame = 7;
            }
            Lighting.AddLight(Projectile.Center, 0f, Projectile.Opacity * 0.1f, 0f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.velocity *= 1.01f;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy);
                Main.dust[dustIndex].velocity *= 2f;
            }
        }
    }
}