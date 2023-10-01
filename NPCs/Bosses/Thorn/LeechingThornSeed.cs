using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class LeechingThornSeed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Life-Draining Thorn");
            Main.projFrames[Projectile.type] = 2;
            ElementID.ProjNature[Type] = true;
            ElementID.ProjBlood[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, 0f, 0f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (host.life < host.lifeMax - 20)
            {
                int steps = (int)host.Distance(target.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(host.Center, target.Center, (float)i / steps), 2, 2, DustID.LifeDrain);
                        dust.velocity = target.DirectionTo(dust.position) * 2;
                        dust.noGravity = true;
                    }
                }
                host.life += 20;
                host.HealEffect(20);
            }
            Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 100, default, 1.2f);
                Main.dust[dustIndex].velocity *= 1.9f;
            }
        }
    }
}