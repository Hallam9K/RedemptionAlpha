using System;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Critters;
using Redemption.NPCs.PreHM;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.Localization;

namespace Redemption.NPCs.Critters
{
    public class Fly : ModNPC
    {
        public enum ActionState
        {
            Flying,
            Landed
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ref float Aggressive => ref NPC.ai[3];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Shimmerfly;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<DevilScentedDebuff>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 8;
            NPC.height = 8;
            NPC.lifeMax = 1;
            NPC.damage = 2;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.npcSlots = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.catchItem = (short)ModContent.ItemType<FlyBait>();
        }

        public NPC npcTarget;
        public Vector2 moveTo;
        private SlotId loop;
        private float loopVolume;
        public int hitCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            if (Aggressive == 1)
                NPC.scale = 1.2f;

            TimerRand = Main.rand.Next(240, 600);
            NPC.velocity = RedeHelper.PolarVector(10, RedeHelper.RandomRotation());
        }
        public override void AI()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            NPC.TargetClosest();

            if (hitCooldown > 0)
                hitCooldown--;

            if (Math.Abs(NPC.velocity.X) > 0.2)
                NPC.spriteDirection = -NPC.direction;

            if (NPC.collideX && NPC.velocity.X != NPC.oldVelocity.X)
                NPC.velocity.X = -NPC.oldVelocity.X;

            if (NPC.collideY && NPC.velocity.Y != NPC.oldVelocity.Y)
                NPC.velocity.Y = -NPC.oldVelocity.Y;

            loopVolume = 0;
            switch (AIState)
            {
                case ActionState.Flying:
                    NPC.noGravity = true;
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;

                    loopVolume = 1 * NPC.scale;

                    if (NPC.velocity.Length() < 4)
                        NPC.velocity = RedeHelper.PolarVector(10, RedeHelper.RandomRotation());

                    NPC.velocity = NPC.velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f));
                    AITimer++;
                    Entity moveTarget;
                    if (Aggressive == 1)
                    {
                        moveTarget = player;
                        if (AITimer % 20 == 0)
                            NPC.velocity = NPC.DirectionTo(moveTarget.Center) * 10f;
                    }
                    else
                    {
                        if (player.RedemptionPlayerBuff().devilScented)
                            Aggressive = 1;
                        float nearestNPCDist = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC possibleTarget = Main.npc[i];
                            if (!possibleTarget.active || possibleTarget.whoAmI == NPC.whoAmI)
                                continue;

                            if (!possibleTarget.RedemptionNPCBuff().devilScented && !NPCLists.Undead.Contains(possibleTarget.type) &&
                                !NPCLists.SkeletonHumanoid.Contains(possibleTarget.type) &&
                                possibleTarget.type != ModContent.NPCType<DevilsTongue>())
                                continue;

                            if (nearestNPCDist != -1 && !(possibleTarget.Distance(NPC.Center) < nearestNPCDist))
                                continue;

                            nearestNPCDist = possibleTarget.Distance(NPC.Center);
                            moveTarget = possibleTarget;
                            if (AITimer % 20 == 0)
                                NPC.velocity = NPC.DirectionTo(moveTarget.Center) * 10f;
                        }
                        CheckNPCHit();
                    }
                    if (AITimer >= TimerRand)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 320);
                        AIState = ActionState.Landed;
                    }

                    break;

                case ActionState.Landed:
                    AITimer++;
                    NPC.rotation = 0;
                    NPC.noGravity = false;

                    if (BaseAI.HitTileOnSide(NPC, 3, false))
                        NPC.velocity *= 0;
                    else
                    {
                        NPC.velocity.Y++;
                        NPC.velocity.X *= 0.98f;
                    }

                    if (AITimer >= TimerRand)
                    {
                        NPC.velocity.Y -= 10;
                        NPC.velocity = RedeHelper.PolarVector(10, RedeHelper.RandomRotation());
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 600);
                        AIState = ActionState.Flying;
                    }

                    if (NPC.ClosestNPCToNPC(ref npcTarget, 100, NPC.Center) && npcTarget.lifeMax > 5 && !npcTarget.Redemption().invisible)
                    {
                        NPC.velocity.Y -= 10;
                        NPC.velocity = RedeHelper.PolarVector(10, RedeHelper.RandomRotation());
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 600);
                        AIState = ActionState.Flying;
                    }

                    if (NPC.DistanceSQ(player.Center) <= 100 * 100)
                    {
                        NPC.velocity.Y -= 10;
                        NPC.velocity = RedeHelper.PolarVector(10, RedeHelper.RandomRotation());
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 600);
                        AIState = ActionState.Flying;
                    }

                    break;
            }
            CustomSounds.UpdateLoopingSound(ref loop, CustomSounds.FlyBuzz with { MaxInstances = 3 }, loopVolume, 0, NPC.position);
            if (NPC.wet)
            {
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.position);
                loopVolume = 0;
                CustomSounds.UpdateLoopingSound(ref loop, CustomSounds.FlyBuzz with { MaxInstances = 3 }, loopVolume, 0, NPC.position);
                NPC.active = false;
            }
        }

        public void CheckNPCHit()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC possibleTarget = Main.npc[i];
                if (!possibleTarget.active || possibleTarget.whoAmI == NPC.whoAmI)
                    continue;

                if (!possibleTarget.RedemptionNPCBuff().devilScented && !NPCLists.Undead.Contains(possibleTarget.type) &&
                    !NPCLists.SkeletonHumanoid.Contains(possibleTarget.type))
                    continue;

                if (hitCooldown > 0 || !NPC.Hitbox.Intersects(possibleTarget.Hitbox))
                    continue;

                if (Main.rand.NextBool(3))
                    possibleTarget.AddBuff(ModContent.BuffType<InfestedDebuff>(), Main.rand.Next(60, 180));
                hitCooldown = 30;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (AIState == ActionState.Landed && NPC.velocity.Y == 0)
                NPC.frame.Y = 0;
            else
            {
                NPC.frameCounter++;

                if (!(NPC.frameCounter >= 4))
                    return;

                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override bool CheckDead()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X * 0.5f,
                    NPC.velocity.Y * 0.5f);
                NPC.DeathSound = SoundID.NPCDeath1 with { Volume = 0 };
                NPC.immuneTime = 30;
                NPC.life = 1;
                return false;
            }

            NPC.DeathSound = SoundID.NPCDeath1;
            loopVolume = 0;
            CustomSounds.UpdateLoopingSound(ref loop, CustomSounds.FlyBuzz with { MaxInstances = 3 }, loopVolume, 0, NPC.position);
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Fly"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life < 0)
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood,
                    NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => Aggressive == 1 && !target.dontHurtCritters;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => target.noKnockback = true;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<InfestedDebuff>(), Main.rand.Next(60, 180));
        }
    }
}