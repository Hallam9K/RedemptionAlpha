using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Redemption.Dusts;
using Terraria.GameContent;
using Redemption.Biomes;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Items.Usable;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Lore;
using Terraria.Audio;
using Terraria.GameContent.Events;
using ReLogic.Content;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Armor.Vanity;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PostML.Melee;

namespace Redemption.NPCs.Bosses.PatientZero
{
    [AutoloadBossHead]
    public class PZ : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Bosses/PatientZero/PZ_Eyelid";
        public enum ActionState
        {
            Begin,
            LaserAttacks,
            MiscAttacks,
            PhaseChange,
            Death
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
            DisplayName.SetDefault("Patient Zero");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 80;
            NPC.friendly = false;
            NPC.damage = 120;
            NPC.defense = 80;
            NPC.lifeMax = 198000;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            NPC.BossBar = ModContent.GetInstance<PZHealthBar>();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic2");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("An unfortunate scientist, mutilated and disfigured by the Xenomite infection. This specimen was Kari Johannson, the father of all T-Bots and patient zero of the xenomite infection. He's been stuck for 50 years, conscious and aware of the situation around him... God, that must be tormentous.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0 && AIState == ActionState.Death && AITimer >= 240)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2f);
                    Main.dust[dustIndex].velocity *= 2f;
                }

