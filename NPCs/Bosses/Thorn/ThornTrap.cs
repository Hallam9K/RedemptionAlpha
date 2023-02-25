using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class ThornTrap : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
            ElementID.ProjNature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 68;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 9)
                    Projectile.frame = 7;
            }
            Projectile.localAI[0] += 1f;
            Projectile.velocity.X *= 0.00f;
            Projectile.velocity.Y += 1.00f;
            if (Projectile.localAI[0] >= 160)
                Projectile.alpha += 10;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (!player.active || player.dead || !Projectile.Hitbox.Intersects(player.Hitbox))
                    continue;

                if (Projectile.frame < 7 || Projectile.ai[0] == 1)
                    continue;

                player.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 10);
            }
        }
        public override bool CanHitPlayer(Player target) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0;
            return false;
        }
    }
}