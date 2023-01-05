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
            return !WorldGen.crimson && RedeQuest.wayfarerVars[0] >= 2 && !RedeHelper.ZephosActive();
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Zephos" };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                chat.Add("Hey there, sorry for the intrusion but I've lost my friend beyond that portal! Mind if I stay here to get some supplies? I'm sure I'll find him eventually.");
            }
            else
            {
                if (!Main.LocalPlayer.Male)
                    chat.Add("So... You like... pirates?");
                else
                    chat.Add("How's it goin' bro!");
                chat.Add("Hey I came from the mainland through that portal, but you don't mind me staying here, right?");
                chat.Add("Yo, I have some pretty cool things, you can have them if you got the money.");
                chat.Add("My favourite colour is orange! Donno why I'm tellin' ya though...");
                chat.Add("I don't know what the deal with cats are. Dogs are definitely better!");
                chat.Add("Have you seen a guy in a cloak, he carries a bow around. I lost him before travelling through the portal, hope he's alright.");
                chat.Add("Wanna know about the time I was a pirate, sailing abroad the vast ocean with fellow pirate people... Actually, I don't remember a lot about being a pirate. I was very young at the time.");
                chat.Add("Did I ever tell you about my victory against a powerful undead druid? It was a close match, it was giant, and its magic was insane! But yeah, I beat it, pretty cool huh? It had flowers growing everywhere on it!");
                chat.Add("This island's gotta lotta chickens! Ever wonder where they came from? Back in Anglon, there are way deadlier chickens, called Anglonic Forest Hens. Funny story, I was with Daerel on one of his walks through the forest, then out of nowhere a giant hen charges through the bushes straight at him! I've never seen him run so fast!");
                chat.Add("I swear I saw a Blobble around here. I didn't expect them to be here, they're native to, uh, Ithon I think. Don't quote me on that though, Daerel's a lot better at remembering useless info than I.");
                if (!Main.dayTime)
                    chat.Add("You never told me there'd be undead here! What, they're called zombies? Well where I'm from they're called undead. There's also a few skeletons out here, normally they like to stay underground. This island is pretty weird. How do you live here?");
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
                        button = "Feel free to stay here";
                        button2 = "Who are you?";
                        break;
                    case 3:
                        button = "Feel free to stay here";
                        button2 = "";
                        break;
                }
            }
            else
            {
                button2 = "Cycle Options";

                switch (ChatNumber)
                {
                    case 0:
                        button = "Shop";
                        break;
                    case 1:
                        button = "Advice";
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
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                switch (RedeQuest.wayfarerVars[0])
                {
                    default:
                        if (firstButton)
                        {
                            Main.npcChatText = "Thanks bro! I may have been a pirate when I was a youngster, but rest assure I will not steal any of your possessions. Just a few bits and bobs needed to help me find my friend, ya know? I'm Zephos, by the way. Pleasure to meet ya.";
                            RedeQuest.wayfarerVars[0] = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = "Where are my manners! M'name is Zephos, I hold no grand title to my name yet, but once I figure out the blade I'm certain your humble abode shall have a fine swordsman one day! As of now, I must attend to the matter of my friend and gather a few helpful resources. I hope my presence doesn't annoy ya.";
                            RedeQuest.wayfarerVars[0] = 3;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 3:
                        if (firstButton)
                        {
                            Main.npcChatText = "Thanks bro! I may have been a pirate when I was a youngster, but rest assure I will not steal any of your possessions. Just a few bits and bobs needed to help me find my friend, ya know? Pleasure to meet ya.";
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
                            shop = true;
                            break;
                        case 1:
                            Main.npcChatText = ChitChat();
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
                        case 4:
                            Main.npcChatText = "(Quests will become available in v0.8.1 - Wayfarer Update)";
                            SoundEngine.PlaySound(SoundID.MenuTick);
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
            chat.Add("When encountering skeletons and undead, holy weapons are most effective against them. On the contrary, shadow weapons aren't as effective. I hate skeletons, used to think they looked kinda funny, until me and Daerel met a skeleton Vex.");
            chat.Add("If you hate slimes, burn them! They'll burn brighter than my passion for attractive ladies" + (Main.LocalPlayer.Male ? "" : "(wink wink)") + ". Or, you could use ice weapons to freeze them, but that isn't as fun.");
            chat.Add("Ever want to sneak up on an Epidotrian skeleton? Or perhaps a chicken? Well invisibility potions are real handy for the job!");
            chat.Add("Skeletons can wield some super rusty weapons, not something you'd wanna get cut by. If you do get a dirty wound, take a dip in some water and it'll disappear!");
            chat.Add("See foes wearing armor or holding shields? You'll need to smash their Guard to deal with them! A hammer or explosives will be your best bet, just make sure your weapon isn't super weak.");
            if (!RedeBossDowned.foundNewb)
                chat.Add("I felt a weird presence beneath that portal I hopped out of, it was super uncanny! Maybe you should check it out.");
            if (RedeBossDowned.erhanDeath == 0)
                chat.Add("I saw a scroll sitting atop a small table next to the portal I came out of, did you pick it up yet? It looked rather... demonic.");
            return chat;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ItemID.Leather);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FlintAndSteel>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BeardedHatchet>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CantripStaff>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LeatherSheath>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Archcloth>());
            if (NPC.downedBoss1)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SilverRapier>());

            if (NPC.downedBoss2)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EaglecrestSpelltome>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SwordSlicer>());
            }

            if (RedeBossDowned.downedEaglecrestGolem)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GolemEye>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ChaliceFragments>());

            if (NPC.downedGolemBoss)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<OphosNotes>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<KingChickenPainting>());
            if (NPC.downedBoss1)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PonderingTreesPainting>());
            if (NPC.downedBoss3)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<MudGuardianPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkeletonGuardianPainting>());
            }
            if (Main.hardMode)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AkkaPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AncientAutoPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DubiousWatcherPainting>());
            }
            if (RedeBossDowned.downedSlayer)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<KSPainting>());
            if (NPC.downedPlantBoss)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<UkkoPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EmeraldHeartPainting>());
            }
            if (NPC.downedMoonlord)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<WardenPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<MythrilsBane>());
            }

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
            item = TextureAssets.Item[ModContent.ItemType<SwordSlicer>()].Value;
            itemSize = 36;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 36;
            itemHeight = 34;
        }
    }
}