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
    public class CoastScarab : ModNPC
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
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 12;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.catchItem = (short)ModContent.ItemType<CoastScarabItem>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SandBlock, 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CoastScarabShell>(), 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Ocean.Chance * 0.4f;
        }
        NPC target;
        public Vector2 moveTo;
        public int hopCooldown;
        public override void AI()
        {
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
                    NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(10);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    if (hopCooldown == 0 && RedeHelper.ClosestNPC(ref target, 32, NPC.Center) && target.damage > 0)
                    {
                        NPC.velocity.X *= target.Center.X < NPC.Center.X ? 1.4f : -1.4f;
                        NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                        AIState = (float)ActionState.Hop;
                    }
                    if (RedeHelper.ClosestNPC(ref target, 100, NPC.Center) && target.damage > 0)
                    {
                        moveTo = NPC.FindGround(10);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    break;
                case (float)ActionState.Wander:
                    if (hopCooldown == 0 && RedeHelper.ClosestNPC(ref target, 32, NPC.Center) && target.damage > 0)
                    {
                        NPC.velocity.X *= target.Center.X < NPC.Center.X ? 1.4f : -1.4f;
                        NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                        AIState = (float)ActionState.Hop;
                    }
                    if (RedeHelper.ClosestNPC(ref target, 100, NPC.Center) && target.damage > 0)
                    {
                        RedeHelper.HorizontallyMove(NPC, new Vector2(target.Center.X < NPC.Center.X ? NPC.Center.X + 50 : NPC.Center.X - 50, NPC.Center.Y), 0.5f, 2, 4, 4, false);
                        return;
                    }
                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Distance(moveTo * 16) < 20)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Idle;
                    }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1, 4, 4, false);
                    break;
                case (float)ActionState.Hop:
                    if (BaseAI.HitTileOnSide(NPC, 3, false))
                    {
                        moveTo = NPC.FindGround(10);
                        hopCooldown = 180;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    break;
            }
            if (NPC.wet && Main.rand.NextBool(20))
            {
                int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height / 2, DustID.SilverCoin, 0, 0, 20, default, 1f);
                Main.dust[sparkle].velocity *= 0;
                Main.dust[sparkle].noGravity = true;
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
                    NPC.frame.Y = 0;
                    break;
                case (float)ActionState.Wander:
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
                case (float)ActionState.Hop:
                    NPC.frame.Y = frameHeight;
                    break;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				new FlavorTextBestiaryInfoElement("A species of scarab commonly found scuttering around the beach. They're rather cute and harmless creatures, but can be used to create dyes if you're a greedy monster.")
            });
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
                int gore1 = ModContent.Find<ModGore>("Redemption/CoastScarabGore1").Type;
                int gore2 = ModContent.Find<ModGore>("Redemption/CoastScarabGore2").Type;
                Gore.NewGore(NPC.position, NPC.velocity, gore1, 1f);
                Gore.NewGore(NPC.position, NPC.velocity, gore2, 1f);
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
}