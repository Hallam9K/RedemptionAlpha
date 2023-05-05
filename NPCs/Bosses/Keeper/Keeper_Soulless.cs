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
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Dusts;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Biomes;
using System.Threading;

namespace Redemption.NPCs.Bosses.Keeper
{
    [AutoloadBossHead]
    public class Keeper_Soulless : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death
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
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Bleeding,
                    BuffID.Poisoned,
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<LaceratedDebuff>(),
                    ModContent.BuffType<BlackenedHeartDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 30),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 110000;
            NPC.damage = 120;
            NPC.defense = 20;
            NPC.knockBackResist = 0f;
            NPC.width = 36;
            NPC.height = 98;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 8, 50, 0);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SilentCaverns");
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                    ParticleManager.NewParticle(NPC.Center, RedeHelper.Spread(4), new SoulParticle(), Color.White, 1);

                for (int i = 0; i < 100; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), Scale: 3);
                    Main.dust[dustIndex].velocity *= 4f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>());
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
                new FlavorTextBestiaryInfoElement("\"Find my beloved.\"")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<KeeperBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KeeperTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<KeeperRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<OcciesCollar>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<KeepersVeil>(), 7));

            //notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<SoulScepter>(), ModContent.ItemType<KeepersClaw>(), ModContent.ItemType<FanOShivs>(), ModContent.ItemType<KeepersKnife>()));
            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<SoulScepter>(), ModContent.ItemType<KeepersClaw>(), ModContent.ItemType<FanOShivs>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GrimShard>(), 1, 2, 4));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedKeeper)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "+0", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Here she plummeted, dragged into the woe of another. Her story is not over yet.", 300, 30, 0, Color.DarkGoldenrod);

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
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ID = reader.ReadInt32();
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

        private float move;
        private float speed = 4;
        private bool Reap;
        private Vector2 origin;
        private bool parried;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            if (AIState == ActionState.Death)
                NPC.DiscourageDespawn(120);
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 64 : NPC.Center.X + 26), (int)(NPC.Center.Y - 38), 38, 86);

            if (NPC.DespawnHandler(1))
                return;

            if (AIState != ActionState.Death && AIState != ActionState.Attacks)
                NPC.LookAtEntity(player);
            bool parryActive = false;
            switch (AIState)
            {
                case ActionState.Begin:
                    NPC.dontTakeDamage = false;
                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                    AIState = ActionState.Idle;
                    AITimer = 0;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Idle:
                    if (AITimer++ == 0)
                    {
                        move = NPC.Center.X;
                        speed = 4;
                    }
                    Reap = false;

                    NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                    MoveClamp();
                    if (NPC.DistanceSQ(player.Center) > 400 * 400)
                        speed *= 1.03f;

                    if (AITimer > 60)
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
                        #region Reaper Slash
                        case 0:
                            int alphaTimer = Main.expertMode ? 40 : 30;
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
                                    parried = false;
                                    AITimer = 200;
                                    NPC.frameCounter = 0;
                                    slashFrame = 0;
                                }
                                if (AITimer >= 200 && slashFrame >= 3 && slashFrame <= 4)
                                {
                                    if (slashFrame is 3)
                                        parryActive = true;
                                    RedeProjectile.SwordClashHostile(SlashHitbox, NPC, ref parried);
                                    if (!parried)
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
                                }
                                if (AITimer >= 235)
                                {
                                    NPC.velocity *= 0f;
                                    if (TimerRand >= 3)
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

                        #region Dusksong
                        case 1:
                            NPC.LookAtEntity(player);

                            NPC.velocity *= 0.96f;

                            if (++AITimer == 30)
                                NPC.velocity = player.Center.DirectionTo(NPC.Center) * 6;

                            if (AITimer == 60)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<Keeper_SoullessDusksong>(), (int)(NPC.damage * 1.1f),
                                    RedeHelper.PolarVector(8, (player.Center - NPC.Center).ToRotation()), true, SoundID.NPCDeath6, NPC.whoAmI);
                                for (int i = 0; i < 30; i++)
                                {
                                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.AncientLight, Scale: 3);
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
                                speed = 4;
                            NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 400 * 400)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 4 && NPC.DistanceSQ(player.Center) <= 400 * 400)
                                speed *= 0.96f;

                            if (AITimer >= 60 && AITimer % 15 == 0)
                            {
                                Vector2 pos = NPC.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(TimerRand)) * 60;
                                NPC.Shoot(pos, ModContent.ProjectileType<ShadowBolt_Soulless>(), NPC.damage,
                                       RedeHelper.PolarVector(0.5f, (player.Center - pos).ToRotation()), true, SoundID.Item20);

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
                                    NPC.alpha += 40;
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
                                    NPC.alpha -= 40;
                                    NPC.velocity *= 0.9f;
                                }
                                if (NPC.alpha <= 0 && AITimer < 200)
                                    AITimer = 200;

                                if (AITimer < 260)
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
                                if (AITimer >= 260 && AITimer < 320)
                                {
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -0.1f * NPC.spriteDirection;
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                    player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(player.RedemptionScreen().ScreenShakeIntensity, 3);

                                    if (AITimer % 2 == 0)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperSoulCharge>(), (int)(NPC.damage * 1.4f), RedeHelper.PolarVector(Main.rand.NextFloat(14, 16), (origin - NPC.Center).ToRotation()), true, SoundID.NPCDeath52 with { Volume = .5f });
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
                            NPC.LookAtEntity(player);

                            if (AITimer++ == 0)
                                speed = 4;
                            NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 400 * 400)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 4 && NPC.DistanceSQ(player.Center) <= 400 * 400)
                                speed *= 0.96f;
                            if (AITimer >= 30 && AITimer % 30 == 0)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<KeeperDreadCoil_Soulless>(),
                                    NPC.damage, RedeHelper.PolarVector(9, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.12f, 0.12f)),
                                    true, SoundID.Item20);
                            }
                            if (AITimer >= 130)
                            {
                                TimerRand = 0;
                                AITimer = 0;
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
                case ActionState.Death:
                    player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                    player.RedemptionScreen().lockScreen = true;
                    player.RedemptionScreen().cutscene = true;
                    NPC.LockMoveRadius(player);
                    player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(player.RedemptionScreen().ScreenShakeIntensity, 3);
                    NPC.velocity *= 0;
                    Reap = false;

                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                        NPC.alpha = 0;
                    }

                    NPC.alpha += 2;

                    if (NPC.alpha > 150)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), Scale: 3);
                            Main.dust[dustIndex].velocity *= 5f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), Scale: 3);
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
            NPC.Redemption().CreateParryWindow(SlashHitbox, ref parryActive);
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
        private int slashFrame;
        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[NPC.target];

            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            if (AIState is ActionState.Attacks && ID == 0 && AITimer >= 200)
            {
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    slashFrame++;
                    NPC.velocity *= 0.8f;
                    if (slashFrame == 3)
                    {
                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                        NPC.velocity.X = MathHelper.Clamp(Math.Abs((player.Center.X - NPC.Center.X) / 30), 30, 70) * NPC.spriteDirection;
                    }
                    if (slashFrame > 5)
                        slashFrame = 0;
                }
                return;
            }

            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 5 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Rectangle rect = NPC.frame;
            Vector2 origin = NPC.frame.Size() / 2;
            if (AIState is ActionState.Attacks && ID == 0 && AITimer >= 200)
            {
                texture = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_ReaperSlash").Value;
                int height = texture.Height / 6;
                int y = height * slashFrame;
                rect = new(0, y, texture.Width, height);
                origin = new(texture.Width / 2f, height / 2f);
            }
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(texture, oldPos + NPC.Size / 2f - screenPos, new Rectangle?(rect), NPC.GetAlpha(Color.GhostWhite) * 0.5f, oldrot[i], origin, NPC.scale + 0.1f, effects, 0);
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
            spriteBatch.Draw(texture, NPC.Center - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
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