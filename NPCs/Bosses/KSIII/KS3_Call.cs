using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Call : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Transmission");
            Main.projFrames[Projectile.type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 10)
                {
                    Projectile.frame = 0;
                    Projectile.Kill();
                }
            }
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || (host.type != ModContent.NPCType<KS3>() && host.type != ModContent.NPCType<KS3_Clone>()))
                Projectile.Kill();
            Vector2 CallPos = new(host.Center.X + 22 * host.spriteDirection, host.Center.Y - 56);
            Projectile.Center = CallPos;
            if (host.spriteDirection == 1)
                Projectile.spriteDirection = -1;
            else 
                Projectile.spriteDirection = 1;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}