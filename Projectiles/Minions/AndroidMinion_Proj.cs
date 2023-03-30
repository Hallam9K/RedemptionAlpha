using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class AndroidMinion_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Android Minion");
            Main.projFrames[Projectile.type] = 10;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 32;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            Target();
            Player projOwner = Main.player[Projectile.owner];
            if (!CheckActive(projOwner))
                return;

            if (Projectile.velocity.Y != 0)
                Projectile.frame = 1;
            else
            {
                if (Projectile.velocity.X == 0)
                    Projectile.frame = 0;
                else
                {
                    if (Projectile.frame < 2)
                        Projectile.frame = 2;
                    if (++Projectile.frameCounter >= 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 10)
                            Projectile.frame = 2;
                    }
                }
            }
            if (RedeHelper.ClosestNPC(ref target2, 1000, Projectile.Center, false, projOwner.MinionAttackTargetNPC))
            {
                if (Projectile.localAI[0]++ % 50 == 0 && projOwner.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(CustomSounds.MissileFire1 with { Volume = 0.3f, Pitch = 0.2f }, Projectile.position);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(14, (target.Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<AndroidMinion_Fist>(), Projectile.damage, Projectile.knockBack, projOwner.whoAmI);
                    Projectile.velocity.X -= 2 * Projectile.spriteDirection;
                }
            }
            if (Main.myPlayer == projOwner.whoAmI && Projectile.DistanceSQ(projOwner.Center) > 2000 * 2000)
            {
                Projectile.position = projOwner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, projOwner, false, 10, 10, 20, 1000, 2000, 0.1f, 6, 10, (proj, owner) => { return target == projOwner ? null : target; });
        }
        private Entity target;
        private NPC target2;
        public void Target()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (RedeHelper.ClosestNPC(ref target2, 1200, Projectile.Center, false, projOwner.MinionAttackTargetNPC))
                target = target2;
            else
                target = projOwner;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<AndroidMinionBuff>());
                return false;
            }
            if (owner.HasBuff(ModContent.BuffType<AndroidMinionBuff>()))
                Projectile.timeLeft = 2;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
        public override bool MinionContactDamage() => false;
    }
}