using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.Items.Placeable.Containers;
using Redemption.Items.Placeable.Furniture.Lab;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Tools.PostML;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBot_NPC : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Lab/Janitor/JanitorBot_Cleaning";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
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
        public override bool CanHitNPC(NPC target) => false;

        public static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.CycleD");
            switch (ChatNumber)
            {
                case 0:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Janitor.1");
                    break;
                case 1:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Janitor.2");
                    break;
                case 2:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Janitor.3");
                    break;
                case 3:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Janitor.4");
                    break;
                case 4:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.8");
                    break;
                case 5:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.9");
                    break;
                case 6:
                    button = Language.GetTextValue("LegacyInterface.28");
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                if (ChatNumber == 6)
                    shopName = "Shop";
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
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add<LabHologramDevice>()
                .Add<OmegaTransmitter>(RedeConditions.DownedBehemoth)
                .Add<LabPlating>()
                .Add<HalogenLamp>()
                .Add<LabRail_L>()
                .Add<LabRail_Mid>()
                .Add<LabRail_R>()
                .Add<XeniumRefinery>(RedeConditions.DownedVolt)
                .Add<XeniumSmelter>(RedeConditions.DownedVolt)
                .Add<ElectricitySign>()
                .Add<SkullSign>()
                .Add<BiohazardSign>()
                .Add<RadioactiveSign>()
                .Add<TestTubes>()
                .Add<LabWallFan>()
                .Add<LabBackDoor2>()
                .Add<ServerCabinet>()
                .Add<HospitalBed>()
                .Add<LabIntercom>()
                .Add<LabComputer>()
                .Add<LabReceptionCouch>()
                .Add<LabCeilingMonitor>()
                .Add<LabReceptionDesk>()
                .Add<LabCeilingLamp>()
                .Add<LabToilet>()
                .Add<OperatorHead>(RedeConditions.IsTBotHead)
                .Add<VoltHead>(RedeConditions.IsTBotHead)
                .Add<JanitorOutfit>(RedeConditions.IsTBotHead)
                .Add<JanitorPants>(RedeConditions.IsTBotHead)
                .Add<AndroidArmour>(RedeConditions.IsTBotHead)
                .Add<AndroidPants>(RedeConditions.IsTBotHead)
                .Add<NoveltyMop>(RedeConditions.IsJanitor)
                .Add<BotHanger>(RedeConditions.DownedVolt)
                .Add<EmptyBotHanger>(RedeConditions.DownedVolt)
                .Add<Keycard>(RedeConditions.KeycardGiven)
                .Add<NanoPickaxe>(RedeConditions.DownedBlisterface);

            npcShop.Register();
        }

        public static string ChitChat()
        {
            string s1 = LabClean ? Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue1Clean") : Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue1");
            string s2 = LabClean ? Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue5Clean") : Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue5");
            return ChatNumber switch
            {
                0 => s1,
                1 => Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue2"),
                2 => Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue3"),
                3 => Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue4"),
                4 => s2,
                5 => Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.CycleDialogue6"),
                _ => "...",
            };
        }
        public static bool LabClean;
        public override string GetChat()
        {
            LabClean = true;
            for (int x = -190; x <= 110; x++)
            {
                for (int y = -110; y <= 120; y++)
                {
                    Point tileToNPC = NPC.Center.ToTileCoordinates();
                    int type = Main.tile[tileToNPC.X + x, tileToNPC.Y + y].TileType;
                    if (type == ModContent.TileType<LabPlatingTileUnsafe>())
                    {
                        LabClean = false;
                        break;
                    }
                }
            }
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new();
            if (BasePlayer.HasChestplate(player, ModContent.ItemType<JanitorOutfit>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<JanitorPants>(), true))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.ChatJanitor1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.ChatJanitor2"));
            }
            else
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.Chat1"));
                if (LabClean)
                    return Main.rand.NextBool() ? chat : Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.ChatClean");
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.Chat2"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.Chat3"));
            }
            return chat;
        }
    }
}