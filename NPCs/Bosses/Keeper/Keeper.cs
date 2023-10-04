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
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.Dusts;
using Redemption.NPCs.Friendly;
using Redemption.BaseExtension;
using ReLogic.Content;
using Terraria.Localization;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Biomes;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;

namespace Redemption.NPCs.Bosses.Keeper
{
    [AutoloadBossHead]
    public class Keeper : ModNPC
    {
        private static Asset<Texture2D> glow;
        private static Asset<Texture2D> veilTex;
        private static Asset<Texture2D> closureTex;
        public override void Unload()
        {
            glow = null;
            veilTex = null;
            closureTex = null;
        }
        public static int secondStageHeadSlot = -1;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Keeper/Keeper_Glow");
            veilTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Keeper/VeilFX");
            closureTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Keeper/Keeper_Closure");

            string texture = "Redemption/NPCs/Bosses/Keeper/Keeper_Head_Boss_Unveiled";
            secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1);
        }

        public override void BossHeadSlot(ref int index)
        {
            if (NPC.type == ModContent.NPCType<KeeperSpirit>())
                return;
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
            // DisplayName.SetDefault("The Keeper");
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 36),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCShadow[Type] = true;
            ElementID.NPCBlood[Type] = true;
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
        }

        public override void HitEffect(NPC.HitInfo hit)
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
        public override bool CanHitNPC(NPC target) => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Keeper"))
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<KeeperBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KeeperTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<KeeperRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<OcciesCollar>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<KeepersVeil>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<SoulScepter>(), ModContent.ItemType<KeepersClaw>(), ModContent.ItemType<FanOShivs>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GrimShard>(), 1, 2, 4));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), -0.6f);

            if (!RedeBossDowned.downedKeeper)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<SorrowfulEssence>());

                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gray, "+0", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.WeddingRing"), 240, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedKeeper, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.Write(Unveiled);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ID = reader.ReadInt32();
            Unveiled = reader.ReadBoolean();
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
            if (AIState == ActionState.Death || AIState == ActionState.SkullDiggerSummon)
                NPC.DiscourageDespawn(120);
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
                if (player2.HasItem(ModContent.ItemType<AbandonedTeddy>()))
                    teddy = true;
                if (player2.HasItem(ModContent.ItemType<SorrowfulEssence>()))
                    sorrowfulEssence = true;
            }
            if (NPC.DespawnHandler(1))
                return;

            if (AIState != ActionState.Death && AIState != ActionState.Unveiled && AIState != ActionState.Attacks)
                NPC.LookAtEntity(player);
            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                        {
                            if (NPC.type != ModContent.NPCType<KeeperSpirit>())
                                RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Keeper.Name"), 60, 90, 0.8f, 0, Color.MediumPurple, Language.GetTextValue("Mods.Redemption.TitleCard.Keeper.Modifier"));
                        }

                        NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 160 : player.Center.X + 160, player.Center.Y - 90);
                        NPC.netUpdate = true;
                    }
                    NPC.alpha -= 2;
                    if (NPC.alpha <= 0)
                    {
                        if (NPC.type == ModContent.NPCType<KeeperSpirit>())
                        {
                            AIState = ActionState.Idle;
                        }
                        else
                        {
                            if (teddy)
                            {
                                AIState = ActionState.Teddy;
                                int teddyItem = Main.LocalPlayer.FindItem(ModContent.ItemType<AbandonedTeddy>());
                                if (teddyItem >= 0)
                                {
                                    Main.LocalPlayer.inventory[teddyItem].stack--;
                                    if (Main.LocalPlayer.inventory[teddyItem].stack <= 0)
                                        Main.LocalPlayer.inventory[teddyItem] = new Item();
                                }
                            }
                            else
                                AIState = ActionState.Idle;
                        }

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
                        if (NPC.type == ModContent.NPCType<KeeperSpirit>())
                        {
                            Unveiled = true;
                            NPC.netUpdate = true;
                            break;
                        }
                        NPC.velocity *= 0;
                        AITimer = 0;
                        AIState = ActionState.Unveiled;
                        NPC.netUpdate = true;
                        break;
                    }
                    if (NPC.dontTakeDamage ? AITimer == -1 : AITimer > 60)
                    {
                        if (NPC.type != ModContent.NPCType<KeeperSpirit>() && teddy)
                        {
                            AIState = ActionState.Teddy;
                            int teddyItem = Main.LocalPlayer.FindItem(ModContent.ItemType<AbandonedTeddy>());
                            if (teddyItem >= 0)
                            {
                                Main.LocalPlayer.inventory[teddyItem].stack--;
                                if (Main.LocalPlayer.inventory[teddyItem].stack <= 0)
                                    Main.LocalPlayer.inventory[teddyItem] = new Item();
                            }
                            AITimer = 0;
                        }
                        else
                        {
                            NPC.dontTakeDamage = false;
                            AttackChoice();
                            AITimer = 0;
                            AIState = ActionState.Attacks;
                            NPC.netUpdate = true;

                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    break;
                case ActionState.Attacks:
                    if (!Unveiled && NPC.life < NPC.lifeMax / 2)
                    {
                        if (NPC.type == ModContent.NPCType<KeeperSpirit>())
                        {
                            Unveiled = true;
                            NPC.netUpdate = true;
                            break;
                        }
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
                                    SoundEngine.PlaySound(SoundID.Zombie83 with { Pitch = .3f }, NPC.position);
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
                                if (AITimer >= 200 && NPC.frame.Y >= 4 * 71 && NPC.frame.Y <= 6 * 71)
                                {
                                    for (int i = 0; i < Main.maxNPCs; i++)
                                    {
                                        NPC target = Main.npc[i];
                                        if (!target.active || target.whoAmI == NPC.whoAmI || (!target.friendly &&
                                            !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
                                            continue;

                                        if (target.immune[NPC.whoAmI] > 0 || !target.Hitbox.Intersects(SlashHitbox))
                                            continue;

                                        target.immune[NPC.whoAmI] = 30;
                                        int hitDirection = target.RightOfDir(NPC);
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

                                        int hitDirection = target.RightOfDir(NPC);
                                        BaseAI.DamagePlayer(target, NPC.damage, 3, hitDirection, NPC);
                                        target.AddBuff(BuffID.Bleeding, 600);
                                    }
                                }
                                if (AITimer >= 235)
                                {
                                    NPC.frameCounter = 0;
                                    NPC.frame.Y = 0;
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
                                        RedeHelper.PolarVector(Main.rand.NextFloat(8, 16), (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.3f, 0.3f)), SoundID.NPCDeath19, NPC.whoAmI);
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
                                NPC.Shoot(pos, ModContent.ProjectileType<ShadowBolt>(), NPC.damage, RedeHelper.PolarVector(Main.expertMode ? 0.5f : 0.3f, (player.Center - pos).ToRotation()), SoundID.Item20);

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
                                    SoundEngine.PlaySound(SoundID.Zombie83 with { Pitch = .3f }, NPC.position);
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
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 150);
                                    vector.Y = (float)(Math.Cos(angle) * 150);
                                    Dust dust = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.DungeonSpirit, newColor: new Color(255, 255, 255, 0), Scale: 1f)];
                                    dust.noGravity = true;
                                    dust.velocity = dust.position.DirectionTo(NPC.Center) * 3f;
                                    origin = player.Center;
                                }
                                if (AITimer >= (Unveiled ? 260 : 280) && AITimer < 320)
                                {
                                    SoulCharging = true;
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -0.1f * NPC.spriteDirection;
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                    player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(player.RedemptionScreen().ScreenShakeIntensity, 3);

                                    if (AITimer % 2 == 0)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperSoulCharge>(), (int)(NPC.damage * 1.4f), RedeHelper.PolarVector(Main.rand.NextFloat(14, 16), (origin - NPC.Center).ToRotation()), SoundID.NPCDeath52 with { Volume = .5f });
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
                                    NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<KeeperDreadCoil>(), NPC.damage, RedeHelper.PolarVector(7, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.08f, 0.08f)), SoundID.Item20);
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
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.None, 1200, 2400, 1200);
                    player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(player.RedemptionScreen().ScreenShakeIntensity, 3);

                    Unveiled = true;
                    Reap = false;

                    if (AITimer++ == 1)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Shriek, NPC.position);

                        NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<VeilFX>(), 0, Vector2.Zero);

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
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Low, 1200, 2400, 1200);
                    Reap = false;

                    if (AITimer++ == 0)
                    {
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + 120 * NPC.spriteDirection), (int)(NPC.Center.Y + 180), ModContent.NPCType<SkullDigger>(), ai3: NPC.whoAmI);

                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }

                    if (AITimer >= 900)
                    {
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Teddy:
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.High, 0, 0, 0);
                    Unveiled = true;
                    NPC.velocity *= .94f;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                        NPC.alpha = 0;
                        NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<VeilFX>(), 0, Vector2.Zero);
                    }

                    if (AITimer == 60)
                        Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Keeper2"), Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                    if (AITimer == 120)
                        TimerRand = 1;
                    if (AITimer == 400)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        TimerRand = 2;
                    }
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
                        CombatText.NewText(NPC.getRect(), Color.GhostWhite, Language.GetTextValue("Mods.Redemption.Cutscene.Keeper.1"), true, false);
                    if (AITimer == 960)
                        CombatText.NewText(NPC.getRect(), Color.GhostWhite, Language.GetTextValue("Mods.Redemption.Cutscene.Keeper.2"), true, false);
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
                        Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Keeper3"), Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                        Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<KeepersCirclet>());
                        Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<KeeperTrophy>());
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperSoul>(), 0, Vector2.Zero);
                        if (!RedeBossDowned.keeperSaved)
                        {
                            RedeWorld.alignment += 3;
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                Player player2 = Main.player[p];
                                if (!player2.active)
                                    continue;

                                CombatText.NewText(player2.getRect(), Color.Gold, "+3", true, false);

                                if (!RedeWorld.alignmentGiven)
                                    continue;

                                if (!Main.dedServ)
                                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.KeeperSave"), 180, 30, 0, Color.DarkGoldenrod);
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
                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.None, 1200, 2400, 1200);
                    }
                    player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(player.RedemptionScreen().ScreenShakeIntensity, 3);
                    NPC.velocity *= 0;
                    Reap = false;

                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Shriek, NPC.position);

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
        public override void PostAI()
        {
            CustomFrames(71);
        }
        public void CustomFrames(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

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
                        Player player = Main.player[NPC.target];
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
        }
        private int VeilFrameY;
        private int VeilCounter;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 4;

            if (++VeilCounter >= 5)
            {
                VeilCounter = 0;
                VeilFrameY++;
                if (VeilFrameY > 5)
                    VeilFrameY = 0;
            }
            if (AIState is ActionState.Teddy)
                return;
            if (AIState is ActionState.Attacks && ID == 0 && AITimer >= 200)
                return;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color angryColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.DarkSlateBlue, Color.DarkRed * 0.7f, Color.DarkSlateBlue);

            if (!NPC.IsABestiaryIconDummy && AIState != ActionState.Teddy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(SoulCharging ? Color.GhostWhite : (Unveiled ? angryColor : Color.DarkSlateBlue)) * 0.5f, oldrot[i], NPC.frame.Size() / 2, (NPC.scale * 2) + 0.1f, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }

            int reapShader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.VoidDye);
            if (Reap)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(reapShader, Main.LocalPlayer, null);
            }
            if (AIState is ActionState.Teddy && TimerRand == 3)
                spriteBatch.Draw(closureTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);
            else
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);

                spriteBatch.Draw(glow.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);
            }

            int height = veilTex.Value.Height / 6;
            int y = height * VeilFrameY;
            Rectangle rect = new(0, y, veilTex.Value.Width, height);
            Vector2 origin = new(veilTex.Value.Width / 2f, height / 2f);
            Vector2 VeilPos = new(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37);
            if (!Unveiled && NPC.life > NPC.lifeMax / 2)
                Main.spriteBatch.Draw(veilTex.Value, VeilPos - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();
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
    }
}