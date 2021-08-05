using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Redemption.Items.Critters;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;

namespace Redemption.NPCs.Critters
{
    public class GrandLarva : ModNPC
    {
        private enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Hop
        }
        public ref float AIState => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
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
            NPC.catchItem = (short)ModContent.ItemType<GrandLarvaBait>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == (float)ActionState.Hop;

        public Vector2 moveTo;
        public int hopCooldown;
        public int hitCooldown;
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            NPC.TargetClosest();
            NPC.LookByVelocity();
            if (hopCooldown > 0) hopCooldown--;
            switch (AIState)
            {
                case (float)ActionState.Begin:
                    TimerRand = Main.rand.Next(80, 180);
                    AIState = (float)ActionState.Idle;
                    break;
                case (float)ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    HopCheck();
                    break;
                case (float)ActionState.Wander:
                    HopCheck();
                    AITimer++;
                    if (AITimer >= TimerRand || (NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16))
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 180);
                        AIState = (float)ActionState.Idle;
                    }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1.2f, 2, 2, false);
                    break;
                case (float)ActionState.Hop:
                    hitCooldown--;
                    for (int k = 0; k < Main.npc.Length; k++)
                    {
                        NPC possibleTarget = Main.npc[k];
                        if (!possibleTarget.active || possibleTarget.whoAmI == NPC.whoAmI || !NPCTags.SkeletonHumanoid.Has(possibleTarget.type))
                            continue;
                        if (hitCooldown <= 0 && NPC.Hitbox.Intersects(possibleTarget.Hitbox))
                        {
                            BaseAI.DamageNPC(possibleTarget, NPC.damage, 2, null);
                            hitCooldown = 60;
                        }
                    }
                    if (BaseAI.HitTileOnSide(NPC, 3, true))
                    {
                        hitCooldown = 0;
                        moveTo = NPC.FindGround(15);
                        hopCooldown = 60;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    break;
            }
        }
        public void HopCheck()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (hopCooldown == 0 && Main.rand.NextBool(200) && NPC.Distance(player.Center) <= 60 && BaseAI.HitTileOnSide(NPC, 3, true))
            {
                NPC.velocity.X += player.Center.X < NPC.Center.X ? -5f : 5f;
                NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                AIState = (float)ActionState.Hop;
            }
            for (int k = 0; k < Main.npc.Length; k++)
            {
                NPC possibleTarget = Main.npc[k];
                if (!possibleTarget.active || possibleTarget.whoAmI == NPC.whoAmI || !NPCTags.SkeletonHumanoid.Has(possibleTarget.type))
                    continue;
                if (hopCooldown == 0 && Main.rand.NextBool(200) && NPC.Sight(possibleTarget, 60, false, true) && BaseAI.HitTileOnSide(NPC, 3, true))
                {
                    NPC.velocity.X += possibleTarget.Center.X < NPC.Center.X ? -5f : 5f;
                    NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                    AIState = (float)ActionState.Hop;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            switch (AIState)
            {
                case (float)ActionState.Begin:
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                        {
                            NPC.frame.Y = 0;
                        }
                    }
                    break;
                case (float)ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.frame.Y = 0;
                    }
                    else
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    break;
                case (float)ActionState.Wander:
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.frameCounter += NPC.velocity.X * 0.5f;
                        if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > 3 * frameHeight)
                            {
                                NPC.frame.Y = 0;
                            }
                        }
                    }
                    else
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    break;
                case (float)ActionState.Hop:
                    NPC.frame.Y = 5 * frameHeight;
                    break;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.04f;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement("Gross insects holding many flies within. Can be used as good bait.")
            });
        }
        public override void OnKill()
        {
            for (int i = 0; i < Main.rand.Next(7, 10); i++)
                NPC.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Fly>());
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (AIState == (float)ActionState.Idle)
            {
                moveTo = NPC.FindGround(10);
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = (float)ActionState.Wander;
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
}