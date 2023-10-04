using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Redemption.Projectiles.Misc;
using System.IO;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Base;
using Terraria.Audio;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameContent;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Usable;
using Redemption.BaseExtension;
using Redemption.Items.Materials.PostML;
using Terraria.DataStructures;
using ReLogic.Content;

namespace Redemption.NPCs.Bosses.Neb.Clone
{
    [AutoloadBossHead]
    public class Nebuleus_Clone : ModNPC
    {
        private static Asset<Texture2D> chain;
        private static Asset<Texture2D> wings;
        private static Asset<Texture2D> armsAni;
        private static Asset<Texture2D> armsPrayAni;
        private static Asset<Texture2D> armsPrayGlow;
        private static Asset<Texture2D> armsStarfallAni;
        private static Asset<Texture2D> armsStarfallGlow;
        private static Asset<Texture2D> armsPiercingAni;
        private static Asset<Texture2D> armsPiercingGlow;
        private static Asset<Texture2D> armsChainAni;
        private static Asset<Texture2D> armsChainGlow;
        private static Asset<Texture2D> armsEyesAni;
        private static Asset<Texture2D> armsEyesGlow;
        public override void Load()
        {
            chain = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/CosmosChain1");
            wings = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Wings");
            armsAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_Idle");
            armsPrayAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_Pray");
            armsPrayGlow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_Pray_Glow");
            armsStarfallAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_Starfall");
            armsStarfallGlow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_Starfall_Glow");
            armsPiercingAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_PiercingNebula");
            armsPiercingGlow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_PiercingNebula_Glow");
            armsChainAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_CosmicChain");
            armsChainGlow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_CosmicChain_Glow");
            armsEyesAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_LongCharge");
            armsEyesGlow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Nebuleus_Arms_LongCharge_Glow");
        }
        public override void Unload()
        {
            chain = null;
            wings = null;
            armsAni = null;
            armsPrayAni = null;
            armsPrayGlow = null;
            armsStarfallAni = null;
            armsStarfallGlow = null;
            armsPiercingAni = null;
            armsPiercingGlow = null;
            armsChainAni = null;
            armsChainGlow = null;
            armsEyesAni = null;
            armsEyesGlow = null;
        }
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Nebuleus";
        public Vector2[] oldPos = new Vector2[5];
        public float[] oldrot = new float[5];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus Mirage");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCCelestial[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 337500;
            NPC.defense = 80;
            NPC.damage = 180;
            NPC.width = 62;
            NPC.height = 84;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.value = Item.buyPrice(0, 30, 0, 0);
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossStarGod1");

            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Celestial] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Nature] *= .9f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Psychic] *= 1.25f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Shadow] *= 1.1f;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.ai[3] == 6;
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && NPC.ai[0] == 11)
                RazzleDazzle();
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            NPC nPC = new();
            nPC.SetDefaults(ModContent.NPCType<Nebuleus>());
            Main.BestiaryTracker.Kills.RegisterKill(nPC);

            potionType = ItemID.SuperHealingPotion;
            if (!Main.expertMode && Main.rand.NextBool(7))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<NebuleusMask>());
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<NebuleusVanity>());
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedNebuleus, -1);
            if (RedeBossDowned.nebDeath < 5)
            {
                RedeBossDowned.nebDeath = 5;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.WorldData);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<NebBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NebuleusTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<NebRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<GildedBonnet>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.NeverTrue(), ModContent.ItemType<NebuleusMask>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.NeverTrue(), ModContent.ItemType<NebuleusVanity>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<LifeFragment>(), 1, 20, 40));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GalaxyHeart>()));

            npcLoot.Add(notExpertRule);
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.75f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(repeat);
                writer.Write(phase);
                writer.Write(ID);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                repeat = reader.ReadInt32();
                phase = reader.ReadInt32();
                ID = reader.ReadInt32();
            }
        }
        public Vector2 MoveVector2;

        Vector2 vector;
        public int frameCounters;
        public int repeat;
        public int floatTimer;
        public int floatTimer2;
        public int[] armFrames = new int[6];
        private readonly Vector2[] ChainPos = new Vector2[4];
        private readonly Vector2[] getGrad = new Vector2[4];
        public Vector2[] temp = new Vector2[4];
        private readonly Rectangle[] ChainHitBoxArea = new Rectangle[4];
        private Rectangle PlayerSafeHitBox;
        public int phase;
        public int circleTimer;
        public int circleRadius;
        public List<int> AttackList = new() { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12 };
        public List<int> CopyList = null;
        public int ID { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }
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

                if ((ID == 0 && phase >= 3) ||
                (ID == 5 && phase <= 1) ||
                (ID == 6 && phase <= 1) ||
                (ID >= 8 && ID <= 10 && phase <= 0) ||
                (ID == 11 && phase < 3) ||
                (ID == 12 && phase < 3))
                    continue;

                attempts++;
            }
        }
        public override void AI()
        {
            Main.time = 16200;
            Main.dayTime = false;
            for (int k = oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldrot[k] = oldrot[k - 1];
            }
            oldPos[0] = NPC.Center;
            oldrot[0] = NPC.rotation;

            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            #region Frames & Animations
            if (NPC.ai[3] != 6)
            {
                if (floatTimer == 0)
                {
                    NPC.velocity.Y += 0.005f;
                    if (NPC.velocity.Y > .3f)
                    {
                        floatTimer = 1;
                        NPC.netUpdate = true;
                    }
                }
                else if (floatTimer == 1)
                {
                    NPC.velocity.Y -= 0.005f;
                    if (NPC.velocity.Y < -.3f)
                    {
                        floatTimer = 0;
                        NPC.netUpdate = true;
                    }
                }
                if (floatTimer2 == 0)
                {
                    NPC.velocity.X += 0.01f;
                    if (NPC.velocity.X > .4f)
                    {
                        floatTimer2 = 1;
                        NPC.netUpdate = true;
                    }
                }
                else if (floatTimer2 == 1)
                {
                    NPC.velocity.X -= 0.01f;
                    if (NPC.velocity.X < -.4f)
                    {
                        floatTimer2 = 0;
                        NPC.netUpdate = true;
                    }
                }
            }
            if (NPC.ai[3] != 6)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += 98;
                    if (NPC.frame.Y > 294)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y = 0;
                    }
                }
            }
            switch (NPC.ai[3])
            {
                case 0: // Idle
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[0]++;
                        frameCounters = 0;
                    }
                    if (armFrames[0] >= 4)
                    {
                        armFrames[0] = 0;
                    }
                    break;
                case 1: // Pray Idle
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[1]++;
                        frameCounters = 0;
                    }
                    if (armFrames[1] >= 6)
                    {
                        armFrames[1] = 5;
                    }
                    break;
                case 2: // Pray End
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[1]++;
                        frameCounters = 0;
                    }
                    if (armFrames[1] >= 8)
                    {
                        NPC.ai[3] = 0;
                        armFrames[1] = 0;
                    }
                    break;
                case 3: // Starfall Idle
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[2]++;
                        frameCounters = 0;
                    }
                    if (armFrames[2] >= 4)
                    {
                        armFrames[2] = 2;
                    }
                    break;
                case 4: // Starfall End
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[2]++;
                        frameCounters = 0;
                    }
                    if (armFrames[2] >= 8)
                    {
                        NPC.ai[3] = 0;
                        armFrames[2] = 0;
                    }
                    break;
                case 5: // Piercing Nebula Throw
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[3]++;
                        frameCounters = 0;
                    }
                    if (armFrames[3] >= 9)
                    {
                        NPC.ai[3] = 0;
                        armFrames[3] = 0;
                    }
                    break;
                case 6: // Charge
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 392;

                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    if (NPC.velocity.X < 0)
                    {
                        NPC.spriteDirection = -1;
                    }
                    else
                    {
                        NPC.spriteDirection = 1;
                    }
                    break;
                case 7: // Chain Throw
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[4]++;
                        frameCounters = 0;
                    }
                    if (armFrames[4] >= 6)
                    {
                        NPC.ai[3] = 0;
                        armFrames[4] = 4;
                    }
                    break;
                case 8: // Long Charge-Up
                    frameCounters++;
                    if (frameCounters >= 5)
                    {
                        armFrames[5]++;
                        frameCounters = 0;
                    }
                    if (armFrames[5] >= 19)
                    {
                        NPC.ai[3] = 0;
                        armFrames[5] = 0;
                    }
                    break;
            }
            #endregion

            if (DespawnHandler())
                return;

            if (NPC.ai[0] > 4)
            {
                if (NPC.ai[0] != 7 && NPC.ai[0] != 8 && NPC.ai[0] != 10 && NPC.ai[0] != 11)
                    NPC.dontTakeDamage = false;
                else
                    NPC.dontTakeDamage = true;
            }
            if (NPC.life < (int)(NPC.lifeMax * 0.75f) && phase < 1)
            {
                NPC.ai[0] = 6;
                NPC.netUpdate = true;
            }
            if (NPC.life < (int)(NPC.lifeMax * 0.5f) && phase < 2)
            {
                NPC.ai[0] = 6;
                NPC.netUpdate = true;
            }
            if (NPC.life < (int)(NPC.lifeMax * 0.25f) && phase < 3)
            {
                NPC.ai[0] = 6;
                NPC.netUpdate = true;
            }
            switch ((int)NPC.ai[0])
            {
                case 0:
                    #region Dramatic Entrance
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (NPC.ai[2] == 2)
                    {
                        DustHelper.DrawStar(NPC.Center, 58, 5, 4, 1, 3, 2, 0, noGravity: true);
                        DustHelper.DrawStar(NPC.Center, 59, 5, 5, 1, 3, 2, 0, noGravity: true);
                        DustHelper.DrawStar(NPC.Center, 60, 5, 6, 1, 3, 2, 0, noGravity: true);
                        DustHelper.DrawStar(NPC.Center, 62, 5, 7, 1, 3, 2, 0, noGravity: true);
                        for (int d = 0; d < 16; d++)
                            ParticleManager.NewParticle(NPC.Center, RedeHelper.Spread(6), new RainbowParticle(), Color.White, 1);
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[2] >= 60)
                    {
                        NPC.ai[0] = 1;
                        NPC.ai[2] = 0;
                        NPC.netUpdate = true;
                    }
                    #endregion
                    break;
                case 1:
                    #region Starting Dialogue
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (NPC.ai[2] == 30)
                    {
                        NPC.ai[3] = 1;
                    }
                    if (NPC.ai[2] >= 150)
                    {
                        NPC.ai[3] = 2;
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 2;
                        NPC.netUpdate = true;
                    }
                    #endregion
                    break;
                case 2:
                    #region Giant Star Serpent
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (NPC.ai[2] > 40)
                    {
                        NPC.ai[0] = 3;
                        NPC.ai[2] = 100;
                        NPC.netUpdate = true;
                    }
                    #endregion
                    break;
                case 3:
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (NPC.ai[2] >= 240)
                    {
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 4;
                        NPC.netUpdate = true;
                    }
                    break;
                case 4:
                    repeat = 0;
                    NPC.LookAtEntity(player);
                    Teleport(false, Vector2.Zero);
                    frameCounters = 0;
                    NPC.rotation = 0f;
                    NPC.velocity = Vector2.Zero;
                    NPC.ai[3] = 0;
                    NPC.ai[0] = 5;
                    NPC.ai[2] = 0;
                    AttackChoice();
                    circleTimer = 0;
                    circleRadius = 800;
                    NPC.netUpdate = true;
                    break;
                case 5:
                    switch (ID)
                    {
                        // Star Blast
                        #region Star Blast
                        case 0:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 1)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout1>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 10) { NPC.ai[3] = 1; }
                            if (phase <= 0 ? NPC.ai[2] == 30 || NPC.ai[2] == 70 : NPC.ai[2] == 30 || NPC.ai[2] == 50)
                            {
                                int pieCut = 8;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CurvingStar_Tele2>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.01f, 0);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (phase <= 0 ? NPC.ai[2] == 50 : NPC.ai[2] == 40)
                            {
                                int pieCut = phase > 1 ? 10 : 8;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CurvingStar_Tele2>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, phase > 1 ? 1.002f : 1.01f, 1);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (phase <= 0 ? NPC.ai[2] == 120 : NPC.ai[2] == 100) { NPC.ai[3] = 2; }
                            if (phase <= 0 ? NPC.ai[2] >= 160 : NPC.ai[2] >= 140)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Starfall
                        #region Starfall
                        case 1:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 1)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout2>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 10) { NPC.ai[3] = 3; }
                            if (NPC.ai[2] >= 30 && NPC.ai[2] <= 70)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    int A;
                                    if (NPC.spriteDirection != 1)
                                    {
                                        A = Main.rand.Next(600, 650);
                                    }
                                    else
                                    {
                                        A = Main.rand.Next(-650, -600);
                                    }
                                    int B = Main.rand.Next(-200, 200) - 700;

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ModContent.ProjectileType<Starfall_Tele>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -12f : 12f, 14f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 40) { NPC.ai[3] = 4; }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Piercing Nebula
                        #region Piercing Nebula
                        case 2:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 1)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout3>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (phase < 1)
                            {
                                if (NPC.ai[2] == 20 || NPC.ai[2] == 50) { NPC.ai[3] = 5; armFrames[3] = 0; }
                                if (NPC.ai[2] == 30 || NPC.ai[2] == 60)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                }
                            }
                            else if (phase == 1)
                            {
                                if (NPC.ai[2] == 20 || NPC.ai[2] == 40 || NPC.ai[2] == 60) { NPC.ai[3] = 5; armFrames[3] = 0; }
                                if (NPC.ai[2] == 30 || NPC.ai[2] == 50 || NPC.ai[2] == 70)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                }
                            }
                            else if (phase == 2)
                            {
                                if (NPC.ai[2] == 20 || NPC.ai[2] == 50) { NPC.ai[3] = 5; armFrames[3] = 0; }
                                if (NPC.ai[2] == 30)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.5f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.5f), NPC.whoAmI);
                                }
                                if (NPC.ai[2] == 60)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.78f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.78f), NPC.whoAmI);
                                }
                            }
                            else
                            {
                                if (NPC.ai[2] == 20 || NPC.ai[2] == 40 || NPC.ai[2] == 60) { NPC.ai[3] = 5; armFrames[3] = 0; }
                                if (NPC.ai[2] == 30 || NPC.ai[2] == 70)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.78f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.78f), NPC.whoAmI);
                                }
                                if (NPC.ai[2] == 50)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 1.2f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 1.2f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.6f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.6f), NPC.whoAmI);
                                }
                            }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Star Dash
                        #region Star Dash
                        case 3:
                            if (NPC.ai[3] != 6) { NPC.LookAtEntity(player); NPC.netUpdate = true; }
                            NPC.ai[2]++;
                            if (phase > 1 ? NPC.ai[2] == 25 : NPC.ai[2] == 5)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<NebTeleLine1>(), 0, NPC.DirectionTo(player.Center + player.velocity * 20f), phase > 1 ? 115 : 190, ai1: NPC.whoAmI);
                            }
                            if (NPC.ai[2] < 55)
                            {
                                vector = player.Center + player.velocity * 20f;
                            }
                            if (NPC.ai[2] == 65)
                            {
                                NPC.ai[3] = 6;
                                Dash((int)NPC.Distance(player.Center) / 16, true, vector);
                            }
                            else if (NPC.ai[2] == 86)
                            {
                                NPC.rotation = 0;
                                NPC.velocity = Vector2.Zero;
                                NPC.netUpdate = true;
                                if (repeat < 3) NPC.Shoot(NPC.Center, ModContent.ProjectileType<GiantStar_Proj>(), NPC.damage, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] >= 90)
                            {
                                if (repeat <= 2)
                                {
                                    repeat++;
                                    if (phase > 1) NPC.ai[2] = 20;
                                    else NPC.ai[2] = 0;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    repeat = 0;
                                    NPC.velocity = Vector2.Zero;
                                    NPC.ai[3] = 0;
                                    NPC.ai[0] = 4;
                                    NPC.ai[2] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            if (NPC.velocity.Length() < 10)
                            {
                                if (NPC.ai[3] == 6) { NPC.ai[3] = 0; }
                            }
                            break;
                        #endregion

                        // Nebula Dash
                        #region Nebula Dash
                        case 4:
                            if (NPC.ai[3] != 6) { NPC.LookAtEntity(player); NPC.netUpdate = true; }
                            NPC.ai[2]++;
                            if (NPC.ai[2] < 15)
                            {
                                NPC.velocity.Y -= 1f;
                            }
                            if (NPC.ai[2] == 15) { NPC.ai[3] = 6; NPC.netUpdate = true; }
                            if (NPC.ai[2] == 5 || NPC.ai[2] == 15)
                            {
                                NPC.Shoot(new Vector2(player.Center.X, player.Center.Y + 350), ModContent.ProjectileType<Dash_Tele>(), 0, new Vector2(0, -6));
                                for (int m = 0; m < 4; m++)
                                {
                                    int dustID = Dust.NewDust(new Vector2(player.Center.X - 1, player.Center.Y - 1 + 350), 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, Color.White, 2f);
                                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(12f, 0f), m / (float)8 * 6.28f);
                                    Main.dust[dustID].noLight = false;
                                    Main.dust[dustID].noGravity = true;
                                }
                            }
                            if (NPC.ai[2] == 10 || (phase >= 1 ? NPC.ai[2] == 20 : NPC.ai[2] == -1))
                            {
                                NPC.Shoot(new Vector2(player.Center.X, player.Center.Y - 350), ModContent.ProjectileType<Dash_Tele>(), 0, new Vector2(0, 6));
                                for (int m = 0; m < 4; m++)
                                {
                                    int dustID = Dust.NewDust(new Vector2(player.Center.X - 1, player.Center.Y - 1 - 350), 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, Color.White, 2f);
                                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(12f, 0f), m / (float)8 * 6.28f);
                                    Main.dust[dustID].noLight = false;
                                    Main.dust[dustID].noGravity = true;
                                }
                            }
                            if (NPC.ai[2] == 50)
                            {
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                NPC.velocity.Y = phase < 1 ? -30 : -35;
                                Teleport(true, new Vector2(0, 350));
                            }
                            if (phase < 1 ? NPC.ai[2] == 70 : NPC.ai[2] == 65)
                            {
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                NPC.velocity.Y = phase < 1 ? 30 : 35;
                                Teleport(true, new Vector2(0, -350));
                            }
                            if (phase < 1 ? NPC.ai[2] == 90 : NPC.ai[2] == 80)
                            {
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                NPC.velocity.Y = phase < 1 ? -30 : -35;
                                Teleport(true, new Vector2(0, 350));
                            }
                            if (phase >= 1)
                            {
                                if (NPC.ai[2] == 95)
                                {
                                    SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                    NPC.velocity.Y = phase < 1 ? 30 : 35;
                                    Teleport(true, new Vector2(0, -350));
                                }
                            }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.velocity = Vector2.Zero;
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Chain of the Cosmos
                        #region Chain of the Cosmos
                        case 5:
                            NPC.LookAtEntity(player);
                            if (!ScreenPlayer.NebCutscene)
                            {
                                if (circleRadius > 700)
                                {
                                    circleRadius -= 2;
                                }
                                for (int k = 0; k < 6; k++)
                                {
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * circleRadius);
                                    vector.Y = (float)(Math.Cos(angle) * circleRadius);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -NPC.DirectionTo(dust2.position) * 2f;
                                }
                                if (NPC.Distance(player.Center) > circleRadius)
                                {
                                    Vector2 movement = NPC.Center - player.Center;
                                    float difference = movement.Length() - circleRadius;
                                    movement.Normalize();
                                    movement *= difference < 17f ? difference : 17f;
                                    player.position += movement;
                                }
                            }
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 3)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout9>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 40) { NPC.ai[3] = 7; }
                            if (NPC.ai[2] == 50) SoundEngine.PlaySound(SoundID.Item125, NPC.position);
                            int sizeOfChains = 32;
                            float speed = 1;
                            NPC.TargetClosest(true);
                            if (NPC.ai[2] == 1)
                            {
                                int randFactor = 80;
                                for (int i = 0; i < ChainPos.Length; i++)
                                    ChainPos[i] = NPC.Center;
                                temp[0] = player.Center + new Vector2(Main.rand.Next(-randFactor, randFactor), Main.rand.Next(-randFactor, randFactor));
                                temp[1] = player.Center + new Vector2((NPC.Center.X - player.Center.X) * 2, 0) + new Vector2(Main.rand.Next(-randFactor, randFactor), Main.rand.Next(-randFactor, randFactor));
                                temp[2] = player.Center + new Vector2((NPC.Center.X - player.Center.X) * 2, (NPC.Center.Y - player.Center.Y) * 2) + new Vector2(Main.rand.Next(-randFactor, randFactor), Main.rand.Next(-randFactor, randFactor));
                                temp[3] = player.Center + new Vector2(0, (NPC.Center.Y - player.Center.Y) * 2) + new Vector2(Main.rand.Next(-randFactor, randFactor), Main.rand.Next(-randFactor, randFactor));
                                for (int i = 0; i < ChainPos.Length; i++)
                                {
                                    temp[i] += temp[i] - ChainPos[i];
                                }
                            }
                            for (int i = 0; i < ChainPos.Length; i++)
                            {
                                getGrad[i] = (temp[i] - ChainPos[i]) / 32f;
                                if (!ChainHitBoxArea[i].Intersects(PlayerSafeHitBox) && NPC.ai[2] < 800 && NPC.ai[2] > 50)
                                {
                                    ChainPos[i] += getGrad[i] * speed;
                                }
                                ChainHitBoxArea[i] = new Rectangle((int)ChainPos[i].X - sizeOfChains / 2, (int)ChainPos[i].Y - sizeOfChains / 2, sizeOfChains, sizeOfChains);
                            }
                            PlayerSafeHitBox = new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height);
                            for (int i = 0; i < ChainPos.Length; i++)
                            {
                                if (ChainHitBoxArea[i].Intersects(PlayerSafeHitBox))
                                {
                                    if (!ScreenPlayer.NebCutscene && NPC.ai[2] < 300)
                                    {
                                        NPC.ai[2] = 180;
                                        for (int m = 0; m < 8; m++)
                                        {
                                            int dustID = Dust.NewDust(new Vector2(player.Center.X - 1, player.Center.Y - 1), 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, Color.White, 2f);
                                            Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(12f, 0f), m / (float)8 * 6.28f);
                                            Main.dust[dustID].noLight = false;
                                            Main.dust[dustID].noGravity = true;
                                        }
                                        ScreenPlayer.NebCutscene = true;
                                    }
                                    if (NPC.ai[2] < 300)
                                    {
                                        ChainPos[i].Y += (NPC.ai[2] - 180) / 30f;
                                        player.Center = ChainPos[i];
                                    }
                                    else
                                    {
                                        if (NPC.ai[2] == 300)
                                        {
                                            for (int m = 0; m < 8; m++)
                                            {
                                                int dustID = Dust.NewDust(new Vector2(player.Center.X, player.Center.Y - 1000), 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, Color.White, 2f);
                                                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(12f, 0f), m / (float)8 * 6.28f);
                                                Main.dust[dustID].noLight = false;
                                                Main.dust[dustID].noGravity = true;
                                            }
                                            temp[i] = new Vector2(player.Center.X, player.Center.Y - 1000);
                                            NPC.Shoot(new Vector2(player.Center.X, player.Center.Y - 1000), ModContent.ProjectileType<StationaryStar>(), 200, Vector2.Zero, SoundID.Item117);
                                        }
                                        else if (temp[i].Y > player.Center.Y && ScreenPlayer.NebCutscene)
                                        {
                                            NPC.ai[2] = 800;
                                            ScreenPlayer.NebCutscene = false;
                                        }

                                        if (NPC.ai[2] < 800)
                                        {
                                            ChainPos[i].Y -= (NPC.ai[2] - 180) / 4f;
                                            player.Center = ChainPos[i];
                                        }

                                    }
                                }
                            }
                            if (!ChainHitBoxArea[0].Intersects(PlayerSafeHitBox)
                                && !ChainHitBoxArea[1].Intersects(PlayerSafeHitBox)
                                && !ChainHitBoxArea[2].Intersects(PlayerSafeHitBox)
                                && !ChainHitBoxArea[3].Intersects(PlayerSafeHitBox))
                            {
                                ScreenPlayer.NebCutscene = false;
                            }
                            for (int i = 0; i < ChainPos.Length; i++)
                            {
                                if (NPC.ai[2] > 800)
                                {
                                    ChainPos[i] += (NPC.Center - ChainPos[i]) / 10f;
                                }
                            }
                            if (NPC.ai[2] == 850)
                            {
                                for (int i = 0; i < ChainPos.Length; i++)
                                    ChainPos[i] = NPC.Center;
                                NPC.velocity = Vector2.Zero;
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            if (NPC.ai[2] >= 140 && !ChainHitBoxArea[0].Intersects(PlayerSafeHitBox)
                                && !ChainHitBoxArea[1].Intersects(PlayerSafeHitBox)
                                && !ChainHitBoxArea[2].Intersects(PlayerSafeHitBox)
                                && !ChainHitBoxArea[3].Intersects(PlayerSafeHitBox)
                                && NPC.ai[2] < 800)
                            {
                                ScreenPlayer.NebCutsceneflag = false;
                                ScreenPlayer.NebCutscene = false;
                                NPC.velocity = Vector2.Zero;
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Eyes of the Cosmos
                        #region Eyes of the Cosmos
                        case 6:
                            NPC.LookAtEntity(player);
                            if (circleRadius > 600)
                            {
                                circleRadius--;
                            }
                            for (int k = 0; k < 6; k++)
                            {
                                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                vector.X = (float)(Math.Sin(angle) * circleRadius);
                                vector.Y = (float)(Math.Cos(angle) * circleRadius);
                                Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, default, 2f)];
                                dust2.noGravity = true;
                                dust2.velocity = -NPC.DirectionTo(dust2.position) * 2f;
                            }
                            if (NPC.Distance(player.Center) > circleRadius)
                            {
                                Vector2 movement = NPC.Center - player.Center;
                                float difference = movement.Length() - circleRadius;
                                movement.Normalize();
                                movement *= difference < 17f ? difference : 17f;
                                player.position += movement;
                            }
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5)
                            {
                                if (phase < 3) { NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout10>(), 0, Vector2.Zero, NPC.whoAmI); }
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<NebRing>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 20) { NPC.ai[3] = 8; }
                            if (NPC.ai[2] == 30)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y + 132), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 67, NPC.Center.Y + 115), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 67, NPC.Center.Y + 115), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 60)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X - 115, NPC.Center.Y + 67), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 115, NPC.Center.Y + 67), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 132, NPC.Center.Y), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 132, NPC.Center.Y), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 115, NPC.Center.Y - 67), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 115, NPC.Center.Y - 67), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 90)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 132), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 67, NPC.Center.Y - 115), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 67, NPC.Center.Y - 115), ModContent.ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, NPC.whoAmI);
                            }
                            if (phase >= 3)
                            {
                                if (NPC.ai[2] == 95)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout1>(), 0, Vector2.Zero, NPC.whoAmI);
                                }
                                if (NPC.ai[2] == 100) { NPC.ai[3] = 1; }
                                if (NPC.ai[2] == 130 || NPC.ai[2] == 170)
                                {
                                    int pieCut = 8;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.01f, 0);
                                            Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                            Main.projectile[projID].netUpdate = true;
                                        }
                                    }
                                }
                                if (NPC.ai[2] == 150)
                                {
                                    int pieCut = 8;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.002f, 1);
                                            Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                            Main.projectile[projID].netUpdate = true;
                                        }
                                    }
                                }
                                if (NPC.ai[2] == 220) { NPC.ai[3] = 2; }
                            }
                            if (NPC.ai[2] >= 250)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Super Starfall
                        #region Super Starfall
                        case 8:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 2)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout8>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 10) { NPC.ai[3] = 3; }
                            if (NPC.ai[2] >= 40 && NPC.ai[2] < 120)
                            {
                                if (Main.rand.NextBool(4))
                                {
                                    int A = Main.rand.Next(-200, 200) * 6;
                                    int B = Main.rand.Next(-200, 200) - 1000;

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ModContent.ProjectileType<Starfall_Tele>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -2f : 2f, 6f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 50) { NPC.ai[3] = 4; }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Erratic Star Blast
                        #region Erratic Star Blast
                        case 9:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 2)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout5>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 10) { NPC.ai[3] = 1; }
                            if ((phase < 2 ? NPC.ai[2] % 5 == 0 : NPC.ai[2] % 3 == 0) && NPC.ai[2] >= 30 && NPC.ai[2] <= 60)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<CurvingStar_Tele2>(), 120, new Vector2(Main.rand.Next(-7, 7), Main.rand.Next(-7, 7)), 1.01f);
                            }
                            if (NPC.ai[2] == 60) { NPC.ai[3] = 2; }
                            if (NPC.ai[2] >= 100)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Piercing Nebula Burst
                        #region Piercing Nebula Burst
                        case 10:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 2)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout6>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 20) { NPC.ai[3] = 5; armFrames[3] = 0; }
                            if (NPC.ai[2] == 30)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 60 && repeat < 3)
                            {
                                Teleport(false, Vector2.Zero);
                                NPC.ai[2] = 20;
                                NPC.ai[3] = 5;
                                armFrames[3] = 0;
                                repeat++;
                                NPC.netUpdate = true;
                            }
                            if (NPC.ai[2] >= 90)
                            {
                                repeat = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Crystal Stars
                        #region Crystal Stars
                        case 11:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 4)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 100), ModContent.ProjectileType<Shout11>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 10) { NPC.ai[3] = 3; }
                            if (NPC.ai[2] >= 40 && NPC.ai[2] < 120)
                            {
                                if (NPC.ai[2] % 15 == 0)
                                {
                                    int A = Main.rand.Next(-200, 200) * 6;
                                    int B = Main.rand.Next(-200, 200) - 1000;

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ModContent.ProjectileType<CrystalStar_Tele>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -2f : 2f, 6f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 50) { NPC.ai[3] = 4; }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Super Star Blast
                        #region Super Star Blast
                        case 12:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10) { NPC.ai[3] = 1; }
                            if (NPC.ai[2] == 30 || NPC.ai[2] == 50)
                            {
                                int pieCut = 8;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.01f, 0);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 40)
                            {
                                int pieCut = 16;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.002f, 1);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 100) { NPC.ai[3] = 2; }
                            if (NPC.ai[2] >= 140)
                            {
                                NPC.ai[3] = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case 6:
                    NPC.LookAtEntity(player);
                    ScreenPlayer.NebCutsceneflag = false;
                    ScreenPlayer.NebCutscene = false;
                    for (int i = 0; i < ChainPos.Length; i++)
                        ChainPos[i] = NPC.Center;
                    frameCounters = 0;
                    NPC.rotation = 0f;
                    NPC.velocity = Vector2.Zero;
                    NPC.ai[3] = 0;
                    NPC.ai[1] = 0;
                    if (RedeBossDowned.nebDeath < 5 || NPC.life >= (int)(NPC.lifeMax * 0.01f)) { NPC.ai[2] = 0; }
                    else { NPC.ai[2] = 2500; }
                    NPC.ai[0] = 7;
                    if (phase < 1 && NPC.life < (int)(NPC.lifeMax * 0.75f)) { phase = 1; NPC.netUpdate = true; }
                    if (phase < 2 && NPC.life < (int)(NPC.lifeMax * 0.5f)) { phase = 2; NPC.netUpdate = true; }
                    if (phase < 3 && NPC.life < (int)(NPC.lifeMax * 0.25f)) { phase = 3; NPC.netUpdate = true; }
                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<ShockwaveBoom>(), 0, Vector2.Zero, NPC.whoAmI);
                    NPC.netUpdate = true;
                    break;
                case 7:
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (NPC.ai[2] == 30)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath59, NPC.position);
                        RazzleDazzle();
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[2] == 40)
                    {
                        Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
                        Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[2] > 250)
                    {
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 4;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            #region Teleporting
            if (Vector2.Distance(NPC.Center, player.Center) >= 950 && NPC.ai[0] > 0 && NPC.ai[1] != 4 && NPC.ai[1] != 5 && NPC.ai[1] != 6 && NPC.ai[1] != 3)
            {
                Teleport(false, Vector2.Zero);
                NPC.netUpdate = true;
            }
            #endregion
        }
        public override bool CheckDead()
        {
            return true;
        }

        #region Methods
        public void Dash(int speed, bool directional, Vector2 target)
        {
            Player player = Main.player[NPC.target];
            RazzleDazzle();
            SoundEngine.PlaySound(SoundID.Item74, NPC.position);
            if (target == Vector2.Zero) { target = player.Center; }
            if (directional)
            {
                NPC.velocity = NPC.DirectionTo(target) * speed;
            }
            else
            {
                NPC.velocity.X = target.X > NPC.Center.X ? speed : -speed;
            }
        }
        public void Teleport(bool specialPos, Vector2 teleportPos)
        {
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Blue * 0.4f, 5, 0.75f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Purple * 0.4f, 5, 1.5f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Pink * 0.4f, 5, 2.25f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.IndianRed * 0.4f, 5, 3f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
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
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Teleport1, NPC.position);
            RazzleDazzle();
        }
        private bool DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                ScreenPlayer.NebCutsceneflag = false;
                ScreenPlayer.NebCutscene = false;

                NPC.velocity *= 0.96f;
                NPC.velocity.Y -= 1;
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                return true;
            }
            else NPC.DiscourageDespawn(60);
            return false;
        }
        public void RazzleDazzle()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.IndianRed, 5, 0.75f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Pink, 5, 1.5f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Purple, 5, 2.25f, 2, 0.7f, 2, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Blue, 5, 3f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            }
        }
        #endregion

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.DamageType == DamageClass.Melee)
                modifiers.FinalDamage *= 2;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Redemption().TechnicallyMelee)
                modifiers.FinalDamage *= 2;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingRainbowDye);
            Vector2 drawCenter = new(NPC.Center.X, NPC.Center.Y);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = oldPos.Length - 1; k >= 0; k -= 1)
            {
                float alpha = 1f - (k + 1) / (float)(oldPos.Length + 2);
                spriteBatch.Draw(texture, oldPos[k] - screenPos, NPC.frame, Main.DiscoColor * (0.5f * alpha), oldrot[k], NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }
            if (NPC.ai[1] == 5 && NPC.ai[2] > 50 && NPC.ai[0] < 6)
            {
                for (int i = 0; i < ChainPos.Length; i++)
                {
                    RedeHelper.DrawBezier(spriteBatch, chain.Value, "", Main.DiscoColor, NPC.Center, ChainPos[i], (NPC.Center + ChainPos[i]) / 2 + new Vector2(0, 130 + (int)(Math.Sin(NPC.ai[2] / 12) * (150 - NPC.ai[2] / 3))), (NPC.Center + ChainPos[i]) / 2 + new Vector2(0, 130 + (int)(Math.Sin(NPC.ai[2] / 12) * (150 - NPC.ai[2] / 3))), 0.04f, 0);
                }
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, RedeColor.NebColour * ((255 - NPC.alpha) / 255f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            spriteBatch.Draw(wings.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            if (NPC.ai[3] != 6)
            {
                if (NPC.ai[3] == 0)
                {
                    int num214 = armsAni.Value.Height / 4;
                    int y6 = num214 * armFrames[0];
                    Main.spriteBatch.Draw(armsAni.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsAni.Value.Width, num214)), RedeColor.NebColour * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(armsAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                }
                if (NPC.ai[3] == 1 || NPC.ai[3] == 2)
                {
                    int num214 = armsPrayAni.Value.Height / 8;
                    int y6 = num214 * armFrames[1];
                    Main.spriteBatch.Draw(armsPrayAni.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsPrayAni.Value.Width, num214)), RedeColor.NebColour * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(armsPrayAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                    Main.spriteBatch.Draw(armsPrayGlow.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsPrayAni.Value.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsPrayAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                }
                if (NPC.ai[3] == 3 || NPC.ai[3] == 4)
                {
                    int num214 = armsStarfallAni.Value.Height / 8;
                    int y6 = num214 * armFrames[2];
                    Main.spriteBatch.Draw(armsStarfallAni.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsStarfallAni.Value.Width, num214)), RedeColor.NebColour * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(armsStarfallAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                    Main.spriteBatch.Draw(armsStarfallGlow.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsStarfallAni.Value.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsStarfallAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                }
                if (NPC.ai[3] == 5)
                {
                    int num214 = armsPiercingAni.Value.Height / 9;
                    int y6 = num214 * armFrames[3];
                    Main.spriteBatch.Draw(armsPiercingAni.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsPiercingAni.Value.Width, num214)), RedeColor.NebColour * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(armsPiercingAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                    Main.spriteBatch.Draw(armsPiercingGlow.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsPiercingAni.Value.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsPiercingAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                }
                if (NPC.ai[3] == 7)
                {
                    int num214 = armsChainAni.Value.Height / 7;
                    int y6 = num214 * armFrames[4];
                    Main.spriteBatch.Draw(armsChainAni.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsChainAni.Value.Width, num214)), RedeColor.NebColour * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(armsChainAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                    Main.spriteBatch.Draw(armsChainGlow.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsChainAni.Value.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsChainAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                }
                if (NPC.ai[3] == 8)
                {
                    int num214 = armsEyesAni.Value.Height / 19;
                    int y6 = num214 * armFrames[5];
                    Main.spriteBatch.Draw(armsEyesAni.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsEyesAni.Value.Width, num214)), RedeColor.NebColour * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(armsEyesAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                    Main.spriteBatch.Draw(armsEyesGlow.Value, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, armsEyesAni.Value.Width, num214)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsEyesAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0f);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
