using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class Chernobyl_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chernobyl");
        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 310f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 17f;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < 30; i++)
            {
                float distance = Main.rand.Next(10) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = dustPosition - nextDustPosition + Projectile.velocity;
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.GreenTorch, dustVelocity, Scale: 0.5f);
                    dust.scale = distance / 30;
                    dust.scale = MathHelper.Clamp(dust.scale, 0.2f, 3);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || (player.dontHurtCritters && npc.lifeMax <= 5))
                    continue;

                if (Projectile.DistanceSQ(npc.Center) > 70 * 70)
                    continue;

                npc.AddBuff(ModContent.BuffType<BileDebuff>(), 180);
            }
        }
    }
}