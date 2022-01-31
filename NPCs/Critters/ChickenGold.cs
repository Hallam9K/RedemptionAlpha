using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Critters;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Critters
{
    public class ChickenGold : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Alert,
            Peck,
            Sit
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
            DisplayName.SetDefault("Gold Chicken");
            Main.npcFrameCount[Type] = 21;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Add(Type);
            NPCID.Sets.GoldCrittersCollection.Add(Type);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 22;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.catchItem = (short)ModContent.ItemType<ChickenGoldItem>();
            Banner = Item.NPCtoBanner(ModContent.NPCType<Chicken>());
            BannerItem = Item.BannerToItem(Banner);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>()));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldDay.Chance;
            float multiplier = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass ? 0.001f : 0f;

            return baseChance * multiplier;
        }

        public Vector2 moveTo;
        private int runCooldown;
        private int waterCooldown;

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.LookByVelocity();
            RedeNPC globalNPC = NPC.Redemption();

            if (NPC.wet && !NPC.lavaWet && waterCooldown < 180)
            {
                NPC.velocity.Y -= 0.3f;
                waterCooldown++;
            }

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

                    if (Main.rand.NextBool(200) && (NPC.collideY || NPC.velocity.Y == 0))
                        AIState = ActionState.Peck;

                    Point tileBelow = NPC.Bottom.ToTileCoordinates();
                    Tile tile = Main.tile[tileBelow.X, tileBelow.Y];

                    if ((NPC.collideY || NPC.velocity.Y == 0) && Main.rand.NextBool(100) && tile.type == TileID.HayBlock && 
                        tile is { IsActiveUnactuated: true } && Main.tileSolid[tile.type])
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(300, 1200);
                        AIState = ActionState.Sit;
                    }

                    SightCheck();
                    break;

                case ActionState.Peck:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    SightCheck();
                    break;

                case ActionState.Sit:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    AITimer++;

                    if (AITimer == TimerRand - 60)
                    {
                        SoundEngine.PlaySound(SoundID.Item16, NPC.position);
                        Item.NewItem(NPC.getRect(), ModContent.ItemType<GoldChickenEgg>());
                    }
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 180);
                        AIState = ActionState.Wander;
                    }

                    Point tileBelow2 = new Vector2(NPC.Center.X, NPC.Bottom.Y).ToTileCoordinates();
                    Tile tile2 = Main.tile[tileBelow2.X, tileBelow2.Y];
                    if (tile2.type != TileID.HayBlock || tile2 is not { IsActiveUnactuated: true } || !Main.tileSolid[tile2.type])
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 180);
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

                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1, 6, 6, false);
                    break;

                case ActionState.Alert:
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

                    if (Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust5>());

                    RedeHelper.HorizontallyMove(NPC, new Vector2(globalNPC.attacker.Center.X < NPC.Center.X ? NPC.Center.X + 100
                        : NPC.Center.X - 100, NPC.Center.Y), 0.2f, 2.5f, 8, 8, NPC.Center.Y > globalNPC.attacker.Center.Y);
                    break;
            }

            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.GoldCritter, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;

        }

        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Peck)
            {
                NPC.rotation = 0;

                if (NPC.frame.Y < 14 * frameHeight)
                    NPC.frame.Y = 14 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 20 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                        AIState = ActionState.Idle;
                    }
                }
                return;
            }
            if (AIState is ActionState.Sit)
            {
                NPC.rotation = 0;

                if (NPC.frame.Y < 9 * frameHeight)
                    NPC.frame.Y = 9 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 13 * frameHeight)
                    {
                        NPC.frame.Y = 13 * frameHeight;
                    }
                }
                return;
            }

            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    NPC.frame.Y = 0;
                }
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
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 2 * frameHeight;
            }
        }

        public void SightCheck()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = RedeHelper.GetNearestNPC(NPC.Center);
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
            bestiaryEntry.UIInfoProvider = new GoldCritterUICollectionInfoProvider(new int[] { ModContent.NPCType<Chicken>() }, 
                ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPC.type]);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(
                    "A chicken with golden feathers. Best not to kill it, for it can lay golden eggs!")
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
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust5>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust5>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
}