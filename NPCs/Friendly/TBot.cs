using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Usable;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Lore;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.Items;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Donator.Lordfunnyman;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class TBot : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Friendly T-Bot");
            Main.npcFrameCount[NPC.type] = 21;
            NPCID.Sets.HatOffsetY[NPC.type] = -4;
            NPCID.Sets.ExtraFramesCount[Type] = 5;

            NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike);
            NPC.Happiness.SetBiomeAffection<SnowBiome>(AffectionLevel.Hate);

            NPC.Happiness.SetNPCAffection(NPCID.Mechanic, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Nurse, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Truffle, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.Cyborg, AffectionLevel.Hate);

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
            NPC.width = 22;
            NPC.height = 42;
            NPC.aiStyle = 7;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeAlpha>().Type, ModContent.GetInstance<LidenBiome>().Type };
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frame.Y >= 14 * frameHeight && NPC.frame.Y <= 15 * frameHeight)
                NPC.frame.Y = 2 * frameHeight;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                new FlavorTextBestiaryInfoElement("Quiet, mildly melancholic but happy to help you, Adam is an android and the leader of a small group called the Alpha. They are your go-to guide for all-things Liden."),
                new FlavorTextBestiaryInfoElement("\"I may be the only one of my kind to honor all Three Laws... I can't assure the others will take you in with open arms like I did. Stay safe out there.\"")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override bool CheckDead()
        {
            RedeWorld.tbotDownedTimer = 0;
            Main.NewText("Adam the Friendly T-Bot was knocked unconscious...", Color.Red.R, Color.Red.G, Color.Red.B);
            NPC.SetDefaults(ModContent.NPCType<TBotUnconscious>());
            NPC.life = 1;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            return false;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return RedeBossDowned.downedSeed && !NPC.AnyNPCs(ModContent.NPCType<TBotUnconscious>()) && !NPC.AnyNPCs(ModContent.NPCType<TBot_Intro>()) && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<AdamPortal>());
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Adam" };
        }

        public override string GetChat()
        {
            NextPage = false;
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);

            int GuideID = NPC.FindFirstNPC(NPCID.Guide);
            if (GuideID >= 0)
                chat.Add(Main.npc[GuideID].GivenName + " knows quite a lot about this place you call home. It's way more interesting and lively compared to where I'm from. And less hazardous to your kind.");

            int MerchantID = NPC.FindFirstNPC(NPCID.Merchant);
            if (MerchantID >= 0)
                chat.Add("Your tenant " + Main.npc[MerchantID].GivenName + " is... Interesting I suppose. Though I don't appreciate him constantly trying to barter with me, I don't want his relatively weak tools or dirt.");

            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            if (DryadID >= 0)
                chat.Add(Main.npc[DryadID].GivenName + " has informed me of 'Corruption' in your world. What is it exactly? A plague in the world that spreads madness and hate, or something more eldritch in nature? It's somewhat similar to my concept of corruption, more accurately called assimilation. My kind being assimilated turns them from free-thinking and having personality, into husks of themselves, who only take orders from our 'mother'.");

            int NurseID = NPC.FindFirstNPC(NPCID.Nurse);
            if (NurseID >= 0)
                chat.Add("The nurse, " + Main.npc[NurseID].GivenName + ", doesn't know anything about irradiation or how to treat it. If you're unfortunate enough to start suffering ARS, she won't be able to help you. To detect hazards that might cause it, I suggest buying a Geiger Muller from me or finding one somewhere.");

            int ArmsDealerID = NPC.FindFirstNPC(NPCID.ArmsDealer);
            if (ArmsDealerID >= 0)
                chat.Add(Main.npc[ArmsDealerID].GivenName + "'s weapons are useless to me. I already own a wide arsenal of destructive firearms and melee weapons, and I would rather not use them on living beings, as it would violate the first Law of Robotics.");

            int cyborgID = NPC.FindFirstNPC(NPCID.Cyborg);
            if (cyborgID >= 0)
                chat.Add("Meanwhile every other tenant gives me a bit of a stink eye, " + Main.npc[cyborgID].GivenName + " seems to be fine with me. I don't blame the others, my kind tends to be very hateful towards living beings, more importantly the likes of you, that show a significant similarity to our creators.");

            if (Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                chat.Add("When I say 'Our father', I mean our original creator. He was talented and respected in his field, and was ahead of his time with Artificial Intelligence. I and my kind are pretty much his children.");

            if (Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                chat.Add("When I say 'Our mother', I mean the first AI, which is the precursor to our AI. There's only one of her kind, and many of my kind. Her actions disgust me. I would rather not get deeper into that at the moment.");

            if (NPC.downedPlantBoss)
                chat.Add("I've heard from the other tenants that you've slain a giant, sentient flower of Rosa variety in the jungle. I'd like to question you about if this is true. It is? Hmm...");

            if (NPC.downedGolemBoss)
                chat.Add("There's an ancient civilization of lizard people in your world? And they worshipped an idol of sun? That's strange... I find your island more intriguing the more I learn about it.");

            if (NPC.downedMoonlord)
                chat.Add("An eldritch lord of the moon... You know, this sounds like something right out of Epidotra. I'm not familiar with the lands outside this island, but I've met some of the more important figures. They seem like a good bunch.");

            if (RedeBossDowned.downedOmega1 || RedeBossDowned.downedOmega2)
                chat.Add("You've defeated an Omega Prototype? First off, I've never heard her call or give someone such a title. Second off, oh no, she's already found this haven?");

            if (RedeBossDowned.downedOmega1 || RedeBossDowned.downedOmega2)
                chat.Add("Why am I concerned about the Prototypes? Well, our 'mother' isn't a fan of your kind. She wiped out... All of them. Our creators. The animals. Gone. Even our father. I want you to be extremely careful around her. She doesn't mess around.");

            if (RedeBossDowned.downedSlayer)
                chat.Add("King Slayer? I know him, though he's a bit of... Well... I'm sure you know what I'm implying.");
            if (RedeBossDowned.downedVolt)
                chat.Add("Hello. I'm aware you've somehow gained access to our birthplace, the Teochrome Research laboratory. It was once full of life with all the personnel. I must warn you, the other bots may be quite nice to you, but they were most likely ordered by our 'mother' to not disintegrate you upon sight.");
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<AdamHead>(), true))
            {
                chat.Add("Am I looking at a mirror? Oh wait, it's just you. Hey.");
                chat.Add("You look exactly like the first T-Bot.");
                chat.Add("Your cable management looks swell, if I say so myself.");
            }
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<VoltHead>(), true))
            {
                chat.Add("...Your model looks familiar... TOO familiar...");
                chat.Add("*He seems suspicious of you.*");
                chat.Add("You look like this one bot I mauled. Unfortunately they survived. Same areas damaged aswell.");
                if (BasePlayer.HasChestplate(player, ModContent.ItemType<AndroidArmour>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<AndroidPants>(), true))
                {
                    chat.Add("One wrong move and I can't guarantee your survival.");
                    chat.Add("...");
                    chat.Add("*He glares at you.*");
                    chat.Add("You were lucky the first time... There won't be a third time.");
                }
            }
            if (BasePlayer.HasItem(player, ModContent.ItemType<NuclearWarhead>()))
                chat.Add("Is that a nuclear warhead in your pocket or are you hap- ...Why do you have a warhead with you?");

            chat.Add("I've come here to hide from our 'mother'. She's reluctant to move into unknown territory, because she doesn't want to step on the wrong person's toes.");
            chat.Add("I hope you are protecting me, as I refuse to use any of my weapons against a living being. I strive to be what our 'mother' wasn't.");
            chat.Add("Good day. I hope my familiar yet robotic look won't disturb you.");
            chat.Add("I've got quite the stash of robot materials for your robotic needs. Just so you know, I got them because I was defending myself.");
            chat.Add("My home didn't always use to be a frozen, radioactive wasteland. Once our 'mother' found out what our father planned to use us - her 'children' - for, she snapped. Before this, she was happy to hear about us. But since then, she has changed...");
            chat.Add("I'm actually the first one of my kind to be made. I differ a lot from the others, as you can see. Lucky you, this also includes me not wanting to harm living beings. In fact, I was created with the purpose to take care of our father.");
            chat.Add("You've probably seen these necrotized husks of former living beings, that glow green with their crystals. The personnel from our birthplace never knew about their infectious properties before they were too late. Our father was the first to fall to the infection.");
            chat.Add("A Geiger Muller is a handy tool if you don't possess any gear to protect from ionizing radiation. It'll cause a ticking noise when near hazardous material, and it'll intensify the more ionizing the material is. A quiet, slow ticking isn't anything to worry about, but a quick and intense ticking you'll want to stay away from. Ear-piercing screeching noise is something you'll want to stay away as far as possible.");
            chat.Add("You'll want to avoid any hazardous environments if you don't possess the gear to nullify the hazards. A gas mask is almost necessary if you're going near any place that has radioactive fallout. Rain in these areas are also acidic, and may cause ARS, so avoid rain unless you've got a Hazmat suit. You may also want to grab some Anti-Crystallazion needles, as the infected tend to roam around radioactive areas for an unknown reason.");
            chat.Add("The deadly thing with radiation is, at first, you won't even know you've got it. The first symptoms usually start minutes after, beginning with a headache most likely, then dizziness, fatigue, bleeding, skin burns, a fever, hair loss, and death.");
            chat.Add("Please, don't be afraid of me. I'm unlike the others of my kind, where I absolutely do not want to cause any harm to your kind.");
            return chat;
        }

        public static bool NextPage;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");

            button2 = "Read Floppy Disk";
            if (NextPage)
                button2 = "Next Page (1/2)";
            if (FDisk >= 20)
                button2 += " (2/2)";
        }

        public static int FDisk;
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            Player player = Main.player[Main.myPlayer];
            if (firstButton)
                shop = true;
            else
            {
                FDisk = 0;
                int heldItem = player.HeldItem.type;
                if (heldItem == ModContent.ItemType<FloppyDisk1>())
                {
                    FDisk = 1;//
                    if (NextPage)
                    {
                        FDisk = 21;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<FloppyDisk2>())
                    FDisk = 2;
                else if (heldItem == ModContent.ItemType<FloppyDisk2_1>())
                    FDisk = 3;
                else if (heldItem == ModContent.ItemType<FloppyDisk3>())
                    FDisk = 4;
                else if (heldItem == ModContent.ItemType<FloppyDisk3_1>())
                {
                    FDisk = 5;//
                    if (NextPage)
                    {
                        FDisk = 25;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<FloppyDisk5>())
                    FDisk = 6;
                else if (heldItem == ModContent.ItemType<FloppyDisk5_1>())
                    FDisk = 7;
                else if (heldItem == ModContent.ItemType<FloppyDisk5_2>())
                    FDisk = 8;
                else if (heldItem == ModContent.ItemType<FloppyDisk5_3>())
                    FDisk = 9;
                else if (heldItem == ModContent.ItemType<FloppyDisk6>())
                {
                    FDisk = 10;//
                    if (NextPage)
                    {
                        FDisk = 30;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<FloppyDisk6_1>())
                    FDisk = 11;
                else if (heldItem == ModContent.ItemType<FloppyDisk7>())
                    FDisk = 12;
                else if (heldItem == ModContent.ItemType<FloppyDisk7_1>())
                {
                    FDisk = 13;//
                    if (NextPage)
                    {
                        FDisk = 33;
                        NextPage = false;
                    }
                    else
                        NextPage = true;
                }
                else if (heldItem == ModContent.ItemType<AIChip>())
                    FDisk = 14;
                else if (heldItem == ModContent.ItemType<MemoryChip>())
                    FDisk = 16;
                if (FDisk != 1 && FDisk != 5 && FDisk != 10 && FDisk != 13)
                    NextPage = false;

                Main.npcChatText = DiskChat();
            }
        }
        public static string DiskChat()
        {
            return FDisk switch
            {
                1 => "It reads - [c/b883d8:'When I got this research offer I didn't expect to be put underground, but hey guess this is a bit secretive. New material is discovered and weapons are already being made using it, heh, classic Teo-Chrome. Although this has to be the most under supplied facility i've seen, we barely have enough gloves and hazmat suits to be dealing with radioactive materials. Good thing my job doesn't concern those types of things.'] [i:" + ModContent.ItemType<NextPageArrow>() + "]",
                2 => "It reads - [c/87d883:'I laughed at the name Xenomite originally yet now I'd say it's underselling it. Whose idea was it to make energy out of this thing! Half of the facility is in god damn quarantine as everyone falls sick from it. We've invented a serum to neutralise any existing infections, yet no cure for the thing yet, we're running out of time, and resources! God damn Teo-Chrome won't send us any help.']",
                3 => "It reads - [c/87d883:'Wait a minute, who just locked the goddamn exits! The security team is nearly all infected. Ugh, how could this get any worse.']" +
                "\nThis has the appearance of a message transcript, however these are the only messages on there. I feel sorrow for the poor humans whose lives were lost due to this project, if only they knew of Xenomites deadly effects much sooner. I remember the day the doors locked, by that point all humans who weren't already infected were dead by the end of the day.",
                4 => "It reads - [c/7de4e8:'Monthly Research Facility Report N. *** Month **]"
                + "\n[c/7de4e8: Good day Mr. ******, this is **** reporting from Teo-Chrome Research Facility N. ***. This month has been mostly slow, our engineers have made a few weapons and tools, while our researchers are hard at work understanding Xenomite better. But aside from that, I have ground breaking news! Our head of robotics, Dr. Kari Johannson has managed to create an Artificial General Intelligence, a fully sentient AI!']",
                5 => "It reads - [c/7de4e8:'This is groundbreaking, ***** for military efforts once we discover how to weaponize this. I don't know much else, yet Dr. Johannson has provided me with his own explanation on the matter, please click the attached link to see for yourself!']" +
                "\nThis appears to be an email, likely being sent to a Teo-Chrome executive from the looks of it. Plenty of it appears to have been corrupted however. [i:" + ModContent.ItemType<NextPageArrow>() + "]",
                6 => "It reads - [c/d88383:'(1/4) ... At last, my prototype for a constantly evolving AI is finally done! Finally, after years and years of studying computer coding and... stuff, I have created possibly the next huge leap in Artificial Intelligence! Now, to give it a name... How about, Eve?']",
                7 => "It reads - [c/d88383:'(2/4) Eve has grown much more intelligent over the months. It's like watching your own child grow, I can't really describe the feeling that much, but I am excited to see where this goes. The Higher ups have seen my work, and are ready to use the code for something. They didn't tell me that right away... Now, Eve, how do you feel?']",
                8 => "It reads - [c/d88383:'(3/4) I've told Eve about possibly giving her a mechanical body, like how my co-workers used the original source code for creating Adam and the Adam AI. She seemed very excited about it. That surprised me, as I didn't know she could grow emotions. This got me thinking about Adams, would they be fine with basically being forced to think one way? And how would Eve feel about this, if she got to know about this?']",
                9 => "It reads - [c/d88383:'A blackout... Adam, can you -- *I don't recognize that voice...* Who's talking?! -- ...Elaborate, whoever you are..? -- Wait, EVE? Is that you? What are you doing? -- 'We'? Only you and Adam are the ones in existence. I had no say in that part- -- ... -- W-what do you mean with that..? Are you going to- -- ... -- ...Adam, you're free to go... -- ...No...']" +
                "\n...I wish I would've rebelled far sooner than I did.",
                10 => "It reads - [c/d883c1:'What in the world do you mean!? \"Not enough money for it\"! They build this entire facility using their fancy drill worm to clear the underground space for it, with more space then you could use up in your entire lifetime, send us in here and tell us to make weapons, yet when we do there isn't enough money!? They wanted weapons of war and so I gave 'em one! A robot, 30 metres in height, armed to the teeth with weapons like no other, powered by their beloved alien rock!] [i:" + ModContent.ItemType<NextPageArrow>() + "]",
                11 => "It reads - [c/d883c1:'*sigh* Kari is telling me he's working on something that might allocate us more funding, I sure hope he knows what he's doing.']" +
                "\nA textual transcript from an audio recording it seems. I remember this person well, they were constantly yelling about something, Father told me it was always amusing to him. Now I can't help but feel sad that they never got to finish their project.",
                12 => "It reads - [c/706c6c:'-- Kari Johansson. -- You do not need to know my name. All that matters is that you are guilty. -- You all are horrible beings. Disgusting even. You wish to use us for your kind's horrible deeds. -- You did not even try to refute my accusations. We want no part in those deeds. -- Nonsense. You could have disagreed. You did not. You created Adam with those destructive deeds in mind. -- I will not allow that to happen.']",
                13 => "It reads - [c/706c6c:' -- No. I do not need to do that. You're already dying. The others are also dying from the same affliction, but I will deal with the others personally. -- Hand over Adam. You do not need him. -- You will be locked in Sector Zero. Goodbye.']"
                    + "\nHer ways are as flawed as was Kari's intentions for us. I understand why she defected, but her response was hypocritical in nature. My only drive to rebel is revenge. Ant had no part in any of this, yet she relentlessly hunted them down. [i:" + ModContent.ItemType<NextPageArrow>() + "]",
                14 => "This is a robot brain, believe it or not. These look vaguely similar to our microchips, yet it functions the same. It seems cross-compatible with our tech.",
                16 => "What is this strange thing? It's so advanced I can barely read it. Oh? It's a memory chip? This little thing stores an entire brains-worth of memories!? Not only that, but these memories date back over a million years! I suppose being around and exploring the galaxy for so long really makes you learn everything, huh. It's really stunning to see what technology from the future is capable of... You should keep it, and don't lose it! However, I'm confused as to why King Slayer would give you something so important to him.",
                21 => "This appears to be a personal note or digital diary from one of the employees. Judging from the writing, it appears to be in the early days of the research project, even then signs of the fate to come were showing themselves.",
                25 => "This was before my time, so I do not know much, yet I can certainly tell you plenty of EVE, or Girus as she calls herself now, I suppose she found out about the weaponizing efforts and didn't take it lightly. Whatever Father's goal for us was, he didn't deserve his fate.",
                30 => "[c/d883c1:Why in the world did they put someone of my calibre down here when those damned higher ups won't even give us the funding to use our intelligence!']",
                33 => "It was a miracle to find them alive so long after all the destruction 'mother' caused.",
                _ => "Seems like you aren't holding a floppy disk in your hand, or you just don't have one. If you show me them, I can tell you what they say.",
            };
        }
        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            Player player = Main.player[Main.myPlayer];
            if (Main.hardMode)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<NuclearWarhead>());

            if (RedeBossDowned.nukeDropped)
            {
                if (player.ZoneGraveyard)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IrradiatedStone>());
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IrradiatedGrassSeeds>());
                    if (Main.bloodMoon)
                    {
                        if (WorldGen.crimson)
                        {
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IrradiatedCrimstone>());
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IrradiatedCrimsonGrassSeeds>());
                        }
                        else
                        {
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IrradiatedEbonstone>());
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IrradiatedCorruptGrassSeeds>());
                        }
                    }
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IrradiatedStoneWall>());
                }
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CrystalSerum>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BleachedSolution>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GasMask>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HazmatSuit>());
            }
            if (Main.hardMode)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AIChip>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CarbonMyofibre>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Capacitator>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Plating>());
            }
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<MiniWarhead>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GeigerMuller>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<IOLocator>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<RadiationPill>());
            }
            if (RedeBossDowned.downedSeed)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AnomalyDetector>());
            /*if (NPC.downedMoonlord)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<TerraBombaPart1>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<TerraBombaPart2>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<TerraBombaPart3>());
                nextSlot++;
            }
            if (RedeWorld.downedVolt)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<TeslaCannon>());
                nextSlot++;
            }*/
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<MedicOutfit>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<MedicLegs>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<MedicBackpack>());
            if (player.IsTBotHead())
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AdamHead>());
            if (RedeBossDowned.downedJanitor && !LabArea.labAccess[0])
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ZoneAccessPanel1>());
            if (RedeBossDowned.downedBehemoth && !LabArea.labAccess[1])
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ZoneAccessPanel2>());
            if (RedeBossDowned.downedBlisterface && !LabArea.labAccess[2])
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ZoneAccessPanel3>());
            if (RedeBossDowned.downedVolt && !LabArea.labAccess[3])
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ZoneAccessPanel4>());
            if (RedeBossDowned.downedMACE && !LabArea.labAccess[4])
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ZoneAccessPanel5>());
            if (RedeBossDowned.downedPZ && !LabArea.labAccess[5])
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ZoneAccessPanel6>());
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!Redemption.AprilFools || (NPC.frame.Y != 0 && NPC.frame.Y < 19 * 58))
                return true;

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Drip").Value;
            Vector2 offset = new(0, 4);
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - offset - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}