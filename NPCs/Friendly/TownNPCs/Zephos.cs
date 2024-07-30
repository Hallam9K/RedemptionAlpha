using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Weapons.PreHM.Melee;
using Terraria.Audio;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Usable.Summons;
using Redemption.Buffs;
using Redemption.Items.Usable;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.HM;
using ReLogic.Content;
using Redemption.BaseExtension;
using Terraria.Localization;
using Redemption.Textures.Emotes;
using Redemption.NPCs.Friendly.TownNPCs;
using Terraria.GameContent.UI;
using Redemption.Globals.NPC;
using System;

namespace Redemption.NPCs.Friendly.TownNPCs
{
    [AutoloadHead]
    public class Zephos : ModNPC
    {
        public static int HeadIndex2;
        public static int HeadIndex3;
        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            HeadIndex2 = Mod.AddNPCHeadTexture(Type, Texture + "2_Head");
            HeadIndex3 = Mod.AddNPCHeadTexture(Type, Texture + "3_Head");
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wayfarer");
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 80;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 26;
            NPCID.Sets.AttackAverageChance[Type] = 20;
            NPCID.Sets.HatOffsetY[Type] = 6;
            NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<ZephosTownNPCEmote>();

            NPC.Happiness.
                SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Love)
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Hate)
                .SetNPCAffection<Daerel>(AffectionLevel.Love)
                .SetNPCAffection(NPCID.Pirate, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Clothier, AffectionLevel.Hate);

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
            NPC.width = 24;
            NPC.height = 46;
            NPC.aiStyle = 7;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Zephos")),
            });
        }

        public override bool CheckDead()
        {
            RedeWorld.zephosDownedTimer = 0;
            Main.NewText(Language.GetTextValue("Mods.Redemption.DialogueBox.Zephos.Unconscious"), Color.Red.R, Color.Red.G, Color.Red.B);
            NPC.SetDefaults(ModContent.NPCType<ZephosUnconscious>());
            NPC.life = 1;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            return false;
        }

        public static int Level = 0;
        public override void AI()
        {
            if (NPC.ai[0] is 15 && NPC.ai[1] == NPCID.Sets.AttackTime[NPC.type])
            {
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<Zephos_SwordSlicer_Slash>(), NPC.damage * NPC.GetAttackDamage_ScaledByStrength(11), new Vector2(5 * NPC.direction, 0), 0, NPC.whoAmI, knockback: 5);
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return !WorldGen.crimson && RedeQuest.wayfarerVars[0] >= 2 && !RedeHelper.ZephosActive();
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Zephos" };
        }
        public override ITownNPCProfile TownNPCProfile() => new ZephosProfile();
        public override string GetChat()
        {
            adviceNum = 0;
            WeightedRandom<string> chat = new(Main.rand);
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.IntroDialogue1"));
            }
            else
            {
                if (!Main.LocalPlayer.Male)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.FemaleDialogue"));
                else
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue2"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue3"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue4"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue5"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue6"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue7"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue8"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue9"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue10"));
                if (!Main.dayTime)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue11"));
            }
            return chat;
        }

        private static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                switch (RedeQuest.wayfarerVars[0])
                {
                    default:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Stay1");
                        button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Stay2");
                        break;
                    case 3:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Stay1");
                        button2 = "";
                        break;
                }
            }
            else
            {
                button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Cycle");

                switch (ChatNumber)
                {
                    case 0:
                        button = Language.GetTextValue("LegacyInterface.28");
                        break;
                    case 1:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Advice");
                        break;
                    case 2:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Zephos.Sharpen");
                        break;
                    case 3:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Zephos.ShineArmor");
                        break;
                }
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                switch (RedeQuest.wayfarerVars[0])
                {
                    default:
                        if (firstButton)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.IntroDialogue2");
                            RedeQuest.wayfarerVars[0] = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.IntroDialogue3");
                            RedeQuest.wayfarerVars[0] = 3;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 3:
                        if (firstButton)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.IntroDialogue4");
                            RedeQuest.wayfarerVars[0] = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                }
            }
            else
            {
                if (firstButton)
                {
                    switch (ChatNumber)
                    {
                        case 0:
                            shopName = "Shop";
                            break;
                        case 1:
                            for (int k = 0; k < 6; k++)
                                NPC.GetGlobalNPC<ExclaimMarkNPC>().exclaimationMark[k] = false;

                            Main.npcChatText = ChitChat();
                            adviceNum++;
                            break;
                        case 2:
                            if (Main.LocalPlayer.BuyItem(500))
                            {
                                SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                Main.LocalPlayer.AddBuff(BuffID.Sharpened, 36000);
                            }
                            else
                            {
                                Main.npcChatText = NoCoinsChat();
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                            break;
                        case 3:
                            if (Main.LocalPlayer.BuyItem(1500))
                            {
                                SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                Main.LocalPlayer.AddBuff(ModContent.BuffType<ShineArmourBuff>(), 36000);
                            }
                            else
                            {
                                Main.npcChatText = NoCoinsChat();
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                            break;
                    }
                }
                else
                {
                    ChatNumber++;
                    if (ChatNumber > 3)
                        ChatNumber = 0;
                }
            }
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;

        public static string NoCoinsChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.NoMoneyDialogue1"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.NoMoneyDialogue2"));
            return chat;
        }
        private static int adviceNum;
        public static string ChitChat()
        {
            List<string> chat = new();
            if (RedeBossDowned.downedPZ && !RedeBossDowned.downedNebuleus)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue1"));
            int FallenID = NPC.FindFirstNPC(ModContent.NPCType<Fallen>());
            if (FallenID >= 0 && Main.LocalPlayer.HasItem(ModContent.ItemType<GolemEye>()) && NPC.downedMoonlord && !RedeBossDowned.downedADD)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue2", Main.npc[FallenID].GivenName));
            if (Main.hardMode && !RedeBossDowned.downedSlayer)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue3"));
            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            if (DryadID >= 0 && RedeQuest.forestNymphVar == 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue4", Main.npc[DryadID].GivenName));
            if (FallenID >= 0 && !Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue5", Main.npc[FallenID].GivenName));
            if (!RedeBossDowned.downedEaglecrestGolem && NPC.downedBoss2)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue6"));
            if (!RedeWorld.alignmentGiven)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue15"));
            if (!Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue7", ElementID.ArcaneS, ElementID.HolyS));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue8", ElementID.HolyS, ElementID.ShadowS));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue9", Main.LocalPlayer.Male ? "" : " (wink wink)", ElementID.IceS));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue10"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue11"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue12"));
            if (!RedeBossDowned.foundNewb)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue13"));
            if (RedeBossDowned.erhanDeath == 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.AdviceDialogue14"));

            string[] chatStr = chat.ToArray();
            int maxAdvice = chatStr.Length;
            if (adviceNum >= maxAdvice)
                adviceNum = 0;

            string num = "(" + (adviceNum + 1) + "/" + maxAdvice + ") ";
            return num + chatStr[adviceNum];
        }
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add(ItemID.Leather)
                .Add<FlintAndSteel>()
                .Add<BeardedHatchet>()
                .Add<CantripStaff>()
                .Add<LeatherSheath>()
                .Add<Archcloth>()
                .Add<SilverRapier>(Condition.DownedEarlygameBoss)
                .Add<EaglecrestSpelltome>(Condition.DownedEowOrBoc)
                .Add<SwordSlicer>(Condition.DownedEowOrBoc)
                .Add<GolemEye>(RedeConditions.DownedEaglecrestGolem)
                .Add<ChaliceFragments>()
                .Add<GildedSeaEmblem>(Condition.InBeach)
                .Add<OphosNotes>(Condition.DownedGolem)
                .Add<KingChickenPainting>()
                .Add<FowlEmperorPainting>(RedeConditions.DownedFowlEmperor)
                .Add<PonderingTreesPainting>(Condition.DownedEarlygameBoss)
                .Add<MudGuardianPainting>(Condition.DownedSkeletron)
                .Add<SkeletonGuardianPainting>(Condition.DownedSkeletron)
                .Add<AkkaPainting>(Condition.Hardmode)
                .Add<AncientAutoPainting>(Condition.Hardmode)
                .Add<DubiousWatcherPainting>(Condition.Hardmode)
                .Add<KSPainting>(RedeConditions.DownedSlayer)
                .Add<UkkoPainting>(Condition.DownedPlantera)
                .Add<EmeraldHeartPainting>(Condition.DownedPlantera)
                .Add<WardenPainting>(Condition.DownedMoonLord)
                .Add<MythrilsBane>(Condition.DownedMoonLord);

            npcShop.Register(); // Name of this shop tab
        }
        public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
        {
            emoteList.Add(EmoteID.EmoteHappiness);
            emoteList.Add(EmoteID.Hungry);
            emoteList.Add(EmoteID.ItemSoup);
            emoteList.Add(EmoteID.ItemCookedFish);
            emoteList.Add(EmoteID.ItemSword);
            emoteList.Add(EmoteID.ItemGoldpile);
            emoteList.Add(EmoteID.BiomeBeach);

            if (otherAnchor?.entity is NPC entityNPC && entityNPC.type == ModContent.NPCType<Fallen>())
            {
                emoteList.Add(EmoteID.EmoteFear);
            }

            // If the Town NPC is speaking to the player
            if (NPC.ai[0] == 7f || NPC.ai[0] == 19f)
            {
                emoteList.Clear();
                emoteList.Add(ModContent.EmoteBubbleType<DaerelTownNPCEmote>());
                if (!closestPlayer.Male)
                {
                    emoteList.Add(EmoteID.EmotionLove);
                    emoteList.Add(EmoteID.DebuffSilence);
                }
            }
            return base.PickEmote(closestPlayer, emoteList, otherAnchor);
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 28;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 30;
        }
        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            Main.GetItemDrawFrame(ModContent.ItemType<SwordSlicer>(), out item, out itemFrame);
            itemFrame = Rectangle.Empty;
            itemSize = 0;
        }
        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 0;
        }
    }
    public class ZephosProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (Zephos.Level > 0)
                return ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Zephos" + (Zephos.Level + 1));
            return ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Zephos");
        }
        public int GetHeadTextureIndex(NPC npc)
        {
            return Zephos.Level switch
            {
                1 => Zephos.HeadIndex2,
                2 => Zephos.HeadIndex3,
                _ => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/TownNPCs/Zephos_Head"),
            };
        }
    }
    public class Zephos_SwordSlicer_Slash : BaseSwordSlicer_Slash
    {
        protected override Entity Owner => Main.npc[(int)Projectile.ai[1]];
    }
}