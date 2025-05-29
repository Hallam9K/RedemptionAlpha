using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.CrossMod;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Friendly;
using Redemption.Particles;
using Redemption.Projectiles.Misc;
using Redemption.Textures;
using Redemption.UI;
using Redemption.UI.ChatUI;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Redemption.NPCs.Bosses.Neb
{
    [AutoloadBossHead]
    public class Nebuleus : ModNPC
    {
        public static Asset<Texture2D> chain;
        public static Asset<Texture2D> wings;
        private Asset<Texture2D> armsAni;
        private Asset<Texture2D> armsPrayAni;
        private Asset<Texture2D> armsPrayGlow;
        private Asset<Texture2D> armsStarfallAni;
        private Asset<Texture2D> armsStarfallGlow;
        public static Asset<Texture2D> armsPiercingAni;
        public static Asset<Texture2D> armsPiercingGlow;
        private Asset<Texture2D> armsChainAni;
        private Asset<Texture2D> armsChainGlow;
        private Asset<Texture2D> armsEyesAni;
        private Asset<Texture2D> armsEyesGlow;
        private Asset<Texture2D> armsBookAni;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            chain = Request<Texture2D>("Redemption/NPCs/Bosses/Neb/CosmosChain1");
            wings = Request<Texture2D>(Texture + "_Wings");
            armsPiercingAni = Request<Texture2D>(Texture + "_Arms_PiercingNebula");
            armsPiercingGlow = Request<Texture2D>(Texture + "_Arms_PiercingNebula_Glow");
        }
        public Vector2[] oldPos = new Vector2[5];
        public float[] oldrot = new float[5];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus, Angel of the Cosmos");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.DryadsWardDebuff] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffType<PureChillDebuff>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffType<DragonblazeDebuff>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffType<MoonflareDebuff>()] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new();
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCCelestial[Type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement("...")
            });
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
        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Neb.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice3 with { Volume = 2f, Pitch = -.4f };
        private readonly Color nebColor = new(255, 100, 174);
        private readonly Color nebColor2 = new(4, 0, 108);
        public readonly Vector2 modifier = new(0, -220);

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.ai[3] == 6;
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
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
            potionType = ItemID.SuperHealingPotion;
            if (!Main.expertMode && Main.rand.NextBool(7))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusMask>());
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusVanity>());
            }
            if (!RedeBossDowned.downedNebuleus)
            {
                RedeQuest.adviceSeen[(int)RedeQuest.Advice.StarSerpent] = true;
                RedeQuest.SyncData();

                RedeWorld.Alignment += 0;
                ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromLiteral("..."), 120, 30, 0, Color.DarkGoldenrod);
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedNebuleus, -1);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
            if (RedeBossDowned.nebDeath < 5)
            {
                RedeBossDowned.nebDeath = 5;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<NebBag>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<NebuleusTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<NebRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<GildedBonnet>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.NeverTrue(), ItemType<NebuleusMask>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.NeverTrue(), ItemType<NebuleusVanity>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<LifeFragment>(), 1, 20, 40));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<GalaxyHeart>()));

            npcLoot.Add(notExpertRule);
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.8f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(repeat);
            writer.Write(phase);
            writer.Write(ID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            repeat = reader.ReadInt32();
            phase = reader.ReadInt32();
            ID = reader.ReadInt32();
        }
        bool transforming;

        private Vector2 vector;
        private int frameCounters;
        private int repeat;
        private int armFrame;
        private Vector2[] DashPos = new Vector2[4];
        private readonly Vector2[] ChainPos = new Vector2[4];
        private readonly Vector2[] getGrad = new Vector2[4];
        private readonly Vector2[] temp = new Vector2[4];
        private readonly Rectangle[] ChainHitBoxArea = new Rectangle[4];
        private Rectangle PlayerSafeHitBox;
        bool title = false;
        private int phase;
        private int circleRadius;
        private readonly List<int> AttackList = new() { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14 };
        private List<int> CopyList = null;
        private int ID { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }
        private void AttackChoice()
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
                (ID == 5 && (phase <= 1 || RedeHelper.AnyProjectiles(ProjectileType<Neb_Moonbeam>()))) ||
                (ID == 6 && (phase <= 1 || RedeHelper.AnyProjectiles(ProjectileType<Neb_Moonbeam>()))) ||
                (ID >= 8 && ID <= 10 && phase <= 0) ||
                (ID == 11 && phase < 3) ||
                (ID == 12 && phase < 3) ||
                (ID == 13 && phase < 3) ||
                (ID == 14 && phase < 2))
                    continue;

                attempts++;
            }
        }
        public override void AI()
        {
            Main.time = 16200;
            Main.dayTime = false;
            Lighting.AddLight(NPC.Center, .8f, .6f, 1);
            if (!title)
            {
                for (int i = 0; i < ChainPos.Length; i++)
                    ChainPos[i] = NPC.Center;
                title = true;
            }
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

            if (DespawnHandler())
                return;

            if (NPC.ai[0] > 4)
            {
                if (NPC.ai[0] != 7 && NPC.ai[0] < 8)
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
            if (NPC.life < (int)(NPC.lifeMax * 0.01f) && phase < 4 && NPC.type == NPCType<Nebuleus>())
            {
                NPC.ai[0] = 6;
                NPC.netUpdate = true;
            }
            switch ((int)NPC.ai[0])
            {
                case 0:
                    #region Dramatic Entrance
                    NPC.LookAtEntity(player);
                    if (NPC.ai[2]++ == 0)
                    {
                        FablesHelper.DisplayBossIntroCard("Mods.Redemption.TitleCard.Neb.Name", "Mods.Redemption.TitleCard.Neb.Modifier", 120, false, Color.Purple, Color.HotPink, Color.HotPink, Color.Purple, "Interstellar Isolation", "musicman");
                    }

                    if (RedeBossDowned.nebDeath == 0 && NPC.type == NPCType<Nebuleus>())
                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 1200);
                    else
                    {
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
                    if (RedeBossDowned.nebDeath == 0)
                    {
                        if (NPC.ai[2] < 1760)
                            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 1200);
                        if (!Main.dedServ && NPC.ai[2] == 60)
                        {
                            string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Intro.1");
                            string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Intro.2");
                            if (RedeWorld.Alignment >= 0)
                                s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Intro.2Alt");
                            string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Intro.3");
                            string s4 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Intro.4");
                            string s5 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Intro.5");
                            DialogueChain chain = new();
                            chain.Add(new(NPC, s1, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s2, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s3, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s4, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s5, nebColor, nebColor2, voice, .03f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (NPC.ai[2] >= 5000)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.nebDeath = 1;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            ArmAnimation(0, true);
                            NPC.ai[2] = 0;
                            NPC.ai[0] = 3;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (NPC.ai[2] == 30 && !Main.dedServ)
                        {
                            string s1 = RedeBossDowned.nebDeath < 5 ? Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Resummon.1") : Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Resummon.2");
                            DialogueChain chain = new();
                            chain.Add(new(NPC, s1, nebColor, nebColor2, voice, .03f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (NPC.ai[2] >= 500)
                        {
                            ArmAnimation(0, true);
                            NPC.ai[2] = 0;
                            NPC.ai[0] = 3;
                            NPC.netUpdate = true;
                        }
                    }
                    #endregion
                    break;
                case 3:
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (NPC.ai[2] >= 60)
                    {
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 4;
                        NPC.netUpdate = true;
                    }
                    break;
                case 4:
                    ResetVars(player);
                    AttackChoice();
                    NPC.netUpdate = true;
                    break;
                case 5:
                    switch (ID)
                    {
                        #region Star Blast
                        case 0:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 1)
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.StarBlast"));
                            if (NPC.ai[2] == 10)
                                ArmAnimation(1, true);
                            if (phase <= 0 ? NPC.ai[2] == 30 || NPC.ai[2] == 70 : NPC.ai[2] == 30 || NPC.ai[2] == 50)
                            {
                                int pieCut = 8;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele2>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.01f, 0);
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
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele2>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, phase > 1 ? 1.002f : 1.01f, 1);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (phase <= 0 ? NPC.ai[2] == 120 : NPC.ai[2] == 100) ArmAnimation(2);
                            if (phase <= 0 ? NPC.ai[2] >= 160 : NPC.ai[2] >= 140)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Starfall
                        case 1:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 1)
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.Starfall"));
                            if (NPC.ai[2] == 10)
                                ArmAnimation(3, true);
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

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ProjectileType<Starfall_Tele>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -12f : 12f, 14f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 40)
                                ArmAnimation(4);
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Piercing Nebula
                        case 2:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 1)
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.PiercingNebula"));
                            if (phase < 1)
                            {
                                if (NPC.ai[2] == 20 || NPC.ai[2] == 50)
                                    ArmAnimation(5, true);
                                if (NPC.ai[2] == 30 || NPC.ai[2] == 60)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                }
                            }
                            else if (phase == 1)
                            {
                                if (NPC.ai[2] == 5)
                                    Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.PiercingNebula2"));

                                if (NPC.ai[2] >= 30 && NPC.ai[2] <= 32)
                                    NPC.Shoot(NPC.Center, ProjectileType<NebMirage_PiercingNebula>(), (int)(NPC.damage * .67f), Vector2.Zero, NPC.whoAmI);
                                if (NPC.ai[2] == 20 || NPC.ai[2] == 40 || NPC.ai[2] == 60)
                                    ArmAnimation(5, true);
                                if (NPC.ai[2] == 30 || NPC.ai[2] == 50 || NPC.ai[2] == 70)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                }
                            }
                            else if (phase == 2)
                            {
                                if (NPC.ai[2] >= 30 && NPC.ai[2] <= 33)
                                    NPC.Shoot(NPC.Center, ProjectileType<NebMirage_PiercingNebula>(), (int)(NPC.damage * .67f), Vector2.Zero, NPC.whoAmI);

                                if (NPC.ai[2] == 20 || NPC.ai[2] == 50)
                                    ArmAnimation(5, true);
                                if (NPC.ai[2] == 30)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.5f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.5f), NPC.whoAmI);
                                }
                                if (NPC.ai[2] == 60)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.78f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.78f), NPC.whoAmI);
                                }
                            }
                            else
                            {
                                if (NPC.ai[2] is 30 or 32 or 50 or 52 or 70 or 72)
                                    NPC.Shoot(NPC.Center, ProjectileType<NebMirage_PiercingNebula>(), (int)(NPC.damage * .67f), Vector2.Zero, NPC.whoAmI);
                                if (NPC.ai[2] == 20 || NPC.ai[2] == 40 || NPC.ai[2] == 60)
                                    ArmAnimation(5, true);
                                if (NPC.ai[2] == 30 || NPC.ai[2] == 70)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.78f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.78f), NPC.whoAmI);
                                }
                                if (NPC.ai[2] == 50)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 1.2f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 1.2f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() + 0.6f), NPC.whoAmI);
                                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation() - 0.6f), NPC.whoAmI);
                                }
                            }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Star Dash
                        case 3:
                            if (NPC.ai[3] != 6)
                                NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (phase > 1 ? NPC.ai[2] == 25 : NPC.ai[2] == 5)
                            {
                                NPC.Shoot(NPC.Center, ProjectileType<NebTeleLine1>(), 0, NPC.DirectionTo(player.Center + player.velocity * 20f), phase > 1 ? 115 : 190, ai1: NPC.whoAmI);
                            }
                            if (NPC.ai[2] < 55)
                            {
                                vector = player.Center + player.velocity * 20f;
                            }
                            if (NPC.ai[2] == 65)
                            {
                                ArmAnimation(6);
                                Dash((int)NPC.Distance(player.Center) / 16, true, vector);
                            }
                            else if (NPC.ai[2] == 86)
                            {
                                NPC.rotation = 0;
                                NPC.velocity = Vector2.Zero;
                                if (repeat < 3) NPC.Shoot(NPC.Center, ProjectileType<GiantStar_Proj>(), NPC.damage, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] > 65 && NPC.ai[2] < 86)
                            {
                                for (int i = 0; i < 2; i++)
                                    ParticleManager.NewParticle(NPC.Center + NPC.velocity, RedeHelper.Spread(3), new RainbowParticle(), Color.White, Main.rand.NextFloat(.4f, .8f), AI4: Main.rand.Next(20, 40));
                                for (int i = 0; i < 5; i++)
                                {
                                    Dust dust = Main.dust[Dust.NewDust(NPC.Center, 2, 2, DustType<GlowDust>(), Scale: 2)];
                                    dust.noGravity = true;
                                    dust.noLight = true;
                                    Color dustColor = new(Main.DiscoR, Main.DiscoG, Main.DiscoB) { A = 0 };
                                    dust.color = dustColor * .5f;
                                }
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
                                    NPC.ai[0] = 4;
                                    NPC.ai[2] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            if (NPC.velocity.Length() < 10)
                            {
                                if (NPC.ai[3] == 6)
                                    NPC.ai[3] = 0;
                            }
                            break;
                        #endregion

                        #region Nebula Dash
                        case 4:
                            if (NPC.ai[3] != 6) { NPC.LookAtEntity(player); NPC.netUpdate = true; }
                            if (NPC.ai[2]++ == 0)
                            {
                                DashPos = Main.rand.Next(8) switch
                                {
                                    1 => new[] { new Vector2(0, -350), new(0, 350), new(0, -350), new(0, 350) },
                                    2 => new[] { new Vector2(0, 350), new(500, 0), new(0, -350), new(0, 350) },
                                    3 => new[] { new Vector2(0, -350), new(-500, 0), new(0, 350), new(0, -350) },
                                    4 => new[] { new Vector2(500, 0), new(-500, 0), new(500, 0), new(-500, 0) },
                                    5 => new[] { new Vector2(-500, 0), new(500, 0), new(-500, 0), new(500, 0) },
                                    6 => new[] { new Vector2(-500, 0), new(500, 0), new(0, 350), new(0, -350) },
                                    7 => new[] { new Vector2(500, 0), new(-500, 0), new(0, -350), new(0, 350) },
                                    _ => new[] { new Vector2(0, 350), new(0, -350), new(0, 350), new(0, -350) },
                                };
                            }
                            if (NPC.ai[2] < 15)
                            {
                                NPC.velocity.Y -= 1f;
                            }
                            if (NPC.ai[2] == 15)
                                ArmAnimation(6);
                            if (NPC.ai[2] == 5 || NPC.ai[2] == 15)
                            {
                                Vector2 a = Vector2.Zero;
                                int b = NPC.ai[2] == 5 ? 0 : 2;
                                if (DashPos[b].X > 0)
                                    a = new Vector2(-6, 0);
                                else if (DashPos[b].X < 0)
                                    a = new Vector2(6, 0);
                                else if (DashPos[b].Y > 0)
                                    a = new Vector2(0, -6);
                                else if (DashPos[b].Y < 0)
                                    a = new Vector2(0, 6);

                                NPC.Shoot(player.Center + DashPos[b], ProjectileType<Dash_Tele>(), 0, a);
                                SoundEngine.PlaySound(SoundID.Item25 with { Pitch = .3f }, player.Center + DashPos[b]);
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], Color.Pink, shakeAmount: 0, scale: 4, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], RedeColor.NebColour, shakeAmount: 0, scale: 5, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], Color.White, shakeAmount: 0, scale: 3, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                            }
                            if (NPC.ai[2] == 10 || (phase >= 1 ? NPC.ai[2] == 20 : NPC.ai[2] == -1))
                            {
                                Vector2 a = Vector2.Zero;
                                int b = NPC.ai[2] == 10 ? 1 : 3;
                                if (DashPos[b].X > 0)
                                    a = new Vector2(-6, 0);
                                else if (DashPos[b].X < 0)
                                    a = new Vector2(6, 0);
                                else if (DashPos[b].Y > 0)
                                    a = new Vector2(0, -6);
                                else if (DashPos[b].Y < 0)
                                    a = new Vector2(0, 6);

                                NPC.Shoot(player.Center + DashPos[b], ProjectileType<Dash_Tele>(), 0, a);
                                SoundEngine.PlaySound(SoundID.Item25 with { Pitch = .3f }, player.Center + DashPos[b]);
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], Color.Pink, shakeAmount: 0, scale: 4, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], RedeColor.NebColour, shakeAmount: 0, scale: 5, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], Color.White, shakeAmount: 0, scale: 3, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                            }
                            if (NPC.ai[2] == 50)
                            {
                                NPC.velocity *= 0f;
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                if (DashPos[0].X > 0)
                                    NPC.velocity.X = phase < 1 ? -30 : -35;
                                else if (DashPos[0].X < 0)
                                    NPC.velocity.X = phase < 1 ? 30 : 35;
                                else if (DashPos[0].Y > 0)
                                    NPC.velocity.Y = phase < 1 ? -30 : -35;
                                else if (DashPos[0].Y < 0)
                                    NPC.velocity.Y = phase < 1 ? 30 : 35;
                                Teleport(true, DashPos[0]);
                            }
                            if (phase < 1 ? NPC.ai[2] == 70 : NPC.ai[2] == 65)
                            {
                                NPC.velocity *= 0f;
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                if (DashPos[1].X > 0)
                                    NPC.velocity.X = phase < 1 ? -30 : -35;
                                else if (DashPos[1].X < 0)
                                    NPC.velocity.X = phase < 1 ? 30 : 35;
                                else if (DashPos[1].Y > 0)
                                    NPC.velocity.Y = phase < 1 ? -30 : -35;
                                else if (DashPos[1].Y < 0)
                                    NPC.velocity.Y = phase < 1 ? 30 : 35;
                                Teleport(true, DashPos[1]);
                            }
                            if (phase < 1 ? NPC.ai[2] == 90 : NPC.ai[2] == 80)
                            {
                                NPC.velocity *= 0f;
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                if (DashPos[2].X > 0)
                                    NPC.velocity.X = phase < 1 ? -30 : -35;
                                else if (DashPos[2].X < 0)
                                    NPC.velocity.X = phase < 1 ? 30 : 35;
                                else if (DashPos[2].Y > 0)
                                    NPC.velocity.Y = phase < 1 ? -30 : -35;
                                else if (DashPos[2].Y < 0)
                                    NPC.velocity.Y = phase < 1 ? 30 : 35;
                                Teleport(true, DashPos[2]);
                            }
                            if (phase >= 1 && NPC.ai[2] == 95)
                            {
                                NPC.velocity *= 0f;
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                if (DashPos[3].X > 0)
                                    NPC.velocity.X = phase < 1 ? -30 : -35;
                                else if (DashPos[3].X < 0)
                                    NPC.velocity.X = phase < 1 ? 30 : 35;
                                else if (DashPos[3].Y > 0)
                                    NPC.velocity.Y = phase < 1 ? -30 : -35;
                                else if (DashPos[3].Y < 0)
                                    NPC.velocity.Y = phase < 1 ? 30 : 35;
                                Teleport(true, DashPos[3]);
                            }
                            if (NPC.velocity.Length() >= 30)
                            {
                                for (int i = 0; i < 2; i++)
                                    ParticleManager.NewParticle(NPC.Center + NPC.velocity, RedeHelper.Spread(3), new RainbowParticle(), Color.White, Main.rand.NextFloat(.4f, .8f), AI4: Main.rand.Next(20, 40));
                                for (int i = 0; i < 5; i++)
                                {
                                    Dust dust = Main.dust[Dust.NewDust(NPC.Center, 2, 2, DustType<GlowDust>(), Scale: 1)];
                                    dust.noGravity = true;
                                    dust.noLight = true;
                                    Color dustColor = new(Main.DiscoR, Main.DiscoG, Main.DiscoB) { A = 0 };
                                    dust.color = dustColor * .5f;
                                }
                            }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.velocity = Vector2.Zero;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

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
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.Chains"));
                            if (NPC.ai[2] == 40)
                                ArmAnimation(7, true);
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
                                            NPC.Shoot(new Vector2(player.Center.X, player.Center.Y - 1000), ProjectileType<StationaryStar>(), (int)(NPC.damage * 1.1f), Vector2.Zero, SoundID.Item117);
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
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

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
                                if (phase < 3)
                                    Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.Eyes"));

                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<NebRing>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 20)
                                ArmAnimation(8, true);
                            if (NPC.ai[2] == 30)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y + 132), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 67, NPC.Center.Y + 115), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 67, NPC.Center.Y + 115), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 60)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X - 115, NPC.Center.Y + 67), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 115, NPC.Center.Y + 67), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 132, NPC.Center.Y), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 132, NPC.Center.Y), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 115, NPC.Center.Y - 67), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 115, NPC.Center.Y - 67), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 90)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y - 132), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X - 67, NPC.Center.Y - 115), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                NPC.Shoot(new Vector2(NPC.Center.X + 67, NPC.Center.Y - 115), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                            }
                            if (phase >= 3)
                            {
                                if (NPC.ai[2] == 95)
                                    Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.StarBlast"));
                                if (NPC.ai[2] == 100)
                                    ArmAnimation(1, true);
                                if (NPC.ai[2] == 130)
                                {
                                    int pieCut = 6;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.01f, 0);
                                            Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                            Main.projectile[projID].netUpdate = true;
                                        }
                                    }
                                }
                                if (NPC.ai[2] == 170)
                                {
                                    int pieCut = 6;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.01f, 1);
                                            Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                            Main.projectile[projID].netUpdate = true;
                                        }
                                    }
                                }
                                if (NPC.ai[2] == 220)
                                    ArmAnimation(2);
                            }
                            if (NPC.ai[2] >= 250)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Super Starfall
                        case 8:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 2)
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.SuperStarfall"));
                            if (NPC.ai[2] == 10)
                                ArmAnimation(3, true);
                            if (NPC.ai[2] >= 40 && NPC.ai[2] < 120)
                            {
                                if (Main.rand.NextBool(4))
                                {
                                    int A = Main.rand.Next(-200, 200) * 6;
                                    int B = Main.rand.Next(-200, 200) - 1000;

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ProjectileType<Starfall_Tele>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -2f : 2f, 6f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 50)
                                ArmAnimation(4);
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Erratic Star Blast
                        case 9:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 2)
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.ErraticStarBlast"));
                            if (NPC.ai[2] == 10)
                                ArmAnimation(1, true);
                            if ((phase < 2 ? NPC.ai[2] % 5 == 0 : NPC.ai[2] % 3 == 0) && NPC.ai[2] >= 30 && NPC.ai[2] <= 60)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<CurvingStar_Tele2>(), (int)(NPC.damage * .7f), new Vector2(Main.rand.Next(-7, 7), Main.rand.Next(-7, 7)), 1.01f);
                            }
                            if (NPC.ai[2] == 60)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 100)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Piercing Nebula Burst
                        case 10:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 2)
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.PiercingNebulaBurst"));
                            if (NPC.ai[2] == 20)
                                ArmAnimation(5, true);
                            if (NPC.ai[2] == 30)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .7f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                            }
                            if (phase >= 3 && NPC.ai[2] == 60)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .7f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                            }
                            if (phase > 1 && NPC.ai[2] == 20)
                                NPC.Shoot(NPC.Center, ProjectileType<NebMirage_PiercingNebula>(), (int)(NPC.damage * .67f), Vector2.Zero, NPC.whoAmI);
                            if (NPC.ai[2] == 60 && repeat < 3)
                            {
                                Teleport(false, Vector2.Zero);
                                NPC.ai[2] = 19;
                                ArmAnimation(5, true);
                                repeat++;
                                NPC.netUpdate = true;
                            }
                            if (NPC.ai[2] >= 90)
                            {
                                repeat = 0;
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Crystal Stars
                        case 11:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 5 && phase < 4)
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.CrystalStars"));
                            if (NPC.ai[2] == 10)
                                ArmAnimation(3, true);
                            if (NPC.ai[2] >= 40 && NPC.ai[2] < 120)
                            {
                                if (NPC.ai[2] % 10 == 0)
                                {
                                    int A = Main.rand.Next(-200, 200) * 6;
                                    int B = Main.rand.Next(-200, 200) - 1000;

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ProjectileType<CrystalStar_Tele>(), (int)(NPC.damage * .7f), new Vector2(NPC.spriteDirection != 1 ? -2f : 2f, 6f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 50)
                                ArmAnimation(4);
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Super Star Blast
                        case 12:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                                ArmAnimation(1, true);
                            if (NPC.ai[2] == 30 || NPC.ai[2] == 50)
                            {
                                int pieCut = 8;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.01f, 0);
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
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.002f, 1);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 100)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 140)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Tome: Moonbeam
                        case 13:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                            {
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.Moonbeam"), new Color(206, 255, 227));
                                ArmAnimation(9, true);
                            }
                            if (NPC.ai[2] > 30 && NPC.ai[2] % 10 == 0 && NPC.ai[2] < 140)
                            {
                                NPC.Shoot(NPC.Center + new Vector2(0, -50), ProjectileType<Neb_Moonbeam_Tele>(), 0, new Vector2(0, -4 - ((NPC.ai[2] - 30) / 10)), SoundID.Item117 with { Pitch = (NPC.ai[2] - 30) / 40 });
                                NPC.Shoot(NPC.Center + new Vector2(0, -50), ProjectileType<Neb_Moonbeam_Tele>(), 0, new Vector2(0, -4 - ((NPC.ai[2] - 30) / 10)), 1);
                                NPC.Shoot(NPC.Center + new Vector2(0, -50), ProjectileType<Neb_Moonbeam_Tele>(), 0, new Vector2(0, -4 - ((NPC.ai[2] - 30) / 10)), 2, 1);
                            }
                            int offset = 0;
                            if (player.velocity.X > 4)
                                offset = 600;
                            else if (player.velocity.X < -4)
                                offset = -600;
                            if (NPC.ai[2] > 60 && NPC.ai[2] < 190)
                            {
                                ParticleManager.NewParticle(new Vector2(player.Center.X + offset, player.Center.Y - (Main.screenHeight / 2)) + new Vector2(Main.rand.Next(-234, 234), Main.rand.Next(0, Main.screenHeight)), new Vector2(0, -Main.rand.NextFloat(1, 3)), new RainbowParticle(), Color.Cyan, Main.rand.NextFloat(.1f, .4f), 0, 0, 0, 0, Main.rand.Next(10, 20), (NPC.ai[2] - 60) / 190);
                            }
                            if (NPC.ai[2] == 190)
                            {
                                for (int i = 0; i < 40; i++)
                                {
                                    ParticleManager.NewParticle(new Vector2(player.Center.X - 234 + offset, player.Center.Y - (Main.screenHeight / 2)) + new Vector2(0, Main.rand.Next(0, Main.screenHeight)), new Vector2(0, -Main.rand.NextFloat(4, 7)), new RainbowParticle(), Color.Cyan, Main.rand.NextFloat(.1f, .4f), AI4: Main.rand.Next(60, 90));
                                    ParticleManager.NewParticle(new Vector2(player.Center.X + 234 + offset, player.Center.Y - (Main.screenHeight / 2)) + new Vector2(0, Main.rand.Next(0, Main.screenHeight)), new Vector2(0, -Main.rand.NextFloat(4, 7)), new RainbowParticle(), Color.Cyan, Main.rand.NextFloat(.1f, .4f), AI4: Main.rand.Next(60, 90));
                                }
                                vector = player.Center + new Vector2(offset, 0);
                                SoundEngine.PlaySound(CustomSounds.NebSound2 with { Pitch = .5f }, player.Center);
                            }
                            if (NPC.ai[2] == 210)
                            {
                                NPC.Shoot(new Vector2(vector.X, player.Center.Y - 800), ProjectileType<Neb_Moonbeam>(), (int)(NPC.damage * 1.5f), Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 300)
                                ArmAnimation(10);
                            if (NPC.ai[2] >= 360)
                            {
                                NPC.ai[0] = 4;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Tome: Meteoric Swarm
                        case 14:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                            {
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.MeteoricSwarm"), Color.Orange);
                                ArmAnimation(9, true);
                            }
                            if (NPC.ai[2] == 60)
                            {
                                for (int k = 0; k < 100; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustType<GlowDust>(), Scale: 12f)];
                                    dust2.noGravity = true;
                                    dust2.color = Color.Orange with { A = 0 };
                                    dust2.velocity = NPC.DirectionTo(dust2.position) * 60f;
                                }
                                SoundEngine.PlaySound(CustomSounds.NebMeteor with { Volume = 2f, Pitch = -.3f });
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 80;
                            }
                            if (NPC.ai[2] > 60 && NPC.ai[2] % 5 == 0 && NPC.ai[2] < 340)
                            {
                                int rand = Main.rand.Next(1, 3);
                                for (int i = 0; i < rand; i++)
                                    NPC.Shoot(player.Center + new Vector2(Main.rand.Next(-(Main.screenWidth / 2), Main.screenWidth / 2), Main.rand.Next(-(Main.screenHeight / 2), Main.screenHeight / 2)), ProjectileType<Neb_Meteor_Tele>(), (int)(NPC.damage * .67f), RedeHelper.Spread(2));
                            }
                            if (NPC.ai[2] == 400)
                                ArmAnimation(10);
                            if (NPC.ai[2] >= 460)
                            {
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
                    ArmAnimation(0, true);
                    NPC.ai[1] = 0;
                    if (RedeBossDowned.nebDeath < 5 || NPC.life >= (int)(NPC.lifeMax * 0.01f)) { NPC.ai[2] = 0; }
                    else { NPC.ai[2] = 4880; }
                    if (NPC.life < (int)(NPC.lifeMax * 0.01f) && NPC.type == NPCType<Nebuleus>())
                    {
                        NPC.ai[0] = 8;
                        phase = 4;
                    }
                    else
                    {
                        NPC.ai[0] = 7;
                        if (phase < 1 && NPC.life < (int)(NPC.lifeMax * 0.75f)) { phase = 1; }
                        if (phase < 2 && NPC.life < (int)(NPC.lifeMax * 0.5f)) { phase = 2; }
                        if (phase < 3 && NPC.life < (int)(NPC.lifeMax * 0.25f)) { phase = 3; }
                    }
                    NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<ShockwaveBoom>(), 0, Vector2.Zero, NPC.whoAmI);
                    NPC.netUpdate = true;
                    break;
                case 7:
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (NPC.ai[2] == 30)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath59 with { Pitch = 1.2f }, NPC.position);
                        RazzleDazzle();
                        if (!Main.dedServ && NPC.type == NPCType<Nebuleus>())
                        {
                            if (phase <= 1)
                            {
                                if (RedeBossDowned.nebDeath < 2)
                                {
                                    DialogueChain chain = new();
                                    chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Interval.1"), nebColor, nebColor2, voice, .02f, 2f, .5f, true, bubble: Bubble, modifier: modifier));
                                    ChatUI.Visible = true;
                                    ChatUI.Add(chain);
                                }
                            }
                            else if (phase == 2)
                            {
                                if (RedeBossDowned.nebDeath < 3)
                                {
                                    DialogueChain chain = new();
                                    chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Interval.2"), nebColor, nebColor2, voice, .02f, 2f, .5f, true, bubble: Bubble, modifier: modifier));
                                    ChatUI.Visible = true;
                                    ChatUI.Add(chain);
                                }
                            }
                            else if (phase == 3)
                            {
                                if (RedeBossDowned.nebDeath < 4)
                                {
                                    DialogueChain chain = new();
                                    chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Interval.3"), nebColor, nebColor2, voice, .02f, 2f, .5f, true, bubble: Bubble, modifier: modifier));
                                    ChatUI.Visible = true;
                                    ChatUI.Add(chain);
                                }
                            }
                        }
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
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (phase <= 1 && RedeBossDowned.nebDeath < 2)
                                RedeBossDowned.nebDeath = 2;
                            if (phase == 2 && RedeBossDowned.nebDeath < 3)
                                RedeBossDowned.nebDeath = 3;
                            if (phase == 3 && RedeBossDowned.nebDeath < 4)
                                RedeBossDowned.nebDeath = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        NPC.ai[2] = 0;
                        if (phase is 3)
                        {
                            ResetVars(player);
                            ID = 13;
                            CopyList.Remove(ID);
                        }
                        else if (phase is 2)
                        {
                            ResetVars(player);
                            ID = 14;
                            CopyList.Remove(ID);
                        }
                        else
                            NPC.ai[0] = 4;
                        NPC.netUpdate = true;
                    }
                    break;
                case 8:
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if ((RedeBossDowned.nebDeath < 5 && NPC.life < (int)(NPC.lifeMax * 0.01f)) ? NPC.ai[2] == 30 : NPC.ai[2] == 4910)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath59 with { Pitch = 1.2f }, NPC.position);
                        RazzleDazzle();
                    }
                    if (Main.windSpeedTarget < 1)
                        Main.windSpeedTarget = 1;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 1200);
                    if (!Main.dedServ && NPC.ai[2] == 100)
                    {
                        string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.1");
                        string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.2");
                        string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.3");
                        string s4 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.4");
                        string s5 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.5");
                        string s6 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.6");
                        string s7 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.7");
                        string s8 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.8");
                        string s9 = RedeWorld.Alignment >= 0 ? Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.9") : Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.9Alt");
                        string s10 = RedeWorld.Alignment >= 0 ? Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.10") : Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.10Alt");
                        string s11 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.11");
                        string s12 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.11Alt");
                        bool endEarly = RedeWorld.Alignment >= 0 && RedeBossDowned.nebDeath >= 5;
                        DialogueChain chain = new();
                        chain.Add(new(NPC, s1, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s2, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s3, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s4, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s5, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s6, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s7, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s8, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s9, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s10, nebColor, nebColor2, voice, .03f, 2f, endEarly ? .5f : 0, endEarly, bubble: Bubble, modifier: modifier, endID: endEarly ? 1 : 0));
                        if (RedeWorld.Alignment >= 0 && RedeBossDowned.nebDeath < 5)
                            chain.Add(new(NPC, s11, nebColor, nebColor2, voice, .03f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                        if (RedeWorld.Alignment < 0)
                            chain.Add(new(NPC, s12, nebColor, nebColor2, voice, .03f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (RedeWorld.Alignment >= 0)
                    {
                        if (NPC.ai[2] >= 5000)
                        {
                            NPC.life = 1;
                            NPC.ai[2] = 0;
                            NPC.ai[0] = 11;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (NPC.ai[2] >= 5000)
                        {
                            if (!RedeBossDowned.downedSlayer && Main.netMode != NetmodeID.MultiplayerClient)
                                ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromKey("Mods.Redemption.UI.Chalice.NebChoice"), 180, 30, 0, Color.DarkGoldenrod);

                            player.Redemption().yesChoice = false;
                            player.Redemption().noChoice = false;

                            NPC.life = 1;
                            NPC.ai[2] = 0;
                            NPC.ai[0] = 9;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case 9:
                    NPC.LookAtEntity(player);
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 1200);
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
                    YesNoUI.DisplayYesNoButtons(player, Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.Spare"), Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.Fight"), new Vector2(0, 15), new Vector2(0, 15), .75f, .75f);
                    if (player.Redemption().yesChoice)
                    {
                        if (ChaliceAlignmentUI.Visible)
                            ChaliceAlignmentUI.Visible = false;
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 11;
                        NPC.netUpdate = true;
                    }
                    else if (player.Redemption().noChoice)
                    {
                        if (!Main.dedServ)
                            ChaliceAlignmentUI.Visible = false;
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 10;
                        NPC.netUpdate = true;
                    }
                    break;
                case 10: // Phase 2
                    NPC.LookAtEntity(player);
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    if (transitionMusicStart && !Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossStarGod2");
                    NPC.ai[2]++;
                    if (!Main.dedServ && NPC.ai[2] == 30)
                    {
                        float dialogueWait = (2f + RedeConfigServer.Instance.DialogueWaitTime) * 60;
                        transitionMusicTimer += 24 + 37 + 57 + 34;
                        transitionMusicPauseTimer += 6 + (dialogueWait * 4);
                        float dialogueSpeed = .03f - RedeConfigServer.Instance.DialogueSpeed;
                        dialogueSpeed = MathHelper.Max(dialogueSpeed, 0.01f);
                        transitionMusicTimer *= dialogueSpeed * 60;
                        transitionMusicTimer += transitionMusicPauseTimer + 30;

                        string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.12");
                        string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.13");
                        string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.14");
                        string s4 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.15");
                        DialogueChain chain = new();
                        chain.Add(new(NPC, s1, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s2, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s3, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                             .Add(new(NPC, s4, nebColor, nebColor2, voice, .03f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (NPC.ai[2] >= 30)
                        TransitionMusic();
                    if (NPC.ai[2] >= 5000)
                    {
                        transforming = true;
                        if (RedeBossDowned.nebDeath < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.nebDeath = 5;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        for (int p = 0; p < Main.maxPlayers; p++)
                        {
                            if (!Main.player[p].active && Main.player[p].statLife >= Main.player[p].statLifeMax2)
                                continue;
                            Main.player[p].statLife += Main.player[p].statLifeMax2;
                            Main.player[p].HealEffect(Main.player[p].statLifeMax2);
                        }
                        NPC.NPCLoot();
                        if (!Main.expertMode && Main.rand.NextBool(7))
                        {
                            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusMask>());
                            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusVanity>());
                        }

                        NPC.SetDefaults(NPCType<Nebuleus2>());
                        NPC.netUpdate = true;
                    }
                    break;
                case 11: // Spared
                    NPC.LookAtEntity(player);
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 1200);
                    NPC.ai[2]++;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
                    if (RedeWorld.Alignment >= 0)
                    {
                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Teleport1 with { Volume = 1 }, NPC.position);
                        if (RedeBossDowned.nebDeath < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.nebDeath = 5;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.StrikeInstantKill();
                    }
                    else
                    {
                        if (!Main.dedServ)
                        {
                            if (NPC.ai[2] == 30)
                            {
                                string s1 = RedeWorld.Alignment >= 0 ? Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.Spare1") : Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Transition.Spare2");
                                DialogueChain chain = new();
                                chain.Add(new(NPC, "...", nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                     .Add(new(NPC, s1, nebColor, nebColor2, voice, .03f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                                chain.OnEndTrigger += Chain_OnEndTrigger;
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);
                            }
                        }
                        if (NPC.ai[2] > 2000)
                        {
                            NPC.dontTakeDamage = false;
                            NPC.netUpdate = true;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Teleport1 with { Volume = 1 }, NPC.position);
                            if (RedeBossDowned.nebDeath < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.nebDeath = 5;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.StrikeInstantKill();
                        }
                    }
                    break;
            }
            #region Teleporting
            if (Vector2.Distance(NPC.Center, player.Center) >= 950 && NPC.ai[0] > 0 && NPC.ai[1] != 4 && NPC.ai[1] != 5 && NPC.ai[1] != 6 && NPC.ai[1] != 3 && !player.GetModPlayer<ScreenPlayer>().lockScreen)
            {
                Teleport(false, Vector2.Zero);
                NPC.netUpdate = true;
            }
            #endregion
        }
        public override void PostAI()
        {
            #region Frames & Animations
            if (NPC.ai[3] != 6)
            {
                NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 70) / 2;
                NPC.position.X += (float)Math.Sin(NPC.localAI[0]++ / 60) / 4;
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
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 4)
                            armFrame = 0;
                    }
                    break;
                case 1: // Pray Idle
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 6)
                            armFrame = 5;
                    }
                    break;
                case 2: // Pray End
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 8)
                            ArmAnimation(0, true);
                    }
                    break;
                case 3: // Starfall Idle
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 4)
                            armFrame = 2;
                    }
                    break;
                case 4: // Starfall End
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 8)
                            ArmAnimation(0, true);
                    }
                    break;
                case 5: // Piercing Nebula Throw
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 9)
                            ArmAnimation(0, true);
                    }
                    break;
                case 6: // Charge
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 392;

                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    if (NPC.velocity.X < 0)
                        NPC.spriteDirection = -1;
                    else
                        NPC.spriteDirection = 1;
                    break;
                case 7: // Chain Throw
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 6)
                            armFrame = 4;
                    }
                    break;
                case 8: // Long Charge-Up
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 19)
                            ArmAnimation(0, true);
                    }
                    break;
                case 9: // Book Open
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 7)
                            armFrame = 5;
                    }
                    break;
                case 10: // Book Close
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 11)
                            ArmAnimation(0, true);
                    }
                    break;
            }
            #endregion
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            NPC.ai[2] = 5000;
        }
        private bool transitionMusicStart;
        private float transitionMusicTimer;
        private float transitionMusicPauseTimer;
        private void TransitionMusic()
        {
            if (((int)transitionMusicTimer) == 190 && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Transformation, NPC.position);
            if (((int)transitionMusicTimer) == 87)
                NPC.Shoot(NPC.Center, ProjectileType<Transition>(), 0, Vector2.Zero);
            if (((int)transitionMusicTimer) == 0)
                NPC.Shoot(NPC.Center, ProjectileType<ShockwaveBoom2>(), 0, Vector2.Zero);
            if (--transitionMusicTimer <= 580)
                transitionMusicStart = true;
        }
        public override bool CheckDead()
        {
            if (NPC.ai[0] == 11)
                return true;

            NPC.life = 1;
            NPC.netUpdate = true;
            return false;
        }

        #region Methods
        private void ResetVars(Player player)
        {
            repeat = 0;
            NPC.LookAtEntity(player);
            Teleport(false, Vector2.Zero);
            frameCounters = 0;
            NPC.rotation = 0f;
            NPC.velocity = Vector2.Zero;
            ArmAnimation(0, true);
            NPC.ai[0] = 5;
            NPC.ai[2] = 0;
            circleRadius = 800;
        }
        private void Dash(int speed, bool directional, Vector2 target)
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
        private void Teleport(bool specialPos, Vector2 teleportPos)
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
                for (int d = 0; d < 16; d++)
                    ParticleManager.NewParticle(NPC.Center, RedeHelper.Spread(6), new RainbowParticle(), Color.White, Main.rand.NextFloat(1f, 1.4f), AI4: Main.rand.Next(20, 40));

                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.IndianRed, 5, 0.75f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Pink, 5, 1.5f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Purple, 5, 2.25f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Blue, 5, 3f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            }
        }
        private void ArmAnimation(int ID, bool resetFrame = false)
        {
            NPC.ai[3] = ID;
            if (resetFrame)
                armFrame = 0;
        }
        #endregion
        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
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
                if (++frameCounters >= 5)
                {
                    frameCounters = 0;
                    if (++armFrame >= 4)
                        armFrame = 0;
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.DamageType == DamageClass.Melee)
                modifiers.FinalDamage *= 1.5f;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Redemption().TechnicallyMelee)
                modifiers.FinalDamage *= 1.5f;
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.type == NPCType<Nebuleus_Clone>())
                return RedeColor.NebColour * NPC.Opacity;
            return base.GetAlpha(drawColor);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = TextureAssets.Npc[NPC.type];
            armsAni ??= Request<Texture2D>(Texture + "_Arms_Idle");
            armsPrayAni ??= Request<Texture2D>(Texture + "_Arms_Pray");
            armsPrayGlow ??= Request<Texture2D>(Texture + "_Arms_Pray_Glow");
            armsStarfallAni ??= Request<Texture2D>(Texture + "_Arms_Starfall");
            armsStarfallGlow ??= Request<Texture2D>(Texture + "_Arms_Starfall_Glow");
            armsChainAni ??= Request<Texture2D>(Texture + "_Arms_CosmicChain");
            armsChainGlow ??= Request<Texture2D>(Texture + "_Arms_CosmicChain_Glow");
            armsEyesAni ??= Request<Texture2D>(Texture + "_Arms_LongCharge");
            armsEyesGlow ??= Request<Texture2D>(Texture + "_Arms_LongCharge_Glow");
            armsBookAni ??= Request<Texture2D>(Texture + "_Arms_Constellations");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.HallowBossDye);
            Vector2 drawCenter = new(NPC.Center.X, NPC.Center.Y);
            if (NPC.type == NPCType<Nebuleus_Clone>())
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive();
            }
            if (!NPC.IsABestiaryIconDummy)
            {
                for (int k = oldPos.Length - 1; k >= 0; k -= 1)
                {
                    float alpha = 1f - (k + 1) / (float)(oldPos.Length + 2);
                    spriteBatch.Draw(texture.Value, oldPos[k] - screenPos, NPC.frame, Main.DiscoColor * (0.5f * alpha), oldrot[k], NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }
            if (NPC.ai[1] == 5 && NPC.ai[2] > 50 && NPC.ai[0] < 6)
            {
                for (int i = 0; i < ChainPos.Length; i++)
                {
                    RedeHelper.DrawBezier(spriteBatch, chain.Value, "", Main.DiscoColor, NPC.Center, ChainPos[i], (NPC.Center + ChainPos[i]) / 2 + new Vector2(0, 130 + (int)(Math.Sin(NPC.ai[2] / 12) * (150 - NPC.ai[2] / 3))), (NPC.Center + ChainPos[i]) / 2 + new Vector2(0, 130 + (int)(Math.Sin(NPC.ai[2] / 12) * (150 - NPC.ai[2] / 3))), 0.04f, 0);
                }
            }
            spriteBatch.Draw(texture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

                spriteBatch.Draw(wings.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, NPC.type == NPCType<Nebuleus_Clone>() ? BlendState.Additive : BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            if (NPC.ai[3] != 6)
            {
                switch (NPC.ai[3])
                {
                    case 0:
                        int height = armsAni.Value.Height / 4;
                        int y = height * armFrame;
                        spriteBatch.Draw(armsAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 1 or 2:
                        height = armsPrayAni.Value.Height / 8;
                        y = height * armFrame;
                        spriteBatch.Draw(armsPrayAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPrayAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsPrayAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsPrayGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPrayAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsPrayAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 3 or 4:
                        height = armsStarfallAni.Value.Height / 8;
                        y = height * armFrame;
                        spriteBatch.Draw(armsStarfallAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsStarfallAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsStarfallAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsStarfallGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsStarfallAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsStarfallAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 5:
                        height = armsPiercingAni.Value.Height / 9;
                        y = height * armFrame;
                        spriteBatch.Draw(armsPiercingAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPiercingAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsPiercingAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsPiercingGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPiercingAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsPiercingAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 7:
                        height = armsChainAni.Value.Height / 7;
                        y = height * armFrame;
                        spriteBatch.Draw(armsChainAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsChainAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsChainAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsChainGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsChainAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsChainAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 8:
                        height = armsEyesAni.Value.Height / 19;
                        y = height * armFrame;
                        spriteBatch.Draw(armsEyesAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsEyesAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsEyesAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsEyesGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsEyesAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsEyesAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 9 or 10:
                        height = armsBookAni.Value.Height / 11;
                        y = height * armFrame;
                        spriteBatch.Draw(armsBookAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsBookAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsBookAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                }
            }
            if (NPC.type == NPCType<Nebuleus_Clone>())
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public void Shout(string name, Color? color = default)
        {
            shoutColor = color ?? new Color(255, 201, 226);
            shout = true;
            shoutTimer = 0;
            shoutName = name;
        }
        public bool shout;
        private string shoutName;
        private int shoutTimer;
        private float shoutOpacity;
        private Color shoutColor;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.type != NPCType<Nebuleus>())
                return;
            if (shout)
            {
                shoutTimer++;
                if (shoutTimer < 20)
                    shoutOpacity += 0.05f;
                else if (shoutTimer >= 40)
                    shoutOpacity -= 0.05f;
                shoutOpacity = MathHelper.Clamp(shoutOpacity, 0, 1);
                int textLength = (int)(FontAssets.DeathText.Value.MeasureString(shoutName).X * (shoutColor != new Color(255, 201, 226) ? 1.3f : 1));
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, shoutName, NPC.Center - new Vector2(textLength / 2, 140) - screenPos, shoutColor * shoutOpacity, 0, Vector2.Zero, Vector2.One * (shoutColor != new Color(255, 201, 226) ? 1.3f : 1));
                if (shoutTimer > 80)
                {
                    shoutOpacity = 0;
                    shout = false;
                    shoutTimer = 0;
                }
            }
        }
    }
}