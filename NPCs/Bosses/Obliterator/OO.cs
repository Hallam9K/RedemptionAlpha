using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Redemption.BaseExtension;
using System;
using Redemption.Dusts;
using Redemption.NPCs.Bosses.Cleaver;
using ReLogic.Content;
using Redemption.Items.Placeable.Trophies;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Usable;
using Redemption.Items.Materials.HM;
using Redemption.Items.Weapons.PostML.Magic;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Accessories.PostML;
using Redemption.UI.ChatUI;
using Terraria.Localization;
using Redemption.Globals.NPC;
using Redemption.Textures;

namespace Redemption.NPCs.Bosses.Obliterator
{
    [AutoloadBossHead]
    public class OO : ModNPC
    {
        private static Asset<Texture2D> glow;
        private static Asset<Texture2D> armB;
        private static Asset<Texture2D> armF;
        private static Asset<Texture2D> armR;
        private static Asset<Texture2D> hands;
        private static Asset<Texture2D> head;
        private static Asset<Texture2D> headGlow;
        private static Asset<Texture2D> legs;
        private static Asset<Texture2D> thruster;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glow = ModContent.Request<Texture2D>(Texture + "_Glow");
            armB = ModContent.Request<Texture2D>(Texture + "_Arm_Back");
            armF = ModContent.Request<Texture2D>(Texture + "_Arm_Front");
            armR = ModContent.Request<Texture2D>(Texture + "_Arm_Rockets");
            hands = ModContent.Request<Texture2D>(Texture + "_Hands");
            head = ModContent.Request<Texture2D>(Texture + "_Head");
            headGlow = ModContent.Request<Texture2D>(Texture + "_Head_Glow");
            legs = ModContent.Request<Texture2D>(Texture + "_Legs");
            thruster = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Gigapora_ThrusterBlue");
        }
        public override void Unload()
        {
            glow = null;
            armB = null;
            armF = null;
            armR = null;
            hands = null;
            head = null;
            headGlow = null;
            legs = null;
            thruster = null;
        }
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
            // DisplayName.SetDefault("Omega Obliterator");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

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
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Fire] *= 1.15f;
        }
        private static Texture2D Bubble => CommonTextures.TextBubble_Omega.Value;
        private static SoundStyle Voice => RedeBossDowned.downedOmega3 ? CustomSounds.Voice5 with { Pitch = -0.5f } : CustomSounds.Voice5;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.OO"))
            });
        }
        public override void HitEffect(NPC.HitInfo hit)
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
            if (!RedeBossDowned.downedOmega3)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OO_GirusTalk>(), 0, 0, Main.myPlayer);

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedOmega3, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<OmegaOblitBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OmegaObliteratorTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<OORelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<ToasterPet>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<OOMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<BlastBattery>(), ModContent.ItemType<OOFingergun>(), ModContent.ItemType<SunInThePalm>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CorruptedXenomite>(), 1, 16, 28));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OmegaPowerCell>(), 1, 4, 8));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RoboBrain>(), 1, 1, 2));
            npcLoot.Add(notExpertRule);
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.8f;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.velocity.Length() > 12;
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BeamAnimation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BeamAnimation = reader.ReadBoolean();
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
        public bool OverheatOverlay;
        public bool OverheatArmRaise;

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

            if (player.active && !player.dead && (!Main.dayTime || AIState is ActionState.Overheat))
                NPC.DiscourageDespawn(120);
            DespawnHandler();
            Lighting.AddLight(NPC.Center, 0.7f, 0.4f, 0.4f);

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
                            Redemption.grooveTimer = 0;
                            ArmRot[0] = MathHelper.PiOver2 + (NPC.spriteDirection == -1 ? 0 : MathHelper.Pi);
                            NPC.LookAtEntity(player);
                            AITimer++;
                            if (NPC.DistanceSQ(DefaultPos) < 100 * 100 || AITimer > 200)
                            {
                                AITimer = 0;
                                if (RedeBossDowned.oblitDeath == 2 || RedeBossDowned.downedOmega3)
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
                                ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.None, 2400, 4800, 0);
                            }
                            if (AITimer == 120)
                            {
                                HandsFrameY[0] = 0;
                                ArmFrameY[0] = 1;
                                HeadFrameY = 1;

                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.ObliteratorYo, NPC.position);
                                if (!Main.dedServ)
                                {
                                    Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.1"), Colors.RarityRed, Color.DarkRed, CustomSounds.Voice1 with { Volume = 0 }, .03f, 1, 0, false, null, Bubble, modifier: modifier);

                                    ChatUI.Visible = true;
                                    ChatUI.Add(d1);
                                }
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
                                NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaMegaBeam>(), 1000, new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.MegaLaser, NPC.whoAmI);
                            }
                            if (AITimer == 350)
                            {
                                ArmFrameY[0] = 2;
                                BeamAnimation = false;
                            }
                            if (AITimer == 400 && !Main.dedServ)
                            {
                                Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.2"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier);
                                if (RedeBossDowned.oblitDeath == 1)
                                    d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.2Alt"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier);

                                DialogueChain chain = new();
                                chain.Add(d1)
                                     .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.3"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier))
                                     .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.4"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier))
                                     .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.5"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier))
                                     .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.6"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier))
                                     .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.7"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier))
                                     .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Intro.8"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, .5f, true, null, Bubble, modifier: modifier, endID: 1));
                                chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                chain.OnEndTrigger += Chain_OnEndTrigger;
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);
                            }
                            if (AITimer > 3000)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.LabSafeS, NPC.position);
                                for (int i = 0; i < 100; i++)
                                {
                                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, Scale: 1.5f);
                                    Main.dust[dustIndex].velocity *= 1.9f;
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    RedeBossDowned.oblitDeath = 2;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.WorldData);
                                }
                                AIState = ActionState.Begin;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Begin:
                    #region Fight Startup
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega2");

                    NPC.LookAtEntity(player);
                    ArmRot[0] = MathHelper.PiOver2 + (NPC.spriteDirection == -1 ? 0 : MathHelper.Pi);
                    if (AITimer++ == 0 && Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.OO.Name"), 60, 90, 0.8f, 0, Color.Red, Language.GetTextValue("Mods.Redemption.TitleCard.OO.Modifier"));
                    if (AITimer < 60)
                        NPC.Move(DefaultPos2, 9, 10);
                    else
                        NPC.velocity *= 0.96f;
                    if (AITimer == 60)
                    {
                        NPC.Shoot(new Vector2(NPC.Center.X - (120 * 16) - 10, NPC.Center.Y + 8), ModContent.ProjectileType<OOBarrier>(), 0, Vector2.Zero, 0, 1);
                        NPC.Shoot(new Vector2(NPC.Center.X + (120 * 16) + 26, NPC.Center.Y + 8), ModContent.ProjectileType<OOBarrier>(), 0, Vector2.Zero, 0, -1);
                        NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y - (120 * 16) - 10), ModContent.ProjectileType<OOBarrierH>(), 0, Vector2.Zero, 0, 1);
                        NPC.Shoot(new Vector2(NPC.Center.X + 8, NPC.Center.Y + (120 * 16) + 26), ModContent.ProjectileType<OOBarrierH>(), 0, Vector2.Zero, 0, -1);

                        ArenaWorld.arenaBoss = "OO";
                        ArenaWorld.arenaTopLeft = new Vector2(NPC.Center.X - (120 * 16) + 8, NPC.Center.Y - (120 * 16) + 8);
                        ArenaWorld.arenaSize = new Vector2(240 * 16, 240 * 16);
                        ArenaWorld.arenaMiddle = NPC.Center;
                        ArenaWorld.arenaActive = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);

                        ArmFrameY[0] = 1;
                        HandsFrameY[0] = 1;

                        if (!Main.dedServ)
                        {
                            string s = Language.GetTextValue("Mods.Redemption.Cutscene.OO.Ready");
                            if (RedeBossDowned.downedOmega3)
                                s = Language.GetTextValue("Mods.Redemption.Cutscene.OO.DownedReady");

                            Dialogue d1 = new(NPC, s, Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, .5f, true, null, Bubble, modifier: modifier); // 176

                            ChatUI.Visible = true;
                            ChatUI.Add(d1);
                        }
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
                                    if (!Main.dedServ)
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
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.OODashReady, NPC.position);
                                    NPC.velocity.X = 8 * NPC.RightOfDir(player);
                                }

                                if (AITimer == 235)
                                    Dash(60 + SpeedBoost, false);

                                if (AITimer > 235 && AITimer % 3 == 0 && NPC.velocity.Length() > 12)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, 12f), SoundID.Item91);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, -12f), SoundID.Item91);
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
                                            NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, RedeHelper.PolarVector((i == 1 ? 25 : 20) + SpeedBoost, (NPC.spriteDirection == -1 ? MathHelper.Pi : 0) + MathHelper.ToRadians(rot - 25)), CustomSounds.BallCreate);
                                        }
                                    }
                                    else
                                        NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, new Vector2((16 + SpeedBoost) * NPC.spriteDirection, 0), CustomSounds.BallCreate);
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
                                    NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, new Vector2((12 + SpeedBoost) * NPC.spriteDirection, 0), CustomSounds.BallCreate);

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
                                        NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-600, 600), player.Center.Y + Main.rand.Next(-600, 600)) + (player.velocity * 20), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, CustomSounds.Alarm2, NPC.whoAmI);
                                }
                                else
                                {
                                    if (AITimer > 200 && AITimer % (NPC.DistanceSQ(player.Center) >= 900 * 900 ? 4 : 6) == 0 && AITimer < 320)
                                        NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-600, 600), player.Center.Y + Main.rand.Next(-600, 600)) + (player.velocity * 20), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, CustomSounds.Alarm2, NPC.whoAmI);
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
                                    NPC.Shoot(HandPos, ModContent.ProjectileType<OO_Fingerflash>(), 150, Vector2.Zero, NPC.whoAmI);
                            }
                            else
                            {
                                if (AITimer % 15 == 0 && AITimer < 300)
                                    NPC.Shoot(HandPos, ModContent.ProjectileType<OO_Fingerflash>(), 150, Vector2.Zero, NPC.whoAmI);
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
                                    if (!RedeBossDowned.downedOmega3 && !Main.dedServ)
                                    {
                                        Dialogue d1 = new(NPC, "Eye beam!", Colors.RarityRed, Color.DarkRed, Voice, .03f, 1.16f, .5f, true, null, Bubble, modifier: modifier);
                                        ChatUI.Visible = true;
                                        ChatUI.Add(d1);
                                    }

                                    for (int i = 0; i < 3; i++)
                                        NPC.Shoot(EyePos, ModContent.ProjectileType<OO_NormalBeam>(), 180, new Vector2(1 * NPC.spriteDirection, 0), CustomSounds.Laser1, NPC.whoAmI, i);
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
                                    NPC.Shoot(EyePos, ModContent.ProjectileType<OO_StunBeam>(), 100, new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.BallFire, NPC.whoAmI);
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
                                    NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-80, 80), player.Center.Y + Main.rand.Next(-80, 80)), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, CustomSounds.Alarm2, NPC.whoAmI);

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
                                        NPC.Shoot(LaserPos, ModContent.ProjectileType<OO_NormalBeam>(), 180, new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.Laser1, NPC.whoAmI, i + 3);

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
                        if (OverheatArmRaise && !RedeBossDowned.downedOmega3)
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
                    if ((TimerRand == 1 && OverheatOverlay) || TimerRand > 1)
                    {
                        player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(3, player.RedemptionScreen().ScreenShakeIntensity);
                        Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.5f).UseIntensity(0.6f).UseColor(Color.DarkRed).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                        player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
                    }
                    if (TimerRand > 1 || (TimerRand == 7 && AITimer < 3000))
                    {
                        NPC.life -= NPC.lifeMax / 1200;
                        NPC.life = (int)MathHelper.Max(NPC.life, 1);
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
                                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 2000, 5000, 0);
                                }
                                if (AITimer == 60 && !Main.dedServ)
                                {
                                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/OmegaOverheat");

                                    if (!RedeBossDowned.downedOmega3)
                                    {
                                        DialogueChain chain = new();
                                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.1"), Colors.RarityRed, Color.DarkRed, Voice with { Pitch = -0.5f }, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 136
                                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.2"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 204
                                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.3"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 238
                                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.4"), Colors.RarityRed, Color.DarkRed, Voice with { Pitch = -0.5f }, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 218
                                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.5"), Colors.RarityRed, Color.DarkRed, Voice with { Pitch = 0.1f, PitchVariance = 0.1f }, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 156
                                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.6"), Colors.RarityRed, Color.DarkRed, Voice with { Pitch = 0.3f, PitchVariance = 0.3f }, .03f, 2f, .5f, true, null, Bubble, modifier: modifier, endID: 1)); // 204
                                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                        chain.OnEndTrigger += Chain_OnEndTrigger;
                                        ChatUI.Visible = true;
                                        ChatUI.Add(chain);
                                    }
                                    else
                                    {
                                        DialogueChain chain = new();
                                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.D1"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 136
                                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.D2"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier, endID: 1)); // 218
                                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                        chain.OnEndTrigger += Chain_OnEndTrigger;
                                        ChatUI.Visible = true;
                                        ChatUI.Add(chain);
                                    }
                                }
                                if (AITimer > 3000)
                                {
                                    HeadFrameY = 0;
                                    AITimer = 0;
                                    TimerRand = 2;
                                    NPC.netUpdate = true;
                                }
                                if (OverheatOverlay)
                                {
                                    NPC.life += (int)(NPC.lifeMax / 120f);
                                    if (NPC.life > NPC.lifeMax)
                                        NPC.life = NPC.lifeMax;
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
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.OODashReady, NPC.position);
                                    NPC.velocity.X = 8 * NPC.RightOfDir(player);
                                }

                                if (AITimer == 225)
                                    Dash(60 + SpeedBoost, false);

                                if (AITimer > 225 && AITimer % 3 == 0 && NPC.velocity.Length() > 12)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, 12f), SoundID.Item91);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<OmegaBlast>(), 140, new Vector2(-4f * NPC.spriteDirection, -12f), SoundID.Item91);
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
                                NPC.Shoot(HandPos, ModContent.ProjectileType<OO_Fingerflash>(), 150, Vector2.Zero, NPC.whoAmI);

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
                                if (AITimer == 305 && !RedeBossDowned.downedOmega3 && !Main.dedServ)
                                {
                                    Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.7"), Colors.RarityRed, Color.DarkRed, Voice with { Pitch = 0.3f, PitchVariance = 0.3f }, .01f, 1.16f, .5f, true, null, Bubble, modifier: modifier);
                                    ChatUI.Visible = true;
                                    ChatUI.Add(d1);
                                }
                                if (AITimer >= 305 && AITimer % 4 == 0 && AITimer <= 355)
                                    NPC.Shoot(EyePos, ModContent.ProjectileType<OO_NormalBeam>(), 180, new Vector2(1 * NPC.spriteDirection, Main.rand.NextFloat(-0.25f, 0.25f)), CustomSounds.Laser1, NPC.whoAmI, -1);

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
                                    NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-900, 900), player.Center.Y + Main.rand.Next(-900, 900)), ModContent.ProjectileType<OO_Crosshair>(), 160, Vector2.Zero, CustomSounds.Alarm2, NPC.whoAmI);

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
                                    NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaPlasmaBall>(), 130, new Vector2((12 + SpeedBoost) * NPC.spriteDirection, 0), CustomSounds.BallCreate);

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
                                    if (!Main.dedServ)
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
                                    NPC.Shoot(LaserPos, ModContent.ProjectileType<OmegaMegaBeam>(), 1000, new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.MegaLaser, NPC.whoAmI);
                                }
                                if (AITimer == 370)
                                    BeamAnimation = false;

                                if (AITimer >= 420)
                                {
                                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.High, 0, 0, 0);
                                }
                                if (AITimer == 450)
                                {
                                    SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                                    RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, DustID.LifeDrain, tex: ModContent.Request<Texture2D>("Redemption/Empty").Value);
                                    if (!Main.dedServ)
                                    {
                                        if (!RedeBossDowned.downedOmega3)
                                        {
                                            DialogueChain chain = new();
                                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.8"), Colors.RarityRed, Color.DarkRed, Voice with { Pitch = -0.5f }, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 228
                                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.9"), Colors.RarityRed, Color.DarkRed, Voice with { Pitch = 0.3f, PitchVariance = 0.3f }, .05f, .05f, 0, false, null, Bubble, modifier: modifier)); // 124
                                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                            ChatUI.Visible = true;
                                            ChatUI.Add(chain);
                                        }
                                        else
                                        {
                                            DialogueChain chain = new();
                                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Desperation.8"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier, endID: 1)); // 228
                                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                            chain.OnEndTrigger += Chain_OnEndTrigger;
                                            ChatUI.Visible = true;
                                            ChatUI.Add(chain);
                                        }
                                    }
                                }
                                if (AITimer > 3000)
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
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;
            NPC.rotation = NPC.velocity.X * 0.01f;
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            Player player = Main.player[NPC.target];
            switch (signature)
            {
                case "a":
                    HeadFrameY = 2;
                    break;
                case "b":
                    HeadFrameY = 0;
                    break;
                case "c":
                    AITimer = 3000;
                    break;
                case "d":
                    SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                    RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, DustID.LifeDrain, tex: ModContent.Request<Texture2D>("Redemption/Empty").Value);
                    break;
                case "e":
                    OverheatOverlay = true;
                    OverheatArmRaise = true;
                    if (!RedeBossDowned.downedOmega3)
                    {
                        player.RedemptionScreen().TimedZoom(new Vector2(1.2f, 1.2f), 80, 280);
                        HeadFrameY = 2;
                    }
                    break;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 3000;
            OverheatArmRaise = false;
        }
        public override void FindFrame(int frameHeight)
        {
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
                NPC.velocity.X = speed * player.RightOfDir(NPC);
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
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
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        RedeBossDowned.oblitDeath = 1;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                    AITimer = 0;
                    TimerRand = 0;
                    NPC.ai[0] = -1;
                }
                else if (NPC.ai[0] == -1)
                {
                    if (AITimer++ == 100 && !Main.dedServ)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Success.1"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, 0, false, null, Bubble, modifier: modifier)) // 154
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Success.2"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, .5f, true, null, Bubble, modifier: modifier)); // 170

                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
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
                        if (!Main.dedServ)
                        {
                            Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.OO.Success.D1"), Colors.RarityRed, Color.DarkRed, Voice, .03f, 2f, .5f, true, null, Bubble, modifier: modifier); // 150

                            ChatUI.Visible = true;
                            ChatUI.Add(d1);
                        }
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
            if (Music == MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega2"))
                Redemption.grooveTimer++;
            else
                Redemption.grooveTimer = 0;
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
        private readonly int[] HandsFrameY = new int[2];
        private int HeadFrameY;
        private readonly int[] HandArmX = new int[] { -18, 0, 6 };
        private readonly int[] HandArmY = new int[] { 8, 0, -14 };
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            if (NPC.ai[0] >= 4)
            {
                texture = ModContent.Request<Texture2D>(Texture + "_Overheat").Value;
                armB = ModContent.Request<Texture2D>(Texture + "_Arm_Back_Overheat");
                armF = ModContent.Request<Texture2D>(Texture + "_Arm_Front_Overheat");
                head = ModContent.Request<Texture2D>(Texture + "_Head_Overheat");
                legs = ModContent.Request<Texture2D>(Texture + "_Legs_Overheat");
            }
            float thrusterScaleX = MathHelper.Lerp(1.5f, 0.5f, -NPC.velocity.Y / 20);
            thrusterScaleX = MathHelper.Clamp(thrusterScaleX, 0.5f, 1.5f);
            float thrusterScaleY = MathHelper.Clamp(-NPC.velocity.Y / 10, 0.3f, 2f);
            Vector2 p = NPC.Center - screenPos;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int armsHeight = armF.Value.Height / 3;
            Rectangle rectArmF = new(0, armsHeight * ArmFrameY[1], armF.Value.Width, armsHeight);
            Rectangle rectArmB = new(0, armsHeight * ArmFrameY[0], armB.Value.Width, armsHeight);
            Vector2 originArms = new(armF.Value.Width / 2f + (-6 * NPC.spriteDirection), armsHeight / 2f - 14);

            int handsHeight = hands.Value.Height / 3;
            Rectangle rectHandF = new(0, handsHeight * HandsFrameY[1], hands.Value.Width / 2, handsHeight);
            Rectangle rectHandB = new(hands.Value.Width / 2, handsHeight * HandsFrameY[0], hands.Value.Width / 2, handsHeight);

            int armRHeight = armR.Value.Height / 5;
            int armRWidth = armR.Value.Width / 3;
            Rectangle rectArmRB = new(armRWidth * ArmFrameY[0], armRHeight * ArmRFrameY[0], armRWidth, armRHeight);
            Rectangle rectArmRF = new(armRWidth * ArmFrameY[1], armRHeight * ArmRFrameY[1], armRWidth, armRHeight);

            int headHeight = head.Value.Height / 3;
            Rectangle rectHead = new(0, headHeight * HeadFrameY, head.Value.Width, headHeight);

            int legsHeight = legs.Value.Height / 5;
            Rectangle rectLegs = new(0, legsHeight * LegFrameY, legs.Value.Width, legsHeight);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive();

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Color glowColor = RedeColor.VlitchGlowColour * 0.7f;
                    if (NPC.frame.Y <= 0)
                    {
                        spriteBatch.Draw(armB.Value, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmB), glowColor, ArmRot[0], originArms, NPC.scale, effects, 0);
                        spriteBatch.Draw(hands.Value, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectHandB), glowColor, ArmRot[0], originArms - new Vector2((NPC.spriteDirection == -1 ? 29 : 12) + (HandArmX[ArmFrameY[0]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[0]]), NPC.scale, effects, 0);
                    }
                    spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, glowColor, oldrot[i], NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                    if (NPC.frame.Y <= 0)
                        spriteBatch.Draw(head.Value, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), glowColor, oldrot[i], NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
                    spriteBatch.Draw(legs.Value, NPC.oldPos[i] + NPC.Size / 2f - screenPos, new Rectangle?(rectLegs), glowColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(22, -78), NPC.scale, effects, 0);
                    if (NPC.frame.Y <= 0)
                    {
                        spriteBatch.Draw(armF.Value, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmF), glowColor, ArmRot[1], originArms, NPC.scale, effects, 0);
                        spriteBatch.Draw(hands.Value, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectHandF), glowColor, ArmRot[1], originArms - new Vector2((NPC.spriteDirection == -1 ? 28 : 13) + (HandArmX[ArmFrameY[1]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[1]]), NPC.scale, effects, 0);
                    }
                }
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }

            if (NPC.frame.Y <= 0)
            {
                // Back Arm
                spriteBatch.Draw(armB.Value, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmB), NPC.GetAlpha(drawColor), ArmRot[0], originArms, NPC.scale, effects, 0);
                // Back Hand
                spriteBatch.Draw(hands.Value, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectHandB), NPC.GetAlpha(drawColor), ArmRot[0], originArms - new Vector2((NPC.spriteDirection == -1 ? 29 : 12) + (HandArmX[ArmFrameY[0]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[0]]), NPC.scale, effects, 0);
                // Rockets Back
                spriteBatch.Draw(armR.Value, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmRB), NPC.GetAlpha(drawColor), ArmRot[0], originArms - new Vector2(NPC.spriteDirection == -1 ? -3 : 14, 0), NPC.scale, effects, 0);
            }
            // Body

            spriteBatch.End();
            spriteBatch.BeginAdditive();

            Vector2 thrusterOrigin = new(thruster.Value.Width / 2f, thruster.Value.Height / 2f - 20);
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                spriteBatch.Draw(thruster.Value, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(NPC.spriteDirection == 1 ? -44 : 0, NPC.rotation) + RedeHelper.PolarVector(4, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(-NPC.velocity.Y / 20, 0, 1), oldrot[i], thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
            }
            spriteBatch.Draw(thruster.Value, p + RedeHelper.PolarVector(30, NPC.rotation) + RedeHelper.PolarVector(NPC.spriteDirection == 1 ? -44 : 0, NPC.rotation) + RedeHelper.PolarVector(4, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(-NPC.velocity.Y / 20, 0, 1), NPC.rotation - MathHelper.PiOver2, thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();

            spriteBatch.Draw(texture, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            spriteBatch.Draw(glow.Value, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (NPC.frame.Y <= 0)
            {
                // Head
                spriteBatch.Draw(head.Value, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
                spriteBatch.Draw(headGlow.Value, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
            }
            // Legs
            spriteBatch.Draw(legs.Value, p, new Rectangle?(rectLegs), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(22, -78), NPC.scale, effects, 0);
            if (NPC.frame.Y <= 0)
            {
                // Front Arm
                spriteBatch.Draw(armF.Value, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmF), NPC.GetAlpha(drawColor), ArmRot[1], originArms, NPC.scale, effects, 0);
                spriteBatch.Draw(hands.Value, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectHandF), NPC.GetAlpha(drawColor), ArmRot[1], originArms - new Vector2((NPC.spriteDirection == -1 ? 28 : 13) + (HandArmX[ArmFrameY[1]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[1]]), NPC.scale, effects, 0);
                spriteBatch.Draw(armR.Value, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmRF), NPC.GetAlpha(drawColor), ArmRot[1], originArms - new Vector2(NPC.spriteDirection == -1 ? -3 : 14, 0), NPC.scale, effects, 0);
            }
            return false;
        }
    }
}
