using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    class NoveltyMop_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Novelty Mop");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 25;
        }
        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (!projOwner.channel)
                Projectile.Kill();

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.timeLeft = 4;
            projOwner.itemTime = 4;
            projOwner.itemAnimation = 4;
            Projectile.direction = projOwner.direction;
            Projectile.spriteDirection = projOwner.direction;
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            Projectile.position.X = projOwner.Center.X - (Projectile.width / 2);
            Projectile.position.Y = projOwner.Center.Y - (Projectile.height / 2) + 4;
        }
    }
}
