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
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Usable;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Materials.HM;
using ReLogic.Content;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Daerel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wayfarer");
            Main.npcFrameCount[Type] = 26;
            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 1000;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 20;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 8;

            NPC.Happiness
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Hate)
                .SetNPCAffection<Zephos>(AffectionLevel.Love)
                .SetNPCAffection(NPCID.Stylist, AffectionLevel.Like)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Merchant, AffectionLevel.Hate);

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
            NPC.height = 48;
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

                new FlavorTextBestiaryInfoElement("A traveller from mainland Epidotra who is friends with Zephos. Most skilled at archery and stealth."),
            });
        }

        public override bool CheckDead()
        {
            RedeWorld.daerelDownedTimer = 0;
            Main.NewText("Daerel the Wayfarer was knocked unconscious...", Color.Red.R, Color.Red.G, Color.Red.B);
            NPC.SetDefaults(ModContent.NPCType<DaerelUnconscious>());
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

            if (NPC.altTexture == 1)
            {
                Asset<Texture2D> hat = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.PartyHat);
                var offset = (NPC.frame.Y / 56) switch
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
                spriteBatch.Draw(hat.Value, NPC.Center - new Vector2(1 * NPC.spriteDirection, 25 + offset) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, hatEffects, 0);
            }
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
            return WorldGen.crimson && RedeQuest.wayfarerVars[0] >= 2 && !RedeHelper.DaerelActive();
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Daerel" };
        }
        public override ITownNPCProfile TownNPCProfile() => new DaerelProfile();
        public override string GetChat()
        {
            adviceNum = 0;
            WeightedRandom<string> chat = new(Main.rand);
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                chat.Add("Hello again, sorry for the intrusion but I've lost my friend through that portal. Mind if I stay here to get some supplies? I'm sure I'll find him eventually.");
            }
            else
            {
                int PartyGirlID = NPC.FindFirstNPC(NPCID.PartyGirl);
                if (PartyGirlID >= 0)
                    chat.Add("I swear " + Main.npc[PartyGirlID].GivenName + " reminds me of a technicoloured pony from another universe...", 0.2);

                if (Main.LocalPlayer.ZoneGraveyard)
                    chat.Add("The atmosphere here is making my spine shiver, please let me live somewhere less spooky.");
                chat.Add("Need anything? I can restring your bow, or poison your weapon. It'll cost you though.");
                if (!NPC.homeless)
                {
                    if (Main.raining && Main.LocalPlayer.ZoneOverworldHeight)
                        chat.Add("The tipper-tapper of rain in the confines of a cosy home never gets old.");

                    chat.Add("You don't mind me staying here, right?");
                    chat.Add("I've been travelling this land for a while, but staying in a house is nice.");
                }
                else
                {
                    if (Main.raining && Main.LocalPlayer.ZoneOverworldHeight)
                        chat.Add("I'm only a fan of rain while in the confines of a cosy home, not when it's dampening my clothes.");
                }
                chat.Add("I got some pretty nice loot I can sell you, I kinda need money right now.");
                chat.Add("My favourite colour is green, not sure why I'm telling you though...");
                chat.Add("Cats are obviously superior to dogs.");
                chat.Add("Have you seen a guy with slicked back, hazel hair? He carries a sword and wears a green tunic last I saw. I lost him before travelling through the portal, hope he's doing alright.");
                chat.Add("One time me and Zephos were in a cave, and then a skeleton with flowers stuck in its ribcage appeared. Zephos thought it was a powerful druid skeleton. He likes to exaggerate. It didn't have any magic, it was just a normal skeleton.");
                chat.Add("Cool Bug Fact: Coast Scarabs are small beetles that live on sandy beaches and eat grains of sand as their primary diet. When wet, their cyan shells will shine. Their shell is normally used to make cyan dyes.");
                chat.Add("Cool Bug Fact: Sandskin Spiders live in deserts, roaming around at night when other tiny insects come out to eat. When the hot day arrives, the spider will borrow a feet under the sand to sleep. Yes, I like bugs.");
                chat.Add("Living Blooms roam this island? They are native to Anglon's lush forests. Living Blooms are more plant than animal. Seems like many creatures got to this island from the portal.");
                if (!Main.dayTime)
                    chat.Add("There are zombies here? Not that I'm surprised, there are many types of undead on the mainland too.");
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
                        button = "Restring Bow (10 silver)";
                        break;
                    case 3:
                        button = "Poison Weapon (25 silver)";
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
                            Main.npcChatText = "Thank you. I'm just here for some resources, not any of your own possessions. Just a few things to help me find my friend. I'm Daerel, by the way. Nice to meet you.";
                            RedeQuest.wayfarerVars[0] = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = "Oh yes, sorry, I'm Daerel. I'm decent with a bow and that's about it right now. Soon I hope to be more skilled in my craft, once I do, I'm certain your humble little island will have a fine archer one day. As of now, I must attend to the matter of my friend and gather a few helpful resources. I hope my presence doesn't intrude on anything.";
                            RedeQuest.wayfarerVars[0] = 3;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 3:
                        if (firstButton)
                        {
                            Main.npcChatText = "Thank you. I'm just here for some resources, not any of your own possessions. Just a few things to help me find my friend. Nice to meet you.";
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
                            adviceNum++;
                            break;
                        case 2:
                            if (Main.LocalPlayer.BuyItem(1000))
                            {
                                SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                Main.LocalPlayer.AddBuff(BuffID.Archery, 21600);
                            }
                            else
                            {
                                Main.npcChatText = NoCoinsChat();
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                            break;
                        case 3:
                            if (Main.LocalPlayer.BuyItem(2500))
                            {
                                SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                Main.LocalPlayer.AddBuff(BuffID.WeaponImbuePoison, 36000);
                            }
                            else
                            {
                                Main.npcChatText = NoCoinsChat();
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                            break;
                    }
                }
                else
                {
                    ChatNumber++;
                    if (ChatNumber > 3)
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

        private static int adviceNum;
        public static string ChitChat()
        {
            List<string> chat = new();
            if (RedeBossDowned.downedPZ && !RedeBossDowned.downedNebuleus)
                chat.Add("Honestly, I'm surprised you still come to me for advice. You seem like a smart person, far more than I. But anyway, I've recently been seeing interesting wyverns in the sky - purple and gold. Give it a check if you wish.");
            int FallenID = NPC.FindFirstNPC(ModContent.NPCType<Fallen>());
            if (FallenID >= 0 && Main.LocalPlayer.HasItem(ModContent.ItemType<GolemEye>()) && NPC.downedMoonlord && !RedeBossDowned.downedADD)
                chat.Add("Hmm, that glowing eye you got looks rather... mythical. And it's giving you a riddle too? \"Surround it with the stones of its origin\"... Well you need to place some stone around it, the question is what kind? Maybe ask " + Main.npc[FallenID].GivenName + ".");
            if (Main.hardMode && !RedeBossDowned.downedSlayer)
                chat.Add("I've seen some robots meandering about the island, scanning random objects and creatures. No idea where they came from, but I did see one get attacked. Once it was weakened, it stopped moving and teleported into the sky. Their purpose is a mystery to me. Maybe you could figure it out?");
            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            if (DryadID >= 0 && RedeQuest.forestNymphVar == 0)
                chat.Add("The dryad, " + Main.npc[DryadID].GivenName + ", says she's seen a Forest Nymph on this island at one point, if you can believe that. They're seldom seen, you'd only be able to find them near giant trees. If you do come across one, I wouldn't linger around for too long, they don't like humans getting in their personal space. I wonder if there were a way to befriend one though?");
            if (FallenID >= 0 && !Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                chat.Add(Main.npc[FallenID].GivenName + " has told me he came from another portal underground. Apparently it leads to some catacombs in Gathuram, but you wouldn't be able to go through it. Still, he's told of some rather intriguing things lying by the portal, I'd give it a check if I were you.");
            if (!RedeBossDowned.downedEaglecrestGolem)
                chat.Add("While I was having a walk I came across some oddly-shaped stones - looked like a boulder with legs. I was curious of course, so I shot it from a safe distance. Nothing happened... and yet I sensed a presence inside it. You, as a slayer of many things, should search around and find it, might be another foe to face.");
            if (!Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                chat.Add("Ever see tiny lights dancing from a slain skeleton? Or perhaps a lantern-carrying ghost underground? Those are lost souls, and as far as I know, only arcane or holy weapons may bring them harm. Not that I'd suggest harming those helpless things.");
            chat.Add("Wanna know about some insects? If you wanna find leaf beetles, or tree bugs as they're called here, then chop down some trees. They live on tree tops, with their green shell camouflaging them in the foliage. Coast Scarabs can also be found on palm trees at the beach, their shells sparkle when wet. Grand larvae are, well, rather gross - though they can make excellent bait. That is, if you're brave enough to get close to one.");
            chat.Add("When encountering skeletons and undead, I think its logical to assume shadow weapons aren't effective against them, while holy weapons are. I used to like the exploring caves, until me and Zephos encountered a skeleton Vex...");
            chat.Add("Best way to deal with slimes? Burn them. Alternatively, ice weapons can freeze them in their place, THEN you can burn them!");
            chat.Add("If you ever wanna sneak up on the Epidotrian skeletons or chickens, invisibility potions are real handy for the job.");
            chat.Add("The weapons skeletons can wield are very rusty, so it'd be bad to be wounded by them. If you do, take a dip in some water and the dirty wound will disappear.");
            chat.Add("See foes wearing armor or holding shields? You'll need to break their Guard to deal with them. A hammer or explosives should be most efficient, just make sure your weapon isn't super weak.");
            if (!RedeBossDowned.foundNewb)
                chat.Add("I felt a strange presence beneath that portal I hopped out of, which is quite peculiar. Maybe you should check it out.");
            if (RedeBossDowned.erhanDeath == 0)
                chat.Add("I noticed a scroll sitting atop a small table next to the portal I came out of, did you pick it up yet? It looked rather... demonic.");

            string[] chatStr = chat.ToArray();
            int maxAdvice = chatStr.Length;
            if (adviceNum >= maxAdvice)
                adviceNum = 0;

            string num = "(" + (adviceNum + 1) + "/" + maxAdvice + ") ";
            return num + chatStr[adviceNum];
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ItemID.Leather);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FlintAndSteel>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BeardedHatchet>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CantripStaff>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DurableBowString>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Archcloth>());

            if (NPC.downedBoss2)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EaglecrestSpelltome>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SilverwoodBow>());
            }

            if (RedeBossDowned.downedEaglecrestGolem)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GolemEye>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ChaliceFragments>());

            if (NPC.downedGolemBoss)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<OphosNotes>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<KingChickenPainting>());
            if (RedeBossDowned.downedFowlEmperor)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FowlEmperorPainting>());
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
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DarkSteelBow>());
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
            damage = 13;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 20;
            randExtraCooldown = 10;
        }

        public override void DrawTownAttackGun(ref float scale, ref int item, ref int closeness)
        {
            scale = 1f;
            item = ModContent.ItemType<SilverwoodBow>();
            closeness = 20;
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.WoodenArrowFriendly;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 10f;
        }
    }
    public class DaerelProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Daerel");
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/Daerel_Head");
    }
}