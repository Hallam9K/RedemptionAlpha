using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Redemption.Buffs.NPCBuffs;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Redemption.BaseExtension;
using Redemption.UI;
using System;
using Redemption.Dusts;
using Redemption.NPCs.Bosses.Cleaver;
using ReLogic.Content;
using Redemption.Items.Placeable.Trophies;
using Terraria.GameContent.ItemDropRules;

namespace Redemption.NPCs.Bosses.Obliterator
{
    [AutoloadBossHead]
    public class OO : ModNPC
    {
        public float[] oldrot = new float[5];

        public enum ActionState
        {
            Intro,
            Begin,
            Idle,
            Attacks,
            Overheat,
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
            DisplayName.SetDefault("Omega Obliterator");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                CustomTexturePath = "Redemption/Textures/Bestiary/OmegaObliterator_Bestiary"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 181000;
            NPC.damage = 200;
            NPC.defense = 80;
            NPC.knockBackResist = 0f;
            NPC.width = 100;
            NPC.height = 160;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.value = Item.buyPrice(0, 20, 0, 0);
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath14;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega2");
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LidenBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("An autonomous war machine mostly of Girus' own design with many integrated weapon systems, such as literal finger guns and an advanced heat dispersion system in the form of a giant beam, capable of obliterating anyone it engulfs - Also where Obliterator got its name from.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.MissileExplosion, NPC.position);
                RedeDraw.SpawnExplosion(NPC.Center, Color.OrangeRed);

                for (int i = 0; i < 80; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].velocity *= 1.9f;
                }
                for (int i = 0; i < 45; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SparksMech, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 15; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/OOGore" + (i + 1)).Type);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            if (!RedeBossDowned.downedVlitch3)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OO_GirusTalk>(), 0, 0, Main.myPlayer);

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedVlitch3, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<OOBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OmegaTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<OORelic>()));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0.8;
            return true;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.velocity.Length() > 12;
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            return !player.active || player.dead || Main.dayTime;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(BeamAnimation);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                BeamAnimation = reader.ReadBoolean();
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            ArmFrameY[0] = 2;
            ArmFrameY[1] = 1;
            ArmRot[0] = MathHelper.PiOver2 + (NPC.spriteDirection == -1 ? 0 : MathHelper.Pi);
            ArmRot[1] = 0f;
            HandsFrameY[0] = 2;
            HandsFrameY[1] = 2;
        }
        public Vector2 MoveVector2;
        public readonly Vector2 modifier = new(-19, -300);

        public bool BeamAnimation;

