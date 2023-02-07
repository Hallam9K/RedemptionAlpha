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
    public class SpiritCommonGuard : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Common Guard");
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
            NPC.width = 30;
            NPC.height = 48;
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
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = ChatNumber switch
            {
                1 => "Anglon?",
                2 => "Ricusa?",
                3 => "Demons?",
                4 => "Request Crux",
                _ => "About you?",
            };
            button2 = "Cycle Dialogue";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                Main.npcChatText = ChitChat();
                if (ChatNumber == 4)
                {
                    if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                    {
                        Main.npcChatText = "Thou must be at least partly within the Spirit Realm for me to give thee what you ask.";
                        ChatNumber = 3;
                        return;
                    }
                    int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                    if (card >= 0)
                    {
                        Main.LocalPlayer.inventory[card].stack--;
                        if (Main.LocalPlayer.inventory[card].stack <= 0)
                            Main.LocalPlayer.inventory[card] = new Item();

                        Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardAnglonSkeletons>());
                        Main.npcChatText = "Aye, take it, within me lies the Common Guard's spirit! May it protect and defend thee against the greatest of foes.";
                        Main.npcChatCornerItem = ModContent.ItemType<CruxCardAnglonSkeletons>();
                        SoundEngine.PlaySound(SoundID.Chat);
                    }
                    else
                    {
                        Main.npcChatText = "I'll need something to imbue first.";
                        Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                    }
                    ChatNumber = 3;
                }
            }
            else
            {
                ChatNumber++;
                int max = 3;
                if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive && !Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardAnglonSkeletons>()))
                    max = 4;
                if (ChatNumber > max)
                    ChatNumber = 0;
            }
        }
        public static string ChitChat()
        {
            return ChatNumber switch
            {
                0 => "Behold, for I am Kleo! Common Guard of Ricusa City! I'm sure if ye were a resident of Anglon you'd be jumping in shock to face such a legendary figure! Oh who am I kidding, my name barely made it past my own district. But no matter! I did my job well and that is all that matters!",
                1 => "By Grace! To think anyone who walks this earth be unaware of the greatest domain in Epidotra! The Hallowed Dominion is where I'm from, and Ricusa City is where I resided. 'Tis a peaceful and prosperous land, home to great cities and castles and all the protection thou canst get! Well, sort of.",
                2 => "Ah, Ricusa... It was such a grand capital - also one of the largest in the domain. Did you know it started as a humble village? Eventually the old capital was laid to ruin, and due to the village's steady growth it was promoted to the new capital. All seemed well for ages to come... That was until the demons came.",
                3 => "A great demon terror invaded the capital with his underlings, never before had I seen such power and brute strength come from a single creature. The walls were broken down and the two legendary brothers - Archon and Vargo - fought against the them. They were great heroes, said they would trounce the demon forces back to Demonhollow. Yet for all their talk they were quickly killed by the leader with very little resistance, and so Ricusa City fell to the scourge. Like much else before it.",
                _ => "...",
            };
        }
        public override bool CanChat() => true;
        public override string GetChat()
        {
            return "What manner of sorcery is this? And here I thought us immune to the physical world's presence...";
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