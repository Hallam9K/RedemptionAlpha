using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Particles;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class LightningStoneMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        NPC target;
        public float launchCooldown;
        public bool detect;
        Player Player => Main.player[Projectile.owner];
        public override void AI()
        {
            if (!CheckActive(Player))
                return;
            OverlapCheck();

            Projectile.rotation += Projectile.velocity.X * 0.05f;
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, .8f * Projectile.Opacity, .6f * Projectile.Opacity);
            Projectile.LookByVelocity();

            glowOpacity -= .05f;
            glowOpacity = MathHelper.Max(glowOpacity, 0);
            launchCooldown--;

            if (RedeHelper.ClosestNPC(ref target, 800, Projectile.Center, true, Player.MinionAttackTargetNPC))
            {
                if (launchCooldown <= 0)
                {
                    Projectile.Move(target.Center, 20, 10);
                    if (RedeHelper.ClosestNPC(ref target, 150, Projectile.Center, true, Player.MinionAttackTargetNPC))
                    {
                        if (launchCooldown <= 0)
                        {
                            Projectile.Move(target.Center, 35, 0);
                            launchCooldown = 10;
                        }
                    }
                }
            }
            else
            {
                if (Projectile.DistanceSQ(Player.Center) >= 80 * 80)
                    Projectile.Move(Player.Center, MathHelper.Lerp(10, 18, Projectile.DistanceSQ(Player.Center) / 6400));
            }
            if (Main.myPlayer == Player.whoAmI && Projectile.DistanceSQ(Player.Center) > 2000 * 2000)
            {
                Projectile.position = Player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 startPos = target.Center;
            Vector2 direction = target.DirectionFrom(Projectile.Center);

            float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
            float randomVel = Main.rand.NextFloat(2f, 3f);

            Vector2 position = target.Center - direction * 10;
            ParticleSystem.NewParticle(position, direction.RotatedBy(randomRotation) * randomVel * 8, new SpeedParticle(), Color.LightYellow, 1);

            for (int i  = 0; i < 3; ++i)
            {
                float randomRotation2 = Main.rand.NextFloat(-0.5f, 0.5f);
                float randomVel2 = Main.rand.NextFloat(2f, 3f);

                Dust dust8 = Dust.NewDustPerfect(startPos, 292, direction.RotatedBy(randomRotation2) * 5 * randomVel2, 0);
                dust8.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                dust8.noGravity = true;
            }
            if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 10);
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<LightningStoneBuff>());
                return false;
            }
            if (owner.HasBuff(ModContent.BuffType<LightningStoneBuff>()))
                Projectile.timeLeft = 2;
            return true;
        }
        public override bool MinionContactDamage() => true;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado);
        }
        private float drawTimer;
        public float glowOpacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = rect.Size() / 2;
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            if (glowOpacity > 0)
                RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.LightGoldenrodYellow, Projectile.rotation, drawOrigin, Projectile.scale, opacity: Projectile.Opacity * glowOpacity);
            return false;
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
    class LightningStoneMinionGlobalNPC : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public static int GetMinionCount() => Main.projectile.Where(p => p.active && p.type == ModContent.ProjectileType<LightningStoneMinion>()).Count();
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ProjectileID.Sets.IsAWhip[projectile.type] && projectile.CountsAsClass(DamageClass.Summon))
            {
                int count = GetMinionCount();
                if (count > 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.type == ModContent.ProjectileType<LightningStoneMinion>() && proj.ModProjectile is LightningStoneMinion stoneMinion && stoneMinion.glowOpacity <= 0)
                        {
                            stoneMinion.glowOpacity = 1;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Zap2 with { Volume = .5f }, proj.position);
                            DustHelper.DrawParticleElectricity<LightningParticle>(RedeHelper.RandAreaInEntity(proj), target.Center, .51f, 20, 0.05f, 1);
                            DustHelper.DrawParticleElectricity<LightningParticle>(RedeHelper.RandAreaInEntity(proj), target.Center, .5f, 20, 0.05f, 1);
                            int hitDirection = target.RightOfDir(proj);
                            BaseAI.DamageNPC(target, proj.damage / 4, proj.knockBack * 2, hitDirection, proj);
                        }
                    }
                }
            }
        }
    }
}
