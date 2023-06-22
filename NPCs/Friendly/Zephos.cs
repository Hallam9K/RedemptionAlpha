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

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Zephos : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wayfarer");
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 80;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 8;

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

        public int Level = -1;
        public int bestiaryTimer = -1;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (Level < 0)
                {
                    Level = 0;
                }

                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
                NPC.frame.X = NPC.frame.Width * Level;

                if (NPC.IsABestiaryIconDummy)
                {
                    bestiaryTimer++;
                    if (bestiaryTimer % 60 == 0)
                    {
                        Level++;
                        if (Level > 2)
                            Level = 0;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 1) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (NPC.altTexture == 1)
            {
                Asset<Texture2D> hat = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.PartyHat);
                var offset = (NPC.frame.Y / 52) switch
                {
                    3 => 2,
                    4 => 2,
                    5 => 2,
                    10 => 2,
                    11 => 2,
                    12 => 2,
                    18 => 2,
                    _ => 0,
                };
                var hatEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 origin = new(hat.Value.Width / 2f, hat.Value.Height / 2f);
                spriteBatch.Draw(hat.Value, NPC.Center - new Vector2(4 * NPC.spriteDirection, 24 + offset) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, hatEffects, 0);
            }
            return false;
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
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue15"));
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

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 28;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            scale = 1f;
            item = TextureAssets.Item[ModContent.ItemType<SwordSlicer>()].Value;
            itemSize = 36;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 36;
            itemHeight = 34;
        }
    }
    public class ZephosProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Zephos");
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/Zephos_Head");
    }
}
