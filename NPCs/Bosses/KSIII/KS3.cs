using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.DataStructures;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Items.Usable;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Redemption.Base;
using Redemption.Projectiles.Misc;
using Redemption.UI;
using Redemption.Dusts;

namespace Redemption.NPCs.Bosses.KSIII
{
    [AutoloadBossHead]
    public class KS3 : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Dialogue,
            GunAttacks,
            SpecialAttacks,
            PhysicalAttacks,
            PhaseChange,
            PhaseTransition1,
            PhaseTransition2,
            PhaseTransition3,
            PhaseTransition4,
            SpareCountdown,
            Spared,
            Attacked,
            Overclock
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ref float AttackChoice => ref NPC.ai[3];

        public float[] oldrot = new float[3];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("King Slayer III");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 36),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 42000;
            NPC.defense = 35;
            NPC.damage = 120;
            NPC.width = 42;
            NPC.height = 106;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossSlayer");
            //BossBag = ModContent.ItemType<SlayerBag>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.velocity.Length() >= 13f && AIState is not ActionState.PhysicalAttacks;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("'What? You want my lore? Go get your own lore!'")
            });
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ModContent.ProjectileType<KS3_Exit>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0));

            if (!RedeBossDowned.downedSlayer)
            {
                RedeWorld.alignment -= NPC.ai[0] == 12 ? 0 : 2;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, NPC.ai[0] == 12 ? "+0" : "-2", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                    {
                        if (AIState is ActionState.Spared)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Good thing you left him be...", 240, 30, 0, Color.DarkGoldenrod);
                        else
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Oh dear, he seems to have a very short temper, and you winning probably made it worse.\nI hope he doesn't do anything stupid.", 240, 30, 0, Color.DarkGoldenrod);
                    }

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSlayer, -1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (phase >= 5)
                damage *= 0.6;
            else
                damage *= 0.75;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(chance);
                writer.Write(phase);
                writer.Write(BodyState);
                writer.Write(gunRot);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                chance = (float)reader.ReadDouble();
                phase = reader.ReadInt32();
                BodyState = reader.ReadInt32();
                gunRot = (float)reader.ReadDouble();
            }
        }

        private float chance = 0.8f;
        private int phase;
        public int HeadType;
        public int BodyState;
        public enum BodyAnim
        {
            Idle, Crossed, Gun, GunShoot, GunEnd, RocketFist, Grenade, Charging, Shrug, ShieldOn, ShieldOff, IdlePhysical, WheelkickStart, Wheelkick, WheelkickEnd, ShoulderBash, ShoulderBashEnd, DropkickStart, Dropkick, Pummel1, Pummel2, Jojo
        }

        const float gunRotLimit = (float)Math.PI / 2;
        public float gunRot;
        private Vector2 ShootPos;

        private int TeleportCount;
        private float TeleGlowTimer;
        private bool TeleGlow;
        private Vector2 TeleVector;

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            DespawnHandler();
            chance = MathHelper.Clamp(chance, 0, 1);
            if (NPC.life < (int)(NPC.lifeMax * 0.75f) && phase < 1)
                AIState = ActionState.PhaseChange;
            else if (NPC.life < NPC.lifeMax / 2 && phase < 2)
                AIState = ActionState.PhaseChange;
            else if (NPC.life < NPC.lifeMax / 4 && phase < 3)
                AIState = ActionState.PhaseChange;
            else if (NPC.life < (int)(NPC.lifeMax * 0.05f) && phase < 4 && !RedeConfigClient.Instance.NoLoreElements)
                AIState = ActionState.PhaseChange;

            player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;

            Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(54, gunRot) + RedeHelper.PolarVector(13 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
            int dmgIncrease = NPC.DistanceSQ(player.Center) > 800 * 800 ? 10 : 0;

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    if (RedeConfigClient.Instance.NoLoreElements && !Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("King Slayer III", 60, 90, 0.8f, 0, Color.Cyan, "Prototype Multium");

                    NPC.LookAtEntity(player);
                    BodyState = (int)BodyAnim.Crossed;
                    player.GetModPlayer<ScreenPlayer>().Rumble(5, 5);
                    TeleVector = NPC.Center;
                    TeleGlow = true;
                    if (AITimer++ > 5)
                    {
                        AITimer = 0;
                        AIState = ActionState.Dialogue;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Dialogue:
                    #region Dialogue Moment
                    NPC.LookAtEntity(player);
                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    AITimer++;
                    if (RedeConfigClient.Instance.NoLoreElements)
                    {
                        if (AITimer == 60)
                        {
                            ArmsFrameY = 1;
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Gun;
                        }
                        NPC.netUpdate = true;
                        if (AITimer >= 160)
                        {
                            if (RedeBossDowned.slayerDeath < 3)
                                RedeBossDowned.slayerDeath = 3;

                            NPC.dontTakeDamage = false;
                            ShootPos = new Vector2(player.Center.X > NPC.Center.X ? Main.rand.Next(-400, -300) : Main.rand.Next(300, 400), Main.rand.Next(-60, 60));
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);
                            HeadType = 0;
                            AITimer = 0;
                            AIState = ActionState.GunAttacks;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                        if (RedeBossDowned.slayerDeath < 3)
                        {
                            if (AITimer == 30)
                            {
                                HeadType = 2;
                                if (!Main.dedServ)
                                {
                                    if (RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_DroneKillCheck>()))
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Did you seriously just destroy my drones?", 280, 1, 0.6f, "King Slayer III:", 1, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                    else
                                    {
                                        if (RedeWorld.alignment >= 0)
                                        {
                                            /*if (Main.LocalPlayer.GetModPlayer<RedePlayer>().omegaPower || player.IsFullTBot())
                                            {
                                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Alright listen here you little scrap of metal.", 280, 1, 0.6f, "King Slayer III:", 1, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                                            }
                                            else if (BasePlayer.HasAccessory(player, ModContent.ItemType<CrownOfTheKing>(), true, true))
                                            {
                                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Alright listen here you little chicken nugget.", 280, 1, 0.6f, "King Slayer III:", 1, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                                            }
                                            else*/
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Alright listen here you little fleshbag.", 280, 1, 0.6f, "King Slayer III:", 1, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                        }
                                        else
                                        {
                                            /*if (Main.LocalPlayer.GetModPlayer<RedePlayer>().omegaPower || player.IsFullTBot())
                                            {
                                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Ah, this little scrap of metal decided to save me the trouble of finding it.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                                            }
                                            else if (BasePlayer.HasAccessory(player, ModContent.ItemType<CrownOfTheKing>(), true, true))
                                            {
                                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Ah, this little chicken nugget decided to save me the trouble of finding it.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                                            }
                                            else*/
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Ah, this little fleshbag decided to save me the trouble of finding it.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                        }
                                    }
                                }
                            }
                            if (AITimer == 310)
                            {
                                HeadType = 1;
                                if (!Main.dedServ)
                                {
                                    if (RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_DroneKillCheck>()))
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Eh, not like I got a shortage of them, but I'm still gonna blast ya for it!", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                    else
                                    {
                                        if (RedeWorld.alignment >= 0)
                                        {
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I warned you, so don't go crying to your mummy when I crush you into the ground!", 280, 1, 0.6f, "King Slayer III:", 1, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                        }
                                        else
                                        {
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You were on my hitlist, so lets skip the small talk and get on with it!", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                        }
                                    }
                                }
                            }
                            if (AITimer == 590)
                            {
                                BodyState = (int)BodyAnim.Idle;
                                HeadType = 0;
                                if (RedeBossDowned.downedKeeper && !Main.dedServ)
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Actually... You were the one that fought the Keeper, weren't you!", 240, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                else
                                    AITimer = 1140;
                            }
                            if (AITimer == 830)
                            {
                                HeadType = 2;
                                if (!Main.dedServ)
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("That was my job!", 120, 1, 0.6f, "King Slayer III:", 2, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                            }
                            if (AITimer == 950)
                            {
                                HeadType = 0;
                                if (!Main.dedServ)
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Great, now I have even more reason to pummel you to ash!", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                            }
                            if (AITimer == 1150)
                            {
                                BodyState = (int)BodyAnim.Crossed;
                                HeadType = 0;
                                NPC.netUpdate = true;
                            }
                            if (AITimer >= 1250)
                            {
                                if (!Main.dedServ)
                                    RedeSystem.Instance.TitleCardUIElement.DisplayTitle("King Slayer III", 60, 90, 0.8f, 0, Color.Cyan, "Prototype Multium");

                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? Main.rand.Next(-400, -300) : Main.rand.Next(300, 400), Main.rand.Next(-60, 60));
                                if (RedeBossDowned.slayerDeath < 3)
                                    RedeBossDowned.slayerDeath = 3;

                                NPC.dontTakeDamage = false;
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);
                                HeadType = 0;
                                AITimer = 0;
                                AIState = ActionState.GunAttacks;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (AITimer == 30 && !Main.dedServ)
                            {
                                if (RedeBossDowned.downedSlayer)
                                {
                                    switch (Main.rand.Next(4))
                                    {
                                        case 0:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("What? Do you want to fight me again?", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                        case 1:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Why must you summon me again?", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                        case 2:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Could you maybe possibly probably potentially LEAVE ME ALONE?", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                        case 3:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Really, a rematch? Fine.", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (Main.rand.Next(5))
                                    {
                                        case 0:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You're quite a resilient fellow...", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                        case 1:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Could've sworn you died...", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                        case 2:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Ready for a rematch?", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                        case 3:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Welp, time to win again!", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                        case 4:
                                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Still wanna fight?", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                            break;
                                    }
                                }
                            }
                            if (AITimer == 180)
                            {
                                BodyState = (int)BodyAnim.Crossed;
                                NPC.netUpdate = true;
                            }
                            if (AITimer >= 240)
                            {
                                if (!Main.dedServ)
                                    RedeSystem.Instance.TitleCardUIElement.DisplayTitle("King Slayer III", 60, 90, 0.8f, 0, Color.Cyan, "Prototype Multium");

                                NPC.dontTakeDamage = false;
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? Main.rand.Next(-400, -300) : Main.rand.Next(300, 400), Main.rand.Next(-60, 60));
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);
                                HeadType = 0;
                                AITimer = 0;
                                AIState = ActionState.GunAttacks;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseChange:
                    NPC.LookAtEntity(player);
                    AITimer = 0;
                    AttackChoice = 0;
                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    NPC.rotation = 0;
                    NPC.velocity *= 0.9f;

                    if (BodyState is (int)BodyAnim.Gun)
                        BodyState = (int)BodyAnim.GunEnd;
                    else if (BodyState is not (int)BodyAnim.Idle && BodyState is not (int)BodyAnim.GunEnd)
                        BodyState = (int)BodyAnim.Idle;

                    if (BodyState is (int)BodyAnim.Idle && NPC.velocity.Length() < 1f)
                    {
                        switch (phase)
                        {
                            case 0:
                                phase = 1;
                                AIState = ActionState.PhaseTransition1;
                                break;
                            case 1:
                                phase = 2;
                                AIState = ActionState.PhaseTransition2;
                                break;
                            case 2:
                                phase = 3;
                                AIState = ActionState.PhaseTransition3;
                                break;
                            case 3:
                                phase = 4;
                                AIState = ActionState.PhaseTransition4;
                                break;
                        }
                        AITimer = 0;
                        AttackChoice = 0;
                    }
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    break;
                case ActionState.GunAttacks:
                    NPC.LookAtEntity(player);
                    if (AttackChoice == 0)
                    {
                        AttackChoice = Main.rand.Next(1, 6);
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        NPC.netUpdate = true;
                    }
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    switch (AttackChoice)
                    {
                        case -1:
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 6);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                                if (AITimer == 0)
                                {
                                    if (Main.rand.NextBool(2))
                                        AITimer = 1;
                                    else
                                        AITimer = 2;
                                    NPC.netUpdate = true;
                                }
                                NPC.velocity *= 0.9f;
                                if (AITimer == 1)
                                {
                                    if (BodyState is (int)BodyAnim.Gun)
                                        BodyState = (int)BodyAnim.GunEnd;

                                    if (BodyState is (int)BodyAnim.Idle && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.SpecialAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                    NPC.netUpdate = true;
                                }
                                else if (AITimer == 2)
                                {
                                    if (BodyState is (int)BodyAnim.Gun)
                                        BodyState = (int)BodyAnim.GunEnd;

                                    if (BodyState is (int)BodyAnim.Idle && NPC.velocity.Length() < 1f)
                                    {
                                        BodyState = (int)BodyAnim.IdlePhysical;
                                        AITimer = 0;
                                        AIState = ActionState.PhysicalAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                    NPC.netUpdate = true;
                                }

                            }
                            break;

                        #region Barrage Shot
                        case 1:
                            gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                            SnapGunToFiringArea();
                            if (AITimer++ % 20 == 0)
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? Main.rand.Next(-400, -300) : Main.rand.Next(300, 400), Main.rand.Next(-60, 60));

                            NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) < 100 * 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (phase <= 1)
                            {
                                if (AITimer % 40 == 0)
                                {
                                    NPC.Shoot(GunOrigin, ProjectileID.PhantasmalBolt, 72, RedeHelper.PolarVector(7 + dmgIncrease, gunRot), true, SoundID.Item1, "Sounds/Custom/Gun1");
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer % 120 == 0)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        int rot = 25 * i;
                                        NPC.Shoot(GunOrigin, ProjectileID.MartianTurretBolt, 72, RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 25)), true, SoundID.Item1, "Sounds/Custom/Gun3");
                                    }
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (AITimer >= 370)
                                {
                                    chance -= Main.rand.NextFloat(0.1f, 0.5f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (phase >= 5)
                            {
                                if (AITimer % 20 == 0)
                                {
                                    NPC.Shoot(GunOrigin, ProjectileID.PhantasmalBolt, 72, RedeHelper.PolarVector(7 + dmgIncrease, gunRot), true, SoundID.Item1, "Sounds/Custom/Gun1");
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer % 100 == 0)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int rot = 25 * i;
                                        NPC.Shoot(GunOrigin, ProjectileID.MartianTurretBolt, 72, RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 50)), true, SoundID.Item1, "Sounds/Custom/Gun3");
                                    }
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (AITimer >= 310)
                                {
                                    chance -= Main.rand.NextFloat(0.1f, 0.5f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                if (AITimer % 35 == 0)
                                {
                                    NPC.Shoot(GunOrigin, ProjectileID.PhantasmalBolt, 72, RedeHelper.PolarVector(7 + dmgIncrease, gunRot), true, SoundID.Item1, "Sounds/Custom/Gun1");
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer % 105 == 0)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int rot = 25 * i;
                                        NPC.Shoot(GunOrigin, ProjectileID.MartianTurretBolt, 72, RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 50)), true, SoundID.Item1, "Sounds/Custom/Gun3");
                                    }
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (AITimer >= 330)
                                {
                                    chance -= Main.rand.NextFloat(0.1f, 0.5f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Bullet Spray
                        case 2:
                            if (AITimer < 245)
                                gunRot.SlowRotation(NPC.DirectionTo(player.Center + player.velocity * 20f).ToRotation(), (float)Math.PI / 60f);

                            SnapGunToFiringArea();
                            AITimer++;
                            ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -300 : 300, 10);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer < 200)
                            {
                                if (NPC.Distance(ShootPos) < 100 || (phase >= 5 ? AITimer > 40 : AITimer > 80))
                                {
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_TeleLine1>(), 0, RedeHelper.PolarVector(10, gunRot), false, SoundID.Item1.WithVolume(0), ai1: NPC.whoAmI);
                                    NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 13f, 14f, true);
                                }
                            }
                            else
                            {
                                if (AITimer < 260)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        Vector2 vector;
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        vector.X = (float)(Math.Sin(angle) * 40);
                                        vector.Y = (float)(Math.Cos(angle) * 40);
                                        Dust dust2 = Main.dust[Dust.NewDust(GunOrigin + vector, 2, 2, DustID.Frost, 0f, 0f, 100, default, 2f)];
                                        dust2.noGravity = true;
                                        dust2.velocity = dust2.position.DirectionTo(GunOrigin) * 10f;
                                    }
                                }
                                NPC.velocity *= 0.96f;
                                if (AITimer == 260)
                                {
                                    NPC.velocity.X = player.Center.X > NPC.Center.X ? -9 : 9;
                                    for (int i = 0; i < Main.rand.Next(5, 8); i++)
                                    {
                                        NPC.Shoot(GunOrigin, ProjectileID.PhantasmalBolt, 72, RedeHelper.PolarVector(Main.rand.Next(7, 11) + dmgIncrease, gunRot + Main.rand.NextFloat(-0.14f, 0.14f)), true, SoundID.Item1, "Sounds/Custom/ShotgunBlast1");
                                    }
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;

                                }
                                if (AITimer > 300)
                                {
                                    chance -= Main.rand.NextFloat(0.05f, 0.3f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Rebound Shot
                        case 3:
                            gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                            SnapGunToFiringArea();
                            AITimer++;
                            ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -450 : 450, -10);
                            NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (phase >= 5 ? AITimer == 40 : AITimer == 60)
                            {
                                NPC.Shoot(GunOrigin, ModContent.ProjectileType<ReboundShot>(), 72, RedeHelper.PolarVector(15 + dmgIncrease, gunRot), true, SoundID.Item1, "Sounds/Custom/Gun2");
                                BodyState = (int)BodyAnim.GunShoot;
                                NPC.netUpdate = true;
                            }
                            if (phase >= 5 ? AITimer > 60 : AITimer > 90)
                            {
                                chance -= Main.rand.NextFloat(0.05f, 0.1f);
                                AITimer = 0;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Rebound Shot II
                        case 4:
                            if (AITimer == 0)
                            {
                                if (phase > 0)
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 6);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                                SnapGunToFiringArea();
                                AITimer++;
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -450 : 450, -10);
                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                                if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                                {
                                    ArmsFrameY = 1;
                                    ArmsFrameX = 0;
                                    BodyState = (int)BodyAnim.Gun;
                                }

                                if (phase >= 5 ? AITimer == 41 || AITimer == 44 || AITimer == 47 : AITimer == 61 || AITimer == 64 || AITimer == 67)
                                {
                                    NPC.Shoot(GunOrigin, ModContent.ProjectileType<ReboundShot>(), 72, RedeHelper.PolarVector(15 + dmgIncrease, gunRot), true, SoundID.Item1, "Sounds/Custom/Gun2");
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (phase >= 5 ? AITimer > 61 : AITimer > 91)
                                {
                                    chance -= Main.rand.NextFloat(0.05f, 0.1f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Barrage Shot II
                        case 5:
                            if (AITimer == 0)
                            {
                                if (phase > 0)
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 6);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                                SnapGunToFiringArea();
                                if (AITimer++ % 20 == 0)
                                    ShootPos = new Vector2(player.Center.X > NPC.Center.X ? Main.rand.Next(-400, -300) : Main.rand.Next(300, 400), Main.rand.Next(-60, 60));

                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                                if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                                {
                                    ArmsFrameY = 1;
                                    ArmsFrameX = 0;
                                    BodyState = (int)BodyAnim.Gun;
                                }

                                if (AITimer % 10 == 0)
                                {
                                    NPC.Shoot(GunOrigin, ProjectileID.PhantasmalBolt, 72, RedeHelper.PolarVector(6 + dmgIncrease, gunRot), true, SoundID.Item1, "Sounds/Custom/Gun1");
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer >= 61)
                                {
                                    chance -= Main.rand.NextFloat(0.02f, 0.2f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.SpecialAttacks:
                    NPC.LookAtEntity(player);
                    if (AttackChoice == 0)
                        AttackChoice = Main.rand.Next(1, 9); chance = Main.rand.NextFloat(0.5f, 1f);

                    NPC.rotation = NPC.velocity.X * 0.01f;
                    switch ((int)AttackChoice)
                    {
                        case -1:
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 9);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                if (AITimer == 0)
                                {
                                    if (Main.rand.Next(2) == 0)
                                        AITimer = 1;
                                    else
                                        AITimer = 2;
                                    NPC.netUpdate = true;
                                }
                                NPC.velocity *= 0.9f;
                                if (AITimer == 1)
                                {
                                    if (BodyState is (int)BodyAnim.Idle)
                                        BodyState = (int)BodyAnim.IdlePhysical;

                                    if (BodyState is (int)BodyAnim.IdlePhysical && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.PhysicalAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else if (AITimer == 2)
                                {
                                    if (BodyState is (int)BodyAnim.Idle)
                                    {
                                        ArmsFrameY = 1;
                                        ArmsFrameX = 0;
                                        BodyState = (int)BodyAnim.Gun;
                                    }

                                    if (BodyState is (int)BodyAnim.Gun && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.GunAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;

                        #region Rocket Fist
                        case 1:
                            ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -300 : 300, -60);
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (NPC.Distance(ShootPos) < 160 || AITimer > 40)
                                {
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 14f, true);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (AITimer == 120)
                                    BodyState = (int)BodyAnim.RocketFist;

                                if (AITimer == 135)
                                    NPC.Shoot(new Vector2(NPC.Center.X + 15 * NPC.spriteDirection, NPC.Center.Y - 11), ModContent.ProjectileType<KS3_Fist>(), 102, new Vector2(10 * NPC.spriteDirection, 0), true, SoundID.Item1, "Sounds/Custom/MissileFire1");

                                if (AITimer > 170)
                                {
                                    chance -= Main.rand.NextFloat(0.03f, 0.1f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Stun Grenade
                        case 2:
                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(3))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 9);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -200 : 200, 0);
                                if (AITimer < 100)
                                {
                                    if (NPC.Distance(ShootPos) < 160 || AITimer > 50)
                                    {
                                        AITimer = 100;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 18f, 14f, true);
                                }
                                else
                                {
                                    NPC.velocity *= 0.9f;
                                    if (AITimer == 120)
                                        BodyState = (int)BodyAnim.Grenade;

                                    if (AITimer == 140)
                                        NPC.Shoot(new Vector2(NPC.Center.X + 21 * NPC.spriteDirection, NPC.Center.Y - 17), ModContent.ProjectileType<KS3_FlashGrenade>(), 78, new Vector2(10 * NPC.spriteDirection, -6), false, SoundID.Item1);

                                    if (AITimer > 180)
                                    {
                                        chance -= Main.rand.NextFloat(0.05f, 0.8f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Beam Cell
                        case 3:
                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(2))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 9);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -320 : 320, 0);
                                if (AITimer < 100)
                                {
                                    if (NPC.Distance(ShootPos) < 160 || AITimer > 60)
                                    {
                                        AITimer = 100;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            Vector2 vector;
                                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                            vector.X = (float)(Math.Sin(angle) * 100);
                                            vector.Y = (float)(Math.Cos(angle) * 100);
                                            Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.Frost, 0f, 0f, 100, default, 1f)];
                                            dust2.noGravity = true;
                                            dust2.velocity = -NPC.DirectionTo(dust2.position) * 5f;
                                        }
                                        NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 8f, 14f, true);
                                    }
                                }
                                else
                                {
                                    NPC.Move(ShootPos, 4f, 14f, true);
                                    if (AITimer == 121)
                                        NPC.Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_BeamCell>(), 96,
                                            RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(35 * NPC.spriteDirection)),
                                            false, SoundID.Item103, ai0: NPC.whoAmI);

                                    if (AITimer > 240)
                                    {
                                        BodyState = (int)BodyAnim.Idle;
                                        chance -= Main.rand.NextFloat(0.5f, 1f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Core Surge
                        case 4:
                            if (AITimer == 0)
                            {
                                if (RedeHelper.Chance(0.75f) && NPC.DistanceSQ(player.Center) < 300 * 300)
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 9);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -80 : 80, 0);
                                if (AITimer < 200)
                                {
                                    if (NPC.DistanceSQ(ShootPos) < 160 * 160 || AITimer > 120)
                                    {
                                        AITimer = 200;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            Vector2 vector;
                                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                            vector.X = (float)(Math.Sin(angle) * 100);
                                            vector.Y = (float)(Math.Cos(angle) * 100);
                                            Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.Frost, 0f, 0f, 100, default, 1f)];
                                            dust2.noGravity = true;
                                            dust2.velocity = -NPC.DirectionTo(dust2.position) * 5f;
                                        }
                                        NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 7f, 14f, true);
                                    }
                                }
                                else
                                {
                                    NPC.velocity *= 0.9f;
                                    if (AITimer == 202)
                                    {
                                        NPC.Shoot(new Vector2(NPC.spriteDirection == 1 ? NPC.Center.X + 2 : NPC.Center.X - 2, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_Surge>(), 80, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Zap1", ai0: NPC.whoAmI);

                                        for (int i = 0; i < 18; i++)
                                            NPC.Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_Surge2>(), 0, RedeHelper.PolarVector(14, MathHelper.ToRadians(20) * i), false, SoundID.Item1.WithVolume(0));
                                    }
                                    if (AITimer > 232)
                                    {
                                        BodyState = (int)BodyAnim.Idle;
                                        chance -= Main.rand.NextFloat(0.4f, 0.7f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Shrug It Off
                        case 5:
                            if (AITimer == 0)
                            {
                                if (NPC.NPCHasAnyBuff())
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 9);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Shrug;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 21)
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
                                    NPC.netUpdate = true;
                                }
                                if (AITimer > 31)
                                {
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Deflect
                        case 6:
                            if (AITimer == 0)
                            {
                                if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()) &&
                                    (player.HeldItem.DamageType == DamageClass.Magic || player.HeldItem.DamageType == DamageClass.Ranged) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 9);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                if (AITimer++ % 20 == 0)
                                    ShootPos = new Vector2(player.Center.X > NPC.Center.X ? Main.rand.Next(-400, -300) : Main.rand.Next(300, 400), Main.rand.Next(-60, 60));

                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);
                                if (AITimer == 16)
                                    NPC.Shoot(new Vector2(NPC.Center.X + 48 * NPC.spriteDirection, NPC.Center.Y - 12), ModContent.ProjectileType<KS3_Reflect>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                                if (AITimer > 231)
                                    BodyState = (int)BodyAnim.ShieldOff;
                                else
                                    BodyState = (int)BodyAnim.ShieldOn;

                                if (AITimer > 261)
                                {
                                    chance -= Main.rand.NextFloat(0.2f, 1f);
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Missile Drones
                        case 7:
                            if (AITimer == 0)
                            {
                                if (phase > 0 && !NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 9);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Alarm2", NPC.whoAmI);
                                    if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()))
                                    {
                                        for (int i = 0; i < Main.rand.Next(2, 5); i++)
                                        {
                                            RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800),
                                                ModContent.NPCType<KS3_MissileDrone>(), NPC.whoAmI);
                                        }
                                    }
                                }
                                if (AITimer > 91)
                                {
                                    chance -= Main.rand.NextFloat(0.2f, 1f);
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Energy Magnet
                        case 8:
                            if (AITimer == 0)
                            {
                                if (phase > 1 && !NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 9);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Alarm2", NPC.whoAmI);
                                    if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()))
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800),
                                                ModContent.NPCType<KS3_Magnet>(), NPC.whoAmI);
                                        }
                                    }
                                }
                                if (AITimer > 91)
                                {
                                    chance -= Main.rand.NextFloat(0.7f, 1f);
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.PhysicalAttacks:
                    if (AttackChoice == 0)
                    {
                        AttackChoice = Main.rand.Next(1, 6);
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        NPC.netUpdate = true;
                    }
                    switch ((int)AttackChoice)
                    {
                        case -1:
                            NPC.LookAtEntity(player);
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 6);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                if (AITimer == 0)
                                {
                                    if (Main.rand.NextBool(2))
                                        AITimer = 1;
                                    else
                                        AITimer = 2;
                                    NPC.netUpdate = true;
                                }
                                NPC.velocity *= 0.9f;
                                if (AITimer == 1)
                                {
                                    if (BodyState is (int)BodyAnim.IdlePhysical)
                                        BodyState = (int)BodyAnim.Idle;

                                    if (BodyState is (int)BodyAnim.Idle && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.SpecialAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else if (AITimer == 2)
                                {
                                    if (BodyState is (int)BodyAnim.IdlePhysical)
                                    {
                                        ArmsFrameY = 1;
                                        ArmsFrameX = 0;
                                        BodyState = (int)BodyAnim.Gun;
                                    }
                                    if (BodyState is (int)BodyAnim.Gun && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.GunAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;

                        #region Guillotine Wheel Kick
                        case 1:
                            NPC.LookAtEntity(player);
                            if (AITimer++ <= 40)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -200 : 200, -60);
                                NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                            }
                            if (AITimer == 40)
                            {
                                NPC.frame.X = 7 * NPC.frame.Width;
                                NPC.frame.Y = 160;
                                BodyState = (int)BodyAnim.WheelkickStart;
                            }

                            if (AITimer > 40 && AITimer < 100)
                            {
                                if (AITimer % 15 == 0)
                                    SoundEngine.PlaySound(SoundID.Item1, NPC.position);

                                NPC.rotation += NPC.velocity.Y / 30;
                                ShootPos = new Vector2(player.velocity.X * 30, -600);
                                if (NPC.Center.Y < player.Center.Y - 600 || AITimer > 80)
                                {
                                    BodyState = (int)BodyAnim.Wheelkick;
                                    NPC.velocity *= 0.2f;
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 34f : 26f, 3f, true);
                            }
                            else if (AITimer >= 100 && AITimer < 200)
                            {
                                if (AITimer >= 100 && AITimer < 110)
                                {
                                    NPC.rotation = 0;
                                    NPC.velocity.Y -= 0.01f;
                                }
                                if (AITimer == 110)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Wave>(), 120, Vector2.Zero, false, SoundID.Item74, ai0: NPC.whoAmI);
                                    NPC.velocity.Y += 40f;
                                }
                                if (AITimer >= 110)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 44 + (8 * NPC.spriteDirection), (int)NPC.Center.Y - 42 - 5, 88, 84);
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(Hitbox))
                                            continue;

                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamagePlayer(target, 120, 3, hitDirection, NPC);
                                    }
                                }
                                if (AITimer > 130 || NPC.Center.Y > player.Center.Y + 400)
                                {
                                    AITimer = 200;
                                    BodyState = (int)BodyAnim.WheelkickEnd;
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (AITimer >= 200)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                if (AITimer > 220)
                                {
                                    ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -200 : 200, -60);
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 30f : 22f, 8f, true);
                                }
                                else
                                    NPC.velocity *= 0.8f;
                            }
                            if (AITimer > 280)
                            {
                                NPC.rotation = 0;
                                chance -= Main.rand.NextFloat(0.1f, 0.3f);
                                AITimer = 0;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Shoulder Bash
                        case 2:
                            AITimer++;
                            if (AITimer < 100)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                NPC.LookAtEntity(player);
                                if (NPC.DistanceSQ(ShootPos) < 50 * 50 || AITimer > 70)
                                {
                                    AITimer = 100;

                                    NPC.frame.Y = 160 * 5;
                                    NPC.frame.X = 0;
                                    BodyState = (int)BodyAnim.ShoulderBash;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -100 : 100, 0);
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                                }
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.rotation = 0;
                                NPC.velocity *= 0.8f;
                                if (AITimer == 101)
                                    NPC.velocity.X = player.Center.X > NPC.Center.X ? -6 : 6;

                                if (AITimer == 110)
                                    NPC.Dash(60, false, SoundID.Item74, player.Center);

                                if (AITimer >= 110 && AITimer <= 130)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 28 + (8 * NPC.spriteDirection), (int)NPC.Center.Y - 53 - 5, 56, 106);
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(Hitbox))
                                            continue;

                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamagePlayer(target, 120, 3, hitDirection, NPC);
                                        target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                                    }
                                }

                                if (AITimer == 130)
                                {
                                    NPC.velocity.X = -15 * NPC.spriteDirection;
                                    BodyState = (int)BodyAnim.ShoulderBashEnd;
                                }
                                if (AITimer > 160)
                                {
                                    NPC.LookAtEntity(player);
                                    chance -= Main.rand.NextFloat(0.05f, 0.2f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Hyperspear Dropkick
                        case 3:
                            NPC.LookAtEntity(player);
                            AITimer++;
                            if (AITimer < 100)
                            {
                                NPC.rotation = 0;
                                if (NPC.DistanceSQ(ShootPos) < 100 * 100 || AITimer > 50)
                                {
                                    ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -150 : 150, 200);
                                    AITimer = 100;
                                    NPC.velocity.X = 0;
                                    NPC.velocity.Y = -25;

                                    NPC.frame.X = 2 * NPC.frame.Width;
                                    NPC.frame.Y = 160;
                                    BodyState = (int)BodyAnim.DropkickStart;

                                    SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_TeleLine2>(), 0, NPC.DirectionTo(player.Center + player.velocity * 20f), false, SoundID.Item1.WithVolume(0), ai1: NPC.whoAmI);
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                            }
                            else if (AITimer >= 100 && AITimer < 200)
                            {
                                NPC.velocity *= 0.97f;
                                if (NPC.velocity.Length() < 6 || AITimer > 160)
                                {
                                    AITimer = 200;
                                    NPC.velocity *= 0f;
                                    BodyState = (int)BodyAnim.Dropkick;
                                    NPC.rotation = (player.Center + player.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (AITimer >= 200)
                            {
                                if (AITimer == 204)
                                    NPC.rotation = (player.Center + player.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);

                                if (AITimer >= 205)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 29, (int)NPC.Center.Y - 59, 58, 118);
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(Hitbox))
                                            continue;

                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamagePlayer(target, 156, 3, hitDirection, NPC);
                                        target.AddBuff(ModContent.BuffType<StaticStunDebuff>(), 120);
                                    }
                                }

                                if (AITimer == 205)
                                    NPC.Dash(40, true, SoundID.Item74, player.Center + player.velocity * 20f);

                                if (AITimer > 260 || NPC.Center.Y > player.Center.Y + 400)
                                {
                                    NPC.rotation = 0;
                                    NPC.velocity *= 0f;
                                    chance -= Main.rand.NextFloat(0.2f, 0.5f);
                                    BodyState = (int)BodyAnim.IdlePhysical;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Iron Pummel
                        case 4:
                            AITimer++;
                            NPC.rotation = NPC.velocity.X * 0.01f;
                            ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -60 : 60, 20);
                            if (AITimer < 100)
                            {
                                NPC.LookAtEntity(player);
                                if (NPC.DistanceSQ(ShootPos) < 50 * 50 || AITimer > 40)
                                {
                                    AITimer = 100;

                                    NPC.frame.X = 0;
                                    BodyState = Main.rand.Next(2) == 0 ? (int)BodyAnim.Pummel1 : (int)BodyAnim.Pummel2;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.velocity *= 0.9f;
                                if (AITimer == 105)
                                    NPC.Dash(10, false, SoundID.Item74, player.Center);

                                if (AITimer >= 105 && AITimer <= 115)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 6 + (28 * NPC.spriteDirection), (int)NPC.Center.Y - 6 - 18, 12, 12);
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(Hitbox))
                                            continue;

                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamagePlayer(target, 96, 3, hitDirection, NPC);
                                    }
                                }

                                if (AITimer == 125 && RedeHelper.Chance(0.4f))
                                {
                                    AITimer = 140;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer > 125)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                                }
                                if (AITimer > 140)
                                {
                                    if (RedeHelper.Chance(0.35f))
                                    {
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        NPC.LookAtEntity(player);
                                        chance -= Main.rand.NextFloat(0.05f, 0.1f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Hologram Flurry
                        case 5:
                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(6))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 6);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.LookAtEntity(player);
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                AITimer++;
                                ShootPos = new Vector2(player.Center.X > NPC.Center.X ? -80 : 80, 20);

                                if (AITimer == 5)
                                {
                                    NPC.frame.X = 7 * NPC.frame.Width;
                                    NPC.frame.Y = 160 * 2;
                                    BodyState = (int)BodyAnim.Jojo;
                                }

                                NPC.Move(ShootPos, NPC.Distance(player.Center) > 300 ? 20f : 9f, 8f, true);
                                if (AITimer >= 15 && AITimer % 3 == 0)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_JojoFist>(), 132, Vector2.Zero, false, SoundID.Item60.WithVolume(0.3f), ai0: NPC.whoAmI);
                                }
                                if (AITimer > 240)
                                {
                                    BodyState = (int)BodyAnim.IdlePhysical;
                                    chance -= Main.rand.NextFloat(0.05f, 0.2f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.PhaseTransition1:
                    #region Phase 1 Transition
                    NPC.LookAtEntity(player);
                    NPC.velocity *= 0.9f;
                    if (AITimer++ == 5)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                    if (RedeConfigClient.Instance.NoLoreElements || RedeBossDowned.slayerDeath >= 4)
                    {
                        if (AITimer == 30)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Alarm2", NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_MissileDrone>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 80)
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(2, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 1;
                            if (RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                            {
                                if (player.HeldItem.DamageType == DamageClass.Melee)
                                {
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("What a nuisance. It would seem my Auto-Shield is ineffective to your blades.\n'Twas meant to protect from high velocity blasts, I should change tactics.", 400, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                                    AttackChoice = 1;
                                }
                                else
                                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("What a nuisance. Your petty projectiles are going through my Auto-Shield.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                            }
                            else
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("What a nuisance. You are only wasting both of our efforts here.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AttackChoice == 1 ? AITimer == 420 : AITimer == 300 && !Main.dedServ)
                        {
                            HeadType = 2;
                            if (TeleportCount > 6)
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Why'd you summon me if you're just gonna run away the entire time?", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                            else
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Might as well blow you to pieces with a few missiles.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AttackChoice == 1 ? AITimer == 700 : AITimer == 580)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Alarm2", NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_MissileDrone>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AttackChoice == 1 ? AITimer > 740 : AITimer > 620)
                        {
                            if (RedeBossDowned.slayerDeath < 4)
                                RedeBossDowned.slayerDeath = 4;

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseTransition2:
                    #region Phase 2 Transition
                    NPC.LookAtEntity(player);
                    NPC.velocity *= 0.9f;
                    if (AITimer++ == 5)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                    if (RedeConfigClient.Instance.NoLoreElements || RedeBossDowned.slayerDeath >= 5)
                    {
                        if (AITimer == 30)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Alarm2", NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_Magnet>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 80)
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 4;
                            /*if (Main.LocalPlayer.GetModPlayer<RedePlayer>().omegaPower || player.IsFullTBot())
                            {
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("This rusty little tincan is more persistent than I thought...", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                            }
                            else if (BasePlayer.HasAccessory(player, ModContent.ItemType<CrownOfTheKing>(), true, true))
                            {
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("The concept of losing to a chicken does not bode well with me...", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                            }
                            else*/
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You pack more of a punch than I thought for such a small fleshbag...", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 300 && !Main.dedServ)
                        {
                            HeadType = 3;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I might even have to take you seriously...", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 480 && !Main.dedServ)
                        {
                            HeadType = 0;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("PAH! What a joke!", 120, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 600 && !Main.dedServ)
                        {
                            HeadType = 2;
                            if (player.HeldItem.DamageType == DamageClass.Ranged || player.HeldItem.DamageType == DamageClass.Magic)
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You like shooting things, correct? Well try shooting me now.", 240, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                            else
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Go ahead, shoot me if you can.", 240, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 840)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Alarm2", NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_Magnet>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 880)
                        {
                            if (RedeBossDowned.slayerDeath < 5)
                                RedeBossDowned.slayerDeath = 5;

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseTransition3:
                    #region Phase 3 Transition
                    NPC.LookAtEntity(player);
                    NPC.velocity *= 0.9f;
                    if (AITimer++ == 5)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                    if (RedeConfigClient.Instance.NoLoreElements || RedeBossDowned.slayerDeath >= 6)
                    {
                        if (AITimer > 80)
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 2;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("This is getting ridiculous! Why can't I kill you?", 240, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 260 && !Main.dedServ)
                        {
                            HeadType = 3;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("*Ahem* Your persistence is admirable, I'll give you that.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 540 && !Main.dedServ)
                        {
                            HeadType = 2;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("But you better realise I'm hardly trying. I ain't bluffing either.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer > 840)
                        {
                            if (RedeBossDowned.slayerDeath < 6)
                                RedeBossDowned.slayerDeath = 6;

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseTransition4:
                    #region Phase 4 Transition
                    NPC.LookAtEntity(player);
                    NPC.velocity *= 0.9f;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    if (AITimer++ == 5)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                    if (RedeBossDowned.slayerDeath >= 7)
                    {
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 3;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("If you stop attacking, I'll go back to more IMPORTANT business.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer > 300)
                        {
                            NPC.life = 1;
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<ProjDeath>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f));
                            HeadType = 0;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = ActionState.SpareCountdown;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 0;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Alright alright alright!", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 200 && !Main.dedServ)
                        {
                            HeadType = 1;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("We'll... call it a draw then.", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 380 && !Main.dedServ)
                        {
                            HeadType = 1;
                            if (TeleportCount > 16)
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You've just been flying away the entire fight. Seriously.", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                            else
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I'm too tired to get mad about this nonsense.", 200, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 580 && !Main.dedServ)
                        {
                            HeadType = 2;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("If you stop attacking, I'll go back to more IMPORTANT business.", 260, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer == 840 && !Main.dedServ)
                        {
                            HeadType = 3;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("But, if you so choose, we can continue... But I won't be happy if I lose.", 280, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                        if (AITimer > 1120)
                        {
                            NPC.life = 1;
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<ProjDeath>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f));
                            HeadType = 0;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = ActionState.SpareCountdown;
                            NPC.netUpdate = true;
                        }
                    }
                    #endregion
                    break;
                case ActionState.SpareCountdown:
                    NPC.LookAtEntity(player);
                    NPC.chaseable = false;
                    NPC.dontTakeDamage = false;
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    AITimer++;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (!Main.dedServ)
                    {
                        if (AITimer == 10) RedeSystem.Instance.DialogueUIElement.DisplayDialogue("5", 60, 1, 0.6f, null, 0, null, null, null, null, 0, 1);
                        if (AITimer == 70) RedeSystem.Instance.DialogueUIElement.DisplayDialogue("4", 60, 1, 0.6f, null, 0, null, null, null, null, 0, 1);
                        if (AITimer == 130) RedeSystem.Instance.DialogueUIElement.DisplayDialogue("3", 60, 1, 0.6f, null, 0, null, null, null, null, 0, 1);
                        if (AITimer == 190) RedeSystem.Instance.DialogueUIElement.DisplayDialogue("2", 60, 1, 0.6f, null, 0, null, null, null, null, 0, 1);
                        if (AITimer == 250) RedeSystem.Instance.DialogueUIElement.DisplayDialogue("1", 60, 1, 0.6f, null, 0, null, null, null, null, 0, 1);
                    }
                    if (AITimer >= 310)
                    {
                        if (RedeBossDowned.slayerDeath < 7)
                            RedeBossDowned.slayerDeath = 7;

                        NPC.dontTakeDamage = true;
                        AITimer = 0;
                        AIState = ActionState.Spared;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacked:
                    #region Attacked
                    NPC.LookAtEntity(player);
                    NPC.chaseable = true;
                    NPC.dontTakeDamage = true;
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (AITimer++ == 5)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0f), ai0: NPC.whoAmI);

                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 1;
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I see how it is...", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                    }
                    if (AITimer == 180)
                        RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 0);

                    if (AITimer == 190)
                        RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 1);

                    if (AITimer == 200)
                        RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 2);

                    if (AITimer == 210)
                        RedeHelper.SpawnNPC((int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 3);

                    if (AITimer > 400)
                    {
                        if (NPC.life < 10000)
                        {
                            int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<HealDust>());
                            Main.dust[dustIndex].velocity.Y = -3;
                            Main.dust[dustIndex].velocity.X = 0;
                            Main.dust[dustIndex].noGravity = true;
                            NPC.life += 200;
                            NPC.HealEffect(200);
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.dontTakeDamage = false;
                            AITimer = 0;
                            AIState = ActionState.Overclock;
                            NPC.life = 10000;
                            NPC.netUpdate = true;
                        }
                    }
                    #endregion
                    break;
                case ActionState.Spared:
                    #region Spared
                    NPC.LookAtEntity(player);
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    NPC.dontTakeDamage = true;
                    AITimer++;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 0;
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Tie it is then. Now don't distract me again.", 260, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                    }
                    if (AITimer == 290 && !Main.dedServ)
                    {
                        HeadType = 1;
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Adios, dingus.", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                    }
                    if (AITimer > 470)
                    {
                        NPC.dontTakeDamage = false;
                        if (RedeBossDowned.slayerDeath < 7)
                            RedeBossDowned.slayerDeath = 7;

                        player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                        NPC.netUpdate = true;
                    }
                    #endregion
                    break;
                case ActionState.Overclock:
                    NPC.LookAtEntity(player);
                    NPC.dontTakeDamage = true;
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    AITimer++;
                    if (AITimer < 30 && !Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
                    else
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossSlayer2");

                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 2;
                        if (RedeBossDowned.slayerDeath >= 8)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Once again, you really are eager to win...", 220, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        else
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I'm disappointed I actually have to overclock this vessel...", 220, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                    }
                    if (AITimer == 250 && !Main.dedServ)
                    {
                        HeadType = 4;
                        if (RedeBossDowned.slayerDeath >= 8)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("... I guess you like doing things the hard way.", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        else
                        {
                            /*if (Main.LocalPlayer.GetModPlayer<RedePlayer>().omegaPower || player.IsFullTBot())
                            {
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("... And for a heap of scrap no less.", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                            }
                            else if (BasePlayer.HasAccessory(player, ModContent.ItemType<CrownOfTheKing>(), true, true))
                            {
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("... And for what? A bloody chicken!?", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, 0);
                            }
                            else*/
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("... And for an annoying brat no less.", 180, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                        }
                    }
                    if (AITimer == 430 && !Main.dedServ)
                    {
                        HeadType = 0;
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Let's begin.", 70, 1, 0.6f, "King Slayer III:", 0.4f, RedeColor.SlayerColour, null, null, NPC.Center, sound: true);
                    }
                    if (AITimer >= 500)
                    {
                        if (RedeBossDowned.slayerDeath < 8)
                            RedeBossDowned.slayerDeath = 8;

                        NPC.dontTakeDamage = false;
                        NPC.chaseable = true;
                        phase = 5;
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        AttackChoice = 1;
                        AITimer = 0;
                        AIState = ActionState.GunAttacks;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            if (MoRDialogueUI.Visible && RedeSystem.Instance.DialogueUIElement.PointPos == NPC.Center)
            {
                if (RedeSystem.Instance.DialogueUIElement.ID == 0)
                {
                    RedeSystem.Instance.DialogueUIElement.PointPos = NPC.Center;
                    RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.SlayerColour;
                }
                else
                {
                    RedeSystem.Instance.DialogueUIElement.PointPos = null;
                    RedeSystem.Instance.DialogueUIElement.TextColor = null;
                }
            }
            #region Teleporting
            if (NPC.DistanceSQ(player.Center) >= 1100 * 1100 && NPC.ai[0] > 0 && !player.GetModPlayer<ScreenPlayer>().lockScreen)
            {
                if (AttackChoice == 3 && AIState is ActionState.PhysicalAttacks)
                    return;
                TeleportCount++;
                Teleport(false, Vector2.Zero);
                NPC.netUpdate = true;
            }
            #endregion
        }

        public override bool CheckDead()
        {
            if (phase >= 5 || RedeConfigClient.Instance.NoLoreElements || AIState is ActionState.Spared)
                return true;
            else
            {
                if (NPC.ai[0] == 10)
                {
                    AITimer = 0;
                    AIState = ActionState.Attacked;
                    NPC.netUpdate = true;
                }
                NPC.life = 1;
                NPC.netUpdate = true;
                return false;
            }
        }

        #region Methods
        void SnapGunToFiringArea()
        {
            //set bpoundries
            float firingRegionCenter = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
            float minFiringRegion = firingRegionCenter - gunRotLimit / 2f;
            float maxFiringRegion = firingRegionCenter + gunRotLimit / 2f;

            //convert gunRot to equivilent angles within a certian range
            while (gunRot < -(float)Math.PI / 2)
            {
                gunRot += (float)Math.PI * 2f;
            }
            while (gunRot > 3f * (float)Math.PI / 2f)
            {
                gunRot -= (float)Math.PI * 2f;
            }

            //detect if gun points outside the firing area
            if (gunRot > maxFiringRegion || gunRot < minFiringRegion)
            {
                float distFromMin = RedeHelper.AngularDifference(minFiringRegion, gunRot);
                float distFromMax = RedeHelper.AngularDifference(maxFiringRegion, gunRot);

                if (distFromMin < distFromMax)
                {
                    gunRot = minFiringRegion;
                }
                else
                {
                    gunRot = maxFiringRegion;
                }
            }
        }

        public void Teleport(bool specialPos, Vector2 teleportPos)
        {
            TeleGlow = true;
            TeleGlowTimer = 0;
            TeleVector = NPC.Center;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!specialPos)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            Vector2 newPos = new(Main.rand.Next(-400, -250), Main.rand.Next(-200, 50));
                            NPC.Center = Main.player[NPC.target].Center + newPos;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            Vector2 newPos2 = new(Main.rand.Next(250, 400), Main.rand.Next(-200, 50));
                            NPC.Center = Main.player[NPC.target].Center + newPos2;
                            NPC.netUpdate = true;
                            break;
                    }
                }
                else
                {
                    NPC.Center = Main.player[NPC.target].Center + teleportPos;
                    NPC.netUpdate = true;
                }
            }
            TeleVector = NPC.Center;
            SoundEngine.PlaySound(SoundID.Item74, NPC.position);
            for (int i = 0; i < 30; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 3f);
                Main.dust[dustIndex].velocity *= 6f;
                Main.dust[dustIndex].noGravity = true;
            }
        }
        #endregion

        private int ArmsFrameY;
        private int ArmsFrameX;
        private int ArmsCounter;
        private int HeadFrame;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (phase >= 5)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].velocity.X = 0f;
                    Main.dust[dustIndex].velocity.Y = -5f;
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

                for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                {
                    oldrot[k] = oldrot[k - 1];
                }
                oldrot[0] = NPC.rotation;

                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 8;
                if (BodyState < (int)BodyAnim.IdlePhysical)
                {
                    if (NPC.velocity.Length() < 13f)
                    {
                        NPC.frame.Y = 0;
                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 3 * NPC.frame.Width)
                                NPC.frame.X = 0;
                        }
                    }
                    else
                    {
                        NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, 0, frameHeight);
                        if (NPC.frame.Y is 0 && NPC.frame.X < 4 * NPC.frame.Width)
                            NPC.frame.X = 4 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                            {
                                NPC.frame.X = 0;
                                NPC.frame.Y = frameHeight;
                            }
                            if (NPC.frame.Y == frameHeight && NPC.frame.X > 1 * NPC.frame.Width)
                            {
                                NPC.frame.X = 6 * NPC.frame.Width;
                                NPC.frame.Y = 0;
                            }
                        }
                    }
                }

                #region Body

                switch (BodyState)
                {
                    case (int)BodyAnim.Idle:
                        ArmsFrameY = 0;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 3)
                                ArmsFrameX = 0;
                        }
                        break;
                    case (int)BodyAnim.Crossed:
                        ArmsFrameY = 0;
                        if (ArmsFrameX < 4)
                            ArmsFrameX = 4;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 7)
                                ArmsFrameX = 4;
                        }
                        break;
                    case (int)BodyAnim.Gun:
                        ArmsFrameY = 1;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 6)
                                ArmsFrameX = 6;
                        }
                        break;
                    case (int)BodyAnim.GunShoot:
                        ArmsFrameY = 1;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 8)
                            {
                                ArmsFrameX = 6;
                                BodyState = (int)BodyAnim.Gun;
                            }
                        }
                        break;
                    case (int)BodyAnim.GunEnd:
                        ArmsFrameY = 1;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX--;
                            if (ArmsFrameX < 0)
                            {
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Idle;
                            }
                        }
                        break;
                    case (int)BodyAnim.RocketFist:
                        ArmsFrameY = 3;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 7)
                            {
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Idle;
                            }
                        }
                        break;
                    case (int)BodyAnim.Grenade:
                        ArmsFrameY = 5;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 5)
                            {
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Idle;
                            }
                        }
                        break;
                    case (int)BodyAnim.Charging:
                        ArmsFrameY = 2;
                        if (ArmsFrameX < 5)
                            ArmsFrameX = 5;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 9)
                                ArmsFrameX = 7;
                        }
                        break;
                    case (int)BodyAnim.Shrug:
                        ArmsFrameY = 4;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 6)
                            {
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Idle;
                            }
                        }
                        break;
                    case (int)BodyAnim.ShieldOn:
                        ArmsFrameY = 2;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX++;
                            if (ArmsFrameX > 4)
                                ArmsFrameX = 3;
                        }
                        break;
                    case (int)BodyAnim.ShieldOff:
                        ArmsFrameY = 2;
                        if (ArmsCounter++ >= 5)
                        {
                            ArmsCounter = 0;
                            ArmsFrameX--;
                            if (ArmsFrameX < 0)
                            {
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Idle;
                            }
                        }
                        break;
                    case (int)BodyAnim.DropkickStart:
                        NPC.frame.Y = frameHeight;
                        if (NPC.frame.X < 2 * NPC.frame.Width)
                            NPC.frame.X = 2 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 5 * NPC.frame.Width)
                                NPC.frame.X = 2 * NPC.frame.Width;
                        }
                        break;
                    case (int)BodyAnim.Dropkick:
                        NPC.frame.Y = frameHeight;
                        NPC.frame.X = 6 * NPC.frame.Width;
                        NPC.frameCounter = 0;
                        Vector2 position = NPC.Center + (Vector2.Normalize(NPC.velocity) * 30f);
                        Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 2f)];
                        dust.position = position;
                        dust.velocity = (NPC.velocity.RotatedBy(1.5708) * 0.33f) + (NPC.velocity / 4f);
                        dust.position += NPC.velocity.RotatedBy(1.5708);
                        dust.fadeIn = 0.5f;
                        dust.noGravity = true;
                        dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 2f)];
                        dust.position = position;
                        dust.velocity = (NPC.velocity.RotatedBy(-1.5708) * 0.33f) + (NPC.velocity / 4f);
                        dust.position += NPC.velocity.RotatedBy(-1.5708);
                        dust.fadeIn = 0.5f;
                        dust.noGravity = true;
                        break;
                    case (int)BodyAnim.WheelkickStart:
                        NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, frameHeight, 2 * frameHeight);
                        if (NPC.frame.Y == frameHeight && NPC.frame.X < 7 * NPC.frame.Width)
                            NPC.frame.X = 7 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                            {
                                NPC.frame.X = 0;
                                NPC.frame.Y = 2 * frameHeight;
                            }
                            if (NPC.frame.Y == 2 * frameHeight && NPC.frame.X > 3 * NPC.frame.Width)
                            {
                                NPC.frame.X = 7 * NPC.frame.Width;
                                NPC.frame.Y = frameHeight;
                            }
                        }
                        break;
                    case (int)BodyAnim.Wheelkick:
                        NPC.frame.Y = 2 * frameHeight;
                        NPC.frame.X = 4 * NPC.frame.Width;
                        NPC.frameCounter = 0;
                        break;
                    case (int)BodyAnim.WheelkickEnd:
                        NPC.frame.Y = 2 * frameHeight;
                        if (NPC.frame.X < 5 * NPC.frame.Width)
                            NPC.frame.X = 5 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 6 * NPC.frame.Width)
                            {
                                NPC.frame.Y = 4 * frameHeight;
                                NPC.frame.X = 4 * NPC.frame.Width;
                                BodyState = (int)BodyAnim.IdlePhysical;
                            }
                        }
                        break;
                    case (int)BodyAnim.Jojo:
                        NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, 2 * frameHeight, 3 * frameHeight);
                        if (NPC.frame.Y == 2 * frameHeight && NPC.frame.X < 7 * NPC.frame.Width)
                            NPC.frame.X = 7 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                            {
                                NPC.frame.X = 0;
                                NPC.frame.Y = 3 * frameHeight;
                            }
                            if (NPC.frame.Y == 3 * frameHeight && NPC.frame.X > 4 * NPC.frame.Width)
                            {
                                NPC.frame.X = 1 * NPC.frame.Width;
                                NPC.frame.Y = 3 * frameHeight;
                            }
                        }
                        break;
                    case (int)BodyAnim.Pummel1:
                        NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, 3 * frameHeight, 4 * frameHeight);
                        if (NPC.frame.Y == 3 * frameHeight && NPC.frame.X < 6 * NPC.frame.Width)
                            NPC.frame.X = 6 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                            {
                                NPC.frame.X = 0;
                                NPC.frame.Y = 4 * frameHeight;
                            }
                            if (NPC.frame.Y == 4 * frameHeight && NPC.frame.X > 0 * NPC.frame.Width)
                            {
                                NPC.frame.Y = 4 * frameHeight;
                                NPC.frame.X = 4 * NPC.frame.Width;
                                BodyState = (int)BodyAnim.IdlePhysical;
                            }
                        }
                        break;
                    case (int)BodyAnim.Pummel2:
                        NPC.frame.Y = 4 * frameHeight;
                        if (NPC.frame.X < 1 * NPC.frame.Width)
                            NPC.frame.X = 1 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 3 * NPC.frame.Width)
                            {
                                NPC.frame.Y = 4 * frameHeight;
                                NPC.frame.X = 4 * NPC.frame.Width;
                                BodyState = (int)BodyAnim.IdlePhysical;
                            }
                        }
                        break;
                    case (int)BodyAnim.IdlePhysical:
                        NPC.frame.Y = 4 * frameHeight;
                        if (NPC.frame.X < 4 * NPC.frame.Width)
                            NPC.frame.X = 4 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                                NPC.frame.X = 4;
                        }
                        break;
                    case (int)BodyAnim.ShoulderBash:
                        NPC.frame.Y = 5 * frameHeight;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 6 * NPC.frame.Width)
                                NPC.frame.X = 2;
                        }
                        break;
                    case (int)BodyAnim.ShoulderBashEnd:
                        NPC.frame.Y = 5 * frameHeight;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X -= NPC.frame.Width;
                            if (NPC.frame.X < 0 * NPC.frame.Width)
                            {
                                NPC.frame.Y = 4 * frameHeight;
                                NPC.frame.X = 4 * NPC.frame.Width;
                                BodyState = (int)BodyAnim.IdlePhysical;
                            }
                        }
                        break;
                }
                #endregion

                switch (HeadType)
                {
                    case 0: // Normal
                        HeadFrame = NPC.frame.X / NPC.frame.Width;
                        break;
                    case 1: // Bored
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 4;
                        break;
                    case 2: // Angry
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 8;
                        break;
                    case 3: // Suspicious
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 12;
                        break;
                    case 4: // Confused
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 16;
                        break;
                }
            }
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity *= 0.96f;
                NPC.velocity.Y -= 1;
                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;
                return;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D Arms = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms").Value;
            Texture2D ArmsGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms_Glow").Value;
            Texture2D Head = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Heads").Value;
            Texture2D HeadGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Heads_Glow").Value;

            Texture2D Overclock = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Overclock").Value;
            Texture2D OverclockGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Overclock_Glow").Value;
            Texture2D OverclockArms = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms_Overclock").Value;
            Texture2D OverclockArmsGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms_Overclock_Glow").Value;

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(phase < 5 ? TextureAssets.Npc[NPC.type].Value : Overclock, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(RedeColor.SlayerColour) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
            }
            spriteBatch.Draw(phase < 5 ? TextureAssets.Npc[NPC.type].Value : Overclock, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(phase < 5 ? Glow : OverclockGlow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (AIState != ActionState.GunAttacks && AIState != ActionState.PhysicalAttacks && AIState != ActionState.SpecialAttacks && NPC.velocity.Length() < 13f && phase < 5)
            {
                Vector2 HeadPos = new(NPC.Center.X - 2 * NPC.spriteDirection, NPC.Center.Y - 35);
                int HeadHeight = Head.Height / 20;
                int yHead = HeadHeight * HeadFrame;
                Rectangle HeadRect = new(0, yHead, Head.Width, HeadHeight);
                spriteBatch.Draw(Head, HeadPos - screenPos, new Rectangle?(HeadRect), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(Head.Width / 2f, HeadHeight / 2f), NPC.scale, effects, 0f);
                spriteBatch.Draw(HeadGlow, HeadPos - screenPos, new Rectangle?(HeadRect), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(Head.Width / 2f, HeadHeight / 2f), NPC.scale, effects, 0f);
            }

            if (BodyState < (int)BodyAnim.IdlePhysical)
            {
                int height = Arms.Height / 6;
                int width = Arms.Width / 10;
                int y = height * ArmsFrameY;
                int x = width * ArmsFrameX;
                Rectangle ArmsRect = new(x, y, width, height);
                Vector2 ArmsOrigin = new(width / 2f, height / 2f);
                Vector2 ArmsPos = new(NPC.Center.X, NPC.Center.Y - 13);

                spriteBatch.Draw(phase < 5 ? Arms : OverclockArms, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(drawColor),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                spriteBatch.Draw(phase < 5 ? ArmsGlow : OverclockArmsGlow, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(Color.White),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D teleportGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position = TeleVector - screenPos;
            Color colour = Color.Lerp(Color.White, Color.Cyan, 1f / TeleGlowTimer * 10f) * (1f / TeleGlowTimer * 10f);
            if (TeleGlow)
            {
                spriteBatch.Draw(teleportGlow, position, new Rectangle?(rect), colour, NPC.rotation, origin, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(teleportGlow, position, new Rectangle?(rect), colour * 0.4f, NPC.rotation, origin, 2f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}