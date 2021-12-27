using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class TeslaGenerator_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tesla Field Generator");
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 44;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            Projectile.velocity.Y += 1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.friendly)
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 400 * 400)
                    continue;

                target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 10);
            }
            for (int k = 0; k < 2; k++)
            {
                Vector2 vector;
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 400);
                vector.Y = (float)(Math.Cos(angle) * 400);
                Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, DustID.Electric)];
                dust2.noGravity = true;
                dust2.velocity = -Projectile.DirectionTo(dust2.position) * 4f;
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<TeslaGeneratorBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<TeslaGeneratorBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }
    }
}