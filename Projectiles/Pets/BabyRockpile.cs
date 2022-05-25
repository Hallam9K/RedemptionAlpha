using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class BabyRockpile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Rockpile");
            Main.projFrames[Projectile.type] = 8;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 42;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            if (Projectile.velocity.Y == 0)
            {
                Projectile.rotation.SlowRotation(0, (float)Math.PI / 20);
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
            }
            else
            {
                Projectile.rotation += Projectile.velocity.X * 0.07f;
                Projectile.frame = 0;
            }
            Projectile.LookByVelocity();

            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, player, true, 6, 8, 100, 1400, 2000, 0.1f, 5, 10);
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<BabyRockpileBuff>()))
                Projectile.timeLeft = 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
    }
}