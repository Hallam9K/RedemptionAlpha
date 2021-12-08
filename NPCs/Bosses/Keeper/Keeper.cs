using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using System.Linq;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.Dusts;
using Redemption.NPCs.Friendly;

namespace Redemption.NPCs.Bosses.Keeper
{
    [AutoloadBossHead]
    public class Keeper : ModNPC
    {
        public static int secondStageHeadSlot = -1;
        public override void Load()
        {
            string texture = BossHeadTexture + "_Unveiled";
            secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1);
        }

        public override void BossHeadSlot(ref int index)
        {
            int slot = secondStageHeadSlot;
            if (Unveiled && slot != -1)
            {
                index = slot;
            }
        }

        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Unveiled,
            Death,
            SkullDiggerSummon,
            Teddy
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
            DisplayName.SetDefault("The Keeper");
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused
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
            NPC.aiStyle = -1;
            NPC.lifeMax = 3500;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.width = 52;
            NPC.height = 128;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 3, 50, 0);
            NPC.SpawnWithHigherTime(30);
            NPC.alpha = 255;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossKeeper");
            BossBag = ModContent.ItemType<KeeperBag>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, Scale: 3);
                    Main.dust[dustIndex].velocity *= 4f;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement("A powerful fallen who had learnt forbidden necromancy, its prolonged usage having mutated her body.")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KeeperTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<KeeperRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<OcciesCollar>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<KeepersVeil>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<SoulScepter>(), ModContent.ItemType<KeepersClaw>(), ModContent.ItemType<FanOShivs>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GrimShard>(), 1, 2, 4));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), -0.6f);

            if (!RedeBossDowned.downedKeeper)
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<SorrowfulEssence>());

                RedeWorld.alignment++;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "+1", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("An undead... disgusting. Good thing you killed it.", 240, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedKeeper, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(ID);
                writer.Write(Unveiled);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ID = reader.ReadInt32();
                Unveiled = reader.ReadBoolean();
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

                attempts++;
            }
        }

        public List<int> AttackList = new() { 0, 1, 2, 3, 4 };
        public List<int> CopyList = null;

        private bool Unveiled;
        private float move;
        private float speed = 6;
        private bool SoulCharging;
        private bool Reap;
        private Vector2 origin;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 64 : NPC.Center.X + 26), (int)(NPC.Center.Y - 38), 38, 86);
            SoulCharging = false;

            bool sorrowfulEssence = false;
            bool teddy = false;
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player2 = Main.player[k];
                if (!player2.active || player2.dead)
                    continue;
                if (player2.HasItem(ModContent.ItemType<SorrowfulEssence>()))
                    sorrowfulEssence = true;
                if (player2.HasItem(ModContent.ItemType<AbandonedTeddy>()))
                    teddy = true;
            }
            DespawnHandler();

            if (AIState != ActionState.Death && AIState != ActionState.Unveiled && AIState != ActionState.Attacks)
                NPC.LookAtEntity(player);

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("The Keeper", 60, 90, 0.8f, 0, Color.MediumPurple, "Octavia von Gailon");

                        NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 160 : player.Center.X + 160, player.Center.Y - 90);
                        NPC.netUpdate = true;
                    }
                    NPC.alpha -= 2;
                    if (NPC.alpha <= 0)
                    {
                        if (teddy && !RedeConfigClient.Instance.NoLoreElements)
                            AIState = ActionState.Teddy;
                        else
                            AIState = ActionState.Idle;

                        AITimer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Idle:
                    if (AITimer++ == 0)
                    {
                        move = NPC.Center.X;
                        speed = 6;
                    }
                    Reap = false;

                    NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                    MoveClamp();
                    if (NPC.DistanceSQ(player.Center) > (NPC.dontTakeDamage ? (800 * 800) : (400 * 400)))
                        speed *= 1.03f;
                    else if (NPC.dontTakeDamage && NPC.velocity.Length() > 6 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                        speed *= 0.96f;

                    if (!Unveiled && NPC.life < NPC.lifeMax / 2)
                    {
                        NPC.velocity *= 0;
                        AITimer = 0;
                        AIState = ActionState.Unveiled;
                        NPC.netUpdate = true;
                        break;
                    }
                    if (NPC.dontTakeDamage ? AITimer == -1 : AITimer > 60)
                    {
                        NPC.dontTakeDamage = false;
                        AttackChoice();
                        AITimer = 0;
                        AIState = ActionState.Attacks;
                        NPC.netUpdate = true;

                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
                case ActionState.Attacks:
                    if (!Unveiled && NPC.life < NPC.lifeMax / 2)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Unveiled;
                        NPC.netUpdate = true;
                        break;
                    }
                    switch (ID)
                    {
                        #region Reaper Slash
                        case 0:
                            int alphaTimer = Main.expertMode ? 20 : 10;
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (AITimer < 40)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.velocity *= 0.9f;
                                }
                                if (AITimer == 40)
                                {
                                    SoundEngine.PlaySound(SoundID.Zombie, (int)NPC.position.X, (int)NPC.position.Y, 83, 1, 0.3f);
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -6f * NPC.spriteDirection;
                                }
                                if (AITimer >= 40)
                                {
                                    NPC.alpha += alphaTimer;
                                    NPC.velocity *= 0.96f;
                                }
                                if (NPC.alpha >= 255)
                                {
                                    Reap = true;
                                    NPC.velocity *= 0f;
                                    NPC.position = new Vector2(player.Center.X + (player.velocity.X > 0 ? 200 : -200) + (player.velocity.X * 20), player.Center.Y - 70);
                                    AITimer = 100;
                                }
                            }
                            else
                            {
                                if (AITimer == 100)
                                {
                                    NPC.velocity.X = 6f * NPC.spriteDirection;
                                }
                                if (AITimer >= 100 && AITimer < 200)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.alpha -= alphaTimer;
                                    NPC.velocity *= 0.96f;
                                }
                                if (NPC.alpha <= 0 && AITimer < 200)
                                {
                                    AITimer = 200;
                                    NPC.frameCounter = 0;
                                    NPC.frame.Y = 0;
                                }
                                if (AITimer >= 200 && NPC.frame.Y >= 4 * 142 && NPC.frame.Y <= 6 * 142)
                                {
                                    foreach (NPC target in Main.npc.Take(Main.maxNPCs))
                                    {
                                        if (!target.active || target.whoAmI == NPC.whoAmI || (!target.friendly &&
                                            !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
                                            continue;

                                        if (target.immune[NPC.whoAmI] > 0 || !target.Hitbox.Intersects(SlashHitbox))
                                            continue;

                                        target.immune[NPC.whoAmI] = 30;
                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamageNPC(target, NPC.damage, 3, hitDirection, NPC);
                                        target.AddBuff(BuffID.Bleeding, 600);
                                    }
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(SlashHitbox))
                                            continue;

                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamagePlayer(target, NPC.damage, 3, hitDirection, NPC);
                                        target.AddBuff(BuffID.Bleeding, 600);
                                    }
                                }
                                if (AITimer >= 235)
                                {
                                    NPC.velocity *= 0f;
                                    if (TimerRand >= (Main.expertMode ? 2 : 1) + (Unveiled ? 1 : 0))
                                    {
                                        Reap = false;
                                        TimerRand = 0;
                                        AITimer = 0;
                                        AIState = ActionState.Idle;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        TimerRand++;
                                        AITimer = 30;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Blood Wave
                        case 1:
                            NPC.LookAtEntity(player);

                            NPC.velocity *= 0.96f;

                            if (++AITimer == 30)
                                NPC.velocity = player.Center.DirectionTo(NPC.Center) * 6;

                            if (AITimer == 60)
                            {
                                BaseAI.DamageNPC(NPC, 50, 0, player, false, true);
                                for (int i = 0; i < 6; i++)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperBloodWave>(), NPC.damage,
                                        RedeHelper.PolarVector(Main.rand.NextFloat(8, 16), (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.3f, 0.3f)),
                                        false, SoundID.NPCDeath19, "", NPC.whoAmI);
                                }
                                for (int i = 0; i < 30; i++)
                                {
                                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, Scale: 3);
                                    Main.dust[dustIndex].velocity *= 5f;
                                }
                            }
                            if (AITimer >= 90)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Shadow Bolts
                        case 2:
                            NPC.LookAtEntity(player);

                            if (AITimer++ == 0)
                                speed = 6;
                            NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 400 * 400)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 6 && NPC.DistanceSQ(player.Center) <= 400 * 400)
                                speed *= 0.96f;

                            if (AITimer >= 60 && AITimer % (Unveiled ? 20 : 25) == 0)
                            {
                                Vector2 pos = NPC.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(TimerRand)) * 60;
                                NPC.Shoot(pos, ModContent.ProjectileType<ShadowBolt>(), NPC.damage,
                                       RedeHelper.PolarVector(Main.expertMode ? 4 : 3, (player.Center - NPC.Center).ToRotation()), false, SoundID.Item20);

                                TimerRand += 45;
                            }
                            if (TimerRand >= 360)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Soul Charge
                        case 3:
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (AITimer == 5)
                                {
                                    NPC.LookAtEntity(player);
                                    SoundEngine.PlaySound(SoundID.Zombie, (int)NPC.position.X, (int)NPC.position.Y, 83, 1, 0.3f);
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -6f * NPC.spriteDirection;
                                }
                                if (AITimer >= 5)
                                {
                                    NPC.alpha += 20;
                                    NPC.velocity *= 0.9f;
                                }
                                if (NPC.alpha >= 255)
                                {
                                    NPC.velocity *= 0f;
                                    NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y - 70);
                                    AITimer = 100;
                                }
                            }
                            else
                            {
                                if (AITimer == 100)
                                    NPC.velocity.X = 6f * NPC.spriteDirection;

                                if (AITimer >= 100 && AITimer < 200)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.alpha -= 20;
                                    NPC.velocity *= 0.9f;
                                }
                                if (NPC.alpha <= 0 && AITimer < 200)
                                    AITimer = 200;

                                if (AITimer < (Unveiled ? 260 : 280))
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.MoveToVector2(new Vector2(player.Center.X - 160 * NPC.spriteDirection, player.Center.Y - 70), 4);
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Dust dust2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit, 1);
                                        dust2.velocity = -NPC.DirectionTo(dust2.position);
                                        dust2.noGravity = true;
                                    }
                                    origin = player.Center;
                                }
                                if (AITimer >= (Unveiled ? 260 : 280) && AITimer < 320)
                                {
                                    SoulCharging = true;
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -0.1f * NPC.spriteDirection;
                                    player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 3;

                                    if (AITimer % 2 == 0)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperSoulCharge>(), (int)(NPC.damage * 1.4f), RedeHelper.PolarVector(Main.rand.NextFloat(14, 16), (origin - NPC.Center).ToRotation()), false, SoundID.NPCDeath52.WithVolume(0.5f));
                                    }
                                }
                                if (AITimer >= 320)
                                    NPC.velocity *= 0.98f;
                            }
                            if (AITimer >= 360)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Dread Coil
                        case 4:
                            if (Unveiled)
                            {
                                NPC.LookAtEntity(player);

                                if (AITimer++ == 0)
                                    speed = 6;
                                NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                                MoveClamp();
                                if (NPC.DistanceSQ(player.Center) > 400 * 400)
                                    speed *= 1.03f;
                                else if (NPC.velocity.Length() > 6 && NPC.DistanceSQ(player.Center) <= 400 * 400)
                                    speed *= 0.96f;
                                if (AITimer >= 30 && AITimer % 30 == 0)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<KeeperDreadCoil>(),
                                        NPC.damage, RedeHelper.PolarVector(7, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.08f, 0.08f)),
                                        false, SoundID.Item20);
                                }
                                if (AITimer >= 130)
                                {
                                    TimerRand = 0;
                                    AITimer = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                TimerRand = 0;
                                AITimer = 60;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Rupture
                        case 5:
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Unveiled:
                    NPC.alpha = 0;
                    player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 3;

                    Unveiled = true;
                    Reap = false;

                    if (AITimer++ == 1)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Shriek").WithVolume(0.4f), NPC.position);

                        NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<VeilFX>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0));

                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }

                    if (AITimer >= 220)
                    {
                        AITimer = 0;
                        if (sorrowfulEssence)
                            AIState = ActionState.SkullDiggerSummon;
                        else
                        {
                            NPC.dontTakeDamage = false;
                            AIState = ActionState.Idle;

                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.SkullDiggerSummon:
                    player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    Reap = false;

                    if (AITimer++ == 0)
                    {
                        RedeHelper.SpawnNPC((int)(NPC.Center.X + 120 * NPC.spriteDirection), (int)(NPC.Center.Y + 180), ModContent.NPCType<SkullDigger>(), ai3: NPC.whoAmI);

                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }

                    if (AITimer >= (RedeConfigClient.Instance.NoLoreElements ? 200 : 660))
                    {
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Teddy:
                    player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;
                    player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    Unveiled = true;

                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                        NPC.alpha = 0;
                        NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<VeilFX>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                    }

                    if (AITimer == 60)
                        Main.NewText("The Keeper noticed the abandoned teddy you're holding...", Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                    if (AITimer == 120)
                        TimerRand = 1;
                    if (AITimer == 320)
                        Main.NewText("She starts to remember something...", Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                    if (AITimer == 400)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        TimerRand = 2;
                    }
                    if (AITimer == 540)
                        Main.NewText("Pain, Anger, Sadness. All those feelings were washed away...", Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                    if (AITimer == 750)
                        Main.NewText("She only feels... at peace...", Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                    if (AITimer == 840)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        NPC.frame.X = 0;
                        TimerRand = 3;
                    }
                    if (AITimer >= 840)
                    {
                        NPC.alpha++;
                        if (Main.rand.NextBool(5))
                            Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.PurificationPowder);
                    }
                    if (AITimer == 900)
                        CombatText.NewText(NPC.getRect(), Color.GhostWhite, "Thank...", true, false);
                    if (AITimer == 960)
                        CombatText.NewText(NPC.getRect(), Color.GhostWhite, "You...", true, false);
                    if (AITimer >= 960)
                    {
                        for (int k = 0; k < 1; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 100);
                            vector.Y = (float)(Math.Cos(angle) * 100);
                            Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, ModContent.DustType<VoidFlame>(), 0f, 0f, 100, default, 3f)];
                            dust2.noGravity = true;
                            dust2.velocity = -NPC.DirectionTo(dust2.position) * 10f;
                        }
                    }
                    if (NPC.alpha >= 255)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.PurificationPowder, 0f, 0f, 100, default, 2.5f);
                            Main.dust[dustIndex].velocity *= 2.6f;
                            int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), 0f, 0f, 100, default, 3f);
                            Main.dust[dustIndex2].velocity *= 2.6f;
                        }
                        Main.NewText("The Keeper's Spirit fades away... ?", Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                        Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<KeepersCirclet>());
                        Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<KeeperTrophy>());
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperSoul>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        if (!RedeBossDowned.keeperSaved)
                        {
                            RedeWorld.alignment += 2;
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                Player player2 = Main.player[p];
                                if (!player2.active)
                                    continue;

                                CombatText.NewText(player2.getRect(), Color.Gold, "+2", true, false);

                                if (!player2.HasItem(ModContent.ItemType<AlignmentTeller>()))
                                    continue;

                                if (!Main.dedServ)
                                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You've redeemed yourself, Octavia may rest in undisturbed peac-", 180, 30, 0, Color.DarkGoldenrod);

                            }
                        }
                        NPC.netUpdate = true;
                        NPC.SetEventFlagCleared(ref RedeBossDowned.keeperSaved, -1);

                        NPC.active = false;
                    }
                    break;
                case ActionState.Death:
                    if (!NPC.AnyNPCs(ModContent.NPCType<SkullDigger>()))
                    {
                        player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;
                        player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                    }
                    player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 3;
                    NPC.velocity *= 0;
                    Reap = false;

                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Shriek").WithVolume(0.4f), NPC.position);

                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                        NPC.alpha = 0;
                    }

                    NPC.alpha++;

                    if (NPC.alpha > 150)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, Scale: 3);
                            Main.dust[dustIndex].velocity *= 5f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, Scale: 3);
                            Main.dust[dustIndex].velocity *= 2f;
                        }
                    }

                    if (NPC.alpha >= 255)
                    {
                        NPC.dontTakeDamage = false;
                        player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
            }
        }

        public void MoveClamp()
        {
            Player player = Main.player[NPC.target];
            int xFar = 240;
            if (NPC.dontTakeDamage)
                xFar = 600;
            if (NPC.Center.X < player.Center.X)
            {
                if (move < player.Center.X - xFar)
                {
                    move = player.Center.X - xFar;
                }
                else if (move > player.Center.X - 120)
                {
                    move = player.Center.X - 120;
                }
            }
            else
            {
                if (move > player.Center.X + xFar)
                {
                    move = player.Center.X + xFar;
                }
                else if (move < player.Center.X + 120)
                {
                    move = player.Center.X + 120;
                }
            }
        }

        public override bool CheckActive()
        {
            return AIState != ActionState.Death && AIState != ActionState.SkullDiggerSummon;
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death && AITimer > 0)
                return true;
            else
            {
                NPC.dontTakeDamage = true;
                SoundEngine.PlaySound(SoundID.NPCDeath19, NPC.position);
                NPC.velocity *= 0;
                NPC.alpha = 0;
                NPC.life = 1;
                AITimer = 0;
                AIState = ActionState.Death;

                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return false;
            }
        }

        private int VeilFrameY;
        private int VeilCounter;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Player player = Main.player[NPC.target];

                for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                {
                    oldrot[k] = oldrot[k - 1];
                }
                oldrot[0] = NPC.rotation;

                if (++VeilCounter >= 5)
                {
                    VeilCounter = 0;
                    VeilFrameY++;
                    if (VeilFrameY > 5)
                        VeilFrameY = 0;
                }

                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 4;
                if (AIState is ActionState.Teddy)
                {
                    if (TimerRand < 3)
                        NPC.frame.X = (TimerRand == 2 ? 3 : 2) * NPC.frame.Width;

                    if (++NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        switch (TimerRand)
                        {
                            case 0:
                                if (NPC.frame.Y > 2 * frameHeight)
                                    NPC.frame.Y = 0 * frameHeight;
                                break;
                            case 1:
                                if (NPC.frame.Y > 8 * frameHeight)
                                    NPC.frame.Y = 6 * frameHeight;
                                break;
                            case 2:
                                if (NPC.frame.Y > 5 * frameHeight)
                                    NPC.frame.Y = 3 * frameHeight;
                                break;
                            case 3:
                                if (NPC.frame.Y > 9 * frameHeight)
                                    NPC.frame.Y = 8 * frameHeight;
                                break;
                        }
                    }
                    return;
                }
                if (AIState is ActionState.Attacks && ID == 0 && AITimer >= 200)
                {
                    NPC.frame.X = NPC.frame.Width;
                    if (++NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        NPC.velocity *= 0.8f;
                        if (NPC.frame.Y == 4 * frameHeight)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                            NPC.velocity.X = MathHelper.Clamp(Math.Abs((player.Center.X - NPC.Center.X) / 30), 30, 50) * NPC.spriteDirection;
                        }
                        if (NPC.frame.Y > 7 * frameHeight)
                            NPC.frame.Y = 0 * frameHeight;
                    }
                    return;
                }
                else
                    NPC.frame.X = 0;

                if (AIState is ActionState.Unveiled or ActionState.Death || SoulCharging)
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;

                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 8 * frameHeight)
                            NPC.frame.Y = 7 * frameHeight;
                    }
                    return;
                }
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 5 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D veilTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Keeper/VeilFX").Value;
            Texture2D closureTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Keeper/Keeper_Closure").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color angryColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.DarkSlateBlue, Color.DarkRed * 0.7f, Color.DarkSlateBlue);

            if (!NPC.IsABestiaryIconDummy && AIState != ActionState.Teddy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(SoulCharging ? Color.GhostWhite : (Unveiled ? angryColor : Color.DarkSlateBlue)) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            int reapShader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.VoidDye);
            if (Reap)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(reapShader, Main.player[Main.myPlayer], null);
            }
            if (AIState is ActionState.Teddy && TimerRand == 3)
                spriteBatch.Draw(closureTex, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            else
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            int height = veilTex.Height / 6;
            int y = height * VeilFrameY;
            Rectangle rect = new(0, y, veilTex.Width, height);
            Vector2 origin = new(veilTex.Width / 2f, height / 2f);
            Vector2 VeilPos = new(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37);
            if (!Unveiled && NPC.life > NPC.lifeMax / 2)
                Main.spriteBatch.Draw(veilTex, VeilPos - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, default);
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return NPC.GetBestiaryEntryColor();
            }
            return null;
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