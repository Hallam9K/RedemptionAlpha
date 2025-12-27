using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Items;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Single;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.PostML.Magic;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Textures.Emotes;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly.TownNPCs
{
    [AutoloadHead]
    public class Fallen : ModRedeNPC
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
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 80;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 40;
            NPCID.Sets.AttackAverageChance[Type] = 20;
            NPCID.Sets.HatOffsetY[Type] = 4;
            NPCID.Sets.FaceEmote[Type] = EmoteBubbleType<OkvotTownNPCEmote>();

            NPC.Happiness.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<SnowBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Hate);
            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Dislike);

            NPC.Happiness.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Clothier, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Nurse, AffectionLevel.Hate);
            NPC.Happiness.SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike);

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
            NPC.damage = 11;
            NPC.defense = 5;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            DialogueBoxStyle = CAVERN;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Fallen")),
            });
        }

        public override void AI()
        {
            if (NPC.ai[0] is 15 && NPC.ai[1] == NPCID.Sets.AttackTime[NPC.type])
            {
                NPC.Shoot(NPC.Center, ProjectileType<Fallen_BeardedHatchet_Proj>(), (int)(NPC.damage * NPCHelper.TownNPCDamageMultiplier()), new Vector2(5 * NPC.direction, 0), NPC.whoAmI, knockback: 5);
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
            return RedeBossDowned.downedKeeper;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Happins", "Tenvon", "Okvot" };
        }
        public override ITownNPCProfile TownNPCProfile() => new FallenProfile();
        private static bool TalkedOnce;
        public override string GetChat()
        {
            if (!TalkedOnce)
            {
                RedeQuest.adviceUnlocked[(int)RedeQuest.Advice.UGPortal] = true;
                if (Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                    RedeQuest.adviceSeen[(int)RedeQuest.Advice.UGPortal] = true;

                RedeQuest.SyncData();
                TalkedOnce = true;
            }
            WeightedRandom<string> chat = new(Main.rand);
            var fallenType = NPC.GivenName switch
            {
                "Tenvon" => 1,
                "Happins" => 2,
                _ => 0,
            };
            switch (fallenType)
            {
                case 0:
                    if (RedeBossDowned.keeperSaved)
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OkvotDialogue1"), 3);

                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OkvotDialogue2"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OkvotDialogue3"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OkvotDialogue4"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OkvotDialogue5"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OkvotDialogue6"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OkvotDialogue7"), 2);
                    break;
                case 1:
                    if (RedeBossDowned.keeperSaved)
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue1"), 3);

                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue2"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue3"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue4"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue5"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue6"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue7"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.TenvonDialogue8"));
                    break;
                case 2:
                    if (RedeBossDowned.keeperSaved)
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.HappinsDialogue1"), 3);

                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.HappinsDialogue2"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.HappinsDialogue3"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.HappinsDialogue4"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.HappinsDialogue5"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.HappinsDialogue6"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.HappinsDialogue7"), 2);
                    break;
            }
            return chat;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<AntiquePureIronHelmet>()));
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add(new Item(ItemType<BronzeWand>()) { shopCustomPrice = 30, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<Earthbind>()) { shopCustomPrice = 15, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, Condition.InBelowSurface)
                .Add(new Item(ItemType<ElderWoodCrossbow>()) { shopCustomPrice = 20, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, Condition.InBelowSurface)
                .Add(new Item(ItemType<Mistfall>()) { shopCustomPrice = 15, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, Condition.InSnow)
                .Add(new Item(ItemID.BookofSkulls) { shopCustomPrice = 40, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, Condition.DownedSkeletron)
                .Add(new Item(ItemType<AncientGrassSeeds>()) { shopCustomPrice = 1, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<AncientDirt>()) { shopCustomPrice = 1, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<ElderWood>()) { shopCustomPrice = 1, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<GathicStone>()) { shopCustomPrice = 1, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<WeddingRing>()) { shopCustomPrice = 15, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<LostSoul>()) { shopCustomPrice = 4, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<Violin>()) { shopCustomPrice = 20, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<ViisaanKantele>()) { shopCustomPrice = 24, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, RedeConditions.DownedADD)
                .Add(new Item(ItemType<OldTophat>()) { shopCustomPrice = 20, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, RedeConditions.OldTophat)
                .Add(new Item(ItemType<ScrunklePainting>()) { shopCustomPrice = 12, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<SleepingStonesPainting>()) { shopCustomPrice = 20, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency })
                .Add(new Item(ItemType<SkullDiggerPainting>()) { shopCustomPrice = 12, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, RedeConditions.DownedSkullDigger)
                .Add(new Item(ItemType<SunkenCaptainPainting>()) { shopCustomPrice = 12, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, Condition.DownedPirates).Add(new Item(ItemID.Ectoplasm) { shopCustomPrice = 10, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, Condition.DownedPlantera)
                .Add(new Item(ItemType<EmptyCruxCard>()) { shopCustomPrice = 30, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, RedeConditions.HasSpiritWalker)
                .Add(new Item(ItemType<SpiritExtractor>()) { shopCustomPrice = 40, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, RedeConditions.HasSpiritWalker)
                .Add(new Item(ItemType<DeadRinger>()) { shopCustomPrice = 30, shopSpecialCurrency = RedeCurrency.AntiqueDorulCurrency }, RedeConditions.DeadRingerGiven);

            npcShop.Register();
        }
        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            foreach (Item item in items)
            {
                if (item == null || item.type == ItemID.None)
                    continue;

                if (item.type is ItemID.Ectoplasm && NPC.downedGolemBoss)
                    item.shopCustomPrice = 6;
            }
        }
        public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
        {
            emoteList.Add(EmoteID.ItemTombstone);
            emoteList.Add(EmoteID.ItemPickaxe);
            emoteList.Add(EmoteID.BiomeRocklayer);
            emoteList.Add(EmoteID.CritterSkeleton);
            return base.PickEmote(closestPlayer, emoteList, otherAnchor);
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 0;
            knockback = 0;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 20;
        }
        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            int itemType = ItemType<BeardedHatchet>();
            Main.GetItemDrawFrame(itemType, out item, out itemFrame);
            itemFrame = Rectangle.Empty;
            itemSize = 0;
        }
        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 0;
        }
    }
    public class FallenProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            var fallenType = npc.GivenName switch
            {
                "Tenvon" => 1,
                "Happins" => 2,
                _ => 0,
            };
            if (fallenType > 0)
                return Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Fallen" + (fallenType + 1));
            return Request<Texture2D>("Redemption/NPCs/Friendly/TownNPCs/Fallen");
        }
        public int GetHeadTextureIndex(NPC npc)
        {
            var fallenType = npc.GivenName switch
            {
                "Tenvon" => Fallen.HeadIndex2,
                "Happins" => Fallen.HeadIndex3,
                _ => GetModHeadSlot("Redemption/NPCs/Friendly/TownNPCs/Fallen_Head"),
            };
            return fallenType;
        }
    }
    public class EyeOriginButton : ChatButton
    {
        public override double Priority => 10.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Fallen.Eye");
        public override string Description(NPC npc, Player player) => string.Empty;
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Fallen>() && player.HasItemInAnyInventory(ItemType<GolemEye>()) && NPC.downedMoonlord && !RedeBossDowned.downedADD;
        public override Color? OverrideColor(NPC npc, Player player)
        {
            return RedeColor.AncientColour;
        }
        public override void OnClick(NPC npc, Player player)
        {
            RedeQuest.adviceSeen[(int)RedeQuest.Advice.UkkoEye] = true;
            RedeQuest.SyncData();
            npc.GetGlobalNPC<ExclaimMarkNPC>().exclaimationMark[4] = false;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.GolemEyeDialogue");
        }
    }
    public class RepairButton : ChatButton
    {
        public override double Priority => 8.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Fallen.Repair.Name");

        public override string Description(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Fallen.Repair.Description");

        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Fallen>();

        const short ZWEIHANDER = 0, FORGOTTEN = 1;
        readonly List<int[]> fragments = new()
        {
            new int[2] { ItemType<ZweihanderFragment1>(), ItemType<ZweihanderFragment2>() },
            new int[2] { ItemType<ForgottenSword>(), ItemType<OphosNotes>() },
        };
        bool HasFragments(Player player, int id, bool dontConsume = false)
        {
            if (player.HasItem(fragments[id][0]) && player.HasItem(fragments[id][1]))
            {
                if (!dontConsume)
                {
                    player.ConsumeItem(fragments[id][0]);
                    player.ConsumeItem(fragments[id][1]);
                }
                return true;
            }
            return false;
        }

        public override Color? OverrideColor(NPC npc, Player player)
        {
            int[][] fragArray = [.. fragments];
            for (int i = 0; i < fragArray.Length; i++)
            {
                if (player.HasItem(fragArray[i][0]) && player.HasItem(fragArray[i][1]))
                {
                    return null;
                }
            }
            return Color.Gray;
        }
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            int[][] Frags = [.. fragments];

            if (HasFragments(player, ZWEIHANDER))
            {
                Main.npcChatCornerItem = ItemType<Zweihander>();
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.ZweihanderRepairDialogue");
                player.QuickSpawnItem(npc.GetSource_Loot(), ItemType<Zweihander>());

                SoundEngine.PlaySound(SoundID.Item37, npc.position);
                return;
            }
            else if (HasFragments(player, FORGOTTEN))
            {
                Main.npcChatCornerItem = ItemType<ForgottenGreatsword>();
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OphosRepairDialogue");
                player.QuickSpawnItem(npc.GetSource_Loot(), ItemType<ForgottenGreatsword>());

                SoundEngine.PlaySound(SoundID.Item37, npc.position);
                return;
            }
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.NoRepairDialogue");
            for (int k = 0; k < Frags.Length; k++)
            {
                if (player.HasItem(Frags[k][0]) || player.HasItem(Frags[k][1]))
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.NotSameRepairDialogue");
            }
        }
    }
    public class Fallen_BeardedHatchet_Proj : BaseBeardedHatchet_Proj
    {
        protected override Entity Owner => Main.npc[(int)Projectile.ai[0]];
        protected override int NewLength => 36;
    }
}