using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles.Ranged;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara_Hammer : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Melee/Ukonvasara";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukonvasara");
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override bool ShouldUpdatePosition() => TeleportTrigger;
        public override void SetSafeDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
            Projectile.alpha = 255;
            Projectile.scale = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().IsHammer = true;
            Projectile.noEnchantmentVisuals = true;
        }
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float SwingSpeed;
        public int pauseTimer;
        public float progress;
        private Player Player => Main.player[Projectile.owner];

        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;
            Projectile.timeLeft = 2;

            if (!TeleportTrigger && !LaunchTrigger)
                Swing();

            if (TeleportTrigger && !LaunchTrigger)
                Teleport();

            if (TeleportTrigger && LaunchTrigger)
                Launch();

            if (Projectile.penetrate == 0)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.7f, Projectile.Opacity * 1f, Projectile.Opacity * 1f);
        }

        public void Swing()
        {
            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            SwingSpeed = 1 / Player.GetAttackSpeed(DamageClass.Melee);
            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            Projectile.Center = armCenter + vector;

            Projectile.spriteDirection = Player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + 3 * MathHelper.PiOver4;

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            progress = Timer / (60 * 100 * SwingSpeed);
            if (Timer++ == 0)
            {
                Projectile.scale *= Projectile.ai[2];
                Length = 55 * Projectile.ai[2];
                startVector = RedeHelper.PolarVector(1, Projectile.spriteDirection * MathHelper.PiOver2 + MathHelper.PiOver2) * Length;
            }
            if (Timer == (int)(30 * 100 * SwingSpeed))
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Swoosh1 with { Pitch = -.6f }, Player.position);
            }
            if (progress < 0.3f)
            {
                Rot = -MathHelper.ToRadians(120 + 100f * MathF.Atan(5 * MathHelper.Pi * (progress - 0.5f))) * Projectile.spriteDirection;
                vector = startVector.RotatedBy(Rot);
            }
            else if (progress < 1f)
            {
                Projectile.friendly = true;
                Rot = MathHelper.ToRadians(120 + 100f * MathF.Atan(5 * MathHelper.Pi * (progress - 0.5f))) * Projectile.spriteDirection;
                vector = startVector.RotatedBy(Rot);
            }
            else
                Projectile.Kill();

            if (Timer == 2)
            {
                Projectile.alpha = 0;
            }
        }

        public bool TeleportTrigger;
        public bool LaunchTrigger;
        public float TeleportTimer;
        private List<Vector2> cache;
        NPC initialTarget;
        NPC target;
        public void Teleport()
        {
            TeleportTimer++;
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 10; i++)
                {
                    cache.Add(initialTarget.Center);
                }
            }
            RedeHelper.SpecialCondition a = PossibleTarget;
            if (RedeHelper.ClosestNPC(ref target, 500, Projectile.Center + new Vector2(250 * Player.direction, 0), true, -1, a))
            {
                target = (NPC)RedeHelper.FindClosestNPC(ref target, 500, Projectile.Center + new Vector2(250 * Player.direction, 0), true, -1, a);
                cache.Add(target.Center);
                int hitDirection = target.RightOfDir(Projectile);
                BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                //Projectile.Move(target.Center, 15, 0);
            }
            else
            {
                Projectile.velocity *= 0;
                Projectile.friendly = false;
                LaunchTrigger = true;
            }
        }
        public bool PossibleTarget(NPC next)
        {
            if (next.immune[Player.heldProj] > 0)
                return false;
            return true;
        }

        public float LaunchTimer;
        public float floating;
        NPC target2;
        public void Launch()
        {
            if (LaunchTimer == 0)
                Projectile.damage *= 5;

            //float progress = MathHelper.Clamp(LaunchTimer / 3000, 0, 1);
            //Projectile.Center += new Vector2(0, -0.16f * (1 - progress));
            LaunchTimer += 1 / MathF.Sqrt(SwingSpeed);
            Projectile.rotation += LaunchTimer * 0.000002f;

            if (Main.rand.NextBool(500))
            {
                for (int i = 0; i < 2; i++)
                    DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center - new Vector2(20 * Projectile.direction, 0), Projectile.Center - new Vector2(20 * Projectile.direction, 0) + RedeHelper.PolarVector(Main.rand.Next(70, 121), RedeHelper.RandomRotation()), .8f, 10, 0.2f);
            }

            if (LaunchTimer < 6000)
            {
                Projectile.Move(Player.Center + new Vector2(-Player.direction * 200f, -100), 1f, 1000);
            }

            if (LaunchTimer > 6000)
            {
                Projectile.friendly = true;
                Projectile.penetrate = 1;
                if (RedeHelper.ClosestNPC(ref target2, 1000, Projectile.Center))
                    Projectile.Move(target2.Center, 1f, 10);
                else
                {
                    Projectile.Move(Player.Center, 0.3f, 10);
                    if (Vector2.Distance(Projectile.Center, Player.Center) < 20f)
                        Projectile.Kill();
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 180);
            TeleportTrigger = true;
            initialTarget = target;
            target.immune[Player.heldProj] = 20;
            Vector2 directionTo = target.DirectionTo(Projectile.Center);
            float num = LaunchTrigger ? 2f : 1;
            if (LaunchTrigger)
            {
                for (int i = 0; i < 8; i++)
                    Dust.NewDustPerfect(target.Center + directionTo * 5 + new Vector2(0, 70), ModContent.DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f) + 3.14f) * Main.rand.NextFloat(4f * num, 5f * num), 0, Color.White * .8f, 3f);
            }
            if (Main.myPlayer == Projectile.owner)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center + new Vector2(Main.rand.NextFloat(-2, 2)), Vector2.Zero, ModContent.ProjectileType<UkonArrowStrike>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 1);
                Main.projectile[p].DamageType = DamageClass.Melee;
                Main.projectile[p].localAI[0] = 35;
                Main.projectile[p].alpha = 0;
                Main.projectile[p].position.Y -= 540;
                Main.projectile[p].frame = 12;
                Main.projectile[p].netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            //Chain lightning

            Texture2D Dash = ModContent.Request<Texture2D>("Redemption/Textures/Trails/Lightning2", AssetRequestMode.ImmediateLoad).Value;
            Rectangle Dashrectangle = Dash.Frame();
            Vector2 Dashorigin = Dashrectangle.Size() / 2f;
            if (LaunchTrigger)
            {
                for (int k = 0; k < cache.Count - 1; k++)
                {
                    Vector2 DashCenter = 0.5f * (cache[k] + cache[k + 1]);
                    float DashRot = (cache[k] - cache[k + 1]).ToRotation();
                    float DashLength = (cache[k] - cache[k + 1]).Length();
                    float opacity = 1 - LaunchTimer / 3000;
                    Vector2 DashScale = new(DashLength * 0.004f, 0.5f);
                    Main.EntitySpriteDraw(Dash, DashCenter - Main.screenPosition, new Rectangle?(Dashrectangle), Color.LightCyan with { A = 0 } * 0.8f * opacity, DashRot, Dashorigin, DashScale, 0, 0);
                }
            }

            float opacity2 = MathHelper.Clamp(LaunchTimer / 3000, 0, 1);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Color.White with { A = 0 } * 0.6f * opacity2, Projectile.rotation, origin, Projectile.scale * 1.5f, spriteEffects, 0);
            return false;
        }
    }
}
