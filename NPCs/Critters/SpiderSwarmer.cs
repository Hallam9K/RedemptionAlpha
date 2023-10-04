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
using Terraria.DataStructures;
using Terraria.Localization;

namespace Redemption.NPCs.Critters
{
    public class SpiderSwarmer : ModNPC
    {
        public enum ActionState
        {
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
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Shimmerfly;
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState is ActionState.Aggressive && !target.dontHurtCritters;
        public override bool CanHitNPC(NPC target) => false;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => target.noKnockback = true;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<SpiderSwarmedDebuff>(), 120);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            target.AddBuff(ModContent.BuffType<SpiderSwarmedDebuff>(), 120);
        }

        public NPC npcTarget;
        public Vector2 moveTo;
        private int runCooldown;
        public int hopCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 180);
        }
        public override void AI()
        {
            NPC.TargetClosest();
            NPC.LookByVelocity();
            RedeNPC globalNPC = NPC.Redemption();

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
                        NPCHelper.HorizontallyMove(NPC, new Vector2(npcTarget.Center.X < NPC.Center.X ? NPC.Center.X + 50 : NPC.Center.X - 50, NPC.Center.Y), 0.5f, 3f, 4, 4, false);
                        return;
                    }

                    AITimer++;

                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                    }

                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.5f, 3f, 6, 6, false);
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
                    Player player = Main.player[NPC.target];
                    if (NPC.ThreatenedCheck(ref runCooldown, 180) || player.dontHurtCritters)
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

                    NPC.DamageHostileAttackers();

                    if (!NPC.Sight(globalNPC.attacker, 150, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.5f, 3f, 6, 6, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public int GetNearestNPC()
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
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
                if (NPC.Sight(player, 100, false, true) && !player.dontHurtCritters)
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
                        if (NPC.frame.Y > 2 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = frameHeight;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Spiderswarmer"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f,
                    NPC.velocity.Y * 0.5f);
        }
    }
}