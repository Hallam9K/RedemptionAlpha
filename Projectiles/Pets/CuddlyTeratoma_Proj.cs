using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class CuddlyTeratoma_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cuddly Teratoma");
            Main.projFrames[Projectile.type] = 7;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(2, Main.projFrames[Projectile.type], 5)
                .WithOffset(2, -20f).WithSpriteDirection(-1)
                .WithCode(DelegateMethods.CharacterPreview.Float);
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            Frames();

            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        private void Frames()
        {
            Player player = Main.player[Projectile.owner];

            if (player.velocity.Length() == 0)
            {
                Vector2 latchPos = new(player.Center.X + (6 * -player.direction), player.position.Y + 2);
                Projectile.Move(latchPos, 10, 1);
                if (Projectile.Center == latchPos)
                {
                    Projectile.spriteDirection = player.direction;
                    Projectile.rotation = 0;
                    Projectile.velocity *= 0;
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 10)
                    {
                        Projectile.frameCounter = 0;
                        if (Main.rand.NextBool(15))
                            Projectile.frame = 1;
                        else
                            Projectile.frame = 0;
                    }
                    return;
                }
            }
            else
                Projectile.Move(new Vector2(player.Center.X + (90 * -player.direction), player.Center.Y - 70), 10, 80);

            if (Projectile.velocity.X < -1 || Projectile.velocity.X > 1)
                Projectile.LookByVelocity();

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if (Projectile.frame < 2)
                Projectile.frame = 2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<CuddlyTeratomaBuff>()))
                Projectile.timeLeft = 2;
        }
    }
}