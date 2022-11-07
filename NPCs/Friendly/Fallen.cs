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
using Redemption.Items.Accessories.PreHM;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Armor.Single;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.Items.Usable;

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

                new FlavorTextBestiaryInfoElement("Fallen are a category of undead with a soul strong enough to form pale brown flesh. Most being aggressive towards humans, this one is a rare case who can sell ritualist equipment and repair fragments of ancient weapons."),
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
            return RedeBossDowned.downedKeeper;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Happins", "Tenvon", "Okvot" };
        }

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

                chat.Add("I wouldn't consume that Blackened Heart if I were you. Only bad things will come of it.");
            }

            chat.Add("Wanna know what lost souls are? When a living thing dies, its soul leaves the body, these are lost souls. they search around the world to look for corpses to infuse with. To ordinary people, lost souls are invisible, but some who use magic can see them fade in and out of the Spirit Realm.");
            chat.Add("The size of the Lost Souls depends on the original individual's power, the stronger the Will of the being, the bigger the soul. Once a soul leaves the body, they cannot infuse with it again. When the soul has found a worthy vessel, it fuses with it. On most occasions, a skeleton will be created. However, if the soul is strong enough, it will form pale-brown flesh on the skeleton, creating a Fallen.");

            switch (FallenType)
            {
                case 0:
                    if (RedeBossDowned.keeperSaved)
                        chat.Add("You saved the Keeper? Thank you for that, I can't imagine the pain she was feeling. If you need Grim Shards, I'll sell them now for you. However, I doubt this is over... Her husband died too, apparently of depression. But he didn't become an undead, his inverted soul made him into something worse.", 3);

                    chat.Add("I may be undead, but I hope for a world where no tears are shed, and no pain is spread. A world of peace. That is who I was before dying, and despite the undead tendency to be more aggressive, I feel the same as I always have.");
                    chat.Add("Do not worry, human. I bring no hatred where I go, despite my undead look, I won't harm you. And I hope you won't harm me either.");
                    chat.Add("Times may come when you have hardship, maybe you struggle fighting a tough enemy, maybe you feel alone on this island with only enemies everywhere you go, but I'll still be here. Don't let my rotten looks deceive you, young one. I will help you.");
                    chat.Add("The wind sings the longest tune... Do you hear it?");
                    chat.Add("In the lowest level of the Catacombs of Gathuram, the floor is littered with broken bones and puddles of water. Dim blue lights often were visible in this level, and nothing is alive there. So if any were to survive the fall, they would be alone forever.");
                    chat.Add("My name's Okvot, I usually sell equipment to other Fallen to help them survive in the Catacombs of Gathuram, however, destiny has brought me into the outside world. I hope my junk can be of use to you.", 2);
                    break;
                case 1:
                    if (RedeBossDowned.keeperSaved)
                        chat.Add("You saved the Keeper? Bah, I guess that's nice of ya. If you need Grim Shards, I'll sell them now for you.", 3);

                    chat.Add("I'm not very interested in talking, what ya want?");
                    chat.Add("Darkness... Ha! What a strange term. You humans fear it more than death itself. You cower in the face of the overwhelming shadow of the night. Pitiful creature! It is not the darkness you should fear, but what lurks within it. So, did I spook ya?! Hahaha!");
                    chat.Add("Ya know what Willpower is, right? It's the essence of yer soul! The stronger yer will to live, the bigger the soul. Did you know you can die of depression? Apparently if you have no will to live, your soul can invert! I got a ton'a willpower! I ain't dying anytime soon... Again.");
                    chat.Add("Give me some of ya sweet doruls! I got some junk to sell.");
                    chat.Add("Once a soul leaves the body, they cannot infuse with it again. When the soul has found a worthy vessel, it fuses with it. Most of the time, a skeleton will be made. However, if the soul is strong enough, it will form pale-brown flesh on the skeleton, creating a Fallen. Like me!");
                    chat.Add("Ever been to Spiritpeak Forest? A quarter of the forest is a giant graveyard, meaning there be a staggerin' number of skeletons, wandering souls and spirits. I used to take walks there when I was still alive.");
                    chat.Add("M'name is Tenvon, I'm a blacksmith, but I guess I'm here now. I also should mention, I got some junk for sale, if ya interested.");
                    break;
                case 2:
                    if (RedeBossDowned.keeperSaved)
                        chat.Add("You saved the Keeper? That's great to hear, she gave me trouble when she had escaped the catacombs. If you need Grim Shards, I'll sell them now for you.", 3);

                    chat.Add("In my first few days of becoming Fallen, every other undead tried to kill me. It was a very scary experience, but I escaped the catacombs and now I'm here.");
                    chat.Add("Do you want to know about Pure-Iron? It's an extremely durable metal only found in the southern region of the world, in Gathuram. I am wearing a Pure-Iron helmet right now in fact.");
                    chat.Add("At the start, this whole 'Fallen' thing was a little overwhelming. I didn't want to 'live' with the fact that I had died, even though I'm not 'living', haha. But I'm more accepting of this now, I'm undead, humans hate me, deal with it. Actually, you're a human, right? Why aren't you attacking me?");
                    chat.Add("You want to know about the Warriors of the Iron Realm? They are the domain of Gathuram's primary warriors. They normally wear Pure-Iron armour.");
                    chat.Add("The Catacombs of Gathuram - where my soul found a vessel - are a seemingly endless network of underground tunnels, crypts, and dungeons spanning all across the Iron Realm. It is considered the largest underground structure in Epidotra.");
                    chat.Add("Hello, the name's Happins. I used to be a Warrior of the Iron Realm... Until I was killed of course.", 2);
                    break;
            }
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");

            button2 = "Repair Fragments";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            Player player = Main.LocalPlayer;

            if (firstButton)
                shop = true;
            else
            {
                int[] Frag = new int[] {
                    player.FindItem(ModContent.ItemType<ZweihanderFragment1>()),
                    player.FindItem(ModContent.ItemType<ZweihanderFragment2>()) };

                if (Frag[0] >= 0 && Frag[1] >= 0)
                {
                    player.inventory[Frag[0]].stack--;
                    player.inventory[Frag[1]].stack--;
                    if (player.inventory[Frag[0]].stack <= 0)
                        player.inventory[Frag[0]] = new Item();
                    if (player.inventory[Frag[1]].stack <= 0)
                        player.inventory[Frag[1]] = new Item();

                    Main.npcChatCornerItem = ModContent.ItemType<Zweihander>();
                    Main.npcChatText = "All done and repaired, here you go.";
                    player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<Zweihander>());

                    SoundEngine.PlaySound(SoundID.Item37, NPC.position);
                    return;
                }
                else
                {
                    Main.npcChatText = "You don't seem to have any fragments on your possession.";
                    for (int k = 0; k < Frag.Length; k++)
                    {
                        if (Frag[k] >= 0)
                            Main.npcChatText = "You have fragments, but none of them are from the same weapon.";
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

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<BronzeWand>());
            shop.item[nextSlot].shopCustomPrice = new int?(30);
            shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<AncientDirt>());
            shop.item[nextSlot].shopCustomPrice = new int?(1);
            shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<ElderWood>());
            shop.item[nextSlot].shopCustomPrice = new int?(1);
            shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<GathicStone>());
            shop.item[nextSlot].shopCustomPrice = new int?(1);
            shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<WeddingRing>());
            shop.item[nextSlot].shopCustomPrice = new int?(15);
            shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<LostSoul>());
            shop.item[nextSlot].shopCustomPrice = new int?(4);
            shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Violin>());
            shop.item[nextSlot].shopCustomPrice = new int?(20);
            shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            if (NPC.downedPlantBoss)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Ectoplasm);
                shop.item[nextSlot].shopCustomPrice = new int?(NPC.downedGolemBoss ? 6 : 10);
                shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
            }
            if (RedeBossDowned.keeperSaved)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GrimShard>());
                shop.item[nextSlot].shopCustomPrice = new int?(6);
                shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
                if (Main.expertMode)
                {
                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<HeartInsignia>());
                    shop.item[nextSlot].shopCustomPrice = new int?(30);
                    shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
                }
            }
            if (RedeWorld.deadRingerGiven)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<DeadRinger>());
                shop.item[nextSlot].shopCustomPrice = new int?(30);
                shop.item[nextSlot++].shopSpecialCurrency = Redemption.AntiqueDorulCurrencyId;
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

        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = TextureAssets.Item[ModContent.ItemType<BeardedHatchet>()].Value;
            itemSize = 38;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 38;
        }
    }
}