using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Soulless
{
    public class Shadebug : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Alert,
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
            DisplayName.SetDefault("Shadebug");
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 14;
            NPC.damage = 60;
            NPC.defense = 30;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath32;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }
        public Vector2 moveTo;
        private int runCooldown;
        private int hopCooldown;
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
                    break;
                case ActionState.Wander:
                    SightCheck();

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                    }

                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1.4f, 6, 6, false);
                    break;

                case ActionState.Alert:
                    HopCheck();
                    if (Main.rand.NextBool(50))
                        SightCheck();

                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 ||
                        runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 200, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    RedeHelper.HorizontallyMove(NPC, new Vector2(globalNPC.attacker.Center.X < NPC.Center.X ? NPC.Center.X + 100
                        : NPC.Center.X - 100, NPC.Center.Y), 0.2f, 3f, 8, 8, NPC.Center.Y > globalNPC.attacker.Center.Y);
                    break;
                case ActionState.Hop:
                    NPC.DamageHostileAttackers(0, 1);
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        moveTo = NPC.FindGround(15);
                        hopCooldown = 60;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    break;
            }
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
                    if (NPC.frame.Y < 1 * frameHeight)
                        NPC.frame.Y = 1 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.75f;
                    if (NPC.frameCounter is >= 5 or <= -5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 8 * frameHeight)
                            NPC.frame.Y = 1 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.1f;
                NPC.frame.Y = 1 * frameHeight;
            }
        }
        public void HopCheck()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            if (hopCooldown == 0 && player.active && !player.dead && NPC.Sight(player, 60, false, true) &&
                BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.velocity.X += player.Center.X < NPC.Center.X ? -7f : 7f;
                NPC.velocity.Y = Main.rand.NextFloat(-3f, -6f);
                AIState = ActionState.Hop;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI)
                    continue;

                if (hopCooldown == 0 && Main.rand.NextBool(60) && NPC.Sight(target, 60, false, true) &&
                    BaseAI.HitTileOnSide(NPC, 3))
                {
                    NPC.velocity.X += target.Center.X < NPC.Center.X ? -5f : 5f;
                    NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                    AIState = ActionState.Hop;
                }
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

                if (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
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
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC();
            if (NPC.Sight(player, 140, true, true))
            {
                globalNPC.attacker = player;
                AITimer = 0;
                if (AIState != ActionState.Alert)
                    AIState = ActionState.Alert;
            }
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 140, true, true))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                AITimer = 0;
                if (AIState != ActionState.Alert)
                    AIState = ActionState.Alert;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("A pathetic mass of soulless too weak to take a humanoid form. These \"bugs\" are at the bottom of the food chain in this dark competition of growth, either created this way due to their meager souls in past life or half-devoured by their competitors.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (AIState is not ActionState.Alert)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), Scale: 2);
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Hop;
        public override bool? CanHitNPC(NPC target) => target.whoAmI != NPC.whoAmI && AIState == ActionState.Hop ? null : false;
        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 10);
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 10);
    }
}