        public List<int> AttackList = new() { 0, 1, 2, 3, 4, 5, 6, 7 };
        public List<int> CopyList = null;
        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }
        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                if ((ID == 6 && NPC.life >= (int)(NPC.lifeMax * 0.6f)) ||
                (ID == 7 && NPC.life >= (int)(NPC.lifeMax * 0.7f)))
                    continue;

                attempts++;
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();
            DespawnHandler();
            Lighting.AddLight(NPC.Center, 0.7f, 0.4f, 0.4f);

            SoundStyle voice = CustomSounds.Voice5;
            if (RedeBossDowned.downedVlitch3)
                voice = CustomSounds.Voice5 with { Pitch = -0.5f };

            float RotFlip = NPC.spriteDirection == -1 ? 0 : MathHelper.Pi;
            int SpeedBoost = NPC.DistanceSQ(player.Center) >= 900 * 900 ? 10 : 0;
            Vector2 DefaultPos = new(player.Center.X - (240 * NPC.spriteDirection), player.Center.Y - 80);
            Vector2 DefaultPos2 = new(player.Center.X - (240 * NPC.spriteDirection), player.Center.Y - 40);
            Vector2 LaserPos = new(NPC.position.X + (NPC.spriteDirection == -1 ? 46 : 16), NPC.position.Y + 70);
            Vector2 RandPos = new(Main.rand.Next(-500, -300) * NPC.spriteDirection, Main.rand.Next(-400, 200));
            Vector2 ChargePos = new(player.Center.X - (400 * NPC.spriteDirection), player.Center.Y);
            Vector2 ShootPos = new(player.Center.X - (300 * NPC.spriteDirection), player.Center.Y - 10);
            Vector2 HandPos = NPC.Center + new Vector2(NPC.spriteDirection == -1 ? -110 : 64, -6);
            Vector2 EyePos = NPC.Center + RedeHelper.PolarVector(60, NPC.rotation - (float)Math.PI / 2) + RedeHelper.PolarVector(50 * NPC.spriteDirection, NPC.rotation);
            Vector2 BottomPos = new(player.Center.X - (500 * NPC.spriteDirection), player.Center.Y + 400);

            switch (AIState)
            {
                case ActionState.Intro:
                    switch (TimerRand)
                    {
                        case 0:
                            if (!Main.dedServ)
                                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                            ArmRot[0] = MathHelper.PiOver2 + (NPC.spriteDirection == -1 ? 0 : MathHelper.Pi);
                            NPC.LookAtEntity(player);
                            AITimer++;
                            if (NPC.DistanceSQ(DefaultPos) < 100 * 100 || AITimer > 200)
                            {
                                AITimer = 0;
                                if (RedeBossDowned.oblitDeath == 2 || RedeBossDowned.downedVlitch3)
                                {
                                    AIState = ActionState.Begin;
                                    TimerRand = 0;
                                }
                                else
                                    TimerRand = 1;
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.MoveToVector2(DefaultPos, 11f);
                            break;
                        case 1:
                            NPC.velocity *= 0.96f;
                            if (AITimer++ >= 30)
                            {
                                NPC.velocity *= 0f;
                                player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                player.RedemptionScreen().lockScreen = true;
                            }
                            if (AITimer == 120)
                            {
                                HandsFrameY[0] = 0;
                                ArmFrameY[0] = 1;
                                HeadFrameY = 1;

                                SoundEngine.PlaySound(CustomSounds.ObliteratorYo, NPC.position);
                                Dialogue d1 = new(NPC, "Yo.", Colors.RarityRed, Color.DarkRed, CustomSounds.Voice1 with { Volume = 0 }, 3, 60, 0, false, modifier: modifier); // 69

                                TextBubbleUI.Visible = true;
                                TextBubbleUI.Add(d1);
                            }
                            if (AITimer >= 120 && AITimer < 188)
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + (-1 * NPC.spriteDirection) + RotFlip, MathHelper.Pi / 50);
                            else if (AITimer >= 188 && AITimer < 350)
                            {
                                HeadFrameY = 0;
                                HandsFrameY[0] = 2;
                                ArmRot[0].SlowRotation(0, MathHelper.Pi / 20);
                            }
                            else
                                ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                            if (AITimer == 170)
                                BeamAnimation = true;
                            if (AITimer < 190 || AITimer > 370)
                                NPC.LookAtEntity(player);
                            if (AITimer >= 190 && AITimer < 238)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(LaserPos + vector, 2, 2, ModContent.DustType<GlowDust>())];
                                    dust2.noGravity = true;
                                    Color dustColor = new(Color.Red.R, Color.Red.G, Color.Red.B) { A = 0 };
                                    dust2.color = dustColor;
                                    dust2.velocity = dust2.position.DirectionTo(LaserPos) * 10f;
                                }
                            }
                            if (AITimer == 190 && player.active && !player.dead)
                            {
                                NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaMegaBeam>(), 1000, new Vector2(10 * NPC.spriteDirection, 0), true, CustomSounds.MegaLaser, NPC.whoAmI);
                            }
                            if (AITimer == 350)
                            {
                                ArmFrameY[0] = 2;
                                BeamAnimation = false;
                            }
                            if (AITimer == 400)
                            {
                                Dialogue d1 = new(NPC, "I guess I can't fool you twice,[10] huh.", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier); // 182
                                Dialogue d2 = new(NPC, "So much for a surprise attack...", Colors.RarityRed, Color.DarkRed, voice, 2, 118, 0, false, modifier: modifier); // 182

                                TextBubbleUI.Visible = true;
                                if (RedeBossDowned.oblitDeath == 1)
                                    TextBubbleUI.Add(d1);
                                else
                                    TextBubbleUI.Add(d2);
                            }
                            if (AITimer == 582)
                            {
                                DialogueChain chain = new();
                                chain.Add(new(NPC, "Hang on,[10] I got a call from Girus.", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 166
                                     .Add(new(NPC, "'I wasted too much energy too quickly?'", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 178
                                     .Add(new(NPC, "'I'm an idiot?'", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 130
                                     .Add(new(NPC, "You're scrapping my personality drive after this fight?", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 210
                                     .Add(new(NPC, "Ah well,[10] request accepted...", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 156
                                     .Add(new(NPC, "Anyway...", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 30, true, modifier: modifier)); // 148
                                HeadFrameY = 2;
                                TextBubbleUI.Visible = true;
                                TextBubbleUI.Add(chain);
                            }
                            if (AITimer == 1266)
                                HeadFrameY = 0;
                            if (AITimer > 1560)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.LabSafeS, NPC.position);
                                for (int i = 0; i < 100; i++)
                                {
                                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, Scale: 1.5f);
                                    Main.dust[dustIndex].velocity *= 1.9f;
                                }
                                RedeBossDowned.oblitDeath = 2;
                                AIState = ActionState.Begin;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Begin:
                    #region Fight Startup
                    NPC.LookAtEntity(player);
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega2");
                    ArmRot[0] = MathHelper.PiOver2 + (NPC.spriteDirection == -1 ? 0 : MathHelper.Pi);
                    if (AITimer++ == 0 && Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Omega Obliterator", 60, 90, 0.8f, 0, Color.Red, "3rd Omega Prototype");
                    if (AITimer < 60)
                        NPC.Move(DefaultPos2, 9, 10);
                    else
                        NPC.velocity *= 0.96f;
                    if (AITimer == 60)
                    {
                        NPC.Shoot(new Vector2(NPC.Center.X - (120 * 16) - 10, NPC.Center.Y + 8), ModContent.ProjectileType<OOBarrier>(), 0, Vector2.Zero, false, SoundID.Item1, 0, 1);
                        NPC.Shoot(new Vector2(NPC.Center.X + (120 * 16) + 26, NPC.Center.Y + 8), ModContent.ProjectileType<OOBarrier>(), 0, Vector2.Zero, false, SoundID.Item1, 0, -1);
                        NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y - (120 * 16) - 10), ModContent.ProjectileType<OOBarrierH>(), 0, Vector2.Zero, false, SoundID.Item1, 0, 1);
                        NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y + (120 * 16) + 26), ModContent.ProjectileType<OOBarrierH>(), 0, Vector2.Zero, false, SoundID.Item1, 0, -1);

                        ArenaWorld.arenaBoss = "OO";
                        ArenaWorld.arenaTopLeft = new Vector2(NPC.Center.X - (120 * 16) + 8, NPC.Center.Y - (120 * 16) + 8);
                        ArenaWorld.arenaSize = new Vector2(240 * 16, 240 * 16);
                        ArenaWorld.arenaMiddle = NPC.Center;
                        ArenaWorld.arenaActive = true;

                        ArmFrameY[0] = 1;
                        HandsFrameY[0] = 1;

                        string s = "Ready for obliteration?";
                        if (RedeBossDowned.downedVlitch3)
                            s = "PREPARE FOR OBLITERATION.";

                        Dialogue d1 = new(NPC, s, Colors.RarityRed, Color.DarkRed, voice, 2, 100, 30, true, modifier: modifier); // 176

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(d1);
                    }
                    if (AITimer >= 236)
                    {
                        NPC.dontTakeDamage = false;
                        ArmFrameY[0] = 2;
                        HandsFrameY[0] = 2;
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    #endregion
                    break;
                case ActionState.Idle:
                    NPC.LookAtEntity(player);

                    ArmFrameY[0] = 2;
                    ArmFrameY[1] = 1;
                    ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                    ArmRot[1] = 0;

                    AIState = ActionState.Attacks;
                    AITimer = 0;
                    TimerRand = 0;
                    AttackChoice();
                    MoveVector2 = RandPos;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Directional Charge
                        case 0:
                            if (AITimer < 180)
                            {
                                ArmRot[0].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                int frame = 0;
                                if (NPC.velocity.Y >= 1)
                                    frame = 1;
                                ArmFrameY[0] = frame;
                                ArmFrameY[1] = frame;
                            }
                            else
                            {
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 1;
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(0, MathHelper.Pi / 40);
                            }
                            if (NPC.velocity.Length() <= 12)
                                NPC.LookAtEntity(player);

                            if (AITimer == 0)
                            {
                                NPC.velocity *= 0f;
                                AITimer = 1;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (AITimer++ == 2 || AITimer == 100)
                                {
                                    SoundEngine.PlaySound(CustomSounds.OODashReady, NPC.position);
                                    NPC.velocity = -NPC.DirectionTo(player.Center) * 7f;
                                }
                                if (AITimer == 40 || AITimer == 140)
                                    Dash(60 + SpeedBoost, true);
                                if (AITimer > 220)
                                {
                                    NPC.velocity *= 0f;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    TimerRand = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Side Charge
                        case 1:
                            if (AITimer >= 200 && AITimer < 260)
                            {
                                ArmRot[0].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                int frame = 0;
                                if (NPC.velocity.Y >= 1)
                                    frame = 1;
                                ArmFrameY[0] = frame;
                                ArmFrameY[1] = frame;
                            }
                            else
                            {
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 1;
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(0, MathHelper.Pi / 40);
                            }
                            if (NPC.velocity.Length() <= 12)
                                NPC.LookAtEntity(player);

                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ChargePos) < 200 * 200 || AITimer >= 60)
                                {
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ChargePos, 15f, 10);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (AITimer == 205)
                                {
                                    SoundEngine.PlaySound(CustomSounds.OODashReady, NPC.position);
                                    NPC.velocity.X = player.Center.X > NPC.Center.X ? -8 : 8;
                                }

                                if (AITimer == 235)
                                    Dash(60 + SpeedBoost, false);

                                if (AITimer > 235 && AITimer % 3 == 0 && NPC.velocity.Length() > 12)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, 12f), true, SoundID.Item91);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, -12f), true, SoundID.Item91);
                                }
                                if (AITimer > 310)
                                {
                                    NPC.velocity *= 0f;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    TimerRand = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Plasma Orb 1
                        case 2:
                            NPC.LookAtEntity(player);
                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ShootPos) < 100 * 100 || AITimer >= (NPC.life < (int)(NPC.lifeMax * 0.4f) ? 20 : 40))
                                {
                                    BeamAnimation = true;
                                    if (NPC.frame.Y >= 404)
                                    {
                                        AITimer = 200;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else
                                    NPC.Move(ShootPos, 16 + SpeedBoost, 10);
                            }
                            else
                            {
                                NPC.velocity *= 0.94f;
                                if (AITimer == 210)
                                {
                                    if (NPC.life < (int)(NPC.lifeMax * 0.6f))
                                    {
                                        for (int i = 0; i < 3; i++)
                                        {
                                            int rot = 25 * i;
                                            NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, RedeHelper.PolarVector((i == 1 ? 25 : 20) + SpeedBoost, (NPC.spriteDirection == -1 ? MathHelper.Pi : 0) + MathHelper.ToRadians(rot - 25)), true, CustomSounds.BallCreate);
                                        }
                                    }
                                    else
                                        NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, new Vector2((16 + SpeedBoost) * NPC.spriteDirection, 0), true, CustomSounds.BallCreate);
                                }

                                if (AITimer > 230)
                                {
                                    BeamAnimation = false;
                                    if (TimerRand >= 2)
                                    {
                                        TimerRand = 0;
                                        NPC.velocity *= 0f;
                                        AIState = ActionState.Idle;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        TimerRand++;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Plasma Orb 2
                        case 3:
                            NPC.LookAtEntity(player);
                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ShootPos) < 500 * 500 || AITimer >= 40)
                                {
                                    BeamAnimation = true;
                                    if (NPC.frame.Y >= 404)
                                    {
                                        if (player.Center.Y > NPC.Center.Y)
                                            NPC.velocity.Y = 20f;
                                        else
                                            NPC.velocity.Y = -20f;

                                        AITimer = 200;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else
                                    NPC.Move(ShootPos, 19 + SpeedBoost, 10);
                            }
                            else
                            {
                                NPC.velocity.Y *= 0.98f;
                                NPC.velocity.X *= 0.8f;
                                if (AITimer > 200 && AITimer % 7 == 0)
                                    NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, new Vector2((12 + SpeedBoost) * NPC.spriteDirection, 0), true, CustomSounds.BallCreate);

                                if (AITimer > 270)
                                {
                                    BeamAnimation = false;
                                    NPC.velocity *= 0f;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Annihilation Cannon
                        case 4:
                            NPC.LookAtEntity(player);
                            if (AITimer >= 200 && AITimer <= 350)
                            {
                                ArmRot[0].SlowRotation(0.4f * -NPC.spriteDirection, MathHelper.Pi / 30);
                                ArmRot[1].SlowRotation(0.4f * -NPC.spriteDirection, MathHelper.Pi / 30);
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 2;
                            }
                            else
                            {
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 1;
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 20);
                                ArmRot[1].SlowRotation(0, MathHelper.Pi / 20);
                            }
                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ShootPos) < 500 * 500 || AITimer >= 60)
                                {
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, 18 + SpeedBoost, 10);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (NPC.life < (int)(NPC.lifeMax * 0.4f))
                                {
                                    if (AITimer > 200 && AITimer % (NPC.DistanceSQ(player.Center) >= 900 * 900 ? 2 : 4) == 0 && AITimer < 320)
                                        NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-600, 600), player.Center.Y + Main.rand.Next(-600, 600)) + (player.velocity * 20), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                                }
                                else
                                {
                                    if (AITimer > 200 && AITimer % (NPC.DistanceSQ(player.Center) >= 900 * 900 ? 4 : 6) == 0 && AITimer < 320)
                                        NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-600, 600), player.Center.Y + Main.rand.Next(-600, 600)) + (player.velocity * 20), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                                }
                                if (AITimer > 380)
                                {
                                    ArmRFrameY[0] = 0;
                                    ArmRFrameY[1] = 0;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Plasma Shots
                        case 5:
                            if (AITimer++ < 300)
                                NPC.LookAtEntity(player);

                            ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 20);
                            if (AITimer <= 305)
                                HandsFrameY[0] = 1;
                            else
                                HandsFrameY[0] = 2;

                            if (NPC.life < (int)(NPC.lifeMax * 0.4f))
                            {
                                if ((NPC.life < (int)(NPC.lifeMax * 0.2f) ? AITimer % 8 == 0 : AITimer % 10 == 0) && AITimer < 300)
                                    NPC.Shoot(HandPos, ModContent.ProjectileType<OO_Fingerflash>(), 150, Vector2.Zero, false, CustomSounds.Laser1, NPC.whoAmI);
                            }
                            else
                            {
                                if (AITimer % 15 == 0 && AITimer < 300)
                                    NPC.Shoot(HandPos, ModContent.ProjectileType<OO_Fingerflash>(), 150, Vector2.Zero, false, CustomSounds.Laser1, NPC.whoAmI);
                            }
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(MoveVector2) < 100 * 100 || AITimer >= 30)
                                {
                                    NPC.netUpdate = true;
                                    AITimer = 200;
                                }
                                else
                                    NPC.Move(MoveVector2, 20 + SpeedBoost, 10, true);
                            }
                            else if (AITimer >= 200 && AITimer < 300)
                            {
                                NPC.velocity *= 0.9f;
                                if (AITimer > 210)
                                {
                                    if (TimerRand >= 4)
                                    {
                                        TimerRand = 0;
                                        NPC.velocity *= 0f;
                                        AITimer = 300;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        TimerRand++;
                                        MoveVector2 = RandPos;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                NPC.MoveToVector2(ShootPos + new Vector2(0, 40), 8 + SpeedBoost);
                                if (AITimer == 305)
                                {
                                    if (!RedeBossDowned.downedVlitch3)
                                    {
                                        Dialogue d1 = new(NPC, "Eye beam!", Colors.RarityRed, Color.DarkRed, voice, 2, 70, 30, true, modifier: modifier);
                                        TextBubbleUI.Visible = true;
                                        TextBubbleUI.Add(d1);
                                    }

                                    for (int i = 0; i < 3; i++)
                                        NPC.Shoot(EyePos, ModContent.ProjectileType<OO_NormalBeam>(), 180, new Vector2(1 * NPC.spriteDirection, 0), true, CustomSounds.Laser1, NPC.whoAmI, i);
                                }
                                if (AITimer > 420)
                                {
                                    NPC.velocity *= 0f;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Stun Beam
                        case 6:
                            AITimer++;
                            if (AITimer >= 200 && AITimer <= 380)
                            {
                                ArmRot[0].SlowRotation(0.4f * -NPC.spriteDirection, MathHelper.Pi / 30);
                                ArmRot[1].SlowRotation(0.4f * -NPC.spriteDirection, MathHelper.Pi / 30);
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 2;
                            }
                            else
                            {
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 1;
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 20);
                                ArmRot[1].SlowRotation(0, MathHelper.Pi / 20);
                            }
                            if (AITimer < 200)
                            {
                                if (AITimer >= 80)
                                {
                                    NPC.Shoot(EyePos, ModContent.ProjectileType<OO_StunBeam>(), 100, new Vector2(10 * NPC.spriteDirection, 0), true, CustomSounds.BallFire, NPC.whoAmI);
                                    NPC.velocity *= 0f;
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.Move(BottomPos, 30 + SpeedBoost, 10);
                                }
                            }
                            else
                            {
                                if (AITimer < 205)
                                    NPC.velocity.Y -= 2f;

                                if (AITimer > 200 && AITimer % 6 == 0 && AITimer < 360 && player.active && !player.dead && player.HasBuff(ModContent.BuffType<StaticStunDebuff>()))
                                    NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-80, 80), player.Center.Y + Main.rand.Next(-80, 80)), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);

                                if (AITimer > 300)
                                    NPC.velocity *= 0.98f;

                                if (AITimer > 400)
                                {
                                    NPC.velocity *= 0f;
                                    AITimer = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Waving Deathrays
                        case 7:
                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ChargePos) < 200 * 200 || AITimer >= 80)
                                {
                                    for (int i = 0; i < 2; i++)
                                        NPC.Shoot(LaserPos, ModContent.ProjectileType<OO_NormalBeam>(), 180, new Vector2(10 * NPC.spriteDirection, 0), true, CustomSounds.Laser1, NPC.whoAmI, i + 3);

                                    NPC.velocity *= 0f;
                                    AITimer = 200;
                                }
                                else
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.MoveToVector2(ChargePos, 30 + SpeedBoost);
                                }
                            }
                            else
                            {
                                NPC.MoveToVector2(ChargePos, 6 + SpeedBoost);
                                if (AITimer > 320)
                                    NPC.velocity *= 0.98f;

                                if (AITimer > 380)
                                {
                                    NPC.velocity *= 0f;
                                    AITimer = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Overheat:
                    if (TimerRand == 1)
                    {
                        if (AITimer >= 878 && AITimer <= 1206 && !RedeBossDowned.downedVlitch3)
                        {
                            ArmRot[0].SlowRotation(MathHelper.PiOver2 + ((1f + Main.rand.NextFloat(-0.05f, 0.05f)) * -NPC.spriteDirection) + RotFlip, MathHelper.Pi / 30);
                            ArmRot[1].SlowRotation(MathHelper.PiOver2 + ((1f + Main.rand.NextFloat(-0.05f, 0.05f)) * -NPC.spriteDirection) + RotFlip, MathHelper.Pi / 30);
                            HandsFrameY[0] = 0;
                            HandsFrameY[1] = 0;
                            ArmFrameY[0] = 0;
                            ArmFrameY[1] = 0;
                        }
                        else
                        {
                            ArmFrameY[0] = 2;
                            ArmFrameY[1] = 1;
                            HandsFrameY[0] = 2;
                            HandsFrameY[1] = 2;
                            ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 20);
                            ArmRot[1].SlowRotation(0, MathHelper.Pi / 20);
                        }
                    }
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, Scale: 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.X = 0;
                    Main.dust[dust].velocity.Y = -5;
                    if ((TimerRand == 1 && AITimer >= (RedeBossDowned.downedVlitch3 ? 196 : 878)) || TimerRand > 1)
                    {
                        player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(3, player.RedemptionScreen().ScreenShakeIntensity);
                        Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.5f).UseIntensity(0.6f).UseColor(Color.DarkRed).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                        player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
                    }
                    switch (TimerRand)
                    {
                        case 0:
                            NPC.LookAtEntity(player);
                            ArmFrameY[0] = 2;
                            ArmFrameY[1] = 1;
                            ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                            ArmRot[1] = 0;
                            HeadFrameY = 0;
                            BeamAnimation = false;

                            ID = 0;
                            AITimer = 0;
                            TimerRand = 1;
                            MoveVector2 = RandPos;
                            NPC.dontTakeDamage = true;
                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                            break;
                        case 1:
                            NPC.LookAtEntity(player);
                            NPC.velocity *= 0.8f;
                            AITimer++;
                            if (AITimer == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                                RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, DustID.LifeDrain, tex: ModContent.Request<Texture2D>("Redemption/Empty").Value);
                                AITimer = 1;
                            }
                            else
                            {
                                if (AITimer < 1208)
                                {
                                    player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                    player.GetModPlayer<ScreenPlayer>().cutscene = true;
                                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                                }
                                if (AITimer == 60)
                                {
                                    if (!RedeBossDowned.downedVlitch3)
                                    {
                                        DialogueChain chain = new();
                                        chain.Add(new(NPC, "SYSTEM OVERLOAD...", Colors.RarityRed, Color.DarkRed, voice with { Pitch = -0.5f }, 2, 100, 0, false, modifier: modifier)) // 136
                                             .Add(new(NPC, "Overload?[30] Damn right I'm overloading!", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 204
                                             .Add(new(NPC, "My circuits are burning with energy![10] This is truly exhilarating!", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 238
                                             .Add(new(NPC, "OVERHEATING...[10] OVERHEATING...[10] OVERHEATING...[10]", Colors.RarityRed, Color.DarkRed, voice with { Pitch = -0.5f }, 2, 100, 0, false, modifier: modifier)) // 218
                                             .Add(new(NPC, "Hahaha.[30] HAHAHAHAHAHAHA!", Colors.RarityRed, Color.DarkRed, voice with { Pitch = 0.1f, PitchVariance = 0.1f }, 2, 100, 0, false, modifier: modifier)) // 156
                                             .Add(new(NPC, "THE POWER OF THE SUN IN MY VERY CORE!", Colors.RarityRed, Color.DarkRed, voice with { Pitch = 0.3f, PitchVariance = 0.3f }, 2, 100, 30, true, modifier: modifier)); // 204
                                        TextBubbleUI.Visible = true;
                                        TextBubbleUI.Add(chain);
                                    }
                                    else
                                    {
                                        DialogueChain chain = new();
                                        chain.Add(new(NPC, "SYSTEM OVERLOAD...", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 136
                                             .Add(new(NPC, "OVERHEATING...[10] OVERHEATING...[10] OVERHEATING...[10]", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)); // 218
                                        TextBubbleUI.Visible = true;
                                        TextBubbleUI.Add(chain);
                                    }
                                }
                                if (AITimer == (RedeBossDowned.downedVlitch3 ? 196 : 638))
                                {
                                    SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                                    RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, DustID.LifeDrain, tex: ModContent.Request<Texture2D>("Redemption/Empty").Value);
                                }
                                if (AITimer == 878 && !RedeBossDowned.downedVlitch3)
                                    HeadFrameY = 2;

                                if (AITimer > (RedeBossDowned.downedVlitch3 ? 414 : 1238))
                                {
                                    HeadFrameY = 0;
                                    AITimer = 0;
                                    TimerRand = 2;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 2:
                            if (AITimer >= 200 && AITimer < 260)
                            {
                                ArmRot[0].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                int frame = 0;
                                if (NPC.velocity.Y >= 1)
                                    frame = 1;
                                ArmFrameY[0] = frame;
                                ArmFrameY[1] = frame;
                            }
                            else
                            {
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 1;
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(0, MathHelper.Pi / 40);
                            }

                            if (NPC.velocity.Length() <= 12)
                                NPC.LookAtEntity(player);
                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ChargePos) < 200 * 200 || AITimer >= 60)
                                {
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ChargePos, 40 + SpeedBoost, 10);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (AITimer == 205)
                                {
                                    SoundEngine.PlaySound(CustomSounds.OODashReady, NPC.position);
                                    NPC.velocity.X = player.Center.X > NPC.Center.X ? -8 : 8;
                                }

                                if (AITimer == 225)
                                    Dash(60 + SpeedBoost, false);

                                if (AITimer > 225 && AITimer % 3 == 0 && NPC.velocity.Length() > 12)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, 12f), true, SoundID.Item91);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, -12f), true, SoundID.Item91);
                                }
                                if (AITimer > 260)
                                {
                                    if (ID >= 1)
                                    {
                                        ArmFrameY[0] = 2;
                                        ArmFrameY[1] = 1;
                                        ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                                        ArmRot[1] = 0;

                                        ID = 0;
                                        NPC.velocity *= 0f;
                                        AITimer = 0;
                                        TimerRand++;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        ID++;
                                        NPC.velocity *= 0f;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        case 3:
                            ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 20);
                            if (AITimer <= 305)
                                HandsFrameY[0] = 1;
                            else
                                HandsFrameY[0] = 2;

                            if (AITimer < 300)
                                NPC.LookAtEntity(player);

                            AITimer++;
                            if (AITimer % 5 == 0 && AITimer < 300)
                                NPC.Shoot(HandPos, ModContent.ProjectileType<OO_Fingerflash>(), 150, Vector2.Zero, false, CustomSounds.Laser1, NPC.whoAmI);

                            if (AITimer < 200)
                            {
                                if (NPC.Distance(MoveVector2) < 100 || AITimer >= 20)
                                {
                                    SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                                    RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, DustID.LifeDrain, tex: ModContent.Request<Texture2D>("Redemption/Empty").Value);
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(MoveVector2, 50 + SpeedBoost, 10, true);
                            }
                            else if (AITimer >= 200 && AITimer < 300)
                            {
                                NPC.velocity *= 0.8f;
                                if (AITimer > 210)
                                {
                                    if (ID >= 4)
                                    {
                                        ID = 0;
                                        NPC.velocity *= 0f;
                                        AITimer = 300;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        ID++;
                                        MoveVector2 = RandPos;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                NPC.MoveToVector2(ShootPos + new Vector2(0, 40), 8 + SpeedBoost);
                                if (AITimer == 305 && !RedeBossDowned.downedVlitch3)
                                {
                                    Dialogue d1 = new(NPC, "EYE BEAM! EYE BEAM! EYE BEAM! EYE BEAM!", Colors.RarityRed, Color.DarkRed, voice with { Pitch = 0.3f, PitchVariance = 0.3f }, 1, 70, 30, true, modifier: modifier);
                                    TextBubbleUI.Visible = true;
                                    TextBubbleUI.Add(d1);
                                }
                                if (AITimer >= 305 && AITimer % 4 == 0 && AITimer <= 355)
                                    NPC.Shoot(EyePos, ModContent.ProjectileType<OO_NormalBeam>(), 180, new Vector2(1 * NPC.spriteDirection, Main.rand.NextFloat(-0.25f, 0.25f)), true, CustomSounds.Laser1, NPC.whoAmI, -1);

                                if (AITimer > 420)
                                {
                                    ID = 0;
                                    NPC.velocity *= 0f;
                                    AITimer = 0;
                                    TimerRand++;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 4:
                            NPC.LookAtEntity(player);
                            if (AITimer >= 200 && AITimer <= 350)
                            {
                                ArmRot[0].SlowRotation(0.4f * -NPC.spriteDirection, MathHelper.Pi / 30);
                                ArmRot[1].SlowRotation(0.4f * -NPC.spriteDirection, MathHelper.Pi / 30);
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 2;
                            }
                            else
                            {
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 1;
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 20);
                                ArmRot[1].SlowRotation(0, MathHelper.Pi / 20);
                            }
                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ShootPos) < 500 * 500 || AITimer >= 30)
                                {
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, 25 + SpeedBoost, 10);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (AITimer > 200 && AITimer % 2 == 0 && AITimer < 320)
                                    NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-900, 900), player.Center.Y + Main.rand.Next(-900, 900)), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);

                                if (AITimer > 380)
                                {
                                    ArmRFrameY[0] = 0;
                                    ArmRFrameY[1] = 0;
                                    NPC.velocity *= 0f;
                                    AITimer = 0;
                                    TimerRand++;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 5:
                            NPC.LookAtEntity(player);
                            ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                            ArmRot[1] = 0;

                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ShootPos) < 500 * 500 || AITimer >= 10)
                                {
                                    BeamAnimation = true;
                                    if (NPC.frame.Y >= 404)
                                    {
                                        if (player.Center.Y > NPC.Center.Y)
                                            NPC.velocity.Y = 20f;
                                        else
                                            NPC.velocity.Y = -20f;

                                        AITimer = 200;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else
                                    NPC.Move(ShootPos, 24 + SpeedBoost, 10);
                            }
                            else
                            {
                                NPC.velocity.Y *= 0.98f;
                                NPC.velocity.X *= 0.5f;
                                if (AITimer > 200 && AITimer % 5 == 0)
                                    NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, new Vector2((12 + SpeedBoost) * NPC.spriteDirection, 0), true, CustomSounds.BallCreate);

                                if (AITimer > 240)
                                {
                                    BeamAnimation = false;
                                    NPC.velocity *= 0f;
                                    AITimer = 0;
                                    TimerRand++;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 6:
                            if (AITimer < 170)
                            {
                                ArmRot[0].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(NPC.velocity.X / 40 - (0.4f * -NPC.spriteDirection), MathHelper.Pi / 40);
                                int frame = 0;
                                if (NPC.velocity.Y >= 1)
                                    frame = 1;
                                ArmFrameY[0] = frame;
                                ArmFrameY[1] = frame;
                            }
                            else
                            {
                                ArmFrameY[0] = 2;
                                ArmFrameY[1] = 1;
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 40);
                                ArmRot[1].SlowRotation(0, MathHelper.Pi / 40);
                            }
                            if (NPC.velocity.Length() > 12)
                                NPC.LookAtEntity(player);

                            if (AITimer == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                                RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, DustID.LifeDrain, tex: ModContent.Request<Texture2D>("Redemption/Empty").Value);
                                NPC.velocity *= 0f;
                                AITimer = 1;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                AITimer++;
                                if (AITimer == 2 || AITimer == 80)
                                {
                                    SoundEngine.PlaySound(CustomSounds.OODashReady, NPC.position);
                                    NPC.velocity = -NPC.DirectionTo(player.Center) * 7f;
                                }

                                if (AITimer == 30 || AITimer == 110)
                                    Dash(70 + SpeedBoost, true);

                                if (AITimer > 170)
                                {
                                    if (ID >= 1)
                                    {
                                        ArmFrameY[0] = 2;
                                        ArmFrameY[1] = 1;
                                        ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                                        ArmRot[1] = 0;

                                        ID = 0;
                                        NPC.velocity *= 0f;
                                        AITimer = 0;
                                        TimerRand++;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        ID++;
                                        NPC.velocity *= 0f;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        case 7:
                            ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                            ArmRot[1] = 0;
                            AITimer++;
                            if (AITimer < 200)
                            {
                                if (NPC.DistanceSQ(ShootPos) < 200 * 200 || AITimer >= 60)
                                {
                                    BeamAnimation = true;
                                    NPC.velocity *= 0f;
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    NPC.MoveToVector2(ShootPos, 34 + SpeedBoost);
                                }
                            }
                            else
                            {
                                if (AITimer < 210 || AITimer > 390)
                                    NPC.LookAtEntity(player);
                                if (AITimer >= 210 && AITimer < 258)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        Vector2 vector;
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        vector.X = (float)(Math.Sin(angle) * 100);
                                        vector.Y = (float)(Math.Cos(angle) * 100);
                                        Dust dust2 = Main.dust[Dust.NewDust(LaserPos + vector, 2, 2, ModContent.DustType<GlowDust>())];
                                        dust2.noGravity = true;
                                        Color dustColor = new(Color.Red.R, Color.Red.G, Color.Red.B) { A = 0 };
                                        dust2.color = dustColor;
                                        dust2.velocity = dust2.position.DirectionTo(LaserPos) * 10f;
                                    }
                                }
                                if (AITimer == 210 && player.active && !player.dead)
                                {
                                    NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaMegaBeam>(), 1000, new Vector2(10 * NPC.spriteDirection, 0), true, CustomSounds.MegaLaser, NPC.whoAmI);
                                }
                                if (AITimer == 370)
                                    BeamAnimation = false;

                                if (AITimer >= 420)
                                {
                                    player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                    player.GetModPlayer<ScreenPlayer>().cutscene = true;
                                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                                }
                                if (AITimer == 450)
                                {
                                    SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                                    RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, DustID.LifeDrain, tex: ModContent.Request<Texture2D>("Redemption/Empty").Value);

                                    if (!RedeBossDowned.downedVlitch3)
                                    {
                                        DialogueChain chain = new();
                                        chain.Add(new(NPC, "CRITICAL CONDITION REACHED...[30] SELF DESTRUCTING...", Colors.RarityRed, Color.DarkRed, voice with { Pitch = -0.5f }, 2, 100, 0, false, modifier: modifier)) // 228
                                             .Add(new(NPC, "Is it getting hot in here[10] or is it just m-", Colors.RarityRed, Color.DarkRed, voice with { Pitch = 0.3f, PitchVariance = 0.3f }, 3, 3, 0, false, modifier: modifier)); // 124
                                        TextBubbleUI.Visible = true;
                                        TextBubbleUI.Add(chain);
                                    }
                                    else
                                    {
                                        DialogueChain chain = new();
                                        chain.Add(new(NPC, "CRITICAL CONDITION REACHED...[30] SELF DESTRUCTING...", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)); // 228
                                        TextBubbleUI.Visible = true;
                                        TextBubbleUI.Add(chain);
                                    }
                                }
                                if (AITimer > (RedeBossDowned.downedVlitch3 ? 678 : 804))
                                {
                                    NPC.velocity *= 0f;
                                    AITimer = 0;
                                    AIState = ActionState.Death;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                    }
                    break;
                case ActionState.Death:
                    NPC.dontTakeDamage = false;
                    player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                    NPC.netUpdate = true;
                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (BeamAnimation)
            {
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;
                }
            }
            else
            {
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y -= frameHeight;
                    if (NPC.frame.Y < 0)
                        NPC.frame.Y = 0;
                }
            }
            if (AIState is ActionState.Attacks || (AIState is ActionState.Overheat && TimerRand > 1))
            {
                if ((NPC.spriteDirection == -1 && NPC.velocity.X >= 8) || (NPC.spriteDirection == 1 && NPC.velocity.X <= -8))
                    HeadFrameY = 1;
                else if ((NPC.spriteDirection == 1 && NPC.velocity.X >= 8) || (NPC.spriteDirection == -1 && NPC.velocity.X <= -8))
                    HeadFrameY = 2;
                else
                    HeadFrameY = 0;
            }
            LegFrameY = 2 + (int)(-NPC.velocity.Y / 6);
            LegFrameY = (int)MathHelper.Clamp(LegFrameY, 0, 4);
            NPC.rotation = NPC.velocity.X * 0.01f;
        }
        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
                return true;
            else
            {
                TimerRand = 0;
                AITimer = 0;
                AIState = ActionState.Overheat;
                NPC.life = 1;
                return false;
            }
        }
        public void Dash(int speed, bool directional)
        {
            Player player = Main.player[NPC.target];
            SoundEngine.PlaySound(SoundID.Item74, NPC.position);
            if (directional)
                NPC.velocity = NPC.DirectionTo(player.Center) * speed;
            else
                NPC.velocity.X = player.Center.X > NPC.Center.X ? speed : -speed;
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            SoundStyle voice = CustomSounds.Voice1 with { Pitch = -0.8f };
            if (!player.active || player.dead)
            {
                NPC.velocity *= 0.96f;
                float RotFlip = NPC.spriteDirection == -1 ? 0 : MathHelper.Pi;
                ArmFrameY[0] = 2;
                ArmFrameY[1] = 1;
                ArmRot[0].SlowRotation(MathHelper.PiOver2 + RotFlip, MathHelper.Pi / 40);
                ArmRot[1].SlowRotation(0, MathHelper.Pi / 40);
                if (AIState is ActionState.Intro && AITimer > 190 && RedeBossDowned.oblitDeath == 0)
                {
                    RedeBossDowned.oblitDeath = 1;
                    AITimer = 0;
                    TimerRand = 0;
                    NPC.ai[0] = -1;
                }
                else if (NPC.ai[0] == -1)
                {
                    if (AITimer++ == 100)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, "Alright,[10] target eliminated.", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 0, false, modifier: modifier)) // 154
                             .Add(new(NPC, "Returning to base...", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 30, true, modifier: modifier)); // 170

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(chain);
                    }
                    if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<OmegaMegaBeam>()))
                        BeamAnimation = false;
                    if (AITimer > 284)
                    {
                        NPC.velocity.Y -= 1;
                        if (NPC.timeLeft > 10)
                            NPC.timeLeft = 10;
                    }
                }
                else
                {
                    if (NPC.ai[0] != -2)
                    {
                        Dialogue d1 = new(NPC, "Target eliminated...", Colors.RarityRed, Color.DarkRed, voice, 2, 100, 30, true, modifier: modifier); // 150

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(d1);
                        AITimer = 0;
                        NPC.ai[0] = -2;
                    }
                    if (NPC.ai[0] == -2 && ++AITimer > 120)
                        NPC.velocity.Y -= 1;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                }
                return;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }
        private int LegFrameY;
        public int[] ArmFrameY = new int[2];
        public float[] ArmRot = new float[2];
        public int[] ArmRFrameY = new int[2];
        private int[] HandsFrameY = new int[2];
        private int HeadFrameY;
        private readonly int[] HandArmX = new int[] { -18, 0, 6 };
        private readonly int[] HandArmY = new int[] { 8, 0, -14 };
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D armB = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Back").Value;
            Texture2D armF = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Front").Value;
            Texture2D armR = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Rockets").Value;
            Texture2D hands = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Hands").Value;
            Texture2D head = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head").Value;
            Texture2D headGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head_Glow").Value;
            Texture2D legs = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Legs").Value;
            Texture2D thruster = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Gigapora_ThrusterBlue").Value;
            if (NPC.ai[0] >= 4)
            {
                texture = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Overheat").Value;
                armB = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Back_Overheat").Value;
                armF = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Front_Overheat").Value;
                head = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head_Overheat").Value;
                legs = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Legs_Overheat").Value;
            }
            float thrusterScaleX = MathHelper.Lerp(1.5f, 0.5f, -NPC.velocity.Y / 20);
            thrusterScaleX = MathHelper.Clamp(thrusterScaleX, 0.5f, 1.5f);
            float thrusterScaleY = MathHelper.Clamp(-NPC.velocity.Y / 10, 0.3f, 2f);
            Vector2 p = NPC.Center - screenPos;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int armsHeight = armF.Height / 3;
            Rectangle rectArmF = new(0, armsHeight * ArmFrameY[1], armF.Width, armsHeight);
            Rectangle rectArmB = new(0, armsHeight * ArmFrameY[0], armB.Width, armsHeight);
            Vector2 originArms = new(armF.Width / 2f + (-6 * NPC.spriteDirection), armsHeight / 2f - 14);

            int handsHeight = hands.Height / 3;
            Rectangle rectHandF = new(0, handsHeight * HandsFrameY[1], hands.Width / 2, handsHeight);
            Rectangle rectHandB = new(hands.Width / 2, handsHeight * HandsFrameY[0], hands.Width / 2, handsHeight);

            int armRHeight = armR.Height / 5;
            int armRWidth = armR.Width / 3;
            Rectangle rectArmRB = new(armRWidth * ArmFrameY[0], armRHeight * ArmRFrameY[0], armRWidth, armRHeight);
            Rectangle rectArmRF = new(armRWidth * ArmFrameY[1], armRHeight * ArmRFrameY[1], armRWidth, armRHeight);

            int headHeight = head.Height / 3;
            Rectangle rectHead = new(0, headHeight * HeadFrameY, head.Width, headHeight);

            int legsHeight = legs.Height / 5;
            Rectangle rectLegs = new(0, legsHeight * LegFrameY, legs.Width, legsHeight);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Color glowColor = RedeColor.VlitchGlowColour * 0.7f;
                    if (NPC.frame.Y <= 0)
                    {
                        spriteBatch.Draw(armB, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmB), glowColor, ArmRot[0], originArms, NPC.scale, effects, 0);
                        spriteBatch.Draw(hands, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectHandB), glowColor, ArmRot[0], originArms - new Vector2((NPC.spriteDirection == -1 ? 29 : 12) + (HandArmX[ArmFrameY[0]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[0]]), NPC.scale, effects, 0);
                    }
                    spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, glowColor, oldrot[i], NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                    if (NPC.frame.Y <= 0)
                        spriteBatch.Draw(head, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), glowColor, oldrot[i], NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
                    spriteBatch.Draw(legs, NPC.oldPos[i] + NPC.Size / 2f - screenPos, new Rectangle?(rectLegs), glowColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(22, -78), NPC.scale, effects, 0);
                    if (NPC.frame.Y <= 0)
                    {
                        spriteBatch.Draw(armF, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmF), glowColor, ArmRot[1], originArms, NPC.scale, effects, 0);
                        spriteBatch.Draw(hands, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectHandF), glowColor, ArmRot[1], originArms - new Vector2((NPC.spriteDirection == -1 ? 28 : 13) + (HandArmX[ArmFrameY[1]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[1]]), NPC.scale, effects, 0);
                    }
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            if (NPC.frame.Y <= 0)
            {
                // Back Arm
                spriteBatch.Draw(armB, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmB), NPC.GetAlpha(drawColor), ArmRot[0], originArms, NPC.scale, effects, 0);
                // Back Hand
                spriteBatch.Draw(hands, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectHandB), NPC.GetAlpha(drawColor), ArmRot[0], originArms - new Vector2((NPC.spriteDirection == -1 ? 29 : 12) + (HandArmX[ArmFrameY[0]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[0]]), NPC.scale, effects, 0);
                // Rockets Back
                spriteBatch.Draw(armR, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmRB), NPC.GetAlpha(drawColor), ArmRot[0], originArms - new Vector2(NPC.spriteDirection == -1 ? -3 : 14, 0), NPC.scale, effects, 0);
            }
            // Body

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 thrusterOrigin = new(thruster.Width / 2f, thruster.Height / 2f - 20);
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                spriteBatch.Draw(thruster, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(NPC.spriteDirection == 1 ? -44 : 0, NPC.rotation) + RedeHelper.PolarVector(4, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(-NPC.velocity.Y / 20, 0, 1), oldrot[i], thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
            }
            spriteBatch.Draw(thruster, p + RedeHelper.PolarVector(30, NPC.rotation) + RedeHelper.PolarVector(NPC.spriteDirection == 1 ? -44 : 0, NPC.rotation) + RedeHelper.PolarVector(4, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(-NPC.velocity.Y / 20, 0, 1), NPC.rotation - MathHelper.PiOver2, thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            spriteBatch.Draw(glow, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (NPC.frame.Y <= 0)
            {
                // Head
                spriteBatch.Draw(head, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
                spriteBatch.Draw(headGlow, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
            }
            // Legs
            spriteBatch.Draw(legs, p, new Rectangle?(rectLegs), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(22, -78), NPC.scale, effects, 0);
            if (NPC.frame.Y <= 0)
            {
                // Front Arm
                spriteBatch.Draw(armF, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmF), NPC.GetAlpha(drawColor), ArmRot[1], originArms, NPC.scale, effects, 0);
                spriteBatch.Draw(hands, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectHandF), NPC.GetAlpha(drawColor), ArmRot[1], originArms - new Vector2((NPC.spriteDirection == -1 ? 28 : 13) + (HandArmX[ArmFrameY[1]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[1]]), NPC.scale, effects, 0);
                spriteBatch.Draw(armR, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmRF), NPC.GetAlpha(drawColor), ArmRot[1], originArms - new Vector2(NPC.spriteDirection == -1 ? -3 : 14, 0), NPC.scale, effects, 0);
            }
            return false;
        }
    }
}
