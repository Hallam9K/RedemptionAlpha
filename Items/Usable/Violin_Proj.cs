using Redemption.Globals;
using Terraria;

namespace Redemption.Items.Usable
{
    class Violin_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Violin");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetSafeDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 18;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (!projOwner.channel)
                Projectile.Kill();
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 4)
                    Projectile.frame = 0;
            }
            Projectile.timeLeft = 10;
            Projectile.direction = projOwner.direction;
            Projectile.spriteDirection = projOwner.direction;
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            Projectile.position.X = projOwner.Center.X - (Projectile.width / 2) + (10 * projOwner.direction);
            Projectile.position.Y = projOwner.Center.Y - (Projectile.height / 2) + 4;
        }
    }
}
