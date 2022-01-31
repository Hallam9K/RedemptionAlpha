using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Plants;
using Redemption.Projectiles.Hostile;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Redemption.BaseExtension;

namespace Redemption.NPCs.PreHM
{
    public class LivingBloom : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Threatened,
            RootAttack
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
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    BuffID.Bleeding,
                    BuffID.Poisoned,
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 52;
            NPC.defense = 3;
            NPC.damage = 13;
            NPC.lifeMax = 45;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 20;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<LivingBloomBanner>();
        }

        public NPC npcTarget;
        public Vector2 moveTo;
        public int runCooldown;

        public override void AI()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            NPC.LookByVelocity();
            RegenCheck();

            if (AITimer % 30 == 0)
            {
                Point grass = new Vector2(NPC.Center.X, NPC.Bottom.Y - 4).ToTileCoordinates();
                GrassCheck(grass.X, grass.Y);
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

                    if (NPC.ClosestNPCToNPC(ref npcTarget, 160, NPC.Center) && npcTarget.lifeMax > 5 && npcTarget.damage > 0 && !npcTarget.Redemption().invisible)
                    {
                        globalNPC.attacker = npcTarget;
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Threatened;
                    }
                    break;

                case ActionState.Wander:
                    if (NPC.ClosestNPCToNPC(ref npcTarget, 160, NPC.Center) && npcTarget.lifeMax > 5 && npcTarget.damage > 0 && !npcTarget.Redemption().invisible)
                    {
                        globalNPC.attacker = npcTarget;
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Threatened;
                    }

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                    }

                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1, 6, 4, false);
                    break;

                case ActionState.Threatened:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, -1, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    RedeHelper.HorizontallyMove(NPC, new Vector2(globalNPC.attacker.Center.X < NPC.Center.X ? NPC.Center.X + 50 : NPC.Center.X - 50, NPC.Center.Y),
                        0.5f, 2, 6, 4, false);

                    if (Main.rand.NextBool(200) && NPC.velocity.Y == 0)
                    {
                        AITimer = 0;
                        AIState = ActionState.RootAttack;
                    }
                    break;
                case ActionState.RootAttack:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 800 * 800 || runCooldown > 180)
                        AIState = ActionState.Wander;

                    for (int i = 0; i < 2; i++)
                    {
                        int dustIndex = Dust.NewDust(NPC.BottomLeft, NPC.width, 1, DustID.DryadsWard, 0f, 0f, 100, default, 1);
                        Main.dust[dustIndex].velocity.Y -= 4f;
                        Main.dust[dustIndex].velocity.X *= 0f;
                        Main.dust[dustIndex].noGravity = true;
                    }

                    NPC.velocity.X *= 0.5f;

                    AITimer++;
                    if (AITimer == 5)
                    {
                        int tilePosY = BaseWorldGen.GetFirstTileFloor((int)(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30)) / 16, (int)(globalNPC.attacker.Center.Y / 16) - 2);
                        NPC.Shoot(new Vector2(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30), (tilePosY * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];
                            if (!target.active || target.whoAmI == NPC.whoAmI || target.whoAmI == globalNPC.attacker.whoAmI || target.Redemption().invisible)
                                continue;

                            if (target.lifeMax < 5 || target.damage == 0 || NPC.DistanceSQ(target.Center) > 400 * 400 || target.type == NPC.type)
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Center.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        }
                        for (int p = 0; p < Main.maxPlayers; p++)
                        {
                            Player target = Main.player[p];
                            if (globalNPC.attacker is NPC)
                                continue;

                            if (!target.active || NPC.DistanceSQ(target.Center) > 400 * 400)
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Center.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        }
                    }
                    else if (AITimer >= 80)
                    {
                        AIState = ActionState.Threatened;
                    }
                    break;
            }
        }
        public static bool GrassCheck(int X, int Y) // Directly from Flower Boots code, cleaned a bit
        {
            Tile tile = Main.tile[X, Y];
            if (tile == null)
            {
                return false;
            }
            if (!tile.IsActive && tile.LiquidAmount == 0 && Main.tile[X, Y + 1] != null && WorldGen.SolidTile(X, Y + 1))
            {
                tile.frameY = 0;
                tile.Slope = 0;
                tile.IsHalfBlock = false;
                if (Main.tile[X, Y + 1].type == TileID.Grass || Main.tile[X, Y + 1].type == TileID.GolfGrass)
                {
                    int num = Main.rand.NextFromList(6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24, 27, 30, 33, 36, 39, 42);
                    switch (num)
                    {
                        case 21:
                        case 24:
                        case 27:
                        case 30:
                        case 33:
                        case 36:
                        case 39:
                        case 42:
                            num += Main.rand.Next(3);
                            break;
                    }
                    tile.IsActive = true;
                    tile.type = TileID.Plants;
                    tile.frameX = (short)(num * 18);
                    tile.Color = Main.tile[X, Y + 1].Color;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, X, Y);
                    }
                    return true;
                }
                if (Main.tile[X, Y + 1].type == TileID.HallowedGrass || Main.tile[X, Y + 1].type == TileID.GolfGrassHallowed)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        tile.IsActive = true;
                        tile.type = TileID.HallowedPlants;
                        tile.frameX = (short)(18 * Main.rand.Next(4, 7));
                        tile.Color = Main.tile[X, Y + 1].Color;
                        while (tile.frameX == 90)
                        {
                            tile.frameX = (short)(18 * Main.rand.Next(4, 7));
                        }
                    }
                    else
                    {
                        tile.IsActive = true;
                        tile.type = TileID.HallowedPlants2;
                        tile.frameX = (short)(18 * Main.rand.Next(2, 8));
                        tile.Color = Main.tile[X, Y + 1].Color;
                        while (tile.frameX == 90)
                        {
                            tile.frameX = (short)(18 * Main.rand.Next(2, 8));
                        }
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, X, Y);
                    }
                    return true;
                }
                if (Main.tile[X, Y + 1].type == 60)
                {
                    tile.IsActive = true;
                    tile.type = 74;
                    tile.frameX = (short)(18 * Main.rand.Next(9, 17));
                    tile.Color = Main.tile[X, Y + 1].Color;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, X, Y);
                    }
                    return true;
                }
            }
            return false;
        }

        int regenTimer;
        public void RegenCheck()
        {
            int regenCooldown = NPC.wet && !NPC.lavaWet ? 30 : 40;
            if ((NPC.wet && !NPC.lavaWet) || (Main.raining && NPC.position.Y < Main.worldSurface && Framing.GetTileSafely(NPC.Center).wall == WallID.None))
            {
                regenTimer++;
                if (regenTimer % regenCooldown == 0 && NPC.life < NPC.lifeMax)
                {
                    NPC.life += 1;
                    NPC.HealEffect(1);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            switch (AIState)
            {
                case ActionState.RootAttack:
                    NPC.frame.Y = 7 * frameHeight;
                    return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 4 * frameHeight;
                else
                {
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 5 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                if (NPC.velocity.Y < 0)
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    if (++NPC.frameCounter >= 3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 9 * frameHeight)
                            NPC.frame.Y = 9 * frameHeight;
                    }
                }
                else
                    NPC.frame.Y = 10 * frameHeight;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.OneFromOptions(2,
                new int[] { ItemID.Daybloom, ItemID.Blinkroot, ItemID.Moonglow, ItemID.Waterleaf, ModContent.ItemType<Nightshade>() }));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnglonicMysticBlossom>(), 100));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldDay.Chance;
            float multiplier = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass ? (Main.raining ? 0.4f : 0.2f) : 0f;

            return baseChance * multiplier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(
                    "A common creature native to Anglon, living in lush forests. They are made out of plant fibre and roots.")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = ActionState.Threatened;
            }

            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                int goreType1 = ModContent.Find<ModGore>("Redemption/LivingBloomGore1").Type;
                int goreType2 = ModContent.Find<ModGore>("Redemption/LivingBloomGore2").Type;

                Gore.NewGore(NPC.position, NPC.velocity, goreType1);
                Gore.NewGore(NPC.position, NPC.velocity, goreType2);

                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}