using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly
{
    public class TBotUnconscious : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Adam");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 56;
            NPC.height = 36;
            NPC.aiStyle = -1;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.dontTakeDamage = true;
            NPC.npcSlots = 0;
        }
        public override bool NeedSaving() => true;
        public override bool UsesPartyHat() => false;
        public override bool CheckActive() => false;
        public override bool CanChat() => true;

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                }
            }
        }
        public override void AI()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<TBot>()))
                NPC.active = false;
            NPC.dontTakeDamage = true;
            NPC.velocity.X = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }
            if (RedeWorld.tbotDownedTimer >= 43200)
            {
                RedeWorld.tbotDownedTimer = 0;

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);

                NPC.Transform(ModContent.NPCType<TBot>());
                NPC.life = NPC.lifeMax;
                Main.NewText("Adam the Friendly T-Bot has woken up!", new Color(45, 114, 233));
            }
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Use Revival Potion";
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.player[Main.myPlayer];
            if (firstButton)
            {
                int potion = player.FindItem(ModContent.ItemType<RevivalPotion>());
                if (potion >= 0)
                {
                    player.inventory[potion].stack--;
                    if (player.inventory[potion].stack <= 0)
                        player.inventory[potion] = new Item();

                    SoundEngine.PlaySound(SoundID.Item3, NPC.position);
                    RedeWorld.tbotDownedTimer = 43200;

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);

                    Main.npcChatText = PotionChat();
                }
                else
                {
                    Main.npcChatCornerItem = ModContent.ItemType<RevivalPotion>();
                    Main.npcChatText = NoPotionChat();
                }
            }
        }
        public static string NoPotionChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            chat.Add("You aren't holding a Revival Potion.");
            return chat;
        }
        public static string PotionChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            chat.Add("Rebooting systems...");
            chat.Add("Running self-repairs...");
            chat.Add("I'm not sure how I drank that, since I'm a robot. I probably shouldn't question it.");
            return chat;
        }
        public override string GetChat()
        {
            if (RedeWorld.tbotDownedTimer < 3600)
                return "Adam has been unconsious for less than a minute.";
            else
                return "Adam has been unconsious for " + RedeWorld.tbotDownedTimer / 3600 + " minute(s).";
        }
    }
}