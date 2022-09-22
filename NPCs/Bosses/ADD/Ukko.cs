using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PostML.Magic;
using Redemption.Items.Weapons.PostML.Summon;
using Redemption.NPCs.Bosses.Thorn;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Buffs.Debuffs;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Items.Weapons.PreHM.Summon;

namespace Redemption.NPCs.Bosses.ADD
{
    [AutoloadBossHead]
    public class Ukko : ModNPC
    {
        private Player player;
        private readonly Vector2[] oldPos = new Vector2[3];
        private readonly float[] oldrot = new float[3];

        private int RunOnce = 0;
        private int StopRain = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ukko");
            Main.npcFrameCount[NPC.type] = 6;
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
            NPC.lifeMax = 320000;
            NPC.damage = 130;
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
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0.75f;
            return true;
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

                new FlavorTextBestiaryInfoElement("")
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
            if (!RedeBossDowned.downedADD)
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

        public override void AI()
        {
            Rain();

            Target();

            DespawnHandler();

            Player player = Main.player[NPC.target];
            for (int k = oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldrot[k] = oldrot[k - 1];
            }
            oldPos[0] = NPC.Center;
            oldrot[0] = NPC.rotation;
            if (NPC.ai[1] == 7)
            {
                NPC.rotation = NPC.velocity.ToRotation();
                if (NPC.velocity.X < 0)
                    NPC.spriteDirection = -1;
                else
                    NPC.spriteDirection = 1;
            }
            else
            {
                NPC.rotation = 0f;
                if (player.Center.X > NPC.Center.X)
                    NPC.spriteDirection = 1;
                else
                    NPC.spriteDirection = -1;
            }
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
            if (NPC.ai[3] == 1)
            {
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
            }
            if (NPC.ai[3] == 2)
            {
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
            }
            if (NPC.ai[3] == 3)
            {
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
            }

            Vector2 ThunderwavePos = new(player.Center.X > NPC.Center.X ? player.Center.X - 700 : player.Center.X + 700, player.Center.Y);

            if (NPC.ai[0] == 0)
            {
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
            }
            else if (NPC.ai[0] == 1)
            {
                if (NPC.DistanceSQ(MoveVector2) < 10 * 10)
                {
                    NPC.velocity *= 0;
                    NPC.ai[0]++;
                    NPC.ai[1] = Main.rand.Next(13);
                    NPC.netUpdate = true;
                }
                else
                {
                    NPC.MoveToVector2(MoveVector2, 30);
                }
            }
            else if (NPC.ai[0] == 2)
            {
                switch ((int)NPC.ai[1])
                {
                    // Thunderclap
                    #region Thunderclap
                    case 0:
                        NPC.ai[2]++;
                        if (NPC.ai[2] == 8)
                        {
                            NPC.ai[3] = 1;
                            int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<UkkoStrike>(), 110 / 3, 3, Main.myPlayer);
                            Main.projectile[p].netUpdate = true;
                        }
                        if (NPC.ai[2] >= 60)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Gust
                    #region Gust
                    case 1:
                        NPC.ai[2]++;
                        if (NPC.ai[2] == 2)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.WindLong, NPC.position);
                        }
                        if (NPC.ai[2] % 2 == 0 && NPC.ai[2] > 8 && NPC.ai[2] < 48)
                        {
                            NPC.ai[3] = 1;
                            for (int i = 0; i < 2; i++)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<UkkoGust>(), 0, 0);
                                Main.projectile[p].netUpdate = true;
                            }
                        }
                        if (NPC.ai[2] >= 50)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Thunderwave
                    #region Thunderwave
                    case 2:
                        if (NPC.ai[2] == 0)
                        {
                            if (NPC.DistanceSQ(ThunderwavePos) < 100 * 100)
                            {
                                NPC.velocity = NPC.DirectionTo(player.Center).RotatedBy(Math.PI / 2) * 40;
                                NPC.ai[2] = 1;
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.MoveToVector2(ThunderwavePos, 30);
                        }
                        else
                        {
                            NPC.ai[2]++;
                            if (Main.raining ? NPC.ai[2] < 30 : NPC.ai[2] < 20)
                                NPC.velocity -= NPC.velocity.RotatedBy(Math.PI / 2) * NPC.velocity.Length() / NPC.Distance(player.Center);
                            else
                                NPC.velocity *= 0f;

                            if (Main.raining ? NPC.ai[2] % 3 == 0 && NPC.ai[2] < 30 : NPC.ai[2] % 5 == 0 && NPC.ai[2] < 20)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);
                                float Speed = 18f;
                                Vector2 vector8 = new(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                                float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                int num54 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1) + Main.rand.Next(-1, 1), (float)(Math.Sin(rotation) * Speed * -1) + Main.rand.Next(-1, 1), ModContent.ProjectileType<UkkoThunderwave>(), 90 / 3, 0f, 0, NPC.whoAmI);
                                Main.projectile[num54].netUpdate = true;
                            }
                            if (Main.raining ? NPC.ai[2] >= 35 : NPC.ai[2] >= 25)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                    #endregion

                    // Call Lightning
                    #region Call Lightning
                    case 3:
                        if (NPC.life < (int)(NPC.lifeMax * 0.6f))
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 2)
                            {
                                NPC.ai[3] = 1;
                                SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                            }
                            if (NPC.ai[2] > 40 && NPC.ai[2] < 160)
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
                            if ((Main.raining ? NPC.ai[2] % 15 == 0 : NPC.ai[2] % 20 == 0) && NPC.ai[2] > 40 && NPC.ai[2] < 160)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + Main.rand.Next(-300, 300), player.Center.Y + Main.rand.Next(-300, 300), 0f, 0f, ModContent.ProjectileType<UkkoStrike>(), 110 / 3, 3, Main.myPlayer);
                                Main.projectile[p].netUpdate = true;
                            }
                            if (NPC.ai[2] >= 180)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Mending
                    #region Mending
                    case 4:
                        if (NPC.life < (int)(NPC.lifeMax * 0.5f) && mendingCooldown == 0)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 2)
                            {
                                NPC.ai[3] = 1;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Quake, NPC.position);
                            }
                            if (NPC.ai[2] < 60)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 3f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);
                                }
                                NPC.life += 200;
                                NPC.HealEffect(200);
                            }
                            if (NPC.ai[2] >= 120)
                            {
                                mendingCooldown = 20;
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Stoneskin
                    #region Stoneskin
                    case 5:
                        if (NPC.life < (int)(NPC.lifeMax * 0.8f) && stoneskinCooldown == 0)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 2)
                            {
                                NPC.ai[3] = 1;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Quake, NPC.position);
                            }
                            if (NPC.ai[2] < 60)
                            {
                                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 2f);
                                dust.velocity = -NPC.DirectionTo(dust.position);

                                NPC.AddBuff(ModContent.BuffType<StoneskinBuff>(), 3600);
                            }
                            if (NPC.ai[2] >= 90)
                            {
                                stoneskinCooldown = 10;
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Dancing Lights
                    #region Dancing Lights
                    case 6:
                        if (player.ZoneHallow)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 1)
                            {
                                NPC.ai[3] = 1;
                            }
                            if (NPC.ai[2] == 30)
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
                            if (NPC.ai[2] > 80 && NPC.ai[2] < 160)
                            {
                                if (Main.rand.NextBool(8))
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + Main.rand.Next(-600, -300), player.Center.Y + Main.rand.Next(-600, 600), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), ModContent.ProjectileType<UkkoDancingLights>(), 0, 0, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (Main.rand.NextBool(8))
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + Main.rand.Next(300, 600), player.Center.Y + Main.rand.Next(-600, 600), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), ModContent.ProjectileType<UkkoDancingLights>(), 0, 0, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (Main.rand.NextBool(8))
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + Main.rand.Next(-600, 600), player.Center.Y + Main.rand.Next(-600, -300), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), ModContent.ProjectileType<UkkoDancingLights>(), 0, 0, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (Main.rand.NextBool(8))
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + Main.rand.Next(-600, 600), player.Center.Y + Main.rand.Next(300, 600), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), ModContent.ProjectileType<UkkoDancingLights>(), 0, 0, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                            }
                            if (NPC.ai[2] >= 200)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Lightning Chariot
                    #region Lightning Chariot
                    case 7:
                        if (chariotCooldown == 0)
                        {
                            NPC.ai[3] = 2;
                            if (NPC.velocity.X < 0)
                            {
                                NPC.rotation += (float)Math.PI;
                            }
                            if (NPC.ai[2] == 0)
                            {
                                if (NPC.DistanceSQ(MoveVector3) < 200 * 200)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Jyrina, NPC.position);

                                    NPC.velocity = NPC.DirectionTo(player.Center) * 50;
                                    NPC.ai[2] = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.MoveToVector2(MoveVector3, 30);
                            }
                            else
                            {
                                NPC.ai[2]++;
                                if (NPC.ai[2] % 8 == 0 && NPC.ai[2] < 50)
                                {
                                    Vector2 ai = RedeHelper.PolarVector(12, -NPC.velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f));
                                    float ai2 = Main.rand.Next(100);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(12, -NPC.velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<UkkoLightning>(), NPC.damage / 4, 0, Main.myPlayer, ai.ToRotation(), ai2);
                                }
                                if (NPC.ai[2] >= 50 && dashCounter < 2)
                                {
                                    dashCounter++;
                                    MoveVector3 = ChargePos();
                                    NPC.ai[2] = 0;
                                }
                                if (NPC.ai[2] >= 20 && dashCounter >= 2)
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, NPC.velocity.X, NPC.velocity.Y, ModContent.ProjectileType<Jyrina>(), 130 / 3, 3, Main.myPlayer, NPC.spriteDirection == 1 ? 0 : 2);
                                    Main.projectile[p].netUpdate = true;
                                    chariotCooldown = 5;
                                    dashCounter = 0;
                                    NPC.ai[3] = 0;
                                    NPC.ai[0] = 0;
                                    NPC.ai[2] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Dust Devil
                    #region Dust Devil
                    case 8:
                        if (player.ZoneDesert)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 2)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.WindLong, NPC.position);
                            }
                            if (NPC.ai[2] % 2 == 0 && NPC.ai[2] > 8 && NPC.ai[2] < 68)
                            {
                                NPC.ai[3] = 1;
                                for (int i = 0; i < 5; i++)
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f), ModContent.ProjectileType<UkkoGust>(), 0, 0);
                                    Main.projectile[p].netUpdate = true;
                                }
                            }
                            if (NPC.ai[2] >= 80)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Lightning Bolts
                    #region Lightning Bolts
                    case 9:
                        NPC.ai[2]++;
                        if (NPC.ai[2] % 10 == 0 && NPC.ai[2] < 50)
                        {
                            Vector2 ai = RedeHelper.PolarVector(15, Main.rand.NextFloat(0, MathHelper.TwoPi));
                            float ai2 = Main.rand.Next(100);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(15, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<UkkoLightning>(), NPC.damage / 4, 0, Main.myPlayer, ai.ToRotation(), ai2);
                        }
                        if (NPC.ai[2] >= 50)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Thunder Surge
                    #region Thunder Surge
                    case 10:
                        if (NPC.life < (int)(NPC.lifeMax * 0.8f))
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 8)
                            {
                                NPC.ai[3] = 1;
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<StormSummonerPro>(), 110 / 3, 3, Main.myPlayer, Main.rand.Next(4));
                                Main.projectile[p].netUpdate = true;
                            }
                            if (NPC.ai[2] >= 80)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Static Dualcast
                    #region Static Dualcast
                    case 11:
                        if (NPC.life < (int)(NPC.lifeMax * 0.5f))
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] < 60)
                            {
                                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 0.7f);
                                dust.velocity = -NPC.DirectionTo(dust.position);
                            }
                            if (NPC.ai[2] == 60)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);

                                float Speed = 8f;
                                Vector2 vector8 = new(NPC.Center.X, NPC.Center.Y);
                                float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                int p1 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), ModContent.ProjectileType<DualcastBall>(), 110 / 3, 3, Main.myPlayer, 0f);
                                Main.projectile[p1].netUpdate = true;
                                int p2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), ModContent.ProjectileType<DualcastBall>(), 110 / 3, 3, Main.myPlayer, 1f);
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (NPC.life < (int)(NPC.lifeMax * 0.3f))
                            {
                                if (NPC.ai[2] == 90)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);

                                    float Speed = 6f;
                                    Vector2 vector8 = new(NPC.Center.X, NPC.Center.Y);
                                    float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                    int p1 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), ModContent.ProjectileType<DualcastBall>(), 110 / 3, 3, Main.myPlayer, 0f);
                                    Main.projectile[p1].netUpdate = true;
                                    int p2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), ModContent.ProjectileType<DualcastBall>(), 110 / 3, 3, Main.myPlayer, 1f);
                                    Main.projectile[p2].netUpdate = true;
                                }
                                if (NPC.ai[2] >= 120)
                                {
                                    NPC.ai[0] = 0;
                                    NPC.ai[2] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                if (NPC.ai[2] >= 90)
                                {
                                    NPC.ai[0] = 0;
                                    NPC.ai[2] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }

                        break;
                    #endregion

                    // Blizzard
                    #region Blizzard
                    case 12:
                        if (player.ZoneSnow)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] < 160)
                            {
                                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BlueFairy, 0f, 0f, 100, default, 0.9f);
                                dust.velocity = -NPC.DirectionTo(dust.position);
                            }
                            if (NPC.ai[2] >= 60 && NPC.ai[2] <= 160)
                            {
                                if (Main.rand.NextBool(5))
                                {
                                    int A = Main.rand.Next(-200, 200) * 6;
                                    int B = Main.rand.Next(-200, 200) - 1000;

                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + A, player.Center.Y + B, 2f, 4f, ModContent.ProjectileType<UkkoBlizzard>(), 80 / 3, 3, Main.myPlayer, 0, 0);
                                    Main.projectile[p].netUpdate = true;
                                }
                            }
                            if (NPC.ai[2] >= 190)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Rain Cloud
                    #region Rain Cloud
                    case 13:
                        NPC.ai[2]++;
                        if (NPC.ai[2] == 8)
                        {
                            NPC.ai[3] = 1;
                            int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<UkkoRainCloud>(), 0, 0, Main.myPlayer);
                            Main.projectile[p].netUpdate = true;
                        }
                        if (NPC.ai[2] >= 60)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    #endregion

                    // Static Discharge
                    #region Static Discharge
                    case 14:
                        if (burstCooldown == 0)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 2)
                            {
                                NPC.ai[3] = 3;
                                SoundEngine.PlaySound(SoundID.Thunder);
                            }
                            if (NPC.ai[2] < 52)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 1f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);
                                }
                            }
                            if (NPC.ai[2] == 52)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);
                                for (int i = -16; i <= 16; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 10 * Vector2.UnitX.RotatedBy(Math.PI / 16 * i), ModContent.ProjectileType<UkkoThunderwave>(), 90 / 3, 0f, 0, NPC.whoAmI);
                                }
                            }
                            if (NPC.ai[2] == 58)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);
                                for (int i = -8; i <= 8; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 8 * Vector2.UnitX.RotatedBy(Math.PI / 8 * i), ModContent.ProjectileType<UkkoThunderwave>(), 90 / 3, 0f, 0, NPC.whoAmI);
                                }
                            }
                            if (NPC.ai[2] >= 160)
                            {
                                burstCooldown = 8;
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.ai[1] = Main.rand.Next(13);
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                        #endregion
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (NPC.ai[1] == 7)
                return true;
            else
                return false;
        }
        public override bool CheckActive()
        {
            player = Main.player[NPC.target];
            if (!player.active || player.dead)
                return true;
            else
                return false;
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
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }
        }
        private void Rain()
        {
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
            Main.rainTime = 3600;
            Main.raining = true;
            Main.maxRaining = 0.65f;
            Main.windSpeedTarget = 1;
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

            if (NPC.ai[3] == 0)
            {
                for (int k = oldPos.Length - 1; k >= 0; k -= 1)
                {
                    float alpha = 1f - (k + 1) / (float)(oldPos.Length + 2);
                    spriteBatch.Draw(texture, oldPos[k] - screenPos, NPC.frame, drawColor * (0.5f * alpha), oldrot[k], NPC.frame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                }

                spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }
            else if (NPC.ai[3] == 1)
            {
                int num214 = wandAni.Height / 9;
                int y6 = num214 * wandFrame;
                Vector2 drawCenter = new(NPC.Center.X + 4, NPC.Center.Y - 16);
                spriteBatch.Draw(wandAni, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, wandAni.Width, num214)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(wandAni.Width / 2f, num214 / 2f), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(wandGlow, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, wandAni.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(wandAni.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
            }
            else if (NPC.ai[3] == 2)
            {
                int num214 = chariotAni.Height / 9;
                int y6 = num214 * chariotFrame;
                Vector2 drawCenter = new(NPC.Center.X, NPC.Center.Y);

                for (int k = oldPos.Length - 1; k >= 0; k -= 1)
                {
                    float alpha = 1f - (k + 1) / (float)(oldPos.Length + 2);
                    spriteBatch.Draw(chariotAni, oldPos[k] - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, chariotAni.Width, num214)), drawColor * (0.5f * alpha), oldrot[k], new Vector2(chariotAni.Width / 2f, num214 / 2f), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                }

                spriteBatch.Draw(chariotAni, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, chariotAni.Width, num214)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(chariotAni.Width / 2f, num214 / 2f), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(chariotGlow, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, chariotAni.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(chariotAni.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
            }
            else if (NPC.ai[3] == 3)
            {
                int num214 = dischargeAni.Height / 12;
                int y6 = num214 * dischargeFrame;
                Vector2 drawCenter = new(NPC.Center.X + 4, NPC.Center.Y - 16);
                spriteBatch.Draw(dischargeAni, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, dischargeAni.Width, num214)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(dischargeAni.Width / 2f, num214 / 2f), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(dischargeGlow, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, dischargeAni.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(dischargeAni.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
            }

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 35; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 2.0f);
                    Main.dust[dustIndex2].velocity *= 4.6f;
                }
            }
            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 1.0f);
            Main.dust[dustIndex].velocity *= 4.6f;

        }
    }
}