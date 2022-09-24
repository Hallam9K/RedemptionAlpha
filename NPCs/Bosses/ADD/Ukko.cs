using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Items.Usable;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Buffs.Debuffs;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Redemption.BaseExtension;
using Terraria.GameContent.UI;

namespace Redemption.NPCs.Bosses.ADD
{
    [AutoloadBossHead]
    public class Ukko : ModNPC
    {
        private Player player;

        private int RunOnce = 0;
        private int StopRain = 0;

        public enum ActionState
        {
            Start,
            ResetVars,
            Idle,
            Attacks,
            AkkaSummon
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AttackID => ref NPC.ai[1];
        public ref float AITimer => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ukko");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    BuffID.Confused,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 40),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 145000;
            NPC.damage = 120;
            NPC.defense = 35;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 8, 0, 0);
            NPC.aiStyle = -1;
            NPC.width = 88;
            NPC.height = 108;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest2");
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                damage = (int)(damage * .75f);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Rain,

                new FlavorTextBestiaryInfoElement("Once a blacksmith in ancient Tethuram who had an immense talent in magic, growing to be seen as a god by many who witnessed his strengths. In his old age, growing weary of his mortality, Ukko infused his soul into a bundle of rocks, allowing him to eventually reform when the times comes. The locals worshipped this golem, until the knowledge of Ukko's soul inside slowly faded away.")
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            /*npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ThornBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThornTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ThornRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<BouquetOfThorns>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ThornMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<CursedGrassBlade>(), ModContent.ItemType<RootTendril>(), ModContent.ItemType<CursedThornBow>(), ModContent.ItemType<BlightedBoline>()));

            npcLoot.Add(notExpertRule);*/
        }
        public override void OnKill()
        {
            if (!RedeBossDowned.downedADD && !NPC.AnyNPCs(ModContent.NPCType<Akka>()))
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gray, "+0", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("It is unknown how these forgotten deities ruled, perhaps defeating them was for the best, or worst.", 300, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedADD, -1);
        }

        public Vector2 MoveVector2;
        public Vector2 MoveVector3;
        public int wandFrame;
        public int frameCounters;
        public int mendingCooldown;
        public int stoneskinCooldown;
        public int chariotCooldown;
        public int burstCooldown;
        public int teamCooldown = 10;
        public int chariotFrame;
        public int dashCounter;
        public int dischargeFrame;
        public bool akkaArrive;

