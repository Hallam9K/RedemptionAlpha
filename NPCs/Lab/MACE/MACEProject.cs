using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.DataStructures;
using Redemption.Globals.NPC;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.Items.Lore;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using System;
using Redemption.WorldGeneration;
using Redemption.Base;
using Redemption.Items.Weapons.PostML.Ranged;

namespace Redemption.NPCs.Lab.MACE
{
    [AutoloadBossHead]
    public class MACEProject : ModNPC
    {
        public enum ActionState
        {
            Begin,
            JawPhase,
            HeadPhase,
            PhaseChange
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MACE Project");
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 92;
            NPC.height = 164;
            NPC.damage = 100;
            NPC.lifeMax = 125000;
            NPC.knockBackResist = 0;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.GetGlobalNPC<GuardNPC>().GuardPoints = NPC.lifeMax / 4;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusicMP");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("An old, unfinished war machine that went haywire. Looks like it never saw the light of day, as it's in progress of being scrapped for parts and components.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 2f);
                    Main.dust[dustIndex].velocity *= 8f;
                }
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreMetalPart").Type);
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreMetalScrap").Type);
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGorePaintScrap").Type);
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreWinglet").Type);
                Gore.NewGore(new Vector2(NPC.Center.X, NPC.Center.Y - 16), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreForeheadGem").Type);
                Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreHead").Type);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<LabPlatingDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[4])
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel5>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedMACE, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk6>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk6_1>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Electronade>()));
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (!NPC.GetGlobalNPC<GuardNPC>().IgnoreArmour && !NPC.HasBuff(BuffID.BrokenArmor) && !NPC.GetGlobalNPC<BuffNPC>().stunned && NPC.GetGlobalNPC<GuardNPC>().GuardPoints >= 0)
            {
                NPC.GetGlobalNPC<GuardNPC>().GuardHit(NPC, ref damage, SoundID.NPCHit4);
                return false;
            }
            NPC.GetGlobalNPC<GuardNPC>().GuardBreakCheck(NPC, DustID.Electric, SoundID.Item37, 10, 1, 4000);
            return true;
        }

        private Vector2 JawCenter;
        private Vector2 CraneOrigin;
        private int GuardPointMax;
        private int Phase;
        private int LaserTimer;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            DespawnHandler();

            if (!player.active || player.dead)
                return;

            Vector2 MouthOrigin = new(NPC.Center.X, NPC.Center.Y + 54);
            Vector2 EyeOrigin = NPC.Center + new Vector2(-22, 4);
            if (Phase == 2)
                EyeOrigin = NPC.Center + new Vector2(-22, 22);

            if (Phase == 0 && NPC.GetGlobalNPC<GuardNPC>().GuardPoints <= GuardPointMax / 2)
            {
                AIState = ActionState.PhaseChange;
                NPC.netUpdate = true;
            }
            if ((Phase == 1 && NPC.GetGlobalNPC<GuardNPC>().GuardPoints <= 0) || (Phase < 2 && NPC.life <= (int)(NPC.lifeMax * 0.75f)))
            {
                NPC.GetGlobalNPC<GuardNPC>().GuardPoints = 0;
                AIState = ActionState.PhaseChange;
                NPC.netUpdate = true;
            }
            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                        {
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("MACE Project", 60, 90, 0.8f, 0, Color.Yellow, "Incomplete War Machine"); SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/SpookyNoise"), NPC.position);
                        }
                        GuardPointMax = NPC.GetGlobalNPC<GuardNPC>().GuardPoints;
                        CraneOrigin = NPC.Center;
                        GlowActive = true;
                    }
                    if (AITimer >= 80)
                    {
                        AITimer = 0;
                        JawOpen = true;
                        AIState = ActionState.JawPhase;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.JawPhase:
                    if (TimerRand2 != 3 && LaserTimer++ % 160 == 0)
                    {
                        GlowActive = true;
                        GlowTimer = 0;
                        NPC.Shoot(EyeOrigin, ModContent.ProjectileType<MACE_Laser>(), NPC.damage, RedeHelper.PolarVector(9, (player.Center - NPC.Center).ToRotation()), false, SoundID.Item125);
                    }
                    switch (TimerRand2)
                    {
                        case -1:
                            NPC.MoveToVector2(CraneOrigin, 10);
                            if (NPC.DistanceSQ(CraneOrigin) < 6 * 6)
                            {
                                JawOpen = true;
                                NPC.velocity *= 0;
                                TimerRand = 0;
                                AITimer = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 0:
                            AITimer++;
                            if (!Main.dedServ)
                            {
                                if (AITimer == 1)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DistortedRoar").WithVolume(.5f), NPC.position);
                                if (AITimer < 60)
                                {
                                    TimerRand += (float)Math.PI / 120;
                                    if (TimerRand >= Math.PI) TimerRand = 0;
                                    float timer = TimerRand;
                                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoR:Shockwave", NPC.Center)?.GetShader().UseProgress(timer).UseOpacity(100f * (1 - timer / 2f)).UseColor(1, 1, 6).UseTargetPosition(MouthOrigin);
                                }
                                else
                                    Terraria.Graphics.Effects.Filters.Scene["MoR:Shockwave"].Deactivate();
                            }
                            if (AITimer == 35)
                            {
                                GlowActive2 = true;
                                GlowTimer2 = 0;
                                for (int i = 0; i < 20; i++)
                                {
                                    int dustIndex = Dust.NewDust(MouthOrigin, 2, 2, DustID.OrangeTorch, Scale: 2f);
                                    Main.dust[dustIndex].velocity *= 10f;
                                }
                                for (int i = 0; i < 5; i++)
                                    NPC.Shoot(MouthOrigin, ModContent.ProjectileType<MACE_Miniblast>(), NPC.damage, RedeHelper.PolarVector(8, MathHelper.ToRadians(72) * i), false, SoundID.Item73);
                            }
                            if (AITimer == 40)
                            {
                                GlowActive2 = true;
                                GlowTimer2 = 0;
                                for (int i = 0; i < 20; i++)
                                {
                                    int dustIndex = Dust.NewDust(MouthOrigin, 2, 2, DustID.OrangeTorch, Scale: 2f);
                                    Main.dust[dustIndex].velocity *= 10f;
                                }
                                for (int i = 0; i < 10; i++)
                                    NPC.Shoot(MouthOrigin, ModContent.ProjectileType<MACE_Miniblast>(), NPC.damage, RedeHelper.PolarVector(8, MathHelper.ToRadians(36) * i), false, SoundID.Item73);
                            }
                            if (AITimer == 80)
                                JawOpen = false;
                            if (AITimer > 160)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            if (AITimer++ == 0)
                                JawOpen = true;
                            if (AITimer >= 50 && AITimer % (Phase == 0 ? 20 : 12) == 0 && AITimer <= 100)
                            {
                                NPC.Shoot(MouthOrigin, ModContent.ProjectileType<BigElectronade>(), NPC.damage, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), false, SoundID.Item61);
                            }
                            if (AITimer == 100)
                                JawOpen = false;
                            if (AITimer > 160)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            switch (TimerRand)
                            {
                                case 0:
                                    if (AITimer++ == 0)
                                        JawOpen = true;
                                    Vector2 v = new((RedeGen.LabVector.X + 48) * 16, NPC.Center.Y);
                                    if (NPC.DistanceSQ(v) < 10 * 10)
                                    {
                                        TimerRand++;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.Move(v, 7, 30);
                                    break;
                                case 1:
                                    Vector2 v2 = new((RedeGen.LabVector.X + 98) * 16, NPC.Center.Y);
                                    if (NPC.DistanceSQ(v2) < 10 * 10)
                                    {
                                        JawOpen = false;
                                        TimerRand++;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        NPC.Move(v2, 5, 30);
                                        if (AITimer++ % (Phase == 0 ? 20 : 15) == 0)
                                        {
                                            GlowActive2 = true;
                                            GlowTimer2 = 0;
                                            for (int i = 0; i < 2; i++)
                                                NPC.Shoot(MouthOrigin - new Vector2(0, 24), ModContent.ProjectileType<MACE_Miniblast>(), NPC.damage, new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-7, -4)), false, SoundID.Item73);
                                        }
                                    }
                                    break;
                                case 2:
                                    if (AITimer++ < 30)
                                        NPC.velocity.X *= 0.9f;
                                    else
                                    {
                                        NPC.MoveToVector2(CraneOrigin, 10);
                                        if (NPC.DistanceSQ(CraneOrigin) < 6 * 6)
                                        {
                                            NPC.velocity *= 0;
                                            TimerRand = 20;
                                            AITimer = 0;
                                            TimerRand2++;
                                            NPC.netUpdate = true;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case 3:
                            GlowTimer++;
                            if (AITimer++ >= TimerRand)
                            {
                                TimerRand -= 2;
                                GlowActive = true;
                                GlowTimer = 0;
                                NPC.Shoot(EyeOrigin, ModContent.ProjectileType<MACE_Laser>(), NPC.damage, RedeHelper.PolarVector(9, (player.Center - EyeOrigin).ToRotation()), false, SoundID.Item125);
                                AITimer = 0;
                            }
                            TimerRand = MathHelper.Clamp(TimerRand, 2, 20);
                            if (TimerRand <= (Phase == 0 ? 2 : 4))
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                if (Phase == 0)
                                    TimerRand2 = 1;
                                else
                                    TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 4:
                            GlowTimer++;
                            if (AITimer++ % 4 == 0)
                            {
                                GlowActive = true;
                                GlowTimer = 0;
                                NPC.Shoot(EyeOrigin, ModContent.ProjectileType<MACE_Laser>(), NPC.damage, RedeHelper.PolarVector(9, (player.Center - EyeOrigin).ToRotation()), false, SoundID.Item125);
                            }
                            if (AITimer >= 30)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 5:
                            if (AITimer++ == 60)
                                JawOpen = true;

                            if (AITimer == 100)
                            {
                                NPC.Shoot(MouthOrigin, ModContent.ProjectileType<MACE_FireBlast>(), (int)(NPC.damage * 1.5f), Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/EnergyChargeSound2", NPC.whoAmI);
                            }
                            if (AITimer >= 100 && AITimer <= 290)
                            {
                                TimerRand -= (float)Math.PI / 120;
                                if (TimerRand <= 0) TimerRand = MathHelper.PiOver2;
                                float timer = TimerRand;
                                Terraria.Graphics.Effects.Filters.Scene.Activate("MoR:Shockwave", NPC.Center)?.GetShader().UseProgress(timer).UseOpacity(100f * (1 - timer / 2f)).UseColor(1, 1, 6).UseTargetPosition(MouthOrigin);
                            }
                            else
                                Terraria.Graphics.Effects.Filters.Scene["MoR:Shockwave"].Deactivate();
                            if (AITimer == 290)
                                player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 30;

                            if (AITimer == 320)
                                JawOpen = false;

                            if (AITimer >= 380)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 1;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.HeadPhase:
                    switch (TimerRand2)
                    {
                        case 0:
                            NPC.MoveToVector2(CraneOrigin, 10);
                            if (NPC.DistanceSQ(CraneOrigin) < 6 * 6)
                            {
                                NPC.velocity *= 0;
                                TimerRand = 0;
                                AITimer = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            if (AITimer++ == 60)
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 16), ModContent.ProjectileType<MACE_Gemshot>(), NPC.damage, RedeHelper.PolarVector(-12, (player.Center - NPC.Center).ToRotation()), false, SoundID.Item125);
                            GlowTimer++;
                            if (AITimer >= 60 && AITimer % 15 == 0)
                            {
                                GlowActive = true;
                                GlowTimer = 0;
                                NPC.Shoot(EyeOrigin, ModContent.ProjectileType<MACE_Laser>(), NPC.damage, RedeHelper.PolarVector(9, (player.Center - EyeOrigin).ToRotation()), false, SoundID.Item125);
                            }
                            if (AITimer >= 180)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            if (AITimer++ == 60)
                            {
                                GlowActive = true;
                                GlowTimer = 0;
                                for (int i = 0; i < 6; i++)
                                    NPC.Shoot(EyeOrigin, ModContent.ProjectileType<MACE_Laser>(), NPC.damage, RedeHelper.PolarVector(8, MathHelper.ToRadians(60) * i), false, SoundID.Item125);
                            }
                            if (AITimer == 120)
                            {
                                GlowActive = true;
                                GlowTimer = 0;
                                for (int i = 0; i < 12; i++)
                                    NPC.Shoot(EyeOrigin, ModContent.ProjectileType<MACE_Laser>(), NPC.damage, RedeHelper.PolarVector(7, MathHelper.ToRadians(30) * i), false, SoundID.Item125);
                            }
                            if (AITimer == 180)
                            {
                                GlowActive = true;
                                GlowTimer = 0;
                                for (int i = 0; i < 18; i++)
                                    NPC.Shoot(EyeOrigin, ModContent.ProjectileType<MACE_Laser>(), NPC.damage, RedeHelper.PolarVector(6, MathHelper.ToRadians(20) * i), false, SoundID.Item125);
                            }
                            if (AITimer >= 220)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 3:
                            if (AITimer++ < 120)
                            {
                                if (player.Center.X > NPC.Center.X)
                                    NPC.velocity.X += 0.2f;
                                else
                                    NPC.velocity.X -= 0.2f;
                            }
                            if (AITimer == 120)
                                NPC.velocity.X = 0;

                            if (AITimer > 120)
                            {
                                NPC.velocity.Y += 0.2f;
                                if (NPC.collideY || NPC.velocity.Y == 0)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/EarthBoom2"), NPC.position);
                                    player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 20;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            int proj = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), new Vector2(NPC.Center.X, NPC.Center.Y + 68),
                                                RedeHelper.PolarVector(6, MathHelper.ToRadians(45) * i), ProjectileID.MartianTurretBolt, NPC.damage / 4, 0, Main.myPlayer);
                                            Main.projectile[proj].tileCollide = false;
                                            Main.projectile[proj].timeLeft = 200;
                                            Main.projectile[proj].netUpdate2 = true;
                                        }
                                        for (int i = 0; i < 18; i++)
                                        {
                                            int proj = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), new Vector2(NPC.Center.X, NPC.Center.Y + 68),
                                                RedeHelper.PolarVector(5, MathHelper.ToRadians(20) * i), ProjectileID.MartianTurretBolt, NPC.damage / 4, 0, Main.myPlayer);
                                            Main.projectile[proj].tileCollide = false;
                                            Main.projectile[proj].timeLeft = 200;
                                            Main.projectile[proj].netUpdate2 = true;
                                        }
                                    }
                                    for (int i = 0; i < 2; i++)
                                        NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y + 38), ModContent.ProjectileType<MACE_GroundShock>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0), "", i);

                                    AITimer = 0;
                                    TimerRand = 0;
                                    TimerRand2++;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 4:
                            if (AITimer++ >= 80)
                            {
                                NPC.MoveToVector2(CraneOrigin, 10);
                                if (NPC.DistanceSQ(CraneOrigin) < 6 * 6)
                                {
                                    NPC.velocity *= 0;
                                    TimerRand = 0;
                                    AITimer = 0;
                                    TimerRand2++;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 5:
                            if (AITimer++ % 10 == 0 && AITimer <= 50)
                            {
                                TimerRand += 3;
                                for (int i = -1; i < 2; i += 2)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 16), ModContent.ProjectileType<MACE_Orb>(), NPC.damage, new Vector2(TimerRand * i, 0f), false, SoundID.Item61);
                                }
                            }
                            if (AITimer >= 120)
                            {
                                AITimer = 0;
                                TimerRand = Main.rand.Next(2);
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 6:
                            if (TimerRand == 0)
                                NPC.velocity.X -= 0.1f;
                            else
                                NPC.velocity.X += 0.1f;
                            if (NPC.collideX || NPC.velocity.X == 0)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/EarthBoom2"), NPC.position);
                                player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 20;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), new Vector2(TimerRand == 0 ? NPC.position.X : NPC.Right.X, NPC.Center.Y), RedeHelper.PolarVector(8, MathHelper.ToRadians(24) * i), ProjectileID.MartianTurretBolt, NPC.damage / 4, 0, Main.myPlayer);
                                        Main.projectile[proj].tileCollide = false;
                                        Main.projectile[proj].timeLeft = 200;
                                        Main.projectile[proj].netUpdate2 = true;
                                    }
                                    for (int i = 0; i < 20; i++)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), new Vector2(TimerRand == 0 ? NPC.position.X : NPC.Right.X, NPC.Center.Y), RedeHelper.PolarVector(7, MathHelper.ToRadians(18) * i), ProjectileID.MartianTurretBolt, NPC.damage / 4, 0, Main.myPlayer);
                                        Main.projectile[proj].tileCollide = false;
                                        Main.projectile[proj].timeLeft = 200;
                                        Main.projectile[proj].netUpdate2 = true;
                                    }
                                }
                                for (int i = 0; i < 6; i++)
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<MACE_Scrap>(), NPC.damage, RedeHelper.Spread(10), false, SoundID.Item1.WithVolume(0));

                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.PhaseChange:
                    switch (Phase)
                    {
                        case 0:
                            Terraria.Graphics.Effects.Filters.Scene["MoR:Shockwave"].Deactivate();
                            JawOpen = false;
                            AITimer = 0;
                            TimerRand = 0;
                            TimerRand2 = -1;
                            Phase = 1;
                            SoundEngine.PlaySound(SoundID.NPCDeath14, NPC.position);
                            for (int i = 0; i < 10; i++)
                            {
                                int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 2f);
                                Main.dust[dustIndex].velocity *= 3f;
                            }
                            for (int k = 0; k < 6; k++)
                                NPC.Shoot(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), ModContent.ProjectileType<MACE_Scrap>(), NPC.damage / 2, RedeHelper.Spread(4), false, SoundID.Item1.WithVolume(0));

                            AIState = ActionState.JawPhase;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            Terraria.Graphics.Effects.Filters.Scene["MoR:Shockwave"].Deactivate();
                            JawOpen = false;
                            AITimer = 0;
                            TimerRand = 0;
                            TimerRand2 = 0;
                            Phase = 2;
                            NPC.height = 126;

                            SoundEngine.PlaySound(SoundID.NPCDeath14, NPC.position);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DistortedRoar").WithVolume(.5f), NPC.position);

                            if (Main.netMode != NetmodeID.Server)
                                Gore.NewGore(new Vector2(NPC.position.X, NPC.Center.Y + 18), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreJaw").Type);

                            for (int i = 0; i < 20; i++)
                            {
                                int dustIndex = Dust.NewDust(NPC.position + new Vector2(0, 110) + NPC.velocity, NPC.width, 54, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 2f);
                                Main.dust[dustIndex].velocity *= 3f;
                            }
                            for (int k = 0; k < 6; k++)
                                NPC.Shoot(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(110, 164))), ModContent.ProjectileType<MACE_Scrap>(), NPC.damage / 2, RedeHelper.Spread(4), false, SoundID.Item1.WithVolume(0));
                            AIState = ActionState.HeadPhase;
                            NPC.netUpdate = true;
                            break;
                    }
                    break;
            }
        }
        private bool GlowActive;
        private int GlowTimer;
        private bool GlowActive2;
        private int GlowTimer2;
        private bool JawOpen;
        public override void FindFrame(int frameHeight)
        {
            if (GlowActive)
            {
                if (GlowTimer++ > 60)
                {
                    GlowActive = false;
                    GlowTimer = 0;
                }
            }
            if (GlowActive2)
            {
                if (GlowTimer2++ > 60)
                {
                    GlowActive2 = false;
                    GlowTimer2 = 0;
                }
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (JawOpen)
            {
                if (JawCenter.Y < 18)
                    JawCenter.Y += 1f;
            }
            else
            {
                if (JawCenter.Y >= 0)
                    JawCenter.Y -= 1f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D trolleyTex = ModContent.Request<Texture2D>("Redemption/NPCs/Lab/MACE/CraneTrolley").Value;
            Texture2D jawTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Jaw").Value;
            Vector2 drawCenter = new(NPC.Center.X, NPC.Center.Y - 18);
            if (Phase == 2)
                drawCenter = NPC.Center;
            Color heat = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(255, 100, 0), Color.Transparent, new Color(255, 100, 0));

            Vector2 drawCenterTrolley = new(drawCenter.X, CraneOrigin.Y - 8);
            Rectangle rect = new(0, 0, trolleyTex.Width, trolleyTex.Height);
            Main.spriteBatch.Draw(trolleyTex, drawCenterTrolley - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), 0, new Vector2(trolleyTex.Width / 2f, trolleyTex.Height / 2f), NPC.scale, SpriteEffects.None, 0);

            if (Phase < 2)
            {
                Vector2 drawCenterJaw = new(drawCenter.X - 1, drawCenter.Y - 1);
                Rectangle rect2 = new(0, 0, jawTex.Width, jawTex.Height);
                Main.spriteBatch.Draw(jawTex, drawCenterJaw + JawCenter - screenPos, new Rectangle?(rect2), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(jawTex.Width / 2f, jawTex.Height / 2f - 60), NPC.scale, SpriteEffects.None, 0);

                float heatOpacity = (float)NPC.GetGlobalNPC<GuardNPC>().GuardPoints / GuardPointMax;
                Main.spriteBatch.Draw(jawTex, drawCenterJaw + JawCenter - screenPos, new Rectangle?(rect2), NPC.GetAlpha(heat) * MathHelper.Lerp(2, 0, heatOpacity), NPC.rotation, new Vector2(jawTex.Width / 2f, jawTex.Height / 2f - 60), NPC.scale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, drawCenter - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(glowTex, drawCenter - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/MACEEyeFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = NPC.Center - screenPos + new Vector2(-22, 4);
            if (Phase == 2)
                position = NPC.Center - screenPos + new Vector2(-22, 22);
            Color colour = Color.Lerp(Color.DarkGreen, Color.White, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (GlowActive)
            {
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour, NPC.rotation, origin, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 0.4f, NPC.rotation, origin, 2f, SpriteEffects.None, 0);
            }
            Texture2D mouthGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect2 = new(0, 0, mouthGlow.Width, mouthGlow.Height);
            Vector2 origin2 = new(mouthGlow.Width / 2, mouthGlow.Height / 2);
            Vector2 position2 = new Vector2(NPC.Center.X - 1, NPC.Center.Y + 38) - screenPos;
            Color colour2 = Color.Lerp(Color.Orange, Color.Orange, 1f / GlowTimer2 * 10f) * (1f / GlowTimer2 * 10f);
            if (GlowActive2)
            {
                spriteBatch.Draw(mouthGlow, position2, new Rectangle?(rect2), colour2 * 0.8f, NPC.rotation, origin2, 1f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => Phase == 2 && (TimerRand2 == 3 || TimerRand2 == 6);
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.alpha += 5;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
    }
}