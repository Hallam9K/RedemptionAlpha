using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.Items.Placeable.Containers;
using Redemption.Items.Placeable.Furniture.Lab;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Tools.PostML;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBot_NPC : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Lab/Janitor/JanitorBot_Cleaning";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 44;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0;
        }

        public override bool UsesPartyHat() => false;
        public override bool CanChat() => true;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ >= 20)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;

        public static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = "Cycle Dialogue";
            switch (ChatNumber)
            {
                case 0:
                    button = "What's up?";
                    break;
                case 1:
                    button = "Other T-Bots?";
                    break;
                case 2:
                    button = "Protector Volt?";
                    break;
                case 3:
                    button = "MACE Project?";
                    break;
                case 4:
                    button = "What's at the bottom of the lab?";
                    break;
                case 5:
                    button = "Girus?";
                    break;
                case 6:
                    button = "Shop";
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                if (ChatNumber == 6)
                    shop = true;
                else
                    Main.npcChatText = ChitChat();
            }
            else
            {
                ChatNumber++;
                if (ChatNumber > 6)
                    ChatNumber = 0;
                if (!RedeBossDowned.downedVolt && ChatNumber == 2)
                    ChatNumber++;
                if (!RedeBossDowned.downedMACE && ChatNumber == 3)
                    ChatNumber++;
            }
        }
        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            Player player = Main.player[Main.myPlayer];
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabHologramDevice>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabPlating>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabPlatingWall>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabPlatform>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabRail_L>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabRail_Mid>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabRail_R>());
            if (RedeBossDowned.downedVolt)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<XeniumRefinery>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<XeniumSmelter>());
            }
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LargeVent>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Vent>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SmallVent>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ElectricitySign>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkullSign>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BiohazardSign>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<RadioactiveSign>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabWorkbench>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<TestTubes>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabTable>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabWallFan>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabDoor>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabBackDoor2>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabChest>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabChair>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ServerCabinet>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HospitalBed>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabIntercom>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabComputer>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabReceptionCouch>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabCeilingMonitor>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabReceptionDesk>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabCeilingLamp>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LabToilet>());
            if (player.IsTBotHead())
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<OperatorHead>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<VoltHead>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<JanitorOutfit>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<JanitorPants>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AndroidArmour>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AndroidPants>());
            }
            if (BasePlayer.HasChestplate(player, ModContent.ItemType<JanitorOutfit>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<JanitorPants>(), true))
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<NoveltyMop>());

            if (RedeBossDowned.downedVolt)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BotHanger>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EmptyBotHanger>());
            }
            if (RedeBossDowned.downedBlisterface)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<NanoPickaxe>());
        }

        public static string ChitChat()
        {
            return ChatNumber switch
            {
                0 => "You've probably seen the personnel who are basically walking corpses, ye? Our security usually deals with them, but it's not enough. They crawl out of overrun rooms like goddamn cockroaches! And I am the one who has to clean all the mess left by them and security.",
                1 => "The other bots have learned to fear me. Good for me, I can actually focus on my job. Except now there is a certain someone bugging me when I'm trying to do my job. Hmm, I wonder who it could be... Now bugger off!",
                2 => "The big bot at Sector Omicron is a cool guy, if you ask me. Probably the only bot I don't feel annoyed by. Now if you excuse me, you brought in some of that slime with ya and I have to clean that up!",
                3 => "There's this one copper-haired bot that I can recall, who works as the crane operator in the Mech Storage room. I don't know that much about her, but her design is a little peculiar. Not that she's more feminine, but the hair... Why tho'?",
                4 => "Don't you dare go into Sector Zero, that place is absolutely filled with the impossible-to-remove black slime! And I don't want ye to bring that stuff here for me to try to pry off the steel plating of the lab! Takes ages.",
                5 => "Respect Girus and she will learn to respect you. Quite sure I'll get a change of a job after 2 centuries of cleaning the lab after the... Incident. Don't ask me 'bout it, I was activated AFTER it happened.",
                _ => "...",
            };
        }
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new();
            if (BasePlayer.HasChestplate(player, ModContent.ItemType<JanitorOutfit>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<JanitorPants>(), true))
            {
                chat.Add("So you've been assigned to janitor duty, eh? Here's a task, go to the reactors and remove that dense lava stuff from under the reactor. My Nano-chisel won't even leave a mark on it!");
                chat.Add("About time she got me an assistant to help clean this mess of a place. Remember to check under the tables for dust.");
            }
            else
            {
                chat.Add("Bugger off, you're bringing a whole bunch of dust in here!");
                chat.Add("Oh it's you again. You better not interrupt my cleaning.");
                chat.Add("Make it quick, I've got to resume mopping the floor.");
            }
            return chat;
        }
    }
}