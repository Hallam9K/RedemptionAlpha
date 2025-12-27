using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.UI.Elements.Base;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.CrossMod;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Items;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Particles;
using Redemption.Textures;
using Redemption.UI;
using Redemption.UI.ChatUI;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    [AutoloadBossHead]
    public class KS3 : ModRedeNPC
    {
        public static Asset<Texture2D> Glow;
        public static Asset<Texture2D> Arms;
        public static Asset<Texture2D> ArmsGlow;
        public static Asset<Texture2D> Head;
        public static Asset<Texture2D> HeadGlow;
        private Asset<Texture2D> Overclock;
        private Asset<Texture2D> OverclockGlow;
        private Asset<Texture2D> OverclockArmsGlow;
        private Asset<Texture2D> OverclockHead;
        private Asset<Texture2D> OverclockHeadGlow;

        private readonly string TexturePath = "Redemption/NPCs/Bosses/KSIII/KS3";
        public override void Load()
        {
            if (Main.dedServ)
                return;
            Glow = Request<Texture2D>(TexturePath + "_Glow");
            Arms = Request<Texture2D>(TexturePath + "_Arms");
            ArmsGlow = Request<Texture2D>(TexturePath + "_Arms_Glow");
            Head = Request<Texture2D>(TexturePath + "_Heads");
            HeadGlow = Request<Texture2D>(TexturePath + "_Heads_Glow");
        }
        public enum ActionState
        {
            PlayerKilled = -1,
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
            Overclock,
            FriendlyDecline,
            FriendlyAccept,
            FriendlyWin,
            OverclockSubphase1,
            OverclockSubphase2,
            OverclockSubphase3,
            OverclockEnd,
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
            // DisplayName.SetDefault("King Slayer III");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.UsesNewTargetting[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0, 36),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 36500;
            NPC.defense = 40;
            NPC.damage = 90;
            NPC.width = 42;
            NPC.height = 106;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 15, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.BossBar = GetInstance<KS3HealthBar>();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TitaniumWill");
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Psychic] *= 1.25f;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.velocity.Length() >= 13f && AIState is not ActionState.PhysicalAttacks;
        }
        public override bool CanHitNPC(NPC target)
        {
            return NPC.velocity.Length() >= 13f && AIState is not ActionState.PhysicalAttacks;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.KS3"))
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<SlayerBag>()));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<KS3Relic>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<KS3Trophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<SlayerProjector>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ItemType<KingSlayerMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(2, 1, ItemType<SlayerGun>(), ItemType<Nanoswarmer>(), ItemType<SlayerFist>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<SlayerController>(), 10));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<Holokey>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<CyberPlating>(), 1, 14, 18));

            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
            if (NPC.ai[0] != 11)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemType<SlayerMedal>());

            if (!RedeBossDowned.downedSlayer)
            {
                RedeQuest.adviceSeen[(int)RedeQuest.Advice.Androids] = true;
                RedeQuest.SyncData();

                if (NPC.ai[0] == 11)
                    RedeWorld.Alignment += 0;
                else
                    RedeWorld.Alignment -= 2;

                if (AIState is ActionState.Spared)
                    ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromKey("Mods.Redemption.UI.Chalice.KS3Spared"), 240, 30, 0, Color.DarkGoldenrod);
                else
                    ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromKey("Mods.Redemption.UI.Chalice.KS3Defeat"), 360, 30, 0, Color.DarkGoldenrod);
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSlayer, -1);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.DamageType == DamageClass.Melee)
            {
                if (AIState == ActionState.PhysicalAttacks)
                    modifiers.FinalDamage *= 1.65f;
                else if (AIState == ActionState.SpecialAttacks)
                    modifiers.FinalDamage *= 1.25f;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Redemption().TechnicallyMelee)
            {
                if (AIState == ActionState.PhysicalAttacks)
                    modifiers.FinalDamage *= 1.65f;
                else if (AIState == ActionState.SpecialAttacks)
                    modifiers.FinalDamage *= 1.25f;
            }
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (phase >= 5)
            {
                if (AIState is ActionState.OverclockSubphase1 or ActionState.OverclockSubphase2 or ActionState.OverclockSubphase3)
                    modifiers.FinalDamage *= .1f;
                else
                    modifiers.FinalDamage *= 1.5f;
            }
            else
                modifiers.FinalDamage *= 0.85f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(chance);
            writer.Write(phase);
            writer.Write(BodyState);
            writer.Write(gunRot);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            chance = reader.ReadSingle();
            phase = reader.ReadInt32();
            BodyState = reader.ReadInt32();
            gunRot = reader.ReadSingle();
        }

        public override bool BlacklistNPCTargets(NPC target) => target.townNPC;

        public float chance = 0.8f;
        public int phase;
        public int HeadType;
        public int BodyState;
        public enum BodyAnim
        {
            Idle, Crossed, Gun, GunShoot, GunEnd, RocketFist, Grenade, Charging, Shrug, ShieldOn, ShieldOff, IdlePhysical, WheelkickStart, Wheelkick, WheelkickEnd, ShoulderBash, ShoulderBashEnd, DropkickStart, Dropkick, Pummel1, Pummel2, Jojo
        }

        const float gunRotLimit = (float)Math.PI / 2;
        public float gunRot;
        const float headRotLimit = (float)Math.PI / 6;
        protected float HeadRotation;
        public Vector2 ShootPos;

        public int TeleportCount;
        public float TeleGlowTimer;
        public bool TeleGlow;
        public Vector2 TeleVector;
        public Vector2 oldTeleVector;

        float zoomTimer;

        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Slayer.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.2f };
        public readonly Vector2 modifier = new(0, -260);
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, .3f, .6f, .8f);

            float defaultGunRot = NPC.spriteDirection == 1 ? 0.2f : (float)Math.PI - 0.2f;

            TargetPlayerByDefault();
            SetPlayerTarget();
            Player player = GetPlayerTarget();
            Entity attacker = Attacker();

            if (AIState is not ActionState.PlayerKilled and not ActionState.OverclockEnd && (!player.active || player.dead))
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (BodyState is (int)BodyAnim.Gun)
                        BodyState = (int)BodyAnim.GunEnd;
                    else if (BodyState is not (int)BodyAnim.Idle && BodyState is not (int)BodyAnim.GunEnd)
                        BodyState = (int)BodyAnim.Idle;

                    gunRot = defaultGunRot;
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    HeadType = 0;
                    AIState = ActionState.PlayerKilled;
                    AITimer = 0;
                    TimerRand = 0;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    return;
                }
            }

            if (AIState > ActionState.PhysicalAttacks || AIState is ActionState.Dialogue)
                NPC.DiscourageDespawn(120);

            chance = MathHelper.Clamp(chance, 0, 1);
            if (NPC.type != NPCType<KS3_Clone>())
            {
                if (NPC.life < (int)(NPC.lifeMax * 0.75f) && phase < 1)
                    AIState = ActionState.PhaseChange;
                else if (NPC.life < NPC.lifeMax / 2 && phase < 2)
                    AIState = ActionState.PhaseChange;
                else if (NPC.life < NPC.lifeMax / 4 && phase < 3)
                    AIState = ActionState.PhaseChange;
                else if (NPC.life < (int)(NPC.lifeMax * 0.05f) && phase < 4)
                    AIState = ActionState.PhaseChange;

                if (phase >= 5)
                {
                    if (NPC.life < (int)(NPC.lifeMax * 0.75f) && phase < 6)
                        AIState = ActionState.PhaseChange;
                    else if (NPC.life < NPC.lifeMax / 2 && phase < 7)
                        AIState = ActionState.PhaseChange;
                    else if (NPC.life < NPC.lifeMax / 4 && phase < 8)
                        AIState = ActionState.PhaseChange;
                    else if (NPC.life < (int)(NPC.lifeMax * 0.05f) && phase < 9)
                        AIState = ActionState.PhaseChange;
                }
            }
            Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(54, gunRot) + RedeHelper.PolarVector(13 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
            int dmgIncrease = NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 10 : 0;
            Vector2 targetHead = new(attacker.Center.X, attacker.position.Y + (attacker.height / 3));

            switch (AIState)
            {
                case ActionState.PlayerKilled:
                    if (RedeBossDowned.slayerDeath2 == 0 && NPC.type == NPCType<KS3>())
                    {
                        gunRot = defaultGunRot;
                        HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                        if (NPC.DistanceSQ(player.Center) >= 400 * 400)
                            NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                        else
                            NPC.velocity *= 0.9f;
                        NPC.rotation = NPC.velocity.X * 0.01f;

                        if (AITimer++ == 30)
                        {
                            string line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.PlayerKilled.0");
                            string line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.PlayerKilled.1");

                            DialogueChain chain = new();
                            chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .1f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer > 5000 || !player.dead)
                        {
                            if (RedeBossDowned.slayerDeath2 < 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.slayerDeath2 = 1;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
                            NPC.active = false;
                        }

                    }
                    else
                    {
                        NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
                        NPC.active = false;
                    }
                    return;
                case ActionState.Begin:
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    NPC.LookAtEntity(player);
                    BodyState = (int)BodyAnim.Crossed;
                    player.RedemptionScreen().Rumble(5, 5);
                    TeleVector = NPC.Center;
                    oldTeleVector = NPC.Center;
                    TeleGlow = true;
                    if (AITimer++ > 5)
                    {
                        FablesHelper.DisplayBossIntroCard("Mods.Redemption.TitleCard.KS3.Name", "Mods.Redemption.TitleCard.KS3.Modifier", 120, false, Color.Cyan, Color.Cyan, Color.Cyan, Color.DarkBlue, "Titanium Will", "Sc0p3r");

                        AITimer = 0;
                        AIState = ActionState.Dialogue;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Dialogue:
                    #region Dialogue Moment
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    NPC.LookAtEntity(player);
                    gunRot = defaultGunRot;
                    AITimer++;
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 1200);
                    if (RedeBossDowned.slayerDeath < 3)
                    {
                        if (AITimer == 30)
                        {
                            HeadType = 2;
                            if (!Main.dedServ)
                            {
                                string line1;
                                if (RedeHelper.AnyProjectiles(ProjectileType<KS3_DroneKillCheck>()))
                                    line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.DroneBreak");
                                else
                                {
                                    if (RedeWorld.Alignment >= 0)
                                    {
                                        if (player.IsFullTBot())
                                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.DroneIfRobot");
                                        else if (player.RedemptionPlayerBuff().ChickenForm)
                                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.DroneIfChicken");
                                        else
                                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.DroneHuman");
                                    }
                                    else
                                    {
                                        if (player.IsFullTBot())
                                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.NoDroneIfRobot");
                                        else if (player.RedemptionPlayerBuff().ChickenForm)
                                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.NoDroneIfChicken");
                                        else
                                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.NoDroneIfHuman");
                                    }
                                }
                                string line2;
                                if (RedeHelper.AnyProjectiles(ProjectileType<KS3_DroneKillCheck>()))
                                    line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.Start");
                                else
                                {
                                    if (RedeWorld.Alignment >= 0)
                                    {
                                        line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.StartGood");
                                        if (player.Redemption().slayerStarRating >= 6)
                                            line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.StartSSR");
                                    }
                                    else
                                        line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.StartEvil");
                                }
                                player.Redemption().slayerStarRating = 0;

                                DialogueChain chain = new();
                                chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                     .Add(new(NPC, "[@h1]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, !RedeBossDowned.downedKeeper, null, Bubble, null, modifier, RedeBossDowned.downedKeeper ? 0 : 1));
                                if (RedeBossDowned.downedKeeper)
                                {
                                    chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.KeeperDowned1"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier));
                                    chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.KeeperDowned2"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier));
                                    chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Intro.KeeperDowned3"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                                }
                                chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                chain.OnEndTrigger += Chain_OnEndTrigger;
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);
                            }
                        }
                        if (AITimer == 5001)
                        {
                            ArmsFrameY = 1;
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Gun;
                            HeadType = 0;
                            NPC.netUpdate = true;
                        }
                        if (AITimer >= 5060)
                        {
                            ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));
                            if (RedeBossDowned.slayerDeath < 3 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.slayerDeath = 3;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);
                            HeadType = 0;
                            AITimer = 0;
                            AIState = ActionState.GunAttacks;
                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        if (AITimer == 30 && !Main.dedServ)
                        {
                            if (RedeBossDowned.slayerDeath2 == 1)
                            {
                                string line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.PlayerKilled.2");
                                string line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.PlayerKilled.3G");
                                if (RedeWorld.Alignment < 0)
                                    line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.PlayerKilled.3B");

                                DialogueChain chain = new();
                                chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, false, null, Bubble, null, modifier))
                                     .Add(new(NPC, line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                                chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                chain.OnEndTrigger += Chain_OnEndTrigger;
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);

                                if (RedeBossDowned.slayerDeath2 < 2 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    RedeBossDowned.slayerDeath2 = 2;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.WorldData);
                                }
                            }
                            else
                            {
                                string line1;
                                if (RedeBossDowned.downedSlayer)
                                {
                                    line1 = Main.rand.Next(5) switch
                                    {
                                        1 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.Downed1"),
                                        2 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.Downed2"),
                                        3 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.Downed3"),
                                        4 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.Downed4"),
                                        _ => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.Downed5"),
                                    };
                                }
                                else
                                {
                                    line1 = Main.rand.Next(5) switch
                                    {
                                        1 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.1"),
                                        2 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.2"),
                                        3 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.3"),
                                        4 => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.4"),
                                        _ => Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Resummon.5"),
                                    };
                                }
                                DialogueChain chain = new();
                                chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                                chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                                chain.OnEndTrigger += Chain_OnEndTrigger;
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);
                            }
                        }
                        if (AITimer == 5001)
                        {
                            ArmsFrameY = 1;
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Gun;
                            NPC.netUpdate = true;
                        }
                        if (AITimer >= 5060)
                        {
                            NPC.dontTakeDamage = false;
                            ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));
                            Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);
                            HeadType = 0;
                            AITimer = 0;
                            AIState = ActionState.GunAttacks;
                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseChange:
                    attacker = player;
                    NPC.LookAtEntity(player);
                    AITimer = 0;
                    AttackChoice = 0;
                    gunRot = defaultGunRot;
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    NPC.rotation = 0;
                    NPC.velocity *= 0.9f;
                    HeadType = 0;

                    for (int k = 0; k < NPC.buffImmune.Length; k++)
                    {
                        if (BuffID.Sets.IsATagBuff[k] || k is BuffID.Confused or BuffID.Poisoned or BuffID.Bleeding or BuffID.BloodButcherer)
                            continue;
                        NPC.buffImmune[k] = false;
                    }

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
                            case 5:
                                phase = 6;
                                AIState = ActionState.OverclockSubphase1;
                                break;
                            case 6:
                                phase = 7;
                                AIState = ActionState.OverclockSubphase2;
                                break;
                            case 7:
                                phase = 8;
                                AIState = ActionState.OverclockSubphase3;
                                break;
                            case 8:
                                phase = 9;
                                AIState = ActionState.OverclockEnd;
                                break;
                        }
                        AITimer = 0;
                        AttackChoice = 0;
                    }
                    if (phase < 5)
                        NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    break;
                case ActionState.GunAttacks:
                    if ((AttackChoice != 2 || AITimer <= 200) && (AttackChoice != 6 || AITimer <= 40))
                        NPC.LookAtEntity(attacker);

                    HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                    SnapHeadToRotArea();

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
                            HeadType = 0;
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 6);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                gunRot = defaultGunRot;
                                HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                                if (AITimer == 0)
                                {
                                    bool closeRangeClass = player.HeldItem.DamageType == DamageClass.Melee;
                                    if (Main.rand.NextBool(closeRangeClass ? 3 : 2))
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
                            gunRot.SlowRotation(NPC.DirectionTo(attacker.Center).ToRotation(), (float)Math.PI / 60f);
                            SnapGunToFiringArea();
                            HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                            SnapHeadToRotArea();

                            HeadType = 3;

                            if (AITimer++ % 20 == 0)
                                ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(attacker), Main.rand.Next(-60, 60));

                            NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) < 100 * 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (phase <= 1)
                            {
                                if (AITimer % 40 == 0 && ArmsFrameX >= 5)
                                {
                                    Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot), CustomSounds.Gun1KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (AITimer % 120 == 0 && ArmsFrameX >= 5)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        int rot = 25 * i;
                                        Shoot(GunOrigin, ProjectileID.MartianTurretBolt, (int)(NPC.damage * .9f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 25)), CustomSounds.Gun3KS);
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
                                if (AITimer % 20 == 0 && ArmsFrameX >= 5)
                                {
                                    Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot), CustomSounds.Gun1KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (AITimer % 100 == 0 && ArmsFrameX >= 5)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int rot = 25 * i;
                                        Shoot(GunOrigin, ProjectileID.MartianTurretBolt, (int)(NPC.damage * .9f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 50)), CustomSounds.Gun3KS);
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
                                if (AITimer % 35 == 0 && ArmsFrameX >= 5)
                                {
                                    Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot), CustomSounds.Gun1KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (AITimer % 105 == 0 && ArmsFrameX >= 5)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int rot = 25 * i;
                                        Shoot(GunOrigin, ProjectileID.MartianTurretBolt, (int)(NPC.damage * .9f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 50)), CustomSounds.Gun3KS);
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
                            NPC.Redemption().ignoreNewTargeting = true;
                            if (AITimer < 245)
                            {
                                HeadType = 3;
                                HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                                SnapHeadToRotArea();

                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center + attacker.velocity * 20f).ToRotation(), (float)Math.PI / 60f);
                            }
                            else
                                HeadType = 0;

                            SnapGunToFiringArea();

                            ShootPos = new Vector2(300 * NPC.RightOfDir(attacker), 10);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer++ < 200)
                            {
                                if (NPC.Distance(ShootPos) < 100 || (phase >= 5 ? AITimer > 40 : AITimer > 80))
                                {
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 13f, 14f);
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
                                    NPC.velocity.X = -9 * NPC.spriteDirection;
                                    for (int i = 0; i < Main.rand.Next(5, 8); i++)
                                    {
                                        Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(Main.rand.Next(8, 13) + dmgIncrease, gunRot + Main.rand.NextFloat(-0.14f, 0.14f)), CustomSounds.ShotgunBlastKS);
                                    }
                                    BodyState = (int)BodyAnim.GunShoot;

                                }
                                if (AITimer > 300)
                                {
                                    if (phase >= 5 && Main.rand.NextBool())
                                    {
                                        TimerRand = 0;
                                        AITimer = 0;
                                        AttackChoice = 6;
                                        NPC.netUpdate = true;
                                        break;
                                    }
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
                            HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                            SnapHeadToRotArea();

                            if (AITimer == 0)
                            {
                                if (phase < 5)
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
                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center).ToRotation(), (float)Math.PI / 60f);
                                SnapGunToFiringArea();

                                AITimer++;
                                ShootPos = new Vector2(450 * NPC.RightOfDir(attacker), -10);
                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                                if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                                {
                                    ArmsFrameY = 1;
                                    ArmsFrameX = 0;
                                    BodyState = (int)BodyAnim.Gun;
                                }

                                if (AITimer == 40)
                                {
                                    Shoot(GunOrigin, ProjectileType<ReboundShot>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(15 + dmgIncrease, gunRot), CustomSounds.Gun2KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (AITimer > 60)
                                {
                                    chance -= Main.rand.NextFloat(0.05f, 0.1f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Rebound Shot II
                        case 4:
                            HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                            SnapHeadToRotArea();

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
                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center).ToRotation(), (float)Math.PI / 60f);
                                SnapGunToFiringArea();

                                AITimer++;
                                ShootPos = new Vector2(450 * NPC.RightOfDir(attacker), -10);
                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                                if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                                {
                                    ArmsFrameY = 1;
                                    ArmsFrameX = 0;
                                    BodyState = (int)BodyAnim.Gun;
                                }

                                int startShot = 61;
                                if (phase >= 5)
                                    startShot = 41;
                                if (AITimer >= startShot && AITimer % 3 == 0 && AITimer <= startShot + (phase >= 5 ? 15 : 9))
                                {
                                    Shoot(GunOrigin, ProjectileType<ReboundShot>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(15 + dmgIncrease, gunRot), CustomSounds.Gun2KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                }
                                if (phase >= 5 ? AITimer > 67 : AITimer > 91)
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
                            HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                            SnapHeadToRotArea();

                            HeadType = 2;

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
                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center).ToRotation(), (float)Math.PI / 60f);
                                SnapGunToFiringArea();

                                if (AITimer++ % 20 == 0)
                                    ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(attacker), Main.rand.Next(-60, 60));

                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                                if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                                {
                                    ArmsFrameY = 1;
                                    ArmsFrameX = 0;
                                    BodyState = (int)BodyAnim.Gun;
                                }

                                if (AITimer % 10 == 0 && ArmsFrameX >= 5)
                                {
                                    Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(7 + dmgIncrease, gunRot), CustomSounds.Gun1KS);
                                    BodyState = (int)BodyAnim.GunShoot;
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

                        #region Blaster Overheat
                        case 6:
                            NPC.Redemption().ignoreNewTargeting = true;

                            int reduce = (int)(TimerRand * 15);
                            reduce = (int)MathHelper.Min(reduce, 50);
                            if (AITimer++ < 40 - reduce)
                            {
                                HeadType = 3;
                                HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                                SnapHeadToRotArea();

                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center + attacker.velocity * 20f).ToRotation(), (float)Math.PI / 60f);
                            }
                            else
                                HeadType = 0;

                            SnapGunToFiringArea();

                            if (AITimer < 40 - reduce)
                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 13f, 14f);
                            else
                                NPC.velocity *= 0.96f;

                            if (reduce >= 50 && TimerRand < 8)
                            {
                                NPC.position += RedeHelper.Spread(2);

                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center + attacker.velocity * 20f).ToRotation(), (float)Math.PI / 90f);
                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 10f : 6f, 14f);
                            }

                            if (AITimer < 60 - reduce)
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
                            if (AITimer == 60 - reduce)
                            {
                                NPC.velocity.X = -3 * NPC.spriteDirection;
                                for (int i = 0; i < Main.rand.Next(5, 8); i++)
                                {
                                    Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(Main.rand.Next(8, 13), gunRot + Main.rand.NextFloat(-0.14f, 0.14f)), CustomSounds.ShotgunBlastKS);
                                }
                                BodyState = (int)BodyAnim.GunShoot;

                            }
                            if (AITimer > 60 - reduce)
                            {
                                if (TimerRand++ < 7)
                                {
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                    break;
                                }
                                HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                                SnapHeadToRotArea();

                                if (TimerRand == 8)
                                {
                                    SoundEngine.PlaySound(CustomSounds.ElectricNoise.WithPitchOffset(0.4f), NPC.position);

                                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.position);
                                    RedeDraw.SpawnExplosion(GunOrigin, Color.OrangeRed, scale: .5f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                                    player.ApplyDamageToNPC(NPC, 100, 0, 1);
                                    NPC.velocity.X -= 4 * NPC.spriteDirection;

                                    ArmsFrameX = 0;
                                    ArmsFrameY = 0;
                                    BodyState = (int)BodyAnim.Idle;

                                    Shoot(GunOrigin, ProjectileType<KS3_BlasterGore>(), 0, new Vector2(-4 * NPC.spriteDirection, -14), NPC.spriteDirection);
                                }
                                if (AITimer == 30)
                                {
                                    ArmsCounter = 0;
                                    ArmsFrameX = 5;
                                    BodyState = (int)BodyAnim.Charging;
                                }
                                if (AITimer == 80)
                                {
                                    ArmsFrameX = 0;
                                    ArmsFrameY = 0;
                                    BodyState = (int)BodyAnim.Idle;
                                }
                                if (AITimer > 120)
                                {
                                    chance = 0;
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
                    if (AttackChoice != 3 || AITimer <= 120)
                        NPC.LookAtEntity(attacker);

                    if (AttackChoice == 0)
                    {
                        AttackChoice = Main.rand.Next(1, 10);
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        NPC.netUpdate = true;
                    }

                    NPC.rotation = NPC.velocity.X * 0.01f;
                    switch ((int)AttackChoice)
                    {
                        case -1:
                            HeadType = 0;
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 10);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                                if (AITimer == 0)
                                {
                                    bool closeRangeClass = player.HeldItem.DamageType == DamageClass.Melee;
                                    if (Main.rand.NextBool(closeRangeClass ? 4 : 2))
                                        AITimer = 2;
                                    else
                                        AITimer = 1;
                                    NPC.netUpdate = true;
                                }
                                NPC.velocity *= 0.9f;
                                if (AITimer == 1)
                                {
                                    if (BodyState is (int)BodyAnim.Idle)
                                    {
                                        BodyState = (int)BodyAnim.IdlePhysical;
                                    }

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
                                    gunRot = defaultGunRot;

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
                            HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                            SnapHeadToRotArea();

                            ShootPos = new Vector2(300 * NPC.RightOfDir(attacker), -60);
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (NPC.Distance(ShootPos) < 160 || AITimer > 40)
                                {
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 14f);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (phase >= 5)
                                {
                                    if (AITimer is 105 or 125 or 145)
                                    {
                                        HeadType = 2;
                                        NPC.frameCounter = 0;
                                        ArmsFrameX = 0;
                                        BodyState = (int)BodyAnim.RocketFist;
                                    }

                                    if (AITimer is 120 or 140 or 160)
                                        Shoot(new Vector2(NPC.Center.X + 15 * NPC.spriteDirection, NPC.Center.Y - 11), ProjectileType<KS3_Fist>(), NPC.damage, new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.MissileFire1);
                                }
                                else
                                {
                                    if (AITimer == 105)
                                    {
                                        HeadType = 2;
                                        NPC.frameCounter = 0;
                                        ArmsFrameX = 0;
                                        BodyState = (int)BodyAnim.RocketFist;
                                    }

                                    if (AITimer == 120)
                                        Shoot(new Vector2(NPC.Center.X + 15 * NPC.spriteDirection, NPC.Center.Y - 11), ProjectileType<KS3_Fist>(), NPC.damage, new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.MissileFire1);
                                }

                                if (AITimer > (phase >= 5 ? 200 : 150))
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
                            HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(3))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                ShootPos = new Vector2(200 * NPC.RightOfDir(attacker), 0);
                                if (AITimer < 100)
                                {
                                    if (NPC.Distance(ShootPos) < 160 || AITimer > 50)
                                    {
                                        AITimer = 100;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 18f, 14f);
                                }
                                else
                                {
                                    NPC.velocity *= 0.9f;
                                    if (AITimer == (phase >= 5 ? 101 : 120))
                                    {
                                        ArmsFrameY = 5;
                                        ArmsCounter = 0;
                                        BodyState = (int)BodyAnim.Grenade;
                                    }

                                    if (AITimer == (phase >= 5 ? 116 : 135))
                                    {
                                        Shoot(new Vector2(NPC.Center.X + 21 * NPC.spriteDirection, NPC.Center.Y - 17), ProjectileType<KS3_FlashGrenade>(), (int)(NPC.damage * .9f), new Vector2(10 * NPC.spriteDirection, -6), SoundID.Item1);
                                        if (phase >= 5)
                                        {
                                            Shoot(new Vector2(NPC.Center.X + 21 * NPC.spriteDirection, NPC.Center.Y - 17), ProjectileType<KS3_FlashGrenade>(), (int)(NPC.damage * .9f), new Vector2(8 * NPC.spriteDirection, -10), SoundID.Item1);
                                            Shoot(new Vector2(NPC.Center.X + 21 * NPC.spriteDirection, NPC.Center.Y - 17), ProjectileType<KS3_FlashGrenade>(), (int)(NPC.damage * .9f), new Vector2(14 * NPC.spriteDirection, -4), SoundID.Item1);
                                        }
                                    }

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
                            HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    HeadType = 2;
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Magic2.WithVolumeScale(2f), NPC.position);
                                    AITimer = 1;
                                }
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(320 * NPC.RightOfDir(attacker), 0);
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

                                            Vector2 vector2;
                                            double angle2 = Main.rand.NextDouble() * 2d * Math.PI;
                                            vector2.X = (float)(Math.Sin(angle2) * 200);
                                            vector2.Y = (float)(Math.Cos(angle2) * 200);
                                            Dust dust = Main.dust[Dust.NewDust(NPC.Center + vector2 + new Vector2(0, 50), 2, 2, DustType<DustSpark2>(), newColor: new Color(20, 242, 170, 0), Scale: 1f)];
                                            dust.noGravity = true;
                                            dust.velocity = dust.position.DirectionTo(NPC.Center + new Vector2(0, 50)) * 6f;
                                        }
                                        NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 8f, 14f);
                                    }
                                }
                                else
                                {
                                    NPC.Redemption().ignoreNewTargeting = true;

                                    NPC.Move(attacker.Center + ShootPos, 4f, 14f);

                                    if (AITimer == 121)
                                    {
                                        HeadType = 0;
                                        if (!Main.dedServ)
                                            SoundEngine.PlaySound(CustomSounds.BallFire.WithPitchOffset(-0.3f), NPC.position);

                                        Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ProjectileType<KS3_BeamCell>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(10, (attacker.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(35 * NPC.spriteDirection)), SoundID.Item103);
                                    }
                                    if (phase >= 5 && AITimer > 121)
                                    {
                                        if (phase >= 5)
                                            NPC.position += RedeHelper.Spread(3);

                                        if (AITimer % 5 == 0 && Main.rand.NextBool())
                                        {
                                            Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ProjectileType<KS3_Lightning>(), (int)(NPC.damage * .8f), Vector2.Zero, 0, Main.rand.Next(500, 1201));
                                        }
                                    }

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
                                if (RedeHelper.Chance(0.75f) && NPC.DistanceSQ(attacker.Center) < 300 * 300)
                                {
                                    HeadType = 3;
                                    AITimer = 1;
                                }
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                                SnapHeadToRotArea();

                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(80 * NPC.RightOfDir(attacker), 0);
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
                                        NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 7f, 14f);
                                    }
                                }
                                else
                                {
                                    NPC.velocity *= 0.9f;
                                    if (AITimer == 202)
                                    {
                                        HeadType = 0;
                                        Shoot(new Vector2(NPC.spriteDirection == 1 ? NPC.Center.X + 2 : NPC.Center.X - 2, NPC.Center.Y - 16), ProjectileType<KS3_Surge>(), (int)(NPC.damage * .9f), Vector2.Zero, CustomSounds.ElectricNoise);

                                        for (int i = 0; i < 18; i++)
                                            Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ProjectileType<KS3_Surge2>(), 0, RedeHelper.PolarVector(14, MathHelper.ToRadians(20) * i));

                                        if (phase > 2 && Main.expertMode)
                                        {
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(8, MathHelper.ToRadians(45) * i), ProjectileID.MartianTurretBolt, NPCHelper.HostileProjDamage(NPC.damage), 0, Main.myPlayer);
                                                    Main.projectile[proj].tileCollide = false;
                                                    Main.projectile[proj].timeLeft = 200;
                                                    Main.projectile[proj].netUpdate = true;
                                                }
                                                for (int i = 0; i < 18; i++)
                                                {
                                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(7, MathHelper.ToRadians(20) * i), ProjectileID.MartianTurretBolt, NPCHelper.HostileProjDamage(NPC.damage), 0, Main.myPlayer);
                                                    Main.projectile[proj].tileCollide = false;
                                                    Main.projectile[proj].timeLeft = 200;
                                                    Main.projectile[proj].netUpdate = true;
                                                }
                                            }
                                        }
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
                                {
                                    HeadType = 4;
                                    AITimer = 1;
                                }
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                                AITimer++;
                                BodyState = (int)BodyAnim.Shrug;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 21)
                                {
                                    for (int k = 0; k < NPC.buffImmune.Length; k++)
                                    {
                                        if (BuffID.Sets.IsATagBuff[k])
                                            continue;
                                        NPC.BecomeImmuneTo(k);
                                    }
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        NPC.ClearImmuneToBuffs(out _);
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
                                if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()) &&
                                    (phase < 5 || player.HeldItem.DamageType == DamageClass.Magic || player.HeldItem.DamageType == DamageClass.Ranged) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = (NPC.Center - new Vector2(0, 34)).DirectionTo(targetHead).ToRotation();
                                SnapHeadToRotArea();

                                if (AITimer++ % 20 == 0)
                                    ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));

                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                                if (AITimer == 16)
                                    Shoot(new Vector2(NPC.Center.X + 48 * NPC.spriteDirection, NPC.Center.Y - 12), ProjectileType<KS3_Reflect>(), 0, Vector2.Zero);

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
                                if (phase > 0 && !NPC.AnyNPCs(NPCType<KS3_MissileDrone>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                                    if (!NPC.AnyNPCs(NPCType<KS3_MissileDrone>()))
                                    {
                                        for (int i = 0; i < Main.rand.Next(2, 5); i++)
                                        {
                                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), NPCType<KS3_MissileDrone>(), NPC.whoAmI);
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
                            NPC.Redemption().ignoreNewTargeting = true;
                            if (AITimer == 0)
                            {
                                if (phase > 1 && !NPC.AnyNPCs(NPCType<KS3_Magnet>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                                    if (!NPC.AnyNPCs(NPCType<KS3_Magnet>()))
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), NPCType<KS3_Magnet>(), NPC.whoAmI);
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

                        #region Missile Barrage
                        case 9:
                            if (AITimer == 0)
                            {
                                if (phase > 2 && !RedeHelper.AnyProjectiles(ProjectileType<KS3_SoSCrosshair>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                                    if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_SoSCrosshair>()))
                                        Shoot(player.Center, ProjectileType<KS3_SoSCrosshair>(), (int)(NPC.damage * 1.8f), Vector2.Zero);
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
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    switch ((int)AttackChoice)
                    {
                        case -1:
                            NPC.LookAtEntity(attacker);
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 6);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
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
                                    gunRot = defaultGunRot;

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
                            NPC.Redemption().ignoreNewTargeting = true;
                            NPC.LookAtEntity(attacker);
                            if (AITimer++ <= 40)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                ShootPos = new Vector2(200 * NPC.RightOfDir(attacker), -60);
                                NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                            }
                            if (AITimer == 40)
                            {
                                NPC.frame.X = 7 * NPC.frame.Width;
                                NPC.frame.Y = 160;
                                BodyState = (int)BodyAnim.WheelkickStart;
                            }

                            if (AITimer > 40 && AITimer < 100)
                            {
                                if (AITimer % 10 == 0)
                                    SoundEngine.PlaySound(CustomSounds.Swoosh1.WithPitchOffset(.2f), NPC.position);

                                NPC.rotation += NPC.velocity.Y / 30;
                                ShootPos = new Vector2(attacker.velocity.X * 30, -600);
                                if (NPC.Center.Y < attacker.Center.Y - 600 || AITimer > 80)
                                {
                                    BodyState = (int)BodyAnim.Wheelkick;
                                    NPC.velocity *= 0.2f;
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 34f : 26f, 3f);
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
                                    Shoot(NPC.Center, ProjectileType<KS3_Wave>(), NPC.damage, Vector2.Zero, SoundID.Item74);
                                    NPC.velocity.Y += 40f;
                                }
                                if (AITimer >= 110)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 44 + (8 * NPC.spriteDirection), (int)NPC.Center.Y - 42 - 5, 88, 84);
                                    DamageInHitbox(Hitbox, NPC.damage, 4.5f);
                                }
                                if (AITimer > 130 || NPC.Center.Y > attacker.Center.Y + 400)
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
                                    ShootPos = new Vector2(200 * NPC.RightOfDir(attacker), -60);
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 30f : 22f, 8f);
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
                            NPC.Redemption().ignoreNewTargeting = true;
                            AITimer++;
                            if (AITimer == 1)
                                Teleport(false, Vector2.Zero);
                            if (AITimer < 100)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                NPC.LookAtEntity(attacker);
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
                                    ShootPos = new Vector2(100 * NPC.RightOfDir(attacker), 0);
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                                }
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.rotation = 0;
                                NPC.velocity *= 0.8f;
                                if (AITimer == 101)
                                    NPC.velocity.X = NPC.RightOfDir(attacker) * 6;

                                if (AITimer == 110)
                                    NPC.Dash(60, false, SoundID.Item74, attacker.Center);

                                if (AITimer >= 110 && AITimer <= 130)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 28 + (8 * NPC.spriteDirection), (int)NPC.Center.Y - 53 - 5, 56, 106);
                                    DamageInHitbox(Hitbox, NPC.damage, 20f);
                                }

                                if (AITimer == 130)
                                {
                                    NPC.velocity.X = -15 * NPC.spriteDirection;
                                    BodyState = (int)BodyAnim.ShoulderBashEnd;
                                }
                                if (AITimer > 160)
                                {
                                    NPC.LookAtEntity(attacker);
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
                            NPC.Redemption().ignoreNewTargeting = true;
                            if (Main.expertMode)
                            {
                                NPC.LookAtEntity(attacker);
                                AITimer++;
                                if (AITimer < 100)
                                {
                                    NPC.rotation = 0;
                                    if (NPC.DistanceSQ(ShootPos) < 100 * 100 || AITimer > 50)
                                    {
                                        ShootPos = new Vector2(150 * NPC.RightOfDir(attacker), 200);
                                        AITimer = 100;
                                        NPC.velocity.X = 0;
                                        NPC.velocity.Y = -30;

                                        NPC.frame.X = 2 * NPC.frame.Width;
                                        NPC.frame.Y = 160;
                                        BodyState = (int)BodyAnim.DropkickStart;

                                        SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                                }
                                else if (AITimer >= 100 && AITimer < 200)
                                {
                                    NPC.velocity *= 0.97f;
                                    if (AITimer == 140)
                                        SoundEngine.PlaySound(CustomSounds.OODashReady.WithPitchOffset(.5f), NPC.position);

                                    if (NPC.velocity.Length() < 5 || AITimer > 180)
                                    {
                                        AITimer = 200;
                                        NPC.velocity *= 0f;
                                        BodyState = (int)BodyAnim.Dropkick;
                                        NPC.rotation = (attacker.Center + attacker.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);
                                        NPC.netUpdate = true;
                                    }
                                }
                                else if (AITimer >= 200)
                                {
                                    if (AITimer >= 205)
                                    {
                                        Rectangle Hitbox = new((int)NPC.Center.X - 29, (int)NPC.Center.Y - 59, 58, 118);
                                        DamageInHitbox(Hitbox, (int)(NPC.damage * 1.1f));
                                    }

                                    if (AITimer == 205)
                                    {
                                        NPC.rotation = (attacker.Center + attacker.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);
                                        NPC.Dash(Main.getGoodWorld ? 50 : 40, true, SoundID.Item74, attacker.Center + attacker.velocity * 20f);
                                    }

                                    if (AITimer > 260 || NPC.Center.Y > attacker.Center.Y + 800)
                                    {
                                        Teleport(false, Vector2.Zero);

                                        NPC.rotation = 0;
                                        NPC.velocity *= 0f;
                                        chance -= Main.rand.NextFloat(0.2f, 0.5f);
                                        NPC.frame.Y = 4 * 80;
                                        if (NPC.frame.X < 4 * NPC.frame.Width)
                                            NPC.frame.X = 4 * NPC.frame.Width;
                                        BodyState = (int)BodyAnim.IdlePhysical;
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                chance -= Main.rand.NextFloat(0.2f, 0.5f);
                                NPC.frame.Y = 4 * 80;
                                if (NPC.frame.X < 4 * NPC.frame.Width)
                                    NPC.frame.X = 4 * NPC.frame.Width;
                                BodyState = (int)BodyAnim.IdlePhysical;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Iron Pummel
                        case 4:
                            NPC.Redemption().ignoreNewTargeting = true;
                            if (AITimer++ == 0)
                                gunRot = 0;
                            if (AITimer == 2 && NPC.DistanceSQ(attacker.Center) > 300 * 300)
                                Teleport(false, Vector2.Zero);
                            NPC.rotation = NPC.velocity.X * 0.01f;
                            ShootPos = new Vector2(60 * NPC.RightOfDir(attacker), 20);
                            if (AITimer < 100)
                            {
                                NPC.LookAtEntity(attacker);
                                if ((NPC.DistanceSQ(attacker.Center + ShootPos) < 50 * 50 && gunRot != 0) || AITimer > 40)
                                {
                                    AITimer = 100;

                                    NPC.frameCounter = 0;
                                    BodyState = Main.rand.NextBool(2) ? (int)BodyAnim.Pummel1 : (int)BodyAnim.Pummel2;
                                    if (BodyState is (int)BodyAnim.Pummel1)
                                    {
                                        NPC.frame.Y = 3 * 80;
                                        NPC.frame.X = 6 * NPC.frame.Width;
                                    }
                                    else
                                    {
                                        NPC.frame.Y = 4 * 80;
                                        NPC.frame.X = 1 * NPC.frame.Width;
                                    }
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.velocity *= 0.9f;
                                if (AITimer == 105)
                                {
                                    gunRot += 1;
                                    NPC.Dash(10, false, CustomSounds.Swoosh1, attacker.Center);
                                }

                                if (AITimer >= 105 && AITimer <= 115)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 6 + (28 * NPC.spriteDirection), (int)NPC.Center.Y - 6 - 18, 12, 12);
                                    DamageInHitbox(Hitbox, (int)(NPC.damage * .9f));
                                }

                                if (AITimer == 125 && RedeHelper.Chance(0.4f))
                                {
                                    AITimer = 140;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer > 125)
                                {
                                    NPC.LookAtEntity(attacker);
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                                }
                                if (AITimer > 140)
                                {
                                    if (gunRot <= 2 || RedeHelper.Chance(0.35f))
                                    {
                                        if (phase >= 5)
                                            Teleport(false, Vector2.Zero);

                                        AITimer = 1;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        NPC.LookAtEntity(attacker);
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
                            NPC.Redemption().ignoreNewTargeting = true;
                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(6) && phase < 5)
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
                                NPC.LookAtEntity(attacker);
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                AITimer++;
                                ShootPos = new Vector2(80 * NPC.RightOfDir(attacker), 20);

                                if (AITimer == 5)
                                {
                                    NPC.frame.X = 7 * NPC.frame.Width;
                                    NPC.frame.Y = 160 * 2;
                                    BodyState = (int)BodyAnim.Jojo;
                                }

                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) > 300 ? 20f : 9f, 8f);
                                if (AITimer >= 15 && AITimer % 3 == 0)
                                {
                                    Shoot(NPC.Center, ProjectileType<KS3_JojoFist>(), (int)(NPC.damage * .9f), Vector2.Zero, SoundID.Item60 with { Volume = .3f });
                                }
                                if (AITimer > 240)
                                {
                                    NPC.frame.Y = 4 * 80;
                                    if (NPC.frame.X < 4 * NPC.frame.Width)
                                        NPC.frame.X = 4 * NPC.frame.Width;

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
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    if (NPC.DistanceSQ(player.Center) >= 600 * 600)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    if (AITimer++ == 5)
                    {
                        Shoot(NPC.Center, ProjectileType<KS3_Shield2>(), 0, Vector2.Zero);
                        Teleport(false, Vector2.Zero);
                    }

                    if (RedeBossDowned.slayerDeath >= 4)
                    {
                        if (AITimer == 30)
                        {
                            Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(NPCType<KS3_MissileDrone>()))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), NPCType<KS3_MissileDrone>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 80)
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(2, 5);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()))
                                Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        if (AITimer < 5000)
                            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Low, 1200, 2400, 1200);
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 1;
                            string line1;
                            if (RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()))
                            {
                                if (player.HeldItem.DamageType == DamageClass.Melee)
                                    line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval1.Effort1");
                                else
                                    line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval1.Effort2");
                            }
                            else
                                line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval1.Effort3");
                            string line2;
                            if (TeleportCount > 6)
                                line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval1.RunAway");
                            else
                                line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval1.End");

                            DialogueChain chain = new();
                            chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, "[@h2]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer == 5001)
                        {
                            Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(NPCType<KS3_MissileDrone>()))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), NPCType<KS3_MissileDrone>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 5040)
                        {
                            if (RedeBossDowned.slayerDeath < 4 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.slayerDeath = 4;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()))
                                Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseTransition2:
                    #region Phase 2 Transition
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    if (NPC.DistanceSQ(player.Center) >= 600 * 600)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    if (AITimer++ == 5)
                    {
                        Shoot(NPC.Center, ProjectileType<KS3_Shield2>(), 0, Vector2.Zero);
                        Teleport(false, Vector2.Zero);
                    }

                    if (RedeBossDowned.slayerDeath >= 5)
                    {
                        if (AITimer == 30)
                        {
                            Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(NPCType<KS3_Magnet>()))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), NPCType<KS3_Magnet>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 80)
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()))
                                Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        if (AITimer < 5000)
                            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Low, 1200, 2400, 1200);
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 4;
                            string line1;
                            if (player.IsFullTBot())
                                line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval2.StartIfRobot");
                            else if (player.RedemptionPlayerBuff().ChickenForm)
                                line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval2.StartIfChicken");
                            else
                                line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval2.StartIfHuman");

                            string line2;
                            if (player.HeldItem.DamageType == DamageClass.Ranged || player.HeldItem.DamageType == DamageClass.Magic)
                                line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval2.Ranged");
                            else
                                line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval2.Normal");

                            DialogueChain chain = new();
                            chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval2.Joke1"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval2.Joke2"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, "[@h2]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer == 5001)
                        {
                            Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(NPCType<KS3_Magnet>()))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), NPCType<KS3_Magnet>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 5040)
                        {
                            if (RedeBossDowned.slayerDeath < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.slayerDeath = 5;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()))
                                Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseTransition3:
                    #region Phase 3 Transition
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    if (NPC.DistanceSQ(player.Center) >= 600 * 600)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    if (AITimer++ == 5)
                    {
                        Shoot(NPC.Center, ProjectileType<KS3_Shield2>(), 0, Vector2.Zero);
                        Teleport(false, Vector2.Zero);
                    }

                    if (RedeBossDowned.slayerDeath >= 6)
                    {
                        if (AITimer == 30)
                        {
                            HeadType = 0;
                            Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_SoSCrosshair>()))
                                Shoot(player.Center, ProjectileType<KS3_SoSCrosshair>(), 98, Vector2.Zero);
                        }
                        if (AITimer > 80)
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()))
                                Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        if (AITimer < 5000)
                            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Low, 1200, 2400, 1200);
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 2;
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval3.1"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval3.2"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval3.3"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer == 5001)
                        {
                            HeadType = 0;
                            Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_SoSCrosshair>()))
                                Shoot(player.Center, ProjectileType<KS3_SoSCrosshair>(), 98, Vector2.Zero);
                        }
                        if (AITimer > 5040)
                        {
                            if (RedeBossDowned.slayerDeath < 6 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.slayerDeath = 6;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ProjectileType<KS3_Shield>()))
                                Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    #endregion
                    break;
                case ActionState.PhaseTransition4:
                    #region Phase 4 Transition
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    if (NPC.DistanceSQ(player.Center) >= 600 * 600)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    if (AITimer++ == 5)
                    {
                        Teleport(false, Vector2.Zero);
                        Shoot(NPC.Center, ProjectileType<KS3_Shield2>(), 0, Vector2.Zero);
                    }

                    if (RedeBossDowned.slayerDeath >= 7)
                    {
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 3;
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval4.Stop"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer > 5000)
                        {
                            player.Redemption().yesChoice = false;
                            player.Redemption().noChoice = false;

                            NPC.life = 1;
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

                            string line1;
                            if (TeleportCount > 16)
                                line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval4.RunAway");
                            else
                                line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval4.Normal");

                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval4.Alright"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval4.Draw1"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval4.Draw2"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Interval4.Draw3"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer > 5000)
                        {
                            if (!RedeBossDowned.downedSlayer && Main.netMode != NetmodeID.MultiplayerClient)
                                ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromKey("Mods.Redemption.UI.Chalice.KS3Choice"), 180, 30, 0, Color.DarkGoldenrod);

                            player.Redemption().yesChoice = false;
                            player.Redemption().noChoice = false;

                            NPC.life = 1;
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
                    NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
                    YesNoUI.DisplayYesNoButtons(player, Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.CallDraw"), Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.Continue"), new Vector2(0, 28), new Vector2(0, 28), .6f, .6f);
                    if (player.Redemption().yesChoice)
                    {
                        if (ChaliceAlignmentUI.Visible)
                            ChaliceAlignmentUI.Visible = false;
                        if (RedeBossDowned.slayerDeath < 7 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.slayerDeath = 7;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        NPC.dontTakeDamage = true;
                        AITimer = 0;
                        AIState = ActionState.Spared;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    else if (player.Redemption().noChoice)
                    {
                        if (!Main.dedServ)
                            ChaliceAlignmentUI.Visible = false;
                        AITimer = 0;
                        AIState = ActionState.Attacked;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacked:
                    #region Attacked
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    NPC.velocity *= .9f;
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    if (!Main.dedServ && phase < 5)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BeyondSteelIntro");

                    if (AITimer == 5)
                        Shoot(NPC.Center, ProjectileType<KS3_Shield2>(), 0, Vector2.Zero);

                    if (phase == 5)
                    {
                        zoomTimer += .1f;
                        zoomTimer = MathHelper.Clamp(zoomTimer, 0, 1f);
                        player.RedemptionScreen().customZoom = 1f + (EaseFunction.EaseCubicOut.Ease(zoomTimer) / 6);
                    }

                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 1;
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Accept"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier));
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer == 180)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), NPCType<SpaceKeeper>(), NPC.whoAmI, 0);

                    if (AITimer == 190)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), NPCType<SpaceKeeper>(), NPC.whoAmI, 1);

                    if (AITimer == 200)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), NPCType<SpaceKeeper>(), NPC.whoAmI, 2);

                    if (AITimer == 210)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), NPCType<SpaceKeeper>(), NPC.whoAmI, 3);

                    if (AITimer == 400 && !Main.dedServ)
                    {
                        HeadType = 2;
                        string line1;
                        if (RedeBossDowned.slayerDeath >= 8)
                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock1");
                        else
                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock2");

                        string line2;
                        if (RedeBossDowned.slayerDeath >= 8)
                            line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock3");
                        else
                        {
                            if (player.IsFullTBot())
                                line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.IfRobot");
                            else if (player.RedemptionPlayerBuff().ChickenForm)
                                line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.IfChicken");
                            else
                                line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.IfHuman");
                        }

                        DialogueChain chain = new();
                        chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 3f, 0, false, null, Bubble, null, modifier))
                             .Add(new(NPC, "[@h4]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier, RedeBossDowned.slayerDeath >= 8 ? 1 : 0));
                        if (RedeBossDowned.slayerDeath < 8)
                        {
                            string line4 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock4");
                            string line5 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock5");
                            string line6 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock6");
                            string line7 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock7");
                            if (RedeQuest.slayerRep >= 2)
                            {
                                line4 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock4Alt");
                                line5 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock5Alt");
                                line6 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock6Alt");
                                line7 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Overclock7Alt");
                            }

                            chain.Add(new(NPC, line4, new Color(170, 255, 255), Color.Black, voice, .03f, 3f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, line5, new Color(170, 255, 255), Color.Black, voice, .03f, 3f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, line6, new Color(170, 255, 255), Color.Black, voice, .03f, 2.98f, 0, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, line7, new Color(170, 255, 255), Color.Black, voice, .03f, 3f, .3f, true, null, Bubble, null, modifier, 1));
                        }
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer > 400)
                    {
                        if (NPC.life < NPC.lifeMax)
                        {
                            if (AITimer % 5 == 0)
                            {
                                int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustType<HealDust>());
                                Main.dust[dustIndex].velocity.Y = -3;
                                Main.dust[dustIndex].velocity.X = 0;
                                Main.dust[dustIndex].noGravity = true;
                            }
                            if (RedeBossDowned.slayerDeath < 8)
                            {
                                NPC.life += NPC.lifeMax / 1200;
                                NPC.HealEffect(NPC.lifeMax / 1200);
                            }
                            else
                            {
                                NPC.life += NPC.lifeMax / 60;
                                NPC.HealEffect(NPC.lifeMax / 60);
                            }
                        }
                        else
                        {
                            if (AITimer > 5000)
                            {
                                AITimer = 0;
                                AIState = ActionState.Overclock;
                                NPC.life = NPC.lifeMax;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                    #endregion
                    break;
                case ActionState.Spared:
                    #region Spared
                    NPC.LookAtEntity(player);
                    NPC.velocity *= 0.9f;

                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 0;
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Spare.Accept"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Spare.Adios"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer > 5001)
                    {
                        NPC.HitSound = null;

                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                        if (RedeBossDowned.slayerDeath < 7 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.slayerDeath = 7;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.StrikeInstantKill();
                    }
                    #endregion
                    break;
                case ActionState.Overclock:
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    gunRot = defaultGunRot;

                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    if (AITimer < 30)
                    {
                        if (phase == 5)
                        {
                            zoomTimer += .1f;
                            zoomTimer = MathHelper.Clamp(zoomTimer, 0, 1f);
                            player.RedemptionScreen().customZoom = 1f + (EaseFunction.EaseCubicOut.Ease(zoomTimer) / 6);
                        }
                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    }
                    else
                    {
                        zoomTimer -= .01f;
                        zoomTimer = MathHelper.Clamp(zoomTimer, 0, 1f);
                        player.RedemptionScreen().customZoom = 1f + (EaseFunction.EaseCubicInOut.Ease(zoomTimer) / 6);
                    }
                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 2;
                        string line3;
                        if (RedeBossDowned.slayerDeath >= 8)
                            line3 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Begin2");
                        else
                            line3 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.Begin");

                        DialogueChain chain = new();
                        chain.Add(new(NPC, line3, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .3f, true, null, Bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer == 4001)
                    {
                        ArmsFrameY = 1;
                        ArmsFrameX = 0;
                        BodyState = (int)BodyAnim.Gun;
                        NPC.netUpdate = true;
                    }
                    if (AITimer >= 5000)
                    {
                        if (!Main.dedServ)
                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BeyondSteel");

                        if (RedeBossDowned.slayerDeath < 8 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.slayerDeath = 8;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        NPC.dontTakeDamage = false;
                        phase = 5;
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        AttackChoice = 1;
                        AITimer = 0;
                        AIState = ActionState.GunAttacks;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
                case ActionState.OverclockSubphase1:
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;

                    if (AITimer++ == 0)
                    {
                        SoundEngine.PlaySound(CustomSounds.ElectricNoise.WithPitchOffset(0.4f), NPC.position);

                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        SoundEngine.PlaySound(SoundID.Tink, NPC.position);
                        RedeDraw.SpawnExplosion(NPC.Center - new Vector2(0, 34), Color.OrangeRed, scale: .5f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center - new Vector2(8 * NPC.spriteDirection, 48), NPC.velocity, Find<ModGore>("Redemption/KS3Gore1").Type);

                        player.ApplyDamageToNPC(NPC, 100, 0, 1);
                    }
                    if (AITimer < 60 && Main.rand.NextBool(30))
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        RedeDraw.SpawnExplosion(new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), Color.OrangeRed, scale: .8f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        player.ApplyDamageToNPC(NPC, 100, 0, 1);
                    }
                    if (AITimer > 60)
                    {
                        AttackChoice = 0;
                        AITimer = 0;
                        AIState = (ActionState)Main.rand.Next(3, 5);

                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.OverclockSubphase2:
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    if (AITimer < 60)
                        NPC.velocity *= 0.9f;
                    else
                        NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) < 100 * 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                    NPC.rotation = NPC.velocity.X * 0.01f;

                    if (AITimer++ == 0)
                    {
                        if (RedeBossDowned.slayerDeath < 9 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.OverclockBreak1"), new Color(170, 255, 255), Color.Black, voice, .02f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }

                        SoundEngine.PlaySound(CustomSounds.ElectricNoise.WithPitchOffset(0.4f), NPC.position);

                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        SoundEngine.PlaySound(SoundID.Tink, NPC.position);
                        RedeDraw.SpawnExplosion(NPC.Center - new Vector2(0, 34), Color.OrangeRed, scale: .5f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center - new Vector2(-14 * NPC.spriteDirection, 50), NPC.velocity, Find<ModGore>("Redemption/KS3Gore2").Type);
                        player.ApplyDamageToNPC(NPC, 100, 0, 1);
                    }
                    if (AITimer < 60 && Main.rand.NextBool(30))
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        RedeDraw.SpawnExplosion(new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), Color.OrangeRed, scale: .8f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        player.ApplyDamageToNPC(NPC, 100, 0, 1);
                    }
                    if (AITimer > (RedeBossDowned.slayerDeath >= 10 ? 60 : 300))
                    {
                        AttackChoice = 0;
                        AITimer = 0;
                        AIState = (ActionState)Main.rand.Next(3, 5);

                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.OverclockSubphase3:
                    NPC.LookAtEntity(player);
                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    if (AITimer < 10)
                        NPC.velocity *= 0.9f;
                    else
                        NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) < 100 * 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                    NPC.rotation = NPC.velocity.X * 0.01f;

                    if (AITimer++ == 0)
                    {
                        if (RedeBossDowned.slayerDeath < 9 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.OverclockBreak2"), new Color(170, 255, 255), Color.Black, voice, .02f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }

                        SoundEngine.PlaySound(CustomSounds.ElectricNoise.WithPitchOffset(0.4f), NPC.position);

                        SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(.5f), NPC.position);
                        SoundEngine.PlaySound(SoundID.Tink, NPC.position);
                        RedeDraw.SpawnExplosion(NPC.Center - new Vector2(0, 34), Color.OrangeRed, scale: .5f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 26), NPC.velocity, Find<ModGore>("Redemption/KS3Gore3").Type);
                        player.ApplyDamageToNPC(NPC, 100, 0, 1);
                    }
                    if (AITimer < 60 && Main.rand.NextBool(30))
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        RedeDraw.SpawnExplosion(new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), Color.OrangeRed, scale: .8f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        player.ApplyDamageToNPC(NPC, 100, 0, 1);
                    }
                    if (AITimer > 80)
                    {
                        if (RedeBossDowned.slayerDeath < 9 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.slayerDeath = 9;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        AttackChoice = 4;
                        AITimer = 0;
                        AIState = ActionState.SpecialAttacks;

                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.OverclockEnd:
                    NPC.LookAtEntity(player);
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);

                    HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (NPC.DistanceSQ(player.Center) >= 400 * 400)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;

                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        NPC.netUpdate = true;

                        SoundEngine.PlaySound(CustomSounds.ElectricNoise.WithPitchOffset(0.4f), NPC.position);

                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        RedeDraw.SpawnExplosion(NPC.Center - new Vector2(0, 34), Color.OrangeRed, scale: .5f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        player.ApplyDamageToNPC(NPC, 100, 0, 1);

                        if (RedeBossDowned.slayerDeath < 10 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.OverclockEnd1"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.OverclockEnd2"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.OverclockEnd3"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, false, null, Bubble, null, modifier))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.OverclockEnd4"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, false, null, Bubble, null, modifier));
                            if (RedeQuest.slayerRep < 4)
                                chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.Continue.OverclockEnd5"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));

                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                    }
                    if (AITimer < 120 && Main.rand.NextBool(30))
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        RedeDraw.SpawnExplosion(new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), Color.OrangeRed, scale: .8f, tex: "Redemption/Textures/BigFlare", rot: RedeHelper.RandomRotation());
                        player.ApplyDamageToNPC(NPC, 100, 0, 1);
                    }
                    if (AITimer > 5000 || RedeBossDowned.slayerDeath >= 10)
                    {
                        NPC.HitSound = null;

                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                        if (RedeBossDowned.slayerDeath < 10 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.slayerDeath = 10;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.StrikeInstantKill();
                    }
                    break;
            }
            #region Teleporting
            if (NPC.DistanceSQ(attacker.Center) >= 1100 * 1100 && NPC.ai[0] > 0 && !player.RedemptionScreen().lockScreen)
            {
                if (AttackChoice == 3 && AIState is ActionState.PhysicalAttacks)
                    return;
                TeleportCount++;
                Teleport(false, Vector2.Zero);
                NPC.netUpdate = true;
            }
            #endregion
        }
        public override void PostAI()
        {
            CustomFrames(80);
        }
        public void CustomFrames(int frameHeight)
        {
            if (phase >= 5)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
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
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

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
                        if (NPC.frame.Y == 4 * frameHeight && NPC.frame.X > 0)
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
                            NPC.frame.X = 4 * NPC.frame.Width;
                    }
                    break;
                case (int)BodyAnim.ShoulderBash:
                    NPC.frame.Y = 5 * frameHeight;

                    if (NPC.frameCounter++ >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.X += NPC.frame.Width;
                        if (NPC.frame.X > 6 * NPC.frame.Width)
                            NPC.frame.X = 2 * NPC.frame.Width;
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

            HeadFrame = HeadType;
            if (phase >= 5)
                HeadFrame = (int)MathHelper.Min(phase, 8);
        }
        public void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            if (signature == "c")
                AITimer = 4000;
            HeadType = signature switch
            {
                "h1" => 1,
                "h2" => 2,
                "h3" => 3,
                "h4" => 4,
                _ => 0,
            };
            switch (signature)
            {
                case "b0":
                    ArmsCounter = 0;
                    ArmsFrameX = 0;
                    BodyState = 0;
                    break;
                case "b1":
                    ArmsCounter = 0;
                    BodyState = (int)BodyAnim.Crossed;
                    break;
                case "b2":
                    ArmsCounter = 0;
                    ArmsFrameX = 0;
                    BodyState = (int)BodyAnim.Shrug;
                    break;
                case "b3":
                    ArmsCounter = 0;
                    ArmsFrameX = 5;
                    BodyState = (int)BodyAnim.Charging;
                    break;
                case "ov":
                    if (!Main.dedServ)
                    {
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BeyondSteel");
                        Main.musicFade[MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BeyondSteel")] = 1f;
                    }

                    NPC.life = NPC.lifeMax;

                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 20;

                    SoundEngine.PlaySound(CustomSounds.HeavyMagic1, NPC.position);
                    RedeDraw.SpawnExplosion(NPC.Center, Color.Red, scale: 1.5f, tex: "Redemption/Textures/BigFlare");
                    RedeDraw.SpawnExplosion(NPC.Center, Color.IndianRed, scale: 1f, tex: "Redemption/Textures/BigFlare");
                    RedeDraw.SpawnExplosion(NPC.Center, Color.White, scale: .5f, tex: "Redemption/Textures/BigFlare");

                    phase = 5;
                    NPC.netUpdate = true;
                    break;
            }
        }
        public void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            if (AIState is not ActionState.GunAttacks and not ActionState.SpecialAttacks and not ActionState.PhysicalAttacks)
                AITimer = 5000;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (AIState is ActionState.PhysicalAttacks)
            {
                if (AttackChoice == 2)
                    target.AddBuff(BuffType<StunnedDebuff>(), 30);
                if (AttackChoice == 3)
                    target.AddBuff(BuffType<StaticStunDebuff>(), 80);
                if (AttackChoice is 4 or 2)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit42.WithPitchOffset(-0.3f), target.position);
                    for (int i = 0; i < 3; i++)
                    {
                        float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                        float randomVel = Main.rand.NextFloat(2f, 3f);
                        Vector2 direction = target.Center.DirectionFrom(NPC.Center);
                        Vector2 position = target.Center - direction * 10;
                        RedeParticleManager.CreateSpeedParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, .8f, Color.White.WithAlpha(0));
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            if (AIState is ActionState.PhysicalAttacks)
            {
                if (AttackChoice == 2 && target.knockBackResist > 0)
                    target.AddBuff(BuffType<StunnedDebuff>(), 30);
                if (AttackChoice is 4 or 2)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit42.WithPitchOffset(-0.3f), target.position);
                    for (int i = 0; i < 3; i++)
                    {
                        float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                        float randomVel = Main.rand.NextFloat(2f, 3f);
                        Vector2 direction = target.Center.DirectionFrom(NPC.Center);
                        Vector2 position = target.Center - direction * 10;
                        RedeParticleManager.CreateSpeedParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, .8f, Color.White.WithAlpha(0));
                    }
                }
            }
        }
        public override bool CheckDead()
        {
            if (AIState is ActionState.Spared or ActionState.OverclockEnd)
                return true;
            else
            {
                NPC.life = 1;
                NPC.netUpdate = true;
                return false;
            }
        }

        #region Methods
        protected void SnapGunToFiringArea()
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
        protected void SnapHeadToRotArea()
        {
            //set bpoundries
            float rotRegionCenter = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
            float minRotRegion = rotRegionCenter - headRotLimit / 2f;
            float maxRotRegion = rotRegionCenter + headRotLimit / 2f;

            //convert HeadRotation to equivilent angles within a certian range
            while (HeadRotation < -(float)Math.PI / 2)
            {
                HeadRotation += (float)Math.PI * 2f;
            }
            while (HeadRotation > 3f * (float)Math.PI / 2f)
            {
                HeadRotation -= (float)Math.PI * 2f;
            }

            if (HeadRotation > maxRotRegion || HeadRotation < minRotRegion)
            {
                float distFromMin = RedeHelper.AngularDifference(minRotRegion, HeadRotation);
                float distFromMax = RedeHelper.AngularDifference(maxRotRegion, HeadRotation);

                if (distFromMin < distFromMax)
                {
                    HeadRotation = minRotRegion;
                }
                else
                {
                    HeadRotation = maxRotRegion;
                }
            }
        }

        public void Teleport(bool specialPos, Vector2 teleportPos)
        {
            Entity attacker = Attacker();

            TeleGlow = true;
            TeleGlowTimer = 0;
            oldTeleVector = NPC.Center;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!specialPos)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            Vector2 newPos = new(Main.rand.Next(-400, -250), Main.rand.Next(-200, 50));
                            NPC.Center = attacker.Center + newPos;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            Vector2 newPos2 = new(Main.rand.Next(250, 400), Main.rand.Next(-200, 50));
                            NPC.Center = attacker.Center + newPos2;
                            NPC.netUpdate = true;
                            break;
                    }
                }
                else
                {
                    NPC.Center = attacker.Center + teleportPos;
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

        protected int ArmsFrameY;
        protected int ArmsFrameX;
        protected int ArmsCounter;
        protected int HeadFrame;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
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
        }

        public static float c = 1f / 255f;
        public Color innerColor = new(100 * c * 0.5f, 242 * c * 0.5f, 170 * c * 0.5f, 0.5f);
        public Color borderColor = new(0 * c, 242 * c, 170 * c, 1f);
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy && AIState is ActionState.Begin && AITimer < 2)
                return false;
            Head ??= Request<Texture2D>(TexturePath + "_Heads");
            HeadGlow ??= Request<Texture2D>(TexturePath + "_Heads_Glow");
            Overclock ??= Request<Texture2D>(TexturePath + "_Overclock");
            OverclockGlow ??= Request<Texture2D>(TexturePath + "_Overclock_Glow");
            OverclockArmsGlow ??= Request<Texture2D>(TexturePath + "_Arms_Overclock_Glow");
            OverclockHead ??= Request<Texture2D>(TexturePath + "_Overclock_Heads");
            OverclockHeadGlow ??= Request<Texture2D>(TexturePath + "_Overclock_Heads_Glow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle ArmsRect = Arms.Frame(10, 6, ArmsFrameX, ArmsFrameY);
            Vector2 ArmsOrigin = ArmsRect.Size() / 2;
            Vector2 ArmsPos = new(0, -13);

            Vector2 headOffset;
            if (NPC.frame.Y == 0)
            {
                headOffset = (NPC.frame.X / NPC.frame.Width) switch
                {
                    1 or 2 => new(0, -2),
                    4 => new(2, -2),
                    5 => new(3, -2),
                    6 => new(6, -2),
                    7 => new(8, -2),
                    _ => Vector2.Zero,
                };
            }
            else
                headOffset = new Vector2(6, -1);

            bool isClone = NPC.type == NPCType<KS3_Clone>();
            Vector2 HeadPos = RedeHelper.OffsetWithRotation(NPC, -2 + headOffset.X, -22 + headOffset.Y);
            Rectangle HeadRect = Head.Frame(1, 9, 0, isClone ? 0 : HeadFrame);

            if (ArmsFrameY == 1 && ArmsFrameX >= 5)
            {
                ArmsOrigin += new Vector2(-13 * NPC.spriteDirection, -5);
                ArmsPos = RedeHelper.OffsetWithRotation(NPC, -13, -5 - 13);
            }

            float scale = 2;
            if (Main.remixWorld)
                scale = 1;

            if (!NPC.IsABestiaryIconDummy)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(phase < 5 ? TextureAssets.Npc[NPC.type].Value : Overclock.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale * scale, effects, 0);
                }
                if (NPC.frame.Y == 0 || (NPC.frame.Y == NPC.frame.Height && NPC.frame.X < 2 * NPC.frame.Width))
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 origin = HeadRect.Size() / 2 + new Vector2(0, 6);
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(Head.Value, oldPos + HeadPos + NPC.Size / 2f - screenPos, new Rectangle?(HeadRect), NPC.ColorTintedAndOpacity(Color.LightCyan) * 0.5f, isClone ? 0 : HeadRotation + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), origin, NPC.scale * scale, effects, 0f);
                    }
                }
                if (BodyState < (int)BodyAnim.IdlePhysical)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(Arms.Value, oldPos + ArmsPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(ArmsRect), NPC.GetAlpha(Color.LightCyan) * 0.5f, BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                        gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);
                    }
                }
            }
            bool shaderApply = !NPC.IsABestiaryIconDummy && NPC.dontTakeDamage && !Main.dedServ && spriteBatch != null;
            if (shaderApply)
            {
                Texture2D HexagonTexture = Request<Texture2D>(Redemption.EMPTY_TEXTURE).Value;
                Effect ShieldEffect = Request<Effect>("Redemption/Effects/Shield").Value;
                ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
                ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
                ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f).ToVector4());
                ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, .3f).ToVector4());

                spriteBatch.End();
                ShieldEffect.Parameters["sinMult"].SetValue(10f);
                ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 8f / HexagonTexture.Width, TextureAssets.Npc[NPC.type].Value.Height / 6 / HexagonTexture.Height));
                ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (TextureAssets.Npc[NPC.type].Value.Width / 2), 1f / (TextureAssets.Npc[NPC.type].Value.Height / 2)));
                ShieldEffect.Parameters["frameAmount"].SetValue(6f);
                spriteBatch.BeginDefault(true);
                ShieldEffect.CurrentTechnique.Passes[0].Apply();
            }

            spriteBatch.Draw(phase < 5 ? TextureAssets.Npc[NPC.type].Value : Overclock.Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * scale, effects, 0);

            if (NPC.frame.Y == 0 || (NPC.frame.Y == NPC.frame.Height && NPC.frame.X < 2 * NPC.frame.Width))
            {
                if (shaderApply)
                {
                    Texture2D HexagonTexture = Request<Texture2D>(Redemption.EMPTY_TEXTURE).Value;
                    Effect ShieldEffect = Request<Effect>("Redemption/Effects/Shield").Value;
                    ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
                    ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
                    ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                    ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f).ToVector4());
                    ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, 0.3f).ToVector4());

                    spriteBatch.End();
                    ShieldEffect.Parameters["sinMult"].SetValue(10f);
                    ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(HeadRect.Width / 1f / HexagonTexture.Width, HeadRect.Height / 9f / HexagonTexture.Height));
                    ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (Head.Value.Width / 2), 1f / (Head.Value.Height / 2)));
                    ShieldEffect.Parameters["frameAmount"].SetValue(9f);
                    spriteBatch.BeginDefault(true);
                    ShieldEffect.CurrentTechnique.Passes[0].Apply();
                }

                if (NPC.IsABestiaryIconDummy)
                    HeadRotation = MathHelper.Pi;
                Vector2 origin = HeadRect.Size() / 2 + new Vector2(0, 6);
                spriteBatch.Draw(Head.Value, NPC.Center + HeadPos - screenPos, new Rectangle?(HeadRect), NPC.ColorTintedAndOpacity(drawColor), isClone ? 0 : HeadRotation + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), origin, NPC.scale * scale, effects, 0f);

                if (shaderApply)
                {
                    spriteBatch.End();
                    spriteBatch.BeginDefault();
                }

                spriteBatch.Draw(HeadGlow.Value, NPC.Center + HeadPos - screenPos, new Rectangle?(HeadRect), NPC.ColorTintedAndOpacity(Color.White), isClone ? 0 : HeadRotation + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), origin, NPC.scale * scale, effects, 0f);
            }
            else
            {
                if (shaderApply)
                {
                    spriteBatch.End();
                    spriteBatch.BeginDefault();
                }
            }

            spriteBatch.Draw(phase < 5 ? Glow.Value : OverclockGlow.Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * scale, effects, 0);

            if (phase >= 5 && HeadFrame >= 6)
            {
                HeadRect = OverclockHead.Frame(8, 18, NPC.frame.X / NPC.frame.Width, (NPC.frame.Y / NPC.frame.Height) + (6 * (HeadFrame - 6)));
                Vector2 HeadOrigin = HeadRect.Size() / 2 + new Vector2(0, 17);
                spriteBatch.Draw(OverclockHead.Value, NPC.Center - screenPos, HeadRect, NPC.ColorTintedAndOpacity(drawColor), NPC.rotation, HeadOrigin, NPC.scale * scale, effects, 0);
                spriteBatch.Draw(OverclockHeadGlow.Value, NPC.Center - screenPos, HeadRect, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, HeadOrigin, NPC.scale * scale, effects, 0);
            }

            if (BodyState < (int)BodyAnim.IdlePhysical)
            {
                if (shaderApply)
                {
                    Texture2D HexagonTexture = Request<Texture2D>(Redemption.EMPTY_TEXTURE).Value;
                    Effect ShieldEffect = Request<Effect>("Redemption/Effects/Shield").Value;
                    ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
                    ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
                    ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                    ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f).ToVector4());
                    ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, 0.3f).ToVector4());

                    spriteBatch.End();
                    ShieldEffect.Parameters["sinMult"].SetValue(10f);
                    ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(ArmsRect.Width / 10f / HexagonTexture.Width, ArmsRect.Height / 6 / HexagonTexture.Height));
                    ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (Arms.Value.Width / 2), 1f / (Arms.Value.Height / 2)));
                    ShieldEffect.Parameters["frameAmount"].SetValue(6f);
                    spriteBatch.BeginDefault(true);
                    ShieldEffect.CurrentTechnique.Passes[0].Apply();
                }

                spriteBatch.Draw(Arms.Value, NPC.Center + ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.ColorTintedAndOpacity(drawColor),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                if (shaderApply)
                {
                    spriteBatch.End();
                    spriteBatch.BeginDefault();
                }

                spriteBatch.Draw(phase < 5 ? ArmsGlow.Value : OverclockArmsGlow.Value, NPC.Center + ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.ColorTintedAndOpacity(Color.White),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> flare = CommonTextures.BigFlare;
            Vector2 flareOrigin = flare.Size() / 2;

            Asset<Texture2D> fadeTele = CommonTextures.FadeTelegraph;
            Asset<Texture2D> fadeTeleCap = CommonTextures.FadeTelegraphCap;

            if (AIState is ActionState.PhysicalAttacks && AttackChoice == 3 && AITimer >= 205 && AITimer < 205 + 10)
            {
                float opacity = MathHelper.Lerp(.5f, 0, (AITimer - 205f) / 10f);
                spriteBatch.Draw(fadeTele.Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Cyan * opacity, NPC.velocity.ToRotation(), new Vector2(0, 64), new Vector2(20, 1), 0, 0f);
                spriteBatch.Draw(fadeTeleCap.Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Cyan * opacity, NPC.velocity.ToRotation() + MathHelper.Pi, new Vector2(0, 64), new Vector2(1, 1), 0, 0f);
            }
            else if (AIState is ActionState.SpecialAttacks && AttackChoice == 3 && AITimer >= 121)
            {
                Asset<Texture2D> flare2 = CommonTextures.WhiteFlare;
                Vector2 flareOrigin2 = flare2.Size() / 2;

                Vector2 corePos = new(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16);
                float opacity = 1;
                if (AITimer >= 160)
                    opacity = MathHelper.Lerp(1f, 0f, (AITimer - 160f) / 80f);

                float rand = Main.rand.NextFloat(.9f, 1.1f);
                spriteBatch.Draw(flare2.Value, corePos - screenPos, null, Color.Cyan.WithAlpha(0) * .5f * opacity * rand, 0, flareOrigin2, 1.5f, 0, 0);
                spriteBatch.Draw(flare2.Value, corePos - screenPos, null, Color.White.WithAlpha(0) * opacity * rand, 0, flareOrigin2, 1f, 0, 0);
            }
            else if (AIState is ActionState.SpecialAttacks && AttackChoice == 4 && AITimer >= 202 && AITimer < 202 + 10)
            {
                Vector2 corePos = new(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16);
                float opacity = MathHelper.Lerp(1f, 0f, (AITimer - 202f) / 10f);
                spriteBatch.Draw(flare.Value, corePos - screenPos, null, Color.Cyan.WithAlpha(0) * .5f * opacity, -MathHelper.PiOver4, flareOrigin, 2f, 0, 0);
                spriteBatch.Draw(flare.Value, corePos - screenPos, null, Color.White.WithAlpha(0) * opacity, -MathHelper.PiOver4, flareOrigin, 1f, 0, 0);

                spriteBatch.Draw(flare.Value, corePos - screenPos, null, Color.Cyan.WithAlpha(0) * .5f * opacity, MathHelper.PiOver4, flareOrigin, 2f, 0, 0);
                spriteBatch.Draw(flare.Value, corePos - screenPos, null, Color.White.WithAlpha(0) * opacity, MathHelper.PiOver4, flareOrigin, 1f, 0, 0);
            }

            Vector2 position = TeleVector - screenPos;
            Vector2 position2 = oldTeleVector - screenPos;
            Color colour = Color.Lerp(Color.White, Color.Cyan, 1f / TeleGlowTimer * 10f) * (1f / TeleGlowTimer * 10f);
            if (TeleGlow)
            {
                spriteBatch.Draw(flare.Value, position, null, colour.WithAlpha(0) * ((60 - TeleGlowTimer) / 60f), 0, flareOrigin, 1f, 0, 0);
                spriteBatch.Draw(flare.Value, position, null, colour.WithAlpha(0) * 0.4f * ((60 - TeleGlowTimer) / 60f), 0, flareOrigin, 1f, 0, 0);

                spriteBatch.Draw(flare.Value, position2, null, colour.WithAlpha(0) * ((60 - TeleGlowTimer) / 60f), 0, flareOrigin, 1f, 0, 0);
                spriteBatch.Draw(flare.Value, position2, null, colour.WithAlpha(0) * 0.4f * ((60 - TeleGlowTimer) / 60f), 0, flareOrigin, 1f, 0, 0);
            }

            if (NPC.frame.X == 6 * NPC.frame.Width && NPC.frame.Y == NPC.frame.Height)
            {
                position = NPC.Center + RedeHelper.OffsetWithRotation(NPC, 2, 56) - screenPos;
                spriteBatch.Draw(flare.Value, position, null, Color.Cyan.WithAlpha(0) * .5f, 0, flareOrigin, 1f, 0, 0);
                spriteBatch.Draw(flare.Value, position, null, Color.White.WithAlpha(0), 0, flareOrigin, .6f, 0, 0);
                spriteBatch.Draw(flare.Value, position, null, Color.White.WithAlpha(0), 0, flareOrigin, .2f, 0, 0);

            }
        }
    }
}