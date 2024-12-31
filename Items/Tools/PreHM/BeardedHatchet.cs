using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.NPCs.Friendly.TownNPCs;
using Redemption.Projectiles.Melee;
using Steamworks;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.PreHM
{
    public class BeardedHatchet : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Increased chance to decapitate skeletons, guaranteeing skull drops" +
                "\nDeals 75% more damage to skeletons"); */

            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            Item.damage = 11;
            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 25;
            Item.useAnimation = 18;
            Item.axe = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = 6550;
            Item.rare = ItemRarityID.White;
            Item.UseSound = null;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeardedHatchet_Proj>();
            Item.Redemption().TechnicallyAxe = true;
        }
    }
    public abstract class BaseBeardedHatchet_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Tools/PreHM/BeardedHatchet";
        protected abstract Entity Owner { get; }
        protected abstract int NewLength { get; }

        private float[] oldrot = new float[6];
        private Vector2[] oldPos = new Vector2[6];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Owner is Player)
                return !target.friendly && progress >= 0.5f ? null : false;
            return !target.friendly && target.type != NPCID.TargetDummy && progress >= 0.5f ? null : false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            if (Owner is Player)
            {
                Projectile.usesOwnerMeleeHitCD = true;
                Projectile.ownerHitCheck = true;
                Projectile.ownerHitCheckDistance = 300f;
            }
            else
                Projectile.npcProj = true;

            Length = NewLength;
            Projectile.Redemption().IsAxe = true;
            Rot = MathHelper.ToRadians(3);
        }

        private Vector2 startVector;
        private Vector2 vector;
        private ref float Length => ref Projectile.localAI[0];
        private ref float Rot => ref Projectile.localAI[1];
        private float Timer;
        private float SwingSpeed;
        private int pauseTimer;
        private float glow;
        private Vector2 mouseOrig;
        private float progress;

        public override void AI()
        {
            Player player = Owner as Player;
            NPC npc = Owner as NPC;
            if (player != null && (player.noItems || player.CCed || player.dead || !player.active))
            {
                Projectile.Kill();
                return;
            }
            if (npc != null && (!npc.active || npc.type != ModContent.NPCType<Fallen>() || npc.ai[1] <= 5))
            {
                Projectile.Kill();
                return;
            }

            if (player != null)
            {
                SwingSpeed = SetSwingSpeed(1);
                progress = Timer / (18 * SwingSpeed);

                player.heldProj = Projectile.whoAmI;
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            else
            {
                SwingSpeed = 1;
                progress = Timer / 40;
            }

            Projectile.spriteDirection = Owner.direction;

            if (Main.myPlayer == Projectile.owner && --pauseTimer <= 0)
            {
                player?.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                if (Timer++ == 0)
                {
                    if (player != null)
                        mouseOrig = Main.MouseWorld;
                    startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - ((MathHelper.PiOver2 + 0.5f) * Projectile.spriteDirection));
                }

                if (Timer == (int)(17 * SwingSpeed))
                {
                    Vector2 vel = RedeHelper.PolarVector(8, (mouseOrig - Owner.Center).ToRotation());
                    if (player != null)
                        SoundEngine.PlaySound(CustomSounds.Swoosh1 with { Volume = .4f, Pitch = -.1f }, Projectile.position);
                    else if (npc != null)
                    {
                        vel = new(8 * npc.spriteDirection, 0);
                        SoundEngine.PlaySound(SoundID.Item71 with { Volume = .5f }, Owner.position);
                    }

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, vel, ModContent.ProjectileType<UkonvasaraSword_Wave>(), 0, 0, Projectile.owner);
                    Main.projectile[p].scale = .3f;
                }

                if (progress < (npc != null ? 0.45f : 0.5f))
                {
                    Rot = -MathHelper.ToRadians(60 * progress) * Projectile.spriteDirection;
                    vector = startVector.RotatedBy(Rot) * Length;
                }
                else
                {
                    if (npc != null)
                        Timer++;
                    Rot = MathHelper.ToRadians(220f * (0.6f + 0.3f * MathF.Atan(5 * MathHelper.Pi * (progress - 0.75f)))) * Projectile.spriteDirection;
                    vector = startVector.RotatedBy(Rot) * Length;
                }

                if (player != null)
                {
                    if (progress <= 0.666f)
                    {
                        glow += 0.06f / SwingSpeed;
                        glow = MathHelper.Clamp(glow, 0f, 1f);
                    }
                    else
                    {
                        glow -= 0.06f / SwingSpeed;
                        glow = MathHelper.Clamp(glow, 0f, 1f);
                    }
                }
                if (progress >= 1)
                    Projectile.Kill();
            }
            if (Timer > 1)
                Projectile.alpha = 0;

            Projectile.Center = (player != null ? player.MountedCenter : Owner.Center) + vector;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = Projectile.rotation;
            oldPos[0] = Projectile.Center;

            if (npc != null)
                v = RedeHelper.PolarVector(12, (Projectile.Center - npc.Center).ToRotation()) + RedeHelper.PolarVector(-12 * npc.spriteDirection, (Projectile.Center - npc.Center).ToRotation() - MathHelper.PiOver2);
        }
        private Vector2 v;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .4f, Pitch = .2f }, Projectile.position);
            if (Owner is Player player)
                player.RedemptionScreen().ScreenShakeIntensity += 3;
            pauseTimer = 4;

            Vector2 directionTo = target.DirectionTo(Owner.Center);
            Dust.NewDustPerfect(target.Center + directionTo * 5 + new Vector2(0, 70) + Owner.velocity, ModContent.DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f) + 3.14f) * Main.rand.NextFloat(4f, 5f) + (Owner.velocity / 2), 0, Color.White * .8f, 2.5f);

            bool skele = NPCLists.SkeletonHumanoid.Contains(target.type);
            bool humanoid = skele || NPCLists.Humanoid.Contains(target.type);
            if (target.life < target.lifeMax && target.life < damageDone * 100 && humanoid)
            {
                if (Main.rand.NextBool(skele ? 20 : 80))
                {
                    CombatText.NewText(target.getRect(), Color.Orange, Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Decapitated"));
                    target.Redemption().decapitated = true;
                    hit.Crit = true;
                    target.StrikeInstantKill();
                }
            }

            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Skeleton.Contains(target.type))
                modifiers.FinalDamage *= 1.75f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin;
            if (Owner is NPC npc)
            {
                origin = new(texture.Width / 2f + (-15 * npc.spriteDirection), texture.Height / 2f - 13);

                Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            else
            {
                origin = new(texture.Width / 2f, texture.Height / 2f);
                Vector2 v = RedeHelper.PolarVector(10, (Projectile.Center - Owner.Center).ToRotation());

                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = oldPos[k] - v - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Color.GhostWhite * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color * Projectile.Opacity * glow * .4f, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
                }

                Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            return false;
        }
    }
    public class BeardedHatchet_Proj : BaseBeardedHatchet_Proj
    {
        protected override Entity Owner => Main.player[Projectile.owner];
        protected override int NewLength => 48;
    }
}