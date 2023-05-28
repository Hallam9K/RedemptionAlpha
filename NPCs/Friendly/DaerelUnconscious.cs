using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly
{
    public class DaerelUnconscious : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Daerel");
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
            NPC.width = 52;
            NPC.height = 34;
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

        public int Level = -1;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (Level < 0)
                {
                    Level = 0;
                }

                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
                NPC.frame.X = NPC.frame.Width * Level;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
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
        }
        public override void AI()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Daerel>()))
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
            if (RedeWorld.daerelDownedTimer >= 43200)
            {
                RedeWorld.daerelDownedTimer = 0;

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);

                NPC.Transform(ModContent.NPCType<Daerel>());
                NPC.life = NPC.lifeMax;
                Main.NewText("Daerel the Wayfarer has woken up!", new Color(45, 114, 233));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
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
                    RedeWorld.daerelDownedTimer = 43200;

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
            return "You aren't holding a Revival Potion.";
        }
        public static string PotionChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            chat.Add("Did I just get knocked out? Thanks for waking me.");
            chat.Add("I'm up. Wide awake. What did you give me? Some sort of potion?");
            chat.Add("Was I asleep or unconscious? Most likely unconscious since, well, I wouldn't just go to sleep on the floor.");
            return chat;
        }
        public override string GetChat()
        {
            if (RedeWorld.daerelDownedTimer < 3600)
                return "Daerel has been unconsious for less than a minute.";
            else
                return "Daerel has been unconsious for " + RedeWorld.daerelDownedTimer / 3600 + " minute(s).";
        }
    }
}