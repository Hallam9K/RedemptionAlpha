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
using Redemption.NPCs.PreHM;
using Redemption.UI.ChatUI;
using Redemption.Items.Weapons.PreHM.Summon;

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
            DisplayName.SetDefault("Calavia");
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
            bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra").Value;
            voice = CustomSounds.Voice1 with { Pitch = 0.6f };
        }
        private Texture2D bubble;
        private SoundStyle voice;

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
                        string s1 = "Oi!";
                        string s2 = "I wouldn't take that spirit back to Arum if I were ya.";
                        string s3 = "And why would that be?";
                        string s4 = "Forgive my bluntness,[0.1] but the barons of Arum are nothing but rancorous demons.";
                        string s5 = "[@b]Excuse me?[0.5] I haven't seen a single droplet of malice from them,[0.1] and I've been workin' with them for years!";
                        string s6 = "As have I...[0.3][@c][@d] in a more direct sense.[0.5][@g] Just trust me lassie,[0.1] break free Kyretha's spirit.[0.5] Let her truly rest.";
                        string s7 = "To doubt the wisdom of a liberated spirit would be most imprudent.";
                        string s8 = ".^0.3^..^0.05^ [@e]Gorhal'on.[0.5][@f] Taborti.[0.6] I'll free it after I go through the portal.";
                        string s9 = "Is that a promise?";
                        string s10 = "Bi'oruen.";
                        string s11 = "Very well.";

                        chain.Add(new(spirit, s1, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(spirit, s2, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(NPC, s3, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(spirit, s4, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(NPC, s5, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(spirit, s6, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(spirit, s7, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(NPC, s8, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(spirit, s9, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(NPC, s10, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: bubble))
                             .Add(new(spirit, s11, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: bubble, endID: 1));
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
            player.currentShoppingSettings.HappinessReport = "";
            if (RedeQuest.calaviaVar > 10)
            {
                WeightedRandom<string> chat = new();
                chat.Add("You are the first khen I've met here - there's been floods of krhu around, so I prefer the change.");
                if (!player.RedemptionAbility().Spiritwalker)
                    chat.Add("That corpse by the portal is oddly soulful, I hope it doesn't come alive.");
                else if (!NPC.AnyNPCs(ModContent.NPCType<SpiritWalkerMan>()))
                    chat.Add("You got a strange ability from that soulful corpse by the portal? My worries of it becoming a danger must've been unfounded, though I yet sense a will therein.");
                if (BasePlayer.HasArmorSet(player, "Pure-Iron", true) || BasePlayer.HasArmorSet(player, "Pure-Iron", false))
                    chat.Add("Are you perhaps another warrior of the Iron Realm to stumble upon this place? Or did you rob that armour you wear?");
                else if (BasePlayer.HasArmorSet(player, "Common Guard", true) || BasePlayer.HasArmorSet(player, "Common Guard", false))
                    chat.Add("I am to assume you're an Anglic knight, based on your attire. Should've figured you wouldn't understand Gathic... Oh well.");
                else if (BasePlayer.HasArmorSet(player, "Dragon-Lead", true) || BasePlayer.HasArmorSet(player, "Dragon-Lead", false))
                    chat.Add("That armour you don is rather threatening. Where did it come from? Reminds me of the Thamor'in dragons that occassionally hinder the western countries.");

                return chat;
            }
            return "Again I must apologise for attacking ye, I asked for your purpose but clearly ye aren't Gathic. I should introduce myself, I am Calavia, a Chief-Warrior of the Iron Realm. It is such a pleasure to meet a khen here, I've seen naught but rotting khru ever since I got here.";
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
                        button = "Assistance?";
                        break;
                    case 4:
                        button = "You want to go back?";
                        button2 = "Why did you enter?";
                        break;
                    case 5:
                        button = "You want to go back?";
                        button2 = "";
                        break;
                    case 6:
                        button = "Just as clueless as you";
                        button2 = "Nothing at all";
                        break;
                    case 7:
                        button = "What will you do?";
                        button2 = "";
                        break;
                    case 8:
                        button = "Anything I can help with?";
                        button2 = "Good luck with that";
                        break;
                    case 9:
                        button = "What do you need?";
                        button2 = "I refuse";
                        break;
                    case 10:
                        button = "I will offer help";
                        button2 = "You're ready to leave";
                        break;
                    case 20:
                        button = "Farewell";
                        button2 = "Don't leave yet";
                        break;
                }
            }
            else
            {
                if (!RedeGen.cryoCrystalSpawn && ChatNumber == 0)
                    ChatNumber++;
                if (ChatNumber is 3 && (RedeQuest.calaviaVar < 12 || (RedeQuest.calaviaVar is 16 && !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive) || Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardCalavia>())))
                    ChatNumber++;
                button2 = "Cycle Options";
                switch (ChatNumber)
                {
                    case 0:
                        if (HasHelmet > 0 && HasShield)
                        {
                            button = "Feeling ready?";
                            break;
                        }
                        button = "Requirements";
                        if (!HasShield && Main.LocalPlayer.HasItem(ModContent.ItemType<PureIronAlloy>()))
                            button = "Offer 6 Pure-Iron Alloy";
                        if (HasHelmet == 0 && Main.LocalPlayer.HasItem(ModContent.ItemType<AntiquePureIronHelmet>()))
                            button = "Offer Antique Pure-Iron Helmet";
                        if (HasHelmet == 0 && Main.LocalPlayer.HasItem(ModContent.ItemType<PureIronHelmet>()))
                            button = "Offer Pure-Iron Helmet";
                        break;
                    case 1:
                        FurnaceNearMe();
                        button = "Gathic Cryo-Furnace?";
                        if (NearFurnace)
                            button = "Forge";
                        break;
                    case 2:
                        button = "About you?";
                        break;
                    case 3:
                        button = "Captive spirit?";
                        if (RedeQuest.calaviaVar is 13 or 14)
                            button = "Explain";
                        if (RedeQuest.calaviaVar is 16)
                            button = "Request Kyretha's Crux";
                        break;
                    case 4:
                        button = "[c/FF6600:You're ready to leave]";
                        break;
                }
            }
        }
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (RedeQuest.calaviaVar < 11 || RedeQuest.calaviaVar == 20)
            {
                switch (RedeQuest.calaviaVar)
                {
                    default:
                        if (firstButton)
                        {
                            Main.npcChatText = "Yeah I came from the portal, 'twas atop a snowy precipice. When I tried going back through however, it took me into what I can only assume to be Gathuram's catacombs. Back and forth I went, yet it was always the same. Here, catacombs, here, catacombs... It is infuriating! How these portals work!?";
                            RedeQuest.calaviaVar = 4;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 4:
                        if (firstButton)
                        {
                            Main.npcChatText = "Precisely! I have a family anticipating my return, yet I see no way back to where I came from. Do ye know anything at all about portals?";
                            RedeQuest.calaviaVar = 6;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = "Our squadron climbed up a mountain to retrieve the body of a famous warrior, and came upon the portal. We should've reported it to the Gatewatch and carried on our business, but my witless curiousity said otherwise and chose to take a looksie. \"In and out, nice and quick\" I thought. Needless to say, ye can see how smart that idea was.";
                            RedeQuest.calaviaVar = 5;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 5:
                        Main.npcChatText = "Precisely! I have a family anticipating my return, yet I see no way back to where I came from. Do you know anything at all about portals?";
                        RedeQuest.calaviaVar = 6;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 6:
                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                        Main.npcChatText = "How helpful. Not like I can blame ye, they're magic of a bygone age. Only the Gatewatch have knowledge of 'em. Speaking of which, they must've been here before. Portals don't just appear with a stone ring and foundation pre-'rected - 'tis the Gatewatch's job to build those. To stabalize 'em, or so I've heard.";
                        RedeQuest.calaviaVar = 7;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 7:
                        Main.npcChatText = "Well I can't just mope around forever, the only way forward is scramming through the catacombs and praying to Sariel I find a way to the surface. It'll be a massive gamble tho', the catacombs are like a subterranean domain in their own right!";
                        if (RedeGen.cryoCrystalSpawn)
                            RedeQuest.calaviaVar = 8;
                        else
                        {
                            Main.npcChatText += " I'll stick around for now, maybe if ye give me a [i:Redemption/GathicCryoFurnace]furnace I could offer ye some smithing.";
                            RedeQuest.calaviaVar = 11;
                        }
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 8:
                        if (firstButton)
                            Main.npcChatText = "How polite of ye to ask. After our little squabble my helm and shield are quite... thrashed. I have nobody to blame but myself, but anyway, if ye could give me some replacements and supplies to aid me, I will be most grateful.";
                        else
                            Main.npcChatText = "I'll need it... Actually, would ye be a dear and get me some equipment to aid me? My shield and helm took quite a blow. Not like I can blame ye, I did initiate the clash.";
                        RedeQuest.calaviaVar = 9;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    case 9:
                        if (firstButton)
                        {
                            Main.npcChatText = "A new [i:Redemption/PureIronHelmet]Pure-Iron Helmet, and some [i/s6:Redemption/PureIronAlloy]Pure-Iron Alloy for remaking my shield. That is all I ask of ye. If ye give me a [i:Redemption/GathicCryoFurnace]furnace I can offer some help back. I work as a blacksmith on the side.";
                            RedeQuest.calaviaVar = 11;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = "... So ye think I can survive the catacombs as I stand? If ye won't assist me, I suppose I got no choice but to give it a shot. I still have some of this place's potions left over, they've come in real handy. Guess I'll stick around in case ye change your mind.";
                            RedeQuest.calaviaVar = 10;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        break;
                    case 10:
                        if (firstButton)
                        {
                            Main.npcChatText = "Thank ye, stranger. I'll need a new [i:Redemption/PureIronHelmet]Pure-Iron Helmet, and some [i/s6:Redemption/PureIronAlloy]Pure-Iron Alloy for remaking my shield. That is all I ask of ye. If ye give me a [i:Redemption/GathicCryoFurnace]furnace I can offer some help back. I work as a blacksmith on the side.";
                            RedeQuest.calaviaVar = 11;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        else
                        {
                            Main.npcChatText = "... If ye insist. *sigh* This'll be a challenge, but I'll trust your judgement. I'll make it through to reunite with my squadron and finally return to my family, there is no better motivation than that. Farewell, stranger.";
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
                            Main.npcChatText = "Still something you need? Or do you not feel I'm ready?";
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
                                Main.npcChatText = "As ready as I can be. I still have a few potions from this place, I've never seen such concoctions before... They must've been created by a master alchemist.";
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
                                Main.npcChatText = "Umgor'ye. The khru are ruthless creatures, a helmet to protect my neck an' head is much appreciated.";
                                if (pureIronHelm < 0 && pureIronHelm2 >= 0)
                                    Main.npcChatText += " Oh, and it is an antique design too? The fur is a nice privilege.";
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

                                Main.npcChatText = "Umgor'ye. I feel much safer with a shield.";
                                SoundEngine.PlaySound(SoundID.Chat);
                                SoundEngine.PlaySound(SoundID.Grab, NPC.position);
                                HasShield = true;
                                NPC.netUpdate = true;
                                return;
                            }
                            Main.npcChatText = "A new [i:Redemption/PureIronHelmet]Pure-Iron Helmet, and some [i/s6:Redemption/PureIronAlloy]Pure-Iron Alloy for remaking my shield. That is all I ask of ye.";
                            if (HasHelmet > 0 && !HasShield)
                                Main.npcChatText += "\nYe have given me the helmet so far. I don't doubt it'll be enough, yet I'd feel much safer with a shield too.";
                            else if (HasHelmet == 0 && HasShield)
                                Main.npcChatText += "\nYe have given me the shield so far. I don't doubt it'll be enough, yet I'd feel much safer with my head covered too.";
                            break;
                        case 1:
                            FurnaceNearMe();
                            if (NearFurnace)
                            {
                                if (Main.LocalPlayer.HasItem(ModContent.ItemType<Mistfall>()))
                                    Main.npcChatText = "That spell tome ye hold is quite weak, would ye like it to become similar to my own?";
                                if (Main.LocalPlayer.HasItem(ModContent.ItemType<Zweihander>()))
                                    Main.npcChatText = "Ye carry a Zweihander? That is a famous blade of the Iron Realm, and so too was it the original form of the Blade of the Mountain - a sword of a mighty warrior who unfortunately fell to a great beast, I am its new holder. Would ye like me to make a replica of it?";
                                SoundEngine.PlaySound(SoundID.MenuOpen);
                                TradeUI.Visible = true;
                            }
                            else
                            {
                                Main.npcChatText = "Yes, place down a [i:Redemption/GathicCryoFurnace]Gathic Cryo-Furnace near me and I will offer to forge some equipment for ye.";
                                SoundEngine.PlaySound(SoundID.Chat);
                            }
                            break;
                        case 2:
                            Main.npcChatText = "Ta? I am a chief-warrior and blacksmith of the Iron Realm. Back in Khen Boldur I have a husband and two kids. I seldom have the time to see 'em, but staying far apart only makes the moments we're together all-the-more special, or am I just bein' too optimistic? It's regrettable, but my job leaves no room for indulgences. I lead a squadron of Arum that primarily scouts out unmarked lands. My men have probably given up on me by now. Typical.";
                            break;
                        case 3:
                            if (RedeQuest.calaviaVar is 16)
                            {
                                if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                                {
                                    Main.npcChatText = "You'll need to be in the spirit realm to do that.";
                                    break;
                                }
                                int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                                if (card >= 0)
                                {
                                    ChatNumber = 2;
                                    Main.LocalPlayer.inventory[card].stack--;
                                    if (Main.LocalPlayer.inventory[card].stack <= 0)
                                        Main.LocalPlayer.inventory[card] = new Item();

                                    Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardCalavia>());
                                    Main.npcChatText = "Kyretha's spirit feels lighter - less shrouded in negative emotion. Alas, that means what the spirit spoke of is true, the barons hide a darkness I have turned a blind eye to. Ya know what? If I ever get back to Arum, I'll look into it, all stealth-like. But anywho, I do believe she'll be willing to give ye her crux, she offers it freely.";
                                    Main.npcChatCornerItem = ModContent.ItemType<CruxCardCalavia>();
                                    SoundEngine.PlaySound(SoundID.Chat);
                                }
                                else
                                {
                                    Main.npcChatText = "Kyretha's spirit feels different now. I never realised the aura I felt was not of her power but of a strong negative emotion, it was foolish to assume the feeling was the norm for spirits of her splendour. I can comphrehend her emotions more clearer, and I do believe she's willing to give ye her crux, if ye have something to imbue.";
                                    Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                                }
                            }
                            else if (RedeQuest.calaviaVar is 13 or 14)
                            {
                                Main.npcChatText = "The spirit I carry is of the original wielder of the Blade of the Mountain, Kyretha, who was slain in an isolated range. She was part of a \"unique\" group who's souls were bound to another, and due to this I was tasked with retrieving her body and soul to send back to the Bastion of Arum - The barons deemed this of utmost priority. I recognise the taboo nature of ensnaring a spirit in glass, but I hope ye understand now.";
                                if (RedeQuest.calaviaVar < 14)
                                {
                                    RedeQuest.calaviaVar = 14;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.WorldData);
                                }
                            }
                            else
                            {
                                Main.npcChatText = "Hm? Oh, so ye noticed. Yeah, I carry a bauble containing a spirit. I didn't elaborate on my job because it's none of ya business. Alas, clarification is in order so ye don't get any wrong ideas.";
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
                                Main.npcChatText = "Thank you for the help, and sorry for the trouble. What lies ahead makes me uneasy, but I'll make it through to reunite with my squadron and finally return to my family, there is no better motivation than that. Farewell, stranger.";
                            }
                            else
                            {
                                Main.npcChatText = "... If ye insist. *sigh* This'll be a challenge, but I'll trust your judgement. I'll make it through  to reunite with my squadron and finally return to my family, there is no better motivation than that. Farewell, stranger.";
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