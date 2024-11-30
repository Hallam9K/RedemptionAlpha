using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.Items.Placeable.Furniture.Lab;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Tools.PostML;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Tiles.Tiles;
using Redemption.UI.Dialect;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBot_NPC : ModRedeNPC
    {
        public override string Texture => "Redemption/NPCs/Lab/Janitor/JanitorBot_Cleaning";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
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

            DialogueBoxStyle = LIDEN;
        }
        public override bool HasTalkButton() => true;
        public override bool HasLeftHangingButton(Player player) => RedeGlobalButton.talkActive;
        public override HangingButtonParams LeftHangingButton(Player player) => new(6, false, -2);

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
                .Add<JanitorEquipment>()
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
                    if (type == TileType<LabPlatingTileUnsafe>() || type == TileType<LabPlatingTileUnsafe2>())
                    {
                        LabClean = false;
                        break;
                    }
                }
            }
            Player player = Main.LocalPlayer;
            WeightedRandom<string> chat = new();
            if (BasePlayer.HasChestplate(player, ItemType<JanitorOutfit>(), true) && BasePlayer.HasLeggings(player, ItemType<JanitorPants>(), true))
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
    public class WhatsUpButton_Janitor : TalkButtonBase
    {
        protected override int YOffset => 0;
        protected override bool LeftSide => true;
        protected override string DialogueType => "Janitor.1";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<JanitorBot_NPC>();
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = JanitorBot_NPC.LabClean ? Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.1Clean") : Language.GetTextValue("Mods.Redemption.Dialogue." + DialogueType);
        }
    }
    public class OtherTBotsButton_Janitor : TalkButtonBase
    {
        protected override int YOffset => 1;
        protected override bool LeftSide => true;
        protected override string DialogueType => "Janitor.2";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<JanitorBot_NPC>();
    }
    public class VoltButton_Janitor : TalkButtonBase
    {
        protected override int YOffset => 2;
        protected override bool LeftSide => true;
        protected override string DialogueType => "Janitor.3";
        protected override bool VisibleRequirement => RedeBossDowned.downedVolt;
        protected override int NPCType => NPCType<JanitorBot_NPC>();
    }
    public class MACEButton_Janitor : TalkButtonBase
    {
        protected override int YOffset => 3;
        protected override bool LeftSide => true;
        protected override string DialogueType => "Janitor.4";
        protected override bool VisibleRequirement => RedeBossDowned.downedMACE;
        protected override int NPCType => NPCType<JanitorBot_NPC>();
    }
    public class BottomOfLabButton_Janitor : TalkButtonBase
    {
        protected override int YOffset => 4;
        protected override bool LeftSide => true;
        protected override string DialogueType => "Janitor.5";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<JanitorBot_NPC>();
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.19");
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = JanitorBot_NPC.LabClean ? Language.GetTextValue("Mods.Redemption.Dialogue.Janitor.5Clean") : Language.GetTextValue("Mods.Redemption.Dialogue." + DialogueType);
        }
    }
    public class GirusButton_Janitor : TalkButtonBase
    {
        protected override int YOffset => 5;
        protected override bool LeftSide => true;
        protected override string DialogueType => "Janitor.6";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<JanitorBot_NPC>();
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.10");
    }
}