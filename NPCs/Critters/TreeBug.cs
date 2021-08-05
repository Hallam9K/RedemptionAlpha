using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Redemption.Items.Critters;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;

namespace Redemption.NPCs.Critters
{
    public class TreeBug : ModNPC
    {
        private enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Hop,
            Eat
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
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 12;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.catchItem = (short)ModContent.ItemType<TreeBugItem>();
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
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(10);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    HopCheck();
                    if (RedeHelper.ClosestNPC(ref target, 100, NPC.Center) && target.damage > 0)
                    {
                        moveTo = NPC.FindGround(10);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    Point tileBelow = NPC.Bottom.ToTileCoordinates();
                    Tile tile = Main.tile[tileBelow.X, tileBelow.Y];
                    if (Main.rand.NextBool(500) && tile != null && tile.IsActiveUnactuated && Main.tileSolid[tile.type] && TileTags.WoodLeaf.Has(tile.type))
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(180, 300);
                        AIState = (float)ActionState.Eat;
                    }
                    break;
                case (float)ActionState.Wander:
                    HopCheck();
                    if (RedeHelper.ClosestNPC(ref target, 100, NPC.Center) && target.damage > 0)
                    {
                        RedeHelper.HorizontallyMove(NPC, new Vector2(target.Center.X < NPC.Center.X ? NPC.Center.X + 50 : NPC.Center.X - 50, NPC.Center.Y), 0.5f, 2, 4, 2, false);
                        return;
                    }
                    AITimer++;
                    if (AITimer >= TimerRand || (NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16))
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Idle;
                    }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1, 4, 2, false);
                    break;
                case (float)ActionState.Hop:
                    if (BaseAI.HitTileOnSide(NPC, 3, true))
                    {
                        moveTo = NPC.FindGround(10);
                        hopCooldown = 180;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }
                    break;
                case (float)ActionState.Eat:
                    NPC.velocity.X *= 0.5f;
                    tileBelow = NPC.Bottom.ToTileCoordinates();
                    tile = Main.tile[tileBelow.X, tileBelow.Y];
                    AITimer++;
                    if (AITimer % 30 == 0 && NPC.life < NPC.lifeMax)
                    {
                        NPC.life++;
                        NPC.HealEffect(1);
                    }
                    if (AITimer >= TimerRand || tile == null || !tile.IsActiveUnactuated || !Main.tileSolid[tile.type] || !TileTags.WoodLeaf.Has(tile.type))
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Idle;
                    }
                    break;
            }
        }
        public void HopCheck()
        {
            if (hopCooldown == 0 && BaseAI.HitTileOnSide(NPC, 3, true) && RedeHelper.ClosestNPC(ref target, 32, NPC.Center) && target.damage > 0)
            {
                NPC.velocity.X *= target.Center.X < NPC.Center.X ? 1.4f : -1.4f;
                NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                AIState = (float)ActionState.Hop;
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
                case (float)ActionState.Eat:
                    NPC.frameCounter++;
                    if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    else if (NPC.frameCounter < 40)
                    {
                        NPC.frame.Y = 5 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreeBugShell>(), 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDayGrassCritter.Chance * (Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass ? 1.7f : 0f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("A beetle commonly found inhabiting trees. It feeds on leaves and uses its leaf-like shell for camouflage from predators. Its shell makes a good source of green dye.")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (AIState == (float)ActionState.Idle || AIState == (float)ActionState.Eat)
            {
                moveTo = NPC.FindGround(10);
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = (float)ActionState.Wander;
            }
            if (NPC.life <= 0)
            {
                int gore1 = ModContent.Find<ModGore>("Redemption/TreeBugGore1").Type;
                int gore2 = ModContent.Find<ModGore>("Redemption/TreeBugGore2").Type;
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