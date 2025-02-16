using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Projectiles.Ranged;
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
    public class Daerel : ModRedeNPC
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
            unconsciousTexture = Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/DaerelUnconscious");
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wayfarer");
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 400;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 6;
            NPCID.Sets.FaceEmote[Type] = EmoteBubbleType<DaerelTownNPCEmote>();

            NPC.Happiness
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Dislike)
                .SetNPCAffection<Zephos>(AffectionLevel.Love)
                .SetNPCAffection(NPCID.Stylist, AffectionLevel.Like)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike);

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
            NPC.height = 48;
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
        public override bool HasLeftHangingButton(Player player) => RedeQuest.wayfarerVars[0] >= 4 && !IsUnconscious(NPC);
        public override bool HasRightHangingButton(Player player) => RedeQuest.wayfarerVars[0] >= 4 && (RedeGlobalButton.talkID != 0 || !RedeGlobalButton.talkActive) && !IsUnconscious(NPC);
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Daerel")),
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
        public static bool IsUnconscious(NPC npc)
        {
            if (npc == null)
                return false;
            if (npc.ModNPC is Daerel daerel)
                return daerel.Unconscious > 0;
            if (npc.ModNPC is Zephos zephos)
                return zephos.Unconscious > 0;
            return false;
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

                spriteBatch.Draw(unconsciousTexture.Value, NPC.Center + new Vector2(0, 8) - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
                return false;
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public static int Level = 0;
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
            return WorldGen.crimson && (RedeQuest.wayfarerVars[0] >= 2 || Main.hardMode);
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { Language.GetTextValue("Mods.Redemption.NPCs.Daerel_Intro.DisplayName") };
        }
        public override ITownNPCProfile TownNPCProfile() => new DaerelProfile();
        public override string GetChat()
        {
            if (Unconscious > 0)
            {
                if (Unconscious >= 43200 - 3600)
                    return Language.GetTextValue("Mods.Redemption.Dialogue.General.UnconsciousStatus1", NPC.FullName);
                return Language.GetTextValue("Mods.Redemption.Dialogue.General.UnconsciousStatus2", NPC.FullName, (int)MathHelper.Lerp(43200 / 3600, 0, Unconscious / 43200f));
            }

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
                }
                else
                {
                    if (Main.raining && Main.LocalPlayer.ZoneOverworldHeight)
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue6"));
                }
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Dialogue5"));
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
        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add(ItemID.Leather)
                .Add<FlintAndSteel>()
                .Add<BeardedHatchet>()
                .Add<CantripStaff>()
                .Add<DurableBowString>()
                .Add<Archcloth>()
                .Add<SilverRapier>(Condition.DownedEarlygameBoss)
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
        public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
        {
            emoteList.Add(EmoteID.DebuffSilence);
            emoteList.Add(EmoteID.BiomeOtherworld);

            if (otherAnchor?.entity is NPC entityNPC && entityNPC.type == NPCType<Fallen>())
            {
                emoteList.Add(EmoteID.EmoteFear);
            }

            // If the Town NPC is speaking to the player
            if (NPC.ai[0] == 7f || NPC.ai[0] == 19f)
            {
                emoteList.Clear();
                emoteList.Add(EmoteBubbleType<ZephosTownNPCEmote>());
            }
            return base.PickEmote(closestPlayer, emoteList, otherAnchor);
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 13;
            knockback = 4f;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 8;
        }
        public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset)
        {
            int itemType = ItemType<SilverwoodBow>();
            Main.GetItemDrawFrame(itemType, out item, out itemFrame);
            horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 12;
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileType<SilverwoodArrow>();
            attackDelay = 1;
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 15f;
            gravityCorrection = 0.2f;
        }
    }
    public class DaerelProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (Daerel.Level > 0)
                return Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Daerel" + (Daerel.Level + 1));
            return Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Daerel");
        }
        public int GetHeadTextureIndex(NPC npc)
        {
            return Daerel.Level switch
            {
                1 => Daerel.HeadIndex2,
                2 => Daerel.HeadIndex3,
                _ => GetModHeadSlot("Redemption/NPCs/Friendly/TownNPCs/Daerel_Head"),
            };
        }
    }
    public class AdviceButton : ChatButton
    {
        public override double Priority => 4.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Advice");
        public override string Description(NPC npc, Player player) => string.Empty;
        public override bool IsActive(NPC npc, Player player) => RedeQuest.wayfarerVars[0] >= 4 && (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>()) && !RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            Main.npcChatCornerItem = 0;
            RedeGlobalButton.talkActive = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
    public class RestringBowButton : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Bow.Name");
        public override string Description(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Bow.Description");
        public override bool IsActive(NPC npc, Player player) => RedeQuest.wayfarerVars[0] >= 4 && npc.type == NPCType<Daerel>() && !RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player)
        {
            float mouseTextColorFactor = (float)(int)Main.mouseTextColor / 255f;
            return new Color((byte)(181f * mouseTextColorFactor), (byte)(192f * mouseTextColorFactor), (byte)(193f * mouseTextColorFactor), Main.mouseTextColor);
        }
        public override void OnClick(NPC npc, Player player)
        {
            if (Main.LocalPlayer.BuyItem(1000))
            {
                SoundEngine.PlaySound(SoundID.Item37, npc.position);
                Main.LocalPlayer.AddBuff(BuffID.Archery, 21600);
            }
            else
            {
                WeightedRandom<string> chat = new(Main.rand);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.NoMoneyDialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.NoMoneyDialogue2"));
                Main.npcChatText = chat;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
    }
    public class PoisonWeaponButton : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Poison.Name");
        public override string Description(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Poison.Description");
        public override bool IsActive(NPC npc, Player player) => RedeQuest.wayfarerVars[0] >= 4 && npc.type == NPCType<Daerel>() && !RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player)
        {
            float mouseTextColorFactor = Main.mouseTextColor / 255f;
            return new Color((byte)(181f * mouseTextColorFactor), (byte)(192f * mouseTextColorFactor), (byte)(193f * mouseTextColorFactor), Main.mouseTextColor);
        }
        public override void OnClick(NPC npc, Player player)
        {
            if (Main.LocalPlayer.BuyItem(2500))
            {
                SoundEngine.PlaySound(SoundID.Item37, npc.position);
                Main.LocalPlayer.AddBuff(BuffID.WeaponImbuePoison, 36000);
            }
            else
            {
                WeightedRandom<string> chat = new(Main.rand);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.NoMoneyDialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.NoMoneyDialogue2"));
                Main.npcChatText = chat;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
    }
    public class WayfarerIntroButton1 : ChatButton
    {
        public override double Priority => 1.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Stay1");
        public override string Description(NPC npc, Player player) => string.Empty;
        public override bool IsActive(NPC npc, Player player) => RedeQuest.wayfarerVars[0] < 4 && (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>());
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);

            string wayfarer = "Daerel";
            if (npc.type == NPCType<Zephos>())
                wayfarer = "Zephos";

            string bro = Main.LocalPlayer.Male ? Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Bro") : Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Broette");

            switch (RedeQuest.wayfarerVars[0])
            {
                default:

                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + wayfarer + ".IntroDialogue2", bro);
                    RedeQuest.wayfarerVars[0] = 4;
                    RedeQuest.SyncData();
                    break;
                case 3:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + wayfarer + ".IntroDialogue4", bro);
                    RedeQuest.wayfarerVars[0] = 4;
                    RedeQuest.SyncData();
                    break;
            }
        }
    }
    public class WayfarerIntroButton2 : ChatButton
    {
        public override double Priority => 2.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Daerel.Stay2");
        public override string Description(NPC npc, Player player) => string.Empty;
        public override bool IsActive(NPC npc, Player player) => RedeQuest.wayfarerVars[0] < 4 && RedeQuest.wayfarerVars[0] != 3 && (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>());
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);

            string wayfarer = "Daerel";
            if (npc.type == NPCType<Zephos>())
                wayfarer = "Zephos";
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + wayfarer + ".IntroDialogue3");
            RedeQuest.wayfarerVars[0] = 3;
            RedeQuest.SyncData();
        }
    }
}