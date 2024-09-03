using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Textures;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    [AutoloadBossHead]
    public class Thorn : ModNPC
    {
        private static Asset<Texture2D> ArmAni;
        private static Asset<Texture2D> DeathAni;
        public static int secondStageHeadSlot = -1;

        public override void BossHeadSlot(ref int index)
        {
            int slot = secondStageHeadSlot;
            if (PhaseTwo && slot != -1)
            {
                index = slot;
            }
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;
            ArmAni = ModContent.Request<Texture2D>(Texture + "_Arm");
            DeathAni = ModContent.Request<Texture2D>(Texture + "_Death");

            string texture = "Redemption/NPCs/Bosses/Thorn/Thorn_Head_Boss2";
            secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1);
        }

        public enum ActionState
        {
            PlayerKilled = -1,
            Begin,
            Idle,
            Attacks,
            TeleportStart,
            TeleportEnd,
            PhaseTransition,
            Death
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
            // DisplayName.SetDefault("Thorn, Bane of the Forest");
            Main.npcFrameCount[NPC.type] = 14;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.BloodButcherer] = false;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0, 40),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 3120;
            NPC.defense = 6;
            NPC.damage = 21;
            NPC.width = 78;
            NPC.height = 74;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.BossBar = ModContent.GetInstance<NoHeadHealthBar>();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossThorn");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
            NPC.damage = (int)(NPC.damage * 0.9f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Thorn"))
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ThornBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThornTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ThornRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<BouquetOfThorns>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ThornMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(2, 1, ModContent.ItemType<AldersStaff>(), ModContent.ItemType<CursedGrassBlade>(), ModContent.ItemType<RootTendril>(), ModContent.ItemType<CursedThornBow>()));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedThorn)
            {
                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.ThornDowned");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(50, 255, 130));
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), new Color(50, 255, 130));

                RedeWorld.alignment += 2;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "+2", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.HeartOfThorns2"), 300, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedThorn, -1);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.JungleGrass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.Write(PhaseTwo);

            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ID = reader.ReadInt32();
            PhaseTwo = reader.ReadBoolean();

            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public List<int> AttackList = new() { 0, 2, 3, 4, 5, 6, 7 };
        public List<int> CopyList = null;
        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }
        void AttackChoice()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                if (ID is 0 && NPC.CountNPCS(ModContent.NPCType<BrambleTrap>()) > 20)
                    continue;
                if (ID is 3 && NPC.life >= (int)(NPC.lifeMax * 0.9f))
                    continue;
                if (ID is 5 && (!PhaseTwo || !Main.expertMode))
                    continue;

                attempts++;
            }
        }
        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.xMas)
                typeName = Language.GetTextValue("Mods.Redemption.NPCs.Thorn.XmasName");
        }
        public bool PhaseTwo;
        public Vector2 lastVector;
        public bool finalSubphaseSlam;
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (AIState is not ActionState.PlayerKilled and not ActionState.Death && (!player.active || player.dead))
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.velocity.X = 0;
                    AIState = ActionState.PlayerKilled;
                    AITimer = 0;
                    TimerRand = 0;
                    NPC.netUpdate = true;
                }
            }

            if (AIState != ActionState.TeleportStart && AIState != ActionState.TeleportEnd && AIState != ActionState.Death)
                NPC.LookAtEntity(player);

            if (AIState is ActionState.TeleportStart or ActionState.TeleportEnd)
                NPC.netOffset *= 0f;

            Vector2 HeartOrigin = new(NPC.Center.X + (13 * NPC.spriteDirection), NPC.Center.Y - 17);
            bool phase2Health = NPC.life < (int)(NPC.lifeMax * .66f);

            if (NPC.alpha < 100 && AIState is not ActionState.TeleportStart and not ActionState.TeleportEnd and not ActionState.Death)
            {
                foreach (Player pl in Main.ActivePlayers)
                {
                    if (pl.dead || !NPC.Hitbox.Intersects(pl.Hitbox))
                        continue;

                    pl.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 10);
                }
            }
            switch (AIState)
            {
                case ActionState.PlayerKilled:
                    if (aniType != STAFF_SLAM && NPC.alpha < 255)
                        SwapAnimation(STAFF_SLAM);
                    if (AITimer++ == 30 && NPC.alpha < 255)
                    {
                        magicOpacity = 1;
                        SoundEngine.PlaySound(SoundID.Zombie103, NPC.position);
                        AITimer = 0;
                        NPC.frame.Y = 10;
                        NPC.frameCounter = 0;
                        NPC.velocity.X = 0;

                        for (int i = 0; i < 30; i++)
                        {
                            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                            {
                                PositionInWorld = RedeHelper.RandAreaInEntity(NPC),
                                MovementVector = new Vector2(0, -Main.rand.Next(0, 2))
                            });
                        }
                        RedeDraw.SpawnExplosion(NPC.Center, Color.White, scale: 1, shakeAmount: 0, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value, rot: RedeHelper.RandomRotation());
                        RedeDraw.SpawnExplosion(NPC.Center, Color.White * .5f, scale: 1.5f, shakeAmount: 0, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value, rot: RedeHelper.RandomRotation());
                        NPC.alpha = 255;
                    }
                    magicOpacity -= .05f;

                    if (AITimer >= 40 && NPC.alpha >= 255)
                        NPC.active = false;
                    return;
                case ActionState.Begin:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Thorn.Name"), 60, 90, 0.8f, 0, Color.LawnGreen, Language.GetTextValue("Mods.Redemption.TitleCard.Thorn.Modifier"));
                    AIState = ActionState.TeleportStart;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Idle:
                    if (NPC.life < NPC.lifeMax / 4)
                    {
                        if (AITimer == 0 && !finalSubphaseSlam)
                        {
                            SwapAnimation(BOTH_SLAM);
                            finalSubphaseSlam = true;
                        }

                        redEyeOpacity -= .01f;
                        redEyeOpacity = MathHelper.Max(redEyeOpacity, -1);
                    }
                    if (++AITimer > 60)
                    {
                        AITimer = 0;
                        if (!PhaseTwo && phase2Health)
                        {
                            TimerRand = 0;
                            AIState = ActionState.PhaseTransition;
                            NPC.netUpdate = true;
                            break;
                        }
                        AttackChoice();
                        AIState = ActionState.Attacks;
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Thorns
                        case 0:
                            if (AITimer++ == 0)
                            {
                                TimerRand = 0;
                                SwapAnimation(HAND_SLAM);
                            }
                            int delay = 22;
                            int amount = 15;
                            if (PhaseTwo)
                            {
                                delay -= 5;
                                amount += 3;
                            }
                            if (NPC.life < NPC.lifeMax * .75f)
                            {
                                delay -= 2;
                                amount += 2;
                            }
                            if (AITimer >= 30 && AITimer % delay == 0 && TimerRand < amount)
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    Vector2 origin = NPC.Center;
                                    origin.X += TimerRand++ * Main.rand.Next(20, 30) * i;
                                    int numtries = 0;
                                    int x = (int)(origin.X / 16);
                                    int y = (int)(origin.Y / 16);
                                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                                    {
                                        y++;
                                        origin.Y = y * 16;
                                    }
                                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                                    {
                                        numtries++;
                                        y--;
                                        origin.Y = y * 16;
                                    }
                                    if (numtries >= 20)
                                        break;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int npc = NPC.NewNPC(NPC.GetSource_FromAI(), (int)origin.X, (int)origin.Y + 14, ModContent.NPCType<BrambleTrap>(), NPC.FindFirstNPC(ModContent.NPCType<BrambleTrap>() + 1));
                                        SoundEngine.PlaySound(SoundID.Item17 with { Volume = 0.6f, Pitch = -0.4f }, Main.npc[npc].Center);
                                    }
                                }
                            }

                            if (AITimer >= 280 || TimerRand >= amount)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Splinters
                        case 2:
                            if (++AITimer == 10)
                            {
                                SwapAnimation(STAFF_SLAM);
                                for (int k = 0; k < 40; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(HeartOrigin + vector, 2, 2, DustID.JungleGrass, 0f, 0f, 100, default, 3f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -HeartOrigin.DirectionTo(dust2.position) * 10f;
                                }
                            }

                            delay = 10;
                            if (PhaseTwo)
                                delay -= 2;
                            if (NPC.life < NPC.lifeMax * .75f)
                                delay -= 2;
                            Vector2 staffPos = NPC.Center + new Vector2(43 * NPC.spriteDirection, -35);
                            if (AITimer >= 40 && AITimer % delay == 0 && AITimer < 70)
                            {
                                SoundEngine.PlaySound(SoundID.Item17, NPC.position);
                                NPC.Shoot(staffPos, ModContent.ProjectileType<Thorn_SplinterBall>(), NPC.damage, new Vector2(Main.rand.Next(10, 36) * NPC.spriteDirection, Main.rand.Next(-20, 2)), ai2: NPC.whoAmI);
                            }

                            if (AITimer >= 220)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Life Drain
                        case 3:
                            if (++AITimer < 60)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(HeartOrigin + vector, 2, 2, DustID.LifeDrain, 0f, 0f, 100, default, 1f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = dust2.position.DirectionTo(HeartOrigin) * 10f;
                                }
                            }

                            if (AITimer is 60 or 120 or 180)
                                lastVector = player.Center;
                            if (AITimer >= 60)
                            {
                                if (PhaseTwo)
                                {
                                    bool otherBursts = AITimer >= 120 && AITimer < 140;
                                    if (NPC.life < NPC.lifeMax / 4)
                                        otherBursts |= AITimer >= 180 && AITimer < 200;
                                    if (AITimer <= 80 || otherBursts)
                                    {
                                        if (Main.rand.NextBool(2))
                                            NPC.Shoot(HeartOrigin, ModContent.ProjectileType<LeechingThornSeed>(), NPC.damage, RedeHelper.PolarVector(11, (lastVector - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), SoundID.Item17, ai2: NPC.whoAmI);
                                    }
                                }
                                else
                                {
                                    if (AITimer <= 90)
                                    {
                                        if (Main.rand.NextBool(3))
                                            NPC.Shoot(HeartOrigin, ModContent.ProjectileType<LeechingThornSeed>(), NPC.damage, RedeHelper.PolarVector(11, (lastVector - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), SoundID.Item17, ai2: NPC.whoAmI);
                                    }
                                }
                            }

                            if (PhaseTwo ? AITimer >= 230 : AITimer >= 180)
                            {
                                TimerRand = 0;
                                AITimer = 0;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Cleave
                        case 4:
                            if (AITimer++ == 0)
                                SwapAnimation(HAND_SLAM);

                            if (aniType is HAND_SLAM && NPC.frame.Y is 8)
                                NPC.frameCounter = 0;

                            Vector2 handPos = NPC.Center + new Vector2(-25 * NPC.spriteDirection, 11);

                            delay = 50;
                            if (PhaseTwo)
                                delay -= 10;
                            if (NPC.life < NPC.lifeMax / 4)
                                delay -= 2;
                            if (NPC.life < (int)(NPC.lifeMax * .85f))
                                delay -= 2;
                            if (NPC.life < (int)(NPC.lifeMax * .65f))
                                delay -= 2;
                            if (AITimer >= 50 && AITimer % delay == 0 && AITimer <= 200)
                            {
                                for (int i = 0; i < 8; ++i)
                                {
                                    Dust dust = Dust.NewDustDirect(handPos - new Vector2(6), 12, 12, DustID.Sandnado, 0.0f, 0.0f, 100, new Color(), 2f);
                                    dust.velocity = -player.DirectionTo(dust.position) * 10;
                                    dust.noGravity = true;
                                }
                                int steps = (int)NPC.Distance(player.Center) / 8;
                                for (int i = 0; i < steps; i++)
                                {
                                    if (Main.rand.NextBool(2))
                                    {
                                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(handPos, player.Center, (float)i / steps), 2, 2, DustID.Sandnado, Scale: 2);
                                        dust.velocity = -player.DirectionTo(dust.position) * 2;
                                        dust.noGravity = true;
                                    }
                                }
                                for (int k = 0; k < 16; k++)
                                {
                                    Vector2 vector;
                                    double angle = k * (Math.PI * 2 / 16);
                                    vector.X = (float)(Math.Sin(angle) * 60);
                                    vector.Y = (float)(Math.Cos(angle) * 60);
                                    Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector - new Vector2(4, 4), 1, 1, DustID.Sandnado, 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -player.DirectionTo(dust2.position) * 10;
                                }
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Saint3 with { Volume = .6f }, player.position);
                                NPC.Shoot(player.Center, ModContent.ProjectileType<Thorn_SlashFlash>(), NPC.damage, RedeHelper.PolarVector(PhaseTwo ? 28 : 22, RedeHelper.RandomRotation()), ai2: NPC.whoAmI);
                            }

                            if (PhaseTwo ? AITimer >= 230 : AITimer >= 250)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    AIState = Main.rand.NextBool(4) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Claw Dance
                        case 5:
                            switch (TimerRand)
                            {
                                default:
                                    if (AITimer++ == 0)
                                    {
                                        SoundEngine.PlaySound(CustomSounds.SpookyNoise with { Pitch = .3f }, NPC.position);
                                        redEyeOpacity = 1;
                                    }

                                    if (Main.netMode != NetmodeID.Server && AITimer < 60)
                                    {
                                        for (int k = 0; k < 2; k++)
                                        {
                                            Vector2 val = RedeHelper.RandAreaInEntity(NPC);
                                            int goreType = Main.rand.Next(1202, 1205);
                                            float s = 1f;
                                            if (goreType is 1202)
                                                s = 2f;
                                            float scale = s + Main.rand.NextFloat() * 1.6f;
                                            Vector2 position5 = val + new Vector2(0f, -18f);
                                            Vector2 velocity = Main.rand.NextVector2Circular(0.7f, 0.25f) * 0.4f + Main.rand.NextVector2CircularEdge(1f, 0.4f) * 0.1f;
                                            velocity *= 2;
                                            velocity.Y = 0;
                                            Gore.NewGorePerfect(NPC.GetSource_FromAI(), position5, velocity, goreType, scale);
                                        }
                                    }
                                    if (AITimer > 60)
                                        NPC.alpha += 5;
                                    if (NPC.alpha >= 255)
                                    {
                                        redEyeOpacity = 0;
                                        NPC.dontTakeDamage = true;
                                        NPC.alpha = 255;
                                        AITimer = 0;
                                        TimerRand = 1;
                                        NPC.netUpdate = true;
                                    }
                                    break;
                                case 1:
                                    int endTime = NPC.life < NPC.lifeMax / 4 ? 280 : 220;
                                    if (AITimer++ >= 60 && AITimer % 50 == 0)
                                    {
                                        for (int k = 0; k < 16; k++)
                                        {
                                            Vector2 vector;
                                            double angle = k * (Math.PI * 2 / 16);
                                            vector.X = (float)(Math.Sin(angle) * 60);
                                            vector.Y = (float)(Math.Cos(angle) * 60);
                                            Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector - new Vector2(4, 4), 1, 1, DustID.Sandnado, 0f, 0f, 100, default, 2f)];
                                            dust2.noGravity = true;
                                            dust2.velocity = -player.DirectionTo(dust2.position) * 10;
                                        }
                                        if (!Main.dedServ)
                                            SoundEngine.PlaySound(CustomSounds.Saint3 with { Volume = .6f }, player.position);
                                        float randRot = RedeHelper.RandomRotation();
                                        amount = 6;
                                        if (NPC.life < NPC.lifeMax / 4)
                                            amount = 7;
                                        if (Main.getGoodWorld)
                                            amount += 1;

                                        for (int i = 0; i < amount; i++)
                                        {
                                            Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, RedeHelper.PolarVector(34, (MathHelper.ToRadians(45) * i) + randRot), ModContent.ProjectileType<Thorn_SlashFlash>(), NPCHelper.HostileProjDamage(NPC.damage), 3, Main.myPlayer, -2, 0, NPC.whoAmI);
                                            (p.ModProjectile as Thorn_SlashFlash).Dance = true;
                                        }
                                        NPC.netUpdate = true;
                                    }
                                    if (AITimer > 220)
                                    {
                                        AITimer = 0;
                                        TimerRand = 2;
                                        NPC.netUpdate = true;
                                    }
                                    break;
                                case 2:
                                    delay = NPC.life < NPC.lifeMax / 4 ? 8 : 10;
                                    if (AITimer++ > 90 && AITimer % delay == 0 && AITimer < 180)
                                    {
                                        for (int k = 0; k < 16; k++)
                                        {
                                            Vector2 vector;
                                            double angle = k * (Math.PI * 2 / 16);
                                            vector.X = (float)(Math.Sin(angle) * 60);
                                            vector.Y = (float)(Math.Cos(angle) * 60);
                                            Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector - new Vector2(4, 4), 1, 1, DustID.Sandnado, 0f, 0f, 100, default, 2f)];
                                            dust2.noGravity = true;
                                            dust2.velocity = -player.DirectionTo(dust2.position) * 10;
                                        }
                                        if (!Main.dedServ)
                                            SoundEngine.PlaySound(CustomSounds.Saint3 with { Volume = .6f }, player.position);
                                        if (NPC.life < NPC.lifeMax / 4)
                                        {
                                            NPC.Shoot(player.Center, ModContent.ProjectileType<Thorn_SlashFlash>(), NPC.damage, new Vector2(Main.rand.NextFloat(-12, 13), Main.rand.NextFloat(22, 32) * (AITimer % 20 == 0 ? 1 : -1)), 0, Main.rand.NextBool() ? 1 : -1, ai2: NPC.whoAmI);
                                        }
                                        NPC.Shoot(player.Center, ModContent.ProjectileType<Thorn_SlashFlash>(), NPC.damage, new Vector2(Main.rand.NextFloat(-4, 5), Main.rand.NextFloat(22, 32) * (AITimer % 20 == 0 ? -1 : 1)), 0, Main.rand.NextBool() ? 1 : -1, ai2: NPC.whoAmI);
                                    }
                                    if (AITimer >= 240)
                                    {
                                        NPC.dontTakeDamage = false;
                                        TimerRand = 0;
                                        AITimer = 0;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            AIState = ActionState.TeleportStart;
                                            NPC.netUpdate = true;
                                        }
                                    }
                                    break;
                            }
                            break;
                        #endregion

                        #region Arcane Bolts
                        case 6:
                            if (++AITimer == 10)
                                SwapAnimation(STAFF_SLAM);

                            staffPos = NPC.Center + new Vector2(43 * NPC.spriteDirection, -35);
                            if (!PhaseTwo)
                            {
                                if (AITimer is 40)
                                    NPC.Shoot(staffPos, ModContent.ProjectileType<Thorn_ArcaneBolt>(), NPC.damage, RedeHelper.PolarVector(6, -MathHelper.PiOver2 + .5f), CustomSounds.Saint9 with { Volume = .3f, Pitch = -.1f }, ai2: NPC.whoAmI);
                                if (AITimer is 45)
                                    NPC.Shoot(staffPos, ModContent.ProjectileType<Thorn_ArcaneBolt>(), NPC.damage, RedeHelper.PolarVector(7, -MathHelper.PiOver2), CustomSounds.Saint9 with { Volume = .3f }, ai2: NPC.whoAmI);
                                if (AITimer is 50)
                                    NPC.Shoot(staffPos, ModContent.ProjectileType<Thorn_ArcaneBolt>(), NPC.damage, RedeHelper.PolarVector(6, -MathHelper.PiOver2 - .5f), CustomSounds.Saint9 with { Volume = .3f, Pitch = .1f }, ai2: NPC.whoAmI);
                            }
                            else
                            {
                                if (AITimer >= 40 && AITimer % (Main.getGoodWorld ? 1 : 10) == 0 && AITimer < 100)
                                    NPC.Shoot(staffPos, ModContent.ProjectileType<Thorn_ArcaneBolt>(), NPC.damage, RedeHelper.PolarVector(Main.rand.Next(10, 16), -MathHelper.PiOver2 + Main.rand.NextFloat(-0.3f, .3f)), CustomSounds.Saint9 with { Volume = .3f }, ai2: NPC.whoAmI);
                            }
                            if (AITimer >= (PhaseTwo ? 180 : 120))
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Seed Rain
                        case 7:
                            if (AITimer++ == 0)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Magic5 with { Volume = .5f }, NPC.position);
                                SwapAnimation(WAVE_STAFF);
                            }

                            armRot -= ((float)Math.Sin(AITimer / 12) / 55) * NPC.spriteDirection;
                            if (AITimer % 30 == 0)
                            {
                                if (armFrame is 10)
                                    armFrame = 11;
                                else
                                    armFrame = 10;
                            }
                            staffPos = NPC.Center + new Vector2(43 * NPC.spriteDirection, -35);

                            delay = PhaseTwo ? 5 : 8;
                            if (AITimer >= 40 && AITimer % delay == 0 && AITimer < 100)
                                NPC.Shoot(staffPos, ModContent.ProjectileType<ThornSeed>(), NPC.damage, RedeHelper.PolarVector(Main.rand.Next(18, 29) / 4, -MathHelper.PiOver2 + Main.rand.NextFloat(-0.6f, .6f)), SoundID.Item42 with { Volume = .5f }, ai2: NPC.whoAmI);

                            if (AITimer >= 120)
                            {
                                SwapAnimation(IDLE);
                                armFrame = 0;
                                armRot = 0;
                                TimerRand = 0;
                                AITimer = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.PhaseTransition:
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        NPC.netUpdate = true;
                    }
                    ScreenPlayer.CutsceneLock(Main.LocalPlayer, NPC, ScreenPlayer.CutscenePriority.None, 1200, 2400, 1200);

                    if (AITimer >= 60)
                    {
                        if (AITimer < 180)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                Vector2 vector;
                                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                vector.X = (float)(Math.Sin(angle) * 150);
                                vector.Y = (float)(Math.Cos(angle) * 150);
                                Dust dust2 = Main.dust[Dust.NewDust(HeartOrigin + vector, 2, 2, DustID.MagicMirror, Scale: 2)];
                                dust2.noGravity = true;
                                dust2.velocity = dust2.position.DirectionTo(HeartOrigin) * 15f;
                            }
                        }

                        if (AITimer % 30 == 0 && AITimer < 180)
                        {
                            SoundEngine.PlaySound(SoundID.Item28 with { Pitch = TimerRand }, NPC.position);
                            TimerRand += 0.1f;
                        }
                        if (AITimer == 150)
                            SwapAnimation(BOTH_SLAM);
                        if (AITimer == 180)
                        {
                            magicOpacity = 1;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.WorldTree with { Volume = .7f }, NPC.position);
                            SoundEngine.PlaySound(SoundID.Item45, NPC.position);
                            for (int i = 0; i < 40; i++)
                            {
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.WallOfFleshGoatMountFlames, new ParticleOrchestraSettings
                                {
                                    PositionInWorld = RedeHelper.RandAreaInEntity(NPC),
                                    MovementVector = new Vector2(0, -Main.rand.Next(4, 9))
                                });
                            }
                            for (int i = 0; i < 10; i++)
                            {
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                                {
                                    PositionInWorld = RedeHelper.RandAreaInEntity(NPC),
                                    MovementVector = new Vector2(0, -Main.rand.Next(0, 2))
                                });
                            }
                            RedeDraw.SpawnExplosion(NPC.Center, Color.SkyBlue, scale: 1, shakeAmount: 10, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/BigFlare").Value);
                            RedeDraw.SpawnExplosion(NPC.Center, Color.Orange, scale: 2, shakeAmount: 10, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/BigFlare").Value);

                            PhaseTwo = true;

                            AdvancedPopupRequest fireText = new();
                            fireText.Text = "+Fire Resistance";
                            fireText.DurationInFrames = 120;
                            fireText.Color = Color.Orange;
                            fireText.Velocity = new Vector2(0, -3);
                            PopupText.NewText(fireText, NPC.Top);

                            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Fire] = .5f;

                            NPC.BecomeImmuneTo(BuffID.OnFire);
                            NPC.BecomeImmuneTo(BuffID.Frostburn);
                            NPC.BecomeImmuneTo(BuffID.ShadowFlame);
                            NPC.BecomeImmuneTo(ModContent.BuffType<DragonblazeDebuff>());
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.ClearImmuneToBuffs(out _);
                            NPC.netUpdate = true;
                        }
                    }
                    magicOpacity -= .01f;
                    if (AITimer is 250)
                        SwapAnimation(HOOD_DROP);
                    if (AITimer >= 280)
                    {
                        magicOpacity = 0;
                        PhaseTwo = true;
                        NPC.dontTakeDamage = false;
                        AITimer = 0;
                        TimerRand = 0;
                        ID = 5;
                        CopyList?.Remove(ID);

                        AIState = ActionState.Attacks;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.TeleportStart:
                    if (aniType != STAFF_SLAM)
                        SwapAnimation(STAFF_SLAM);
                    if (AITimer++ > 30)
                    {
                        magicOpacity = 1;
                        SoundEngine.PlaySound(SoundID.Zombie103, NPC.position);
                        AITimer = 0;
                        NPC.frame.Y = 10;
                        NPC.frameCounter = 0;
                        AIState = ActionState.TeleportEnd;
                        NPC.velocity.X = 0;

                        Vector2 pos = new(player.Center.X, BaseWorldGen.GetFirstTileFloor((int)player.Center.X / 16, (int)player.Center.Y / 16) * 16);
                        Vector2 newPos = NPC.FindGroundPlayer(15) - new Vector2(0, NPC.height);

                        bool landed = false;
                        int attempts2 = 0;
                        while (!landed && attempts2++ < 100)
                        {
                            newPos = NPCHelper.FindGroundVector(NPC, pos, 15) - new Vector2(0, NPC.height);
                            if (newPos.DistanceSQ(pos - new Vector2(0, NPC.height)) < 100 * 100)
                                continue;

                            landed = true;
                        }

                        for (int i = 0; i < 30; i++)
                        {
                            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                            {
                                PositionInWorld = RedeHelper.RandAreaInEntity(NPC),
                                MovementVector = new Vector2(0, -Main.rand.Next(0, 2))
                            });
                        }
                        RedeDraw.SpawnExplosion(NPC.Center, Color.White, scale: 1, shakeAmount: 0, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value, rot: RedeHelper.RandomRotation());
                        RedeDraw.SpawnExplosion(NPC.Center, Color.White * .5f, scale: 1.5f, shakeAmount: 0, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value, rot: RedeHelper.RandomRotation());
                        int steps = (int)NPC.Distance(newPos) / 16;
                        for (int i = 0; i < steps; i++)
                        {
                            if (Main.rand.NextBool(2))
                            {
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                                {
                                    PositionInWorld = Vector2.Lerp(RedeHelper.RandAreaInEntity(NPC), newPos + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), (float)i / steps),
                                    MovementVector = -newPos.DirectionTo(NPC.position) * 2
                                });
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.position = newPos;
                            int attempts = 0;
                            while (attempts++ < 1000 && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height + 2, true))
                                NPC.position.Y += 2;
                            NPC.netUpdate = true;
                        }

                        for (int i = 0; i < 30; i++)
                        {
                            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                            {
                                PositionInWorld = RedeHelper.RandAreaInEntity(NPC),
                                MovementVector = new Vector2(0, -Main.rand.Next(0, 2))
                            });
                        }
                        RedeDraw.SpawnExplosion(NPC.Center, Color.White, scale: 1, shakeAmount: 0, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value, rot: RedeHelper.RandomRotation());
                        RedeDraw.SpawnExplosion(NPC.Center, Color.White * .5f, scale: 1.5f, shakeAmount: 0, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value, rot: RedeHelper.RandomRotation());
                        SoundEngine.PlaySound(SoundID.Zombie103, NPC.position);
                    }
                    break;
                case ActionState.TeleportEnd:
                    NPC.LookAtEntity(player);
                    NPC.alpha = 0;
                    magicOpacity -= .02f;
                    if (magicOpacity <= 0)
                    {
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Death:
                    if (AITimer < 280)
                        ScreenPlayer.CutsceneLock(Main.LocalPlayer, NPC, ScreenPlayer.CutscenePriority.None, 1200, 2400, 1200);

                    armRot = 0;
                    if (AITimer++ == 0)
                    {
                        magicOpacity = 0;
                        NPC.alpha = 0;
                        NPC.dontTakeDamage = true;
                        NPC.netUpdate = true;
                    }
                    if (AITimer < 35 + 60 && NPC.frame.Y is 5)
                        NPC.frameCounter = 0;
                    if (AITimer < 70 + 35 + 120 && NPC.frame.Y is 10)
                        NPC.frameCounter = 0;
                    if (AITimer >= 300)
                        magicOpacity += .006f;
                    if (magicOpacity >= 1)
                    {
                        NPC.HitSound = null;
                        NPC.DeathSound = null;
                        SoundEngine.PlaySound(SoundID.Grass, NPC.position);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int i = 0; i < 60; i++)
                                Gore.NewGore(NPC.GetSource_FromThis(), RedeHelper.RandAreaInEntity(NPC) - new Vector2(5, 6), RedeHelper.SpreadUp(10), ModContent.Find<ModGore>("Redemption/DeadThornFX").Type, Main.rand.NextFloat(1, 1.5f));
                            for (int i = 0; i < 20; i++)
                                Gore.NewGore(NPC.GetSource_FromThis(), RedeHelper.RandAreaInEntity(NPC) - new Vector2(18, 18), RedeHelper.SpreadUp(2), ModContent.Find<ModGore>("Redemption/ThornGore2").Type);
                            for (int i = 0; i < 30; i++)
                                Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.Next(-6, 7) * NPC.spriteDirection, Main.rand.Next(-98, 1)) - new Vector2(5, 6), RedeHelper.SpreadUp(1), ModContent.Find<ModGore>("Redemption/DeadThornFX").Type, Main.rand.NextFloat(1, 1.5f));
                        }
                        //for (int i = 0; i < 20; i++)
                        //    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BrownMoss, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                        for (int i = 0; i < 30; i++)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, Scale: 2);
                            dust.alpha += Main.rand.Next(100);
                            dust.velocity *= 0.2f;
                            dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                            dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
                        }
                        //for (int i = 0; i < 20; i++)
                        //    Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.Next(-6, 7) * NPC.spriteDirection, Main.rand.Next(-98, 1)), DustID.BrownMoss, Scale: 2);

                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.StrikeInstantKill();
                    }
                    break;
            }
        }
        public override void PostAI()
        {
            AnimationFrames();
        }
        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
            {
                if (magicOpacity >= 1)
                    return true;
                NPC.life = 1;
                return false;
            }
            else
            {
                for (int i = 0; i < 40; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.JungleGrass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.position);
                NPC.life = 1;
                AITimer = 0;
                TimerRand = 0;
                NPC.frame.X = 0;
                SwapAnimation(DEATH);
                AIState = ActionState.Death;
                NPC.netUpdate = true;
                return false;
            }
        }
        int aniType;
        int armFrame;
        float armRot;
        const short IDLE = 0, HAND_SLAM = 1, STAFF_SLAM = 2, BOTH_SLAM = 3, HOOD_DROP = 4, WAVE_STAFF = 5, DEATH = 6;
        private void SwapAnimation(int type = 0, bool noResetFrame = false)
        {
            NPC.frameCounter = 0;
            if (!noResetFrame)
            {
                NPC.frame.Y = 0;
                armFrame = 0;
            }
            aniType = type;
        }

        private void AnimationFrames()
        {
            Player player = Main.player[NPC.target];
            switch (aniType)
            {
                default:
                    if (++NPC.frameCounter >= (NPC.life < NPC.lifeMax / 4 ? 5 : 10))
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        if (NPC.frame.Y > 5)
                            NPC.frame.Y = 0;
                        if (aniType != WAVE_STAFF)
                        {
                            armFrame++;
                            if (armFrame > 4)
                                armFrame = 0;
                        }
                    }
                    break;
                case HAND_SLAM:
                    if (NPC.frame.Y < 6)
                        NPC.frame.Y = 6;

                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        armFrame++;
                        if (armFrame > 4)
                            armFrame = 0;
                        if (NPC.frame.Y is 9)
                        {
                            PunchCameraModifier camPunch = new(NPC.Center, new Vector2(0f, 1f * RedeConfigClient.Instance.ShakeIntensity), 10f, 4f, 30, 1000f, "Thorn");
                            Main.instance.CameraModifiers.Add(camPunch);
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.position);
                        }
                        if (NPC.frame.Y > 10)
                            SwapAnimation(IDLE);
                    }
                    break;
                case STAFF_SLAM:
                    if (armFrame < 5)
                        armFrame = 5;

                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        armFrame++;
                        if (NPC.frame.Y > 5)
                            NPC.frame.Y = 0;
                        if (armFrame is 8)
                        {
                            SoundEngine.PlaySound(SoundID.Dig, NPC.position);
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Pitch = .2f }, NPC.position);

                            for (int i = 0; i < 9; i++)
                            {
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                                {
                                    PositionInWorld = NPC.Center + new Vector2(46 * NPC.spriteDirection, 34) + new Vector2(Main.rand.Next(-17, 18), Main.rand.Next(-5, 6)),
                                    MovementVector = new Vector2(0, -Main.rand.Next(4, 15))
                                });
                            }
                        }
                        if (armFrame > 9)
                            SwapAnimation(IDLE);
                    }
                    break;
                case BOTH_SLAM:
                    if (armFrame < 5)
                        armFrame = 5;
                    if (NPC.frame.Y < 6)
                        NPC.frame.Y = 6;

                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        armFrame++;
                        if (NPC.frame.Y is 9)
                        {
                            PunchCameraModifier camPunch = new(NPC.Center, new Vector2(0f, 1f * RedeConfigClient.Instance.ShakeIntensity), 10f, 4f, 30, 1000f, "Thorn");
                            Main.instance.CameraModifiers.Add(camPunch);
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.position);

                            SoundEngine.PlaySound(SoundID.Dig, NPC.position);
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Pitch = .2f }, NPC.position);

                            if (NPC.life < NPC.lifeMax / 4)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.WorldTree with { Volume = .5f, Pitch = .2f }, NPC.position);

                                for (int i = 0; i < 20; i++)
                                {
                                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                                    {
                                        PositionInWorld = RedeHelper.RandAreaInEntity(NPC),
                                        MovementVector = new Vector2(0, -Main.rand.Next(4, 15))
                                    });
                                }
                            }
                            for (int i = 0; i < 9; i++)
                            {
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                                {
                                    PositionInWorld = NPC.Center + new Vector2(46 * NPC.spriteDirection, 34) + new Vector2(Main.rand.Next(-17, 18), Main.rand.Next(-5, 6)),
                                    MovementVector = new Vector2(0, -Main.rand.Next(4, 15))
                                });
                            }
                        }
                        if (NPC.frame.Y > 10)
                            NPC.frame.Y = 0;
                        if (armFrame > 9)
                            SwapAnimation(IDLE);
                    }
                    break;
                case HOOD_DROP:
                    if (NPC.frame.Y < 11)
                        NPC.frame.Y = 11;
                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        if (NPC.frame.Y > 13)
                        {
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;
                            SwapAnimation(IDLE);
                        }
                    }
                    break;
                case DEATH:
                    if (++NPC.frameCounter >= 7)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        if (NPC.frame.Y is 3)
                        {
                            PunchCameraModifier camPunch = new(NPC.Center, new Vector2(0f, 1f * RedeConfigClient.Instance.ShakeIntensity), 10f, 4f, 30, 1000f, "Thorn");
                            Main.instance.CameraModifiers.Add(camPunch);
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.position);
                        }
                        if (NPC.frame.Y is 7)
                        {
                            if (Main.netMode != NetmodeID.Server)
                            {
                                Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + new Vector2(64 * NPC.spriteDirection, -34), new Vector2(2 * NPC.spriteDirection, -1), ModContent.Find<ModGore>("Redemption/ThornGore1").Type, 1);
                            }

                            SoundEngine.PlaySound(SoundID.Dig, NPC.position);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Saint9 with { Volume = .4f, Pitch = -.1f }, NPC.position);

                            for (int i = 0; i < 9; i++)
                            {
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                                {
                                    PositionInWorld = NPC.Center + new Vector2(46 * NPC.spriteDirection, 34) + new Vector2(Main.rand.Next(-17, 18), Main.rand.Next(-5, 6)),
                                    MovementVector = new Vector2(0, -Main.rand.Next(4, 15))
                                });
                            }
                        }
                        if (NPC.frame.Y is 12)
                        {
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 7;
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                            SoundEngine.PlaySound(SoundID.DD2_SkeletonDeath with { Pitch = -.5f }, NPC.position);
                            SoundEngine.PlaySound(SoundID.Grass, NPC.position);
                            SoundEngine.PlaySound(SoundID.Item17, NPC.position);

                            if (Main.netMode != NetmodeID.Server)
                            {
                                Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + new Vector2(3 * NPC.spriteDirection, -12), new Vector2(2 * NPC.spriteDirection, -15), ModContent.Find<ModGore>("Redemption/EpidotrianSkeletonGore2").Type, 1);

                                for (int i = 0; i < 16; i++)
                                    Gore.NewGore(NPC.GetSource_FromThis(), RedeHelper.RandAreaInEntity(NPC) - new Vector2(5, 6), RedeHelper.SpreadUp(20), ModContent.Find<ModGore>("Redemption/ThornFX").Type, Main.rand.NextFloat(1, 1.3f));
                            }
                        }
                        if (NPC.frame.Y > 18)
                            NPC.frame.Y = 18;
                    }
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                if (++NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y++;
                    if (NPC.frame.Y > 5)
                        NPC.frame.Y = 0;
                    armFrame++;
                    if (armFrame > 4)
                        armFrame = 0;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        float magicOpacity;
        float redEyeOpacity;
        float drawTimer;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawCenter = NPC.Center + new Vector2(0, NPC.gfxOffY);
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle rect;
            Vector2 origin;

            // Arm
            Rectangle armRect = ArmAni.Frame(1, 12, 0, armFrame);
            Vector2 armOrigin = armRect.Size() / 2 - new Vector2(20 * NPC.spriteDirection, 0);
            Vector2 armCenter = drawCenter + new Vector2(19 * NPC.spriteDirection, -4);

            if (AIState is ActionState.Death)
            {
                // Death
                Rectangle deathRect = DeathAni.Frame(1, 19, 0, NPC.frame.Y);
                Vector2 deathOrigin = deathRect.Size() / 2 - new Vector2(5 * NPC.spriteDirection, -19);

                if (NPC.frame.Y is 5)
                    spriteBatch.Draw(ArmAni.Value, armCenter + RedeHelper.Spread((AITimer - 95) / 20) - screenPos, armRect, NPC.GetAlpha(drawColor), NPC.rotation + armRot, armOrigin, NPC.scale, effects, 0f);

                Color color = Color.Lerp(drawColor, new Color(92, 64, 51), magicOpacity);
                spriteBatch.Draw(DeathAni.Value, drawCenter - screenPos, deathRect, NPC.GetAlpha(color), NPC.rotation, deathOrigin, NPC.scale * 2, effects, 0f);

                return false;
            }
            // Body
            rect = texture.Frame(2, Main.npcFrameCount[Type], NPC.frame.X, NPC.frame.Y);
            origin = rect.Size() / 2 - new Vector2(11 * NPC.spriteDirection, 0);

            if (AIState is ActionState.TeleportStart or ActionState.TeleportEnd)
            {
                spriteBatch.End();

                Effect effect = ModContent.Request<Effect>("Redemption/Effects/Teleport", AssetRequestMode.ImmediateLoad).Value;

                Texture2D noise = ModContent.Request<Texture2D>("Redemption/Textures/Noise/swirlnoiseharsh").Value;
                float progress = AIState is ActionState.TeleportStart ? MathF.Max(-1, AITimer / 10 - 2) : MathF.Min(2, (1 - magicOpacity) * 4);
                float dir = AIState is ActionState.TeleportStart ? 1 : -1;

                effect.Parameters["uImageSize0"].SetValue(ArmAni.Size());
                effect.Parameters["uSourceRect"].SetValue(new Vector4(armRect.X, armRect.Y, armRect.Width, armRect.Height));
                effect.Parameters["uImageSize1"].SetValue(new Vector2(10, 10));
                effect.Parameters["direction"].SetValue(dir);
                effect.Parameters["progress"].SetValue(progress);
                effect.Parameters["widthFactor"].SetValue(5);
                effect.Parameters["lineColor"].SetValue(new Vector4(10, 10, 10, 1));

                Main.graphics.GraphicsDevice.Textures[1] = noise;
                spriteBatch.BeginDefault(true);
                effect.CurrentTechnique.Passes[0].Apply();

                if (aniType is not HOOD_DROP)
                    spriteBatch.Draw(ArmAni.Value, armCenter - screenPos, armRect, NPC.GetAlpha(drawColor), NPC.rotation + armRot, armOrigin, NPC.scale, effects, 0f);

                spriteBatch.End();

                effect.Parameters["uImageSize0"].SetValue(texture.Size());
                effect.Parameters["uSourceRect"].SetValue(new Vector4(rect.X, rect.Y, rect.Width, rect.Height));

                Main.graphics.GraphicsDevice.Textures[1] = noise;
                spriteBatch.BeginDefault(true);
                effect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(texture, drawCenter - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0f);

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            else
            {
                if (aniType is not HOOD_DROP)
                    spriteBatch.Draw(ArmAni.Value, armCenter - screenPos, armRect, NPC.GetAlpha(drawColor), NPC.rotation + armRot, armOrigin, NPC.scale, effects, 0f);
                spriteBatch.Draw(texture, drawCenter - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0f);
            }
            if (NPC.IsABestiaryIconDummy)
                return false;

            if (magicOpacity > 0 && AIState is ActionState.PhaseTransition && AITimer >= 180)
            {
                Color color = Color.Orange;

                if (aniType is not HOOD_DROP)
                    RedeDraw.DrawTreasureBagEffect(spriteBatch, ArmAni.Value, ref drawTimer, armCenter - screenPos, armRect, color, NPC.rotation + armRot, armOrigin, NPC.scale, effects, magicOpacity);
                RedeDraw.DrawTreasureBagEffect(spriteBatch, texture, ref drawTimer, drawCenter - screenPos, rect, color, NPC.rotation, origin, NPC.scale, effects, magicOpacity);
            }

            var yOffset = NPC.frame.Y switch
            {
                2 or 3 or 7 or 8 or 9 => 2,
                _ => 0,
            };

            Vector2 eyePosL = drawCenter + new Vector2(15 * NPC.spriteDirection, -19 - yOffset) - screenPos;
            Vector2 eyePosR = drawCenter + new Vector2(7 * NPC.spriteDirection, -19 - yOffset) - screenPos;
            float eyeScale = NPC.scale + Main.rand.NextFloat(-.05f, .05f);
            if (redEyeOpacity > 0)
            {
                spriteBatch.Draw(CommonTextures.WhiteFlare.Value, eyePosL, null, NPC.GetAlpha(Color.Red with { A = 0 }) * redEyeOpacity, NPC.rotation, CommonTextures.WhiteFlare.Size() / 2, eyeScale * .5f, effects, 0f);
                spriteBatch.Draw(CommonTextures.WhiteFlare.Value, eyePosR, null, NPC.GetAlpha(Color.Red with { A = 0 }) * redEyeOpacity, NPC.rotation, CommonTextures.WhiteFlare.Size() / 2, eyeScale * .5f, effects, 0f);

                spriteBatch.Draw(CommonTextures.WhiteFlare.Value, eyePosL, null, NPC.GetAlpha(Color.IndianRed with { A = 0 }) * redEyeOpacity, NPC.rotation, CommonTextures.WhiteFlare.Size() / 2, eyeScale * .3f, effects, 0f);
                spriteBatch.Draw(CommonTextures.WhiteFlare.Value, eyePosR, null, NPC.GetAlpha(Color.IndianRed with { A = 0 }) * redEyeOpacity, NPC.rotation, CommonTextures.WhiteFlare.Size() / 2, eyeScale * .3f, effects, 0f);
            }
            else if (redEyeOpacity < 0)
            {
                spriteBatch.Draw(CommonTextures.WhiteFlare.Value, eyePosR, null, NPC.GetAlpha(Color.Blue with { A = 0 }) * -redEyeOpacity, NPC.rotation, CommonTextures.WhiteFlare.Size() / 2, eyeScale * .5f, effects, 0f);

                spriteBatch.Draw(CommonTextures.WhiteFlare.Value, eyePosR, null, NPC.GetAlpha(Color.SkyBlue with { A = 0 }) * -redEyeOpacity, NPC.rotation, CommonTextures.WhiteFlare.Size() / 2, eyeScale * .3f, effects, 0f);
            }
            return false;
        }
    }
}
