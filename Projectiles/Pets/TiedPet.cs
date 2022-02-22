using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class TiedPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tied");
            Main.projFrames[Projectile.type] = 16;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 20;
            Projectile.height = 34;
            AIType = ProjectileID.BabyDino;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            if (Projectile.velocity.Y >= -0.1f && Projectile.velocity.Y <= 0.1f)
            {
                if (Projectile.frame < 8)
                    Projectile.frame = 8;
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 15)
                        Projectile.frame = 8;
                }
            }
            else
            {
                if (Projectile.velocity.X == 0)
                    Projectile.frame = 0;
                else
                {
                    Projectile.frameCounter += (int)(Projectile.velocity.X * 0.5f);
                    if (Projectile.frameCounter >= 5 || Projectile.frameCounter <= -5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 7)
                            Projectile.frame = 1;
                    }
                }
            }
            if (player.ZoneDesert || player.ZoneUndergroundDesert || player.ZoneUnderworldHeight)
            {
                if (Main.rand.NextBool(4))
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
                    Main.dust[dustIndex].noGravity = false;
                    Main.dust[dustIndex].velocity *= 0f;
                }
                if (Main.rand.NextBool(900))
                {
                    EmoteBubble.NewBubble(2, new WorldUIAnchor(Projectile), 120);
                }
            }
            if (Main.rand.NextBool(20000))
            {
                switch (Main.rand.Next(7))
                {
                    case 0:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "Boo!", false, false);
                        break;
                    case 1:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "Got a sauna?", false, false);
                        break;
                    case 2:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "spooky scary skeletons~", false, false);
                        break;
                    case 3:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "noniin...", false, false);
                        break;
                    case 4:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "My ladle knows no mercy.", false, false);
                        break;
                    case 5:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "Too dapper for the dagger", false, false);
                        break;
                    case 6:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "ded", false, false);
                        break;
                }
            }
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<TiedPetBuff>()))
                Projectile.timeLeft = 2;
        }
    }
}