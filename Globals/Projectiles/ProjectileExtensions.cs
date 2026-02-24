using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Redemption.Globals.Projectiles
{
    internal static class ProjectileExtensions
    {
        public static bool MinionOrShot(this Projectile proj)
        {
            return proj.minion || ProjectileID.Sets.MinionShot[proj.type];
        }

        public static bool SentryOrShot(this Projectile proj)
        {
            return proj.sentry || ProjectileID.Sets.SentryShot[proj.type];
        }

        /// <summary>
        /// Returns true if the projectile is a minion, a sentry, or if it was fired by either.
        /// </summary>
        public static bool MinionOrSentryOrShot(this Projectile proj)
        {
            return proj.MinionOrShot() || proj.SentryOrShot();
        }

        /// <summary>
        /// Returns the draw position of this projectile.
        /// </summary>
        public static Vector2 DrawPosition(this Projectile projectile)
        {
            return projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
        }

        public static SpriteEffects BasicSpriteEffects(this Projectile projectile) => projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        public static bool ShouldHandleNet(this Projectile proj)
        {
            return Main.netMode == NetmodeID.SinglePlayer || (proj.owner >= 255 && Main.netMode != NetmodeID.MultiplayerClient) || Main.myPlayer == proj.owner;
        }
    }
}