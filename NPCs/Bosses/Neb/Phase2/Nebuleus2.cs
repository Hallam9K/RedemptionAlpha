using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.Particles;
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
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    [AutoloadBossHead]
    public class Nebuleus2 : ModNPC
    {
        public static Asset<Texture2D> wings;
        private Asset<Texture2D> book;
        private Asset<Texture2D> armsAni;
        private Asset<Texture2D> armsPrayAni;
        private Asset<Texture2D> armsPrayGlow;
        private Asset<Texture2D> armsStarfallAni;
        private Asset<Texture2D> armsStarfallGlow;
        public static Asset<Texture2D> armsPiercingAni;
        public static Asset<Texture2D> armsPiercingGlow;
        private Asset<Texture2D> armsEyesAni;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            wings = Request<Texture2D>(Texture + "_Wings");
            armsPiercingAni = Request<Texture2D>(Texture + "_Arms_PiercingNebula");
            armsPiercingGlow = Request<Texture2D>(Texture + "_Arms_PiercingNebula_Glow");
        }

        public Vector2[] oldPos = new Vector2[5];
        public float[] oldrot = new float[5];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus, Angel of the Cosmos");
            Main.npcFrameCount[NPC.type] = 9;
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
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCCelestial[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 600000;
            NPC.defense = 85;
            NPC.damage = 210;
            NPC.width = 90;
            NPC.height = 90;
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
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossStarGod2");

            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Celestial] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Nature] *= .9f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Psychic] *= 1.25f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Shadow] *= 1.1f;
        }
        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Neb.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice3 with { Volume = 2f, Pitch = -.4f };
        private readonly Color nebColor = new(255, 100, 174);
        private readonly Color nebColor2 = new(4, 0, 108);
        public readonly Vector2 modifier = new(0, -240);

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.ai[3] == 6;
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
                RazzleDazzle();

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            NPC nPC = new();
            nPC.SetDefaults(NPCType<Nebuleus>());
            Main.BestiaryTracker.Kills.RegisterKill(nPC);

            potionType = ItemID.SuperHealingPotion;
            if (!Main.expertMode && Main.rand.NextBool(7))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusMask>());
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusVanity>());
            }
            if (RedeBossDowned.nebDeath < 7)
            {
                RedeWorld.Alignment -= 6;
                ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromLiteral("..."), 120, 30, 0, Color.DarkGoldenrod);
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedNebuleus, -1);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (RedeBossDowned.nebDeath < 7)
            {
                RedeBossDowned.nebDeath = 7;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
            int Proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<NebFalling>(), 0, 0, Main.myPlayer);
            Main.npc[Proj].netUpdate = true;
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

            npcLoot.Add(ItemDropRule.Common(ItemType<LifeFragment>(), 1, 20, 40));
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

            writer.Write(attackTimer[0]);
            writer.Write(attackTimer[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            repeat = reader.ReadInt32();
            phase = reader.ReadInt32();
            ID = reader.ReadInt32();

            attackTimer[0] = reader.ReadSingle();
            attackTimer[1] = reader.ReadSingle();
        }
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
        private readonly float[] attackTimer = new float[2];
        private bool eyeFlare;
        private float eyeFlareTimer;
        private int circleRadius;
        private float teleGlowTimer;
        private bool teleGlow;
        private Vector2 teleVector;
        private readonly List<int> AttackList = new() { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
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

                if ((ID is 2 or 4 or 5 or 10 or 16 or 17 && RedeHelper.AnyProjectiles(ProjectileType<Neb_Moonbeam>())) ||
                    (ID is 2 or 4 && NPC.life > (int)(NPC.lifeMax * .75f)) ||
                    (ID is 15 or 16 or 17 && NPC.life > (int)(NPC.lifeMax * .6f)) ||
                    (ID is 18 && NPC.life > (int)(NPC.lifeMax * .3f)) ||
                    (ID is 19 && NPC.life > NPC.lifeMax / 2) ||
                    (ID is 20 && NPC.life > (int)(NPC.lifeMax * .8f)))
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

            if (NPC.ai[0] > 1)
                NPC.dontTakeDamage = false;
            else
                NPC.dontTakeDamage = true;
            switch ((int)NPC.ai[0])
            {
                case 0:
                    #region Dramatic Entrance
                    NPC.LookAtEntity(player);
                    if (RedeBossDowned.nebDeath != 5 && NPC.type == NPCType<Nebuleus2>())
                    {
                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    }

                    if (NPC.ai[2] == 1)
                    {
                        DustHelper.DrawStar(NPC.Center, 58, 5, 4, 1, 4, 2, 0, noGravity: true);
                        DustHelper.DrawStar(NPC.Center, 59, 5, 5, 1, 4, 2, 0, noGravity: true);
                        DustHelper.DrawStar(NPC.Center, 60, 5, 6, 1, 4, 2, 0, noGravity: true);
                        DustHelper.DrawStar(NPC.Center, 62, 5, 7, 1, 4, 2, 0, noGravity: true);
                        for (int d = 0; d < 32; d++)
                            ParticleManager.NewParticle<RainbowParticle>(NPC.Center, RedeHelper.Spread(8), Color.White, 1);
                    }
                    if (++NPC.ai[2] >= 10)
                    {
                        NPC.ai[0] = 1;
                        NPC.ai[2] = 0;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    #endregion
                    break;
                case 1:
                    #region Starting Dialogue
                    NPC.LookAtEntity(player);
                    NPC.ai[2]++;
                    if (RedeBossDowned.nebDeath == 5 && NPC.type == NPCType<Nebuleus2>())
                    {
                        if (NPC.ai[2] < 5000)
                            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                        if (!Main.dedServ && NPC.ai[2] == 30)
                        {
                            string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.P2.1");
                            string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.P2.2");
                            string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.P2.3");
                            string s4 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.P2.4");
                            string s5 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.P2.5");
                            string s6 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.P2.6");
                            DialogueChain chain = new();
                            chain.Add(new(NPC, s1, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s2, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s3, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s4, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s5, nebColor, nebColor2, voice, .03f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s6, nebColor, nebColor2, voice, .03f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (NPC.ai[2] >= 5080)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.nebDeath = 6;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            NPC.ai[3] = 0;
                            NPC.ai[0] = 2;
                            TitleCard.BroadcastTitle(NetworkText.FromKey("Mods.Redemption.TitleCard.Neb.Name"), 60, 90, 0.8f, Color.HotPink, NetworkText.FromKey("Mods.Redemption.TitleCard.Neb.Ultimate"));
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (NPC.ai[2] >= 120)
                        {
                            if (NPC.type == NPCType<Nebuleus2>())
                                TitleCard.BroadcastTitle(NetworkText.FromKey("Mods.Redemption.TitleCard.Neb.Name"), 60, 90, 0.8f, Color.HotPink, NetworkText.FromKey("Mods.Redemption.TitleCard.Neb.Ultimate"));

                            ArmAnimation(0, true);
                            NPC.ai[2] = 0;
                            NPC.ai[0] = 2;
                            NPC.netUpdate = true;
                        }
                    }
                    #endregion
                    break;
                case 2:
                    repeat = 0;
                    NPC.LookAtEntity(player);
                    Teleport(false, Vector2.Zero);
                    frameCounters = 0;
                    NPC.rotation = 0f;
                    NPC.velocity = Vector2.Zero;
                    ArmAnimation(0, true);
                    NPC.ai[0] = 3;
                    NPC.ai[2] = 0;
                    AttackChoice();
                    circleRadius = 800;
                    NPC.netUpdate = true;
                    break;
                case 3:
                    switch (ID)
                    {
                        #region Star Blast I
                        case 0:
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
                            if (NPC.ai[2] == 40 || NPC.ai[2] == 60)
                            {
                                int pieCut = 8;
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
                            if (NPC.ai[2] == 80)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Star Blast II
                        case 1:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                                ArmAnimation(1, true);
                            if (NPC.ai[2] > 30 && (NPC.life <= (int)(NPC.lifeMax * .5f) ? NPC.ai[2] % 3 == 0 : NPC.ai[2] % 4 == 0) && NPC.ai[2] <= 140)
                            {
                                attackTimer[0] += (float)Math.PI / 6 / 480 * NPC.ai[2];
                                if (attackTimer[0] > (float)Math.PI)
                                {
                                    attackTimer[0] -= (float)Math.PI * 2;
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(6f, 0).RotatedBy(attackTimer[0] + Math.PI / 2 * i),
                                            ProjectileType<CurvingStar_Tele2>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0f, Main.myPlayer, 1.005f);
                                    }
                                }
                            }
                            if (NPC.ai[2] == 140)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 180)
                            {
                                attackTimer[0] = 0;
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Star Blast III
                        case 2:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (circleRadius > 600)
                            {
                                circleRadius--;
                            }
                            if (NPC.ai[2] == 1)
                                circleRadius = 1100;
                            if (NPC.ai[2] == 10)
                                ArmAnimation(1, true);
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
                            if (NPC.ai[2] == 30)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int pieCut = 4;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.001f, 2);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.001f, 3);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 40)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int pieCut = 4;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.001f, 2);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(9f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.001f, 3);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(9f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 35)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int pieCut = 8;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.001f, 2);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(3f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.001f, 3);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(3f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 340)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 360)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Starfall
                        case 3:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
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

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ProjectileType<Starfall_Tele2>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -12f : 12f, 14f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 40)
                                ArmAnimation(4);
                            if (NPC.ai[2] >= 100)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Eyes of the Cosmos I
                        case 4:
                            NPC.LookAtEntity(player);
                            if (circleRadius > 900)
                            {
                                circleRadius--;
                            }
                            for (int k = 0; k < 6; k++)
                            {
                                Vector2 vectorC;
                                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                vectorC.X = (float)(Math.Sin(angle) * circleRadius);
                                vectorC.Y = (float)(Math.Cos(angle) * circleRadius);
                                Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vectorC, 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, default, 2f)];
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
                                circleRadius = 1200;
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<NebRing>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 10)
                                ArmAnimation(8, true);
                            if (NPC.ai[2] > 30 && NPC.ai[2] % 3 == 0 && NPC.ai[2] <= 180)
                            {
                                attackTimer[0] += (float)Math.PI / 15;
                                if (attackTimer[0] > (float)Math.PI)
                                {
                                    attackTimer[0] -= (float)Math.PI * 2;
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(6f, 0).RotatedBy(attackTimer[0] + Math.PI / 2),
                                        ProjectileType<CosmicEye3>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .7f)), 0f, Main.myPlayer, NPC.whoAmI);
                                }
                            }
                            if (NPC.ai[2] == 40 || NPC.ai[2] == 120)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int pieCut = 8;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.001f, 0);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(2f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] >= 360)
                            {
                                attackTimer[0] = 0;
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Eyes of the Cosmos II
                        case 5:
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
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<NebRing>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 20)
                                ArmAnimation(8, true);
                            if (NPC.ai[2] == 30)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int k = 0; k < 16; k++)
                                    {
                                        double angle = k * (Math.PI * 2 / 16);
                                        vector.X = (float)(Math.Sin(angle) * 180);
                                        vector.Y = (float)(Math.Cos(angle) * 180);
                                        NPC.Shoot(new Vector2((int)NPC.Center.X + (int)vector.X, (int)NPC.Center.Y + (int)vector.Y), ProjectileType<CosmicEye>(), (int)(NPC.damage * .7f), Vector2.Zero, CustomSounds.NebSound1, NPC.whoAmI);
                                    }
                                }
                            }
                            if (NPC.ai[2] == 100)
                                ArmAnimation(1, true);
                            if (NPC.ai[2] == 130 || NPC.ai[2] == 150)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int pieCut = 4;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.02f, 0);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 140 || NPC.ai[2] == 160)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int pieCut = 4;
                                    for (int m = 0; m < pieCut; m++)
                                    {
                                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<CurvingStar_Tele4>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .67f)), 0, Main.myPlayer, 1.02f, 1);
                                        Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), m / (float)pieCut * 6.28f);
                                        Main.projectile[projID].netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.ai[2] == 220)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 250)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Super Starfall
                        case 7:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                                ArmAnimation(3, true);
                            if (NPC.ai[2] >= 40 && NPC.ai[2] < 120)
                            {
                                if (Main.rand.NextBool(4))
                                {
                                    int A = Main.rand.Next(-200, 200) * 6;
                                    int B = Main.rand.Next(-200, 200) - 1000;

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ProjectileType<Starfall_Tele2>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -2f : 2f, 6f), SoundID.Item9 with { Volume = .5f });
                                }
                            }
                            if (NPC.ai[2] == 50)
                                ArmAnimation(4);
                            if (NPC.ai[2] >= 180)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Erratic Star Blast
                        case 8:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                                ArmAnimation(1, true);
                            if (NPC.ai[2] % 3 == 0 && NPC.ai[2] >= 30 && NPC.ai[2] <= 60)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<CurvingStar_Tele4>(), (int)(NPC.damage * .67f), new Vector2(Main.rand.Next(-7, 7), Main.rand.Next(-7, 7)), 1.01f);
                            }
                            if (NPC.ai[2] == 60)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 100)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Crystal Stars
                        case 9:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                                ArmAnimation(3, true);
                            if (NPC.ai[2] >= 40 && NPC.ai[2] < 120)
                            {
                                if (NPC.ai[2] % 12 == 0)
                                {
                                    int A = Main.rand.Next(-200, 200) * 6;
                                    int B = Main.rand.Next(-200, 200) - 1000;

                                    NPC.Shoot(new Vector2(player.Center.X + A, player.Center.Y + B), ProjectileType<CrystalStar_Tele>(), (int)(NPC.damage * .67f), new Vector2(NPC.spriteDirection != 1 ? -2f : 2f, 6f));
                                }
                            }
                            if (NPC.ai[2] == 50)
                                ArmAnimation(4);
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Chain of the Cosmos
                        case 10:
                            NPC.LookAtEntity(player);
                            if (!ScreenPlayer.NebCutscene)
                            {
                                if (circleRadius > 700)
                                    circleRadius -= 2;
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
                            if (NPC.ai[2] == 40)
                                ArmAnimation(8, true);
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
                                ArmAnimation(0, true);
                                NPC.ai[0] = 2;
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
                                ArmAnimation(0, true);
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Star Dash
                        case 11:
                            if (NPC.ai[3] != 6) { NPC.LookAtEntity(player); NPC.netUpdate = true; }
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 25)
                            {
                                NPC.Shoot(NPC.Center, ProjectileType<NebTeleLine1>(), 0, NPC.DirectionTo(player.Center + player.velocity * 20f), 115, ai1: NPC.whoAmI);
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
                                NPC.netUpdate = true;
                                if (repeat < 3) NPC.Shoot(NPC.Center, ProjectileType<GiantStar_Proj>(), (int)(NPC.damage * 0.85f), Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] > 65 && NPC.ai[2] < 86 && NPC.ai[2] % 2 == 0)
                            {
                                NPC.Shoot(NPC.Center, ProjectileType<StarBolt>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(3, NPC.rotation + MathHelper.PiOver2), SoundID.Item91);
                                NPC.Shoot(NPC.Center, ProjectileType<StarBolt>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(3, NPC.rotation - MathHelper.PiOver2), SoundID.Item91);
                            }
                            if (NPC.ai[2] > 65 && NPC.ai[2] < 86)
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
                            if (NPC.ai[2] >= 90)
                            {
                                if (repeat <= 2)
                                {
                                    repeat++;
                                    NPC.ai[2] = 24;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    repeat = 0;
                                    NPC.velocity = Vector2.Zero;
                                    NPC.ai[0] = 2;
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
                        case 12:
                            if (NPC.ai[3] != 6) { NPC.LookAtEntity(player); NPC.netUpdate = true; }
                            if (NPC.ai[2]++ == 0)
                            {
                                DashPos = Main.rand.Next(8) switch
                                {
                                    1 => new[] { new Vector2(0, -400), new(0, 400), new(0, -400) },
                                    2 => new[] { new Vector2(0, 400), new(550, 0), new(0, -400) },
                                    3 => new[] { new Vector2(0, -400), new(-550, 0), new(0, 400) },
                                    4 => new[] { new Vector2(550, 0), new(-550, 0), new(550, 0) },
                                    5 => new[] { new Vector2(-550, 0), new(550, 0), new(-550, 0) },
                                    6 => new[] { new Vector2(-550, 0), new(550, 0), new(0, 400) },
                                    7 => new[] { new Vector2(550, 0), new(-550, 0), new(0, -400) },
                                    _ => new[] { new Vector2(0, 400), new(0, -400), new(0, 400) },
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

                                NPC.Shoot(player.Center + DashPos[b], ProjectileType<Dash_Tele2>(), 0, a);
                                SoundEngine.PlaySound(SoundID.Item25 with { Pitch = .3f }, player.Center + DashPos[b]);
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], Color.Pink, shakeAmount: 0, scale: 4, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], RedeColor.NebColour, shakeAmount: 0, scale: 5, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                                RedeDraw.SpawnExplosion(player.Center + DashPos[b], Color.White, shakeAmount: 0, scale: 3, noDust: true, tex: "Redemption/Textures/WhiteFlare");
                            }
                            if (NPC.ai[2] == 10)
                            {
                                Vector2 a = Vector2.Zero;
                                int b = 1;
                                if (DashPos[b].X > 0)
                                    a = new Vector2(-6, 0);
                                else if (DashPos[b].X < 0)
                                    a = new Vector2(6, 0);
                                else if (DashPos[b].Y > 0)
                                    a = new Vector2(0, -6);
                                else if (DashPos[b].Y < 0)
                                    a = new Vector2(0, 6);

                                NPC.Shoot(player.Center + DashPos[b], ProjectileType<Dash_Tele2>(), 0, a);
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
                                    NPC.velocity.X = -35;
                                else if (DashPos[0].X < 0)
                                    NPC.velocity.X = 35;
                                else if (DashPos[0].Y > 0)
                                    NPC.velocity.Y = -35;
                                else if (DashPos[0].Y < 0)
                                    NPC.velocity.Y = 35;
                                Teleport(true, player.Center + DashPos[0]);
                            }
                            if (NPC.ai[2] == 65)
                            {
                                NPC.velocity *= 0f;
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                if (DashPos[1].X > 0)
                                    NPC.velocity.X = -35;
                                else if (DashPos[1].X < 0)
                                    NPC.velocity.X = 35;
                                else if (DashPos[1].Y > 0)
                                    NPC.velocity.Y = -35;
                                else if (DashPos[1].Y < 0)
                                    NPC.velocity.Y = 35;
                                Teleport(true, player.Center + DashPos[1]);
                            }
                            if (NPC.ai[2] == 80)
                            {
                                NPC.velocity *= 0f;
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                if (DashPos[2].X > 0)
                                    NPC.velocity.X = -35;
                                else if (DashPos[2].X < 0)
                                    NPC.velocity.X = 35;
                                else if (DashPos[2].Y > 0)
                                    NPC.velocity.Y = -35;
                                else if (DashPos[2].Y < 0)
                                    NPC.velocity.Y = 35;
                                Teleport(true, player.Center + DashPos[2]);
                            }
                            if (NPC.ai[2] == 95)
                            {
                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                NPC.velocity.X = 0;
                                NPC.velocity.Y = -30;
                                NPC.rotation = 0;
                                NPC.LookAtEntity(player);
                                Teleport(true, player.Center + new Vector2(400, 400));
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
                            if (NPC.ai[2] == 95 || NPC.ai[2] == 115 || NPC.ai[2] == 135)
                                ArmAnimation(5, true);
                            if (NPC.ai[2] == 105 || NPC.ai[2] == 125 || NPC.ai[2] == 145)
                            {
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                            }
                            if (NPC.ai[2] > 95)
                            {
                                NPC.velocity *= 0.94f;
                            }
                            if (NPC.ai[2] >= 180)
                            {
                                NPC.velocity = Vector2.Zero;
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Piercing Nebula I
                        case 13:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 20 || NPC.ai[2] == 40 || NPC.ai[2] == 60)
                                ArmAnimation(5, true);
                            if (NPC.ai[2] is 20 or 40 or 60)
                                NPC.Shoot(NPC.Center, ProjectileType<Neb2Mirage_PiercingNebula>(), (int)(NPC.damage * .67f), Vector2.Zero, NPC.whoAmI);
                            if (NPC.ai[2] == 30 || NPC.ai[2] == 50 || NPC.ai[2] == 70)
                            {
                                Teleport(false, Vector2.Zero);
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<PNebula1_Tele>(), (int)(NPC.damage * .67f), RedeHelper.PolarVector(18, (player.Center - NPC.Center).ToRotation()), NPC.whoAmI);
                            }
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Piercing Nebula II
                        case 14:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] is 30 or 32 or 50 or 52)
                                NPC.Shoot(NPC.Center, ProjectileType<Neb2Mirage_PiercingNebula>(), (int)(NPC.damage * .67f), Vector2.Zero, NPC.whoAmI);
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
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Warp Dashes
                        case 15:
                            if (NPC.ai[3] != 6) { NPC.LookAtEntity(player); NPC.netUpdate = true; }
                            NPC.ai[2]++;
                            if (NPC.ai[2] >= 100) { NPC.velocity *= 0.9f; }
                            else { NPC.velocity *= 0.98f; }
                            if (NPC.ai[2] == 5)
                            {
                                eyeFlare = true;
                                Teleport(false, Vector2.Zero);
                                NPC.velocity = -NPC.DirectionTo(player.Center) * 16;
                            }
                            if (NPC.ai[2] == 50) { Teleport(true, player.Center + new Vector2(-800, 0)); NPC.netUpdate = true; }
                            if (NPC.ai[2] == 90) { Teleport(true, player.Center + new Vector2(800, 0)); NPC.netUpdate = true; }
                            if (NPC.ai[2] == 50 || NPC.ai[2] == 90)
                            {
                                ArmAnimation(6);
                                Dash(70, false, Vector2.Zero);
                                NPC.netUpdate = true;
                            }
                            if (NPC.ai[2] % 3 == 0 && NPC.velocity.Length() > 40)
                            {
                                NPC.Shoot(NPC.Center, ProjectileType<StarBolt>(), (int)(NPC.damage * .67f), NPC.velocity.RotatedBy(Math.PI / 2) / 20, SoundID.Item117);
                                NPC.Shoot(NPC.Center, ProjectileType<StarBolt>(), (int)(NPC.damage * .67f), NPC.velocity.RotatedBy(-Math.PI / 2) / 20, SoundID.Item117);
                            }
                            if (NPC.velocity.Length() < 10)
                            {
                                if (NPC.ai[3] == 6)
                                    NPC.ai[3] = 0;
                            }
                            else
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
                            if (NPC.ai[2] >= 140 && NPC.velocity.Length() < 6)
                            {
                                NPC.velocity = Vector2.Zero;
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Shining Aurora
                        case 16:
                            NPC.LookAtEntity(player);
                            if (circleRadius > 800)
                                circleRadius--;
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
                                SoundEngine.PlaySound(SoundID.Item159);
                                circleRadius = 1300;
                                eyeFlare = true;
                                NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ProjectileType<NebRing>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (NPC.ai[2] == 10)
                                ArmAnimation(1, true);
                            if (NPC.ai[2] > 30 && NPC.ai[2] <= 300)
                            {
                                attackTimer[0] += (float)Math.PI / 3 / 300 * NPC.ai[2];
                                if (attackTimer[0] > (float)Math.PI)
                                {
                                    attackTimer[0] -= (float)Math.PI * 2;
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(1f, 0).RotatedBy(attackTimer[0] + Math.PI / 2),
                                        ProjectileType<StarBolt>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .7f)), 0f, Main.myPlayer);
                                }
                            }
                            if (NPC.ai[2] == 300)
                                ArmAnimation(2);
                            if (NPC.ai[2] >= 350)
                            {
                                attackTimer[0] = 0;
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Spiralling Shine
                        case 17:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] > 85)
                            {
                                for (int k = 0; k < 6; k++)
                                {
                                    Vector2 vectorC;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vectorC.X = (float)(Math.Sin(angle) * circleRadius);
                                    vectorC.Y = (float)(Math.Cos(angle) * circleRadius);
                                    Dust dust2 = Main.dust[Dust.NewDust(vector + vectorC, 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -NPC.DirectionTo(dust2.position) * 2f;
                                }
                                if (vector.Distance(player.Center) > circleRadius)
                                {
                                    Vector2 movement = vector - player.Center;
                                    float difference = movement.Length() - circleRadius;
                                    movement.Normalize();
                                    movement *= difference < 17f ? difference : 17f;
                                    player.position += movement;
                                }
                            }
                            if (NPC.ai[2] == 1)
                            {
                                circleRadius = 700;
                                eyeFlare = true;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.NebSound2, NPC.position);
                            }
                            if (NPC.ai[2] >= 80 && NPC.ai[2] < 342)
                            {
                                if (NPC.ai[3] != 6) { NPC.LookAtEntity(player); NPC.netUpdate = true; }
                                if (NPC.ai[2] == 85)
                                {
                                    vector = player.Center;
                                    Teleport(true, player.Center + new Vector2(0, -700));
                                    NPC.Shoot(player.Center, ProjectileType<Neb_Vortex>(), (int)(NPC.damage * .8f), Vector2.Zero);
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.EnergyCharge2 with { Pitch = -.8f }, player.Center);
                                }
                                if (NPC.ai[2] > 85 && NPC.ai[2] % 3 == 0)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(2, (vector - NPC.Center).ToRotation()), ProjectileType<StarBolt>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .7f)), 0, Main.myPlayer, NPC.whoAmI);
                                        Main.projectile[p].timeLeft = 155;
                                    }
                                }
                                if (NPC.ai[2] > 85)
                                {
                                    NPC.rotation = (vector - NPC.Center).ToRotation();
                                }
                                if (NPC.ai[2] > 85 && attackTimer[0] < 90)
                                {
                                    ArmAnimation(6);
                                    attackTimer[0] += 3;
                                    NPC.Center = vector + new Vector2(0f, -1f).RotatedBy(MathHelper.ToRadians(attackTimer[0])) * 700;
                                }
                                if (attackTimer[0] == 90)
                                {
                                    Teleport(true, vector + new Vector2(0, 700));
                                }
                                if (attackTimer[0] >= 90 && attackTimer[0] < 180)
                                {
                                    ArmAnimation(6);
                                    attackTimer[0] += 3;
                                    NPC.Center = vector + new Vector2(0f, -1f).RotatedBy(MathHelper.ToRadians(attackTimer[0] + 90)) * 700;
                                }
                                if (attackTimer[0] == 180)
                                {
                                    Teleport(true, vector + new Vector2(700, 0));
                                }
                                if (attackTimer[0] >= 180 && attackTimer[0] < 270)
                                {
                                    ArmAnimation(6);
                                    attackTimer[0] += 3;
                                    NPC.Center = vector + new Vector2(0f, -1f).RotatedBy(MathHelper.ToRadians(attackTimer[0] - 90)) * 700;
                                }
                                if (attackTimer[0] == 270)
                                {
                                    Teleport(true, vector + new Vector2(-700, 0));
                                }
                                if (attackTimer[0] >= 270 && attackTimer[0] < 360)
                                {
                                    ArmAnimation(6);
                                    attackTimer[0] += 3;
                                    NPC.Center = vector + new Vector2(0f, -1f).RotatedBy(MathHelper.ToRadians(attackTimer[0])) * 700;
                                }
                            }
                            if (attackTimer[0] == 360)
                            {
                                NPC.rotation = 0;
                                NPC.velocity *= 0;
                                attackTimer[0] = 0;
                                NPC.netUpdate = true;
                            }
                            if (NPC.ai[2] == 342)
                            {
                                Teleport(false, Vector2.Zero);
                                ArmAnimation(0, true);
                                NPC.rotation = 0;
                                NPC.velocity *= 0;
                                attackTimer[0] = 0;
                            }
                            if (NPC.ai[2] >= 460)
                            {
                                NPC.rotation = 0;
                                NPC.velocity *= 0;
                                attackTimer[0] = 0;
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Tome: Moonbeam
                        case 18:
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
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Tome: Meteoric Swarm
                        case 19:
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
                                    NPC.Shoot(player.Center + new Vector2(Main.rand.Next(-(Main.screenWidth / 2), Main.screenWidth / 2), Main.rand.Next(-(Main.screenHeight / 2), Main.screenHeight / 2)), ProjectileType<Neb_Meteor_Tele>(), (int)(NPC.damage * .67f), RedeHelper.Spread(2), 0, 1);
                            }
                            if (NPC.ai[2] == 400)
                                ArmAnimation(10);
                            if (NPC.ai[2] >= 460)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Tome: Cosmic Lightning
                        case 20:
                            NPC.LookAtEntity(player);
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                            {
                                Shout(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Shout.CosmicFulgura"), new(193, 255, 219));
                                ArmAnimation(9, true);
                            }
                            if (NPC.ai[2] > 20 && NPC.ai[2] % 30 == 0 && NPC.ai[2] < 340)
                            {
                                int rand = Main.rand.Next(4, 6);
                                for (int i = 0; i < rand; i++)
                                    NPC.Shoot(player.Center + new Vector2(Main.rand.Next(-(Main.screenWidth / 2), Main.screenWidth / 2), -1500), ProjectileType<Neb_Lightning_Tele>(), (int)(NPC.damage * .67f), new Vector2(Main.rand.Next(-12, 13), 0));
                            }
                            if (NPC.ai[2] == 310)
                                ArmAnimation(10);
                            if (NPC.ai[2] >= 340)
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
            }
            if (NPC.DistanceSQ(player.Center) <= 200 * 200)
            {
                player.AddBuff(BuffType<NebHealBuff>(), 20);
            }
            #region Teleporting
            if (Vector2.Distance(NPC.Center, player.Center) >= 950 && NPC.ai[0] > 0 && NPC.ai[1] != 2 && NPC.ai[1] != 4 && NPC.ai[1] != 5 && NPC.ai[1] != 10 && NPC.ai[1] != 11 && NPC.ai[1] != 12 && NPC.ai[1] < 15 && !player.GetModPlayer<ScreenPlayer>().lockScreen)
            {
                Teleport(false, Vector2.Zero);
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
                    NPC.frame.Y += 124;
                    if (NPC.frame.Y > 868)
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
                        if (++armFrame >= 4)
                            armFrame = 2;
                    }
                    break;
                case 2: // Pray End
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 5)
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
                        if (++armFrame >= 7)
                            ArmAnimation(0, true);
                    }
                    break;
                case 5: // Piercing Nebula Throw
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 6)
                            ArmAnimation(0, true);
                    }
                    break;
                case 6: // Charge
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 992;

                    if (NPC.ai[1] != 17)
                        NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    if (NPC.velocity.X < 0)
                        NPC.spriteDirection = -1;
                    else
                        NPC.spriteDirection = 1;
                    break;
                case 8: // Eyes Punch
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++armFrame >= 8)
                            ArmAnimation(0, true);
                    }
                    break;
                case 9: // Book Open
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (++bookFrame >= 6)
                            bookFrame = 5;
                        if (++armFrame >= 4)
                            armFrame = 2;
                    }
                    break;
                case 10: // Book Close
                    if (++frameCounters >= 5)
                    {
                        frameCounters = 0;
                        if (--bookFrame < 0)
                            ArmAnimation(0, true);
                        if (--armFrame < 0)
                            armFrame = 0;
                    }
                    break;
            }
            if (eyeFlare)
            {
                eyeFlareTimer++;
                if (eyeFlareTimer > 60)
                {
                    eyeFlare = false;
                    eyeFlareTimer = 0;
                }
            }
            if (teleGlow)
            {
                teleGlowTimer += 3;
                if (teleGlowTimer > 60)
                {
                    teleGlow = false;
                    teleGlowTimer = 0;
                }
            }
            #endregion
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            NPC.ai[2] = 5000;
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
            Player player = Main.player[NPC.target];
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Blue * 0.4f, 5, 0.8f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Purple * 0.4f, 5, 1.6f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Pink * 0.4f, 5, 2.4f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.IndianRed * 0.4f, 5, 3.2f, 1, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            teleGlow = true;
            teleGlowTimer = 0;
            teleVector = NPC.Center;
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
                    NPC.Center = teleportPos;
                    NPC.netUpdate = true;
                }
            }
            teleVector = NPC.Center;
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Teleport2, NPC.position);
            player.GetModPlayer<ScreenPlayer>().Rumble(5, 6);
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

                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.IndianRed, 5, 0.8f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Pink, 5, 1.6f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Purple, 5, 2.4f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
                DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Blue, 5, 3.2f, 2, 0.7f, 2, 0, ai1: Main.rand.Next(50, 60));
            }
        }
        private void ArmAnimation(int ID, bool resetFrame = false)
        {
            NPC.ai[3] = ID;
            if (resetFrame)
            {
                armFrame = 0;
                bookFrame = 0;
            }
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
                    NPC.frame.Y += 124;
                    if (NPC.frame.Y > 868)
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
            if (NPC.type == NPCType<Nebuleus2_Clone>())
                return RedeColor.NebColour * NPC.Opacity;
            return base.GetAlpha(drawColor);
        }
        private int bookFrame;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = TextureAssets.Npc[NPC.type];
            book ??= Request<Texture2D>(Texture + "_Book");
            armsAni ??= Request<Texture2D>(Texture + "_Arms_Idle");
            armsPrayAni ??= Request<Texture2D>(Texture + "_Pray");
            armsPrayGlow ??= Request<Texture2D>(Texture + "_Pray_Glow");
            armsStarfallAni ??= Request<Texture2D>(Texture + "_Starfall");
            armsStarfallGlow ??= Request<Texture2D>(Texture + "_Starfall_Glow");
            armsEyesAni ??= Request<Texture2D>(Texture + "_Punch");

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
            if (NPC.ai[1] == 10 && NPC.ai[2] > 50)
            {
                for (int i = 0; i < ChainPos.Length; i++)
                {
                    RedeHelper.DrawBezier(spriteBatch, Nebuleus.chain.Value, "", Main.DiscoColor, NPC.Center, ChainPos[i], (NPC.Center + ChainPos[i]) / 2 + new Vector2(0, 130 + (int)(Math.Sin(NPC.ai[2] / 12) * (150 - NPC.ai[2] / 3))), (NPC.Center + ChainPos[i]) / 2 + new Vector2(0, 130 + (int)(Math.Sin(NPC.ai[2] / 12) * (150 - NPC.ai[2] / 3))), 0.04f, 0);
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
                spriteBatch.Begin(SpriteSortMode.Deferred, NPC.type == NPCType<Nebuleus2_Clone>() ? BlendState.Additive : BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
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
                        height = armsPrayAni.Value.Height / 5;
                        y = height * armFrame;
                        spriteBatch.Draw(armsPrayAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPrayAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsPrayAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsPrayGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPrayAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsPrayAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 3 or 4 or 9 or 10:
                        height = armsStarfallAni.Value.Height / 7;
                        y = height * armFrame;
                        spriteBatch.Draw(armsStarfallAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsStarfallAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsStarfallAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsStarfallGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsStarfallAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsStarfallAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);

                        if (NPC.ai[3] is 9 or 10)
                        {
                            height = book.Value.Height / 6;
                            y = height * bookFrame;
                            spriteBatch.Draw(book.Value, drawCenter - new Vector2(0, 60 + ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 20) / 3)) - screenPos, new Rectangle?(new Rectangle(0, y, book.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(book.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        }
                        break;
                    case 5:
                        height = armsPiercingAni.Value.Height / 6;
                        y = height * armFrame;
                        spriteBatch.Draw(armsPiercingAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPiercingAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsPiercingAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        spriteBatch.Draw(armsPiercingGlow.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsPiercingAni.Value.Width, height)), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(armsPiercingAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                    case 8:
                        height = armsEyesAni.Value.Height / 8;
                        y = height * armFrame;
                        spriteBatch.Draw(armsEyesAni.Value, drawCenter - screenPos, new Rectangle?(new Rectangle(0, y, armsEyesAni.Value.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(armsEyesAni.Value.Width / 2f, height / 2f), NPC.scale, effects, 0f);
                        break;
                }
            }
            if (NPC.type == NPCType<Nebuleus2_Clone>())
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D flare = Request<Texture2D>("Redemption/Textures/PurpleEyeFlare").Value;
            if (eyeFlare)
            {
                Vector2 position = NPC.Center - screenPos + new Vector2(0, -14);
                RedeDraw.DrawEyeFlare(spriteBatch, ref eyeFlareTimer, position, Color.Pink, NPC.rotation, 1, 0, flare);
                Vector2 position2 = NPC.Center - screenPos + new Vector2(NPC.spriteDirection == 1 ? 8 : -8, -14);
                RedeDraw.DrawEyeFlare(spriteBatch, ref eyeFlareTimer, position2, Color.Pink, NPC.rotation, .95f, 0, flare);
            }
            spriteBatch.End();
            spriteBatch.BeginAdditive();

            Texture2D teleportGlow = Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position3 = teleVector - screenPos;
            Color colour2 = Color.Lerp(Color.White, Main.DiscoColor, 1f / teleGlowTimer * 10f) * (1f / teleGlowTimer * 10f);
            if (teleGlow)
            {
                spriteBatch.Draw(teleportGlow, position3, new Rectangle?(rect2), colour2, NPC.rotation, origin2, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(teleportGlow, position3, new Rectangle?(rect2), colour2 * 0.4f, NPC.rotation, origin2, 2f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.BeginDefault();

            if (NPC.type != NPCType<Nebuleus2>())
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
    }
}