                for (int i = 0; i < 8; i++)
                {
                    for (int k = 0; k < 3; k++)
                        Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 90 + Main.rand.Next(0, 180), NPC.Center.Y - 90 + Main.rand.Next(0, 180)), NPC.velocity, ModContent.Find<ModGore>("Redemption/PZGoreFlesh" + (k + 1)).Type);
                }
                for (int i = 0; i < 6; i++)
                {
                    for (int k = 0; k < 3; k++)
                        Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 90 + Main.rand.Next(0, 180), NPC.Center.Y - 90 + Main.rand.Next(0, 180)), NPC.velocity, ModContent.Find<ModGore>("Redemption/PZGoreShard" + (k + 1)).Type);
                }
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 90 + Main.rand.Next(0, 180), NPC.Center.Y - 90 + Main.rand.Next(0, 180)), NPC.velocity, ModContent.Find<ModGore>("Redemption/PZGoreGoo1").Type);
                for (int i = 0; i < 8; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 90 + Main.rand.Next(0, 180), NPC.Center.Y - 90 + Main.rand.Next(0, 180)), NPC.velocity, ModContent.Find<ModGore>("Redemption/PZGoreGoop").Type);
                Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.position.X, NPC.Center.Y + 26), NPC.velocity, ModContent.Find<ModGore>("Redemption/PZGoreKari").Type);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/PZGoreEye").Type);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/PZGoreGooEye").Type);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 1f);
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[5])
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel6>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedPZ, -1);
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ProjectileID.LastPrismLaser)
                damage /= 3;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk7>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk7_1>()));

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PZBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PZTrophy>(), 10));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PZMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<PZGauntlet>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MedicKit>()));
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(OpenEye);
                writer.Write(Randomize);
                writer.Write(ID);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                OpenEye = reader.ReadBoolean();
                Randomize = reader.ReadBoolean();
                Phase = reader.ReadInt32();
                ID = reader.ReadInt32();
            }
        }
        public bool OpenEye;
        public bool Randomize;
        public int Phase;
        public int ID;
        public List<int> LaserAttackList = new() { 0, 1, 2, 3, 4, 5, 6 };
        public List<int> MiscAttackList = new() { 0, 1, 2, 3, 4, 5, 6 };
        public List<int> CopyLaserList = null;
        public List<int> CopyMiscList = null;

        public List<float> EyeTeleList = new()
        {
            MathHelper.ToRadians(22.5f),
            MathHelper.ToRadians(67.5f),
            MathHelper.ToRadians(112.5f),
            MathHelper.ToRadians(157.5f),
            MathHelper.ToRadians(202.5f),
            MathHelper.ToRadians(247.5f),
            MathHelper.ToRadians(292.5f),
            MathHelper.ToRadians(337.5f)
        };
        public List<float> CopyEyeList = null;
        void EyeChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyEyeList == null || CopyEyeList.Count == 0)
                    CopyEyeList = new List<float>(EyeTeleList);
                TimerRand = CopyEyeList[Main.rand.Next(0, CopyEyeList.Count)];
                CopyEyeList.Remove(TimerRand);
                NPC.netUpdate = true;
                attempts++;
            }
        }
        void AttackChoice(bool Misc = false)
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyLaserList == null || CopyLaserList.Count == 0)
                    CopyLaserList = new List<int>(LaserAttackList);
                if (CopyMiscList == null || CopyMiscList.Count == 0)
                    CopyMiscList = new List<int>(MiscAttackList);
                if (Misc)
                {
                    ID = CopyMiscList[Main.rand.Next(0, CopyMiscList.Count)];
                    CopyMiscList.Remove(ID);
                }
                else
                {
                    ID = CopyLaserList[Main.rand.Next(0, CopyLaserList.Count)];
                    CopyLaserList.Remove(ID);
                }
                NPC.netUpdate = true;

                if (ID >= 3 && Phase < 1)
                    continue;
                if (ID >= 6 && Phase < 2)
                    continue;

                attempts++;
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            DespawnHandler();

            if (!player.active || player.dead)
                return;

            if (AIState != ActionState.Death && !NPC.AnyNPCs(ModContent.NPCType<PZ_Kari>()))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 3, (int)NPC.Center.Y + 149, ModContent.NPCType<PZ_Kari>(), NPC.whoAmI);

            if (Phase == 0 && NPC.life <= (int)(NPC.lifeMax * 0.75f))
                PhaseSwap();
            else if (Phase == 1 && NPC.life <= NPC.lifeMax / 2)
                PhaseSwap();
            else if (Phase == 2 && NPC.life <= (int)(NPC.lifeMax * 0.35f))
                PhaseSwap();
            else if (AIState != ActionState.Death && Phase >= 3 && NPC.life <= (int)(NPC.lifeMax * 0.1f))
                PhaseSwap();

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ >= 978)
                    {
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Kari Johansson", 60, 90, 0.8f, 0, Color.Green, "Patient Zero");
                        AITimer = 0;
                        OpenEye = true;
                        NPC.dontTakeDamage = false;
                        AIState = ActionState.LaserAttacks;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
                case ActionState.LaserAttacks:
                    OpenEye = true;
                    switch (ID)
                    {
                        #region Phase 1
                        case 0:
                            NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            if (AITimer++ >= 60)
                                NPC.alpha += 10;
                            if (AITimer == 60)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PatientZeroLaser, NPC.position);
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), (int)(NPC.damage * 1.2f), RedeHelper.PolarVector(10, MathHelper.ToRadians(90) * i), false, SoundID.Item1, NPC.whoAmI, -1);
                            }
                            if (AITimer >= 150 && AITimer % 3 == 0 && AITimer <= 156)
                            {
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Tele>(), 0, RedeHelper.PolarVector(8, (MathHelper.ToRadians(90) * i) + (TimerRand - MathHelper.ToRadians(25)) + MathHelper.PiOver4), false, SoundID.Item1);

                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<CausticTear>(), (int)(NPC.damage * 0.85f), RedeHelper.PolarVector(8, (MathHelper.ToRadians(90) * i) + (TimerRand - MathHelper.ToRadians(25)) + MathHelper.PiOver4), true, SoundID.Item42);

                                TimerRand += MathHelper.ToRadians(25);
                                NPC.netUpdate = true;
                            }
                            if (AITimer > 240)
                            {
                                NextAttack();
                            }
                            break;
                        case 1:
                            NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            if (AITimer++ >= 60)
                                NPC.alpha += 10;
                            if (AITimer == 60)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PatientZeroLaser, NPC.position);
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), (int)(NPC.damage * 1.2f), RedeHelper.PolarVector(10, (MathHelper.ToRadians(90) * i) + MathHelper.PiOver4), false, SoundID.Item1, NPC.whoAmI, -1);
                            }
                            if (AITimer >= 150 && AITimer % 3 == 0 && AITimer <= 156)
                            {
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Tele>(), 0, RedeHelper.PolarVector(8, (MathHelper.ToRadians(90) * i) + TimerRand - MathHelper.ToRadians(25)), false, SoundID.Item1);

                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<CausticTear>(), (int)(NPC.damage * 0.85f), RedeHelper.PolarVector(8, (MathHelper.ToRadians(90) * i) + TimerRand - MathHelper.ToRadians(25)), true, SoundID.Item42);

                                TimerRand += MathHelper.ToRadians(25);
                                NPC.netUpdate = true;
                            }
                            if (AITimer > 240)
                            {
                                NextAttack();
                            }
                            break;
                        case 2:
                            if (AITimer++ < 60)
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60);
                            if (AITimer >= 30)
                                NPC.alpha += 10;
                            if (AITimer == 60)
                            {
                                NPC.rotation = 0;
                                TimerRand = 0.005f;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PatientZeroLaser, NPC.position);
                                for (int i = 0; i < 2; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), NPC.damage, RedeHelper.PolarVector(10, NPC.rotation), false, SoundID.Item1, NPC.whoAmI, i);
                                for (int i = 0; i < 2; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), NPC.damage, RedeHelper.PolarVector(10, NPC.rotation), false, SoundID.Item1, NPC.whoAmI, 6 + i);
                            }
                            if (AITimer >= 140 && AITimer <= 192)
                            {
                                TimerRand *= 1.02f;
                                NPC.rotation += TimerRand;
                            }
                            else if (AITimer > 192)
                            {
                                TimerRand *= 0.98f;
                                NPC.rotation += TimerRand;
                            }

                            if (AITimer >= 140 && AITimer % 24 == 0 && AITimer <= 280)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Tele>(), 0, RedeHelper.PolarVector(8, NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation()), false, SoundID.Item1);

                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<CausticTear>(), (int)(NPC.damage * 0.85f), RedeHelper.PolarVector(8, NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation()), true, SoundID.Item42);
                            }
                            if (AITimer > 300)
                            {
                                NextAttack();
                            }
                            break;
                        #endregion
                        #region Phase 2
                        case 3:
                            if (AITimer++ >= 60)
                                NPC.alpha += 10;
                            else
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            if (AITimer == 60)
                            {
                                TimerRand2 = 0.005f;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PatientZeroLaserL, NPC.position);
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(90) * i), false, SoundID.Item1, NPC.whoAmI, i);
                            }
                            if (AITimer >= 140 && AITimer <= 300)
                            {
                                TimerRand2 *= 1.02f;
                                NPC.rotation += TimerRand2;
                            }
                            else if (AITimer > 300)
                            {
                                TimerRand2 *= 0.96f;
                                NPC.rotation += TimerRand2;
                            }
                            TimerRand2 = MathHelper.Clamp(TimerRand2, 0, 0.022f);
                            if (AITimer >= 150 && AITimer % 20 == 0 && AITimer <= 320)
                            {
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Tele>(), 0, RedeHelper.PolarVector(6, (MathHelper.ToRadians(90) * i) + (TimerRand - MathHelper.ToRadians(40)) + MathHelper.PiOver4), false, SoundID.Item1);

                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<CausticTear>(), (int)(NPC.damage * 0.85f), RedeHelper.PolarVector(6, (MathHelper.ToRadians(90) * i) + (TimerRand - MathHelper.ToRadians(40)) + MathHelper.PiOver4), true, SoundID.Item42);

                                TimerRand += MathHelper.ToRadians(40);
                                NPC.netUpdate = true;
                            }
                            if (AITimer > 340)
                            {
                                NextAttack();
                            }
                            break;
                        case 4:
                            if (AITimer++ >= 60)
                                NPC.alpha += 10;
                            else
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            if (AITimer == 60)
                            {
                                TimerRand = Main.rand.Next(2);
                                TimerRand2 = 0.005f;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PatientZeroLaserL, NPC.position);
                                for (int i = 0; i < 2; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(90) * i), false, SoundID.Item1, NPC.whoAmI, i);
                            }
                            if (AITimer >= 140 && AITimer <= 380)
                            {
                                TimerRand2 += 0.0002f;
                                if (TimerRand == 0)
                                    NPC.rotation += TimerRand2;
                                else
                                    NPC.rotation -= TimerRand2;
                            }
                            else if (AITimer > 380)
                            {
                                TimerRand2 -= 0.0002f;
                                if (TimerRand == 0)
                                    NPC.rotation += TimerRand2;
                                else
                                    NPC.rotation -= TimerRand2;
                            }
                            TimerRand2 = MathHelper.Clamp(TimerRand2, -0.007f, 0.007f);
                            if (AITimer >= 200 && AITimer % 15 == 0 && AITimer <= 660)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Tele>(), 0, RedeHelper.PolarVector(8, NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation()), false, SoundID.Item1);

                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<TearOfPain>(), (int)(NPC.damage * 0.95f), RedeHelper.PolarVector(13, NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation()), true, SoundID.Item42);

                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Tele>(), 0, RedeHelper.PolarVector(-8, NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation()), false, SoundID.Item1);

                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<TearOfPain>(), (int)(NPC.damage * 0.95f), RedeHelper.PolarVector(-13, NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation()), true, SoundID.Item42);
                            }
                            if (AITimer > 680)
                            {
                                NextAttack();
                            }
                            break;
                        case 5:
                            NPC.alpha += 10;
                            if (AITimer++ < 60)
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            if (AITimer == 60)
                            {
                                NPC.rotation = MathHelper.PiOver4;
                                TimerRand2 = 0.005f;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PatientZeroLaser, NPC.position);
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(90) * i), false, SoundID.Item1, NPC.whoAmI, i);
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(90) * i), false, SoundID.Item1, NPC.whoAmI, 6 + i);
                            }
                            if (AITimer >= 140 && AITimer <= 164)
                            {
                                TimerRand2 *= 1.02f;
                                NPC.rotation += TimerRand2;
                            }
                            else if (AITimer > 164)
                            {
                                TimerRand2 *= 0.98f;
                                NPC.rotation += TimerRand2;
                            }
                            TimerRand2 = MathHelper.Clamp(TimerRand2, 0, 0.02f);
                            if (AITimer == 104)
                            {
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Tele>(), 0, RedeHelper.PolarVector(9, MathHelper.ToRadians(90) * i), false, SoundID.Item1);
                            }
                            if (AITimer == 144)
                            {
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<TearOfPainBall>(), NPC.damage, RedeHelper.PolarVector(13, MathHelper.ToRadians(90) * i), true, SoundID.Item42);
                            }
                            if (AITimer > 260)
                            {
                                NextAttack();
                            }
                            break;
                        #endregion
                        #region Phase 3
                        case 6:
                            if (AITimer < 140)
                                NPC.alpha -= 10;
                            else
                                NPC.alpha += 10;
                            if (AITimer++ == 0)
                                CopyEyeList = null;
                            if (AITimer >= 20 && AITimer % 20 == 0 && AITimer <= 100)
                            {
                                EyeChoice();
                            }
                            if (AITimer >= 40 && AITimer % 20 == 0 && AITimer <= 120)
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<EyeRadius_Tele2>(), 0, RedeHelper.PolarVector(2, NPC.rotation), false, SoundID.Item1, NPC.whoAmI);
                            if (AITimer >= 20 && AITimer <= 120)
                                NPC.rotation.SlowRotation(TimerRand, (float)Math.PI / 20);

                            if (AITimer == 140)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.PatientZeroLaserL, NPC.position);
                                for (int i = 0; i < 8; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Laser>(), NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(45) * i), false, SoundID.Item1, NPC.whoAmI, -1);
                            }
                            if (AITimer > 480)
                            {
                                NextAttack();
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.MiscAttacks:
                    OpenEye = true;
                    switch (ID)
                    {
                        #region Phase 1
                        case 0:
                            if (AITimer > 20)
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            NPC.alpha -= 10;
                            if (AITimer++ >= 60 && AITimer % 30 == 0 && AITimer <= 160)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer > 180)
                            {
                                NextAttack();
                            }
                            break;
                        case 1:
                            if (AITimer++ == 0)
                                TimerRand = 0.01f;
                            NPC.rotation += TimerRand;
                            NPC.alpha -= 10;
                            if (AITimer >= 60 && (TimerRand >= 0.1f ? AITimer % 22 == 0 : AITimer % 32 == 0) && AITimer <= 170)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer >= 160)
                                TimerRand *= 0.92f;
                            else
                                TimerRand *= 1.02f;
                            TimerRand = MathHelper.Clamp(TimerRand, 0, 0.2f);
                            if (AITimer > 220)
                            {
                                NextAttack();
                            }
                            break;
                        case 2:
                            if (AITimer++ > 20)
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            NPC.alpha -= 10;
                            if (AITimer == 60)
                            {
                                for (int i = -1; i < 2; i += 2)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation + (MathHelper.ToRadians(35) * i)), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer == 110)
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation), true, CustomSounds.MACEProjectLaunch);
                            if (AITimer > 180)
                            {
                                ID = -1;
                                NextAttack();
                            }
                            break;
                        #endregion
                        #region Phase 2
                        case 3:
                            if (AITimer > 20)
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            NPC.alpha -= 10;
                            if (AITimer++ >= 60 && AITimer % 20 == 0 && AITimer <= 160)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer > 180)
                            {
                                NextAttack();
                            }
                            break;
                        case 4:
                            if (AITimer++ > 20)
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            NPC.alpha -= 10;
                            if (AITimer == 60 || AITimer == 140)
                            {
                                for (int i = 0; i < 3; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation + ((MathHelper.ToRadians(30) * i) - MathHelper.ToRadians(30))), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer == 100)
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation), true, CustomSounds.MACEProjectLaunch);
                            if (AITimer > 180)
                            {
                                NextAttack();
                            }
                            break;
                        case 5:
                            if (AITimer++ == 0)
                                TimerRand = 0.01f;
                            NPC.rotation -= TimerRand;
                            NPC.alpha -= 10;
                            if (AITimer >= 60 && AITimer % 22 == 0 && AITimer <= 170)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer >= 160)
                                TimerRand *= 0.92f;
                            else
                                TimerRand *= 1.02f;
                            TimerRand = MathHelper.Clamp(TimerRand, 0, 0.1f);
                            if (AITimer > 220)
                            {
                                Randomize = true;
                                NextAttack();
                            }
                            break;
                        #endregion
                        #region Phase 3
                        case 6:
                            if (AITimer++ > 20)
                                NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 30);
                            NPC.alpha -= 10;
                            if (AITimer >= 60 && AITimer % 10 == 0 && AITimer <= 90)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation + Main.rand.NextFloat(-0.1f, 0.1f)), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer >= 140 && AITimer % 10 == 0 && AITimer <= 170)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Blast>(), NPC.damage, RedeHelper.PolarVector(7, NPC.rotation + Main.rand.NextFloat(-0.1f, 0.1f)), true, CustomSounds.MACEProjectLaunch);
                            }
                            if (AITimer > 220)
                            {
                                Randomize = true;
                                NextAttack();
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.PhaseChange:
                    NPC.alpha -= 10;
                    OpenEye = false;
                    TimerRand = 0;
                    ID = 0;
                    if (AITimer == -1)
                    {
                        CopyLaserList = null;
                        CopyMiscList = null;
                        AIState = ActionState.LaserAttacks;
                        AITimer = 0;
                        Phase++;
                        switch (Phase)
                        {
                            case 1:
                                ID = 3;
                                Randomize = false;
                                break;
                            case 2:
                                ID = 6;
                                Randomize = false;
                                break;
                            case 3:
                                ID = 6;
                                break;

                        }
                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
                case ActionState.Death:
                    NPC.alpha -= 10;
                    OpenEye = true;
                    switch (ID)
                    {
                        case 0:
                            ID = 1;
                            TimerRand = 3;
                            NPC.dontTakeDamage = true;
                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                            break;
                        case 1:
                            player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                            player.RedemptionScreen().lockScreen = true;
                            player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity, 5);
                            player.RedemptionScreen().TimedZoom(new Vector2(1.4f, 1.4f), 100, 100);
                            NPC.LockMoveRadius(player);
                            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1f).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);

                            TimerRand *= 0.98f;
                            NPC.rotation.SlowRotation(NPC.DirectionTo(Main.player[RedeHelper.GetNearestAlivePlayer(NPC)].Center).ToRotation(), (float)Math.PI / 90);
                            NPC.rotation += Main.rand.NextFloat(-TimerRand, TimerRand);
                            if (AITimer++ >= 180)
                                MoonlordDeathDrama.RequestLight(1f, NPC.Center);
                            if (AITimer >= 240)
                            {
                                NPC.dontTakeDamage = false;
                                player.ApplyDamageToNPC(NPC, 99999, 0, 0, false);
                                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                            }
                            break;
                    }
                    break;
            }
            NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 255);
        }
        private void NextAttack()
        {
            AITimer = 0;
            TimerRand = 0;
            TimerRand2 = 0;
            if (AIState is ActionState.LaserAttacks)
            {
                if (Randomize)
                    AttackChoice(true);
                AIState = ActionState.MiscAttacks;
            }
            else if (AIState is ActionState.MiscAttacks)
            {
                if (Randomize)
                    AttackChoice();
                else
                    ID++;

                AIState = ActionState.LaserAttacks;
            }
            NPC.netUpdate = true;
        }
        private void PhaseSwap()
        {
            AIState = ActionState.PhaseChange;
            NPC.dontTakeDamage = true;
            NPC.netUpdate = true;
            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }
        private int BodyFrame;
        private int KariFrame;
        private int GrooveTimer;
        public override void FindFrame(int frameHeight)
        {
            if (AIState > ActionState.Begin && Main.netMode == NetmodeID.SinglePlayer)
            {
                if (GrooveTimer == 0)
                {
                    NPC.scale += 0.04f;
                    if (NPC.scale > 1.04f)
                        GrooveTimer = 1;
                }
                else if (GrooveTimer == 1)
                {
                    NPC.scale -= 0.04f;
                    if (NPC.scale <= 1f)
                        GrooveTimer = 2;
                }
                else if (GrooveTimer++ >= 27)
                    GrooveTimer = 0;
            }
            NPC.frameCounter++;
            if (NPC.frameCounter % 10 == 0)
            {
                BodyFrame++;
                if (BodyFrame > 7)
                    BodyFrame = 0;
            }
            if (NPC.frameCounter % 20 == 0)
            {
                KariFrame++;
                if (KariFrame > 3)
                    KariFrame = 0;
            }
            if (!NPC.IsABestiaryIconDummy)
            {
                if (OpenEye && NPC.frame.Y != 0)
                {
                    if (NPC.frameCounter++ >= 20)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
                else if (!OpenEye && NPC.frame.Y != 2 * frameHeight)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 20)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
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
                    NPC.active = false;
                }
            }
        }
        public override bool CheckDead()
        {
            if (AIState == ActionState.Death && AITimer >= 240)
                return true;

            NPC.life = 1;
            return false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D BodyAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Body").Value;
            Texture2D BodyGlowAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Body_Glow").Value;
            Texture2D EyeAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Pupil").Value;
            Texture2D KariAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Kari").Value;
            Texture2D SlimeAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Slime").Value;

            Vector2 drawCenterC = new(NPC.Center.X + 5, NPC.Center.Y + 7);
            spriteBatch.Draw(SlimeAni, drawCenterC - screenPos, new Rectangle?(new Rectangle(0, 0, SlimeAni.Width, SlimeAni.Height)), drawColor, 0, new Vector2(SlimeAni.Width / 2f, SlimeAni.Height / 2f), 1, SpriteEffects.None, 0f);

            Vector2 drawCenterB = new(NPC.Center.X - 2, NPC.Center.Y + 14);
            int widthB = BodyAni.Height / 8;
            int yB = widthB * BodyFrame;
            spriteBatch.Draw(BodyAni, drawCenterB - screenPos, new Rectangle?(new Rectangle(0, yB, BodyAni.Width, widthB)), drawColor, 0, new Vector2(BodyAni.Width / 2f, widthB / 2f), NPC.scale * 2, SpriteEffects.None, 0f);
            spriteBatch.Draw(BodyGlowAni, drawCenterB - screenPos, new Rectangle?(new Rectangle(0, yB, BodyAni.Width, widthB)), new Color(255, 255, 255, NPC.Opacity), 0, new Vector2(BodyAni.Width / 2f, widthB / 2f), NPC.scale * 2, SpriteEffects.None, 0f);

            if (AIState != ActionState.PhaseChange)
            {
                Vector2 drawCenterD = new(NPC.Center.X + 1, NPC.Center.Y + 123);
                int widthD = KariAni.Height / 4;
                int yD = widthD * KariFrame;
                spriteBatch.Draw(KariAni, drawCenterD - screenPos, new Rectangle?(new Rectangle(0, yD, KariAni.Width, widthD)), drawColor, 0, new Vector2(KariAni.Width / 2f, widthD / 2f), NPC.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(EyeAni, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, 0, EyeAni.Width, EyeAni.Height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(EyeAni.Width / 2f, EyeAni.Height / 2f), NPC.scale, 0, 0f);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, 0, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}