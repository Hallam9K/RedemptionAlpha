using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.Globals.Players;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.Items.Donator.Lordfunnyman;
using Redemption.Items.Lore;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Usable.Summons;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.Textures.Emotes;
using Redemption.UI.Dialect;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly.TownNPCs
{
    [AutoloadHead]
    public class TBot : ModRedeNPC
    {
        public static Asset<Texture2D> unconsciousTexture;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            unconsciousTexture = Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/TBotUnconscious");
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Friendly T-Bot");
            Main.npcFrameCount[NPC.type] = 21;
            NPCID.Sets.HatOffsetY[NPC.type] = -6;
            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.FaceEmote[Type] = EmoteBubbleType<AdamTownNPCEmote>();

            NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike);
            NPC.Happiness.SetBiomeAffection<SnowBiome>(AffectionLevel.Hate);

            NPC.Happiness.SetNPCAffection(NPCID.Mechanic, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Nurse, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Truffle, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.Cyborg, AffectionLevel.Hate);

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
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
            SpawnModBiomes = new int[2] { GetInstance<LidenBiomeAlpha>().Type, GetInstance<LidenBiome>().Type };
            if (Unconscious > 0)
                NPC.dontTakeDamage = true;

            DialogueBoxStyle = LIDEN;
        }
        public override bool HasReviveButton() => Unconscious > 0;

        private int questCounter;
        int starsFrame;
        int starsCounter;
        public override void FindFrame(int frameHeight)
        {
            if (Unconscious > 0)
            {
                if (++starsCounter >= 10)
                {
                    starsCounter = 0;
                    if (++starsFrame > 3)
                    {
                        starsCounter = 0;
                        starsFrame = 0;
                    }
                }
                return;
            }
            if (++questCounter >= 4)
            {
                questCounter = 0;
                if (questFrame++ > 1)
                    questFrame = 0;
            }
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

        public int Unconscious;
        public override void LoadData(TagCompound tag)
        {
            warheadKnown = tag.GetBool("warheadKnown");
            Unconscious = tag.GetInt("Unconscious");
        }
        public override void SaveData(TagCompound tag)
        {
            tag["warheadKnown"] = warheadKnown;
            tag["Unconscious"] = Unconscious;
        }
        public override bool UsesPartyHat() => Unconscious <= 0;
        public override bool PreAI()
        {
            if (Unconscious > 0)
            {
                NPC.velocity.X = 0;
                if (--Unconscious == 1)
                {
                    Revived();
                    return false;
                }
                if (!NPC.dontTakeDamage)
                    NPC.dontTakeDamage = true;
                return false;
            }
            return true;
        }
        public void Revived()
        {
            Unconscious = 0;
            NPC.life = NPC.lifeMax;
            NPC.dontTakeDamage = false;
            NPC.netUpdate = true;
            Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.NPCWokeUp", NPC.FullName), new Color(50, 125, 255));
        }
        public override bool CheckDead()
        {
            if (Unconscious > 0)
            {
                NPC.life = NPC.lifeMax;
                NPC.netUpdate = true;
                return true;
            }
            Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Unconscious", NPC.FullName), Color.Red.R, Color.Red.G, Color.Red.B);
            RedeGlobalButton.talkActive = false;
            NPC.life = NPC.lifeMax;
            NPC.dontTakeDamage = true;
            Unconscious = 43200;
            NPC.netUpdate = true;
            return false;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return RedeBossDowned.downedSeed && !NPC.AnyNPCs(NPCType<TBot_Intro>()) && !RedeHelper.AnyProjectiles(ProjectileType<AdamPortal>());
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { Language.GetTextValue("Mods.Redemption.NPCs.TBot_Intro.DisplayName") };
        }
        public override ITownNPCProfile TownNPCProfile() => new TBotProfile();
        public override string GetChat()
        {
            if (Unconscious > 0)
            {
                if (Unconscious >= 43200 - 3600)
                    return Language.GetTextValue("Mods.Redemption.Dialogue.General.UnconsciousStatus1", NPC.FullName);
                return Language.GetTextValue("Mods.Redemption.Dialogue.General.UnconsciousStatus2", NPC.FullName, (int)MathHelper.Lerp(43200 / 3600, 0, Unconscious / 43200f));
            }
            Player player = Main.LocalPlayer;
            WeightedRandom<string> chat = new(Main.rand);

            /*
            int GuideID = NPC.FindFirstNPC(NPCID.Guide);
            if (GuideID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.GuideDialogue", Main.npc[GuideID].GivenName));

            int MerchantID = NPC.FindFirstNPC(NPCID.Merchant);
            if (MerchantID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.MerchantDialogue", Main.npc[MerchantID].GivenName));

            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            if (DryadID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.DryadDialogue", Main.npc[DryadID].GivenName));
            */
            int NurseID = NPC.FindFirstNPC(NPCID.Nurse);
            /*if (NurseID >= 0 && RedeBossDowned.nukeDropped)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.NurseDialogue", Main.npc[NurseID].GivenName));

            int ArmsDealerID = NPC.FindFirstNPC(NPCID.ArmsDealer);
            if (ArmsDealerID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.ArmsDealerDialogue", Main.npc[ArmsDealerID].GivenName));

            int cyborgID = NPC.FindFirstNPC(NPCID.Cyborg);
            if (cyborgID >= 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.CyborgDialogue", Main.npc[cyborgID].GivenName));

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
            */
            if (Main.dayTime)
            {
                string day = Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Morning");
                if (Main.time >= 54000 / 2)
                    day = Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Afternoon");
                if (Main.time >= 37800)
                    day = Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Evening");
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.1", day));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Day1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Day2"));
            }
            else
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Night"));

            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.2"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.3"));

            string dayTime = Main.dayTime ? Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Today") : Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Tonight");
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.4", dayTime));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.5"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.6", dayTime));

            bool debuffed = false;
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (player.HasBuff(i) && Main.debuff[i])
                {
                    debuffed = true;
                    break;
                }
            }
            if (player.statLife >= player.statLifeMax2 - 20 && !debuffed)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Healthy1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Healthy2"));
            }
            if (player.statLife <= player.statLifeMax2 / 2)
            {
                if (NurseID >= 0)
                {
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Unhealthy1"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Unhealthy2", Main.npc[NurseID].GivenName));
                }
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Unhealthy3"));
            }



            /*if (RedeBossDowned.downedJanitor)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue17"), 1.5);
            if (RedeBossDowned.nukeDropped || RedeBossDowned.downedJanitor)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue18"), 1.5);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue19"), 1.5);
            }
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Dialogue20"));*/

            if (player.RedemptionRad().radiationLevel >= .5f)
            {
                string rad = Language.GetTextValue("Mods.Redemption.Dialogue.TBot.DialogueRad");
                if (player.RedemptionRad().radiationLevel is >= 1f and < 1.5f)
                    rad = Language.GetTextValue("Mods.Redemption.Dialogue.TBot.DialogueRad2");
                if (player.RedemptionRad().radiationLevel >= 1.5f)
                    rad = Language.GetTextValue("Mods.Redemption.Dialogue.TBot.DialogueRad3");
                chat.Add(rad, 100);
            }
            return chat;
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
                .Add<CrystalSerum>(RedeConditions.NukeDroppedOrDownedMechBossAll)
                .Add<BleachedSolution>(RedeConditions.NukeDropped)
                .Add<GasMask>(Condition.Hardmode)
                .Add<HazmatSuit4>(Condition.Hardmode, RedeConditions.IsFinlandDay)
                .Add<HazmatSuit>(Condition.Hardmode, RedeConditions.IsNotFinlandDay)
                .Add<AIChip>(Condition.Hardmode)
                .Add<CarbonMyofibre>(Condition.Hardmode)
                .Add<Capacitor>(Condition.Hardmode)
                .Add<Plating>(Condition.Hardmode)
                .Add<RadiationPill>(Condition.Hardmode)
                .Add<GeigerMuller>(Condition.Hardmode)
                .Add<IOLocator>(Condition.DownedMechBossAll)
                .Add<MiniWarhead>(Condition.DownedMechBossAll)
                .Add<AnomalyDetector>(RedeConditions.DownedSeed)
                .Add<TerraBombaTail>(Condition.DownedMechBossAll)
                .Add<TerraBombaCore>(Condition.DownedMechBossAll)
                .Add<TerraBombaNose>(Condition.DownedMechBossAll)
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
        public static bool warheadKnown;
        private int questFrame;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Unconscious > 0)
            {
                Rectangle rect = unconsciousTexture.Frame(1, 4, 0, starsFrame);
                Vector2 origin = rect.Size() / 2;

                spriteBatch.Draw(unconsciousTexture.Value, NPC.Center + new Vector2(0, 3) - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
                return false;
            }

            if (!NPC.IsABestiaryIconDummy && Main.hardMode && !DialoguePlayer.GetTalkStateLocal(DialoguePlayer.TalkType.AdamShop) && !RedeBossDowned.nukeDropped)
            {
                Texture2D questMark = Request<Texture2D>("Redemption/Textures/QuestMark").Value;
                int Height = questMark.Height / 3;
                int y = Height * questFrame;
                Rectangle rect = new(0, y, questMark.Width, Height);
                Vector2 origin = new(questMark.Width / 2f, Height / 2f);
                float scaleOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi * 2f) / 6f;
                spriteBatch.Draw(questMark, NPC.Center - screenPos - new Vector2(0, 50), new Rectangle?(rect), NPC.GetAlpha(RedeColor.QuestMarkerColour), 0, origin, 1 + scaleOffset, 0, 0);
            }

            if (!Redemption.AprilFools || (NPC.frame.Y != 0 && NPC.frame.Y < 19 * 58))
                return true;

            Texture2D texture = Request<Texture2D>(Texture + "_Drip").Value;
            Vector2 offset = new(0, 4);
            spriteBatch.Draw(texture, NPC.Center - offset - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
        {
            emoteList.Add(EmoteID.DebuffSilence);
            emoteList.Add(EmoteID.ItemLifePotion);
            emoteList.Add(EmoteID.BiomeOtherworld);
            emoteList.Add(EmoteID.BiomeSnow);
            emoteList.Add(EmoteID.WeatherStorming);
            emoteList.Add(EmoteID.WeatherSnowstorm);
            emoteList.Add(EmoteID.CritterButterfly);
            return base.PickEmote(closestPlayer, emoteList, otherAnchor);
        }
    }
    public class TBotProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/TBot");
        public int GetHeadTextureIndex(NPC npc) => GetModHeadSlot("Redemption/NPCs/Friendly/TownNPCs/TBot_Head");
    }
    public class FloppyDiskButton : ChatButton
    {
        public override double Priority => 8.0;
        public override string Text(NPC npc, Player player)
        {
            int itemID = ItemType<FloppyDisk1>();

            if (player.HeldItem.type == ItemType<AIChip>())
                itemID = ItemType<AIChip>();

            if (player.HeldItem.type == ItemType<MemoryChip>())
                itemID = ItemType<MemoryChip>();

            return Mod.GetLocalization("DialogueBox.Read").WithFormatArgs(Lang.GetItemNameValue(itemID)).Value;
        }
        public override string Description(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.TBot.FloppyDisc.Description");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<TBot>();
        private readonly int[] disk = new int[] {
                    ItemType<FloppyDisk1>(),
                    ItemType<FloppyDisk2>(),
                    ItemType<FloppyDisk2_1>(),
                    ItemType<FloppyDisk3>(),
                    ItemType<FloppyDisk3_1>(),
                    ItemType<FloppyDisk5>(),
                    ItemType<FloppyDisk5_1>(),
                    ItemType<FloppyDisk5_2>(),
                    ItemType<FloppyDisk5_3>(),
                    ItemType<FloppyDisk6>(),
                    ItemType<FloppyDisk6_1>(),
                    ItemType<FloppyDisk7>(),
                    ItemType<FloppyDisk7_1>(),
                    ItemType<AIChip>(),
                    ItemType<MemoryChip>() };

        public override Color? OverrideColor(NPC npc, Player player)
        {
            for (int i = 0; i < disk.Length; i++)
            {
                if (player.HeldItem.type == disk[i])
                    return null;
            }
            return Color.Gray;
        }
        public static int FDisk;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            FDisk = 0;
            int heldItem = player.HeldItem.type;
            if (heldItem == disk[0])
                FDisk = 1;
            else if (heldItem == disk[1])
                FDisk = 2;
            else if (heldItem == disk[2])
                FDisk = 3;
            else if (heldItem == disk[3])
                FDisk = 4;
            else if (heldItem == disk[4])
                FDisk = 5;
            else if (heldItem == disk[5])
                FDisk = 6;
            else if (heldItem == disk[6])
                FDisk = 7;
            else if (heldItem == disk[7])
                FDisk = 8;
            else if (heldItem == disk[8])
                FDisk = 9;
            else if (heldItem == disk[9])
                FDisk = 10;
            else if (heldItem == disk[10])
                FDisk = 11;
            else if (heldItem == disk[11])
                FDisk = 12;
            else if (heldItem == disk[12])
                FDisk = 13;//
            else if (heldItem == disk[13])
                FDisk = 14;
            else if (heldItem == disk[14])
                FDisk = 16;

            Main.npcChatText = DiskChat();
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
                _ => Language.GetTextValue("Mods.Redemption.Dialogue.TBot.NoFloppyDisk")
            };
        }
    }
}