using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
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
using Terraria.Utilities;

namespace Redemption.NPCs.Bosses.Gigapora
{
    [AutoloadBossHead]
    public class Gigapora : ModNPC
    {
        private static Asset<Texture2D> glowMask;
        private static Asset<Texture2D> drill;
        private static Asset<Texture2D> drillShoot;
        private static Asset<Texture2D> thrusterBlue;
        private static Asset<Texture2D> thrusterOrange;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glowMask = ModContent.Request<Texture2D>(Texture + "_Glow");
            drill = ModContent.Request<Texture2D>(Texture + "_Drill");
            drillShoot = ModContent.Request<Texture2D>(Texture + "_Drill_Shoot");
            thrusterBlue = ModContent.Request<Texture2D>(Texture + "_ThrusterBlue");
            thrusterOrange = ModContent.Request<Texture2D>(Texture + "_ThrusterOrange");
        }
        public override void Unload()
        {
            glowMask = null;
            drill = null;
            drillShoot = null;
            thrusterBlue = null;
            thrusterOrange = null;
        }
        public float[] oldrot = new float[6];
        public enum ActionState
        {
            Intro,
            WormAILol,
            ProtectCore,
            Gigabeam,
            Death,
            Flamethrowers,
            BurrowAtk,
            CrossBomb
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];

