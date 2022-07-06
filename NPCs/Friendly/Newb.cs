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

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Newb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fool");
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
                NPC.frame.X = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SightDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 4);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

        }
        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return RedeBossDowned.foundNewb;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Newb" };
        }

        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<KingSlayerMask>(), true))
            {
                chat.Add("Heheh! Hewwo mister slayer! Wait... who's that?");
            }
            if (player.RedemptionPlayerBuff().ChickenForm)
            {
                chat.Add("IT'S A CHICKEN! Come on mister chicken, time for your walk!");
            }
            else
                chat.Add("Chickens very funny! I fed chicken grain but I threw a crown on floor instead, but chicken pecked it anyway! ... And then it exploded!!");/*
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<ArmorHKHead>(), true) && BasePlayer.HasChestplate(player, ModContent.ItemType<ArmorHK>(), true) && BasePlayer.HasChestplate(player, ModContent.ItemType<ArmorHKLeggings>(), true))
            {
                chat.Add("Do I know you?");
            }*/
            chat.Add("My shoes aren't muddy! Where is all the mud!?");
            chat.Add("Trees here are funny colours! Where are yellow leaves! They all green! ... Green is good colour too.", 0.6);
            chat.Add("What's your name? Is it Garry? I bet it's Garry! Garry the Gentle is your name now!", 0.4);
            chat.Add("This island is not MY island! Where are my people!?", 0.4);
            chat.Add("They're coming, the red is coming! Don't stay! ... Oh hewwo!", 0.2);
            chat.Add("Me like emeralds, they green! Rubies me hate! Too red!", 0.2);
            chat.Add("What is beyond portal? Let's find out Johnny! ... Wait that isn't right name...", 0.2);
            chat.Add("Me sowwy! Me go with yellow knight!", 0.2);
            if (RedeWorld.alignment < 0)
                chat.Add("Your ambitions are futile and will decayed, dare not proceed down the path of sin lest you face the very earth you walk upon. The death which lingers on your soul will consume you from within until you are but a husk unworthy of swift retribution.", 0.05);
            chat.Add("Who you? You human?");
            chat.Add("Me find shiny stones!");
            chat.Add("You look stupid! Haha!");
            chat.Add("My dirt is 10% off!");
            chat.Add("Heheheh!");
            chat.Add("Hewwo! I am Newb!");
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ItemID.DirtBlock);
            shop.item[nextSlot++].SetDefaults(ItemID.MudBlock);
            if (NPC.downedBoss1)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.Amethyst);
                shop.item[nextSlot++].SetDefaults(ItemID.Topaz);
            }
            if (NPC.downedBoss2)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.Sapphire);
                shop.item[nextSlot++].SetDefaults(ItemID.Emerald);
                shop.item[nextSlot].SetDefaults(ItemID.Geode);
                shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(0, NPC.downedBoss3 ? 1 : 2, 0, 0);
            }
            if (NPC.downedBoss3)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.Ruby);
                shop.item[nextSlot++].SetDefaults(ItemID.Diamond);
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

        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = TextureAssets.Item[ItemID.WoodenSword].Value;
        }
    }
}