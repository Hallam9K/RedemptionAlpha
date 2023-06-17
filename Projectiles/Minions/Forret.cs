using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;

namespace Redemption.Projectiles.Minions
{
    public class Forret : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 42;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => Projectile.velocity.Length() > 4;

        public override void AI()
        {
            Target();
            Player projOwner = Main.player[Projectile.owner];

            if (!CheckActive(projOwner))
                return;

            if (Projectile.velocity.Y == 0)
            {
                if (Projectile.velocity.X >= -0.2f && Projectile.velocity.X <= 0.2f)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 10)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame = 0;
                        if (Main.rand.NextBool(16))
                            Projectile.frame = 1;
                    }
                }
                else
                {
                    if (Projectile.frame < 2)
                        Projectile.frame = 2;

                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 4)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;

                        if (Projectile.frame >= Main.projFrames[Projectile.type])
                            Projectile.frame = 2;
                    }
                }
            }
            else
                Projectile.frame = 2;

            Projectile.localAI[0]--;
            Projectile.localAI[0] = MathHelper.Clamp(Projectile.localAI[0], 0, 80);
            if (Projectile.localAI[0] <= 0 && BaseAI.HitTileOnSide(Projectile, 3) && RedeHelper.ClosestNPC(ref target2, 100, Projectile.Center, false, projOwner.MinionAttackTargetNPC))
            {
                Projectile.velocity.X += target.Center.X < Projectile.Center.X ? -8 : 8;
                Projectile.velocity.Y = Main.rand.NextFloat(-3f, -5f);
                Projectile.localAI[0] = 80;
            }

            if (Main.myPlayer == projOwner.whoAmI && Projectile.DistanceSQ(projOwner.Center) > 2000 * 2000)
            {
                Projectile.position = projOwner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, projOwner, false, 9, 6, 40, 1400, 2000, 0.1f, 6, 10, (proj, owner) => { return target == projOwner ? null : target; });
        }

        private Entity target;
        private NPC target2;
        public void Target()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (RedeHelper.ClosestNPC(ref target2, 600, Projectile.Center, false, projOwner.MinionAttackTargetNPC))
                target = target2;
            else
                target = projOwner;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ForretBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ForretBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.knockBackResist > 0)
                modifiers.Knockback.Flat += Math.Abs(Projectile.velocity.X);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (player.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
    }
}
