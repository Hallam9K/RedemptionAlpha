using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.Projectiles.Misc
{
    public class SandDust_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dust Cloud");
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, Scale: 2f);
            Main.dust[dustIndex].noGravity = true;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 30)
                Projectile.velocity *= 0.9f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.dontTakeDamage)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.AddBuff(ModContent.BuffType<SandDustDebuff>(), 120);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(ModContent.BuffType<SandDustDebuff>(), 60);
        }
    }
}