        public override void AI()
        {
            Rain();

            Target();

            DespawnHandler();

            Player player = Main.player[NPC.target];
            if (AIState is not ActionState.AkkaSummon)
            {
                if (AttackID == 7)
                {
                    NPC.rotation = NPC.velocity.ToRotation();
                    NPC.LookByVelocity();
                }
                else
                {
                    NPC.rotation = 0f;
                    NPC.LookAtEntity(player);
                }
            }
            Vector2 ThunderwavePos = new(player.Center.X > NPC.Center.X ? player.Center.X - 700 : player.Center.X + 700, player.Center.Y);
            int akkaID = NPC.FindFirstNPC(ModContent.NPCType<Akka>());
            bool akkaActive = false;
            if (akkaID > -1 && Main.npc[akkaID].active && Main.npc[akkaID].type == ModContent.NPCType<Akka>())
                akkaActive = true;

            if (!akkaArrive && NPC.life <= (int)(NPC.lifeMax * 0.75f) && AIState is not ActionState.AkkaSummon)
            {
                NPC.rotation = 0;
                NPC.ai[3] = 0;
                AIState = ActionState.AkkaSummon;
                AttackID = 0;
                AITimer = 0;
                NPC.netUpdate = true;
            }
            switch (AIState)
            {
                case ActionState.Start:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Ukko", 60, 90, 0.8f, 0, Color.LightGoldenrodYellow, "Ancient God of Weather");

                    NPC.Shoot(new Vector2(NPC.Center.X - (118 * 16) - 10, NPC.Center.Y + 8), ModContent.ProjectileType<UkkoBarrier>(), 0, Vector2.Zero, false, SoundID.Item1, 0, 1);
                    NPC.Shoot(new Vector2(NPC.Center.X + (118 * 16) + 26, NPC.Center.Y + 8), ModContent.ProjectileType<UkkoBarrier>(), 0, Vector2.Zero, false, SoundID.Item1, 0, -1);
                    NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y - (118 * 16) - 10), ModContent.ProjectileType<UkkoBarrierH>(), 0, Vector2.Zero, false, SoundID.Item1, 0, 1);
                    NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y + (118 * 16) + 26), ModContent.ProjectileType<UkkoBarrierH>(), 0, Vector2.Zero, false, SoundID.Item1, 0, -1);

                    ArenaWorld.arenaBoss = "ADD";
                    ArenaWorld.arenaTopLeft = new Vector2(NPC.Center.X - (120 * 16) + 8, NPC.Center.Y - (120 * 16) + 8);
                    ArenaWorld.arenaSize = new Vector2(240 * 16, 240 * 16);
                    ArenaWorld.arenaMiddle = NPC.Center;
                    ArenaWorld.arenaActive = true;

                    NPC.ai[0]++;
                    NPC.netUpdate = true;
                    break;
                case ActionState.ResetVars:
                    if (mendingCooldown > 0)
                        mendingCooldown--;
                    if (stoneskinCooldown > 0 && !NPC.HasBuff(ModContent.BuffType<StoneskinBuff>()))
                        stoneskinCooldown--;
                    if (chariotCooldown > 0)
                        chariotCooldown--;
                    if (burstCooldown > 0)
                        burstCooldown--;
                    if (teamCooldown > 0)
                        teamCooldown--;
                    MoveVector2 = Pos();
                    MoveVector3 = ChargePos();
                    NPC.ai[0]++;
                    break;
                case ActionState.Idle:
                    if (NPC.DistanceSQ(MoveVector2) < 10 * 10)
                    {
                        NPC.velocity *= 0;
                        NPC.ai[0]++;
                        AttackID = Main.rand.Next(15);
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        NPC.MoveToVector2(MoveVector2, 40);
                    }
                    break;
                case ActionState.Attacks:
                    switch (AttackID)
                    {
                        #region Thunderclap
                        case 0:
                            AITimer++;
                            if (AITimer == 8)
                            {
                                NPC.ai[3] = 1;
                                NPC.Shoot(player.Center, ModContent.ProjectileType<UkkoStrike>(), (int)(NPC.damage * 0.92f), Vector2.Zero, false, SoundID.Item1);
                            }
                            if (AITimer >= 60)
                            {
                                AIState = ActionState.ResetVars;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Gust
                        case 1:
                            AITimer++;
                            if (AITimer == 2)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.WindLong, NPC.position);
                            }
                            if (AITimer % 2 == 0 && AITimer > 8 && AITimer < 48)
                            {
                                NPC.ai[3] = 1;
                                for (int i = 0; i < 2; i++)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoGust>(), 0, RedeHelper.Spread(8), false, SoundID.Item1);
                                }
                            }
                            if (AITimer >= 50)
                            {
                                AIState = ActionState.ResetVars;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Thunderwave
                        case 2:
                            if (AITimer == 0)
                            {
                                if (NPC.DistanceSQ(ThunderwavePos) < 100 * 100)
                                {
                                    NPC.velocity = NPC.DirectionTo(player.Center).RotatedBy(Math.PI / 2) * 40;
                                    AITimer = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.MoveToVector2(ThunderwavePos, 30);
                            }
                            else
                            {
                                AITimer++;
                                if (!akkaActive ? AITimer < 30 : AITimer < 20)
                                    NPC.velocity -= NPC.velocity.RotatedBy(Math.PI / 2) * NPC.velocity.Length() / NPC.Distance(player.Center);
                                else
                                    NPC.velocity *= 0f;

                                if (!akkaActive ? AITimer % 3 == 0 && AITimer < 30 : AITimer % 4 == 0 && AITimer < 30)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoThunderwave>(), (int)(NPC.damage * 0.8f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), true, CustomSounds.Zap2);
                                }
                                if (!akkaActive ? AITimer >= 35 : AITimer >= 25)
                                {
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Call Lightning
                        case 3:
                            if (NPC.life < (int)(NPC.lifeMax * 0.6f))
                            {
                                AITimer++;
                                if (AITimer == 2)
                                {
                                    NPC.ai[3] = 1;
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                }
                                if (AITimer > 40 && AITimer < 160)
                                {
                                    if (NPC.DistanceSQ(MoveVector2) < 10 * 10)
                                    {
                                        MoveVector2 = Pos();
                                        NPC.velocity *= 0;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.MoveToVector2(MoveVector2, 30);
                                }
                                if ((!akkaActive ? AITimer % 15 == 0 : AITimer % 20 == 0) && AITimer > 40 && AITimer < 160)
                                {
                                    NPC.Shoot(player.Center + new Vector2(Main.rand.Next(-300, 301)), ModContent.ProjectileType<UkkoStrike>(), (int)(NPC.damage * 0.92f), Vector2.Zero, false, SoundID.Item1);
                                }
                                if (AITimer >= 180)
                                {
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Mending
                        case 4:
                            if (NPC.life < (int)(NPC.lifeMax * 0.5f) && mendingCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer == 2)
                                {
                                    NPC.ai[3] = 1;
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Quake with { Pitch = 0.2f }, NPC.position);
                                }
                                if (AITimer % 20 == 0)
                                    SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                if (AITimer < 60)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 3f);
                                        dust.velocity = -NPC.DirectionTo(dust.position);
                                    }
                                    NPC.life += 50;
                                    NPC.HealEffect(50);
                                }
                                if (AITimer >= 120)
                                {
                                    mendingCooldown = 20;
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Stoneskin
                        case 5:
                            if (NPC.life < (int)(NPC.lifeMax * 0.8f) && stoneskinCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer == 2)
                                {
                                    NPC.ai[3] = 1;
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Quake with { Pitch = 0.1f }, NPC.position);
                                }
                                if (AITimer < 60)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 3f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);

                                    NPC.AddBuff(ModContent.BuffType<StoneskinBuff>(), 3600);
                                }
                                if (AITimer >= 90)
                                {
                                    stoneskinCooldown = 10;
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Dancing Lights
                        case 6:
                            if (player.ZoneHallow)
                            {
                                AITimer++;
                                if (AITimer == 1)
                                {
                                    NPC.ai[3] = 1;
                                }
                                if (AITimer == 30)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.PinkTorch, 0f, 0f, 100, default, 3f);
                                        Main.dust[dustIndex].velocity *= 4.2f;
                                    }
                                    for (int i = 0; i < 10; i++)
                                    {
                                        int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.GoldFlame, 0f, 0f, 100, default, 3f);
                                        Main.dust[dustIndex].velocity *= 4.2f;
                                    }
                                }
                                if (AITimer > 80 && AITimer < 160)
                                {
                                    if (Main.rand.NextBool(3))
                                    {
                                        NPC.Shoot(player.Center + RedeHelper.PolarVector(Main.rand.Next(300, 601), Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<UkkoDancingLights>(), 0, RedeHelper.Spread(1), false, SoundID.Item1);
                                    }
                                }
                                if (AITimer >= 200)
                                {
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Lightning Chariot
                        case 7:
                            if (chariotCooldown == 0)
                            {
                                NPC.ai[3] = 2;
                                if (NPC.velocity.X < 0)
                                {
                                    NPC.rotation += (float)Math.PI;
                                }
                                if (AITimer == 0)
                                {
                                    if (NPC.DistanceSQ(MoveVector3) < 200 * 200)
                                    {
                                        if (!Main.dedServ)
                                            SoundEngine.PlaySound(CustomSounds.Jyrina, NPC.position);

                                        NPC.velocity = NPC.DirectionTo(player.Center) * 50;
                                        AITimer = 1;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.MoveToVector2(MoveVector3, 30);
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer % 8 == 0 && AITimer < 50)
                                    {
                                        Vector2 ai = RedeHelper.PolarVector(12, -NPC.velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f));
                                        float ai2 = Main.rand.Next(100);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(12, -NPC.velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<UkkoLightning>(), (int)(NPC.damage * 0.9f) / 4, 0, Main.myPlayer, ai.ToRotation(), ai2);
                                    }
                                    if (AITimer >= 50 && dashCounter < 2)
                                    {
                                        dashCounter++;
                                        MoveVector3 = ChargePos();
                                        AITimer = 0;
                                    }
                                    if (AITimer >= 20 && dashCounter >= 2)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Jyrina>(), NPC.damage, NPC.velocity, false, SoundID.Item1, NPC.spriteDirection == 1 ? 0 : 2);
                                        chariotCooldown = 5;
                                        dashCounter = 0;
                                        NPC.ai[3] = 0;
                                        AIState = ActionState.ResetVars;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Dust Devil
                        case 8:
                            if (player.ZoneDesert)
                            {
                                AITimer++;
                                if (AITimer == 2)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.WindLong, NPC.position);
                                }
                                if (AITimer % 2 == 0 && AITimer > 8 && AITimer < 68)
                                {
                                    NPC.ai[3] = 1;
                                    for (int i = 0; i < 5; i++)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoGust>(), NPC.damage, RedeHelper.Spread(10), false, SoundID.Item1);
                                    }
                                }
                                if (AITimer >= 80)
                                {
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Lightning Bolts
                        case 9:
                            AITimer++;
                            if (AITimer % 10 == 0 && AITimer < 50)
                            {
                                Vector2 ai = RedeHelper.PolarVector(15, Main.rand.NextFloat(0, MathHelper.TwoPi));
                                float ai2 = Main.rand.Next(100);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(15, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<UkkoLightning>(), (int)(NPC.damage * 0.9f) / 4, 0, Main.myPlayer, ai.ToRotation(), ai2);
                            }
                            if (AITimer >= 50)
                            {
                                AIState = ActionState.ResetVars;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Thunder Surge
                        case 10:
                            if (NPC.life < (int)(NPC.lifeMax * 0.8f))
                            {
                                AITimer++;
                                if (AITimer == 8)
                                {
                                    NPC.ai[3] = 1;
                                    NPC.Shoot(player.Center, ModContent.ProjectileType<StormSummonerPro>(), (int)(NPC.damage * 0.92f), Vector2.Zero, false, SoundID.Item1, Main.rand.Next(4));
                                }
                                if (AITimer >= 80)
                                {
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Static Dualcast
                        case 11:
                            if (NPC.life < (int)(NPC.lifeMax * 0.5f))
                            {
                                AITimer++;
                                if (AITimer < 60)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 0.7f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);
                                }
                                if (AITimer == 60)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(8, (player.Center - NPC.Center).ToRotation()), true, CustomSounds.Zap2);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(8, (player.Center - NPC.Center).ToRotation()), true, CustomSounds.Zap2, 1);
                                }
                                if (NPC.life < (int)(NPC.lifeMax * 0.3f))
                                {
                                    if (AITimer == 90)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(6, (player.Center - NPC.Center).ToRotation()), true, CustomSounds.Zap2);
                                    }
                                    if (AITimer >= 120)
                                    {
                                        AIState = ActionState.ResetVars;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else
                                {
                                    if (AITimer >= 90)
                                    {
                                        AIState = ActionState.ResetVars;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }

                            break;
                        #endregion

                        #region Blizzard
                        case 12:
                            if (player.ZoneSnow)
                            {
                                AITimer++;
                                if (AITimer < 160)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BlueFairy, 0f, 0f, 100, default, 0.9f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);
                                }
                                if (AITimer >= 60 && AITimer <= 160)
                                {
                                    if (Main.rand.NextBool(5))
                                    {
                                        int A = Main.rand.Next(-200, 200) * 6;
                                        int B = Main.rand.Next(-200, 200) - 1000;

                                        NPC.Shoot(player.Center + new Vector2(A, B), ModContent.ProjectileType<UkkoBlizzard>(), (int)(NPC.damage * 0.75f), new Vector2(2, 4), false, SoundID.Item1);
                                    }
                                }
                                if (AITimer >= 190)
                                {
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Rain Cloud
                        case 13:
                            AITimer++;
                            if (AITimer == 8)
                            {
                                NPC.ai[3] = 1;
                                NPC.Shoot(player.Center - new Vector2(0, 200), ModContent.ProjectileType<UkkoRainCloud>(), 0, Vector2.Zero, false, SoundID.Item1);
                            }
                            if (AITimer >= 60)
                            {
                                AIState = ActionState.ResetVars;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Static Discharge
                        case 14:
                            if (burstCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer == 2)
                                {
                                    NPC.ai[3] = 3;
                                    SoundEngine.PlaySound(SoundID.Thunder);
                                }
                                if (AITimer < 52)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 1f);
                                        dust.velocity = -NPC.DirectionTo(dust.position);
                                    }
                                }
                                if (AITimer == 52)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);
                                    for (int i = -16; i <= 16; i++)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoThunderwave>(), (int)(NPC.damage * 0.8f), 10 * Vector2.UnitX.RotatedBy(Math.PI / 16 * i), false, SoundID.Item1);
                                    }
                                }
                                if (AITimer == 58)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);
                                    for (int i = -8; i <= 8; i++)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoThunderwave>(), (int)(NPC.damage * 0.8f), 8 * Vector2.UnitX.RotatedBy(Math.PI / 8 * i), false, SoundID.Item1);
                                    }
                                }
                                if (AITimer >= 160)
                                {
                                    burstCooldown = 8;
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.AkkaSummon:
                    switch (AttackID)
                    {
                        case 0:
                            if (AITimer == 0)
                            {
                                if (NPC.DistanceSQ(ArenaWorld.arenaMiddle - new Vector2(0, 60)) < 20 * 20)
                                {
                                    AITimer = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.MoveToVector2(ArenaWorld.arenaMiddle - new Vector2(0, 60), 40);
                                break;
                            }
                            NPC.velocity *= 0.8f;
                            if (AITimer++ == 10)
                            {
                                if (!Main.dedServ)
                                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                                NPC.spriteDirection = 1;
                                NPC.dontTakeDamage = true;
                                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                            }
                            if (AITimer >= 60)
                            {
                                player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                player.RedemptionScreen().lockScreen = true;
                                player.RedemptionScreen().cutscene = true;
                                NPC.LockMoveRadius(player);
                            }
                            if (AITimer >= 120)
                            {
                                MoveVector3 = NPC.Center;
                                AITimer = 0;
                                AttackID = 1;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            player.RedemptionScreen().ScreenFocusPosition = MoveVector3;
                            player.RedemptionScreen().lockScreen = true;
                            player.RedemptionScreen().cutscene = true;

                            if (AITimer++ <= 30)
                                NPC.velocity.Y += 0.4f;
                            else
                            {
                                NPC.velocity.Y -= AITimer >= 70 ? 10f : 0.4f;
                            }
                            if (AITimer == 70)
                            {
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;
                                SoundEngine.PlaySound(CustomSounds.ElectricSlash2, NPC.position);
                            }

                            if (NPC.Center.Y <= ArenaWorld.arenaTopLeft.Y - 100)
                            {
                                NPC.velocity *= 0;
                                NPC.alpha = 255;
                                AITimer = 0;
                                AttackID = 2;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            if (AITimer == 120)
                                SoundEngine.PlaySound(SoundID.Item165 with { Pitch = -0.1f });
                            if (AITimer++ >= 240)
                            {
                                player.RedemptionScreen().ScreenFocusPosition = MoveVector3;
                                player.RedemptionScreen().lockScreen = true;
                                player.RedemptionScreen().cutscene = true;
                                NPC.alpha = 0;
                                if (NPC.DistanceSQ(MoveVector3) < 20 * 20)
                                {
                                    if (!Main.dedServ)
                                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest2");

                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 80, (int)ArenaWorld.arenaTopLeft.Y - 100, ModContent.NPCType<Akka>(), 0, 0, 0, NPC.whoAmI);
                                    NPC.spriteDirection = 1;
                                    AITimer = 0;
                                    AttackID = 3;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.MoveToVector2(MoveVector3, 40);
                            }
                            break;
                        case 3:
                            player.RedemptionScreen().ScreenFocusPosition = MoveVector3;
                            player.RedemptionScreen().lockScreen = true;
                            player.RedemptionScreen().cutscene = true;
                            NPC.velocity *= 0.9f;
                            if (AITimer == 180)
                                EmoteBubble.NewBubble(0, new WorldUIAnchor(NPC), 50);

                            if (AITimer++ >= 250)
                            {
                                NPC.dontTakeDamage = false;
                                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                                akkaArrive = true;
                                AITimer = 0;
                                AttackID = 0;
                                AIState = ActionState.ResetVars;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 112;
                if (NPC.frame.Y > (112 * 5))
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                }
            }
            switch (NPC.ai[3])
            {
                case 1:
                    frameCounters++;
                    if (frameCounters > 6)
                    {
                        wandFrame++;
                        frameCounters = 0;
                    }
                    if (wandFrame >= 9)
                    {
                        NPC.ai[3] = 0;
                        wandFrame = 0;
                        frameCounters = 0;
                    }
                    break;
                case 2:
                    frameCounters++;
                    if (frameCounters > 3)
                    {
                        chariotFrame++;
                        frameCounters = 0;
                    }
                    if (chariotFrame >= 9)
                    {
                        chariotFrame = 0;
                    }
                    break;
                case 3:
                    frameCounters++;
                    if (frameCounters > 5)
                    {
                        dischargeFrame++;
                        frameCounters = 0;
                    }
                    if (dischargeFrame >= 12)
                    {
                        frameCounters = 0;
                        NPC.ai[3] = 0;
                        dischargeFrame = 0;
                    }
                    break;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return AttackID == 7;
        }
        public override bool CheckActive()
        {
            player = Main.player[NPC.target];
            return !player.active || player.dead;
        }
        public Vector2 Pos()
        {
            Vector2 Pos1 = new(player.Center.X > NPC.Center.X ? player.Center.X + Main.rand.Next(-500, -300) : player.Center.X + Main.rand.Next(300, 500), player.Center.Y + Main.rand.Next(-400, 200));
            return Pos1;
        }
        public Vector2 ChargePos()
        {
            Vector2 ChargePos1 = new(player.Center.X > NPC.Center.X ? player.Center.X - 1400 : player.Center.X + 1400, player.Center.Y + Main.rand.Next(-80, 80));
            return ChargePos1;
        }
        private void Target()
        {
            player = Main.player[NPC.target];
        }
        private void DespawnHandler()
        {
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.velocity = new Vector2(0f, -20f);
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
        private void Rain()
        {
            int akkaID = NPC.FindFirstNPC(ModContent.NPCType<Akka>());
            if (Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f || Main.player[NPC.target].dead)
            {
                if (StopRain == 0)
                {
                    Main.StopRain();
                    Main.SyncRain();
                    StopRain = 1;
                }
            }
            if (RunOnce == 0)
            {
                Main.StopRain();
                Main.SyncRain();
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
            if ((AIState is ActionState.AkkaSummon && AttackID >= 1) || (akkaID > -1 && Main.npc[akkaID].active))
            {
                Main.rainTime = 3600;
                Main.raining = true;
                Main.maxRaining = 0.4f;
                Main.windSpeedTarget = 0.5f;
            }
            else
            {
                Main.rainTime = 3600;
                Main.raining = true;
                Main.maxRaining = 0.65f;
                Main.windSpeedTarget = 1;
            }
            RunOnce = 1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D wandAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_WandRaise").Value;
            Texture2D wandGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_WandRaise_Glow").Value;
            Texture2D dischargeAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Burst").Value;
            Texture2D dischargeGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Burst_Glow").Value;
            Texture2D chariotAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Chariot").Value;
            Texture2D chariotGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Chariot_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightGoldenrodYellow, Color.LightYellow * 0.7f, Color.LightGoldenrodYellow);

            switch (NPC.ai[3])
            {
                case 0:
                    if (!NPC.IsABestiaryIconDummy)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                        GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                        for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                        {
                            Vector2 oldPos = NPC.oldPos[i];
                            spriteBatch.Draw(texture, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                        }
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                    spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                    spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                    break;
                case 1:
                    int wandHeight = wandAni.Height / 9;
                    int wandY = wandHeight * wandFrame;
                    Vector2 wandDrawCenter = new(NPC.Center.X + 4, NPC.Center.Y - 16);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(wandAni, oldPos + new Vector2(4, -16) + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, wandY, wandAni.Width, wandHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(wandAni.Width / 2f, wandHeight / 2f), NPC.scale, effects, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    spriteBatch.Draw(wandAni, wandDrawCenter - screenPos, new Rectangle?(new Rectangle(0, wandY, wandAni.Width, wandHeight)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(wandAni.Width / 2f, wandHeight / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(wandGlow, wandDrawCenter - screenPos, new Rectangle?(new Rectangle(0, wandY, wandAni.Width, wandHeight)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(wandAni.Width / 2f, wandHeight / 2f), NPC.scale, effects, 0f);
                    break;
                case 2:
                    int chariotHeight = chariotAni.Height / 9;
                    int chariotY = chariotHeight * chariotFrame;
                    Vector2 chariotDrawCenter = new(NPC.Center.X, NPC.Center.Y);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(chariotAni, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, chariotY, chariotAni.Width, chariotHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(chariotAni.Width / 2f, chariotHeight / 2f), NPC.scale, effects, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    spriteBatch.Draw(chariotAni, chariotDrawCenter - screenPos, new Rectangle?(new Rectangle(0, chariotY, chariotAni.Width, chariotHeight)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(chariotAni.Width / 2f, chariotHeight / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(chariotGlow, chariotDrawCenter - screenPos, new Rectangle?(new Rectangle(0, chariotY, chariotAni.Width, chariotHeight)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(chariotAni.Width / 2f, chariotHeight / 2f), NPC.scale, effects, 0f);
                    break;
                case 3:
                    int dischargeHeight = dischargeAni.Height / 12;
                    int dischargeY = dischargeHeight * dischargeFrame;
                    Vector2 dischargeDrawCenter = new(NPC.Center.X + 4, NPC.Center.Y - 16);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(dischargeAni, oldPos + new Vector2(4, -16) + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, dischargeY, dischargeAni.Width, dischargeHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(dischargeAni.Width / 2f, dischargeHeight / 2f), NPC.scale, effects, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    spriteBatch.Draw(dischargeAni, dischargeDrawCenter - screenPos, new Rectangle?(new Rectangle(0, dischargeY, dischargeAni.Width, dischargeHeight)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(dischargeAni.Width / 2f, dischargeHeight / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(dischargeGlow, dischargeDrawCenter - screenPos, new Rectangle?(new Rectangle(0, dischargeY, dischargeAni.Width, dischargeHeight)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(dischargeAni.Width / 2f, dischargeHeight / 2f), NPC.scale, effects, 0f);
                    break;
            }
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 35; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, Scale: 2);
                    Main.dust[dustIndex2].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone);
        }
    }
}