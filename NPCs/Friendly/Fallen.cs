using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.Localization;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Tiles;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Melee;
using Terraria.Audio;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Tools.PreHM;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Armor.Single;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.Items.Usable;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Materials.HM;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Items.Armor.Vanity;
using ReLogic.Content;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Fallen : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 100;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 40;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 14;

            NPC.Happiness.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<SnowBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Hate);
            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Dislike);

            NPC.Happiness.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Clothier, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Nurse, AffectionLevel.Hate);
            NPC.Happiness.SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike);

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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Fallen")),
            });
        }

        public int FallenType = -1;
        public int bestiaryTimer = -1;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (FallenType < 0)
                {
                    switch (NPC.GivenName)
                    {
                        case "Okvot":
                            FallenType = 0;
                            break;
                        case "Tenvon":
                            FallenType = 1;
                            break;
                        case "Happins":
                            FallenType = 2;
                            break;
                    }
                }

                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
                if (NPC.IsABestiaryIconDummy)
                    NPC.frame.X = 0;
                else
                    NPC.frame.X = NPC.frame.Width * FallenType;

                if (NPC.IsABestiaryIconDummy)
                {
                    bestiaryTimer++;
                    if (bestiaryTimer % 60 == 0)
                    {
                        FallenType++;
                        if (FallenType > 1)
                            FallenType = -1;
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
                var offset = (NPC.frame.Y / 54) switch
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
                spriteBatch.Draw(hat.Value, NPC.Center - new Vector2(3 * NPC.spriteDirection, 24 + offset) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, hatEffects, 0);
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
            return RedeBossDowned.downedKeeper;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Happins", "Tenvon", "Okvot" };
        }
        public override ITownNPCProfile TownNPCProfile() => new FallenProfile();
        public override string GetChat()
        {
            WeightedRandom<string> chat = new(Main.rand);

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                    continue;

                if (!player.HasItem(ModContent.ItemType<BlackenedHeart>()))
                    continue;

                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.BlackenedHeartDialogue"));
            }

            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.Dialogue1"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.Dialogue2"));

            switch (FallenType)
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

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");

            button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Fallen.Repair");
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<GolemEye>()) && NPC.downedMoonlord && !RedeBossDowned.downedADD)
                button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Fallen.Eye");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.LocalPlayer;

            if (firstButton)
                shopName = "Shop";
            else
            {
                if (Main.LocalPlayer.HasItem(ModContent.ItemType<GolemEye>()) && NPC.downedMoonlord && !RedeBossDowned.downedADD)
                {
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.GolemEyeDialogue");
                    return;
                }
                int[] Frag = new int[] {
                    player.FindItem(ModContent.ItemType<ZweihanderFragment1>()),
                    player.FindItem(ModContent.ItemType<ZweihanderFragment2>()),
                    player.FindItem(ModContent.ItemType<ForgottenSword>()),
                    player.FindItem(ModContent.ItemType<OphosNotes>()) };

                if (Frag[0] >= 0 && Frag[1] >= 0)
                {
                    player.inventory[Frag[0]].stack--;
                    player.inventory[Frag[1]].stack--;
                    if (player.inventory[Frag[0]].stack <= 0)
                        player.inventory[Frag[0]] = new Item();
                    if (player.inventory[Frag[1]].stack <= 0)
                        player.inventory[Frag[1]] = new Item();

                    Main.npcChatCornerItem = ModContent.ItemType<Zweihander>();
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.ZweihanderRepairDialogue");
                    player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<Zweihander>());

                    SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                    return;
                }
                else if (Frag[2] >= 0 && Frag[3] >= 0)
                {
                    player.inventory[Frag[2]].stack--;
                    player.inventory[Frag[3]].stack--;
                    if (player.inventory[Frag[2]].stack <= 0)
                        player.inventory[Frag[2]] = new Item();
                    if (player.inventory[Frag[3]].stack <= 0)
                        player.inventory[Frag[3]] = new Item();

                    Main.npcChatCornerItem = ModContent.ItemType<ForgottenGreatsword>();
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.OphosRepairDialogue");
                    player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<ForgottenGreatsword>());

                    SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                    return;
                }
                else
                {
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.NoRepairDialogue");
                    for (int k = 0; k < Frag.Length; k++)
                    {
                        if (Frag[k] >= 0)
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Fallen.NotSameRepairDialogue");
                    }
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }

            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AntiquePureIronHelmet>()));
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add(new Item(ModContent.ItemType<BronzeWand>()) { shopCustomPrice = 30, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<Earthbind>()) { shopCustomPrice = 15, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, Condition.InBelowSurface)
                .Add(new Item(ModContent.ItemType<Mistfall>()) { shopCustomPrice = 15, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, Condition.InSnow)
                .Add(new Item(ModContent.ItemType<AncientDirt>()) { shopCustomPrice = 1, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<ElderWood>()) { shopCustomPrice = 1, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<GathicStone>()) { shopCustomPrice = 1, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<WeddingRing>()) { shopCustomPrice = 15, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<LostSoul>()) { shopCustomPrice = 4, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<Violin>()) { shopCustomPrice = 20, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<OldTophat>()) { shopCustomPrice = 20, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, Condition.PlayerCarriesItem(ModContent.ItemType<CruxCardTied>()))
                .Add(new Item(ModContent.ItemType<ScrunklePainting>()) { shopCustomPrice = 12, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId })
                .Add(new Item(ModContent.ItemType<SkullDiggerPainting>()) { shopCustomPrice = 12, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, RedeConditions.DownedSkullDigger)
                .Add(new Item(ModContent.ItemType<SunkenCaptainPainting>()) { shopCustomPrice = 12, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, Condition.DownedPirates)
                .Add(new Item(ItemID.Ectoplasm) { shopCustomPrice = 10, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, Condition.DownedPlantera)
                .Add(new Item(ModContent.ItemType<EmptyCruxCard>()) { shopCustomPrice = 30, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, RedeConditions.HasSpiritWalker)
                .Add(new Item(ModContent.ItemType<DeadRinger>()) { shopCustomPrice = 30, shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId }, RedeConditions.DeadRingerGiven);

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
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 46;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = TextureAssets.Item[ModContent.ItemType<BeardedHatchet>()].Value;
            itemSize = 38;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 38;
        }
    }
    public class FallenProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Fallen");
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/Fallen_Head");
    }
}
