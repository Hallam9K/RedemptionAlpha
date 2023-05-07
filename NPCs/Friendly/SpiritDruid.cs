using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Items.Materials.PreHM;
using Terraria.Audio;

namespace Redemption.NPCs.Friendly
{
    public class SpiritDruid : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Druid");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 34;
            NPC.height = 52;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;

        public bool floatTimer;
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            NPC.LookAtEntity(player);

            if (AITimer < 60)
                NPC.velocity *= 0.94f;

            if (AITimer++ == 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.Center, 2, 2, DustID.DungeonSpirit, 0f, 0f, 100, default, 2);
                    Main.dust[dustIndex].velocity *= 2f;
                    Main.dust[dustIndex].noGravity = true;
                }
                DustHelper.DrawDustImage(NPC.Center, DustID.DungeonSpirit, 0.5f, "Redemption/Effects/DustImages/DeadRingerDust", 2, true, 0);
            }
            NPC.alpha += Main.rand.Next(-10, 11);
            NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 40, 60);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= 4 * frameHeight)
                    NPC.frame.Y = 0;
            }
            if (!floatTimer)
            {
                NPC.velocity.Y += 0.03f;
                if (NPC.velocity.Y > .5f)
                {
                    floatTimer = true;
                    NPC.netUpdate = true;
                }
            }
            else if (floatTimer)
            {
                NPC.velocity.Y -= 0.03f;
                if (NPC.velocity.Y < -.5f)
                {
                    floatTimer = false;
                    NPC.netUpdate = true;
                }
            }
        }
        public static int ChatNumber = 0;
        public static bool what;
        public static bool request;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            bool offering = Main.LocalPlayer.HasItem(ItemID.NaturesGift);
            switch (ChatNumber)
            {
                case 0:
                    button = "About you?";
                    break;
                case 1:
                    button = "Thamor?";
                    break;
                case 2:
                    button = "How did you get here?";
                    break;
                case 3:
                    button = request && offering ? "Offer Nature's Gift" : "Request Crux";
                    break;
            }
            button2 = "Cycle Dialogue";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                Main.npcChatText = ChitChat();
                if (ChatNumber == 3)
                {
                    int offering = Main.LocalPlayer.FindItem(ItemID.NaturesGift);
                    if (request && offering >= 0)
                    {
                        if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                        {
                            Main.npcChatText = "Get a little more into the Spirit Realm, and I will give it to you.";
                            ChatNumber = 3;
                            return;
                        }
                        int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                        if (card >= 0)
                        {
                            Main.LocalPlayer.inventory[offering].stack--;
                            if (Main.LocalPlayer.inventory[offering].stack <= 0)
                                Main.LocalPlayer.inventory[offering] = new Item();

                            Main.LocalPlayer.inventory[card].stack--;
                            if (Main.LocalPlayer.inventory[card].stack <= 0)
                                Main.LocalPlayer.inventory[card] = new Item();

                            Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardMossyGoliath>());
                            Main.npcChatText = "A fine flower. It'll do. Take it, take the spirit of a goliath. It will serve you well, as it had to I.";
                            Main.npcChatCornerItem = ModContent.ItemType<CruxCardMossyGoliath>();
                            SoundEngine.PlaySound(SoundID.Chat);
                            ChatNumber = 3;
                        }
                        else
                        {
                            Main.npcChatText = "I'll need something to imbue first.";
                            Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                        }
                    }
                    else
                    {
                        Main.npcChatText = "You'll be lucky. I do not give to Anglonic fol'hakks freely. Assuming you are from that haughty place? It matters not, if you offer a blessing to me, I shall do the same to you. I request an enchanted flower to grow atop my grave - a blue one, preferably. Find one that looks pretty.";
                        Main.npcChatCornerItem = ItemID.NaturesGift;
                    }
                    request = true;
                }
            }
            else
            {
                ChatNumber++;
                int max = 2;
                if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive && !Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardMossyGoliath>()))
                    max = 3;
                if (ChatNumber > max)
                    ChatNumber = 0;
            }
        }
        public static string ChitChat()
        {
            return ChatNumber switch
            {
                0 => "Sent me back to look upon my remains and now you ask for a friendly chat. Gralvr! What a strange thing. Speaking to spirits have become a bore, so I might as well. I come from the Kul'don rainforests in what most call Thamor. We lived as a commune of druids, far from others. To us, the world was a small thing, we kept to ourselves and never stepped foot beyond our territory.",
                1 => "It is what the other spirits I've met call the region I lived in. During my time of being alive, I had never thought of what the world was like beyond the rainforests, as we had never travelled beyond them. To us, the rainforests were our entire world, at least until we were forced out.",
                2 => "A reasonable question. Of course, the portals here lead to Anglon and Gathuram, none are even close to Thamor, let alone the wilds of Kul'don. We were driven out by force, a posse of Anglonic soldiers came by in search of our goliaths - ones we had tamed and have been a part of our commune for generations. We retaliated of course, and drove them out. A short-lived victory, for they reappeared with their leader. They not only captured our goliaths, but us as well. To Anglon we were sent, and it is there I had escaped.",
                _ => "...",
            };
        }
        public override bool CanChat() => true;
        public override string GetChat()
        {
            return "Tokk'gralvr! Dorvr'kok ok kolbv? Ah, groa'g Anglic fofolg. It's always Anglic speakers, never any of my own... Whatever. I don't know how you took me back to my place of defeat, do you want something from me?";
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}