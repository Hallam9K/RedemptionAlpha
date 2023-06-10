using System;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Buffs.Minions;
using Redemption.BaseExtension;

namespace Redemption.Projectiles.Minions
{
    public class MicroshieldDrone : ModProjectile
    {
        public float speed;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Microshield Drone");
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = false;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 0;
        }
        public override bool? CanCutTiles() => false;
        public bool iLoveRedemption;
        public float distance = 1000;
        public bool shieldDisabled;
        public int shieldAlpha;
        private void LookInDirection(Vector2 look)
        {
            float angle = 0.5f * (float)Math.PI;
            if (look.X != 0f)
            {
                angle = (float)Math.Atan(look.Y / look.X);
            }
            else if (look.Y < 0f)
            {
                angle += (float)Math.PI;
            }
            if (look.X < 0f)
            {
                angle += (float)Math.PI;
            }
            Projectile.rotation = angle - (float)Math.PI / 2;
        }
        Vector2 moveTo;
        public override void AI()
        {
            if (shieldDisabled)
                Projectile.frame = 3;
            else
            {
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 3)
                        Projectile.frame = 0;
                }
                Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.7f, 0f, 0f);
            }
            Player owner = Main.player[Projectile.owner];
            if (!CheckActive(owner))
                return;
            if (!iLoveRedemption)
            {
                Vector2 playerDir = Projectile.Center - owner.Center;
                LookInDirection(-playerDir);
                Projectile.Move(owner.Center, 30, 17);
                distance = 1000;
            }
            Projectile.localAI[1]++;
            for (int j = 0; j < Main.maxProjectiles; j++)
            {
                Projectile projectile = Main.projectile[j];
                if (!projectile.active || projectile.type == Type || !projectile.hostile || projectile.damage <= 0 || projectile.velocity == Vector2.Zero || projectile.ProjBlockBlacklist())
                    iLoveRedemption = false;
                else
                {
                    if (projectile.Hitbox.Intersects(Projectile.Hitbox) && !shieldDisabled)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(10, (Projectile.Center - owner.Center).ToRotation() + Main.rand.NextFloat(-0.05f, 0.05f)), ModContent.ProjectileType<GirusDischarge>(), 180, 3, Main.myPlayer);
                            }
                        }
                        if (projectile.damage < 500 - projectile.localAI[0])
                        {
                            projectile.velocity *= -1;
                            projectile.friendly = true;
                            projectile.hostile = false;
                            projectile.Redemption().ReflectDamageIncrease = 4;
                        }
                        Projectile.localAI[0] += projectile.damage * 0.75f;
                        CombatText.NewText(Projectile.getRect(), Color.MediumVioletRed, (int)(projectile.damage * 0.75f), true, true);
                        SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
                        if (Projectile.localAI[0] >= 500)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath56, Projectile.position);
                            for (int i = 0; i < 7; i++)
                            {
                                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 100, default, 1.5f);
                                Main.dust[dustIndex].velocity *= 1.9f;
                            }
                            Projectile.localAI[0] = 0;
                            shieldDisabled = true;
                        }
                    }
                    float shootToX = projectile.position.X + projectile.width * 0.5f - Projectile.Center.X;
                    float shootToY = projectile.position.Y - Projectile.Center.Y;
                    float distanceNew = (float)Math.Sqrt(shootToX * shootToX + shootToY * shootToY);
                    if (distanceNew < distance)
                    {
                        distance = distanceNew;
                        iLoveRedemption = true;
                        Vector2 distFalseMag = projectile.Center - owner.Center;
                        Vector2 playerFalseMag = Projectile.Center - owner.Center;
                        LookInDirection(-playerFalseMag);
                        float distFromPlayer = 60;
                        float distTrueMag = Vector2.Distance(projectile.Center, owner.Center);
                        if ((projectile.velocity.X > 0 && distFalseMag.X < 0 || projectile.velocity.X < 0 && distFalseMag.X > 0 && projectile.type != Type || Math.Abs(projectile.velocity.X) < 1) && (projectile.velocity.Y > 0 && distFalseMag.Y < 0 || projectile.velocity.Y < 0 && distFalseMag.Y > 0 || Math.Abs(projectile.velocity.Y) < 1))
                        {
                            if (Projectile.localAI[1] % 20 == 0)
                                moveTo = new Vector2(distFalseMag.X * distFromPlayer / distTrueMag, distFalseMag.Y * distFromPlayer / distTrueMag);
                        }
                        Projectile.Move(owner.Center + moveTo, 10, 10);
                    }
                    else
                        iLoveRedemption = false;
                }
            }
            if (shieldDisabled)
            {
                if (++Projectile.localAI[0] >= 600)
                {
                    shieldDisabled = false;
                    Projectile.localAI[0] = 0;
                }
            }
            else
            {
                if (Projectile.localAI[0] > 0 && Projectile.localAI[1] % 60 == 0)
                    Projectile.localAI[0]--;
            }
            if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
            {
                Projectile.position = owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<MicroshieldDroneBuff>());
                return false;
            }
            if (owner.HasBuff(ModContent.BuffType<MicroshieldDroneBuff>()))
                Projectile.timeLeft = 2;
            return true;
        }
        public override bool MinionContactDamage() => false;
    }
}
