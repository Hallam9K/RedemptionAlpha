using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lantard
{
    public class LantardPatreon_Pet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ralsei");
            Main.projFrames[Projectile.type] = 10;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 30;
            Projectile.height = 48;
            AIType = ProjectileID.BabyDino;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;
            return true;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y >= 1 || Projectile.velocity.Y <= -1)
                Projectile.frame = 9;
            else
            {
                if (Projectile.velocity.X == 0)
                    Projectile.frame = 0;
                else
                {
                    Projectile.frameCounter += (int)(Projectile.velocity.X * 0.5f);
                    if (Projectile.frameCounter >= 5 || Projectile.frameCounter <= -5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 9)
                            Projectile.frame = 1;
                    }
                }
            }
            Player player = Main.player[Projectile.owner];
            BuffPlayer modPlayer = player.GetModPlayer<BuffPlayer>();
            if (player.dead)
                modPlayer.lantardPet = false;
            if (modPlayer.lantardPet)
                Projectile.timeLeft = 2;
        }
    }
}