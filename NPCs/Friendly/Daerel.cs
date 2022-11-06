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

        public override string GetChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            if (RedeQuest.wayfarerVars[0] < 4)
            {
                chat.Add("Hello again, sorry for the intrusion but I've lost my friend through that portal. Mind if I stay here to get some supplies? I'm sure I'll find him eventually.");
            }
            else
            {
                int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
                if (DryadID >= 0)
                    chat.Add("Is " + Main.npc[DryadID].GivenName + " a half-Nymph? Or just a weirdo who doesn't wear actual clothes?");

                int PartyGirlID = NPC.FindFirstNPC(NPCID.PartyGirl);
                if (PartyGirlID >= 0)
                    chat.Add("I swear " + Main.npc[PartyGirlID].GivenName + " reminds me of a technicoloured pony from another universe...", 0.2);

                chat.Add("Need anything? I can restring your bow, or poison your weapon. It'll cost you though.");
                if (!NPC.homeless)
                {
                    chat.Add("You don't mind me staying here, right?");
                    chat.Add("I've been travelling this land for a while, but staying in a house is nice.");
                }
                chat.Add("I got some pretty nice loot I can sell you, I kinda need money right now.");
                chat.Add("My favourite colour is green, not sure why I'm telling you though...");
                chat.Add("Cats are obviously superior to dogs.");
                chat.Add("Have you seen a guy with slicked back, hazel hair? He carries a sword and wears a green tunic last I saw. I lost him before travelling through the portal, hope he's doing alright.");
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
                        button = "Talk";
                        break;
                    case 2:
                        button = "Restring Bow (10 silver)";
                        break;
                    case 3:
                        button = "Poison Weapon (25 silver)";
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
                            Main.npcChatText = "Thank you. I'm just here for some resources, not any of your own possessions. Just a few things to help me find my friend. I'm Daerel, by the way. Nice to meet you.";
                            RedeQuest.wayfarerVars[0] = 4;
                        }
                        else
                        {
                            Main.npcChatText = "Oh yes, sorry, I'm Daerel. I'm decent with a bow and that's about it right now. Soon I hope to be more skilled in my craft, once I do, I'm certain your humble little island will have a fine archer one day. As of now, I must attend to the matter of my friend and gather a few helpful resources. I hope my presence doesn't intrude on anything.";
                            RedeQuest.wayfarerVars[0] = 3;
                        }
                        break;
                    case 3:
                        if (firstButton)
                        {
                            Main.npcChatText = "Thank you. I'm just here for some resources, not any of your own possessions. Just a few things to help me find my friend. Nice to meet you.";
                            RedeQuest.wayfarerVars[0] = 4;
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
                            if (Main.LocalPlayer.BuyItem(1000))
                            {
                                SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                                Main.LocalPlayer.AddBuff(BuffID.Archery, 36000);
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
            chat.Add("I've lost someone, his name is Zephos and wears a green tunic. I'm sure I'll find him eventually.");
            chat.Add("One time me and Zephos were in a cave, and then a skeleton with flowers stuck in its ribcage appeared. Zephos thought it was a powerful druid skeleton. He likes to exaggerate. It didn't have any magic, it was just a normal skeleton.");
            chat.Add("If you wanna find Leaf Beetles, or Tree Bugs as they're called here, then chop down some trees. They live on tree tops, their leaf-green shell camouflaging them in the foliage. They eat the bark off of trees, and if their tree is destroyed or rotted, it will climb down and find another suitable tree to live on.");
            chat.Add("Cool Bug Fact: Coast Scarabs are small beetles that live on sandy beaches and eat grains of sand as their primary diet. When wet, their cyan shells will shine. Their shell is normally used to make cyan dyes.");
            chat.Add("Cool Bug Fact: Sandskin Spiders live in deserts, roaming around at night when other tiny insects come out to eat. When the hot day arrives, the spider will borrow a feet under the sand to sleep. Yes, I like bugs.");
            chat.Add("When encountering skeletons and undead, I think its logical to assume shadow weapons aren't effective against them, while holy weapons are. I used to like the exploring caves, until me and Zephos encountered a skeleton Vex...");
            chat.Add("Best way to deal with slimes? Burn them. Alternatively, ice weapons can freeze them in their place, THEN you can burn them!");
            chat.Add("If you ever wanna sneak up on the Epidotrian skeletons or chickens, invisibility potions are real handy for the job.");
            chat.Add("The weapons skeletons can wield are very rusty, so it'd be bad to be wounded by them. If you do, take a dip in some water and the dirty wound will disappear.");
            chat.Add("Living Blooms roam this island? They are native to Anglon's lush forests. Living Blooms are more plant than animal. Seems like many creatures got to this island from the portal.");
            if (!Main.dayTime)
                chat.Add("There are zombies here? Not that I'm surprised, there are many types of undead on the mainland too.");
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

            if (NPC.downedBoss2)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EaglecrestSpelltome>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SilverwoodBow>());
            }

            if (RedeBossDowned.downedEaglecrestGolem)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GolemEye>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ChaliceFragments>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<KingChickenPainting>());
            if (NPC.downedBoss1)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PonderingTreesPainting>());
            if (Main.hardMode)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AkkaPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AncientAutoPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DubiousWatcherPainting>());
            }
            if (NPC.downedPlantBoss)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<UkkoPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EmeraldHeartPainting>());
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
}