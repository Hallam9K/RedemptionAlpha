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
            NPC.height = 40;
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
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 2;
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

        public override string TownNPCName()
        {
            return "Newb";
        }

        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);
            /*if (BasePlayer.HasHelmet(player, ModContent.ItemType<KingSlayerMask>(), true))
            {
                chat.Add("Heheh! Hewwo mister slayer! Wait... who's that?");
            }
            if (BasePlayer.HasAccessory(player, ModContent.ItemType<CrownOfTheKing>(), true, true))
            {
                chat.Add("IT'S A CHICKEN! Come on mister chicken, time for your walk!");
            }
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<ArmorHKHead>(), true) && BasePlayer.HasChestplate(player, ModContent.ItemType<ArmorHK>(), true) && BasePlayer.HasChestplate(player, ModContent.ItemType<ArmorHKLeggings>(), true))
            {
                chat.Add("Do I know you?");
            }*/
            chat.Add("Who you? You Terrarian?");
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