using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using System.Collections.Generic;
using Redemption.Walls;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Lore;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Materials.HM;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Redemption.WorldGeneration;
using Redemption.Tiles.Tiles;
using Redemption.Biomes;
using Redemption.BaseExtension;
using Redemption.Items.Donator.Lizzy;
using Terraria.GameContent.UI;
using Redemption.Items.Materials.PostML;

namespace Redemption.NPCs.Friendly
{
    public class KS3Sitting : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("King Slayer III");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.width = 56;
            NPC.height = 82;
            NPC.lifeMax = 999;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;
        }

        public override bool UsesPartyHat() => false;
        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => false;
        public override bool CanChat() => true;
        public override bool CheckActive()
        {
            return false;
        }
        public override void AI()
        {
            if (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus)
                NPC.active = false;

            NPC.direction = 1;
            if (NPC.AnyNPCs(ModContent.NPCType<KS3>()))
            {
                for (int i = 0; i < 15; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Frost, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                NPC.active = false;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile lizzy = Main.projectile[i];
                if (!lizzy.active || lizzy.type != ModContent.ProjectileType<LizzyPet>())
                    continue;

                if (lizzy.frame != 8)
                    continue;

                if (NPC.localAI[0]++ % 500 == 0)
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            AITimer++;
            if (AITimer >= 300 && AITimer <= 370)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 6 * frameHeight)
                        NPC.frame.Y = 0;
                }
                if (AITimer >= 370)
                {
                    NPC.frame.Y = 0;
                    AITimer = 0;
                }
                if (AITimer == 330)
                    NPC.Shoot(new Vector2(NPC.Center.X - 80, NPC.Center.Y - 50), ModContent.ProjectileType<KS3Sitting_Hologram>(), 0, Vector2.Zero, false, SoundID.Item1, Main.rand.Next(4));
            }
        }

        private static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = "Cycle Dialogue";

            switch (ChatNumber)
            {
                case 0:
                    button = "Why are you here?";
                    break;
                case 1:
                    button = "Crashed Spaceship?";
                    break;
                case 2:
                    button = "(Give Uranium)";
                    break;
                case 3:
                    button = "You aren't fighting me?";
                    break;
                case 4:
                    button = "Are you a human?";
                    break;
                case 5:
                    button = "Hall of Heroes?";
                    break;
                case 6:
                    button = "Abandoned Lab?";
                    break;
                case 7:
                    button = "Epidotra?";
                    break;
                case 8:
                    button = "Other world?";
                    break;
                case 9:
                    button = "Data Logs?";
                    break;
                case 10:
                    button = "Quest";
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            Player player = Main.LocalPlayer;
            Main.npcChatCornerItem = 0;
            if (firstButton)
            {
                if (ChatNumber == 2 && RedeWorld.slayerRep == 0)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int Urani = player.FindItem(ModContent.ItemType<Uranium>());
                    if (Urani >= 0)
                    {
                        player.inventory[Urani].stack--;
                        if (player.inventory[Urani].stack <= 0)
                            player.inventory[Urani] = new Item();

                        WeightedRandom<string> chat = new(Main.rand);
                        chat.Add("You really went out of your way to give me some uranium. Thanks I guess.");
                        chat.Add("I could've found some myself you know.");
                        chat.Add("Thanks... ?");
                        Main.npcChatText = chat;

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.SilverCoin, 20);
                        ChatNumber++;
                        RedeWorld.slayerRep++;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);

                        CombatText.NewText(NPC.getRect(), Color.LightCyan, "New Dialogue Available", true, false);

                        SoundEngine.PlaySound(SoundID.Chat);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<Uranium>();
                        Main.npcChatText = "You don't have any uranium, idiot.";
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep == 1)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int WiringKit = player.FindItem(ModContent.ItemType<SlayerWiringKit>());
                    if (WiringKit >= 0)
                    {
                        player.inventory[WiringKit].stack--;
                        if (player.inventory[WiringKit].stack <= 0)
                            player.inventory[WiringKit] = new Item();

                        Main.npcChatText = "You actually bothered to do it... Good job.";

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.GoldCoin, 4);
                        RedeWorld.slayerRep++;
                        CombatText.NewText(NPC.getRect(), Color.LightCyan, "New Dialogue Available", true, false);

                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipFix1", AssetRequestMode.ImmediateLoad).Value;

                        Point origin = RedeGen.slayerShipPoint.ToPoint();

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<SlayerWiringKit>();
                        Main.npcChatText = QuestChat();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep == 2)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int HullPlating = player.FindItem(ModContent.ItemType<SlayerHullPlating>());
                    if (HullPlating >= 0)
                    {
                        player.inventory[HullPlating].stack--;
                        if (player.inventory[HullPlating].stack <= 0)
                            player.inventory[HullPlating] = new Item();

                        Main.npcChatText = "How can you even carry that? Uh, thanks, I suppose.";

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.GoldCoin, 8);
                        RedeWorld.slayerRep++;
                        CombatText.NewText(NPC.getRect(), Color.LightCyan, "New Dialogue Available", true, false);

                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(0, 255, 255)] = ModContent.TileType<SlayerShipPanelTile>(),
                            [new Color(255, 0, 255)] = ModContent.TileType<ShipGlassTile>(),
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipFix2", AssetRequestMode.ImmediateLoad).Value;

                        Point origin = RedeGen.slayerShipPoint.ToPoint();

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<SlayerHullPlating>();
                        Main.npcChatText = QuestChat();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep == 3)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int ShipEngine = player.FindItem(ModContent.ItemType<SlayerShipEngine>());
                    if (ShipEngine >= 0)
                    {
                        player.inventory[ShipEngine].stack--;
                        if (player.inventory[ShipEngine].stack <= 0)
                            player.inventory[ShipEngine] = new Item();

                        Main.npcChatCornerItem = ModContent.ItemType<MemoryChip>();
                        Main.npcChatText = "Was helping me with all that really necessary for you? You don't gain anything from it. But thank you regardless. I'll be leaving soon, but I want you to have this. I have yet to figure out a use for it, but take it.";

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.GoldCoin, 12);
                        player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<MemoryChip>());
                        RedeWorld.slayerRep++;
                        RedeWorld.alignment += 2;

                        CombatText.NewText(NPC.getRect(), Color.LightCyan, "New Dialogue Available", true, false);
                        CombatText.NewText(player.getRect(), Color.Gold, "+2", true, false);

                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(0, 255, 255)] = ModContent.TileType<SlayerShipPanelTile>(),
                            [new Color(255, 0, 255)] = ModContent.TileType<ShipGlassTile>(),
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };

                        Dictionary<Color, int> colorToWall = new()
                        {
                            [new Color(0, 255, 0)] = ModContent.WallType<SlayerShipPanelWallTile>(),
                            [Color.Black] = -1
                        };

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipFix2", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipWallsFix", AssetRequestMode.ImmediateLoad).Value;

                        Point origin = RedeGen.slayerShipPoint.ToPoint();

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<SlayerShipEngine>();
                        Main.npcChatText = QuestChat();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep >= 4)
                {
                    Main.npcChatText = QuestChat();
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
                else if (ChatNumber == 9 && RedeWorld.slayerRep >= 4)
                    shop = true;
                else
                    Main.npcChatText = ChitChat();
            }
            else
            {
                ChatNumber++;
                if (ChatNumber > 10)
                    ChatNumber = 0;
                if (RedeWorld.slayerRep == 0 && ChatNumber > 2)
                    ChatNumber = 0;
                if (RedeWorld.slayerRep >= 1 && ChatNumber == 2)
                    ChatNumber++;
                if (!Main.LocalPlayer.Redemption().foundHall && ChatNumber == 5)
                    ChatNumber++;
            }
        }
        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog2>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog3>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog4>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog5>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog6>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog7>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog8>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog9>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog10>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog11>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog12>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog13>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog14>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog15>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog16>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog17>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog18>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog19>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog20>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog21>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog22>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog24>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog25>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog26>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Datalog27>());
        }

        public static string QuestChat()
        {
            return RedeWorld.slayerRep switch
            {
                1 => "You want to do something? Craft me a wiring kit, should be simple enough. I would do it, but I can't be bothered to get off this chair. Use that Cyber Fabricator the room under me.\n\nRequires: 20[i:" + ModContent.ItemType<Cyberscrap>() + "], 4[i:" + ModContent.ItemType<Plating>() + "], 1[i:" + ModContent.ItemType<Capacitator>() + "], 2[i:" + ModContent.ItemType<CarbonMyofibre>() + "], 8[i:20]/[i:703], 15[i:530]",
                2 => "You seem rather eager to help... Well, just get me some hull plating would you? Thanks.\n\nRequires: 50[i:" + ModContent.ItemType<Cyberscrap>() + "], 12[i:" + ModContent.ItemType<Plating>() + "], 6[i:" + ModContent.ItemType<CarbonMyofibre>() + "]",
                3 => "This is probably a bit too complicated for you, but craft me a AFTL engine. AFTL stands for 'Almost Faster Than Light' since, well, I don't know if going faster than light is even possible. I'm planning on leaving soon, so be quick.\n\nRequires: 70[i:" + ModContent.ItemType<Cyberscrap>() + "], 8[i:" + ModContent.ItemType<Plating>() + "], 6[i:" + ModContent.ItemType<Capacitator>() + "], 8[i:" + ModContent.ItemType<CarbonMyofibre>() + "], 20[i:" + ModContent.ItemType<Plutonium>() + "], 20[i:" + ModContent.ItemType<Uranium>() + "], 50[i:530]",
                4 => "You've done a lot for me, thank you, I guess. There isn't anything else that you can do now. But I appreciate the help you've given me.",
                _ => "",
            };
        }
        public static string ChitChat()
        {
            switch (ChatNumber)
            {
                case 0:
                    if (RedeWorld.slayerRep >= 1 && RedeWorld.slayerRep < 4)
                        return "Fixing this crashed ship, and just reflecting on our fight. Honestly I'm just doing it because I got nothing else to do.";
                    else if (RedeWorld.slayerRep >= 4)
                        return "The ship is fixed... I just can't be bothered to get up.";
                    else
                        return "I can ask you the same question. If you've come here to just chit-chat after our fight I'm not interested.";
                case 1:
                    if (RedeWorld.slayerRep == 1)
                        return "Yes. An android thought it'd be a good idea to 'borrow' it, and ended up yeeting it 20 feet under. The uranium you gave me should help.";
                    else if (RedeWorld.slayerRep >= 2 && RedeWorld.slayerRep < 4)
                        return "Yes. An android thought it'd be a good idea to 'borrow' it, and ended up yeeting it 20 feet under. The things you gave me should help.";
                    else if (RedeWorld.slayerRep >= 4)
                        return "Why are you still here? Ship's fixed now.";
                    else
                        return "What's the matter, you never seen a spaceship before? Some android thought it'd be a good idea to 'borrow' it, and ended up yeeting it 20 feet under. Unfortunately I've ran out of Uranium, but I can't be bothered to find it right now.";
                case 3:
                    if (RedeWorld.slayerRep == 3)
                        return "If you really want to fight, go ahead, use that cyber tech thing. I don't care.";
                    else if (RedeWorld.slayerRep >= 4)
                        return "After all you've done, I don't feel like fighting you. A duel, maybe.";
                    else
                        return Main.rand.NextBool(2) ? "Because unlike you, I actually have a life." : "Why would I want to fight now? I lost.";
                case 4:
                    if (RedeWorld.slayerRep == 2)
                        return "I was long ago, I became a robot when I realised it would be the only way to survive my world's end.";
                    else if (RedeWorld.slayerRep >= 3)
                        return "I wish I still was, mate. I can't eat or sleep in this robot body, but I still feel the pain of hunger and tiredness I can't satisfy. If it weren't for the AI I created for myself, I wouldn't be able to talk or move.";
                    else
                        return "I'm not some dude in a spacesuit, I'm a complete robot with a human mind. You may think that's cool, but it's really not. I seriously regret becoming one.";
                case 5:
                    if (RedeWorld.slayerRep == 2)
                        return "I did say I'm part of the Heroes, but I'm considering leaving. It's getting in the way of my... other projects.";
                    else if (RedeWorld.slayerRep == 3)
                        return "The members of the Heroes are all dingbats. The leader just travels around the world without aim, and I don't even know what the other 2 members are up to.";
                    else if (RedeWorld.slayerRep >= 4)
                        return Main.rand.NextBool(3) ? "... There's something strange about the Demigod's statue... It doesn't look like him. Did someone change it?" : "There are 4 members of the Heroes. The first is that demigod doofus, honestly he's a chill guy, I just hate how much stronger he is compared to me. The 2nd member is some moron who's supposedly invincible, not once have I seen him get hurt. 3rd is... Well she's probably the most normal out of all of us, but I don't know what she's up to now.";
                    else
                        return "You saw my statue there? Yeah, I'm part of the Heroes. But it's pretty boring, I'm always assigned to kill the weaklings, like the Keeper.";
                case 6:
                    if (RedeWorld.slayerRep >= 4)
                        return "I'm planning to check it out soon, it's located on the other world but I can easily fly there with the SoS.";
                    else
                        return "Haven't been there myself, but I'm looking into it. Could have plenty of supplies to fix this ship. It had a security system, but it malfunctioned, so I might take a look and maybe steal some supplies.";
                case 7:
                    if (RedeWorld.slayerRep >= 2 && RedeWorld.slayerRep < 4)
                        return "The world is called Epidotra, it's where the Heroes are from.";
                    else if (RedeWorld.slayerRep >= 4)
                        return "This tiny island appeared on this planet out of nowhere, I was going to check it out a year or so ago, but I remember seeing the demigod here so I didn't bother. The mainland has 6 domains, Anglon, Ithon, Gathuram, Nirin, Erellon, and Thamor. There's another domain which is it's own island called Swaylan, but that's disconnected from the rest of the world.";
                    else
                        return "What? You don't even know the name of the planet you're on? Maybe read a book for once in your life.";
                case 8:
                    if (RedeWorld.slayerRep >= 2 && RedeWorld.slayerRep < 4)
                        return "You see the planet on the right in that one hologram? That's what I've named Liden, a radioactive and desolate wasteland devoid of any life.";
                    else if (RedeWorld.slayerRep >= 4)
                        return "I'll be leaving soon to check it out. Scans show barely any life, just a frozen radioactive wasteland.";
                    else
                        return "The other world just suddenly appeared out of nowhere, I'm interested in it though. I've scanned the surface and it seems to be rather frozen, with so far no signs of human life. Reminds me of a planet I checked out while in space... In fact, reminds me of many planets.";
                case 9:
                    if (RedeWorld.slayerRep == 2)
                        return "I really just wrote those to keep my sanity. I don't want to tell you how boring travelling through space for a million years is.";
                    else if (RedeWorld.slayerRep == 3)
                        return "I wrote those every day I was in space, waiting for my world to be restored. Even though I'm back now, I don't feel satisfied.";
                    else
                        return "Can you not read through my data logs please.";
            }
            return "...";
        }
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);

            if (RedeBossDowned.downedSlayer && !Main.LocalPlayer.InModBiome<SlayerShipBiome>())
                chat.Add("Wait a second... This isn't my ship. Did you move my chair?");
            else
            {
                if (RedeWorld.slayerRep >= 2)
                {
                    chat.Add("Hey, I'm busy... Procrastinating.");
                    chat.Add("What is it?");
                    chat.Add("Come to help again or what?");
                    chat.Add("Leave me alone.");
                }
                else if (RedeWorld.slayerRep >= 4)
                {
                    chat.Add("Hey, what's up?");
                    chat.Add("Come by to talk or what?");
                    chat.Add("You've been a big help, thanks.");
                }
                else
                {
                    if (player.IsFullTBot())
                        chat.Add("Oh great, the robot is here... What do you want?");
                    else
                        chat.Add("Oh great, the flesh bag is here... What do you want?");

                    chat.Add("Did you really feel the need to break into my ship?");
                    chat.Add("Fight's over. I'm busy. Get lost.");
                    chat.Add("Could you just leave me alone.");
                }
                if (NPC.downedMoonlord)
                    chat.Add("I'll be leaving here soon, make it quick.");

                if (BasePlayer.HasHelmet(player, ModContent.ItemType<KingSlayerMask>(), true))
                    chat.Add("What have you got on your head? Are you trying to cosplay as me or something?");

                /*if (BasePlayer.HasHelmet(player, ModContent.ItemType<AndroidHead>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<PrototypeSilverHead>(), true))
                {
                    chat.Add("I'm not an idiot ya know, I know one of my own minions when I see one.");
                }*/
                if (player.wellFed && RedeWorld.slayerRep < 2)
                {
                    chat.Add("Look at you all well fed... Good for you.");
                }
                if (player.RedemptionPlayerBuff().ChickenForm)
                {
                    chat.Add("How did a chicken break into my ship?");
                }
            }
            return chat;
        }
    }
}