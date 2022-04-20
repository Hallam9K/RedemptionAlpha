using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Usable;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.Audio;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Accessories.PreHM;
using Redemption.BaseExtension;
using Terraria.GameContent.UI;
using Redemption.UI;

namespace Redemption.NPCs.Bosses.Erhan
{
    [AutoloadBossHead]
    public class Erhan : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Fallen,
            Death,
            Bible,
            BibleAttacks
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erhan, Anglonic High Priest");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 36),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 2600;
            NPC.damage = 21;
            NPC.defense = 6;
            NPC.knockBackResist = 0f;
            NPC.width = 34;
            NPC.height = 60;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 1, 25, 0);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false ? null : false;
        public override bool CheckActive()
        {
            return AIState != ActionState.Death;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0 && !Spared && AIState == ActionState.Death)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 30; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GoldFlame, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                for (int i = 0; i < 30; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/ErhanGore1").Type, 1);
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/ErhanGore2").Type, 1);
                for (int i = 0; i < 10; i++)
                    Gore.NewGore(NPC.position, RedeHelper.SpreadUp(8), ModContent.Find<ModGore>("Redemption/ErhanGore3").Type, 1);

            }
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("A high priest of Fairwood, tasked himself to purify the forest's curse after it consumed it's warden.")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ErhanBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ErhanTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ErhanRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<DevilsAdvocate>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ErhanHelmet>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<Bindeklinge>(), ModContent.ItemType<HolyBible>(), ModContent.ItemType<HallowedHandGrenade>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ErhanMagnifyingGlass>(), 2));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!Spared && RedeBossDowned.erhanDeath < 3)
                RedeBossDowned.erhanDeath = 3;

            if (!RedeBossDowned.downedErhan)
            {
                string fight = Spared ? "fighting" : "slaying";
                RedeWorld.alignment -= Spared ? 1 : 2;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, Spared ? "-1" : "-2", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Attempting to summon a demon and " + fight + " a priest... Are you alright in the head?", 240, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedErhan, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(ID);
                writer.Write(AttackNumber);
                writer.Write(TimerRand2);
                writer.Write(Spared);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ID = reader.ReadInt32();
                AttackNumber = reader.ReadInt32();
                TimerRand2 = reader.ReadInt32();
                Spared = reader.ReadBoolean();
            }
        }

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

                if (ID == 4 && AttackNumber <= 5)
                    continue;

                AttackNumber++;
                attempts++;
            }
        }

        public List<int> AttackList = new() { 0, 1, 2, 3, 4 };
        public List<int> CopyList = null;

        private float move;
        private float speed = 6;
        private int AttackNumber;
        private bool floatTimer;
        private float TimerRand2;
        private bool Spared;
        private Vector2 playerOrigin;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            Vector2 text = new Vector2(NPC.Center.X, NPC.position.Y - 140) - Main.screenPosition;
            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextPos = text;
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            DespawnHandler();

            if (AIState is not ActionState.Fallen && AIState is not ActionState.Death && AIState is not ActionState.Bible)
            {
                NPC.LookAtEntity(player);
                if (!floatTimer)
                {
                    NPC.velocity.Y += 0.03f;
                    if (NPC.velocity.Y > .5f)
                    {
                        floatTimer = true;
                        NPC.netUpdate = true;
                    }
                }
                else if (floatTimer)
                {
                    NPC.velocity.Y -= 0.03f;
                    if (NPC.velocity.Y < -.5f)
                    {
                        floatTimer = false;
                        NPC.netUpdate = true;
                    }
                }
            }

            switch (AIState)
            {
                case ActionState.Begin:
                    switch (TimerRand)
                    {
                        case 0:
                            if (!Main.dedServ)
                                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                            SoundEngine.PlaySound(SoundID.Item68, NPC.position);
                            player.RedemptionScreen().ScreenShakeIntensity = 14;
                            HolyFlare = true;
                            TeleGlow = true;
                            TimerRand = 1;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            if (RedeBossDowned.erhanDeath <= 0)
                            {
                                if (!Main.dedServ)
                                {
                                    if (AITimer++ == 0)
                                    {
                                        ArmType = 2;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Great heavens!!", 120, 1, 0.6f, "Erhan:", 2f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    }
                                    if (AITimer >= 120)
                                    {
                                        player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                        player.RedemptionScreen().lockScreen = true;
                                        NPC.LockMoveRadius(player);
                                    }
                                    if (AITimer == 120)
                                    {
                                        ArmType = 0;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Doth thine brain be stuck in a well!?", 240, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    }
                                    if (AITimer == 360)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("To summon a demon, so close to my land... 'Tis heresy!", 240, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, null, NPC.Center, sound: true);
                                    if (AITimer == 600)
                                    {
                                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 200);
                                        ArmType = 2;
                                        HeadFrameY = 1;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Repent! Repent for thy sins!", 200, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    }
                                    if (AITimer == 800)
                                    {
                                        ArmType = 0;
                                        HeadFrameY = 0;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Lest I smack'eth thine buttocks with the Hand of Judgement!", 240, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    }
                                }
                                if (AITimer >= 1040)
                                {
                                    if (!Main.dedServ)
                                    {
                                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Erhan", 60, 90, 0.8f, 0, Color.Goldenrod,
                                            "Anglonic High Priest");
                                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
                                    }
                                    if (RedeBossDowned.erhanDeath == 0)
                                        RedeBossDowned.erhanDeath = 1;

                                    TimerRand = 0;
                                    AITimer = 0;
                                    NPC.dontTakeDamage = false;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                }
                            }
                            else
                            {
                                if (AITimer++ == 0 && !Main.dedServ)
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("CEASE!", 120, 1, 0.6f, "Erhan:", 2f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);

                                if (AITimer >= 120)
                                {
                                    if (!Main.dedServ)
                                    {
                                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Erhan", 60, 90, 0.8f, 0, Color.Goldenrod,
                                            "Anglonic High Priest");
                                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
                                    }
                                    TimerRand = 0;
                                    AITimer = 0;
                                    NPC.dontTakeDamage = false;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                }
                            }
                            break;
                    }
                    break;
                case ActionState.Idle:
                    if (AITimer++ == 0)
                    {
                        move = NPC.Center.X;
                        speed = 9;
                    }
                    NPC.Move(new Vector2(move, player.Center.Y - 250), speed, 50, false);
                    MoveClamp();
                    if (NPC.DistanceSQ(player.Center) > 800 * 800)
                        speed *= 1.03f;
                    else if (NPC.velocity.Length() > 9 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                        speed *= 0.96f;

                    if (AITimer > 80)
                    {
                        if (AttackNumber != 0 && AttackNumber % 5 == 0)
                        {
                            if (Main.expertMode)
                            {
                                TimerRand = 0;
                                AIState = ActionState.Bible;
                            }
                            else
                                AIState = ActionState.Fallen;
                        }
                        else
                        {
                            AttackChoice();
                            AIState = ActionState.Attacks;
                        }
                        AITimer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Lightmass
                        case 0:
                            AITimer++;
                            if (AITimer < 60)
                                NPC.Move(new Vector2(player.Center.X + (40 * NPC.spriteDirection), player.Center.Y - 250), 10, 40, false);
                            else
                                NPC.velocity *= 0.96f;

                            if (AITimer == 80)
                                ArmType = 1;
                            if (AttackNumber > 5)
                            {
                                if (AITimer >= 100 && AITimer % (AttackNumber > 10 ? 8 : 12) == 0 && AITimer <= 130)
                                {
                                    TeleGlow = true;
                                    TeleGlowTimer = 0;
                                    for (int i = 0; i < Main.rand.Next(4, 7); i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Erhan_Lightmass>(), NPC.damage,
                                            new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), false, SoundID.Item101);
                                }
                            }
                            else
                            {
                                if (AITimer == 100 || (Main.rand.NextBool(2) ? AITimer == 120 : AITimer == -1))
                                {
                                    TeleGlow = true;
                                    TeleGlowTimer = 0;
                                    for (int i = 0; i < Main.rand.Next(4, 7); i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Erhan_Lightmass>(), NPC.damage,
                                            new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), false, SoundID.Item101);
                                }
                            }
                            if (AITimer == 140)
                                ArmType = 0;

                            if (AITimer >= 200)
                            {
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Scorching Rays
                        case 1:
                            if (AITimer++ == 0)
                            {
                                move = NPC.Center.X;
                                speed = 7;
                            }
                            NPC.Move(new Vector2(move, player.Center.Y - 250), speed, 50, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 800 * 800)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 9 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                                speed *= 0.96f;

                            if (AITimer == 20)
                            {
                                HeadFrameY = 1;
                                ArmType = 2;
                            }
                            if (AITimer == 40)
                            {
                                NPC.Shoot(new Vector2(player.Center.X, player.Center.Y - 600),
                                    ModContent.ProjectileType<ScorchingRay>(), (int)(NPC.damage * 1.5f),
                                    new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            if (AITimer >= 70 && AITimer % 30 == 0 && AITimer <= 220)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-600, 600), player.Center.Y - 600),
                                    ModContent.ProjectileType<ScorchingRay>(), (int)(NPC.damage * 1.5f),
                                    new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            if (AttackNumber > 5 && AITimer >= 80 && AITimer % 80 == 0 && AITimer <= 360)
                            {
                                TeleGlow = true;
                                TeleGlowTimer = 0;
                                for (int i = 0; i < Main.rand.Next(4, 7); i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Erhan_Lightmass>(), NPC.damage,
                                        new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), false, SoundID.Item101);
                            }
                            if (AITimer == 340)
                            {
                                HeadFrameY = 0;
                                ArmType = 0;
                            }

                            if (AITimer >= 350)
                            {
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Holy Spears
                        case 2:
                            AITimer++;
                            if (AITimer < 80)
                                NPC.Move(new Vector2(player.Center.X + (40 * NPC.spriteDirection), player.Center.Y - 250), 10, 40, false);
                            else
                                NPC.velocity *= 0.5f;

                            if (AITimer == 80)
                                ArmType = 1;
                            if (AITimer >= 90 && AITimer % 5 == 0 && AITimer <= 130)
                            {
                                player.RedemptionScreen().ScreenShakeIntensity = 4;
                                TimerRand += (float)Math.PI / 15;
                                if (TimerRand > (float)Math.PI)
                                {
                                    TimerRand -= (float)Math.PI * 2;
                                }
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Proj>(), NPC.damage,
                                    new Vector2(0.1f, 0).RotatedBy(TimerRand + Math.PI / 2), false, SoundID.Item125);
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Proj>(), NPC.damage,
                                    new Vector2(0.1f, 0).RotatedBy(-TimerRand + Math.PI / 2), false, SoundID.Item125);

                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Tele>(), 0,
                                    new Vector2(0.1f, 0).RotatedBy(TimerRand + Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Tele>(), 0,
                                    new Vector2(0.1f, 0).RotatedBy(-TimerRand + Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                            }
                            if (AttackNumber > 5)
                            {
                                if (AITimer > 130 && AITimer % 5 == 0 && AITimer <= 165)
                                {
                                    player.RedemptionScreen().ScreenShakeIntensity = 4;
                                    TimerRand -= (float)Math.PI / 13;
                                    if (TimerRand > (float)Math.PI)
                                    {
                                        TimerRand -= (float)Math.PI * 2;
                                    }
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Proj>(), NPC.damage,
                                        new Vector2(0.1f, 0).RotatedBy(TimerRand + Math.PI / 2), false, SoundID.Item125, "", 1);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Proj>(), NPC.damage,
                                        new Vector2(0.1f, 0).RotatedBy(-TimerRand + Math.PI / 2), false, SoundID.Item125, "", 1);

                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Tele>(), 0,
                                        new Vector2(0.1f, 0).RotatedBy(TimerRand + Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Tele>(), 0,
                                        new Vector2(0.1f, 0).RotatedBy(-TimerRand + Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                                }
                            }
                            if (AttackNumber > 5 ? AITimer == 180 : AITimer == 150)
                                ArmType = 0;

                            if (AttackNumber > 5 ? AITimer >= 200 : AITimer >= 160)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Holy Phalanx
                        case 3:
                            AITimer++;
                            if (AITimer < 80)
                                NPC.Move(new Vector2(player.Center.X + (40 * NPC.spriteDirection), player.Center.Y - 270), 10, 40, false);
                            else
                                NPC.velocity *= 0.5f;

                            if (AITimer == 80)
                                ArmType = 1;
                            if (AITimer == 80)
                            {
                                for (int i = 0; i < 2; i++)
                                    NPC.Shoot(new Vector2(player.Center.X + 600 * (i == 0 ? -1 : 1), player.Center.Y - 600), ModContent.ProjectileType<ScorchingRay>(), (int)(NPC.damage * 1.5f), new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            if (AITimer >= 90 && AITimer % 7 == 0 && AITimer <= 130 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slice3").WithPitchVariance(0.1f), NPC.position);
                                SoundEngine.PlaySound(SoundID.Item125, NPC.Center);
                                int p = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HolyPhalanx_Proj>(), NPC.damage / 4, 3, Main.myPlayer, NPC.whoAmI, TimerRand * 60);
                                Main.projectile[p].localAI[0] += TimerRand * 7;
                                TimerRand++;
                            }
                            if (AttackNumber >= 5 ? AITimer == 130 : AITimer == 200)
                                ArmType = 0;

                            if (AttackNumber >= 5 ? AITimer == 150 : AITimer >= 220)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Ray of Guidance
                        case 4:
                            if (AITimer++ == 0)
                            {
                                move = NPC.Center.X;
                                speed = 7;
                            }
                            NPC.Move(new Vector2(move, player.Center.Y - 250), speed, 50, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 800 * 800)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 9 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                                speed *= 0.96f;

                            if (AITimer == 20)
                            {
                                HeadFrameY = 1;
                                ArmType = 2;
                            }
                            if (AITimer == 40)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + (Main.rand.NextBool(2) ? 300 : -300), player.Center.Y - 800),
                                    ModContent.ProjectileType<RayOfGuidance>(), (int)(NPC.damage * 2f),
                                    Vector2.Zero, false, SoundID.Item162);
                            }
                            if (AttackNumber > 7)
                            {
                                if (AITimer >= 60 && AITimer % 60 == 0 && AITimer <= 360)
                                {
                                    NPC.Shoot(new Vector2(player.Center.X + (Main.rand.Next(600, 900) *
                                        (Main.rand.NextBool() ? -1 : 1)), player.Center.Y - 600), ModContent.ProjectileType<ScorchingRay>(),
                                        (int)(NPC.damage * 1.5f), new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                                }
                            }
                            if (AITimer >= 80 && AITimer % 80 == 0 && AITimer <= 360)
                            {
                                TeleGlow = true;
                                TeleGlowTimer = 0;
                                for (int i = 0; i < Main.rand.Next(4, 7); i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Erhan_Lightmass>(), NPC.damage,
                                        new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), false, SoundID.Item101);
                            }
                            if (AITimer == 420)
                            {
                                HeadFrameY = 0;
                                ArmType = 0;
                            }

                            if (AITimer >= 440)
                            {
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Bible:
                    switch (TimerRand)
                    {
                        case 0:
                            if (AITimer++ == 0)
                            {
                                move = NPC.Center.X;
                                speed = 9;
                            }
                            if (AITimer < 50)
                            {
                                NPC.Move(new Vector2(move, player.Center.Y - 250), speed, 50, false);
                                MoveClamp();
                                if (NPC.DistanceSQ(player.Center) > 800 * 800)
                                    speed *= 1.03f;
                                else if (NPC.velocity.Length() > 9 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                                    speed *= 0.96f;
                            }
                            else
                                NPC.velocity *= 0.8f;

                            if (AITimer == 60)
                            {
                                ArmType = 3;
                                HeadFrameY = 2;
                                NPC.Shoot(NPC.Center + new Vector2(80 * NPC.spriteDirection, 20), ModContent.ProjectileType<Erhan_Bible>(), NPC.damage, new Vector2(0, -1), true, SoundID.Item1, "Sounds/Custom/Choir", NPC.whoAmI);
                            }
                            if (AITimer == 180)
                            {
                                ArmType = 0;
                                HeadFrameY = 0;
                            }
                            break;
                        case 1:
                            NPC.LookAtEntity(player);
                            if (AITimer++ == 0)
                            {
                                move = NPC.Center.X;
                                speed = 9;
                            }
                            NPC.Move(new Vector2(move, player.Center.Y - 300), speed, 50, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 800 * 800)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 9 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                                speed *= 0.96f;

                            if (AITimer >= 460)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Fallen;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            NPC.LookAtEntity(player);
                            if (AITimer++ == 0)
                            {
                                playerOrigin = player.Center;
                                for (int i = 0; i < 2; i++)
                                    NPC.Shoot(new Vector2(player.Center.X + 800 * (i == 0 ? -1 : 1), player.Center.Y - 600), ModContent.ProjectileType<ScorchingRay>(), (int)(NPC.damage * 1.5f), new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            if (AITimer == 100 || AITimer == 200 || AITimer == 300)
                            {
                                for (int i = 0; i < 2; i++)
                                    NPC.Shoot(new Vector2(playerOrigin.X + 800 * (i == 0 ? -1 : 1), playerOrigin.Y - 600), ModContent.ProjectileType<ScorchingRay>(), (int)(NPC.damage * 1.5f), new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            if (AITimer < 120)
                                NPC.Move(new Vector2(playerOrigin.X + 600, player.Center.Y - 270), 18, 20, false);
                            else if (AITimer >= 80 && AITimer < 220)
                                NPC.velocity *= 0.5f;

                            if (AITimer == 120)
                                ArmType = 1;
                            if (AITimer >= 130 && AITimer % 7 == 0 && AITimer <= 170 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slice3").WithPitchVariance(0.1f), NPC.position);
                                SoundEngine.PlaySound(SoundID.Item125, NPC.Center);
                                int p = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HolyPhalanx_Proj>(), NPC.damage / 4, 3, Main.myPlayer, NPC.whoAmI, TimerRand2 * 60);
                                Main.projectile[p].localAI[0] += TimerRand2 * 7;
                                TimerRand2++;
                            }
                            if (AITimer >= 220)
                            {
                                NPC.Move(new Vector2(playerOrigin.X - 600, player.Center.Y - 270), 6, 40, false);
                            }
                            if (AITimer == 240)
                                ArmType = 0;
                            if (AITimer >= 460)
                            {
                                TimerRand = 0;
                                TimerRand2 = 0;
                                AITimer = 0;
                                AIState = ActionState.Fallen;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Fallen:
                    switch (TimerRand)
                    {
                        case 0:
                            NPC.velocity *= 0.96f;
                            if (NPC.velocity.Length() <= 1)
                            {
                                NPC.velocity.X = 0;
                                TimerRand = 1;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            AITimer++;
                            NPC.noGravity = false;
                            NPC.noTileCollide = false;
                            if ((AITimer > 5 && NPC.velocity.Y == 0) || AITimer > 300)
                            {
                                AITimer = 0;
                                TimerRand = 2;
                                NPC.netUpdate = true;

                            }
                            break;
                        case 2:
                            AITimer++;
                            if (AITimer < 60)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.GoldFlame, Scale: 2)];
                                    dust2.noGravity = true;
                                    dust2.velocity = dust2.position.DirectionTo(NPC.Center) * 15f;
                                }
                            }

                            if (AITimer % 20 == 0 && AITimer < 60)
                            {
                                SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 28, 1, TimerRand2);
                                TimerRand2 += 0.1f;
                                NPC.netUpdate = true;
                            }

                            if (AITimer == 60)
                            {
                                DustHelper.DrawCircle(NPC.Center, DustID.GoldFlame, 5, 5, 5, 1, 2, nogravity: true);
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Erhan_HolyShield>(), 0, Vector2.Zero, false,
                                        SoundID.Item29, "", NPC.whoAmI, i);
                            }

                            if (RedeBossDowned.erhanDeath < 2 && !Main.dedServ)
                            {
                                if (AITimer == 60)
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Huzzah! *pant* A shield!", 180, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                if (AITimer == 240)
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("But alas, this shield is useless against a blade!", 180, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                if (AITimer == 500)
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("*pant* *pant* Alright'eth! I hath regained my breath. Have at thee!", 180, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                            }
                            if (AITimer >= (RedeBossDowned.erhanDeath < 2 ? 500 : 360))
                            {
                                if (RedeBossDowned.erhanDeath < 2)
                                    RedeBossDowned.erhanDeath = 2;

                                AttackNumber++;
                                NPC.noGravity = true;
                                NPC.noTileCollide = true;
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 0;
                                if (AttackNumber > 10 && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<Erhan_HolyShield2>()))
                                {
                                    DustHelper.DrawCircle(NPC.Center, DustID.GoldFlame, 5, 5, 5, 1, 2, nogravity: true);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Erhan_HolyShield2>(), 0, Vector2.Zero, false,
                                        SoundID.Item29, "", NPC.whoAmI);
                                }
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Death:
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (RedeBossDowned.downedErhan)
                    {
                        if (AITimer++ == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item68, NPC.position);
                            player.RedemptionScreen().ScreenShakeIntensity = 14;
                            TeleGlow = true;
                            HolyFlare = true;
                            NPC.alpha = 255;
                        }
                        if (AITimer >= 5)
                        {
                            Spared = true;
                            NPC.dontTakeDamage = false;
                            player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        switch (TimerRand)
                        {
                            case 0:
                                NPC.noGravity = true;
                                NPC.noTileCollide = true;

                                if (AITimer++ == 0)
                                {
                                    NPC.dontTakeDamage = true;
                                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                }
                                if (AITimer < 120)
                                    NPC.Move(new Vector2(player.Center.X - 250 * NPC.spriteDirection, player.Center.Y - 250), 7, 30);
                                else
                                    NPC.velocity *= 0.9f;
                                if (AITimer >= 180)
                                {
                                    ArmType = 0;
                                    HeadFrameY = 0;
                                    NPC.velocity *= 0;
                                    TimerRand = 1;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                                break;
                            case 1:
                                player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                player.RedemptionScreen().lockScreen = true;
                                NPC.LockMoveRadius(player);
                                if (!Main.dedServ)
                                {
                                    if (AITimer++ == 10)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I foresee my defeat creeping up on me.", 180, 1, 0.6f, "Erhan:", 0.5f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    if (AITimer == 190)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Well...", 180, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    if (AITimer == 370)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("If all else fail'eth...", 180, 1, 0.6f, "Erhan:", 0.5f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    if (AITimer == 600)
                                    {
                                        ArmType = 1;
                                        NPC.Shoot(NPC.Center + new Vector2(40 * NPC.spriteDirection, -80),
                                            ModContent.ProjectileType<HolyHandGrenadeOfAnglon>(), 0, Vector2.Zero, false, SoundID.Item30);
                                    }
                                    if (AITimer == 620)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Grenade.", 120, 1, 0.6f, "Erhan:", 0, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    if (AITimer == 800)
                                    {
                                        EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 60);
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...", 120, 1, 0.6f, "Erhan:", 0, Color.LightGoldenrodYellow, null, text, NPC.Center);
                                    }
                                    if (AITimer == 860)
                                    {
                                        HeadFrameY = 1;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("How doth one use this thing?", 60, 1, 0.6f, "Erhan:", 0, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                    }
                                    if (AITimer >= 920)
                                    {
                                        HeadFrameY = 0;
                                        ArmType = 0;
                                        AITimer = 0;
                                        TimerRand = 2;
                                        NPC.netUpdate = true;
                                    }
                                }
                                break;
                            case 2:
                                player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                player.RedemptionScreen().lockScreen = true;

                                int dustIndex1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                                Main.dust[dustIndex1].noGravity = true;
                                Dust dust = Main.dust[dustIndex1];
                                dust.velocity.X = 0f;
                                dust.velocity.Y = -5f;
                                if (AITimer++ >= 80)
                                {
                                    NPC.noGravity = false;
                                    NPC.noTileCollide = false;
                                    if ((AITimer > 85 && NPC.velocity.Y == 0) || AITimer > 300)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            for (int i = 0; i < NPC.buffTime.Length; i++)
                                            {
                                                NPC.buffTime[i] = 0;
                                                NPC.buffType[i] = 0;
                                            }

                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SendNPCBuffs, number: NPC.whoAmI);
                                        }

                                        for (int i = 0; i < 20; i++)
                                        {
                                            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, Alpha: 100, Scale: 3f);
                                            Main.dust[dustIndex].noGravity = true;
                                            Main.dust[dustIndex].velocity *= 2;
                                        }
                                        AITimer = 0;
                                        TimerRand = 3;
                                        NPC.netUpdate = true;
                                    }
                                }
                                break;
                            case 3:
                                if (AITimer++ == 0)
                                {
                                    NPC.dontTakeDamage = false;
                                    NPC.chaseable = false;
                                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                }
                                if (AITimer == 60)
                                {
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("It would appear'eth, I hath lost.", 180, 1, 0.6f, "Erhan:", 0.5f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                }
                                if (AITimer == 300)
                                {
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("If, mayhaps you shan't spare my life...", 180, 1, 0.6f, "Erhan:", 0.5f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                }
                                if (AITimer == 600)
                                {
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Send master Hallowed Knight my regards.", 180, 1, 0.6f, "Erhan:", 0.5f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                }
                                if (AITimer == 1200)
                                {
                                    NPC.dontTakeDamage = true;
                                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                }

                                if (AITimer == 1200)
                                {
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Well... 'Til we meet again!", 180, 1, 0.6f, "Erhan:", 0.5f, Color.LightGoldenrodYellow, null, text, NPC.Center, sound: true);
                                }
                                if (AITimer == 1380)
                                {
                                    SoundEngine.PlaySound(SoundID.Item68, NPC.position);
                                    player.RedemptionScreen().ScreenShakeIntensity = 14;
                                    TeleGlow = true;
                                    HolyFlare = true;
                                    NPC.alpha = 255;
                                }
                                if (AITimer >= 1385)
                                {
                                    Spared = true;
                                    NPC.dontTakeDamage = false;
                                    player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
            {
                if (!Spared)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("", 1, 1, 0, "", 0, null, null, null, null);
                return true;
            }
            else
            {
                for (int k = 0; k < NPC.buffImmune.Length; k++)
                    NPC.buffImmune[k] = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < NPC.buffTime.Length; i++)
                    {
                        NPC.buffTime[i] = 0;
                        NPC.buffType[i] = 0;
                    }

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SendNPCBuffs, number: NPC.whoAmI);
                }

                NPC.dontTakeDamage = true;
                NPC.velocity *= 0;
                NPC.alpha = 0;
                NPC.life = 1;
                AITimer = 0;
                TimerRand = 0;
                AIState = ActionState.Death;
                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return false;
            }
        }

        private int ArmFrameY;
        private int ArmType;
        private int HeadFrameY;
        private bool HolyFlare;
        private int HolyFlareTimer;
        private bool TeleGlow;
        private int TeleGlowTimer;

        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            if (HolyFlare)
            {
                HolyFlareTimer++;
                if (HolyFlareTimer > 60)
                {
                    HolyFlare = false;
                    HolyFlareTimer = 0;
                }
            }
            if (TeleGlow)
            {
                TeleGlowTimer += 3;
                if (TeleGlowTimer > 60)
                {
                    TeleGlow = false;
                    TeleGlowTimer = 0;
                }
            }

            if ((AIState is ActionState.Fallen && TimerRand != 0) ||
                (AIState is ActionState.Death && TimerRand == 3))
            {
                if (++NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    ArmFrameY++;
                    if (ArmFrameY > 3)
                        ArmFrameY = 0;
                }
                return;
            }

            ArmFrameY = (NPC.frame.Y / frameHeight) + (6 * ArmType);

            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 5 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public void MoveClamp()
        {
            Player player = Main.player[NPC.target];
            int xFar = 400;
            if (NPC.Center.X < player.Center.X)
            {
                if (move < player.Center.X - xFar)
                {
                    move = player.Center.X - xFar;
                }
                else if (move > player.Center.X - 200)
                {
                    move = player.Center.X - 200;
                }
            }
            else
            {
                if (move > player.Center.X + xFar)
                {
                    move = player.Center.X + xFar;
                }
                else if (move < player.Center.X + 200)
                {
                    move = player.Center.X + 200;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D ArmsTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms").Value;
            Texture2D HeadTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head").Value;
            Texture2D FallTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Fall").Value;
            Texture2D GroundedTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Grounded").Value;
            Texture2D BoomedTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Boomed").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.Yellow, Color.Goldenrod * 0.7f, Color.Yellow);

            if (AIState is ActionState.Death && TimerRand == 2)
            {
                Rectangle rectBoomed = new(0, 0, BoomedTex.Width, BoomedTex.Height);
                Vector2 originBoomed = new(BoomedTex.Width / 2f, BoomedTex.Height / 2f);
                spriteBatch.Draw(BoomedTex, NPC.Center - screenPos, new Rectangle?(rectBoomed), NPC.GetAlpha(drawColor), NPC.rotation, originBoomed, NPC.scale, effects, 0);
                return false;
            }
            if ((AIState is ActionState.Fallen && TimerRand != 0) ||
                (AIState is ActionState.Death && TimerRand == 3))
            {
                if (TimerRand <= 1)
                {
                    Rectangle rectFall = new(0, 0, FallTex.Width, FallTex.Height);
                    Vector2 originFall = new(FallTex.Width / 2f, FallTex.Height / 2f);
                    spriteBatch.Draw(FallTex, NPC.Center - screenPos, new Rectangle?(rectFall), NPC.GetAlpha(drawColor), NPC.rotation, originFall, NPC.scale, effects, 0);
                }
                else
                {
                    int heightGrounded = GroundedTex.Height / 4;
                    int yGrounded = heightGrounded * ArmFrameY;
                    Rectangle rectGrounded = new(0, yGrounded, GroundedTex.Width, heightGrounded);
                    Vector2 originGrounded = new(GroundedTex.Width / 2f, heightGrounded / 2f);
                    spriteBatch.Draw(GroundedTex, NPC.Center - screenPos + new Vector2(0, 10), new Rectangle?(rectGrounded), NPC.GetAlpha(drawColor), NPC.rotation, originGrounded, NPC.scale, effects, 0);
                }


                return false;
            }
            int heightHead = HeadTex.Height / 3;
            int yHead = heightHead * HeadFrameY;
            Rectangle rectHead = new(0, yHead, HeadTex.Width, heightHead);
            Vector2 originHead = new(HeadTex.Width / 2f, heightHead / 2f);
            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(HeadTex, oldPos + NPC.Size / 2f - screenPos - new Vector2(-2 * NPC.spriteDirection, 33), new Rectangle?(rectHead), NPC.GetAlpha(shaderColor) * 0.5f, oldrot[i], originHead, NPC.scale + 0.1f, effects, 0);
                }
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(shaderColor) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.Draw(HeadTex, NPC.Center - screenPos - new Vector2(-2 * NPC.spriteDirection, 33), new Rectangle?(rectHead), NPC.GetAlpha(drawColor), NPC.rotation, originHead, NPC.scale, effects, 0);

            int heightArms = ArmsTex.Height / 24;
            int yArms = heightArms * ArmFrameY;
            Rectangle rectArms = new(0, yArms, ArmsTex.Width, heightArms);
            Vector2 originArms = new(ArmsTex.Width / 2f, heightArms / 2f);
            spriteBatch.Draw(ArmsTex, NPC.Center - screenPos + new Vector2(-2 * NPC.spriteDirection, -10), new Rectangle?(rectArms), NPC.GetAlpha(drawColor), NPC.rotation, originArms, NPC.scale, effects, 0);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = NPC.Center - screenPos;
            Color colour = Color.Lerp(Color.White, Color.White, 1f / HolyFlareTimer * 10f) * (1f / HolyFlareTimer * 10f);
            if (HolyFlare)
            {
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour, NPC.rotation, origin, 3f, SpriteEffects.None, 0);
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 0.4f, NPC.rotation, origin, 2.5f, SpriteEffects.None, 0);
            }

            {
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour, NPC.rotation, origin, 3f, SpriteEffects.None, 0);
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 0.4f, NPC.rotation, origin, 2.5f, SpriteEffects.None, 0);
            }

            Texture2D teleportGlow = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow3").Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = NPC.Center - screenPos;
            Color colour2 = Color.Lerp(Color.White, Color.White, 1f / TeleGlowTimer * 10f) * (1f / TeleGlowTimer * 10f);
            if (TeleGlow)
            {
                spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, NPC.rotation, origin2, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, NPC.rotation, origin2, 2f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (AIState is ActionState.Fallen && TimerRand == 2 && item.DamageType == DamageClass.Melee)
                damage *= 2;

            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (ItemTags.Celestial.Has(item.type) || ItemTags.Psychic.Has(item.type))
                    damage = (int)(damage * 0.9f);

                if (ItemTags.Holy.Has(item.type))
                    damage = (int)(damage * 0.5f);

                if (ItemTags.Shadow.Has(item.type))
                    damage = (int)(damage * 1.25f);
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (AIState is ActionState.Fallen && TimerRand == 2 && projectile.Redemption().TechnicallyMelee)
                damage *= 2;

            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (ProjectileTags.Celestial.Has(projectile.type) || ProjectileTags.Psychic.Has(projectile.type))
                    damage = (int)(damage * 0.9f);

                if (ProjectileTags.Holy.Has(projectile.type))
                    damage = (int)(damage * 0.5f);

                if (ProjectileTags.Shadow.Has(projectile.type))
                    damage = (int)(damage * 1.25f);
            }
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.alpha += 2;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
    }
}