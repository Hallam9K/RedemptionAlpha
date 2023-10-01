using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoRainHealing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Healing Rain");
            ElementID.ProjWater[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.5f;
            Projectile.alpha -= 10;
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (player.active && !player.dead && Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    player.AddBuff(BuffID.Wet, 600);
                    player.statLife += 1;
                    player.HealEffect(1);
                    Projectile.Kill();
                }
            }
            for (int p = 0; p < Main.maxNPCs; p++)
            {
                NPC npc = Main.npc[p];
                if (npc.active && !npc.immortal && !npc.dontTakeDamage && Projectile.Hitbox.Intersects(npc.Hitbox))
                {
                    if (npc.life < npc.lifeMax - 100)
                    {
                        npc.AddBuff(BuffID.Wet, 600);
                        npc.life += 100;
                        npc.HealEffect(100);
                        Projectile.Kill();
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Rain);
        }
    }
}