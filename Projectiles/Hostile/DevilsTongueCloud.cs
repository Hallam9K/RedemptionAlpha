using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class DevilsTongueCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pollen Cloud");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 132;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.X / 50;
            if (Projectile.timeLeft > 100)
                Projectile.alpha -= 2;
            else
            {
                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Projectile.ai[0] == 0)
            { 
                Projectile.ai[0] = 1;
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player target = Main.player[p];
                if (!target.active || target.dead)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.AddBuff(ModContent.BuffType<DevilScentedDebuff>(), 360);
                target.AddBuff(BuffID.Confused, 40);
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.dontTakeDamage)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.AddBuff(ModContent.BuffType<DevilScentedDebuff>(), 360);
                target.AddBuff(BuffID.Confused, 40);
            }
        }
    }
}