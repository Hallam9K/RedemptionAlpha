using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.Dusts;
using Redemption.Base;
using Redemption.Items.Armor.Vanity;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.BaseExtension;
using Redemption.Items.Usable;
using ReLogic.Content;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Newb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fool");
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 80;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 18;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4;

            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike);
            NPC.Happiness.SetBiomeAffection<OceanBiome>(AffectionLevel.Hate);

            NPC.Happiness.SetNPCAffection<Zephos>(AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection<Daerel>(AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Wizard, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Clothier, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);

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
            NPC.width = 18;
            NPC.height = 46;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 9999;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

                new FlavorTextBestiaryInfoElement("..."),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
                NPC.frame.X = RedeBossDowned.downedNebuleus ? NPC.frame.Width : 0;
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
        public override void AI()
        {
            if (RedeWorld.newbGone)
                NPC.active = false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SightDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 4);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return RedeBossDowned.foundNewb && !RedeWorld.newbGone;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Newb" };
        }
        public override ITownNPCProfile TownNPCProfile() => new NewbProfile();
        public override string GetChat()
        {
            if (RedeBossDowned.downedNebuleus)
                Main.LocalPlayer.currentShoppingSettings.HappinessReport = "";
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);
            if (RedeBossDowned.downedNebuleus)
            {
                if (RedeBossDowned.nebDeath > 6)
                {
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.WokeDialogue1"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.WokeDialogue2"));
                }
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.WokeDialogue3"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.WokeDialogue4"));
            }
            else
            {
                if (BasePlayer.HasHelmet(player, ModContent.ItemType<KingSlayerMask>(), true))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.KingSlayerMaskDialogue"));
                if (player.RedemptionPlayerBuff().ChickenForm)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.ChickenDialogue"));
                else
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue2")); // 9.7%
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue3"), 0.6); // 5.8%
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue4"), 0.4); // 3.9%
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue5"), 0.4);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue6"), 0.2); // 1.9%
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue7"), 0.2);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue8"), 0.2);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue9"), 0.2);
                if (RedeWorld.alignment < 0)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.HuhDialogue"), 0.05); // 0.48%
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue10"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue11"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue12"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue13"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue14"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Fool.Dialogue15"));
            }
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            if (!RedeBossDowned.downedNebuleus)
                button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
                shopName = "Shop";
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add(ItemID.DirtBlock)
                .Add(ItemID.MudBlock)
                .Add(ItemID.Amethyst, RedeConditions.DownedEarlyGameBossAndMoR)
                .Add(ItemID.Topaz, RedeConditions.DownedEarlyGameBossAndMoR)
                .Add(ItemID.Sapphire, RedeConditions.DownedEoCOrBoCOrKeeper)
                .Add(ItemID.Emerald, RedeConditions.DownedEoCOrBoCOrKeeper)
                .Add(new Item(ItemID.Geode) { shopCustomPrice = Item.buyPrice(0, 2) }, RedeConditions.DownedEoCOrBoCOrKeeper)
                .Add(ItemID.Ruby, RedeConditions.DownedSkeletronOrSeed)
                .Add(ItemID.Diamond, RedeConditions.DownedSkeletronOrSeed)
                .Add<OreBomb>(Condition.InBelowSurface)
                .Add<OrePowder>(Condition.InBelowSurface, Condition.Hardmode);

            npcShop.Register();
        }
        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            foreach (Item item in items)
            {
                if (item == null || item.type == ItemID.None)
                    continue;

                if (item.type is ItemID.Geode && NPC.downedBoss3)
                    item.shopCustomPrice = Item.buyPrice(0, 1);
            }
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 18;
            knockback = 4f;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 34;
        }

        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = TextureAssets.Item[ItemID.WoodenSword].Value;
        }
    }
    public class NewbProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Newb");
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/Newb_Head");
    }
}