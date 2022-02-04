using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Placeable.Trophies;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Redemption.Globals.RenderTargets.ShieldLayer;

namespace Redemption.NPCs.Bosses.Gigapora
{
    [AutoloadBossHead]
    public class Gigapora : ModNPC, IShieldSprite
    {
        public float[] oldrot = new float[6];
        public enum ActionState
        {
            Intro,
            WormAILol,
            ProtectCore,
            Gigabeam
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
            DisplayName.SetDefault("Omega Gigapora");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
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
                CustomTexturePath = "Redemption/Textures/Bestiary/OmegaGigapora_Bestiary"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 180;
            NPC.damage = 140;
            NPC.defense = 80;
            NPC.lifeMax = 50000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 15, 0, 0);
            NPC.boss = true;
            NPC.behindTiles = true;
            NPC.trapImmune = true;
            NPC.immortal = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossVlitch1G");
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LidenBiome>().Type };
            //BossBag = ModContent.ItemType<OmegaGigaporaBag>();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("gigagigiaigiaiiga.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            Redemption.Targets.ShieldLayer.Sprites.Remove(this);
            if (!RedeBossDowned.downedVlitch2)
            {
                //Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Gigapora_GirusTalk>(), 0, 0, Main.myPlayer);
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedVlitch1, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OmegaTrophy>(), 10));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            // TODO: loot
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
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

        private bool spawned;
        private float shieldAlpha;
        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (NPC.immortal)
                shieldAlpha += 0.04f;
            else
                shieldAlpha -= 0.04f;
            shieldAlpha = MathHelper.Clamp(shieldAlpha, 0, 1);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile target = Main.projectile[i];
                if (!target.active || target.minion || !target.friendly || target.damage <= 0)
                    continue;

                if (target.velocity.Length() == 0 || target.Redemption().TechnicallyMelee || !NPC.Hitbox.Intersects(target.Hitbox))
                    continue;

                if (NPC.immortal)
                    DustHelper.DrawCircle(target.Center, DustID.LifeDrain, 1, 2, 2, nogravity: true);
                target.Kill();
            }

            DespawnHandler();

            if (!spawned)
            {
                NPC.TargetClosest(false);
                Redemption.Targets.ShieldLayer.Sprites.Add(this);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;
                    int[] Type = { 0, 1, -1, -2, 2, -3, -4, 3, -5, -6, 4, -7, -8, 5, -9, -10, 6, -11, 7 };
                    for (int i = 0; i < Type.Length; ++i)
                    {
                        latestNPC = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Gigapora_BodySegment>(), NPC.whoAmI, 0, latestNPC);
                        if (latestNPC != Main.maxNPCs)
                        {
                            Main.npc[latestNPC].realLife = NPC.whoAmI;
                            Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                            Main.npc[latestNPC].netUpdate = true;
                            Main.npc[latestNPC].ai[2] = Type[i];

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, latestNPC);
                        }
                        else
                        {
                            NPC.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                            return;
                        }
                    }
                }
                spawned = true;
            }

            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();
            Vector2 targetPos;
            Point ground = NPC.Center.ToTileCoordinates();

            if (NPC.HasValidTarget)
                NPC.timeLeft = 300;

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Intro:
                    if (AITimer++ == 0)
                    {
                        NPC.velocity.Y = -20;
                        NPC.velocity.X = -4;
                    }
                    if (AITimer == 60 && !Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Omega Gigapora", 60, 90, 0.8f, 0, Color.Red,
                            "2nd Omega Prototype");
                    if (AITimer >= 80 && AITimer < 140)
                    {
                        NPC.velocity *= 0.96f;
                        NPC.velocity.X -= 0.3f;
                        NPC.velocity.Y += 0.4f;
                    }
                    else if (AITimer >= 140)
                    {
                        NPC.velocity.X *= 0.98f;
                        NPC.velocity.Y += 0.1f;
                        if (AITimer >= 220)
                        {
                            AIState = ActionState.WormAILol;
                            AITimer = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    break;

                case ActionState.WormAILol:
                    if (player.Center.Y - 400 > NPC.Center.Y)
                        TimerRand = 1;
                    if (TimerRand == 1)
                    {
                        if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).IsActive)
                            TimerRand = 0;
                        if (player.Center.X > NPC.Center.X && NPC.velocity.X < 6)
                            NPC.velocity.X += 0.1f;
                        else if (NPC.velocity.X > -6)
                            NPC.velocity.X -= 0.1f;
                        if (NPC.velocity.Y <= 20)
                            NPC.velocity.Y += 0.2f;
                    }
                    else if (TimerRand == 0)
                        WormMovement(player, 18, 0.1f);
                    if (++AITimer > 600)
                    {
                        TimerRand = 0;
                        AITimer = 0;
                        if (NPC.AnyNPCs(ModContent.NPCType<Gigapora_ShieldCore>()) && Main.rand.NextBool(2))
                            AIState = ActionState.ProtectCore;
                        else
                            AIState = ActionState.Gigabeam;
                    }
                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    break;
                case ActionState.ProtectCore:
                    int n = NPC.FindFirstNPC(ModContent.NPCType<Gigapora_ShieldCore>());
                    if (n != -1)
                    {
                        targetPos = RedeHelper.CenterPoint(Main.npc[n].Center, player.Center);
                        switch (TimerRand)
                        {
                            case 0:
                                if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).IsActive)
                                {
                                    if (AITimer > 0)
                                    {
                                        TimerRand = 0;
                                        AITimer = 0;
                                        AIState = ActionState.WormAILol;
                                    }
                                    else
                                        TimerRand = 1;
                                }
                                if (player.Center.X > NPC.Center.X && NPC.velocity.X < 6)
                                    NPC.velocity.X += 0.1f;
                                else if (NPC.velocity.X > -6)
                                    NPC.velocity.X -= 0.1f;
                                if (NPC.velocity.Y <= 20)
                                    NPC.velocity.Y += 0.2f;
                                break;
                            case 1:
                                if (NPC.DistanceSQ(new Vector2(targetPos.X, player.Center.Y + 400)) < 100 * 100 || AITimer++ >= 180)
                                {
                                    AITimer = 0;
                                    TimerRand = 2;
                                }
                                else
                                    Movement(new Vector2(targetPos.X, player.Center.Y + 500), 0.1f, 20);
                                break;
                            case 2:
                                if (NPC.DistanceSQ(targetPos) < 150 * 150 || AITimer++ >= 120)
                                {
                                    AITimer = 100;
                                    TimerRand = 0;
                                }
                                else
                                {
                                    Main.npc[n].ai[3] = 1;
                                    Movement(targetPos, 0.5f, 20);
                                }
                                break;
                        }
                    }
                    else
                    {
                        TimerRand = 0;
                        AITimer = 0;
                        AIState = ActionState.WormAILol;
                    }
                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    break;
                case ActionState.Gigabeam:
                    targetPos = new Vector2(player.Center.X + (player.Center.X > NPC.Center.X ? -400 : 400), player.Center.Y + 800);
                    switch (TimerRand)
                    {
                        case 0:
                            if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).IsActive)
                            {
                                if (AITimer > 0)
                                {
                                    TimerRand = 0;
                                    AITimer = 0;
                                    AIState = ActionState.WormAILol;
                                }
                                else
                                    TimerRand = 1;
                            }
                            if (player.Center.X > NPC.Center.X && NPC.velocity.X < 6)
                                NPC.velocity.X += 0.1f;
                            else if (NPC.velocity.X > -6)
                                NPC.velocity.X -= 0.1f;
                            if (NPC.velocity.Y <= 20)
                                NPC.velocity.Y += 0.2f;
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 1:
                            if (NPC.DistanceSQ(targetPos) < 100 * 100 || AITimer++ >= 600)
                            {
                                NPC.velocity.X *= 0.1f;
                                NPC.velocity.Y = -30;
                                AITimer = 0;
                                TimerRand = 2;
                            }
                            else
                                Movement(targetPos, 0.4f, 20);
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 2:
                            if (player.Center.Y + 60 > NPC.Center.Y)
                                AITimer = 1;
                            if (AITimer > 0)
                            {
                                AITimer++;
                                DrillLaser = true;
                                NPC.velocity *= 0.94f;
                                if (NPC.velocity.Y > -4 || AITimer >= 120)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/GigabeamSound").WithVolume(1.5f), NPC.position);
                                    AITimer = 0;
                                    TimerRand = 3;
                                }
                            }
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 3:
                            NPC.rotation.SlowRotation(NPC.DirectionTo(player.Center).ToRotation() + 1.57f, (float)Math.PI / 220f);
                            NPC.velocity = RedeHelper.PolarVector(-4, NPC.rotation + 1.57f);
                            if (AITimer < 80)
                            {
                                for (int k = 0; k < 2; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 200);
                                    vector.Y = (float)(Math.Cos(angle) * 200);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + RedeHelper.PolarVector(128, NPC.rotation - 1.57f) + vector, 2, 2, ModContent.DustType<GlowDust>(), 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = dust2.position.DirectionTo(NPC.Center + RedeHelper.PolarVector(128, NPC.rotation - 1.57f)) * 10f;
                                    Color dustColor = new(216, 35, 10) { A = 0 };
                                    dust2.color = dustColor;
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 200);
                                    vector.Y = (float)(Math.Cos(angle) * 200);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + RedeHelper.PolarVector(128, NPC.rotation - 1.57f) + vector, 2, 2, ModContent.DustType<GlowDust>(), 0f, 0f, 100, default, 1f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = dust2.position.DirectionTo(NPC.Center + RedeHelper.PolarVector(128, NPC.rotation - 1.57f)) * 10f;
                                    Color dustColor = new(255, 200, 193) { A = 0 };
                                    dust2.color = dustColor;
                                }
                            }
                            if (AITimer++ == 80)
                            {
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(30, Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity);
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigabeam>(), (int)(NPC.damage * 1.5f), Vector2.Zero, false, SoundID.Item1.WithVolume(0), "", NPC.whoAmI);
                            }
                            if (AITimer >= 340)
                            {
                                DrillLaser = false;
                                AITimer = 100;
                                TimerRand = 0;
                            }
                            break;
                    }
                    break;
            }
            if (AIState > ActionState.Intro && Framing.GetTileSafely(ground.X, ground.Y).IsActive)
            {
                player.RedemptionScreen().ScreenShakeIntensity = 3;
                if (NPC.soundDelay == 0)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Quake")
                            .WithVolume(MathHelper.Clamp(NPC.velocity.Length() / 10, 0.1f, 2f)), NPC.position);
                    NPC.soundDelay = 80;
                }
            }
            NPC.netUpdate = true;
        }
        private float BodyTimer;
        private int BodyState;
        public override void PostAI()
        {
            Player player = Main.player[NPC.target];
            Point ground = NPC.Center.ToTileCoordinates();
            if (NPC.type == ModContent.NPCType<Gigapora_BodySegment>())
            {
                switch (BodyState)
                {
                    case 0:
                        if (BodyTimer++ >= 600)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC seg = Main.npc[i];
                                if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>() || seg.ai[2] != 1 || seg.ai[0] != 0)
                                    continue;

                                if (seg.DistanceSQ(player.Center) >= 400 * 400 || Framing.GetTileSafely(ground.X, ground.Y).IsActive)
                                    continue;

                                SoundEngine.PlaySound(SoundID.Item61, NPC.position);
                                seg.ai[0] = 1;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int index = NPC.NewNPC((int)seg.Center.X, (int)seg.Center.Y, ModContent.NPCType<Gigapora_ShieldCore>(), 0, seg.whoAmI);
                                    Main.npc[index].velocity = Main.npc[(int)NPC.ai[3]].velocity;
                                    Main.npc[index].frameCounter = -25;
                                    if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, number: index);
                                }
                                BodyTimer = 0;
                                BodyState = 1;
                            }
                        }
                        break;
                }
            }
        }
        private int DrillFrame;
        private bool DrillLaser;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight)
                    NPC.frame.Y = 0;

                if (DrillLaser)
                {
                    DrillFrame++;
                    if (DrillFrame > 12)
                        DrillFrame = 12;
                }
                else
                {
                    if (DrillFrame > 5)
                        DrillFrame--;
                    else
                        DrillFrame++;
                    if (DrillFrame == 5)
                        DrillFrame = 0;
                }
            }
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (NPC.immortal)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/BallFire").WithVolume(0.5f).WithPitchVariance(0.1f), NPC.position);
                damage = 0;
                return false;
            }
            return true;
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity *= 0.96f;
                NPC.velocity.Y -= 5;
                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;
                return;
            }
        }
        private void WormMovement(Player player, float maxSpeed = 18f, float turnSpeed = 0.07f, float acceleration = 0.25f)
        {
            float comparisonSpeed = player.velocity.Length() * 1.5f;
            float rotationDifference = MathHelper.WrapAngle(NPC.velocity.ToRotation() - NPC.DirectionTo(player.Center).ToRotation());
            bool inFrontOfMe = Math.Abs(rotationDifference) < MathHelper.ToRadians(90 / 2);
            if (maxSpeed < comparisonSpeed && inFrontOfMe)
                maxSpeed = comparisonSpeed;

            if (NPC.DistanceSQ(player.Center) > 1500 * 1500)
            {
                turnSpeed *= 2f;
                acceleration *= 2f;

                if (inFrontOfMe && maxSpeed < 30f)
                    maxSpeed = 30f;
            }

            if (NPC.velocity.Length() > maxSpeed)
                NPC.velocity *= 0.99f;

            Vector2 target = player.Center;
            float distX = target.X - NPC.Center.X;
            float distY = target.Y - NPC.Center.Y;
            float num5 = maxSpeed / (float)Math.Sqrt(distX * distX + distY * distY);
            float num6 = distX * num5;
            float num7 = distY * num5;
            if ((NPC.velocity.X > 0f && num6 > 0f || NPC.velocity.X < 0f && num6 < 0f) && (NPC.velocity.Y > 0f && num7 > 0f || NPC.velocity.Y < 0f && num7 < 0f))
            {
                if (NPC.velocity.X < num6)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > num6)
                    NPC.velocity.X -= acceleration;
                if (NPC.velocity.Y < num7)
                    NPC.velocity.Y += acceleration;
                else if (NPC.velocity.Y > num7)
                    NPC.velocity.Y -= acceleration;
            }
            if (NPC.velocity.X > 0f && num6 > 0f || NPC.velocity.X < 0f && num6 < 0f || NPC.velocity.Y > 0f && num7 > 0f || NPC.velocity.Y < 0f && num7 < 0f)
            {
                if (NPC.velocity.X < num6)
                    NPC.velocity.X += turnSpeed;
                else if (NPC.velocity.X > num6)
                    NPC.velocity.X -= turnSpeed;
                if (NPC.velocity.Y < num7)
                    NPC.velocity.Y += turnSpeed;
                else if (NPC.velocity.Y > num7)
                    NPC.velocity.Y -= turnSpeed;

                if (Math.Abs(num7) < maxSpeed * 0.2f && (NPC.velocity.X > 0f && num6 < 0f || NPC.velocity.X < 0f && num6 > 0f))
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y += turnSpeed * 2f;
                    else
                        NPC.velocity.Y -= turnSpeed * 2f;
                }
                if (Math.Abs(num6) < maxSpeed * 0.2f && (NPC.velocity.Y > 0f && num7 < 0f || NPC.velocity.Y < 0f && num7 > 0f))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X += turnSpeed * 2f;
                    else
                        NPC.velocity.X -= turnSpeed * 2f;
                }
            }
            else if (Math.Abs(distX) > Math.Abs(distY))
            {
                if (NPC.velocity.X < num6)
                    NPC.velocity.X += turnSpeed * 1.1f;
                else if (NPC.velocity.X > num6)
                    NPC.velocity.X -= turnSpeed * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < maxSpeed * 0.5f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y += turnSpeed;
                    else
                        NPC.velocity.Y -= turnSpeed;
                }
            }
            else
            {
                if (NPC.velocity.Y < num7)
                    NPC.velocity.Y += turnSpeed * 1.1f;
                else if (NPC.velocity.Y > num7)
                    NPC.velocity.Y -= turnSpeed * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < maxSpeed * 0.5f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X += turnSpeed;
                    else
                        NPC.velocity.X -= turnSpeed;
                }
            }
        }
        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
        {
            if (NPC.Center.X < targetPos.X)
            {
                NPC.velocity.X += speedModifier;
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += speedModifier * 2;
            }
            else
            {
                NPC.velocity.X -= speedModifier;
                if (NPC.velocity.X > 0)
                    NPC.velocity.X -= speedModifier * 2;
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (NPC.type == ModContent.NPCType<Gigapora>())
            {
                if (!NPC.IsABestiaryIconDummy)
                {
                    Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                    Texture2D drill = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Drill").Value;
                    var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White * shieldAlpha, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                    int height = drill.Height / 13;
                    int y = height * DrillFrame;
                    spriteBatch.Draw(drill, NPC.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, y, drill.Width, height)), Color.White * shieldAlpha, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(-2, 96), NPC.scale, effects, 0);
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.type == ModContent.NPCType<Gigapora>())
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                Texture2D glowMask = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
                Texture2D drill = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Drill").Value;
                Texture2D thrusterBlue = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_ThrusterBlue").Value;
                var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float thrusterScaleX = MathHelper.Lerp(1.5f, 0.5f, NPC.velocity.Length() / 20);
                thrusterScaleX = MathHelper.Clamp(thrusterScaleX, 0.5f, 1.5f);
                float thrusterScaleY = MathHelper.Clamp(NPC.velocity.Length() / 10, 0.3f, 2f);
                float pulse = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1, 0.2f, 1);
                Vector2 pos = NPC.Center + new Vector2(0, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Vector2 thrusterBOrigin = new(thrusterBlue.Width / 2f, thrusterBlue.Height / 2f - 20);
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(thrusterBlue, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * 0.5f * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), oldrot[i], thrusterBOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                    spriteBatch.Draw(thrusterBlue, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(-40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * 0.5f * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), oldrot[i], thrusterBOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                }
                spriteBatch.Draw(thrusterBlue, pos + RedeHelper.PolarVector(40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), NPC.rotation, thrusterBOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                spriteBatch.Draw(thrusterBlue, pos + RedeHelper.PolarVector(-40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), NPC.rotation, thrusterBOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(texture, pos - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.Draw(glowMask, pos - screenPos, NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                int height = drill.Height / 13;
                int y = height * DrillFrame;
                spriteBatch.Draw(drill, pos - screenPos, new Rectangle?(new Rectangle(0, y, drill.Width, height)), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(-2, 96), NPC.scale, effects, 0);
            }
            return false;
        }
    }
}