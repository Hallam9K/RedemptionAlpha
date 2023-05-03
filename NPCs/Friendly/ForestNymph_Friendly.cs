using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Items;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.PreHM;
using Redemption.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class ForestNymph_Friendly : ForestNymph
    {
        public override string Texture => "Redemption/NPCs/PreHM/ForestNymph";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forest Nymph");
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.AllowDoorInteraction[Type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    BuffID.Bleeding,
                    BuffID.Poisoned,
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>()
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.townNPC = true;
            NPC.friendly = true;
            if (RedeQuest.forestNymphVar < 4)
                TownNPCStayingHomeless = true;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax *= 2;
        }
        public override bool UsesPartyHat() => false;
        public override bool CanChat() => AIState <= ActionState.Wander;
        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => false;
        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Nyssa",
                "Ammi",
                "Alderis",
                "Maple",
                "Lavender",
                "Ambrose",
                "Nelida",
                "Syllessa"
            };
        }
        private bool setStats;
        private int playerFollow;
        public override void PostAI()
        {
            if (++NPC.breath <= 0)
                NPC.breath = 9000;
            if (!setStats)
            {
                if (Main.expertMode)
                {
                    NPC.lifeMax *= Main.masterMode ? 3 : 2;
                    NPC.life = NPC.lifeMax;
                }
                SetStats();
                if (RedeQuest.forestNymphVar > 1)
                {
                    NPC.lifeMax += 500;
                    NPC.life += 500;
                }
                setStats = true;
            }
            if (RedeQuest.forestNymphVar < 4)
                NPC.homeless = true;
            else
                TownNPCStayingHomeless = false;

            if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].whoAmI == NPC.whoAmI)
            {
                NPC.LookAtEntity(Main.LocalPlayer);
                AIState = ActionState.Idle;
                AITimer = 0;
            }
            else if (following && (AIState is ActionState.Idle or ActionState.Wander) && playerFollow != -1)
            {
                AITimer = 0;
                AIState = ActionState.Wander;
                moveTo = Main.player[playerFollow].Center / 16;
                if (NPC.Center.X > Main.player[playerFollow].Center.X - 2 && NPC.Center.X < Main.player[playerFollow].Center.X + 2)
                    NPC.velocity.X = 0;
            }
            if (!Main.dayTime && !NPC.homeless && !following)
            {
                if (!IsNpcOnscreen(NPC.Center) && Vector2.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), new Vector2(NPC.homeTileX, NPC.homeTileY) * 16) > Main.screenWidth / 2 + 100)
                {
                    NPC.Center = new Vector2(NPC.homeTileX, NPC.homeTileY - 2) * 16;
                    if (AIState <= ActionState.Wander)
                        AIState = ActionState.Sleeping;
                }
            }
        }
        private static bool IsNpcOnscreen(Vector2 center)
        {
            int w = NPC.sWidth + NPC.safeRangeX * 2;
            int h = NPC.sHeight + NPC.safeRangeY * 2;
            Rectangle npcScreenRect = new((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
            foreach (Player player in Main.player)
            {
                if (player.active && player.getRect().Intersects(npcScreenRect)) return true;
            }
            return false;
        }
        public override bool CheckConditions(int left, int right, int top, int bottom)
        {
            int score = 0;
            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    int type = Main.tile[x, y].TileType;
                    if (type is TileID.LivingWood or TileID.LivingMahoganyLeaves or TileID.LivingMahogany or TileID.LeafBlock)
                        score++;

                    if (Main.tile[x, y].WallType is WallID.LivingWood or WallID.LivingLeaf)
                        score++;
                }
            }

            return score >= (right - left) * (bottom - top) / 2;
        }
        public static int ChatNumber = 0;
        public bool following;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            if ((RedeWorld.alignment < 0 && !RedeBossDowned.downedTreebark) || (RedeWorld.alignment < 2 && RedeBossDowned.downedTreebark))
                return;
            button2 = "Cycle Options";
            switch (ChatNumber)
            {
                case 0:
                    switch (RedeQuest.forestNymphVar)
                    {
                        default:
                            button = "Offer Herb Bag";
                            break;
                        case 1:
                            button = "Offer Anglonic Mystic Blossom";
                            break;
                        case 2:
                            button = "Home?";
                            break;
                        case 3:
                            button = "Offer building a home";
                            break;
                        case 4:
                            if (NPC.homeless)
                                button = "Home Requirements";
                            else
                                button = "How is your home?";
                            break;
                        case 5:
                            button = "Talk";
                            break;
                    }
                    break;
                case 1:
                    button = "Trade";
                    break;
                case 2:
                    button = "Nature's Blessing";
                    break;
                case 3:
                    if (!following)
                        button = "Follow";
                    else
                        button = "Stop Following";
                    break;
                case 4:
                    button = "Decorate Hair";
                    break;
                case 5:
                    button = "Style Hair";
                    break;
                case 6:
                    button = "Request Crux";
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            Player player = Main.LocalPlayer;
            if (firstButton)
            {
                switch (ChatNumber)
                {
                    case 0:
                        if (RedeQuest.forestNymphVar == 0)
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);

                            int HerbBag = player.FindItem(ItemID.HerbBag);
                            if (HerbBag >= 0)
                            {
                                player.inventory[HerbBag].stack--;
                                if (player.inventory[HerbBag].stack <= 0)
                                    player.inventory[HerbBag] = new Item();

                                string line = Personality switch
                                {
                                    PersonalityState.Calm => "Oh, how lovely. I'll accept this beautiful array of flowers, and return the gesture by giving you this.",
                                    PersonalityState.Shy => "For me? How interesting. Would this be a gesture of kindness? I will accept. You may have this in return.",
                                    PersonalityState.Jolly => "What a cute little collection of flowers, I shall accept this gift with glee. Have this in return.",
                                    PersonalityState.Aggressive => "Trying to win my trust with flowers? Whatever, I will accept. Take this in return, if you want.",
                                    _ => "Is this a gesture of friendship? Interesting. I will accept this lovely array of flowers, and in return I will give to you this.",
                                };
                                Main.npcChatText = line;

                                RedeQuest.forestNymphVar++;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);

                                player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<ForestCore>());
                                Main.npcChatCornerItem = ModContent.ItemType<ForestCore>();
                                SoundEngine.PlaySound(SoundID.Chat);
                                return;
                            }
                            else
                            {
                                Main.npcChatCornerItem = ItemID.HerbBag;
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                        }
                        else if (RedeQuest.forestNymphVar == 1)
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);

                            int Flower = player.FindItem(ModContent.ItemType<AnglonicMysticBlossom>());
                            if (Flower >= 0)
                            {
                                player.inventory[Flower].stack--;
                                if (player.inventory[Flower].stack <= 0)
                                    player.inventory[Flower] = new Item();

                                string line = Personality switch
                                {
                                    PersonalityState.Calm => "Offering this to me is quite the shock. Whatever I may bestow in return would not rival such rarity. T",
                                    PersonalityState.Shy => "For me? Are you sure? It is such a rarity I can not simply comprehend as a mere gift. It almost makes me nervous - to give away so freely, you must have a golden heart. But I do not know what to give in return! Forgive me, t",
                                    PersonalityState.Jolly => "Goodness me! Such a rarity to bequeath to me, I am astounded by your generosity. Unfortunately I have not one thing in my possessions that may match your gift. T",
                                    PersonalityState.Aggressive => "You... Goodness. Why would you possibly give me such a rarity! I cannot help but sense malicious intent. And yet, the World says you are good, and the World does not lie. In return for this, t",
                                    _ => "This is... To offer such rarity to me is bewildering, to say the least. What may I bestow that may rival this? My possessions are limited, however my blessings are boundless. T",
                                };
                                Main.npcChatText = line + "ake my weapon, for it is the only tangible gift I have.";
                                NPC.lifeMax += 500;
                                NPC.life += 500;
                                RedeQuest.forestNymphVar++;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);

                                player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<ForestNymphsSickle>());
                                Main.npcChatCornerItem = ModContent.ItemType<ForestNymphsSickle>();
                                SoundEngine.PlaySound(SoundID.Chat);
                                return;
                            }
                            else
                            {
                                Main.npcChatCornerItem = ModContent.ItemType<AnglonicMysticBlossom>();
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                        }
                        else if (RedeQuest.forestNymphVar == 2)
                        {
                            SoundEngine.PlaySound(SoundID.Chat);
                            Main.npcChatText = "We usually live in giant hollowed-out trees with a body of water nearby. The tree is only good for hiding, but the water is vital for our survival, as we cannot blossom without it. The living trees this island has are the closest things to the great oaks back through the portal, but they could use some sprucing up. They aren't exactly homely.";
                            RedeQuest.forestNymphVar++;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);

                        }
                        else if (RedeQuest.forestNymphVar >= 3)
                        {
                            SoundEngine.PlaySound(SoundID.Chat);
                            if (RedeQuest.forestNymphVar >= 5)
                            {
                                Main.npcChatText = ChitChat();
                                break;
                            }
                            if (RedeQuest.forestNymphVar == 3)
                            {
                                Main.npcChatText = "Your fae spoke truthfully about you. If it wouldn't be too much work, I will accept a home you create for me. I have requirements, however. For one, I would prefer if it was made of living wood - I give you permission to renovate an existing living tree if so inclined, but as long as it's made of living wood I'll be satisfied. Secondly, I need a body of water nearby. A small one is fine, just as long as I can submerge my roots into it.";
                                RedeQuest.forestNymphVar++;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            else if (RedeQuest.forestNymphVar == 4)
                            {
                                if (NPC.homeless)
                                    Main.npcChatText = "I would prefer if it was made of living wood - I give you permission to renovate an existing living tree if so inclined, but as long as it's made of living wood I'll be satisfied. Secondly, I need a body of water nearby. A small one is fine, just as long as I can submerge my roots into it.";
                                else
                                {
                                    int score = 0;
                                    for (int x = -40; x <= 40; x++)
                                    {
                                        for (int y = -25; y <= 25; y++)
                                        {
                                            Tile tile = Framing.GetTileSafely(NPC.homeTileX + x, NPC.homeTileY + y);
                                            if (tile.LiquidAmount >= 255 && tile.LiquidType == LiquidID.Water)
                                                score++;
                                        }
                                    }
                                    if (score < 20)
                                    {
                                        string line = Personality switch
                                        {
                                            PersonalityState.Calm => "Make a nice body of water close at hand, and I'll accept this new home.",
                                            PersonalityState.Shy => "If you can, please create a body of water close to my new home.",
                                            PersonalityState.Jolly => "A small lake or pond is very important for us forest nymphs. I'll be most pleased if one was present.",
                                            PersonalityState.Aggressive => "I did ask for a body of water, did I not? Make one and I'll accept this place as my home.",
                                            _ => "If you can get a body of water to be close to my new home I will accept it.",
                                        };
                                        Main.npcChatText = "It is suitable. I can see myself getting comfortable in here for now, however I see no pools of water nearby. " + line + "\n\n(" + score.ToString() + "/20 water required)";
                                    }
                                    else
                                    {
                                        string line = Personality switch
                                        {
                                            PersonalityState.Calm => "This is a pleasant place. I am thankful for it, and all you have done. If only the other humans were so kind...",
                                            PersonalityState.Shy => "This place is warm and cosy, I was very nervous around most humans, but it would seem my fear was misplaced...",
                                            PersonalityState.Jolly => "Oh, it is beautiful! A wonderful little abode for me. I cannot thank you enough, to think a human would be so giving...",
                                            PersonalityState.Aggressive => "It's alright. Nothing too special, I guess. Actually, nevermind, I am being too harsh. Most humans have not been too kind to me...",
                                            _ => "It is a lovely abode. You have helped me more than most humans...",
                                        };
                                        Main.npcChatText = line + " You are human, are you not? The World does not recognise you as its child, but no matter. You have my gratitude.";

                                        if (RedeQuest.forestNymphVar < 5)
                                        {
                                            RedeWorld.alignment++;
                                            for (int p = 0; p < Main.maxPlayers; p++)
                                            {
                                                Player player2 = Main.player[p];
                                                if (!player2.active)
                                                    continue;

                                                CombatText.NewText(player2.getRect(), Color.Gold, "+1", true, false);

                                                if (!RedeWorld.alignmentGiven)
                                                    continue;

                                                if (!Main.dedServ)
                                                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Sometimes it is the little acts of kindness that count.", 240, 30, 0, Color.DarkGoldenrod);

                                            }
                                            RedeQuest.forestNymphVar = 5;
                                        }
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.WorldData);
                                    }
                                }
                            }
                        }
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        TradeUI.Visible = true;
                        break;
                    case 2:
                        for (int i = 0; i < 20; i++)
                        {
                            int dustIndex = Dust.NewDust(new Vector2(Main.LocalPlayer.position.X, Main.LocalPlayer.Bottom.Y - 2), Main.LocalPlayer.width, 2, DustID.DryadsWard);
                            Main.dust[dustIndex].velocity.Y = -Main.rand.Next(3, 7);
                            Main.dust[dustIndex].velocity.X = 0;
                            Main.dust[dustIndex].noGravity = true;
                        }
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.position);
                        Main.LocalPlayer.AddBuff(BuffID.Lucky, 10800 * ((RedeWorld.alignment / 2) + 1));
                        break;
                    case 3:
                        playerFollow = Main.LocalPlayer.whoAmI;
                        following = !following;
                        if (following)
                        {
                            string line = Personality switch
                            {
                                PersonalityState.Calm => "Sure. It better be worth it.",
                                PersonalityState.Shy => "I'm not sure... Alright, fine, just don't take me too far from home.",
                                PersonalityState.Jolly => "Follow? I'll agree, but to where I wonder?",
                                PersonalityState.Aggressive => "Fine. But if you lead me into a pack of ghouls I will not be happy in the slightest.",
                                _ => "As you wish. I will not be pleased if you make me wonder too far from my territory though.",
                            };
                            Main.npcChatText = line;
                        }
                        break;
                    case 4:
                        FlowerType++;
                        if (FlowerType > 5)
                            FlowerType = 0;
                        break;
                    case 5:
                        HairExtType++;
                        if (HairExtType > 2)
                            HairExtType = 0;
                        break;
                    case 6:
                        if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                        {
                            Main.npcChatText = "I cannot gift it to thee unless you're partly in the realm of spirits.";
                            ChatNumber--;
                            return;
                        }
                        int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                        if (card >= 0)
                        {
                            Main.LocalPlayer.inventory[card].stack--;
                            if (Main.LocalPlayer.inventory[card].stack <= 0)
                                Main.LocalPlayer.inventory[card] = new Item();

                            Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardForestNymph>());
                            Main.npcChatText = "Fascinating. I never would've expected one such as yourself to have this ability. As you wish, I will give to thee the spirit of my kind.";
                            Main.npcChatCornerItem = ModContent.ItemType<CruxCardForestNymph>();
                            SoundEngine.PlaySound(SoundID.Chat);
                        }
                        else
                        {
                            Main.npcChatText = "You bare no object to imbue.";
                            Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                        }
                        ChatNumber--;
                        break;
                }
            }
            else
            {
                bool skip = true;
                while (skip)
                {
                    ChatNumber++;
                    if (ChatNumber > 6)
                        ChatNumber = 0;
                    if (RedeQuest.forestNymphVar < 1 && (ChatNumber == 4 || ChatNumber == 5))
                        skip = true;
                    else if (RedeWorld.alignment <= 0 && ChatNumber == 2)
                        skip = true;
                    else if (RedeQuest.forestNymphVar < 2 && ChatNumber == 3)
                        skip = true;
                    else if (RedeQuest.forestNymphVar < 5 && ChatNumber == 1)
                        skip = true;
                    else if (ChatNumber == 6 && (RedeQuest.forestNymphVar < 5 || !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive || Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardForestNymph>())))
                        skip = true;
                    else
                        skip = false;
                }
            }
        }
        private string ChitChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            string bothChat = BothChat();
            if (bothChat != null)
                chat.Add(bothChat);
            chat.Add("Do you wonder why there aren't more of us around? It is simply the matter of strict requirements for our seeds to grow, and the centuries it takes for them to fully blossom. We usually produce seeds in the Spring season once every decade - a long time for humans. If you see a peculiar bulb near my pond, I'd recommend saying farewell to me and this place indefinitely. A blooming nymph is a fragile thing, and I won't take any chances.");
            chat.Add("I have learnt many things from my observations of humans, along with the languages they speak. I understand they have a false perception of us, some say we were created by the World to replicate the \"perfect image of humans\". Of course, that is merely a rumour fuelled with egotism. Humans are far from perfect, form or otherwise.");
            chat.Add("Artistic depictions created by humans show us having a much more similar appearance to them than what is accurate. They seldom get the chance to get a close look at us so it is understandable. I came across such artwork in a book I stole, it was... the complete opposite of flattering.");
            string s = "";
            if (NPC.GivenName == "Nyssa")
                s = " I found it so humorous that I renamed myself to Nyssa.";
            chat.Add("Humanity's intrigue of us have generated assumptions of how we act and our culture. I've even heard of a fictional figure known as Nyssa, the Forest Nymph Queen - which is based on the idea that we have some sort of hierarchy, which is false." + s);
            if (RedeWorld.alignment >= 4)
                chat.Add("You're curious about our connection with the World, are you not? It is a rather well-kept secret, but the World trusts you enough for me to say. With meditation, we nymphs are able to call upon Epidotra and speak to it; this grants insight beyond human limits, which can be used to figure out a creature's true intentions or to be alerted of dangers - natural and unnatural - that besiege the earth.");
            else
                chat.Add("You're curious about our connection with the World, are you not? Hmm... It is a secret well-kept from others, and I will keep it that way for the time being.");
            return chat;
        }
        private string BothChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            Player player = Main.player[Main.myPlayer];
            if (RedeBossDowned.downedTreebark)
                chat.Add("A forewarning to you, I will never forget your act of felling towards the tree-folk. I only speak to you now for the good you have done despite that.");
            if (RedeBossDowned.downedADD)
                chat.Add("I had sensed the return of a great presence, however it was short-lived. You defeated her I presume? A wise decision - Akka was a friend of Nature only in soul. Her mind, however, was only filled with demand and grim desires. Ukko, her husband, was the same.");
            if (RedeBossDowned.nukeDropped)
                chat.Add("I do not approve of the horrific destruction you have caused to a portion of the land, what manner of power caused this? A blast that withers earth, mutates plant, and decays air. Such power is otherworldly.");
            if (BasePlayer.HasArmorSet(player, "Common Guard", true) || BasePlayer.HasArmorSet(player, "Common Guard", false))
            {
                chat.Add("Common Guard, are you? Members of their troop used to lurk around the forests I came from. They got too close for comfort so I left and found the portal here.");
            }
            if (RedeBossDowned.downedThorn)
            {
                chat.Add("The World tells me you slew that blighted warden of Faywood. Shame he had to fall, but at least that horrid blight is ceasing. I kept my distance from people back at my old home, though I do recall his name - Alder, it was.");
            }
            if (BasePlayer.HasArmorSet(player, "Dryad", true) || BasePlayer.HasArmorSet(player, "Dryad", false))
                chat.Add("I see you too have no use for neither fancy nor practical attire.");
            if (BasePlayer.HasHelmet(player, ItemID.GarlandHat, true))
            {
                string line = Personality switch
                {
                    PersonalityState.Calm => "Although, I do find it quite cute.",
                    PersonalityState.Shy => "I'll take it as a compliment.",
                    PersonalityState.Jolly => "It definitely suits you though!",
                    PersonalityState.Aggressive => "Probably the latter.",
                    _ => "It suits you.",
                };
                chat.Add("I see you have decorated your hair with various flowers, to mimic or to mock me? That remains unclear. " + line);
            }
            return chat;
        }
        public override string GetChat()
        {
            Main.LocalPlayer.currentShoppingSettings.HappinessReport = "";

            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);
            if ((RedeWorld.alignment < 0 && !RedeBossDowned.downedTreebark) || (RedeWorld.alignment < 2 && RedeBossDowned.downedTreebark))
                return "Leave. The fae's trust in you was an act of folly, the World deems you a danger.";

            if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                chat.Add("Oh, how vulgar. You are able to see within the other realm and as such are bearing witness to my ethereal form. Or mayhaps you aren't submerged deep enough to see it in full? No matter.", 2);
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<ThornMask>(), true))
                chat.Add("You aren't blighted, are you? No, it is just a mask.");
            if (BasePlayer.HasArmorSet(player, "Living Wood", true) || BasePlayer.HasArmorSet(player, "Living Wood", false))
            {
                chat.Add("You better hope that wood you don is not from my home.");
                chat.Add("You don the wood of a living tree. If I find out you've taken it from my home, I won't be happy.");
            }
            string bothChat = BothChat();
            if (bothChat != null)
                chat.Add(bothChat);

            if (Personality is PersonalityState.Aggressive && RedeQuest.forestNymphVar < 2)
                chat.Add("Better watch yourself.");
            if (Personality is PersonalityState.Shy)
                chat.Add("...");
            if (RedeQuest.forestNymphVar < 2)
                chat.Add("Don't make yourself too comfy, I only trust you because of your fae's kind words about you.");
            else
                chat.Add("The blossom you gave me is often gifted to others to bring good luck and the blessing of a long life. Being who I am - a forest nymph, as you'd call me - I can use these blossoms to grant me a greater soul and connection with the World; Epidotra, people call it.");
            if (Personality is PersonalityState.Jolly)
                chat.Add("Come to talk to me? I don't mind the company, as long as nature deems you well.");
            else if (RedeQuest.forestNymphVar < 2)
                chat.Add("What do you want? I'd rather be left alone.");
            else
                chat.Add("We nymphs are rather solitary beings, so consider the fact that I don't mind your presence a blessing.");
            return chat;
        }
        public override void LoadData(TagCompound tag)
        {
            Personality = (PersonalityState)tag.GetInt("Personality");
            EyeType = tag.GetInt("EyeType");
            HairExtType = tag.GetInt("HairExtType");
            HairType = tag.GetInt("HairType");
            HasHat = tag.GetBool("HasHat");
            FlowerType = tag.GetInt("FlowerType");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Personality"] = (int)Personality;
            tag["EyeType"] = EyeType;
            tag["HairExtType"] = HairExtType;
            tag["HairType"] = HairType;
            tag["FlowerType"] = FlowerType;
            tag["HasHat"] = HasHat;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0;
    }
}