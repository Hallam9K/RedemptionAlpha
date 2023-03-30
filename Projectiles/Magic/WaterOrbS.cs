using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Projectiles.Magic
{
    public class WaterOrbS : WaterOrb
    {
        public override string Texture => "Redemption/Projectiles/Magic/WaterOrb";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Orb");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ElementID.ProjWater[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override bool PreAI()
        {
            if (Projectile.velocity.Length() < 8)
                Projectile.velocity *= 1.1f;

            if (Projectile.localAI[1] == 0)
            {
                AdjustMagnitude(ref Projectile.velocity);
            }
            Projectile.localAI[1]++;
            Vector2 move = Vector2.Zero;
            float distance = 600;
            bool targetted = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.CanBeChasedBy() || !Collision.CanHit(Projectile.Center, 0, 0, target.Center, 0, 0) || target.Redemption().invisible)
                    continue;

                Vector2 newMove = target.Center - Projectile.Center;
                float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                if (distanceTo < distance)
                {
                    move = newMove;
                    distance = distanceTo;
                    targetted = true;
                }
            }
            if (targetted)
            {
                AdjustMagnitude(ref move);
                Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                AdjustMagnitude(ref Projectile.velocity);
            }
            return true;
        }

        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 8f)
            {
                vector *= 8f / magnitude;
            }
        }
    }
}