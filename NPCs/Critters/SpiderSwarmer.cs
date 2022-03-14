using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Critters;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Critters
{
    public class SpiderSwarmer : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Hop,
            Aggressive
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
            Main.npcFrameCount[Type] = 3;
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
            NPC.lifeMax = 1;
            NPC.damage = 2;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = 0;
            NPC.npcSlots = 0;
            NPC.knockBackResist = 0.5f;
            NPC.scale = 0.7f;
            NPC.aiStyle = -1;
            NPC.catchItem = (short)ModContent.ItemType<SpiderSwarmerItem>();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState is ActionState.Aggressive;
        public override bool? CanHitNPC(NPC target) => false;
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => target.noKnockback = true;
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SpiderSwarmedDebuff>(), 120);
        }

        public NPC npcTarget;
        public Vector2 moveTo;
        private int runCooldown;
        public int hopCooldown;

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.LookByVelocity();
            RedeNPC globalNPC = NPC.Redemption();

            if (hopCooldown > 0)
                hopCooldown--;

            switch (AIState)
            {
                case ActionState.Begin:
                    TimerRand = Main.rand.Next(80, 180);
                    AIState = ActionState.Idle;
                    break;

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

                    SightCheck();
                    HopCheck();

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    break;

                case ActionState.Wander:
                    HopCheck();
                    SightCheck();

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0)
                    {
                        RedeHelper.HorizontallyMove(NPC,
                            new Vector2(npcTarget.Center.X < NPC.Center.X ? NPC.Center.X + 50 : NPC.Center.X - 50,
                                NPC.Center.Y), 0.5f, 3f, 4, 4, false);
                        return;
                    }

                    AITimer++;

                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                    }

                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.5f, 3f, 6, 6, false);
                    break;

                case ActionState.Hop:
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        moveTo = NPC.FindGround(15);
                        hopCooldown = 60;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }
                    break;

                case ActionState.Aggressive:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (hopCooldown == 0 && BaseAI.HitTileOnSide(NPC, 3) && NPC.Sight(globalNPC.attacker, 60, false, true))
                    {
                        NPC.velocity.X *= 2f;
                        NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f); 
                        hopCooldown = 80;
                    }

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC target = Main.npc[i];
                        if (!target.active || target.whoAmI == NPC.whoAmI || target != NPC.Redemption().attacker)
                            continue;

                        if (target.immune[NPC.whoAmI] > 0 || !NPC.Hitbox.Intersects(target.Hitbox))
                            continue;

                        target.immune[NPC.whoAmI] = 30;
                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                        BaseAI.DamageNPC(target, NPC.damage, 0, hitDirection, NPC);
                        target.AddBuff(ModContent.BuffType<SpiderSwarmedDebuff>(), 120);
                    }

                    if (!NPC.Sight(globalNPC.attacker, 150, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    bool jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.5f, 3f, 6, 6, NPC.Center.Y > globalNPC.attacker.Center.Y);
                    break;
            }
        }

        public int GetNearestNPC()
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan)
                    continue;

                if (target.lifeMax <= 5 || target.type == NPC.type)
                    continue;

                if (nearestNPCDist != -1 && !(target.Distance(NPC.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(NPC.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC();
            if (CountCheck() > 5)
            {
                if (NPC.Sight(player, 100, false, true))
                {
                    globalNPC.attacker = player;
                    AITimer = 0;
                    AIState = ActionState.Aggressive;
                }
                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 100, false, true))
                {
                    globalNPC.attacker = Main.npc[gotNPC];
                    AITimer = 0;
                    AIState = ActionState.Aggressive;
                }
            }
            return;
        }

        public void HopCheck()
        {
            if (hopCooldown != 0 || !BaseAI.HitTileOnSide(NPC, 3) ||
                !RedeHelper.ClosestNPC(ref npcTarget, 60, NPC.Center) || npcTarget.damage <= 0)
                return;

            NPC.velocity.X *= npcTarget.Center.X < NPC.Center.X ? 3f : -3f;
            NPC.velocity.Y = Main.rand.NextFloat(-4f, -9f);
            AIState = ActionState.Hop;
        }

        public int CountCheck()
        {
            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC spider = Main.npc[i];
                if (!spider.active || spider.whoAmI == NPC.whoAmI || spider.type != NPC.type)
                    continue;

                if (NPC.DistanceSQ(spider.Center) > 500 * 500)
                    continue;

                count++;
            }
            return count;
        }

        public override void FindFrame(int frameHeight)
        {
            switch (AIState)
            {
                case (float)ActionState.Begin:
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 2 * frameHeight)
                            NPC.frame.Y = 0;
                    }

                    break;

                case ActionState.Idle:
                    NPC.frame.Y = 0;
                    break;

                case ActionState.Wander:
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 2 * frameHeight)
                            NPC.frame.Y = 0;
                    }

                    break;

                case ActionState.Hop:
                    NPC.frame.Y = frameHeight;
                    break;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement("Lives in dark caverns or basements. These on their own are harmless, but if you see a swarm of them, prepare for a thousand bite marks.")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f,
                    NPC.velocity.Y * 0.5f);
        }
    }
}