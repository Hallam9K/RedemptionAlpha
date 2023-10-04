using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Usable;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Lore;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Donator.Lordfunnyman;
using Terraria.GameContent;
using ReLogic.Content;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class TBot : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Friendly T-Bot");
            Main.npcFrameCount[NPC.type] = 21;
            NPCID.Sets.HatOffsetY[NPC.type] = -4;
            NPCID.Sets.ExtraFramesCount[Type] = 5;

            NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike);
            NPC.Happiness.SetBiomeAffection<SnowBiome>(AffectionLevel.Hate);

            NPC.Happiness.SetNPCAffection(NPCID.Mechanic, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Nurse, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Truffle, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.Cyborg, AffectionLevel.Hate);

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 22;
            NPC.height = 42;
            NPC.aiStyle = 7;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeAlpha>().Type, ModContent.GetInstance<LidenBiome>().Type };
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frame.Y >= 14 * frameHeight && NPC.frame.Y <= 15 * frameHeight)
                NPC.frame.Y = 2 * frameHeight;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.TBot1")),
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.TBot2"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override bool CheckDead()
        {
            RedeWorld.tbotDownedTimer = 0;
            Main.NewText(Language.GetTextValue("Mods.Redemption.DialogueBox.TBot.Unconscious"), Color.Red.R, Color.Red.G, Color.Red.B);
            NPC.SetDefaults(ModContent.NPCType<TBotUnconscious>());
            NPC.life = 1;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            return false;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return RedeBossDowned.downedSeed && !NPC.AnyNPCs(ModContent.NPCType<TBotUnconscious>()) && !NPC.AnyNPCs(ModContent.NPCType<TBot_Intro>()) && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<AdamPortal>());
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Adam" };
        }
        public override ITownNPCProfile TownNPCProfile() => new TBotProfile();
        public override string GetChat()
        {
            NextPage = false;
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);

            int GuideID = NPC.FindFirstNPC(NPCID.Guide);
            if (GuideID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.GuideDialogue", Main.npc[GuideID].GivenName));

            int MerchantID = NPC.FindFirstNPC(NPCID.Merchant);
            if (MerchantID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.MerchantDialogue", Main.npc[MerchantID].GivenName));

            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            if (DryadID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.DryadDialogue", Main.npc[DryadID].GivenName));

            int NurseID = NPC.FindFirstNPC(NPCID.Nurse);
            if (NurseID >= 0 && RedeBossDowned.nukeDropped)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.NurseDialogue", Main.npc[NurseID].GivenName));

            int ArmsDealerID = NPC.FindFirstNPC(NPCID.ArmsDealer);
            if (ArmsDealerID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.ArmsDealerDialogue", Main.npc[ArmsDealerID].GivenName));

            int cyborgID = NPC.FindFirstNPC(NPCID.Cyborg);
            if (cyborgID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.CyborgDialogue", Main.npc[cyborgID].GivenName));

            if (Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue1"), 1.5);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue2"), 1.5);
            }

            if (NPC.downedPlantBoss)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue3"), 1.5);

            if (NPC.downedGolemBoss)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue4"), 1.5);

            if (NPC.downedMoonlord)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue5"), 1.5);

            if (RedeBossDowned.downedOmega1 || RedeBossDowned.downedOmega2)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue6"), 1.5);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue7"), 1.5);
            }

            if (RedeBossDowned.downedSlayer)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue8"), 1.5);
            if (RedeBossDowned.downedVolt)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue9"), 2);
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<AdamHead>(), true))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.AdamHeadDialogue1"), 3);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.AdamHeadDialogue2"), 3);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.AdamHeadDialogue3"), 3);
            }
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<VoltHead>(), true))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.VoltHeadDialogue1"), 3);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.VoltHeadDialogue2"), 3);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.VoltHeadDialogue3"), 3);
                if (BasePlayer.HasChestplate(player, ModContent.ItemType<AndroidArmour>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<AndroidPants>(), true))
                {
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.VoltDialogue1"), 3);
                    chat.Add("...", 3);
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.VoltDialogue2"), 3);
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.VoltDialogue3"), 3);
                }
            }
            if (BasePlayer.HasItem(player, ModContent.ItemType<NuclearWarhead>()))
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue10"), 1.5);

            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue11"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue12"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue13"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue14"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue15"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue16"));
            if (RedeBossDowned.downedJanitor)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue17"), 1.5);
            if (RedeBossDowned.nukeDropped || RedeBossDowned.downedJanitor)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue18"), 1.5);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue19"), 1.5);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue20"), 1.5);
            }
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue21"));
            return chat;
        }

        public static bool NextPage;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");

            button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.TBot.FloppyDisc");
            if (NextPage)
                button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.TBot.NextPage");
            if (FDisk >= 20)
                button2 += Language.GetTextValue("Mods.Redemption.DialogueBox.TBot.Next");
        }

        public static int FDisk;
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.player[Main.myPlayer];
            if (firstButton)
                shopName = "Shop";
            else
            {
                FDisk = 0;
                int heldItem = player.HeldItem.type;
                if (heldItem == ModContent.ItemType<FloppyDisk1>())
                {
                    FDisk = 1;//
                    if (NextPage)
                    {
                        FDisk = 21;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<FloppyDisk2>())
                    FDisk = 2;
                else if (heldItem == ModContent.ItemType<FloppyDisk2_1>())
                    FDisk = 3;
                else if (heldItem == ModContent.ItemType<FloppyDisk3>())
                    FDisk = 4;
                else if (heldItem == ModContent.ItemType<FloppyDisk3_1>())
                {
                    FDisk = 5;//
                    if (NextPage)
                    {
                        FDisk = 25;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<FloppyDisk5>())
                    FDisk = 6;
                else if (heldItem == ModContent.ItemType<FloppyDisk5_1>())
                    FDisk = 7;
                else if (heldItem == ModContent.ItemType<FloppyDisk5_2>())
                    FDisk = 8;
                else if (heldItem == ModContent.ItemType<FloppyDisk5_3>())
                    FDisk = 9;
                else if (heldItem == ModContent.ItemType<FloppyDisk6>())
                {
                    FDisk = 10;//
                    if (NextPage)
                    {
                        FDisk = 30;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<FloppyDisk6_1>())
                    FDisk = 11;
                else if (heldItem == ModContent.ItemType<FloppyDisk7>())
                    FDisk = 12;
                else if (heldItem == ModContent.ItemType<FloppyDisk7_1>())
                {
                    FDisk = 13;//
                    if (NextPage)
                    {
                        FDisk = 33;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<AIChip>())
                    FDisk = 14;
                else if (heldItem == ModContent.ItemType<MemoryChip>())
                    FDisk = 16;
                if (FDisk != 1 && FDisk != 5 && FDisk != 10 && FDisk != 13)
                    NextPage = false;

                Main.npcChatText = DiskChat();
            }
        }
        public static string DiskChat()
        {
            return FDisk switch
            {
                1 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk1"),
                2 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk2"),
                3 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk3"),
                4 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk4"),
                5 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk5"),
                6 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk6"),
                7 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk7"),
                8 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk8"),
                9 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk9"),
                10 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk10"),
                11 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk11"),
                12 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk12"),
                13 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk13"),
                14 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.AIChipLine"),
                16 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.MemoryChipLine"),
                21 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk14"),
                25 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk15"),
                30 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk16"),
                33 => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.FloppyDisk17"),
                _ => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.NoFloppyDisk")
            };
        }
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add<NuclearWarhead>(Condition.Hardmode)
                .Add<IrradiatedStone>(RedeConditions.NukeDropped, Condition.InGraveyard)
                .Add<IrradiatedGrassSeeds>(RedeConditions.NukeDropped, Condition.InGraveyard)
                .Add<IrradiatedCrimstone>(RedeConditions.NukeDropped, Condition.InGraveyard, Condition.BloodMoon, Condition.CrimsonWorld)
                .Add<IrradiatedCrimsonGrassSeeds>(RedeConditions.NukeDropped, Condition.InGraveyard, Condition.BloodMoon, Condition.CrimsonWorld)
                .Add<IrradiatedEbonstone>(RedeConditions.NukeDropped, Condition.InGraveyard, Condition.BloodMoon, Condition.CorruptWorld)
                .Add<IrradiatedCorruptGrassSeeds>(RedeConditions.NukeDropped, Condition.InGraveyard, Condition.BloodMoon, Condition.CorruptWorld)
                .Add<CrystalSerum>(RedeConditions.NukeDropped)
                .Add<BleachedSolution>(RedeConditions.NukeDropped)
                .Add<GasMask>(RedeConditions.NukeDropped)
                .Add<HazmatSuit4>(RedeConditions.NukeDropped, RedeConditions.IsFinlandDay)
                .Add<HazmatSuit>(RedeConditions.NukeDropped, RedeConditions.IsNotFinlandDay)
                .Add<AIChip>(Condition.Hardmode)
                .Add<CarbonMyofibre>(Condition.Hardmode)
                .Add<Capacitor>(Condition.Hardmode)
                .Add<Plating>(Condition.Hardmode)
                .Add<RadiationPill>(RedeConditions.NukeDroppedOrDownedMechBossAll)
                .Add<GeigerMuller>(RedeConditions.NukeDroppedOrDownedMechBossAll)
                .Add<IOLocator>(Condition.DownedMechBossAll)
                .Add<MiniWarhead>(Condition.DownedMechBossAll)
                .Add<AnomalyDetector>(RedeConditions.DownedSeed)
                .Add<MedicOutfit>()
                .Add<MedicLegs>()
                .Add<MedicBackpack>()
                .Add<AdamHead>(RedeConditions.IsTBotHead)
                .Add<ZoneAccessPanel1>(new Condition("", () => RedeBossDowned.downedJanitor && !LabArea.labAccess[0]))
                .Add<ZoneAccessPanel2>(new Condition("", () => RedeBossDowned.downedBehemoth && !LabArea.labAccess[1]))
                .Add<ZoneAccessPanel3>(new Condition("", () => RedeBossDowned.downedBlisterface && !LabArea.labAccess[2]))
                .Add<ZoneAccessPanel4>(new Condition("", () => RedeBossDowned.downedVolt && !LabArea.labAccess[3]))
                .Add<ZoneAccessPanel5>(new Condition("", () => RedeBossDowned.downedMACE && !LabArea.labAccess[4]))
                .Add<ZoneAccessPanel6>(new Condition("", () => RedeBossDowned.downedPZ && !LabArea.labAccess[5]));

            npcShop.Register();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!Redemption.AprilFools || (NPC.frame.Y != 0 && NPC.frame.Y < 19 * 58))
                return true;

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Drip").Value;
            Vector2 offset = new(0, 4);
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - offset - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
    public class TBotProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/TBot");
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/TBot_Head");
    }
}
