using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class Trumpet_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Trumpet");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 16;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                    Projectile.frame = 0;
            }
            Projectile.direction = projOwner.direction;
            Projectile.spriteDirection = projOwner.direction;
            Projectile.Center = projOwner.Center - new Vector2(-22 * Projectile.spriteDirection, -5);
        }
    }
}
