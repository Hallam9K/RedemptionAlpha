using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.DataStructures;
using Redemption.Buffs.Debuffs;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Items.Usable;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Redemption.Base;
using Redemption.Dusts;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items;
using Redemption.Items.Materials.HM;
using Redemption.Items.Accessories.HM;
using Redemption.Buffs.NPCBuffs;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Armor.Vanity;
using ReLogic.Content;
using Redemption.UI.ChatUI;
using Redemption.UI;
using Terraria.Localization;

namespace Redemption.NPCs.Bosses.KSIII
{
    [AutoloadBossHead]
    public class KS3 : ModNPC
    {
        private static Asset<Texture2D> Glow;
        private static Asset<Texture2D> Arms;
        private static Asset<Texture2D> ArmsGlow;
        private static Asset<Texture2D> Head;
        private static Asset<Texture2D> HeadGlow;
        private static Asset<Texture2D> Overclock;
        private static Asset<Texture2D> OverclockGlow;
        private static Asset<Texture2D> OverclockArmsGlow;
        public override void Load()
        {
            Glow = ModContent.Request<Texture2D>(Texture + "_Glow");
            Arms = ModContent.Request<Texture2D>(Texture + "_Arms");
            ArmsGlow = ModContent.Request<Texture2D>(Texture + "_Arms_Glow");
            Head = ModContent.Request<Texture2D>(Texture + "_Heads");
            HeadGlow = ModContent.Request<Texture2D>(Texture + "_Heads_Glow");
            Overclock = ModContent.Request<Texture2D>(Texture + "_Overclock");
            OverclockGlow = ModContent.Request<Texture2D>(Texture + "_Overclock_Glow");
            OverclockArmsGlow = ModContent.Request<Texture2D>(Texture + "_Arms_Overclock_Glow");
        }
        public override void Unload()
        {
            Glow = null;
            Arms = null;
            ArmsGlow = null;
            Head = null;
            HeadGlow = null;
            Overclock = null;
            OverclockGlow = null;
            OverclockArmsGlow = null;
        }
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
            // DisplayName.SetDefault("King Slayer III");
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
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
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
            NPC.lifeMax = 36500;
            NPC.defense = 30;
            NPC.damage = 84;
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
            NPC.rarity = 1;
            voice = CustomSounds.Voice6 with { Pitch = 0.2f };
            bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Slayer").Value;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossSlayer");
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Psychic] *= 1.25f;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.velocity.Length() >= 13f && AIState is not ActionState.PhysicalAttacks;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("'What? You want my lore? Go get your own lore!'")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SlayerBag>()));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<KS3Relic>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KS3Trophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SlayerProjector>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<KingSlayerMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<SlayerGun>(), ModContent.ItemType<Nanoswarmer>(), ModContent.ItemType<SlayerFist>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SlayerController>(), 10));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Holokey>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CyberPlating>(), 1, 14, 18));

            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ModContent.ProjectileType<KS3_Exit>(), 0, Vector2.Zero, false, SoundID.Item1);
            if (NPC.ai[0] != 11)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<SlayerMedal>());

            if (!RedeBossDowned.downedSlayer)
            {
                RedeWorld.alignment -= NPC.ai[0] == 11 ? 0 : 2;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, NPC.ai[0] == 11 ? "+0" : "-2", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                    {
                        if (AIState is ActionState.Spared)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.KS3Spared"), 240, 30, 0, Color.DarkGoldenrod);
                        else
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.KS3Defeat"), 240, 30, 0, Color.DarkGoldenrod);
                    }
                }
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
                modifiers.FinalDamage *= 0.75f;
            else
                modifiers.FinalDamage *= 0.85f;
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

        private SoundStyle voice;
        private Texture2D bubble;
        public readonly Vector2 modifier = new(0, -260);
        public override void AI()
        {
            if (AIState > ActionState.PhysicalAttacks || AIState is ActionState.Dialogue)
                NPC.DiscourageDespawn(120);
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
            else if (NPC.life < (int)(NPC.lifeMax * 0.05f) && phase < 4)
                AIState = ActionState.PhaseChange;

            player.RedemptionScreen().ScreenFocusPosition = NPC.Center;

            Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(54, gunRot) + RedeHelper.PolarVector(13 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
            int dmgIncrease = NPC.DistanceSQ(player.Center) > 800 * 800 ? 10 : 0;

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("King Slayer III", 60, 90, 0.8f, 0, Color.Cyan, "Prototype Multium");

                    NPC.LookAtEntity(player);
                    BodyState = (int)BodyAnim.Crossed;
                    player.RedemptionScreen().Rumble(5, 5);
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
                    player.RedemptionScreen().lockScreen = true;
                    player.RedemptionScreen().cutscene = true;
                    NPC.LockMoveRadius(player);
                    if (RedeBossDowned.slayerDeath < 3)
                    {
                        if (AITimer == 30)
                        {
                            HeadType = 2;
                            if (!Main.dedServ)
                            {
                                string line1;
                                if (RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_DroneKillCheck>()))
                                    line1 = "Did you seriously just destroy my drones?";
                                else
                                {
                                    if (RedeWorld.alignment >= 0)
                                    {
                                        if (player.IsFullTBot())
                                            line1 = "Alright listen here you little scrap of metal.";
                                        else if (player.RedemptionPlayerBuff().ChickenForm)
                                            line1 = "Alright listen here you little chicken nugget.";
                                        else
                                            line1 = "Alright listen here you little fleshbag.";
                                    }
                                    else
                                    {
                                        if (player.IsFullTBot())
                                            line1 = "Ah,[0.1] this little scrap of metal decided to save me the trouble of finding it.";
                                        else if (player.RedemptionPlayerBuff().ChickenForm)
                                            line1 = "Ah,[0.1] this little chicken nugget decided to save me the trouble of finding it.";
                                        else
                                            line1 = "Ah,[0.1] this little fleshbag decided to save me the trouble of finding it.";
                                    }
                                }
                                string line2;
                                if (RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_DroneKillCheck>()))
                                    line2 = "Eh,[0.1] not like I got a shortage of them,[0.1] but I'm still gonna blast ya for it!";
                                else
                                {
                                    if (RedeWorld.alignment >= 0)
                                    {
                                        line2 = "I warned you,[0.1] so don't go crying to your mummy when I crush you into the ground!";
                                        if (player.Redemption().slayerStarRating >= 6)
                                            line2 = "Do you understand how annoying it is to be spam-pinged by my units because of your trigger-happy arse?[1] Maybe I should teach you a lesson!";
                                    }
                                    else
                                        line2 = "You were on my hitlist,[0.1] so lets skip the small talk and get on with it!";
                                }
                                player.Redemption().slayerStarRating = 0;

                                DialogueChain chain = new();
                                chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                     .Add(new(NPC, "[@h1]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, !RedeBossDowned.downedKeeper, null, bubble, null, modifier, RedeBossDowned.downedKeeper ? 0 : 1));
                                if (RedeBossDowned.downedKeeper)
                                {
                                    chain.Add(new(NPC, "[@b0][@h0]Actually...[0.5] You were the one that fought the Keeper, weren't you!", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier));
                                    chain.Add(new(NPC, "[@h2]That was my job!", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier));
                                    chain.Add(new(NPC, "[@h0]Great,[0.1] now I have even more reason to pummel you to ash!", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
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
                            if (!Main.dedServ)
                                RedeSystem.Instance.TitleCardUIElement.DisplayTitle("King Slayer III", 60, 90, 0.8f, 0, Color.Cyan, "Prototype Multium");

                            ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));
                            if (RedeBossDowned.slayerDeath < 3)
                            {
                                RedeBossDowned.slayerDeath = 3;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);
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
                            string line1;
                            if (RedeBossDowned.downedSlayer)
                            {
                                line1 = Main.rand.Next(5) switch
                                {
                                    1 => "[@h1]Why must you summon me again?",
                                    2 => "[@h2]Could you maybe possibly probably potentially LEAVE ME ALONE?",
                                    3 => "[@h1]Really,[0.1] a rematch?[0.5] Fine.",
                                    4 => "[@h1]Why don't you go preemptively die and save me the effort.",
                                    _ => "[@h4]What?[0.5] Do you want to fight me again?",
                                };
                            }
                            else
                            {
                                line1 = Main.rand.Next(5) switch
                                {
                                    1 => "[@h3]Could've sworn you died...",
                                    2 => "Ready for a rematch?",
                                    3 => "Welp,[0.1] time to win again!",
                                    4 => "Still wanna fight?",
                                    _ => "[@h3]You're quite a resilient fellow...",
                                };
                            }
                            DialogueChain chain = new();
                            chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
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
                            if (!Main.dedServ)
                                RedeSystem.Instance.TitleCardUIElement.DisplayTitle("King Slayer III", 60, 90, 0.8f, 0, Color.Cyan, "Prototype Multium");

                            NPC.dontTakeDamage = false;
                            ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);
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
                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    break;
                case ActionState.GunAttacks:
                    if (AttackChoice != 2 || AITimer <= 200)
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
                                        gunRot = 0;
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
                                ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));

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
                                    NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot), true, CustomSounds.Gun1KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer % 120 == 0)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        int rot = 25 * i;
                                        NPC.Shoot(GunOrigin, ProjectileID.MartianTurretBolt, (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 25)), true, CustomSounds.Gun3KS);
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
                                    NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot), true, CustomSounds.Gun1KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer % 100 == 0)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int rot = 25 * i;
                                        NPC.Shoot(GunOrigin, ProjectileID.MartianTurretBolt, (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 50)), true, CustomSounds.Gun3KS);
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
                                    NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot), true, CustomSounds.Gun1KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer % 105 == 0)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int rot = 25 * i;
                                        NPC.Shoot(GunOrigin, ProjectileID.MartianTurretBolt, (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 50)), true, CustomSounds.Gun3KS);
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
                            if (AITimer++ == 0)
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_TeleLine1>(), 0, RedeHelper.PolarVector(10, gunRot), false, SoundID.Item1, ai1: NPC.whoAmI);

                            ShootPos = new Vector2(300 * NPC.RightOfDir(player), 10);

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
                                    NPC.velocity.X = -9 * NPC.spriteDirection;
                                    for (int i = 0; i < Main.rand.Next(5, 8); i++)
                                    {
                                        NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(Main.rand.Next(8, 13) + dmgIncrease, gunRot + Main.rand.NextFloat(-0.14f, 0.14f)), true, CustomSounds.ShotgunBlastKS);
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
                                gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                                SnapGunToFiringArea();
                                AITimer++;
                                ShootPos = new Vector2(450 * NPC.RightOfDir(player), -10);
                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                                if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                                {
                                    ArmsFrameY = 1;
                                    ArmsFrameX = 0;
                                    BodyState = (int)BodyAnim.Gun;
                                }

                                if (AITimer == 40)
                                {
                                    NPC.Shoot(GunOrigin, ModContent.ProjectileType<ReboundShot>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(15 + dmgIncrease, gunRot), true, CustomSounds.Gun2KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
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
                                ShootPos = new Vector2(450 * NPC.RightOfDir(player), -10);
                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

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
                                    NPC.Shoot(GunOrigin, ModContent.ProjectileType<ReboundShot>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(15 + dmgIncrease, gunRot), true, CustomSounds.Gun2KS);
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;
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
                                    ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));

                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                                if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                                {
                                    ArmsFrameY = 1;
                                    ArmsFrameX = 0;
                                    BodyState = (int)BodyAnim.Gun;
                                }

                                if (AITimer % 10 == 0)
                                {
                                    NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(7 + dmgIncrease, gunRot), true, CustomSounds.Gun1KS);
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
                    if (AttackChoice != 3 || AITimer <= 120)
                        NPC.LookAtEntity(player);

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
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 10);
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
                                    if (BodyState is (int)BodyAnim.Idle)
                                    {
                                        gunRot = 0;
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
                                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

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
                            ShootPos = new Vector2(300 * NPC.RightOfDir(player), -60);
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
                                if (AITimer == 105)
                                    BodyState = (int)BodyAnim.RocketFist;

                                if (AITimer == 120)
                                    NPC.Shoot(new Vector2(NPC.Center.X + 15 * NPC.spriteDirection, NPC.Center.Y - 11), ModContent.ProjectileType<KS3_Fist>(), NPC.damage, new Vector2(10 * NPC.spriteDirection, 0), true, CustomSounds.MissileFire1);

                                if (AITimer > 150)
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
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                ShootPos = new Vector2(200 * NPC.RightOfDir(player), 0);
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
                                    {
                                        ArmsFrameY = 5;
                                        ArmsCounter = 0;
                                        BodyState = (int)BodyAnim.Grenade;
                                    }

                                    if (AITimer == 140)
                                        NPC.Shoot(new Vector2(NPC.Center.X + 21 * NPC.spriteDirection, NPC.Center.Y - 17), ModContent.ProjectileType<KS3_FlashGrenade>(), (int)(NPC.damage * .9f), new Vector2(10 * NPC.spriteDirection, -6), true, SoundID.Item1);

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
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(320 * NPC.RightOfDir(player), 0);
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
                                        NPC.Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_BeamCell>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(35 * NPC.spriteDirection)),
                                            true, SoundID.Item103, ai0: NPC.whoAmI);

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
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(80 * NPC.RightOfDir(player), 0);
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
                                        NPC.Shoot(new Vector2(NPC.spriteDirection == 1 ? NPC.Center.X + 2 : NPC.Center.X - 2, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_Surge>(), (int)(NPC.damage * .9f), Vector2.Zero, true, CustomSounds.ElectricNoise, NPC.whoAmI);

                                        for (int i = 0; i < 18; i++)
                                            NPC.Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_Surge2>(), 0, RedeHelper.PolarVector(14, MathHelper.ToRadians(20) * i), false, SoundID.Item1);

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
                                BodyState = (int)BodyAnim.Shrug;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 21)
                                {
                                    for (int k = 0; k < NPC.buffImmune.Length; k++)
                                    {
                                        if (BuffID.Sets.IsAnNPCWhipDebuff[k])
                                            continue;
                                        NPC.buffImmune[k] = true;
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
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                if (AITimer++ % 20 == 0)
                                    ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));

                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);
                                if (AITimer == 16)
                                    NPC.Shoot(new Vector2(NPC.Center.X + 48 * NPC.spriteDirection, NPC.Center.Y - 12), ModContent.ProjectileType<KS3_Reflect>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

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
                                    AttackChoice = Main.rand.Next(1, 10);
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
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                                    if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()))
                                    {
                                        for (int i = 0; i < Main.rand.Next(2, 5); i++)
                                        {
                                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800),
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
                                    AttackChoice = Main.rand.Next(1, 10);
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
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                                    if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()))
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800),
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

                        #region Missile Barrage
                        case 9:
                            if (AITimer == 0)
                            {
                                if (phase > 2 && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_SoSCrosshair>()) && Main.rand.NextBool(4))
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
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                                    if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_SoSCrosshair>()))
                                        NPC.Shoot(player.Center, ModContent.ProjectileType<KS3_SoSCrosshair>(), (int)(NPC.damage * 1.8f), Vector2.Zero, false, SoundID.Item1, NPC.whoAmI);
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
                                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

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
                                ShootPos = new Vector2(200 * NPC.RightOfDir(player), -60);
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
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Wave>(), NPC.damage, Vector2.Zero, true, SoundID.Item74, ai0: NPC.whoAmI);
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

                                        int hitDirection = target.RightOfDir(NPC);
                                        BaseAI.DamagePlayer(target, NPC.damage, 3, hitDirection, NPC);
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
                                    ShootPos = new Vector2(200 * NPC.RightOfDir(player), -60);
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
                            if (AITimer == 1)
                                Teleport(false, Vector2.Zero);
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
                                    ShootPos = new Vector2(100 * NPC.RightOfDir(player), 0);
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                                }
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.rotation = 0;
                                NPC.velocity *= 0.8f;
                                if (AITimer == 101)
                                    NPC.velocity.X = NPC.RightOfDir(player) * 6;

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

                                        int hitDirection = target.RightOfDir(NPC);
                                        if (!target.noKnockback)
                                            target.velocity.X = 20 * NPC.spriteDirection;
                                        BaseAI.DamagePlayer(target, NPC.damage, 3, hitDirection, NPC);
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
                            if (Main.expertMode)
                            {
                                NPC.LookAtEntity(player);
                                AITimer++;
                                if (AITimer < 100)
                                {
                                    NPC.rotation = 0;
                                    if (NPC.DistanceSQ(ShootPos) < 100 * 100 || AITimer > 50)
                                    {
                                        ShootPos = new Vector2(150 * NPC.RightOfDir(player), 200);
                                        AITimer = 100;
                                        NPC.velocity.X = 0;
                                        NPC.velocity.Y = -25;

                                        NPC.frame.X = 2 * NPC.frame.Width;
                                        NPC.frame.Y = 160;
                                        BodyState = (int)BodyAnim.DropkickStart;

                                        SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_TeleLine2>(), 0, NPC.DirectionTo(player.Center + player.velocity * 20f), false, SoundID.Item1, ai1: NPC.whoAmI);
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

                                            int hitDirection = target.RightOfDir(NPC);
                                            BaseAI.DamagePlayer(target, (int)(NPC.damage * 1.1f), 3, hitDirection, NPC);
                                        }
                                    }

                                    if (AITimer == 205)
                                        NPC.Dash(40, true, SoundID.Item74, player.Center + player.velocity * 20f);

                                    if (AITimer > 260 || NPC.Center.Y > player.Center.Y + 400)
                                    {
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
                            AITimer++;
                            if (AITimer == 1 && NPC.DistanceSQ(player.Center) > 300 * 300)
                                Teleport(false, Vector2.Zero);
                            NPC.rotation = NPC.velocity.X * 0.01f;
                            ShootPos = new Vector2(60 * NPC.RightOfDir(player), 20);
                            if (AITimer < 100)
                            {
                                NPC.LookAtEntity(player);
                                if ((NPC.DistanceSQ(player.Center + ShootPos) < 50 * 50 && gunRot != 0) || AITimer > 40)
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
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.velocity *= 0.9f;
                                if (AITimer == 105)
                                {
                                    gunRot += 1;
                                    NPC.Dash(10, false, CustomSounds.Swoosh1, player.Center);
                                }

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

                                        int hitDirection = target.RightOfDir(NPC);
                                        BaseAI.DamagePlayer(target, (int)(NPC.damage * .9f), 3, hitDirection, NPC);
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
                                    if (gunRot <= 1 || RedeHelper.Chance(0.35f))
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
                                ShootPos = new Vector2(80 * NPC.RightOfDir(player), 20);

                                if (AITimer == 5)
                                {
                                    NPC.frame.X = 7 * NPC.frame.Width;
                                    NPC.frame.Y = 160 * 2;
                                    BodyState = (int)BodyAnim.Jojo;
                                }

                                NPC.Move(ShootPos, NPC.Distance(player.Center) > 300 ? 20f : 9f, 8f, true);
                                if (AITimer >= 15 && AITimer % 3 == 0)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_JojoFist>(), (int)(NPC.damage * .9f), Vector2.Zero, true, SoundID.Item60 with { Volume = .3f }, ai0: NPC.whoAmI);
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
                    if (NPC.DistanceSQ(player.Center) >= 600 * 600)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    if (AITimer++ == 5)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);
                        Teleport(false, Vector2.Zero);
                    }

                    if (RedeBossDowned.slayerDeath >= 4)
                    {
                        if (AITimer == 30)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_MissileDrone>(), NPC.whoAmI);
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
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        if (AITimer < 5000)
                            player.RedemptionScreen().lockScreen = true;
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 1;
                            string line1;
                            if (RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                            {
                                if (player.HeldItem.DamageType == DamageClass.Melee)
                                    line1 = "What a nuisance.[1] It would seem my Auto-Shield is ineffective to your attacks.";
                                else
                                    line1 = "What a nuisance.[1] Your petty projectiles are going through my Auto-Shield.";
                            }
                            else
                                line1 = "What a nuisance.[1] You are only wasting both of our efforts here.";

                            string line2;
                            if (TeleportCount > 6)
                                line2 = "Why'd you summon me if you're just gonna run away the entire time?";
                            else
                                line2 = "Might as well blow you to pieces with a few missiles.";

                            DialogueChain chain = new();
                            chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h2]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer == 5001)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_MissileDrone>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 5040)
                        {
                            if (RedeBossDowned.slayerDeath < 4)
                            {
                                RedeBossDowned.slayerDeath = 4;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

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
                    if (NPC.DistanceSQ(player.Center) >= 600 * 600)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    if (AITimer++ == 5)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);
                        Teleport(false, Vector2.Zero);
                    }

                    if (RedeBossDowned.slayerDeath >= 5)
                    {
                        if (AITimer == 30)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_Magnet>(), NPC.whoAmI);
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
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        if (AITimer < 5000)
                            player.RedemptionScreen().lockScreen = true;
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 4;
                            string line1;
                            if (player.IsFullTBot())
                                line1 = "This rusty little tincan is more persistent than I thought...";
                            else if (player.RedemptionPlayerBuff().ChickenForm)
                                line1 = "The concept of losing to a chicken does not bode well with me...";
                            else
                                line1 = "You pack more of a punch than I thought for such a small fleshbag...";

                            string line2;
                            if (player.HeldItem.DamageType == DamageClass.Ranged || player.HeldItem.DamageType == DamageClass.Magic)
                                line2 = "You like shooting things,[0.1] correct?[0.5] Well try shooting me now.";
                            else
                                line2 = "Go ahead,[0.1] shoot me if you can.";

                            DialogueChain chain = new();
                            chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h3]I might even have to take you seriously...", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h0]PAH![0.5] What a joke!", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h2]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer == 5001)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                            HeadType = 0;
                            if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800), ModContent.NPCType<KS3_Magnet>(), NPC.whoAmI);
                                }
                            }
                        }
                        if (AITimer > 5040)
                        {
                            if (RedeBossDowned.slayerDeath < 5)
                            {
                                RedeBossDowned.slayerDeath = 5;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

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
                    if (NPC.DistanceSQ(player.Center) >= 600 * 600)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    if (AITimer++ == 5)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);
                        Teleport(false, Vector2.Zero);
                    }

                    if (RedeBossDowned.slayerDeath >= 6)
                    {
                        if (AITimer == 30)
                        {
                            HeadType = 0;
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_SoSCrosshair>()))
                                NPC.Shoot(player.Center, ModContent.ProjectileType<KS3_SoSCrosshair>(), 98, Vector2.Zero, false, SoundID.Item1, NPC.whoAmI);
                        }
                        if (AITimer > 80)
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    else
                    {
                        if (AITimer < 5000)
                            player.RedemptionScreen().lockScreen = true;
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 2;
                            DialogueChain chain = new();
                            chain.Add(new(NPC, "This is getting ridiculous![0.5] Why can't I kill you?", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h3]*Ahem*[1] Your persistence is admirable,[0.1] I'll give you that.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h2]But you better realise I'm hardly trying.[1][@h1] I ain't bluffing either.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer == 5001)
                        {
                            HeadType = 0;
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, true, CustomSounds.Alarm2, NPC.whoAmI);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_SoSCrosshair>()))
                                NPC.Shoot(player.Center, ModContent.ProjectileType<KS3_SoSCrosshair>(), 98, Vector2.Zero, false, SoundID.Item1, NPC.whoAmI);
                        }
                        if (AITimer > 5040)
                        {
                            if (RedeBossDowned.slayerDeath < 6)
                            {
                                RedeBossDowned.slayerDeath = 6;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            NPC.dontTakeDamage = false;
                            AttackChoice = 0;
                            AITimer = 0;
                            AIState = (ActionState)Main.rand.Next(3, 5);
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()))
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

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
                    NPC.velocity *= 0.9f;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    player.RedemptionScreen().lockScreen = true;
                    NPC.LockMoveRadius(player);
                    if (AITimer++ == 5)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

                    if (RedeBossDowned.slayerDeath >= 7)
                    {
                        if (AITimer == 20 && !Main.dedServ)
                        {
                            HeadType = 3;
                            DialogueChain chain = new();
                            chain.Add(new(NPC, "If you stop attacking,[0.1] I'll go back to more [0.1]IMPORTANT[0.1] business.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
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
                                line1 = "You've just been flying away the entire fight.[1] Seriously.";
                            else
                                line1 = "I'm too tired to get mad about this nonsense.";

                            DialogueChain chain = new();
                            chain.Add(new(NPC, "Alright alright alright!", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h1]We'll...[0.5] call it a draw then.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h2]If you agree to stop,[0.1] I'll go back to more [0.1]IMPORTANT[0.1] business.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                                 .Add(new(NPC, "[@h3]But,[0.1] if you so choose,[0.1] we can continue...[1] But I won't be happy if I lose.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer > 5000)
                        {
                            if (RedeWorld.alignmentGiven && !Main.dedServ && !RedeBossDowned.downedSlayer)
                                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.KS3Choice"), 180, 30, 0, Color.DarkGoldenrod);

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
                    player.RedemptionScreen().lockScreen = true;
                    NPC.LockMoveRadius(player);
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (!Main.dedServ && !YesNoUI.Visible)
                        RedeSystem.Instance.YesNoUIElement.DisplayYesNoButtons("Call Draw", "Continue", new Vector2(0, 28), new Vector2(0, 28), .6f, .6f);

                    if (player.Redemption().yesChoice)

                    {
                        if (ChaliceAlignmentUI.Visible)
                            ChaliceAlignmentUI.Visible = false;

                        if (RedeBossDowned.slayerDeath < 7)
                        {
                            RedeBossDowned.slayerDeath = 7;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        YesNoUI.Visible = false;
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
                        YesNoUI.Visible = false;
                        AITimer = 0;
                        AIState = ActionState.Attacked;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacked:
                    #region Attacked
                    NPC.LookAtEntity(player);
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    player.RedemptionScreen().lockScreen = true;
                    player.RedemptionScreen().cutscene = true;
                    NPC.LockMoveRadius(player);
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (AITimer == 5)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield2>(), 0, Vector2.Zero, false, SoundID.Item1, ai0: NPC.whoAmI);

                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 1;
                        DialogueChain chain = new();
                        chain.Add(new(NPC, "I see how it is...", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier));
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer == 180)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 0);

                    if (AITimer == 190)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 1);

                    if (AITimer == 200)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 2);

                    if (AITimer == 210)
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(800, 900), ModContent.NPCType<SpaceKeeper>(), NPC.whoAmI, 3);

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
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    #endregion
                    break;
                case ActionState.Spared:
                    #region Spared
                    NPC.LookAtEntity(player);
                    player.RedemptionScreen().lockScreen = true;
                    player.RedemptionScreen().cutscene = true;
                    NPC.LockMoveRadius(player);
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
                        chain.Add(new(NPC, "Tie it is then.[1] Now don't distract me again.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                             .Add(new(NPC, "[@h1]Adios,[0.1] dingus.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer > 5001)
                    {
                        NPC.dontTakeDamage = false;
                        if (RedeBossDowned.slayerDeath < 7)
                        {
                            RedeBossDowned.slayerDeath = 7;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    #endregion
                    break;
                case ActionState.Overclock:
                    NPC.LookAtEntity(player);
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    if (AITimer < 4000)
                    {
                        player.RedemptionScreen().lockScreen = true;
                        NPC.LockMoveRadius(player);
                    }
                    if (AITimer < 120 && !Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
                    else
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossSlayer2");

                    if (AITimer == 30 && !Main.dedServ)
                    {
                        HeadType = 2;
                        string line1;
                        if (RedeBossDowned.slayerDeath >= 8)
                            line1 = "Once again,[0.1] you really are eager to win...";
                        else
                            line1 = "I'm disappointed I actually have to overclock this vessel...";

                        string line2;
                        if (RedeBossDowned.slayerDeath >= 8)
                            line2 = "... I guess you like doing things the hard way.";
                        else
                        {
                            if (player.IsFullTBot())
                                line2 = "... And for a heap of scrap no less.";
                            else if (player.RedemptionPlayerBuff().ChickenForm)
                                line2 = "... And for what?[0.5] A bloody chicken!?";
                            else
                                line2 = "... And for an annoying brat no less.";
                        }

                        DialogueChain chain = new();
                        chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                             .Add(new(NPC, "[@h4]" + line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, bubble, null, modifier))
                             .Add(new(NPC, "[@h0][@c]Let's begin.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 5000)
                    {
                        if (RedeBossDowned.slayerDeath < 8)
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
            }
            #region Teleporting
            if (NPC.DistanceSQ(player.Center) >= 1100 * 1100 && NPC.ai[0] > 0 && !player.RedemptionScreen().lockScreen)
            {
                if (AttackChoice == 3 && AIState is ActionState.PhysicalAttacks)
                    return;
                TeleportCount++;
                Teleport(false, Vector2.Zero);
                NPC.netUpdate = true;
            }
            #endregion
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
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
            if (signature == "b0")
                BodyState = 0;
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 5000;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (AIState is ActionState.PhysicalAttacks)
            {
                if (AttackChoice == 2)
                    target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 30);
                if (AttackChoice == 3)
                    target.AddBuff(ModContent.BuffType<StaticStunDebuff>(), 120);
            }
        }
        public override bool CheckDead()
        {
            if (phase >= 5 || AIState is ActionState.Spared)
                return true;
            else
            {
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

                HeadFrame = HeadType switch
                {
                    1 => HeadFrame = (NPC.frame.X / NPC.frame.Width) + 4, // Bored
                    2 => HeadFrame = (NPC.frame.X / NPC.frame.Width) + 8, // Angry
                    3 => HeadFrame = (NPC.frame.X / NPC.frame.Width) + 12, // Suspicious
                    4 => HeadFrame = (NPC.frame.X / NPC.frame.Width) + 16, // Confused
                    _ => HeadFrame = NPC.frame.X / NPC.frame.Width // Normal
                };
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
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = Arms.Value.Height / 6;
            int width = Arms.Value.Width / 10;
            int y = height * ArmsFrameY;
            int x = width * ArmsFrameX;
            Rectangle ArmsRect = new(x, y, width, height);
            Vector2 ArmsOrigin = new(width / 2f, height / 2f);
            Vector2 ArmsPos = new(NPC.Center.X, NPC.Center.Y - 13);

            if (!NPC.IsABestiaryIconDummy)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(phase < 5 ? TextureAssets.Npc[NPC.type].Value : Overclock.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);
                }
                if (BodyState < (int)BodyAnim.IdlePhysical)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        Main.spriteBatch.Draw(Arms.Value, oldPos - new Vector2(0, 13) + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(ArmsRect), NPC.GetAlpha(Color.LightCyan) * 0.5f, BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                        gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);
                    }
                }
            }
            spriteBatch.Draw(phase < 5 ? TextureAssets.Npc[NPC.type].Value : Overclock.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);
            spriteBatch.Draw(phase < 5 ? Glow.Value : OverclockGlow.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);

            if (AIState != ActionState.GunAttacks && AIState != ActionState.PhysicalAttacks && AIState != ActionState.SpecialAttacks && NPC.velocity.Length() < 13f && phase < 5)
            {
                Vector2 HeadPos = new(NPC.Center.X - 2 * NPC.spriteDirection, NPC.Center.Y - 35);
                int HeadHeight = Head.Value.Height / 20;
                int yHead = HeadHeight * HeadFrame;
                Rectangle HeadRect = new(0, yHead, Head.Value.Width, HeadHeight);
                spriteBatch.Draw(Head.Value, HeadPos - screenPos, new Rectangle?(HeadRect), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(Head.Value.Width / 2f, HeadHeight / 2f), NPC.scale * 2, effects, 0f);
                spriteBatch.Draw(HeadGlow.Value, HeadPos - screenPos, new Rectangle?(HeadRect), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(Head.Value.Width / 2f, HeadHeight / 2f), NPC.scale * 2, effects, 0f);
            }

            if (!NPC.IsABestiaryIconDummy && NPC.dontTakeDamage && !Main.dedServ && spriteBatch != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:ScanShader"]?.GetShader().Shader;
                effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["alpha"].SetValue(1);
                effect.Parameters["red"].SetValue(new Color(0.1f, 1f, 1f, 1).ToVector4());
                effect.Parameters["red2"].SetValue(new Color(0.1f, 1f, 1f, 0.9f).ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            if (BodyState < (int)BodyAnim.IdlePhysical)
            {
                spriteBatch.Draw(Arms.Value, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(drawColor),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                spriteBatch.Draw(phase < 5 ? ArmsGlow.Value : OverclockArmsGlow.Value, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(Color.White),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                if (!NPC.IsABestiaryIconDummy && NPC.dontTakeDamage && !Main.dedServ && spriteBatch != null)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                    Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:ScanShader"]?.GetShader().Shader;
                    effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                    effect.Parameters["alpha"].SetValue(1);
                    effect.Parameters["red"].SetValue(new Color(0.1f, 1f, 1f, 1).ToVector4());
                    effect.Parameters["red2"].SetValue(new Color(0.1f, 1f, 1f, 0.9f).ToVector4());

                    effect.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(Arms.Value, ArmsPos - screenPos, new Rectangle?(ArmsRect), Color.White,
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                }
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
