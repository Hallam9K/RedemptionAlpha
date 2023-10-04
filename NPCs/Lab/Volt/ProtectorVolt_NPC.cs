using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity.TBot;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
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
            button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.CycleD");
            switch (ChatNumber)
            {
                case 0:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.1");
                    break;
                case 1:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.2");
                    break;
                case 2:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.3");
                    break;
                case 3:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.4");
                    break;
                case 4:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.5");
                    break;
                case 5:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.6");
                    break;
                case 6:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.7");
                    break;
                case 7:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.8");
                    break;
                case 8:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.9");
                    break;
                case 9:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.10");
                    break;
                case 10:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.11");
                    break;
                case 11:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.12");
                    break;
                case 22:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.13");
                    break;
            }
            if (NextPage)
                button = Language.GetTextValue("Mods.Redemption.DialogueBox.TBot.NextPage");
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
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue1"));
                    break;
                case 1:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue2"));
                    break;
                case 2://
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue3"));
                    break;
                case 3:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue4"));
                    break;
                case 4:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue5"));
                    break;
                case 5:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue6"));
                    break;
                case 6:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue7"));
                    break;
                case 7:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue8"));
                    break;
                case 8:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue9"));
                    break;
                case 9:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue10"));
                    break;
                case 10:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue11"));
                    break;
                case 22:
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.CycleDialogue12"));
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
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Bot1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Bot2"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Bot3"));
            }
            else
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Normal1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Normal2"));
            }
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<VoltHead>(), true))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Volt1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Volt2"));
            }
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<AdamHead>(), true))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Adam1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Adam2"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Adam3"));
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
