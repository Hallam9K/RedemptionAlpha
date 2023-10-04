using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Critters;
using Redemption.Items.Placeable.Banners;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.Critters
{
    public class GrandLarva : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Hop
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Shimmerfly;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 16;
            NPC.defense = 0;
            NPC.damage = 2;
            NPC.lifeMax = 35;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
            NPC.catchItem = (short)ModContent.ItemType<GrandLarvaBait>();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<GrandLarvaBanner>();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Hop;

        public Vector2 moveTo;
        public int hopCooldown;
        public int hitCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 180);
        }
        public override void AI()
        {
            NPC.GetNearestAlivePlayer();
            NPC.TargetClosest();
            NPC.LookByVelocity();

            if (hopCooldown > 0)
                hopCooldown--;

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;

                    AITimer++;

                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    HopCheck();
                    break;

                case ActionState.Wander:
                    HopCheck();
                    AITimer++;

                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 180);
                        AIState = ActionState.Idle;
                    }

                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1.2f, 2, 2, false);
                    break;

                case ActionState.Hop:
                    hitCooldown--;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC possibleTarget = Main.npc[i];
                        if (!possibleTarget.active || possibleTarget.whoAmI == NPC.whoAmI ||
                            !NPCLists.Undead.Contains(possibleTarget.type) &&
                            !NPCLists.SkeletonHumanoid.Contains(possibleTarget.type))
                            continue;

                        if (hitCooldown > 0 || !NPC.Hitbox.Intersects(possibleTarget.Hitbox))
                            continue;

                        BaseAI.DamageNPC(possibleTarget, NPC.damage, 2, NPC);
                        possibleTarget.AddBuff(ModContent.BuffType<InfestedDebuff>(), Main.rand.Next(300, 900));
                        hitCooldown = 60;
                    }

                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        hitCooldown = 0;
                        moveTo = NPC.FindGround(15);
                        hopCooldown = 60;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    break;
            }
        }

        public void HopCheck()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            if (hopCooldown == 0 && Main.rand.NextBool(200) && player.active && !player.dead && NPC.DistanceSQ(player.Center) <= 60 * 60 &&
                BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.velocity.X += player.Center.X < NPC.Center.X ? -5f : 5f;
                NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                AIState = ActionState.Hop;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC possibleTarget = Main.npc[i];
                if (!possibleTarget.active || possibleTarget.whoAmI == NPC.whoAmI ||
                    !NPCLists.Undead.Contains(possibleTarget.type) && !NPCLists.SkeletonHumanoid.Contains(possibleTarget.type))
                    continue;

                if (hopCooldown == 0 && Main.rand.NextBool(200) && NPC.Sight(possibleTarget, 60, false, true) &&
                    BaseAI.HitTileOnSide(NPC, 3))
                {
                    NPC.velocity.X += possibleTarget.Center.X < NPC.Center.X ? -5f : 5f;
                    NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                    AIState = ActionState.Hop;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Hop)
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 5 * frameHeight;
                return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float cave = SpawnCondition.Cavern.Chance * 0.07f;
            float desert = SpawnCondition.OverworldDayDesert.Chance * (spawnInfo.Player.ZoneBeach ? 0f : 0.1f);
            float desertUG = SpawnCondition.DesertCave.Chance * 0.1f;
            return Math.Max(cave, Math.Max(desert, desertUG));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundDesert,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.GrandLarva"))
            });
        }

        public override void OnKill()
        {
            for (int i = 0; i < Main.rand.Next(4, 7); i++)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Fly>(), ai3: 1);
        }

        public override bool CanHitNPC(NPC target) => target.lifeMax > 5;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<InfestedDebuff>(), Main.rand.Next(300, 900));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            target.AddBuff(ModContent.BuffType<InfestedDebuff>(), Main.rand.Next(300, 900));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AIState == ActionState.Idle)
            {
                moveTo = NPC.FindGround(10);
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = ActionState.Wander;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 12; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}