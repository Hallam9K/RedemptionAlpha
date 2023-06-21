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
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Usable;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Materials.HM;
using ReLogic.Content;
using Redemption.BaseExtension;
using Terraria.Localization;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Daerel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wayfarer");
            Main.npcFrameCount[Type] = 26;
            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 1000;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 20;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 8;

            NPC.Happiness
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Hate)
                .SetNPCAffection<Zephos>(AffectionLevel.Love)
                .SetNPCAffection(NPCID.Stylist, AffectionLevel.Like)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike);

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
            NPC.height = 48;
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Daerel")),
            });
        }

        public override bool CheckDead()
        {
            RedeWorld.daerelDownedTimer = 0;
            Main.NewText(Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Unconscious"), Color.Red.R, Color.Red.G, Color.Red.B);
            NPC.SetDefaults(ModContent.NPCType<DaerelUnconscious>());
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

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (NPC.altTexture == 1)
            {
                Asset<Texture2D> hat = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.PartyHat);
                var offset = (NPC.frame.Y / 56) switch
                {
                    3 => 2,
                    4 => 2,
                    5 => 2,
                    10 => 2,
                    11 => 2,
                    12 => 2,
                    _ => 0,
                };
                var hatEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 origin = new(hat.Value.Width / 2f, hat.Value.Height / 2f);
                spriteBatch.Draw(hat.Value, NPC.Center - new Vector2(1 * NPC.spriteDirection, 25 + offset) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, hatEffects, 0);
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
            return WorldGen.crimson && RedeQuest.wayfarerVars[0] >= 2 && !RedeHelper.DaerelActive();
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Daerel" };
        }
        public override ITownNPCProfile TownNPCProfile() => new DaerelProfile();
        public override string GetChat()
        {
            adviceNum = 0;
            WeightedRandom<string> chat = new(Main.rand);
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.IntroDialogue1"));
            }
            else
            {
                int PartyGirlID = NPC.FindFirstNPC(NPCID.PartyGirl);
                if (PartyGirlID >= 0)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.PartyGirlDialogue", Main.npc[PartyGirlID].GivenName), 0.2);

                if (Main.LocalPlayer.ZoneGraveyard)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue2"));
                if (!NPC.homeless)
                {
                    if (Main.raining && Main.LocalPlayer.ZoneOverworldHeight)
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue3"));

                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue4"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue5"));
                }
                else
                {
                    if (Main.raining && Main.LocalPlayer.ZoneOverworldHeight)
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue6"));
                }
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue7"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue8"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue9"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue10"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue11"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue12"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue13"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue14"));
                if (!Main.dayTime)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue15"));
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
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Bow");
                        break;
                    case 3:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Poison");
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
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.IntroDialogue2");
                            RedeQuest.wayfarerVars[0] = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.IntroDialogue3");
                            RedeQuest.wayfarerVars[0] = 3;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 3:
                        if (firstButton)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.IntroDialogue4");
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
                            if (Main.LocalPlayer.BuyItem(1000))
                            {
                                SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                Main.LocalPlayer.AddBuff(BuffID.Archery, 21600);
                            }
                            else
                            {
                                Main.npcChatText = NoCoinsChat();
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                            break;
                        case 3:
                            if (Main.LocalPlayer.BuyItem(2500))
                            {
                                SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                Main.LocalPlayer.AddBuff(BuffID.WeaponImbuePoison, 36000);
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
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.NoMoneyDialogue1"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.NoMoneyDialogue2"));
            return chat;
        }

        private static int adviceNum;
        public static string ChitChat()
        {
            List<string> chat = new();
            if (RedeBossDowned.downedPZ && !RedeBossDowned.downedNebuleus)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue1"));
            int FallenID = NPC.FindFirstNPC(ModContent.NPCType<Fallen>());
            if (FallenID >= 0 && Main.LocalPlayer.HasItem(ModContent.ItemType<GolemEye>()) && NPC.downedMoonlord && !RedeBossDowned.downedADD)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue2", Main.npc[FallenID].GivenName));
            if (Main.hardMode && !RedeBossDowned.downedSlayer)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue3"));
            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            if (DryadID >= 0 && RedeQuest.forestNymphVar == 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue4", Main.npc[DryadID].GivenName));
            if (FallenID >= 0 && !Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue5", Main.npc[FallenID].GivenName));
            if (!RedeBossDowned.downedEaglecrestGolem && NPC.downedBoss2)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue6"));
            if (!RedeWorld.alignmentGiven)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue16"));
            if (!Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue7", ElementID.ArcaneS, ElementID.HolyS));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue8"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue9", ElementID.ShadowS, ElementID.HolyS));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue10", ElementID.IceS));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue11"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue12"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue13"));
            if (!RedeBossDowned.foundNewb)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue14"));
            if (RedeBossDowned.erhanDeath == 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.AdviceDialogue15"));

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
                .Add<DurableBowString>()
                .Add<Archcloth>()
                .Add<EaglecrestSpelltome>(Condition.DownedEowOrBoc)
                .Add<SilverwoodBow>(Condition.DownedEowOrBoc)
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
                .Add<DarkSteelBow>(Condition.DownedMoonLord);

            npcShop.Register(); // Name of this shop tab
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 13;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 20;
            randExtraCooldown = 10;
        }

        public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset)
        {
            scale = 1f;
            item = TextureAssets.Item[ModContent.ItemType<SilverwoodBow>()].Value;
            horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, ModContent.ItemType<SilverwoodBow>()).X - 20;
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.WoodenArrowFriendly;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 10f;
        }
    }
    public class DaerelProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Daerel");
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/Daerel_Head");
    }
}
