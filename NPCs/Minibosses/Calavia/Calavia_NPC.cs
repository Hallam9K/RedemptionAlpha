using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI;
using Redemption.Items.Armor.PreHM.PureIron;
using Redemption.Items.Armor.Single;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Furniture.Misc;
using Terraria.Audio;
using Terraria.ModLoader.IO;
using Redemption.UI;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Dusts;
using Redemption.WorldGeneration;
using Terraria.Utilities;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.NPCs.Friendly;
using Redemption.UI.ChatUI;
using Redemption.Items.Weapons.PreHM.Summon;
using Terraria.Localization;
using Redemption.Textures;

namespace Redemption.NPCs.Minibosses.Calavia
{
    [AutoloadHead]
    public class Calavia_NPC : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/Calavia/Calavia";
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Calavia");
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[Type] = 20;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.friendly = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.npcSlots = 0;
            NPC.townNPC = true;
            TownNPCStayingHomeless = true;
            NPC.dontTakeDamage = true;
        }
        private static Texture2D Bubble => CommonTextures.TextBubble_Epidotra.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice1 with { Pitch = 0.6f };
        private bool HasShield;
        private int HasHelmet;
        public override void LoadData(TagCompound tag)
        {
            HasShield = tag.GetBool("HasShield");
            HasHelmet = tag.GetInt("HasHelmet");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["HasShield"] = HasShield;
            tag["HasHelmet"] = HasHelmet;
        }
        public override bool UsesPartyHat() => false;
        public override bool CanChat() => RedeQuest.calaviaVar < 21 && RedeQuest.calaviaVar != 15;
        public override bool CheckActive() => false;
        readonly DialogueChain chain = new();
        public override void AI()
        {
            if (++NPC.breath <= 0)
                NPC.breath = 9000;
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            int spiritWhoAmI = NPC.FindFirstNPC(ModContent.NPCType<SpiritWalkerMan>());
            if (RedeQuest.calaviaVar == 14 && Main.LocalPlayer.talkNPC == -1)
            {
                if (spiritWhoAmI > -1)
                {
                    RedeQuest.calaviaVar = 15;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }
            else if (RedeQuest.calaviaVar is 15)
            {
                if (spiritWhoAmI > -1)
                {
                    NPC spirit = Main.npc[spiritWhoAmI];
                    SoundStyle spiritVoice = CustomSounds.Voice2;

                    if (AITimer++ == 0)
                    {
                        spirit.LookAtEntity(NPC);
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(spirit), 60);
                    }
                    if (AITimer == 60)
                        NPC.LookAtEntity(spirit);

                    if (AITimer == 60 && chain.Dialogue.Count == 0)
                    {
                        string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.1");
                        string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.2");
                        string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.3");
                        string s4 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.4");
                        string s5 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.5");
                        string s6 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.6");
                        string s7 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.7");
                        string s8 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.8");
                        string s9 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.9");
                        string s10 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.10");
                        string s11 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.11");

                        chain.Add(new(spirit, s1, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s2, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, s3, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s4, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, s5, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s6, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s7, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, s8, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s9, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, s10, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s11, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble, endID: 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 10000)
                    {
                        AITimer = 0;
                        RedeQuest.calaviaVar = 16;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                    if (AITimer >= 60 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                    {
                        Vector2 focus = RedeHelper.CenterPoint(NPC.Center, spirit.Center);
                        player.RedemptionScreen().ScreenFocusPosition = Vector2.Lerp(focus, player.Center, player.DistanceSQ(focus) / (1200 * 1200));
                        player.RedemptionScreen().lockScreen = true;
                    }
                }
                else
                {
                    ChatUI.Remove(chain);
                    chain.Dialogue.Clear();
                    ChatUI.Visible = false;
                    AITimer = 0;
                    RedeQuest.calaviaVar = 13;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
                if (player.DistanceSQ(NPC.Center) > 2000 * 2000)
                {
                    ChatUI.Remove(chain);
                    chain.Dialogue.Clear();
                    ChatUI.Visible = false;
                    AITimer = 0;
                    RedeQuest.calaviaVar = 13;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }
            else if (RedeQuest.calaviaVar == 20 && Main.LocalPlayer.talkNPC == -1)
            {
                if (HasShield && HasHelmet > 0)
                    RedeQuest.calaviaVar = 22;
                else
                    RedeQuest.calaviaVar = 21;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            else if (RedeQuest.calaviaVar > 20)
            {
                Vector2 gathicPortalPos = new(((RedeGen.gathicPortalVector.X + 51) * 16) - 8, (RedeGen.gathicPortalVector.Y + 18) * 16);
                if (AITimer++ == 60)
                {
                    NPC.velocity.Y = -7;
                    NPC.velocity.X = 2;
                }
                if (AITimer >= 120)
                {
                    NPC.noTileCollide = true;
                    NPC.Move(gathicPortalPos, 20, 30);
                    NPC.alpha += 5;
                }
                if (AITimer >= 60)
                {
                    NPC.rotation += 0.1f;
                    NPC.velocity.X *= 0.99f;

                    if (NPC.DistanceSQ(gathicPortalPos) < 60 * 60)
                        NPC.alpha += 10;
                    if (NPC.alpha >= 255)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, NPC.position);
                        for (int i = 0; i < 30; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 1, 0, 0, default, 0.5f);
                            Main.dust[dust].noGravity = true;
                            Color dustColor = new(Color.LightBlue.R, Color.LightBlue.G, Color.LightBlue.B) { A = 0 };
                            Main.dust[dust].color = dustColor;
                            Main.dust[dust].velocity *= 3f;
                        }
                        NPC.active = false;
                    }
                }
                return;
            }
            if (NPC.Sight(player, 600, false, true) && RedeQuest.calaviaVar != 15)
                NPC.LookAtEntity(player);
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            int spiritWhoAmI = NPC.FindFirstNPC(ModContent.NPCType<SpiritWalkerMan>());
            if (spiritWhoAmI == -1)
                return;
            NPC spirit = Main.npc[spiritWhoAmI];
            switch (signature)
            {
                case "b":
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                    break;
                case "c":
                    EmoteBubble.NewBubble(2, new WorldUIAnchor(spirit), 120);
                    break;
                case "d":
                    spirit.spriteDirection *= -1;
                    break;
                case "g":
                    spirit.spriteDirection *= -1;
                    break;
                case "e":
                    NPC.spriteDirection *= -1;
                    break;
                case "f":
                    NPC.spriteDirection *= -1;
                    break;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 10000;
        }
        public override string GetChat()
        {
            Player player = Main.LocalPlayer;
            if (RedeQuest.calaviaVar > 10)
            {
                WeightedRandom<string> chat = new();
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat2"));
                if (!player.RedemptionAbility().Spiritwalker)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat3"));
                else if (!NPC.AnyNPCs(ModContent.NPCType<SpiritWalkerMan>()))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat4"));
                if (BasePlayer.HasArmorSet(player, "Pure-Iron", true) || BasePlayer.HasArmorSet(player, "Pure-Iron", false))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat5"));
                else if (BasePlayer.HasArmorSet(player, "Common Guard", true) || BasePlayer.HasArmorSet(player, "Common Guard", false))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat6"));
                else if (BasePlayer.HasArmorSet(player, "Dragon-Lead", true) || BasePlayer.HasArmorSet(player, "Dragon-Lead", false))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat7"));

                return chat;
            }
            return Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat1");
        }
        private bool NearFurnace;
        private void FurnaceNearMe()
        {
            NearFurnace = false;
            for (int x = -6; x <= 6; x++)
            {
                for (int y = -6; y <= 6; y++)
                {
                    Point npcPos = NPC.Center.ToTileCoordinates();
                    Tile tile = Framing.GetTileSafely(npcPos.X + x, npcPos.Y + y);
                    if (tile.TileType == ModContent.TileType<GathicCryoFurnaceTile>())
                    {
                        NearFurnace = true;
                        break;
                    }
                }
            }
        }
        private static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            if (RedeQuest.calaviaVar < 11 || RedeQuest.calaviaVar == 20)
            {
                switch (RedeQuest.calaviaVar)
                {
                    default:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.1");
                        break;
                    case 4:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.2");
                        button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.2Alt");
                        break;
                    case 5:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.3");
                        button2 = "";
                        break;
                    case 6:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.4");
                        button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.4Alt");
                        break;
                    case 7:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.5");
                        button2 = "";
                        break;
                    case 8:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.6");
                        button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.6Alt");
                        break;
                    case 9:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.7");
                        button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.7Alt");
                        break;
                    case 10:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.8");
                        button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.8Alt");
                        break;
                    case 20:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.9");
                        button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.9Alt");
                        break;
                }
            }
            else
            {
                if (!RedeGen.cryoCrystalSpawn && ChatNumber == 0)
                    ChatNumber++;
                if (ChatNumber is 3 && (RedeQuest.calaviaVar < 12 || (RedeQuest.calaviaVar is 16 && !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive) || Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardCalavia>())))
                    ChatNumber++;
                button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Cycle");
                switch (ChatNumber)
                {
                    case 0:
                        if (HasHelmet > 0 && HasShield)
                        {
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Ready");
                            break;
                        }
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Require");
                        if (!HasShield && Main.LocalPlayer.HasItem(ModContent.ItemType<PureIronAlloy>()))
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Alloy");
                        if (HasHelmet == 0 && Main.LocalPlayer.HasItem(ModContent.ItemType<AntiquePureIronHelmet>()))
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.AntiqueHelmet");
                        if (HasHelmet == 0 && Main.LocalPlayer.HasItem(ModContent.ItemType<PureIronHelmet>()))
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Helmet");
                        break;
                    case 1:
                        FurnaceNearMe();
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Furnace");
                        if (NearFurnace)
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Forge");
                        break;
                    case 2:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.10");
                        break;
                    case 3:
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.11");
                        if (RedeQuest.calaviaVar is 13 or 14)
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.12");
                        if (RedeQuest.calaviaVar is 16)
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Crux");
                        break;
                    case 4:
                        button = "[c/FF6600:" + Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Leave") + "]";
                        break;
                }
            }
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (RedeQuest.calaviaVar < 11 || RedeQuest.calaviaVar == 20)
            {
                switch (RedeQuest.calaviaVar)
                {
                    default:
                        if (firstButton)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine1");
                            RedeQuest.calaviaVar = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 4:
                        if (firstButton)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine2");
                            RedeQuest.calaviaVar = 6;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine3");
                            RedeQuest.calaviaVar = 5;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 5:
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine2");
                        RedeQuest.calaviaVar = 6;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 6:
                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine4");
                        RedeQuest.calaviaVar = 7;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 7:
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine5");
                        if (RedeGen.cryoCrystalSpawn)
                            Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine5B");
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (RedeGen.cryoCrystalSpawn)
                                RedeQuest.calaviaVar = 8;
                            else
                                RedeQuest.calaviaVar = 11;
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 8:
                        if (firstButton)
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine6");
                        else
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine7");
                        RedeQuest.calaviaVar = 9;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 9:
                        if (firstButton)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine8");
                            RedeQuest.calaviaVar = 11;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine9");
                            RedeQuest.calaviaVar = 10;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 10:
                        if (firstButton)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine10");
                            RedeQuest.calaviaVar = 11;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine11");
                            RedeQuest.calaviaVar = 20;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 20:
                        if (firstButton)
                        {
                            Main.npcChatText = "";
                            Main.LocalPlayer.releaseInventory = false;
                            if (HasShield && HasHelmet > 0)
                                RedeQuest.calaviaVar = 22;
                            else
                                RedeQuest.calaviaVar = 21;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine12");
                            RedeQuest.calaviaVar = 11;
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
                            if (HasHelmet > 0 && HasShield)
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ReadyLine");
                                break;
                            }
                            int pureIronAlloy = Main.LocalPlayer.FindItem(ModContent.ItemType<PureIronAlloy>());
                            int pureIronHelm = Main.LocalPlayer.FindItem(ModContent.ItemType<PureIronHelmet>());
                            int pureIronHelm2 = Main.LocalPlayer.FindItem(ModContent.ItemType<AntiquePureIronHelmet>());
                            if (HasHelmet == 0 && (pureIronHelm >= 0 || pureIronHelm2 >= 0))
                            {
                                if (pureIronHelm >= 0)
                                {
                                    Main.LocalPlayer.inventory[pureIronHelm].stack--;
                                    if (Main.LocalPlayer.inventory[pureIronHelm].stack <= 0)
                                        Main.LocalPlayer.inventory[pureIronHelm] = new Item();
                                    HasHelmet = 1;
                                }
                                else
                                {
                                    Main.LocalPlayer.inventory[pureIronHelm2].stack--;
                                    if (Main.LocalPlayer.inventory[pureIronHelm2].stack <= 0)
                                        Main.LocalPlayer.inventory[pureIronHelm2] = new Item();
                                    HasHelmet = 2;
                                }
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.HelmetLine");
                                if (pureIronHelm < 0 && pureIronHelm2 >= 0)
                                    Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.HelmetLineB");
                                SoundEngine.PlaySound(SoundID.Chat);
                                SoundEngine.PlaySound(SoundID.Grab, NPC.position);
                                NPC.netUpdate = true;
                                return;
                            }
                            if (!HasShield && pureIronAlloy >= 0 && Main.LocalPlayer.inventory[pureIronAlloy].stack >= 6)
                            {
                                Main.LocalPlayer.inventory[pureIronAlloy].stack -= 6;
                                if (Main.LocalPlayer.inventory[pureIronAlloy].stack <= 0)
                                    Main.LocalPlayer.inventory[pureIronAlloy] = new Item();

                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ShieldLine");
                                SoundEngine.PlaySound(SoundID.Chat);
                                SoundEngine.PlaySound(SoundID.Grab, NPC.position);
                                HasShield = true;
                                NPC.netUpdate = true;
                                return;
                            }
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.RequirementsLine");
                            if (HasHelmet > 0 && !HasShield)
                                Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.RequirementsLineB");
                            else if (HasHelmet == 0 && HasShield)
                                Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.RequirementsLineC");
                            break;
                        case 1:
                            FurnaceNearMe();
                            if (NearFurnace)
                            {
                                if (Main.LocalPlayer.HasItem(ModContent.ItemType<Mistfall>()))
                                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.MistfallLine");
                                if (Main.LocalPlayer.HasItem(ModContent.ItemType<Zweihander>()))
                                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ZweihanderLine");
                                SoundEngine.PlaySound(SoundID.MenuOpen);
                                TradeUI.Visible = true;
                            }
                            else
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.FurnaceLine");
                                SoundEngine.PlaySound(SoundID.Chat);
                            }
                            break;
                        case 2:
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.AboutMeLine");
                            break;
                        case 3:
                            if (RedeQuest.calaviaVar is 16)
                            {
                                int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                                if (card >= 0)
                                {
                                    ChatNumber = 2;
                                    Main.LocalPlayer.inventory[card].stack--;
                                    if (Main.LocalPlayer.inventory[card].stack <= 0)
                                        Main.LocalPlayer.inventory[card] = new Item();

                                    Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardCalavia>());
                                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.CruxDialogue");
                                    Main.npcChatCornerItem = ModContent.ItemType<CruxCardCalavia>();
                                    SoundEngine.PlaySound(SoundID.Chat);
                                }
                                else
                                {
                                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.NoCruxDialogue");
                                    Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                                }
                            }
                            else if (RedeQuest.calaviaVar is 13 or 14)
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ConfrontLine2");
                                if (RedeQuest.calaviaVar < 14)
                                {
                                    RedeQuest.calaviaVar = 14;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.WorldData);
                                }
                            }
                            else
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ConfrontLine1");
                                if (RedeQuest.calaviaVar < 13)
                                {
                                    RedeQuest.calaviaVar = 13;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.WorldData);
                                }
                            }
                            break;
                        case 4:
                            if (HasHelmet > 0 && HasShield)
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.LeaveLine");
                            }
                            else
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine11");
                            }
                            RedeQuest.calaviaVar = 20;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
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
        public override bool CanGoToStatue(bool toKingStatue) => false;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 19 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
            else
                NPC.frame.Y = 5 * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture + "2").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(Calavia.CloakTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.LegsTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HasHelmet > 0)
            {
                var effects2 = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Texture2D helmet = ModContent.Request<Texture2D>("Redemption/Items/Armor/PreHM/PureIron/PureIronHelmet_Head").Value;
                if (HasHelmet == 2)
                    helmet = ModContent.Request<Texture2D>("Redemption/Items/Armor/Single/AntiquePureIronHelmet_Head").Value;
                spriteBatch.Draw(helmet, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects2, 0);
            }
            if (HasShield)
                spriteBatch.Draw(Calavia.ShieldTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.ArmTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}
