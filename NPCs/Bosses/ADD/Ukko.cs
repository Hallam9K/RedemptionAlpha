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
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Redemption.BaseExtension;
using Terraria.GameContent.UI;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Terraria.GameContent.ItemDropRules;
using Redemption.Particles;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Accessories.PostML;
using System.IO;
using ReLogic.Content;
using Terraria.Localization;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.ADD
{
    [AutoloadBossHead]
    public class Ukko : ModNPC
    {
        private static Asset<Texture2D> glowMask;
        private static Asset<Texture2D> wandAni;
        private static Asset<Texture2D> wandGlow;
        private static Asset<Texture2D> swipeAni;
        private static Asset<Texture2D> swipeGlow;
        private static Asset<Texture2D> swipeSlash;
        private static Asset<Texture2D> chariotAni;
        private static Asset<Texture2D> chariotGlow;
        private static Asset<Texture2D> jyrina;
        private static Asset<Texture2D> jyrinaGlow;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glowMask = ModContent.Request<Texture2D>(Texture + "_Glow");
            wandAni = ModContent.Request<Texture2D>(Texture + "_WandRaise");
            wandGlow = ModContent.Request<Texture2D>(Texture + "_WandRaise_Glow");
            swipeAni = ModContent.Request<Texture2D>(Texture + "_Swipe");
            swipeGlow = ModContent.Request<Texture2D>(Texture + "_Swipe_Glow");
            swipeSlash = ModContent.Request<Texture2D>(Texture + "_Swipe_Slash");
            chariotAni = ModContent.Request<Texture2D>(Texture + "_Chariot");
            chariotGlow = ModContent.Request<Texture2D>(Texture + "_Chariot_Glow");
            jyrina = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/ADD/Jyrina");
            jyrinaGlow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/ADD/Jyrina_Glow");
        }
        public override void Unload()
        {
            glowMask = null;
            wandAni = null;
            wandGlow = null;
            swipeAni = null;
            swipeGlow = null;
            swipeSlash = null;
            chariotAni = null;
            chariotGlow = null;
            jyrina = null;
            jyrinaGlow = null;
        }
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
            // DisplayName.SetDefault("Ukko");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 40),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCThunder[Type] = true;
            ElementID.NPCEarth[Type] = true;
        }
        public int GuardPointMax;
        public override void SetDefaults()
        {
            NPC.lifeMax = 128000;
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
            GuardPointMax = NPC.lifeMax / 50;
            NPC.RedemptionGuard().GuardBroken = true;
            NPC.BossBar = ModContent.GetInstance<UkkoHealthBar>();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossUkko");

            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Blood] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Earth] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Thunder] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Water] *= .9f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Wind] *= .9f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Ice] *= 1.25f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Shadow] *= 1.1f;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ProjectileID.LastPrismLaser)
                modifiers.FinalDamage /= 3;

            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                modifiers.FinalDamage *= .75f;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (RedeBossDowned.downedGGBossFirst == 1 && RedeBossDowned.downedGGBossFirst == 2)
                modifiers.FinalDamage *= .75f;

            if (NPC.RedemptionGuard().GuardPoints >= 0 && !NPC.RedemptionGuard().GuardBroken)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.DD2_WitherBeastCrystalImpact, .25f, false, DustID.Stone, CustomSounds.EarthBoom, 10, 1, 2000);
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Rain,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Ukko"))
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<UkkoBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UkonKirvesTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<UkkoRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<JyrinaMount>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<UkkoMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Salamanisku>(), ModContent.ItemType<Ukonvasara>()));

            npcLoot.Add(notExpertRule);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
            if (!RedeBossDowned.downedADD && !NPC.AnyNPCs(ModContent.NPCType<Akka>()))
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gray, "+0", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.ADDDefeat"), 300, 30, 0, Color.DarkGoldenrod);

                }
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<Akka>()) && !RedeBossDowned.downedADD && RedeBossDowned.downedGGBossFirst == 0)
                RedeBossDowned.downedGGBossFirst = 3;
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedADD, -1);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(akkaArrive);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            akkaArrive = reader.ReadBoolean();
        }

        public Vector2 MoveVector2;
        public Vector2 MoveVector3;
        public int frameCounters;
        public int mendingCooldown;
        public int stoneskinCooldown;
        public int chariotCooldown;
        public int burstCooldown;
        public int chariotFrame;
        public int jyrinaFrame;
        public int dashCounter;
        public int swipeFrame;
        public bool akkaArrive;

        public override void AI()
        {
            Rain();

            Target();

            if (NPC.DespawnHandler(0, 20))
                return;
            Player player = Main.player[NPC.target];
            if (player.active && !player.dead)
                NPC.DiscourageDespawn(60);

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
            Vector2 ThunderwavePos = new(player.Center.X + (700 * NPC.RightOfDir(player)), player.Center.Y);
            Vector2 EarthProtectPos = new(player.Center.X, player.Center.Y - 400);
            Vector2 HammerPos = new(NPC.Center.X - (24 * NPC.spriteDirection), NPC.Center.Y - 101);
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
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Ukko.Name"), 60, 90, 0.8f, 0, Color.LightGoldenrodYellow, Language.GetTextValue("Mods.Redemption.TitleCard.Ukko.Modifier"));

                    NPC.Shoot(new Vector2(NPC.Center.X - (118 * 16) - 10, NPC.Center.Y + 8), ModContent.ProjectileType<UkkoBarrier>(), 0, Vector2.Zero, 0, 1);
                    NPC.Shoot(new Vector2(NPC.Center.X + (118 * 16) + 26, NPC.Center.Y + 8), ModContent.ProjectileType<UkkoBarrier>(), 0, Vector2.Zero, 0, -1);
                    NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y - (118 * 16) - 10), ModContent.ProjectileType<UkkoBarrierH>(), 0, Vector2.Zero, 0, 1);
                    NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y + (118 * 16) + 26), ModContent.ProjectileType<UkkoBarrierH>(), 0, Vector2.Zero, 0, -1);

                    ArenaWorld.arenaBoss = "ADD";
                    ArenaWorld.arenaTopLeft = new Vector2(NPC.Center.X - (120 * 16) + 8, NPC.Center.Y - (120 * 16) + 8);
                    ArenaWorld.arenaSize = new Vector2(240 * 16, 240 * 16);
                    ArenaWorld.arenaMiddle = NPC.Center;
                    ArenaWorld.arenaActive = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);

                    NPC.ai[0]++;
                    NPC.netUpdate = true;
                    break;
                case ActionState.ResetVars:
                    if (mendingCooldown > 0)
                        mendingCooldown--;
                    if (stoneskinCooldown > 0 && NPC.RedemptionGuard().GuardPoints <= 0)
                        stoneskinCooldown--;
                    if (chariotCooldown > 0)
                        chariotCooldown--;
                    if (burstCooldown > 0)
                        burstCooldown--;
                    NPC.ai[3] = 0;
                    swipeFrame = 0;
                    MoveVector2 = Pos();
                    MoveVector3 = ChargePos();
                    NPC.ai[0]++;
                    break;
                case ActionState.Idle:
                    if (NPC.DistanceSQ(MoveVector2) < 10 * 10)
                    {
                        AITimer = 0;
                        NPC.velocity *= 0;
                        NPC.ai[0]++;
                        AttackID = Main.rand.Next(15);
                        if (akkaActive && (Main.npc[akkaID].ModNPC as Akka).teamCooldown == 0 && Main.npc[akkaID].ai[1] >= 8 && Main.npc[akkaID].ai[0] == 3)
                            AttackID = Main.npc[akkaID].ai[1] + 7;

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
                                NPC.Shoot(player.Center, ModContent.ProjectileType<UkkoStrike>(), (int)(NPC.damage * 0.92f), Vector2.Zero);
                            }
                            if (AITimer == 20)
                                NPC.ai[3] = 3;
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
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoGust>(), 0, RedeHelper.Spread(8));
                                }
                            }
                            if (AITimer >= 50)
                            {
                                NPC.ai[3] = 0;
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
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoThunderwave>(), (int)(NPC.damage * 0.8f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), CustomSounds.Zap2);
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
                                    NPC.ai[3] = 3;
                                    frameCounters = 0;
                                    swipeFrame = 0;
                                    NPC.Shoot(player.Center + new Vector2(Main.rand.Next(-300, 301)), ModContent.ProjectileType<UkkoStrike>(), (int)(NPC.damage * 0.92f), Vector2.Zero);
                                }
                                if (AITimer >= 180)
                                {
                                    NPC.ai[3] = 0;
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
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Quake with { Pitch = 0.2f }, NPC.position);
                                }
                                if (AITimer % 20 == 0)
                                {
                                    NPC.ai[3] = 3;
                                    SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                }
                                if (AITimer < 60)
                                {
                                    NPC.ai[3] = 3;
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 3f);
                                        dust.velocity = -NPC.DirectionTo(dust.position);
                                    }
                                    NPC.life += 100;
                                    NPC.HealEffect(100);
                                }
                                if (AITimer >= 120)
                                {
                                    NPC.ai[3] = 0;
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
                                if (AITimer++ == 0)
                                {
                                    if (NPC.RedemptionGuard().GuardPoints > 0)
                                    {
                                        AttackID = Main.rand.Next(15);
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
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

                                    NPC.AddBuff(ModContent.BuffType<StoneskinBuff>(), 600);
                                }
                                if (AITimer == 60)
                                {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.position);
                                    DustHelper.DrawCircle(NPC.Center, DustID.Sandnado, 10, 1, 1, 1, 2, nogravity: true);
                                    NPC.RedemptionGuard().GuardPoints = GuardPointMax;
                                    NPC.RedemptionGuard().GuardBroken = false;
                                }
                                if (AITimer >= 90)
                                {
                                    NPC.ai[3] = 0;
                                    stoneskinCooldown = 20;
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
                                if (AITimer == 30)
                                {
                                    NPC.ai[3] = 1;
                                    for (int i = 0; i < 10; i++)
                                    {
                                        int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, Scale: 3f);
                                        Main.dust[dustIndex].velocity *= 4.2f;
                                    }
                                    for (int i = 0; i < 10; i++)
                                    {
                                        int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldFlame, Scale: 3f);
                                        Main.dust[dustIndex].velocity *= 4.2f;
                                    }
                                }
                                if (AITimer > 80 && AITimer < 160)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        int dustIndex = Dust.NewDust(HammerPos - new Vector2(1, 1), 1, 1, DustID.PinkTorch, Scale: 2f);
                                        Main.dust[dustIndex].velocity *= 10f;
                                    }
                                    if (Main.rand.NextBool(3))
                                    {
                                        NPC.Shoot(player.Center + RedeHelper.PolarVector(Main.rand.Next(300, 601), RedeHelper.RandomRotation()), ModContent.ProjectileType<UkkoDancingLights>(), 0, RedeHelper.Spread(1));
                                    }
                                }
                                if (AITimer >= 200)
                                {
                                    NPC.ai[3] = 0;
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
                                if (NPC.velocity.X < 0 && dashCounter >= 2)
                                {
                                    NPC.rotation += (float)Math.PI;
                                }
                                else
                                    NPC.rotation = 0;
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
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(12, -NPC.velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<UkkoLightning>(), NPCHelper.HostileProjDamage((int)(NPC.damage * 0.9f)), 0, Main.myPlayer, ai.ToRotation(), ai2);
                                    }
                                    if (AITimer >= 50 && dashCounter < 2)
                                    {
                                        dashCounter++;
                                        MoveVector3 = ChargePos();
                                        AITimer = 0;
                                    }
                                    if (AITimer >= 20 && dashCounter >= 2)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Jyrina>(), NPC.damage, NPC.velocity, NPC.spriteDirection == 1 ? 0 : 2);
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
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoGust>(), NPC.damage, RedeHelper.Spread(10));
                                    }
                                }
                                if (AITimer == 70)
                                    NPC.ai[3] = 0;
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
                                for (int i = 0; i < 3; i++)
                                    DustHelper.DrawParticleElectricity<LightningParticle>(HammerPos, HammerPos + RedeHelper.PolarVector(90, RedeHelper.RandomRotation()), 1, 20, 0.1f);

                                NPC.ai[3] = 1;
                                Vector2 ai = RedeHelper.PolarVector(15, RedeHelper.RandomRotation());
                                float ai2 = Main.rand.Next(100);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), HammerPos, RedeHelper.PolarVector(15, RedeHelper.RandomRotation()), ModContent.ProjectileType<UkkoLightning>(), NPCHelper.HostileProjDamage((int)(NPC.damage * 0.9f)), 0, Main.myPlayer, ai.ToRotation(), ai2);
                            }
                            if (AITimer >= 50)
                            {
                                NPC.ai[3] = 0;
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
                                    NPC.Shoot(player.Center, ModContent.ProjectileType<StormSummonerPro>(), (int)(NPC.damage * 0.92f), Vector2.Zero, Main.rand.Next(4));
                                }
                                if (AITimer == 20)
                                    NPC.ai[3] = 3;
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
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, 0f, 0f, 100, default, 1.5f);
                                    dust.velocity = -NPC.DirectionTo(dust.position) * 2f;
                                }
                                if (AITimer == 42)
                                    NPC.ai[3] = 3;
                                if (AITimer == 60)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(8, (player.Center - NPC.Center).ToRotation()), CustomSounds.Zap2);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(8, (player.Center - NPC.Center).ToRotation()), CustomSounds.Zap2, 1);
                                }
                                if (NPC.life < (int)(NPC.lifeMax * 0.3f))
                                {
                                    if (AITimer == 72)
                                    {
                                        swipeFrame = 0;
                                        NPC.ai[3] = 3;
                                    }
                                    if (AITimer == 90)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(6, (player.Center - NPC.Center).ToRotation()), CustomSounds.Zap2);
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(6, (player.Center - NPC.Center).ToRotation()), CustomSounds.Zap2, 1);
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

                                        NPC.Shoot(player.Center + new Vector2(A, B), ModContent.ProjectileType<UkkoBlizzard>(), (int)(NPC.damage * 0.75f), new Vector2(2, 4));
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
                                NPC.Shoot(player.Center - new Vector2(0, 200), ModContent.ProjectileType<UkkoRainCloud>(), 0, Vector2.Zero);
                            }
                            if (AITimer == 40)
                                NPC.ai[3] = 0;
                            if (AITimer >= 60)
                            {
                                NPC.ai[3] = 0;
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
                                    NPC.ai[3] = 1;
                                    SoundEngine.PlaySound(SoundID.Thunder);
                                }
                                if (AITimer < 52)
                                {
                                    for (int k = 0; k < 2; k++)
                                    {
                                        Vector2 vector;
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        vector.X = (float)(Math.Sin(angle) * 90);
                                        vector.Y = (float)(Math.Cos(angle) * 90);
                                        Dust dust2 = Main.dust[Dust.NewDust(HammerPos + vector, 2, 2, DustID.Sandnado, Scale: 2)];
                                        dust2.noGravity = true;
                                        dust2.velocity = dust2.position.DirectionTo(HammerPos) * 9f;
                                    }
                                }
                                if (AITimer == 52)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 10;
                                    for (int i = -16; i <= 16; i++)
                                    {
                                        NPC.Shoot(HammerPos, ModContent.ProjectileType<UkkoThunderwave>(), (int)(NPC.damage * 0.8f), 10 * Vector2.UnitX.RotatedBy(Math.PI / 16 * i));
                                    }
                                }
                                if (AITimer == 58)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Zap2, NPC.position);
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 20;
                                    for (int i = -8; i <= 8; i++)
                                    {
                                        NPC.Shoot(HammerPos, ModContent.ProjectileType<UkkoThunderwave>(), (int)(NPC.damage * 0.8f), 8 * Vector2.UnitX.RotatedBy(Math.PI / 8 * i));
                                    }
                                }
                                if (AITimer == 70)
                                    NPC.ai[3] = 0;
                                if (AITimer >= 160)
                                {
                                    NPC.ai[3] = 0;
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

                        #region TA: Bubbles
                        case 15:
                            if (akkaActive)
                            {
                                if (AITimer++ == 102)
                                {
                                    NPC.ai[3] = 1;
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);
                                }
                                if (AITimer < 152 && AITimer >= 102)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado);
                                        dust.velocity = -NPC.DirectionTo(dust.position);
                                    }
                                }
                                if (AITimer == 134)
                                    NPC.ai[3] = 3;
                                if (AITimer == 152)
                                {
                                    for (int i = 0; i < Main.rand.Next(9, 14); i++)
                                        DustHelper.DrawParticleElectricity<LightningParticle>(NPC.Center, NPC.Center + RedeHelper.PolarVector(Main.rand.Next(160, 210), RedeHelper.RandomRotation()), 3, 40, 0.1f, 1);

                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 30;
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<UkkoElectricBlast>(), 0, Vector2.Zero, CustomSounds.Thunderstrike, NPC.whoAmI);
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
                                NPC.velocity *= 0;
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region TA: Earth Barrier
                        case 16:
                            if (akkaActive)
                            {
                                NPC.MoveToVector2(EarthProtectPos, 30);
                                if (AITimer++ == 6)
                                {
                                    if (player.ZoneHallow)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<EarthBarrier>(), 0, Vector2.Zero);
                                    else if (player.ZoneCorrupt)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<EarthBarrier>(), 0, Vector2.Zero, 1);
                                    else if (player.ZoneCrimson)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<EarthBarrier>(), 0, Vector2.Zero, 3);
                                    else if (player.ZoneDesert)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<EarthBarrier>(), 0, Vector2.Zero, 4);
                                    else
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<EarthBarrier>(), 0, Vector2.Zero, 2);
                                }
                                if (AITimer < 120)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sandnado, Scale: 0.7f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);
                                }
                                if (AITimer > 120 && AITimer % 20 == 0)
                                {
                                    int speed = Main.rand.Next(4, 8);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(speed, (player.Center - NPC.Center).ToRotation()), CustomSounds.Zap2);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<DualcastBall>(), (int)(NPC.damage * 0.92f), RedeHelper.PolarVector(speed, (player.Center - NPC.Center).ToRotation()), CustomSounds.Zap2, 1);
                                }
                                if (AITimer == 295)
                                {
                                    Main.npc[akkaID].ai[2] = 100;
                                    Main.npc[akkaID].netUpdate = true;
                                }
                                if (AITimer >= 300)
                                {
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                NPC.velocity *= 0;
                                AttackID = Main.rand.Next(15);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.AkkaSummon:
                    if (RedeBossDowned.ADDDeath == 2)
                    {
                        if (!Main.dedServ)
                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossUkkoAkka");

                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 80, (int)ArenaWorld.arenaTopLeft.Y - 100, ModContent.NPCType<Akka>(), 0, 0, 0, NPC.whoAmI);
                        akkaArrive = true;
                        AITimer = 0;
                        AttackID = 0;
                        AIState = ActionState.ResetVars;
                        NPC.netUpdate = true;
                        break;
                    }
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
                                ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
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
                            ScreenPlayer.CutsceneLock(player, MoveVector3, ScreenPlayer.CutscenePriority.High, 0, 0, 0);
                            if (AITimer++ <= 30)
                                NPC.velocity.Y += 0.4f;
                            else
                            {
                                NPC.velocity.Y -= AITimer >= 70 ? 10f : 0.4f;
                            }
                            if (AITimer == 70)
                            {
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.ElectricSlash2, NPC.position);
                            }

                            if (NPC.Center.Y <= ArenaWorld.arenaTopLeft.Y - 100)
                            {
                                if (!Main.dedServ)
                                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossUkkoAkka");

                                NPC.velocity *= 0;
                                NPC.alpha = 255;
                                AITimer = 0;
                                AttackID = 2;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            if (AITimer == 120)
                            {
                                SoundEngine.PlaySound(SoundID.Item165 with { Pitch = -0.1f });
                            }
                            if (AITimer++ >= 240)
                            {
                                ScreenPlayer.CutsceneLock(player, MoveVector3, ScreenPlayer.CutscenePriority.High, 0, 0, 0);
                                NPC.alpha = 0;
                                if (NPC.DistanceSQ(MoveVector3) < 20 * 20)
                                {
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
                            ScreenPlayer.CutsceneLock(player, MoveVector3, ScreenPlayer.CutscenePriority.High, 0, 0, 0);
                            NPC.velocity *= 0.9f;
                            if (AITimer == 180)
                                EmoteBubble.NewBubble(0, new WorldUIAnchor(NPC), 50);

                            if (AITimer++ >= 300)
                            {
                                NPC.dontTakeDamage = false;
                                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                                akkaArrive = true;
                                AITimer = 0;
                                AttackID = 0;
                                AIState = ActionState.ResetVars;
                                NPC.netUpdate = true;
                                if (RedeBossDowned.ADDDeath < 2)
                                    RedeBossDowned.ADDDeath = 2;
                            }
                            break;
                    }
                    break;
            }
        }
        public override void PostAI()
        {
            CustomFrames();
        }
        private void CustomFrames()
        {
            FlareTimer++;
            switch (NPC.ai[3])
            {
                case 1:
                    FlareTimer = 10;
                    break;
                case 2:
                    frameCounters++;
                    if (frameCounters % 3 == 0)
                    {
                        chariotFrame++;
                        jyrinaFrame++;
                    }
                    if (chariotFrame >= 3)
                        chariotFrame = 0;
                    if (jyrinaFrame >= 9)
                    {
                        jyrinaFrame = 0;
                        frameCounters = 0;
                    }
                    break;
                case 3:
                    frameCounters++;
                    if (frameCounters > 5)
                    {
                        swipeFrame++;
                        frameCounters = 0;
                    }
                    if (swipeFrame >= 6)
                    {
                        frameCounters = 0;
                        swipeFrame = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if ((NPC.velocity.X < -3 && NPC.spriteDirection == -1) || (NPC.velocity.X > 3 && NPC.spriteDirection == 1))
                NPC.frame.Y = frameHeight * 3;
            else if ((NPC.velocity.X < -3 && NPC.spriteDirection == 1) || (NPC.velocity.X > 3 && NPC.spriteDirection == -1))
                NPC.frame.Y = frameHeight * 4;
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > frameHeight * 2)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y = 0;
                    }
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return AttackID == 7;
        }
        public Vector2 Pos()
        {
            Vector2 Pos1 = new(player.Center.X + (Main.rand.Next(300, 500) * NPC.RightOfDir(player)), player.Center.Y + Main.rand.Next(-400, 200));
            return Pos1;
        }
        public Vector2 ChargePos()
        {
            Vector2 ChargePos1 = new(player.Center.X + (1400 * NPC.RightOfDir(player)), player.Center.Y + Main.rand.Next(-80, 80));
            return ChargePos1;
        }
        private void Target()
        {
            player = Main.player[NPC.target];
        }
        private void Rain()
        {
            int akkaID = NPC.FindFirstNPC(ModContent.NPCType<Akka>());
            if (Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f || Main.player[NPC.target].dead)
            {
                if (StopRain == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.StopRain();
                        Main.SyncRain();
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                    StopRain = 1;
                }
            }
            if (RunOnce == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.StopRain();
                    Main.SyncRain();
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.WorldData);
                }
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
        private float drawTimer;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightGoldenrodYellow, Color.LightYellow * 0.7f, Color.LightGoldenrodYellow);

            Vector2 n = new(14 * NPC.spriteDirection, 0);
            switch (NPC.ai[3])
            {
                case 0:
                    if (!NPC.IsABestiaryIconDummy)
                    {
                        spriteBatch.End();
                        spriteBatch.BeginAdditive(true);
                        GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                        for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                        {
                            Vector2 oldPos = NPC.oldPos[i];
                            spriteBatch.Draw(texture, oldPos - n + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                        }
                        spriteBatch.End();
                        spriteBatch.BeginDefault();
                    }
                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, NPC.Center - n - screenPos, NPC.frame, Color.LightYellow * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects);

                    spriteBatch.Draw(texture, NPC.Center - n - screenPos, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                    spriteBatch.Draw(glowMask.Value, NPC.Center - n - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                    break;
                case 1:
                    Vector2 wandDrawCenter = new(13 * NPC.spriteDirection, -28);
                    spriteBatch.End();
                    spriteBatch.BeginAdditive(true);
                    GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(wandAni.Value, oldPos + new Vector2(13 * NPC.spriteDirection, -28) - n + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), null, NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(wandAni.Value.Width / 2f, wandAni.Value.Height / 2f), NPC.scale, effects, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.BeginDefault();

                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, wandAni.Value, ref drawTimer, NPC.Center - n + wandDrawCenter - screenPos, null, Color.LightYellow * NPC.Opacity, NPC.rotation, new Vector2(wandAni.Value.Width / 2f, wandAni.Value.Height / 2f), NPC.scale, effects);

                    spriteBatch.Draw(wandAni.Value, NPC.Center - n + wandDrawCenter - screenPos, null, drawColor * NPC.Opacity, NPC.rotation, new Vector2(wandAni.Value.Width / 2f, wandAni.Value.Height / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(wandGlow.Value, NPC.Center - n + wandDrawCenter - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(wandAni.Value.Width / 2f, wandAni.Value.Height / 2f), NPC.scale, effects, 0f);
                    break;
                case 2:
                    int chariotHeight = chariotAni.Value.Height / 3;
                    int chariotY = chariotHeight * chariotFrame;

                    int jyrinaHeight = jyrina.Value.Height / 9;
                    int jyrinaY = jyrinaHeight * jyrinaFrame;
                    spriteBatch.End();
                    spriteBatch.BeginAdditive(true);
                    GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(chariotAni.Value, oldPos - n + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, chariotY, chariotAni.Value.Width, chariotHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(chariotAni.Value.Width / 2f, chariotHeight / 2f), NPC.scale, effects, 0);
                        spriteBatch.Draw(jyrina.Value, oldPos - n + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, jyrinaY, jyrina.Value.Width, jyrinaHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(jyrina.Value.Width / 2f, jyrinaHeight / 2f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.BeginDefault();

                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, chariotAni.Value, ref drawTimer, NPC.Center - n - screenPos, new Rectangle?(new Rectangle(0, chariotY, chariotAni.Value.Width, chariotHeight)), Color.LightYellow * NPC.Opacity, NPC.rotation, new Vector2(chariotAni.Value.Width / 2f, chariotHeight / 2f), NPC.scale, effects);

                    spriteBatch.Draw(chariotAni.Value, NPC.Center - n - screenPos, new Rectangle?(new Rectangle(0, chariotY, chariotAni.Value.Width, chariotHeight)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(chariotAni.Value.Width / 2f, chariotHeight / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(chariotGlow.Value, NPC.Center - n - screenPos, new Rectangle?(new Rectangle(0, chariotY, chariotAni.Value.Width, chariotHeight)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(chariotAni.Value.Width / 2f, chariotHeight / 2f), NPC.scale, effects, 0f);

                    spriteBatch.Draw(jyrina.Value, NPC.Center - n - screenPos, new Rectangle?(new Rectangle(0, jyrinaY, jyrina.Value.Width, jyrinaHeight)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(jyrina.Value.Width / 2f, jyrinaHeight / 2f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                    spriteBatch.Draw(jyrinaGlow.Value, NPC.Center - n - screenPos, new Rectangle?(new Rectangle(0, jyrinaY, jyrina.Value.Width, jyrinaHeight)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(jyrina.Value.Width / 2f, jyrinaHeight / 2f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                    break;
                case 3:
                    int swipeHeight = swipeAni.Value.Height / 6;
                    int swipeY = swipeHeight * swipeFrame;
                    Vector2 swipeDrawCenter = new(14 * NPC.spriteDirection, -9);

                    int swipeSHeight = swipeSlash.Value.Height / 6;
                    int swipeSY = swipeSHeight * swipeFrame;
                    Vector2 swipeSDrawCenter = new(41 * NPC.spriteDirection, -41);
                    spriteBatch.End();
                    spriteBatch.BeginAdditive(true);
                    GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(swipeAni.Value, oldPos + new Vector2(14 * NPC.spriteDirection, -9) - n + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, swipeY, swipeAni.Value.Width, swipeHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(swipeAni.Value.Width / 2f, swipeHeight / 2f), NPC.scale, effects, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.BeginDefault();

                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, swipeAni.Value, ref drawTimer, NPC.Center - n + swipeDrawCenter - screenPos, new Rectangle?(new Rectangle(0, swipeY, swipeAni.Value.Width, swipeHeight)), Color.LightYellow * NPC.Opacity, NPC.rotation, new Vector2(swipeAni.Value.Width / 2f, swipeHeight / 2f), NPC.scale, effects);

                    spriteBatch.Draw(swipeAni.Value, NPC.Center - n + swipeDrawCenter - screenPos, new Rectangle?(new Rectangle(0, swipeY, swipeAni.Value.Width, swipeHeight)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(swipeAni.Value.Width / 2f, swipeHeight / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(swipeGlow.Value, swipeDrawCenter - screenPos, new Rectangle?(new Rectangle(0, swipeY, swipeAni.Value.Width, swipeHeight)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(swipeAni.Value.Width / 2f, swipeHeight / 2f), NPC.scale, effects, 0f);

                    spriteBatch.Draw(swipeSlash.Value, NPC.Center - n + swipeSDrawCenter - screenPos, new Rectangle?(new Rectangle(0, swipeSY, swipeSlash.Value.Width, swipeSHeight)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(swipeSlash.Value.Width / 2f, swipeSHeight / 2f), NPC.scale, effects, 0f);
                    break;
            }
            return false;
        }
        private float FlareTimer = 60;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - screenPos + new Vector2(4 * NPC.spriteDirection, -53);
            RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position, Color.LightYellow, NPC.rotation);
        }
        public override void HitEffect(NPC.HitInfo hit)
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
