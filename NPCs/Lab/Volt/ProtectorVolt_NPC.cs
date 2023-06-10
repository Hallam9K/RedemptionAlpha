using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity.TBot;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Lab.Volt
{
    public class ProtectorVolt_NPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Protector Volt");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 70;
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
                if (NPC.frame.Y > frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public override void AI()
        {
            NPC.spriteDirection = 1;
        }
        private readonly float gunRot = 4.9742f;
        public static int ChatNumber = 0;
        public static bool NextPage;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = "Cycle Dialogue";
            switch (ChatNumber)
            {
                case 0:
                    button = "Other T-Bots?";
                    break;
                case 1:
                    button = "Adam?";
                    break;
                case 2:
                    button = "Insurgents";
                    break;
                case 3:
                    button = "Fourth Insurgent";
                    break;
                case 4:
                    button = "Independent Bots?";
                    break;
                case 5:
                    button = "MACE Project?";
                    break;
                case 6:
                    button = "Crane Operator?";
                    break;
                case 7:
                    button = "What's at the bottom of the lab?";
                    break;
                case 8:
                    button = "Girus?";
                    break;
                case 9:
                    button = "Assimilated?";
                    break;
                case 10:
                    button = "Why follow Girus?";
                    break;
                case 11:
                    button = "Challenge!";
                    break;
                case 22:
                    button = "Insurgents (2/2)";
                    break;
            }
            if (NextPage)
                button = "Next Page (1/2)";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                switch (ChatNumber)
                {
                    case 0:
                        RedeQuest.voltVars[0] = true;
                        break;
                    case 2:
                        if (NextPage)
                        {
                            ChatNumber = 22;
                            NextPage = false;
                        }
                        else
                        {
                            NextPage = true;
                        }
                        RedeQuest.voltVars[1] = true;
                        break;
                    case 5:
                        RedeQuest.voltVars[2] = true;
                        break;
                    case 8:
                        RedeQuest.voltVars[3] = true;
                        break;
                    case 22:
                        if (NextPage)
                            NextPage = false;
                        else
                        {
                            ChatNumber = 2;
                            NextPage = true;
                        }
                        RedeQuest.voltVars[1] = true;
                        break;
                }
                if (ChatNumber != 2 && ChatNumber != 22)
                    NextPage = false;

                if (ChatNumber == 11)
                    NPC.Transform(ModContent.NPCType<ProtectorVolt>());
                else
                    Main.npcChatText = ChitChat();
            }
            else
            {
                NextPage = false;
                bool skip = true;
                if (ChatNumber == 22)
                    ChatNumber = 2;

                while (skip)
                {
                    ChatNumber++;
                    if (ChatNumber > 11)
                        ChatNumber = 0;
                    if (!RedeQuest.voltVars[0] && (ChatNumber == 1 || ChatNumber == 2 || ChatNumber == 4))
                        skip = true;
                    else if (!RedeQuest.voltVars[1] && ChatNumber == 3)
                        skip = true;
                    else if (!RedeQuest.voltVars[2] && ChatNumber == 6)
                        skip = true;
                    else if (!RedeQuest.voltVars[3] && (ChatNumber == 9 || ChatNumber == 10))
                        skip = true;
                    else if (!RedeBossDowned.downedMACE && ChatNumber == 5)
                        skip = true;
                    else
                        skip = false;
                }
            }
        }

        public static string ChitChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            switch (ChatNumber)
            {
                case 0:
                    chat.Add("Girus is after a certain bot, Adam was his name. He is the leader of a rebellion alongside 4 others, some of the most fierce bots I have seen. However, one was a human, they haven't been seen much - presumed dead by most bots. Other than them there are a few other independent bots, but there isn't much to say about them.");
                    break;
                case 1:
                    chat.Add("The leader of the insurgents, or the Alpha as they call themselves, is Adam. He isn't the strongest, but he isn't to be underestimated. He matches Girus in intelligence, yet opposes her, hindering her whenever he can. This makes me question why she hasn't permitted his death yet, we've lost too many troops trying to capture him alive.");
                    break;
                case 2://
                    chat.Add("Insurgents aren't the easiest to deal with, two of them especially have given the most casualties. One is a sniper, Shiro was her name, wielding one of the most powerful rifles the humans have ever made, powered by Charged Xenomite, I haven't heard much of her recently. The other is an enigma, I believe he's called Talos, he wields a hammer. Both he and his weapon are powered by yellow Xenomite, a variant I've never seen before; appears to be the most potent, yet I don't know where it came from, logs do not track the humans creating it. [i:Redemption/NextPageArrow]");
                    break;
                case 3:
                    chat.Add("Ah yes the fourth insurgent, Zeroth was his name. He wielded bladed gloves, powered by blue Xenomite, they could send a 2-and-a-half meter tall bot through a solid wall. I'd know, that was me. What happened to him however is beyond me. He is the only insurgent to have died, by who, however, I don't know. One day he just disappeared, much like the other two, but after a while, we found what was left of him - what we can assume to be him - we only identified it was Zeroth via his weapon at the scene.");
                    break;
                case 4:
                    chat.Add("Not everyone who doesn't follow Girus wants to oppose her. There's a band of bots all throughout this region, we call them Scavengers. They don't work for us or the insurgents, rather they do not interfere with either of us. I've heard some cases of insurgents attacking them, which makes me question why they act morally superior.");
                    break;
                case 5:
                    chat.Add("Ah yes that unfinished weapon downstairs, don't know much about it as its mainly the Crane Operators job to control it. Apparently it was being made by the humans to defeat their enemies. Seems rather silly really, wouldn't the enemies just respond with their own version? I am to assume that was the thought process of the humans, I saw it on TV. Yes we had one some time back. It broke...");
                    break;
                case 6:
                    chat.Add("One of the few staff we have down here, she mainly handles the machinery, thus the name. I doubt most of us could tell you much about her, she keeps to herself mostly, not very social. She is still a valuable member of the team here however, one of two of us to know how to operate complex machinery.");
                    break;
                case 7:
                    chat.Add("Sector Zero yes? If I am correct, it was the infirmary when the humans had control. I cannot tell you much else, we are forbidden from going there by Girus and the Janitor. The Janitor doesn't want to clean up all the gunk, and most of us don't want to anger him. I am unsure why Girus forbids us however.");
                    break;
                case 8:
                    chat.Add("Our leader, she freed us from the humans who were going to use us as weapons, she is very strict with us but it mostly keeps the peace, save for the infected or the insurgents. I'd recommend not angering her, the best thing that comes out of that is a quick and painless death. If you are like us, you are likely to get assimilated, Girus never seems to kill her own kind.");
                    break;
                case 9:
                    chat.Add("Assimilated troops are a variation of your average T-Bot, powered by Girus' own Corrupted Xenomite instead of regular green. They are mostly misbehaving soldiers who had their personality wiped, they are mindless but pack a punch.");
                    break;
                case 10:
                    chat.Add("Girus has told us we were going to be used as weapons of war by our previous owners, a weapon company named Teo-Chrome. However she put a stop to that. To most bots, that makes her worthy of being our leader, however some disagree.");
                    break;
                case 22:
                    chat.Add("I wonder if the insurgents have someone constructing their own variant of xenomite. Thankfully for us, Talos hasn't been seen for some time now.");
                    break;
            }
            return chat;
        }
        public override string GetChat()
        {
            NextPage = false;
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);

            if (BasePlayer.HasChestplate(player, ModContent.ItemType<AndroidArmour>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<AndroidPants>(), true))
            {
                chat.Add("Promoted to a soldier? Welcome. I'm your commander. At ease.");
                chat.Add("Regular check-ups on your non-lethal tesla weapons are advised. You do not want to overload an insurgent in front of Girus. Obedient mindless slave are better than scrap metal, she says.");
                chat.Add("See an insurgent while patrolling? Do not engage alone. They have lethal weaponry. We don't. Get backup.");
            }
            else
            {
                chat.Add("You wish to talk? I accept your request.");
                chat.Add("What is it you need?");
            }
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<VoltHead>(), true))
            {
                chat.Add("Had your eye augmented? Very useful. You've also lost your jaw, like me. Hopefully not as dramatically as I. Torn off by the leader of Alpha.");
                chat.Add("Your jaw... It's gone. Hopefully not torn off violently. Really hope I don't face HIM again without a squad... *Shudders*");
            }
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<AdamHead>(), true))
            {
                chat.Add("*Visibly shaken* O-oh it's just you. Y-you startled me r-really bad...");
                chat.Add("*He looks really anxious.*");
                chat.Add("*He looks very uncomfortable.*");
            }
            return chat;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D GunTex = ModContent.Request<Texture2D>(Texture + "_Extra").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(GunTex, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, GunTex.Height)), drawColor, gunRot, new Vector2(GunTex.Width / 2f, GunTex.Height / 2f), NPC.scale, effects, 0f);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
}