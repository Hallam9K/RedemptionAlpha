using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Redemption.Base;
using System;
using Redemption.Buffs.Debuffs;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class BloodstainedPike_Proj : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 60f;
        protected virtual float HoldoutRangeMax => 126f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodstained Pike");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.GetGlobalProjectile<RedeProjectile>().TechnicallyMelee = true;
        }

        private readonly List<int> skewered = new();
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax;

            player.heldProj = Projectile.whoAmI;

            if (Projectile.timeLeft > duration)
                Projectile.timeLeft = duration;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            float halfDuration = duration * 0.5f;
            float progress;

            if (Projectile.timeLeft < halfDuration)
            {
                if (player.channel)
                {
                    player.itemTime = player.itemAnimationMax / 2;
                    player.itemAnimation = player.itemAnimationMax / 2;
                    Projectile.timeLeft = (int)halfDuration;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (skewered.Contains(i))
                        {
                            NPC npc = Main.npc[i];
                            if (npc.life <= 0 || !npc.active)
                                skewered.Remove(i);
                            npc.Center = Projectile.Center;
                            int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, Scale: 2f);
                            Main.dust[dust].noGravity = true;
                            if (skewered.Count >= 5)
                            {
                                for (int k = 0; k < 20; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(npc.Center + vector, 2, 2, DustID.Ichor, Scale: 2)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -npc.DirectionTo(dust2.position) * 10f;
                                }
                                BaseAI.KillNPCWithLoot(npc);
                            }
                        }
                    }
                }
                else
                    skewered.Clear();

                progress = Projectile.timeLeft / halfDuration;
            }
            else
                progress = (duration - Projectile.timeLeft) / halfDuration;

            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (skewered.Count < 5 && player.channel && (Projectile.timeLeft > player.itemAnimationMax / 2 || player.velocity.Length() > 1) && (target.life <= 500 || target.life <= target.lifeMax / 10) && !target.boss && target.width < 100 && target.height < 100 && !target.dontTakeDamage && !target.immortal)
            {
                if (target.life > 0)
                    skewered.Add(target.whoAmI);
            }
        }
        public override void Kill(int timeLeft)
        {
            skewered.Clear();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !skewered.Contains(target.whoAmI) ? null : false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 v = Projectile.Center - RedeHelper.PolarVector(64, (Projectile.Center - player.Center).ToRotation());
            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}