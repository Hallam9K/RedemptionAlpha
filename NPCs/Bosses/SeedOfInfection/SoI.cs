using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Items.Usable;
using System.IO;
using Terraria.Audio;
using System;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Biomes;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Weapons.PreHM.Magic;
using Terraria.Localization;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    [AutoloadBossHead]
    public class SoI : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death,
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Seed of Infection");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 76;
            NPC.height = 76;
            NPC.damage = 36;
            NPC.defense = 10;
            NPC.lifeMax = 4000;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.value = Item.buyPrice(0, 4, 0, 0);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.netAlways = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossXeno1");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.velocity.Length() > 10;
        public override bool CanHitNPC(NPC target) => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.SoI"))
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SoIBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoITrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<SoIRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<CuddlyTeratoma>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<InfectedMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<XenoXyston>(), ModContent.ItemType<CystlingSummon>(), ModContent.ItemType<ContagionSpreader>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<XenomiteShard>(), 1, 12, 22));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ToxicGrenade>(), 1, 20, 30));

            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedSeed && !RedeHelper.TBotActive())
            {
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<AdamPortal>(), 0, Vector2.Zero, NPC.target);
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSeed, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ID = reader.ReadInt32();
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

                if (ID == 5 && NPC.CountNPCS(ModContent.NPCType<SeedGrowth>()) > 0)
                    continue;

                attempts++;
            }
        }

        public List<int> CopyList = null;
        public List<int> AttackList = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private float move;
        private float speed;
        private Vector2 target;
        private bool FreakOut;
        private int repeat;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            if (NPC.DespawnHandler())
                return;

            if (AIState != ActionState.Death)
                NPC.LookAtEntity(player, true);

            if (AIState != ActionState.Death && ID != 6)
                NPC.rotation += 0.09f;

            if (NPC.velocity.Length() >= 10)
                trailOpacity += 0.3f;
            else
                trailOpacity -= 0.04f;
            trailOpacity = MathHelper.Clamp(trailOpacity, 0, 1);
            Vector2 HighPos = new(player.Center.X + 240 * NPC.spriteDirection, player.Center.Y - 50);
            Vector2 FarPos = new(player.Center.X + 320 * NPC.spriteDirection, player.Center.Y - 25);
            Vector2 TopPos = new(player.Center.X, player.Center.Y - 120);

            switch (AIState)
            {
                case ActionState.Begin:
                    switch (TimerRand)
                    {
                        case 0:
                            NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 160 : player.Center.X + 160, player.Center.Y - 90);
                            for (int i = 0; i < 10; i++)
                            {
                                int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenFairy, 0f, 0f, 100, default, 3.5f);
                                Main.dust[dustIndex].velocity *= 2.9f;
                            }
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<StrangePortal>(), 0, Vector2.Zero);

                            TimerRand = 1;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            if (AITimer++ > 100)
                                NPC.alpha -= 4;
                            if (AITimer >= 180)
                            {
                                if (!Main.dedServ)
                                    RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.SoI.Name"), 60, 90, 0.8f, 0, Color.ForestGreen, Language.GetTextValue("Mods.Redemption.TitleCard.SoI.Modifier"));
                                NPC.dontTakeDamage = false;
                                NPC.alpha = 0;
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                            }
                            break;
                    }
                    break;
                case ActionState.Idle:
                    if (AITimer++ == 0)
                    {
                        move = NPC.Center.X;
                        speed = 5;
                    }
                    NPC.Move(new Vector2(move, player.Center.Y), speed, 10, false);
                    MoveClamp();
                    if (NPC.DistanceSQ(player.Center) > 600 * 600)
                        speed *= 1.02f;
                    else if (NPC.DistanceSQ(player.Center) <= 600 * 600)
                        speed *= 0.96f;

                    if (AITimer >= 30)
                    {
                        AttackChoice();
                        AITimer = 0;
                        AIState = ActionState.Attacks;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Default
                        case 0:
                            if (AITimer++ == 0 || AITimer == 60)
                            {
                                for (int k = 0; k < 20; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.GreenFairy, 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -NPC.DirectionTo(dust2.position) * 10;
                                }
                            }
                            if (AITimer < 40)
                            {
                                TimerRand += 0.1f;
                                NPC.velocity = -NPC.DirectionTo(target) * TimerRand;
                            }
                            if (AITimer <= 30)
                                target = player.Center;

                            if (AITimer == 40)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
                                DustHelper.DrawCircle(NPC.Center, DustID.GreenFairy, 6, 2, 2, 1, 3, nogravity: true);
                                FreakOut = true;
                                TimerRand = 0;
                                NPC.velocity = NPC.DirectionTo(target) * 30;
                            }
                            if (AITimer > 60 && AITimer < 100)
                            {
                                FreakOut = false;
                                TimerRand += 0.1f;
                                NPC.velocity = -NPC.DirectionTo(target) * TimerRand;
                            }
                            if (AITimer <= 90)
                                target = player.Center;

                            if (AITimer == 100)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
                                DustHelper.DrawCircle(NPC.Center, DustID.GreenFairy, 6, 2, 2, 1, 3, nogravity: true);
                                FreakOut = true;
                                TimerRand = 0;
                                NPC.velocity = NPC.DirectionTo(target) * 30;
                            }
                            if (AITimer >= 120)
                            {
                                FreakOut = false;
                                AITimer = 20;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Shard Shot
                        case 1:
                            NPC.Move(HighPos, 5, 40);
                            if (++AITimer >= 80 && AITimer % (NPC.life <= (int)(NPC.lifeMax * 0.75f) ? (NPC.life <= (int)(NPC.lifeMax * 0.25f) ? 20 : 40) : 80) == 0)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    int rot = 20 * i;
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_ShardShot>(), NPC.damage, RedeHelper.PolarVector(8,
                                        (player.Center - NPC.Center).ToRotation() + MathHelper.ToRadians(rot - 20)), SoundID.Item42);
                                }
                                if (NPC.life < NPC.lifeMax / 2)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        int rot = 80 * i;
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_ShardShot>(), NPC.damage, RedeHelper.PolarVector(8,
                                            (player.Center - NPC.Center).ToRotation() + MathHelper.ToRadians(rot - 40)), SoundID.Item42);
                                    }
                                }
                            }
                            if (AITimer >= 120)
                            {
                                FreakOut = false;
                                AITimer = 20;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Irradiated Rain
                        case 2:
                            if (++AITimer < 70)
                                NPC.MoveToVector2(TopPos, 8);
                            else
                                NPC.velocity *= 0.9f;

                            if (AITimer == 90)
                            {
                                for (int i = 0; i < (NPC.life < NPC.lifeMax / 2 ? 14 : 8); i++)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_ShardShot>(), NPC.damage,
                                        new Vector2(Main.rand.Next(-8, 9), Main.rand.Next(-14, -4)), SoundID.Item42, 1);
                                }
                            }
                            if (AITimer >= 150)
                            {
                                AITimer = 20;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Toxic Sludge
                        case 3:
                            if (NPC.life < NPC.lifeMax / 2)
                            {
                                NPC.Move(FarPos, 9, 40);
                                if (++AITimer >= 100 && AITimer % 50 == 0 && AITimer <= (Main.expertMode ? 250 : 200))
                                {
                                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_ToxicSludge>(), NPC.damage, RedeHelper.PolarVector(12, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), SoundID.Item60);
                                    }
                                    NPC.velocity += player.Center.DirectionTo(NPC.Center) * 4;
                                }
                                if (AITimer >= 300)
                                {
                                    AITimer = 20;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AITimer = 60;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Xenomite Shot
                        case 4:
                            if (NPC.life < NPC.lifeMax / 2)
                            {
                                if (++AITimer < 80)
                                {
                                    TimerRand += 0.05f;
                                    NPC.velocity = -NPC.DirectionTo(player.Center) * TimerRand;
                                }

                                if (AITimer >= 80 && AITimer % 40 == 0 && AITimer <= 200)
                                    Dash(NPC.life <= (int)(NPC.lifeMax * 0.25f) ? 30 : 25);

                                if ((AITimer > 90 && AITimer < 120) || (AITimer > 130 && AITimer < 160) || (AITimer > 170 && AITimer < 200))
                                {
                                    FreakOut = false;
                                    TimerRand += 0.01f;
                                    NPC.velocity = -NPC.DirectionTo(player.Center) * TimerRand;
                                }

                                if (AITimer >= 210)
                                    NPC.velocity *= 0.7f;

                                if (AITimer >= 20 && AITimer <= 210)
                                {
                                    if (Main.rand.NextBool(10))
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_XenomiteShot>(), NPC.damage,
                                            new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-14, -4)), SoundID.Item17);
                                }

                                if (AITimer >= 250)
                                {
                                    FreakOut = false;
                                    AITimer = 20;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AITimer = 60;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Hive Growths
                        case 5:
                            NPC.rotation += 0.03f;
                            NPC.velocity *= 0.9f;
                            if (++AITimer >= 30 && AITimer % (NPC.life < NPC.lifeMax / 2 ? 10 : 15) == 0 && AITimer <= 90)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SeedGrowth>(), NPC.whoAmI, Main.rand.Next(100, 180));
                            }
                            if (AITimer >= 130)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Scatter Splatter
                        case 6:
                            if (NPC.life < NPC.lifeMax / 4)
                            {
                                FreakOut = true;
                                NPC.rotation += TimerRand / 40;
                                NPC.velocity *= 0.9f;
                                if (++AITimer >= 30 && AITimer <= 200)
                                {
                                    if (Main.rand.NextBool(10))
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_ToxicSludge>(), NPC.damage, new Vector2(Main.rand.Next(-8, 9), Main.rand.Next(-14, 15)), SoundID.Item17);
                                    }
                                }
                                if (AITimer < 160)
                                    TimerRand += 0.08f;
                                else
                                    TimerRand -= 0.08f;

                                if (AITimer >= 240)
                                {
                                    FreakOut = false;
                                    AITimer = 0;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AITimer = 60;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Xenomite Beam
                        case 7:
                            if (NPC.life <= (int)(NPC.lifeMax * 0.75f))
                            {
                                if (AITimer < (NPC.life < NPC.lifeMax / 2 ? 100 : 70))
                                    NPC.Move(TopPos, 9, 20);
                                else
                                    NPC.velocity *= 0.8f;

                                if (++AITimer == 70)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<SeedLaser>(), (int)(NPC.damage * 1.1f), new Vector2(0, 10), SoundID.Item103, ai0: NPC.whoAmI);

                                if (AITimer >= 100)
                                {
                                    AITimer = 20;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AITimer = 60;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Splitting Shards
                        case 8:
                            if (AITimer++ <= 80)
                                NPC.Move(HighPos, 10, 40);
                            else
                                NPC.velocity *= .96f;

                            if (AITimer == 120)
                                target = player.Center;
                            if (AITimer >= 120 && AITimer < 123)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_SplitShard>(), NPC.damage,
                                    RedeHelper.PolarVector(12, (target - NPC.Center).ToRotation()
                                    + TimerRand - MathHelper.ToRadians(15)), SoundID.Item42);

                                NPC.velocity += target.DirectionTo(NPC.Center) * 1;
                                TimerRand += MathHelper.ToRadians(15);
                            }
                            if (NPC.life < NPC.lifeMax / 2)
                            {
                                if (AITimer == 123)
                                {
                                    TimerRand = 0;
                                    target = player.Center;
                                }
                                if (AITimer >= 124 && AITimer < 129)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_SplitShard>(), NPC.damage,
                                        RedeHelper.PolarVector(8, (target - NPC.Center).ToRotation()
                                        + TimerRand + MathHelper.ToRadians(40)), SoundID.Item42);

                                    NPC.velocity += target.DirectionTo(NPC.Center) * 1;
                                    TimerRand -= MathHelper.ToRadians(20);
                                }
                            }
                            if (AITimer >= 180)
                            {
                                FreakOut = false;
                                AITimer = 20;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Irradiated Rain II
                        case 9:
                            if (NPC.life <= NPC.lifeMax / 2)
                            {
                                if (AITimer++ == 0)
                                    TimerRand = player.RightOfDir(NPC);
                                if (AITimer < 80)
                                    NPC.Move(player.Center + new Vector2(600 * -TimerRand, -Main.rand.Next(400, 451)), 12, 30);
                                else
                                {
                                    NPC.Move(player.Center + new Vector2(600 * TimerRand, -Main.rand.Next(400, 451)), 12, 40);
                                    if (AITimer % (NPC.life <= NPC.lifeMax / 4 ? 3 : 4) == 0 && Main.rand.NextBool())
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoI_ShardShot>(), NPC.damage, new Vector2(0, Main.rand.Next(6, 10)), SoundID.Item42);

                                    }
                                }
                                if (AITimer >= 300 || (TimerRand == -1 ? NPC.Center.X <= player.Center.X - 600 : NPC.Center.X >= player.Center.X + 600))
                                {
                                    if (repeat <= 2 && !Main.rand.NextBool(4))
                                    {
                                        TimerRand *= -1;
                                        AITimer = 60;
                                        repeat++;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        AITimer = 20;
                                        TimerRand = 0;
                                        AIState = ActionState.Idle;
                                        repeat = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                AITimer = 60;
                                TimerRand = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Death:
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }

                    FreakOut = true;
                    TimerRand += 0.1f;
                    NPC.rotation += TimerRand / 40;

                    if (++AITimer >= 240)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.position);
                        for (int i = -32; i <= 32; i++)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<InfectionDust>(), 0, 10 * Vector2.UnitX.RotatedBy(Math.PI / 32 * i));
                        }

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int i = 0; i < 7; i++)
                                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SoIGore" + (i + 1)).Type, 1);
                        }

                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenFairy, 0f, 0f, 100, default, 3.5f);
                            Main.dust[dust].velocity *= 3f;
                            Main.dust[dust].noGravity = true;
                        }

                        NPC.dontTakeDamage = false;
                        player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
            }
            NPC.rotation += NPC.velocity.X * 0.01f;
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;
        }

        public void Dash(int speed)
        {
            Player player = Main.player[NPC.target];
            SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
            DustHelper.DrawCircle(NPC.Center, DustID.GreenFairy, 6, 2, 2, 1, 3, nogravity: true);
            FreakOut = true;
            TimerRand = 0;
            NPC.velocity = NPC.DirectionTo(player.Center) * speed;
        }

        public void MoveClamp()
        {
            Player player = Main.player[NPC.target];
            if (NPC.Center.X < player.Center.X)
            {
                if (move < player.Center.X - 240)
                {
                    move = player.Center.X - 240;
                }
                else if (move > player.Center.X - 120)
                {
                    move = player.Center.X - 120;
                }
            }
            else
            {
                if (move > player.Center.X + 240)
                {
                    move = player.Center.X + 240;
                }
                else if (move < player.Center.X + 120)
                {
                    move = player.Center.X + 120;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;

            NPC.frame.X = 0;
            if (FreakOut)
                NPC.frame.X = NPC.frame.Width;

            if (++NPC.frameCounter >= (FreakOut ? 5 : 10))
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (!FreakOut && NPC.frame.Y == 4 * frameHeight)
                {
                    if (!Main.rand.NextBool(4))
                        NPC.frame.Y = 0;
                }
                if (NPC.frame.Y > 5 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
                return true;
            else
            {
                NPC.dontTakeDamage = true;
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

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return NPC.GetBestiaryEntryColor();
            }
            return null;
        }
        private float trailOpacity;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D EyeTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/SeedOfInfection/SoI_Eye").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightGreen) * 0.5f * trailOpacity, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!FreakOut)
                spriteBatch.Draw(EyeTex, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), 0, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
