using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Critters;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.Critters
{
    public class BoneSpider : ModNPC
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
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 10;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.catchItem = (short) ModContent.ItemType<BoneSpiderItem>();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public NPC npcTarget;
        public Vector2 moveTo;
        public int hopCooldown;

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.LookByVelocity();

            if (hopCooldown > 0)
                hopCooldown--;

            switch (AIState)
            {
                case (float) ActionState.Begin:
                    TimerRand = Main.rand.Next(80, 180);
                    AIState = (float) ActionState.Idle;
                    break;

                case (float) ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;

                    AITimer++;

                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float) ActionState.Wander;
                    }

                    HopCheck();

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float) ActionState.Wander;
                    }

                    break;

                case (float) ActionState.Wander:
                    HopCheck();

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0)
                    {
                        RedeHelper.HorizontallyMove(NPC,
                            new Vector2(npcTarget.Center.X < NPC.Center.X ? NPC.Center.X + 50 : NPC.Center.X - 50,
                                NPC.Center.Y), 0.5f, 2.5f, 4, 4, false);
                        return;
                    }

                    AITimer++;

                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float) ActionState.Idle;
                    }

                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1.5f, 6, 6, false);
                    break;

                case (float) ActionState.Hop:
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        moveTo = NPC.FindGround(15);
                        hopCooldown = 60;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float) ActionState.Wander;
                    }

                    break;
            }
        }

        public void HopCheck()
        {
            if (hopCooldown != 0 || !BaseAI.HitTileOnSide(NPC, 3) ||
                !RedeHelper.ClosestNPC(ref npcTarget, 60, NPC.Center) || npcTarget.damage <= 0)
                return;

            NPC.velocity.X *= npcTarget.Center.X < NPC.Center.X ? 2f : -2f;
            NPC.velocity.Y = Main.rand.NextFloat(-2f, -7f);
            AIState = (float) ActionState.Hop;
        }

        public override void FindFrame(int frameHeight)
        {
            switch (AIState)
            {
                case (float) ActionState.Begin:
                    NPC.frameCounter += NPC.velocity.X * 0.5f;

                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;

                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }

                    break;

                case (float) ActionState.Idle:
                    NPC.frame.Y = 0;
                    break;

                case (float) ActionState.Wander:
                    NPC.frameCounter += NPC.velocity.X * 0.5f;

                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }

                    break;

                case (float) ActionState.Hop:
                    NPC.frame.Y = frameHeight;
                    break;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Cavern.Chance * 0.1f;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement("An skeletal spider ready to spook you at any moment!")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (AIState == (float) ActionState.Idle)
            {
                moveTo = NPC.FindGround(10);
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = (float) ActionState.Wander;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f,
                        NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}