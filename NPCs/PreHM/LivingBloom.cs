using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Plants;
using Redemption.Projectiles.Hostile;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class LivingBloom : ModNPC
    {
        private enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Threatened,
            RootAttack
        }

        public ref float AIState => ref NPC.ai[0];

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

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
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            NPC.TargetClosest();
            NPC.LookByVelocity();
            RegenCheck();

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

                    if (NPC.ClosestNPCToNPC(ref npcTarget, 160, NPC.Center) && npcTarget.lifeMax > 5 && npcTarget.damage > 0)
                    {
                        globalNPC.attacker = npcTarget;
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Threatened;
                    }
                    break;

                case (float)ActionState.Wander:
                    if (NPC.ClosestNPCToNPC(ref npcTarget, 160, NPC.Center) && npcTarget.lifeMax > 5 && npcTarget.damage > 0)
                    {
                        globalNPC.attacker = npcTarget;
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Threatened;
                    }

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Idle;
                    }

                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1, 6, 4, false);
                    break;

                case (float)ActionState.Threatened:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 800 * 800 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = (float)ActionState.Wander;
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
                        AIState = (float)ActionState.RootAttack;
                    }
                    break;
                case (float)ActionState.RootAttack:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 800 * 800 || runCooldown > 180)
                        AIState = (float)ActionState.Wander;

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
                        foreach (NPC target in Main.npc)
                        {
                            if (!target.active || target.whoAmI == NPC.whoAmI || target.whoAmI == globalNPC.attacker.whoAmI)
                                continue;

                            if (target.lifeMax < 5 || target.damage == 0 || NPC.DistanceSQ(target.Center) > 600 * 600 || target.type == NPC.type)
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Center.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        }
                        foreach (Player target in Main.player)
                        {
                            if (!target.active || target.whoAmI == globalNPC.attacker.whoAmI || NPC.DistanceSQ(target.Center) > 600 * 600)
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Center.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        }
                    }
                    else if (AITimer >= 80)
                    {
                        AIState = (float)ActionState.Threatened;
                    }
                    break;
            }
        }
        public bool GrassCheck()
        {
            Point grass = new Vector2(NPC.Center.X, NPC.Bottom.Y).ToTileCoordinates();
            Tile tile = Main.tile[grass.X, grass.Y];
            if (tile is not { IsActiveUnactuated: true } || !Main.tileSolid[tile.type] || !TileID.Sets.Conversion.Grass[tile.type])
            {
                return true;
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
                case (float)ActionState.RootAttack:
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
            float multiplier = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass ? (Main.raining ? 1.8f : 1f) : 0f;

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
            if (AIState is (float)ActionState.Idle or (float)ActionState.Wander)
            {
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = (float)ActionState.Threatened;
            }

            if (NPC.life <= 0)
            {
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