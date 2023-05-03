using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Minions;
using Redemption.Dusts;
using Redemption.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class MoonflareGuardian : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ElementID.ProjFire[Type] = true;
            ElementID.ProjNature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 50;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;
        NPC target;
        int projBuffed;
        private readonly List<int> targets = new();
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            CheckActive(owner);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            OverlapCheck();

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, .8f * Projectile.Opacity, .6f * Projectile.Opacity);
            Projectile.LookByVelocity();
            if (Projectile.localAI[0]++ >= 180 && !Main.dayTime && Main.moonPhase != 4)
            {
                targets.Clear();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || !proj.minion || proj.type == Type || proj.GetGlobalProjectile<MoonflareMinionBuff>().moonflareBuff > 0)
                        continue;

                    targets.Add(proj.whoAmI);
                    int[] targetsArr = targets.ToArray();
                    projBuffed = Utils.Next(Main.rand, targetsArr);

                    SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, Projectile.position);
                    Projectile.localAI[1] = 1;
                }
                Projectile.localAI[0] = 0;
            }
            if (Projectile.localAI[1] == 1)
            {
                if (projBuffed == -1)
                {
                    Projectile.localAI[0] = 0;
                    Projectile.localAI[1] = 0;
                }
                else
                {
                    Projectile p = Main.projectile[projBuffed];
                    Projectile.velocity *= 0.9f;
                    if (Projectile.localAI[0]++ % 10 == 0 && Projectile.localAI[0] < 60)
                        RedeDraw.SpawnCirclePulse(Projectile.Center, Color.LightGoldenrodYellow, 0.1f + (Projectile.localAI[0] / 100), Projectile);
                    if (Projectile.localAI[0]++ % 10 == 0 && Projectile.localAI[0] >= 60)
                        RedeDraw.SpawnCirclePulse(p.Center, Color.LightGoldenrodYellow, 1.2f - (Projectile.localAI[0] / 100), p);
                    if (Projectile.localAI[0] >= 120)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, p.position);
                        DustHelper.DrawCircle(p.Center, ModContent.DustType<MoonflareDust>(), 4, 2, 2, nogravity: true);
                        p.GetGlobalProjectile<MoonflareMinionBuff>().moonflareBuff = 300;
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 0;
                    }
                }
            }
            else
            {
                if (RedeHelper.ClosestNPC(ref target, 900, Projectile.Center, false, owner.MinionAttackTargetNPC))
                {
                    if (Projectile.DistanceSQ(target.Center) >= 200 * 200)
                        Projectile.Move(target.Center, 10, 40);
                    else
                        Projectile.velocity *= 0.98f;

                    if (++Projectile.ai[0] % 80 == 0 && Main.myPlayer == owner.whoAmI)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(target.Center).RotatedByRandom(0.8f) * 4, ModContent.ProjectileType<MoonflareGuardian_Proj>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                    }
                }
                else
                {
                    if (Projectile.DistanceSQ(owner.Center) >= 200 * 200)
                        Projectile.Move(owner.Center, Projectile.DistanceSQ(owner.Center) > 700 * 700 ? 18 : 10, 40);
                    else
                        Projectile.velocity *= 0.98f;
                }
            }
            if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
            {
                Projectile.position = owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
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
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<MoonflareGuardianBuff>()))
                Projectile.timeLeft = 2;
        }
    }
    public class MoonflareMinionBuff : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int moonflareBuff;
        public int origDmg;

        public override void PostAI(Projectile projectile)
        {
            if (moonflareBuff <= 0)
                return;
            projectile.damage += 3;
            if (Main.rand.NextBool(8))
            {
                int d = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<MoonflareDust>(), 0, 0);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.Y -= 2;
                Main.dust[d].velocity.X *= 0;
            }

            Lighting.AddLight(projectile.Center, projectile.Opacity, .8f * projectile.Opacity, .6f * projectile.Opacity);
            moonflareBuff--;
        }
    }
}
