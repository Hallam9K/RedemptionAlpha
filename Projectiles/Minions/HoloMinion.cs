using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class HoloMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hologram");
            Main.projFrames[Projectile.type] = 2;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 60;
            Projectile.usesLocalNPCImmunity = true;
        }
        int mode = 0;
        const int chickenMode = 0;
        const int eyeMode = 1;
        const int swordMode = 2;
        Vector2 flyOffset;
        Vector2 flyTo;
        int counter = 0;
        NPC target;
        int attackCooldown = 0;
        void DustPuff()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, RedeHelper.PolarVector(Main.rand.Next(4), Main.rand.NextFloat() * (float)Math.PI * 2));
                d.noGravity = true;
            }
        }
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!CheckActive(player))
                return;

            OverlapCheck();
            if (counter % 60 == 0)
            {
                flyOffset = RedeHelper.PolarVector(Main.rand.Next(100), Main.rand.NextFloat() * (float)Math.PI * 2);
            }
            counter++;
            attackCooldown--;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > (mode == eyeMode ? 3 : 1))
                    Projectile.frame = 0;
            }
            switch (mode)
            {
                case chickenMode:
                    flyTo = player.Center + flyOffset - 30 * Vector2.UnitY;
                    if ((player.Center - Projectile.Center).Length() < 400)
                    {
                        if (RedeHelper.ClosestNPC(ref target, 10000, Projectile.Center, false, player.MinionAttackTargetNPC))
                        {
                            mode = eyeMode;
                            DustPuff();
                        }
                    }
                    Projectile.rotation = Projectile.velocity.X * (float)Math.PI / 40f;
                    break;
                case eyeMode:
                    if (RedeHelper.ClosestNPC(ref target, 10000, Projectile.Center, false, player.MinionAttackTargetNPC))
                    {
                        Vector2 diff = target.Center - Projectile.Center;
                        if (diff.Length() < 350)
                        {
                            mode = swordMode;
                            DustPuff();
                        }
                        else
                        {
                            Projectile.rotation = diff.ToRotation() + (float)Math.PI;
                            if (attackCooldown < 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item75, Projectile.position);
                                int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(25, diff.ToRotation()), ProjectileID.LaserMachinegunLaser, Projectile.damage, Projectile.knockBack, player.whoAmI);
                                Main.projectile[p].DamageType = DamageClass.Summon;
                                Main.projectile[p].netUpdate = true;
                                attackCooldown = 60;
                            }
                            flyTo = player.Center + flyOffset + RedeHelper.PolarVector(-50, diff.ToRotation());
                        }
                    }
                    else
                    {
                        mode = chickenMode;
                    }
                    break;
                case swordMode:
                    if (attackCooldown < 0)
                    {
                        if (RedeHelper.ClosestNPC(ref target, 400, Projectile.Center, true, player.MinionAttackTargetNPC))
                        {
                            flyTo = target.Center + flyOffset * .5f;
                            Vector2 diff = flyTo - Projectile.Center;
                            Projectile.velocity = RedeHelper.PolarVector(20, diff.ToRotation());
                            attackCooldown = 20;

                        }
                        else
                        {
                            mode = chickenMode;
                            DustPuff();
                        }
                    }
                    if (attackCooldown < 10)
                        Projectile.velocity *= .8f;
                    Projectile.rotation += Projectile.velocity.X * (float)Math.PI * 2 / 30f;
                    break;
            }

            if (mode != swordMode)
            {
                Vector2 diff = flyTo - Projectile.Center;
                if (diff.Length() < 10)
                    Projectile.velocity = diff;
                else
                    Projectile.velocity = RedeHelper.PolarVector(10, diff.ToRotation());
            }
            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            Projectile.alpha += Main.rand.Next(-10, 11);
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 30, 60);
        }
        public override bool MinionContactDamage()
        {
            return mode == swordMode;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<HoloMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<HoloMinionBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        private void OverlapCheck()
        {
            float overlapVelocity = 0.04f;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                        Projectile.velocity.X -= overlapVelocity;
                    else
                        Projectile.velocity.X += overlapVelocity;

                    if (Projectile.position.Y < other.position.Y)
                        Projectile.velocity.Y -= overlapVelocity;
                    else
                        Projectile.velocity.Y += overlapVelocity;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            if (mode != chickenMode)
            {
                if (mode == eyeMode)
                    texture = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "3").Value;
                else
                    texture = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "2").Value;
            }
            int frameHeight = texture.Height / (mode == eyeMode ? 4 : 2);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight), Projectile.GetAlpha(new Color(255, 255, 255, 0)), Projectile.rotation, new Vector2(texture.Width, frameHeight) * .5f, Vector2.One, (Projectile.velocity.X > 0 && mode != eyeMode) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }
    }
}