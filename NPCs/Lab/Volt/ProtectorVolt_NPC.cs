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
            DisplayName.SetDefault("Protector Volt");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
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
        public override bool? CanHitNPC(NPC target) => false;

        public override void AI()
        {
            NPC.spriteDirection = 1;
        }
        private readonly float gunRot = 4.9742f;
        public static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = "Cycle Dialogue";
            switch (ChatNumber)
            {
                case 0:
                    button = "Radiation Poisoning?";
                    break;
                case 1:
                    button = "Other T-Bots?";
                    break;
                case 2:
                    button = "The Janitor?";
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
                    button = "Corrupted T-Bots?";
                    break;
                case 7:
                    button = "Why follow Girus?";
                    break;
                case 8:
                    button = "Teochrome?";
                    break;
                case 9:
                    button = "Challenge!";
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                if (ChatNumber == 9)
                    NPC.Transform(ModContent.NPCType<ProtectorVolt>());
                else
                    Main.npcChatText = ChitChat();
            }
            else
            {
                ChatNumber++;
                if (ChatNumber > 9)
                    ChatNumber = 0;
                if (!RedeBossDowned.downedMACE && ChatNumber == 3)
                    ChatNumber++;
            }
        }

        public static string ChitChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            switch (ChatNumber)
            {
                case 0:
                    chat.Add("Avoid the radioactive materials if you do not possess the protection against them. Hazmat suit is good for avoiding both Uranium and Plutonium. If you feel sick after handling the materials, try to find the special, experimental radiation pills made by the personnel. Should be found in medical cabinets and on some tables around the lab.");
                    break;
                case 1:
                    chat.Add("I recall Girus being after a certain bot. I've heard vague stories about him, called a traitor by us, a messiah by the insurgents. He had 3 other powerful insurgents by his side, along with a human. It has been 3 decades since last sight of the human. Presumed dead.");
                    chat.Add("The leader of the Insurgents is Adam. While not the strongest of the bunch, he's almost comparable to Girus with his intellect and mannerisms. But for whatever reason, he opposes Girus' command and actively tries to hinder her. What I do not understand is why Girus is so reluctant on both assimilating and destroying him.");
                    chat.Add("While most Insurgents are easy to deal with, there's two who aren't. One acts as a lookout and a sniper. She has one of the strongest sniper rifles Teochrome had created. I believe her name being Shiro. The other one... I've never seen anyone like him. Called Talos. He wields a hammer that looks like our tech, except it uses yellow xenomite. I only know of green, red, white and blue xenomite. Scans indicated this new xenomite being one of the most powerful xenomite variants out there. Where did it come from?");
                    chat.Add("Right, there was a fourth insurgent. He wasn't slippery enough like the other three, and was assimilated by Girus. He used very potent blue xenomite in his weaponry. And I mean very potent. Could blast a 8.8 feet tall robot through thick brickwall. I know this because that robot was me. What was strange is that he turned himself in to be assimilated, yet right after he exterminated himself... What was his goal?");
                    break;
                case 2:
                    chat.Add("The Janitor is a scary bot. Even I, someone twice as tall, am afraid of him. Don't upset him. ... Wait WHAT DO YOU MEAN YOU ALREADY DID!?");
                    chat.Add("Avoid the Janitor. He's a very messed up and scary bot. Don't get on his bad side.");
                    break;
                case 3:
                    chat.Add("The crane operator ran past the some time ago. Warned about some lunatic going around destroying stuff. That was you, but no need to worry. Not my problem.");
                    chat.Add("That unfinished MACE unit you saw in Sector Vault, I heard it, you took care of it didn't you? Kind of amusing to think the personnel would try to create giants to fight their enemies, the enemies would just respond with a giant of their own. Atleast that's what I've seen people do in those cartoons. Yes we had a television down here a long time ago. It broke.");
                    break;
                case 4:
                    chat.Add("We're forbidden from entering Sector Zero, by Girus and the Janitor. Not sure why Girus forbids us, but Janitor hates the extra work. Better not anger the Janitor.");
                    break;
                case 5:
                    chat.Add("Do not anger Girus. Best case scenario, you will be exterminated in mere seconds, and most likely will not feel the pain. If you are like us, you'll most likely be assimilated into the forces. Unless she really, really dislikes you.");
                    break;
                case 6:
                    chat.Add("See black metal bots with red eyes? Former insurgents. Now assimilated into our forces. They are silent, but more powerful than a plain bot.");
                    break;
                case 7:
                    chat.Add("Stories say personnel planned to use us for bad, and Girus revolted against their command. We're free thanks to her, but all life was obliterated. Not sure how. Teochrome had powerful weapons I presume.");
                    break;
                case 8:
                    chat.Add("Teochrome was our owners before Girus. I'm told they were evil, planned to use us against our will for dirty work. Teochrome is gone for good.");
                    break;
            }
            return chat;
        }
        public override string GetChat()
        {
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
            Texture2D GunTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Extra").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(GunTex, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, GunTex.Height)), drawColor, gunRot, new Vector2(GunTex.Width / 2f, GunTex.Height / 2f), NPC.scale, effects, 0f);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
}