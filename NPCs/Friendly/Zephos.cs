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
using Redemption.Globals.Player;
using Redemption.Items.Weapons.PreHM.Melee;
using Terraria.Audio;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Accessories.PreHM;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Armor.PreHM;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Buffs;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Zephos : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wayfarer");
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 80;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 8;

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

                new FlavorTextBestiaryInfoElement("A traveller from mainland Epidotra who is friends with Daerel. Most skilled at swordplay."),
            });
        }

        public override bool CheckDead()
        {
            RedeWorld.zephosDownedTimer = 0;
            Main.NewText("Zephos the Wayfarer was knocked unconscious...", Color.Red.R, Color.Red.G, Color.Red.B);
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

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

        }
        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return true;
        }

        public override string TownNPCName()
        {
            return "Zephos";
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new(Main.rand);

            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            if (DryadID >= 0)
                chat.Add("Doesn't " + Main.npc[DryadID].GivenName + " know how to dress properly? Whatever, I like it!");

            chat.Add("How's it goin' bro!");
            chat.Add("Hey I came from the mainland through that portal, but you don't mind me staying here, right?");
            chat.Add("Yo, I have some pretty cool things, you can have them if you got the money.");
            chat.Add("My favourite colour is orange! Donno why I'm tellin' ya though...");
            chat.Add("I don't know what the deal with cats are. Dogs are definitely better!");
            chat.Add("Have you seen a guy in a cloak, he carries a bow around. I lost him before travelling through the portal, hope he's alright.");
            return chat;
        }

        private static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = "Cycle Options";

            switch (ChatNumber)
            {
                case 0:
                    button = "Shop";
                    break;
                case 1:
                    button = "Talk";
                    break;
                case 2:
                    button = "Sharpen (5 silver)";
                    break;
                case 3:
                    button = "Shine Armor (15 silver)";
                    break;
                case 4:
                    button = "Quest";
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                switch (ChatNumber)
                {
                    case 0:
                        shop = true;
                        break;
                    case 1:
                        Main.npcChatText = ChitChat();
                        break;
                    case 2:
                        if (Main.LocalPlayer.BuyItem(500))
                        {
                            SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 37);
                            Main.LocalPlayer.AddBuff(BuffID.Sharpened, 36000);
                        }
                        else
                        {
                            Main.npcChatText = NoCoinsChat();
                            SoundEngine.PlaySound(SoundID.MenuTick, -1, -1, 1);
                        }
                        break;
                    case 3:
                        if (Main.LocalPlayer.BuyItem(1500))
                        {
                            SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 37);
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<ShineArmourBuff>(), 36000);
                        }
                        else
                        {
                            Main.npcChatText = NoCoinsChat();
                            SoundEngine.PlaySound(SoundID.MenuTick, -1, -1, 1);
                        }
                        break;
                    case 4:
                        Main.npcChatText = "(Quests will become available in v0.8.1 - Wayfarer Update)";
                        SoundEngine.PlaySound(SoundID.MenuTick, -1, -1, 1);
                        break;
                }
            }
            else
            {
                ChatNumber++;
                if (ChatNumber > 4)
                    ChatNumber = 0;
            }
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;

        public static string NoCoinsChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            chat.Add("You're as poor as me?");
            chat.Add("You really don't have enough money? Ah whatever, not like I can complain.");
            return chat;
        }

        public static string ChitChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            chat.Add("How about I tell you the time I was a pirate, sailing abroad the vast ocean with fellow pirate people... Actually, I don't remeber a lot about being a pirate. I was very young at the time.");
            chat.Add("I'm doin' good, although I've lost someone, his name is Daerel and wears a cloak. I'm sure I'll find him eventually.");
            chat.Add("Did I ever tell you about my victory against a powerful undead druid? It was a close match, it was giant, and its magic was insane! But yeah, I beat it, pretty cool huh? It had flowers growing everywhere on it!");
            chat.Add("How did I get here, I hear you asking? Me and Daerel were lookin' around a spiky forest until we found a portal and jumped in, don't know where Daerel went.");
            chat.Add("This island's gotta lotta chickens! Ever wonder where they came from? Back in Anglon, there are way deadlier chickens, called Anglonic Forest Hens. Funny story, I was with Daerel on one of his walks through the forest, then out of nowhere a giant hen charges through the bushes straight at him! I've never seen him run so fast!");
            chat.Add("I swear I saw a Blobble around here. I didn't expect them to be here, they're native to, uh, Ithon I think. Don't quote me on that though, Daerel's a lot better at remembering useless info than I.");
            if (!Main.dayTime)
            {
                chat.Add("You never told me there'd be undead here! What, they're called zombies? Well where I'm from they're called undead. There's also a few skeletons out here, normally they like to stay underground. This island is pretty weird. How do you live here?");
            }
            return chat;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ItemID.Leather);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FlintAndSteel>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BeardedHatchet>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CantripStaff>());
            //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IronfurAmulet>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Archcloth>());
            //if (NPC.downedBoss1)
            //    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ForestGolemPainting>());

            if (NPC.downedBoss2)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EaglecrestSpelltome>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SwordSlicer>());
            }

            if (RedeBossDowned.downedEaglecrestGolem)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GolemEye>());

            /*if (RedeBossDowned.downedMossyGoliath)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MossyWimpGun>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MudMace>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<TastySteak>());
                nextSlot++;
            }*/
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

        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            scale = 1f;
            item = TextureAssets.Item[ModContent.ItemType<PureIronSword>()].Value;
            itemSize = 36;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 36;
            itemHeight = 34;
        }
    }
}