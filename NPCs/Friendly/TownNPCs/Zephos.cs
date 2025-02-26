using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs;
using Redemption.Globals;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Quest;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Textures.Emotes;
using Redemption.UI.Dialect;
using ReLogic.Content;
using ReLogic.Graphics;
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
    public class Zephos : ModRedeNPC
    {
        public static int HeadIndex2;
        public static int HeadIndex3;
        public static Asset<Texture2D> unconsciousTexture;
        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            HeadIndex2 = Mod.AddNPCHeadTexture(Type, Texture + "2_Head");
            HeadIndex3 = Mod.AddNPCHeadTexture(Type, Texture + "3_Head");
            if (Main.dedServ)
                return;
            unconsciousTexture = Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/ZephosUnconscious");
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
            NPCID.Sets.FaceEmote[Type] = EmoteBubbleType<ZephosTownNPCEmote>();

            NPC.Happiness.
                SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Love)
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection<Daerel>(AffectionLevel.Love)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Pirate, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);

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
            NPC.width = 24;
            NPC.height = 46;
            NPC.aiStyle = 7;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            if (Unconscious > 0)
                NPC.dontTakeDamage = true;

            DialogueBoxStyle = EPIDOTRA;

            AnimationType = NPCID.Guide;
        }
        public override bool HasReviveButton() => Unconscious > 0;
        public override bool HasLeftHangingButton(Player player) => RedeQuest.wayfarerVars[0] >= 4 && !Daerel.IsUnconscious(NPC);
        public override bool HasRightHangingButton(Player player) => RedeQuest.wayfarerVars[0] >= 4 && (RedeGlobalButton.talkID != 0 || !RedeGlobalButton.talkActive) && !Daerel.IsUnconscious(NPC);
        public override HangingButtonParams LeftHangingButton(Player player)
        {
            if (RedeGlobalButton.talkActive)
                return new HangingButtonParams(4, true, -2);
            return new HangingButtonParams(1);
        }
        public override HangingButtonParams RightHangingButton(Player player)
        {
            if (RedeGlobalButton.talkActive)
            {
                int boxNum = RedeGlobalButton.talkID switch
                {
                    3 or 4 => 4,
                    2 => 3,
                    1 => 6,
                    _ => 7,
                };
                return new HangingButtonParams(boxNum, false, -2);
            }
            return new HangingButtonParams(1);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Zephos")),
            });
        }

        public int Unconscious;
        public override void LoadData(TagCompound tag)
        {
            Unconscious = tag.GetInt("Unconscious");
        }
        public override void SaveData(TagCompound tag)
        {
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
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Unconscious > 0)
            {
                Rectangle rect = unconsciousTexture.Frame(3, 4, Level, starsFrame);
                Vector2 origin = rect.Size() / 2;

                spriteBatch.Draw(unconsciousTexture.Value, NPC.Center + new Vector2(0, 7) - screenPos, rect, NPC.ColorTintedAndOpacity(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
                return false;
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public static int Level = 0;
        public override void AI()
        {
            if (NPC.ai[0] is 15 && NPC.ai[1] == NPCID.Sets.AttackTime[NPC.type])
            {
                NPC.Shoot(NPC.Center, ProjectileType<Zephos_SwordSlicer_Slash>(), NPC.damage * NPC.GetAttackDamage_ScaledByStrength(11), new Vector2(5 * NPC.direction, 0), 0, NPC.whoAmI, knockback: 5);
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
            return !WorldGen.crimson && (RedeQuest.wayfarerVars[0] >= 2 || Main.hardMode);
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { Language.GetTextValue("Mods.Redemption.NPCs.Zephos_Intro.DisplayName") };
        }
        public override ITownNPCProfile TownNPCProfile() => new ZephosProfile();
        public override string GetChat()
        {
            if (Unconscious > 0)
            {
                if (Unconscious >= 43200 - 3600)
                    return Language.GetTextValue("Mods.Redemption.Dialogue.General.UnconsciousStatus1", NPC.FullName);
                return Language.GetTextValue("Mods.Redemption.Dialogue.General.UnconsciousStatus2", NPC.FullName, (int)MathHelper.Lerp(43200 / 3600, 0, Unconscious / 43200f));
            }

            if (!RedeQuest.bonusQuestComplete)
            {
                bool incomplete = false;
                for (int i = 0; i < (int)RedeQuest.Bonuses.Count; i++)
                {
                    if (!RedeQuest.bonusDiscovered[i] && i != (int)RedeQuest.Bonuses.Clash)
                        incomplete = true;
                }
                if (!incomplete)
                {
                    Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), ItemType<CrystallizedKnowledge>());
                    Main.npcChatCornerItem = ItemType<CrystallizedKnowledge>();

                    RedeQuest.bonusQuestComplete = true;
                    RedeQuest.SyncData();
                    return Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.BookQuestComplete");
                }
            }

            WeightedRandom<string> chat = new(Main.rand);
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.IntroDialogue1"));
            }
            else
            {
                string bro = Main.LocalPlayer.Male ? Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Bro") : Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Broette");
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Dialogue1", bro));
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
        public override bool CanGoToStatue(bool toKingStatue) => true;
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add(ItemID.Leather)
                .Add<FlintAndSteel>()
                .Add<BeardedHatchet>()
                .Add<CantripStaff>()
                .Add<LeatherSheath>()
                .Add<Archcloth>()
                .Add<BookOfBonuses>(RedeConditions.ElementBookObtained)
                .Add<SilverRapier>(Condition.DownedEarlygameBoss)
                .Add<EaglecrestSpelltome>(Condition.DownedEowOrBoc)
                .Add<SwordSlicer>(Condition.DownedEowOrBoc)
                .Add<GolemEye>(RedeConditions.DownedEaglecrestGolem)
                .Add<ChaliceFragments>()
                .Add<GildedSeaEmblem>(Condition.InBeach)
                .Add<CrystallizedKnowledge>(RedeConditions.ElementBookQuest)
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

            if (otherAnchor?.entity is NPC entityNPC && entityNPC.type == NPCType<Fallen>())
            {
                emoteList.Add(EmoteID.EmoteFear);
            }

            // If the Town NPC is speaking to the player
            if (NPC.ai[0] == 7f || NPC.ai[0] == 19f)
            {
                emoteList.Clear();
                emoteList.Add(EmoteBubbleType<DaerelTownNPCEmote>());
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
            Main.GetItemDrawFrame(ItemType<SwordSlicer>(), out item, out itemFrame);
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
                return Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Zephos" + (Zephos.Level + 1));
            return Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Zephos");
        }
        public int GetHeadTextureIndex(NPC npc)
        {
            return Zephos.Level switch
            {
                1 => Zephos.HeadIndex2,
                2 => Zephos.HeadIndex3,
                _ => GetModHeadSlot("Redemption/NPCs/Friendly/TownNPCs/Zephos_Head"),
            };
        }
    }
    public class SharpenButton : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Zephos.Sharpen.Name");
        public override string Description(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Zephos.Sharpen.Description");
        public override bool IsActive(NPC npc, Player player) => RedeQuest.wayfarerVars[0] >= 4 && npc.type == NPCType<Zephos>() && !RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player)
        {
            float mouseTextColorFactor = Main.mouseTextColor / 255f;
            return new Color((byte)(181f * mouseTextColorFactor), (byte)(192f * mouseTextColorFactor), (byte)(193f * mouseTextColorFactor), Main.mouseTextColor);
        }
        public override void OnClick(NPC npc, Player player)
        {
            if (Main.LocalPlayer.BuyItem(500))
            {
                SoundEngine.PlaySound(SoundID.Item37, npc.position);
                Main.LocalPlayer.AddBuff(BuffID.Sharpened, 36000);
            }
            else
            {
                WeightedRandom<string> chat = new(Main.rand);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.NoMoneyDialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.NoMoneyDialogue2"));
                Main.npcChatText = chat;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
    }
    public class ShineArmorButton : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player)
        {
            if (player.statDefense <= 0 || (player.armor[0] == null && player.armor[1] == null && player.armor[2] == null))
                return "???";
            return Language.GetTextValue("Mods.Redemption.DialogueBox.Zephos.ShineArmor.Name");
        }
        public override string Description(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Zephos.ShineArmor.Description");
        public override bool IsActive(NPC npc, Player player) => RedeQuest.wayfarerVars[0] >= 4 && npc.type == NPCType<Zephos>() && !RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player)
        {
            if (player.statDefense <= 0 || (player.armor[0] == null && player.armor[1] == null && player.armor[2] == null))
                return Color.Gray;

            float mouseTextColorFactor = Main.mouseTextColor / 255f;
            return new Color((byte)(181f * mouseTextColorFactor), (byte)(192f * mouseTextColorFactor), (byte)(193f * mouseTextColorFactor), Main.mouseTextColor);

        }
        public override void OnClick(NPC npc, Player player)
        {
            if (player.statDefense <= 0 || (player.armor[0] == null && player.armor[1] == null && player.armor[2] == null))
                return;
            if (Main.LocalPlayer.BuyItem(1500))
            {
                SoundEngine.PlaySound(SoundID.Item37, npc.position);
                Main.LocalPlayer.AddBuff(BuffType<ShineArmourBuff>(), 36000);
            }
            else
            {
                WeightedRandom<string> chat = new(Main.rand);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.NoMoneyDialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.NoMoneyDialogue2"));
                Main.npcChatText = chat;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
    }
    public class Zephos_SwordSlicer_Slash : BaseSwordSlicer_Slash
    {
        protected override Entity Owner => Main.npc[(int)Projectile.ai[1]];
    }
}