        public bool Active { get => NPC.active; set => NPC.active = value; }
        public static float c = 1f / 255f;
        public Color innerColor = new(150 * c * 0.5f, 20 * c * 0.5f, 54 * c * 0.5f, 0.5f);
        public Color borderColor = new(215 * c, 79 * c, 214 * c, 1f);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Gigapora");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                CustomTexturePath = "Redemption/Textures/Bestiary/OmegaGigapora_Bestiary"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 140;
            NPC.damage = 96;
            NPC.defense = 20;
            NPC.lifeMax = 300000;
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
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega1");
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LidenBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Gigapora"))
            });
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                if (NPC.type == ModContent.NPCType<Gigapora>())
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/GigaporaGore1").Type);
                    for (int i = 0; i < 4; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/GigaporaGoreDrill" + (i + 1)).Type);
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC seg = Main.npc[i];
                    if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>())
                        continue;

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.MissileExplosion, seg.position);
                    RedeDraw.SpawnExplosion(seg.Center, Color.OrangeRed);
                }
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.MissileExplosion, NPC.position);
                RedeDraw.SpawnExplosion(NPC.Center, Color.OrangeRed);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            if (!RedeBossDowned.downedOmega2)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Gigapora_GirusTalk>(), 0, 0, Main.myPlayer);
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedOmega2, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<OmegaGigaporaBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OmegaGigaporaTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<GigaporaRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<PowerDrill>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<DrillHeadHead>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OmegaPowerCell>(), 1, 2, 4));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CorruptedXenomite>(), 1, 8, 16));
            npcLoot.Add(notExpertRule);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return AIState != ActionState.CrossBomb;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Ejected);
            writer.Write(BodyState);
            writer.Write(BodyTimer);
            writer.Write(facing);
            writer.Write(spawned);
            writer.Write(xbombDone);
            writer.Write(flameDone);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Ejected = reader.ReadInt32();
            BodyState = reader.ReadInt32();
            BodyTimer = reader.ReadSingle();
            facing = reader.ReadSingle();
            spawned = reader.ReadBoolean();
            xbombDone = reader.ReadBoolean();
            flameDone = reader.ReadBoolean();
        }

        private bool spawned;
        private float shieldAlpha;
        private float facing;

        private float BodyTimer;
        private int BodyState;
        private int Ejected;
        private bool xbombDone;
        private bool flameDone;
        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (NPC.immortal && AIState is not ActionState.Death && NPC.ai[3] == 0)
                shieldAlpha += 0.04f;
            else
                shieldAlpha -= 0.04f;
            shieldAlpha = MathHelper.Clamp(shieldAlpha, 0, 1);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile target = Main.projectile[i];
                if (!target.active || !target.friendly || target.damage <= 0)
                    continue;

                if (target.velocity.Length() == 0 || target.ProjBlockBlacklist() || !NPC.Hitbox.Intersects(target.Hitbox))
                    continue;

                if (NPC.immortal && AIState is not ActionState.Death && NPC.ai[3] == 0)
                    RedeDraw.SpawnRing(target.Center, Color.Red, 0.13f, 0.7f);
                target.Kill();
            }

            if (NPC.DespawnHandler(0, 5))
                return;

            if (!spawned)
            {
                NPC.TargetClosest(false);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;
                    int[] Type = { 0, 1, -1, -2, 2, -3, -4, 3, -5, -6, 4, -7, -8, 5, -9, -10, 6, -11, 7 };
                    for (int i = 0; i < Type.Length; ++i)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Gigapora_BodySegment>(), NPC.whoAmI, 0, latestNPC);
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
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Gigapora.Name"), 60, 90, 0.8f, 0, Color.Red, Language.GetTextValue("Mods.Redemption.TitleCard.Gigapora.Modifier"));
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
                        if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).HasTile)
                            TimerRand = 0;
                        if (player.RightOf(NPC) && NPC.velocity.X < 6)
                            NPC.velocity.X += 0.1f;
                        else if (NPC.velocity.X > -6)
                            NPC.velocity.X -= 0.1f;
                        if (NPC.velocity.Y <= 20)
                            NPC.velocity.Y += 0.2f;
                    }
                    else if (TimerRand == 0)
                    {
                        int speed = 18;
                        if (Framing.GetTileSafely(ground.X, ground.Y).HasTile)
                            speed = 12;

                        float turnSpeed = 0.06f;
                        if (NPC.DistanceSQ(player.Center) <= 600 * 600)
                            turnSpeed = 0.001f;
                        WormMovement(player, speed, turnSpeed);
                    }
                    if (++AITimer > 500)
                    {
                        TimerRand = 0;
                        AITimer = 0;

                        WeightedRandom<ActionState> choice = new(Main.rand);
                        if (NPC.AnyNPCs(ModContent.NPCType<Gigapora_ShieldCore>()))
                            choice.Add(ActionState.ProtectCore);
                        if (BodyState >= 5 || NPC.life <= NPC.lifeMax / 2)
                            choice.Add(ActionState.Gigabeam, BodyState >= 7 ? 1.5f : 1);
                        if (!flameDone)
                            choice.Add(ActionState.Flamethrowers, BodyState >= 3 ? .4f : 1);
                        choice.Add(ActionState.BurrowAtk, BodyState >= 2 ? .8f : 0);
                        if (!xbombDone)
                            choice.Add(ActionState.CrossBomb);

                        AIState = choice;
                        NPC.netUpdate = true;
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
                                if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).HasTile)
                                {
                                    if (AITimer > 0)
                                    {
                                        TimerRand = 0;
                                        AITimer = 440;
                                        AIState = ActionState.WormAILol;
                                    }
                                    else
                                        TimerRand = 1;
                                }
                                if (player.RightOf(NPC) && NPC.velocity.X < 6)
                                    NPC.velocity.X += 0.1f;
                                else if (NPC.velocity.X > -6)
                                    NPC.velocity.X -= 0.1f;
                                if (NPC.velocity.Y <= 20)
                                    NPC.velocity.Y += 0.2f;
                                break;
                            case 1:
                                if (NPC.DistanceSQ(new Vector2(targetPos.X, player.Center.Y + 500)) < 100 * 100 || AITimer++ >= 180)
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
                    targetPos = new Vector2(player.Center.X + (400 * NPC.RightOfDir(player)), player.Center.Y + 800);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC core = Main.npc[i];
                        if (!core.active || core.type != ModContent.NPCType<Gigapora_ShieldCore>() || core.ai[1] == 5)
                            continue;

                        core.ai[1] = 1;
                        core.ai[2] = 1;

                    }
                    switch (TimerRand)
                    {
                        case 0:
                            if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).HasTile)
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
                            if (player.RightOf(NPC) && NPC.velocity.X < 6)
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
                            if (player.Center.Y + 100 > NPC.Center.Y)
                                AITimer = 1;
                            if (AITimer > 0)
                            {
                                AITimer++;
                                DrillLaser = true;
                                NPC.velocity *= 0.94f;
                                if (NPC.velocity.Y > -4 || AITimer >= 120)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.GigaLaserCharge, NPC.position);
                                    AITimer = 0;
                                    TimerRand = 3;
                                }
                            }
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 3:
                            ScreenPlayer.CutsceneLock(player, NPC.Center, ScreenPlayer.CutscenePriority.None, 1600, 1600, 0, true);
                            NPC.rotation.SlowRotation(NPC.DirectionTo(player.Center).ToRotation() + 1.57f, (float)Math.PI / 220f);
                            NPC.velocity = RedeHelper.PolarVector(-4, NPC.rotation + 1.57f);
                            if (AITimer < 120)
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
                            if (AITimer++ == 120)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.GigaLaserFire, NPC.position);
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 30;
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigabeam>(), (int)(NPC.damage * 1.5f), Vector2.Zero, NPC.whoAmI);
                            }

                            if (AITimer == 453 && !Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.GigaLaserCoolDown, NPC.position);
                            if (AITimer >= 470)
                            {
                                DrillLaser = false;
                                AITimer = 100;
                                TimerRand = 0;
                            }
                            break;
                    }
                    break;
                case ActionState.Flamethrowers:
                    targetPos = new Vector2(player.Center.X + (600 * NPC.RightOfDir(player)), player.Center.Y + 800);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC core = Main.npc[i];
                        if (!core.active || core.type != ModContent.NPCType<Gigapora_ShieldCore>() || core.ai[1] == 5)
                            continue;

                        core.ai[1] = 1;
                        core.ai[2] = 1;

                    }
                    switch (TimerRand)
                    {
                        case 0:
                            if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).HasTile)
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
                            if (player.RightOf(NPC) && NPC.velocity.X < 6)
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
                                NPC.velocity.Y = -20;
                                AITimer = 0;
                                TimerRand = 2;
                            }
                            else
                                Movement(targetPos, 0.4f, 20);
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 2:
                            if (AITimer > 0)
                            {
                                AITimer++;
                                NPC.Move(player.Center - new Vector2(0, 200), 30, 10);
                                if (NPC.DistanceSQ(player.Center - new Vector2(0, 200)) < 200 * 200 || AITimer >= 120)
                                {
                                    facing = NPC.velocity.X < 0 ? MathHelper.Pi : 0;
                                    NPC.velocity *= 0.9f;
                                    AITimer = 0;
                                    TimerRand = 3;
                                }
                            }
                            else
                            {
                                if (player.Center.Y + 100 > NPC.Center.Y)
                                    AITimer = 1;
                            }
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 3:
                            AITimer++;
                            NPC.rotation.SlowRotation(1.57f + facing, (float)Math.PI / 200f);
                            NPC.velocity = RedeHelper.PolarVector(-30 / ((AITimer / 10) + 1), NPC.rotation + 1.57f);
                            if ((NPC.velocity.X > -3 && NPC.velocity.X < 3) || AITimer >= 80)
                            {
                                AITimer = 0;
                                TimerRand = 4;
                            }
                            break;
                        case 4:
                            bool s = player.Center.X < NPC.Center.X;
                            if (facing == 0)
                                s = player.Center.X > NPC.Center.X;
                            NPC.rotation.SlowRotation(1.57f + facing, (float)Math.PI / 200f);
                            NPC.velocity = RedeHelper.PolarVector((s ? -7 : -3) * ((AITimer / 200) + 1), NPC.rotation + 1.57f);

                            if (AITimer++ >= 200)
                            {
                                flameDone = true;
                                xbombDone = false;
                                AITimer = 100;
                                TimerRand = 0;
                            }
                            break;
                    }
                    break;
                case ActionState.BurrowAtk:
                    targetPos = new Vector2(player.Center.X, player.Center.Y + 1800);
                    switch (TimerRand)
                    {
                        case 0:
                            if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).HasTile)
                            {
                                if (AITimer > 0)
                                {
                                    AITimer = 300;
                                    TimerRand = 0;
                                    AIState = ActionState.WormAILol;
                                }
                                else
                                    TimerRand = 1;
                            }
                            if (player.RightOf(NPC) && NPC.velocity.X < 6)
                                NPC.velocity.X += 0.1f;
                            else if (NPC.velocity.X > -6)
                                NPC.velocity.X -= 0.1f;
                            if (NPC.velocity.Y <= 20)
                                NPC.velocity.Y += 0.2f;
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 1:
                            if (NPC.DistanceSQ(targetPos) < 200 * 200 || AITimer++ >= 600)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Quake with { Pitch = -.1f, Volume = 2 }, player.position);
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
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 2;
                            int _ = BaseWorldGen.GetFirstTileFloor((int)NPC.Center.X / 16, (int)player.position.Y / 16);
                            if (_ * 16 > NPC.Center.Y)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom with { Pitch = -.2f, Volume = 2 }, NPC.position);
                                NPC.velocity *= 0.9f;
                                AITimer = 100;
                                TimerRand = 0;
                                for (int i = 0; i < Main.rand.Next(24, 31); i++)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigapora_Rubble>(), NPC.damage, new Vector2(Main.rand.Next(-14, 15), Main.rand.Next(-30, -19)));
                                }
                            }
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                    }
                    break;
                case ActionState.CrossBomb:
                    targetPos = new Vector2(player.Center.X + (400 * NPC.RightOfDir(player)), player.Center.Y + 800);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC core = Main.npc[i];
                        if (!core.active || core.type != ModContent.NPCType<Gigapora_ShieldCore>() || core.ai[1] == 5)
                            continue;

                        core.ai[1] = 1;
                        core.ai[2] = 1;

                    }
                    switch (TimerRand)
                    {
                        case 0:
                            if (player.Center.Y < NPC.Center.Y && Framing.GetTileSafely(ground.X, ground.Y).HasTile)
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
                            if (player.RightOf(NPC) && NPC.velocity.X < 6)
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
                                NPC.velocity.Y = -24;
                                AITimer = 0;
                                TimerRand = 2;
                            }
                            else
                                Movement(targetPos, 0.4f, 20);
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 2:
                            if (player.Center.Y + 100 > NPC.Center.Y)
                                AITimer = 1;
                            if (AITimer > 0)
                            {
                                AITimer++;
                                NPC.velocity *= 0.94f;
                                if (NPC.velocity.Y > -16 || AITimer >= 120)
                                {
                                    AITimer = 0;
                                    TimerRand = 3;
                                }
                            }
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        case 3:
                            NPC.rotation.SlowRotation(NPC.DirectionTo(player.Center).ToRotation() + 1.57f, (float)Math.PI / 120f);
                            NPC.velocity = RedeHelper.PolarVector(-16, NPC.rotation + 1.57f);
                            if (AITimer++ >= 280)
                            {
                                flameDone = false;
                                xbombDone = true;
                                AITimer = 100;
                                TimerRand = 0;
                            }
                            break;
                    }
                    break;
                case ActionState.Death:
                    switch (TimerRand)
                    {
                        case 0:
                            AITimer = 0;
                            if (player.Center.Y < NPC.Center.Y || Framing.GetTileSafely(ground.X, ground.Y).HasTile)
                            {
                                NPC.velocity.Y = -25;
                                NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            }
                            else
                            {
                                NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                                NPC.velocity.Y *= 0.8f;
                                if (NPC.velocity.Y >= -3)
                                {
                                    NPC.velocity *= 0;
                                    TimerRand = 1;
                                }
                            }
                            NPC.velocity.X *= 0.98f;
                            break;
                        case 1:
                            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.None, 3000, 6000, 0);
                            if (AITimer++ >= 180)
                            {
                                NPC.immortal = false;
                                NPC.dontTakeDamage = false;
                                player.ApplyDamageToNPC(NPC, 99999, 0, 0, false);
                                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                            }
                            break;
                    }
                    break;
            }
            if (BodyState < 1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC seg = Main.npc[i];
                    if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>() || seg.ai[2] > 0 || seg.ai[0] == 2)
                        continue;
                    seg.ai[0] = 0;
                }
            }
            switch (BodyState)
            {
                case 0:
                    if (BodyTimer++ >= 600)
                    {
                        EjectCore();
                        if (Ejected >= 1)
                        {
                            Ejected = 0;
                            BodyTimer = 0;
                            BodyState = 1;
                        }
                    }
                    break;
                case 1:
                    if (NPC.CountNPCS(ModContent.NPCType<Gigapora_ShieldCore>()) <= 0)
                        BodyState = 2;
                    break;
                case 2:
                    if (BodyTimer++ >= 300)
                    {
                        EjectCore(2);
                        if (Ejected >= 1)
                        {
                            Ejected = 0;
                            BodyTimer = 0;
                            BodyState = 3;
                        }
                    }
                    break;
                case 3:
                    if (NPC.CountNPCS(ModContent.NPCType<Gigapora_ShieldCore>()) <= 0)
                        BodyState = 4;
                    break;
                case 4:
                    if (BodyTimer++ >= 300)
                    {
                        EjectCore(3);
                        EjectCore(4);
                        if (Ejected >= 2)
                        {
                            Ejected = 0;
                            BodyTimer = 0;
                            BodyState = 5;
                        }
                    }
                    break;
                case 5:
                    if (NPC.CountNPCS(ModContent.NPCType<Gigapora_ShieldCore>()) <= 0)
                        BodyState = 6;
                    break;
                case 6:
                    if (BodyTimer++ >= 400)
                    {
                        EjectCore(5);
                        EjectCore(6);
                        if (Ejected >= 2)
                        {
                            Ejected = 0;
                            BodyTimer = 0;
                            BodyState = 7;
                        }
                    }
                    break;
                case 7:
                    if (NPC.CountNPCS(ModContent.NPCType<Gigapora_ShieldCore>()) <= 0)
                        BodyState = 8;
                    break;
                case 8:
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC seg = Main.npc[i];
                        if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>())
                            continue;

                        seg.dontTakeDamage = true;
                        seg.ai[0] = 2;
                        if (Main.netMode == NetmodeID.Server && seg.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: seg.whoAmI);
                    }
                    AITimer = 0;
                    TimerRand = 0;
                    AIState = ActionState.Death;
                    BodyState = 9;
                    break;
                case 9:
                    if (Main.rand.NextBool(100))
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        RedeDraw.SpawnExplosion(new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), Color.OrangeRed);
                    }
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC seg = Main.npc[i];
                        if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>())
                            continue;

                        if (Main.rand.NextBool(100))
                        {
                            SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                            RedeDraw.SpawnExplosion(new Vector2(seg.position.X + Main.rand.Next(seg.width), seg.position.Y + Main.rand.Next(seg.height)), Color.OrangeRed);
                        }
                    }
                    break;
            }
            if (AIState > ActionState.Intro && Framing.GetTileSafely(ground.X, ground.Y).HasTile)
            {
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(3, player.RedemptionScreen().ScreenShakeIntensity);
                if (NPC.soundDelay == 0)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Quake with { Volume = MathHelper.Clamp(NPC.velocity.Length() / 10, 0.1f, 2f) }, NPC.position);
                    NPC.soundDelay = 80;
                }
            }
            NPC.netUpdate = true;
        }
        public override bool CheckDead()
        {
            if (NPC.type == ModContent.NPCType<Gigapora>())
            {
                if (AIState is ActionState.Death && AITimer >= 180 && TimerRand == 1)
                    return true;
                BodyState = 8;
                NPC.dontTakeDamage = true;
                NPC.life = 1;
                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return false;
            }
            NPC.dontTakeDamage = true;
            NPC.life = 1;
            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            return false;
        }
        public override void PostAI()
        {
            Player player = Main.player[NPC.target];
            if (player.active && !player.dead && !Main.dayTime)
                NPC.DiscourageDespawn(60);
        }
        public void EjectCore(int segID = 1)
        {
            Player player = Main.player[NPC.target];
            Point ground = NPC.Center.ToTileCoordinates();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC seg = Main.npc[i];
                if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>() || seg.ai[2] != segID || seg.ai[0] != 0)
                    continue;

                if (seg.DistanceSQ(player.Center) >= 600 * 600 || Framing.GetTileSafely(ground.X, ground.Y).HasTile)
                    continue;

                SoundEngine.PlaySound(SoundID.Item61, NPC.position);
                seg.ai[0] = 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float lifeScale = (segID - 1f) / 10f;
                    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)seg.Center.X, (int)seg.Center.Y, ModContent.NPCType<Gigapora_ShieldCore>(), 0, seg.whoAmI);
                    Main.npc[index].lifeMax = (int)(Main.npc[index].lifeMax * (lifeScale + 1));
                    Main.npc[index].life = Main.npc[index].lifeMax;
                    Main.npc[index].velocity = NPC.velocity;
                    Main.npc[index].frameCounter = -25;
                    if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: index);
                }
                Ejected++;
            }
        }
        private int DrillFrame;
        private bool DrillLaser;
        public override void FindFrame(int frameHeight)
        {
            if (BodyState >= 8)
            {
                NPC.frame.Y = 2 * frameHeight;
                return;
            }
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
                    if (DrillFrame > 15)
                        DrillFrame = 15;
                }
                else
                {
                    if (DrillFrame > 8)
                        DrillFrame--;
                    else
                        DrillFrame++;
                    if (DrillFrame == 8)
                        DrillFrame = 0;
                }
            }
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.immortal && AIState is not ActionState.Death && NPC.ai[3] == 0)
            {
                NPC.HitSound = CustomSounds.BallFire with { Volume = .5f };
                modifiers.SetMaxDamage(1);
                modifiers.DisableCrit();
                modifiers.HideCombatText();
                CombatText.NewText(NPC.getRect(), Color.Orange, 0, true, true);
                NPC.life++;
                return;
            }
            else
                NPC.HitSound = SoundID.NPCHit4;
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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.type == ModContent.NPCType<Gigapora>())
            {
                Texture2D HexagonTexture = ModContent.Request<Texture2D>("Redemption/Textures/Hexagons", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Effect ShieldEffect = ModContent.Request<Effect>("Redemption/Effects/Shield", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
                ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
                ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f * shieldAlpha).ToVector4());
                ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, shieldAlpha).ToVector4());

                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float thrusterScaleX = MathHelper.Lerp(1.5f, 0.5f, NPC.velocity.Length() / 20);
                thrusterScaleX = MathHelper.Clamp(thrusterScaleX, 0.5f, 1.5f);
                float thrusterScaleY = MathHelper.Clamp(NPC.velocity.Length() / 10, 0.3f, 2f);
                Vector2 v = RedeHelper.Spread(4);
                Vector2 pos = NPC.Center;
                if (BodyState >= 9)
                    pos = NPC.Center + v;

                spriteBatch.End();
                spriteBatch.BeginAdditive();

                Vector2 thrusterOrigin = new(thrusterBlue.Value.Width / 2f, thrusterBlue.Value.Height / 2f - 20);
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(BodyState >= 8 ? thrusterOrange.Value : thrusterBlue.Value, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * 0.5f * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), oldrot[i], thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                    spriteBatch.Draw(BodyState >= 8 ? thrusterOrange.Value : thrusterBlue.Value, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(-40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * 0.5f * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), oldrot[i], thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                }
                spriteBatch.Draw(BodyState >= 8 ? thrusterOrange.Value : thrusterBlue.Value, pos + RedeHelper.PolarVector(40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), NPC.rotation, thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                spriteBatch.Draw(BodyState >= 8 ? thrusterOrange.Value : thrusterBlue.Value, pos + RedeHelper.PolarVector(-40, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(NPC.velocity.Length() / 20, 0, 1), NPC.rotation, thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);

                spriteBatch.End();
                ShieldEffect.Parameters["sinMult"].SetValue(10f);
                ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(texture.Width / 2f / HexagonTexture.Width, texture.Height / 3 / HexagonTexture.Height));
                ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (texture.Width / 2), 1f / (texture.Height / 2)));
                ShieldEffect.Parameters["frameAmount"].SetValue(3f);
                spriteBatch.BeginDefault(true);
                ShieldEffect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(texture, pos - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.End();
                spriteBatch.BeginDefault();

                spriteBatch.Draw(glowMask.Value, pos - screenPos, NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.End();

                ShieldEffect.Parameters["sinMult"].SetValue(30f / 6f);
                ShieldEffect.Parameters["frameAmount"].SetValue(8f);
                spriteBatch.BeginDefault(true);

                if (DrillFrame >= 8)
                {
                    ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(drillShoot.Value.Width / 2f / HexagonTexture.Width, drillShoot.Value.Height / 8 / HexagonTexture.Height));
                    ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (drillShoot.Value.Width / 2), 1f / (drillShoot.Value.Height / 2)));
                    ShieldEffect.CurrentTechnique.Passes[0].Apply();
                    int height = drillShoot.Value.Height / 8;
                    int y = height * (DrillFrame - 8);
                    spriteBatch.Draw(drillShoot.Value, pos - screenPos, new Rectangle?(new Rectangle(0, y, drillShoot.Value.Width, height)), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(16, 98), NPC.scale, effects, 0);
                }
                else
                {
                    ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(drill.Value.Width / 2f / HexagonTexture.Width, drill.Value.Height / 8 / HexagonTexture.Height));
                    ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (drill.Value.Width / 2), 1f / (drill.Value.Height / 2)));
                    ShieldEffect.CurrentTechnique.Passes[0].Apply();
                    int height = drill.Value.Height / 8;
                    int y = height * DrillFrame;
                    spriteBatch.Draw(drill.Value, pos - screenPos, new Rectangle?(new Rectangle(0, y, drill.Value.Width, height)), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(-8, 96), NPC.scale, effects, 0);
                }
            }
            return false;
        }
    }
}
