using Microsoft.Xna.Framework;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class HiveCyst_Proj : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/SeedOfInfection/SeedGrowth";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hive Cyst");
            Main.projFrames[Projectile.type] = 4;

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ElementID.ProjPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 44;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.minionSlots = 0;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit13, Projectile.position);
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }
        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => true;
        private NPC target2;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.ownedProjectileCounts[ModContent.ProjectileType<HiveCyst_Proj>()] >= 3 && Main.rand.NextBool(2))
                Projectile.Kill();
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if (Projectile.alpha >= 0)
                Projectile.alpha -= 5;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            OverlapCheck();

            if (RedeHelper.ClosestNPC(ref target2, 800, Projectile.Center, false, owner.MinionAttackTargetNPC))
                Projectile.Move(new Vector2(target2.Center.X, target2.Center.Y), 10, 1);
            else
                Projectile.Move(new Vector2(owner.Center.X + (20 + Projectile.minionPos * 40) * -owner.direction, owner.Center.Y - 42), 10, 70);

            if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
            {
                Projectile.position = owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

        }

        public override bool? CanHitNPC(NPC target)
        {
            return target == target2 ? null : false;
        }

        private void OverlapCheck()
        {
            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }
    }
}