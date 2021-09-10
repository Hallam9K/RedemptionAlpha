using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class PetChicken : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 24;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, player, true, teleportDist: 2000);
            CheckActive(player);
            Projectile.LookByVelocity();
            if (Main.rand.NextBool(200) && Projectile.velocity.Y == 0 && Projectile.localAI[0] == 0)
                Projectile.localAI[0] = 1;

            if (Projectile.localAI[0] == 1)
            {
                Projectile.velocity *= 0;
                Projectile.rotation = 0;

                if (Projectile.frame < 9)
                    Projectile.frame = 9;

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame > 15)
                    {
                        Projectile.frame = 0;
                        Projectile.localAI[0] = 0;
                    }
                }
                return;
            }
            if (Projectile.velocity.Y == 0)
            {
                Projectile.rotation = 0;
                if (Projectile.velocity.X == 0)
                    Projectile.frame = 0;
                else
                {
                    if (Projectile.frame < 1)
                        Projectile.frame = 1;

                    Projectile.frameCounter += (int)(Projectile.velocity.X * 0.75f);
                    if (Projectile.frameCounter is >= 5 or <= -5)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 8)
                            Projectile.frame = 1;
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                Projectile.frame = 2;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<PetChickenBuff>()))
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();
        }
    }
}