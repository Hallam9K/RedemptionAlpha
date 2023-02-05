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
    public class SpiritGathicMan : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Old Man");
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
            NPC.width = 44;
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
                1 => "Olden Ruins?",
                2 => "God of Decay?",
                3 => "False Gods?",
                4 => "Dead Ringer?",
                5 => "Request Crux",
                _ => "About you?",
            };
            button2 = "Cycle Dialogue";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                Main.npcChatText = ChitChat();
                if (ChatNumber == 5)
                {
                    if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                    {
                        Main.npcChatText = "Get in tha Spirit Realm, garbar! Can't do anythin' otherwise!";
                        ChatNumber = 4;
                        return;
                    }
                    int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                    if (card >= 0)
                    {
                        Main.LocalPlayer.inventory[card].stack--;
                        if (Main.LocalPlayer.inventory[card].stack <= 0)
                            Main.LocalPlayer.inventory[card] = new Item();

                        Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardGathicSkeletons>());
                        Main.npcChatText = "Sure thing, lad! Have the spirits of my people, and may they stab ya foes hard!";
                        Main.npcChatCornerItem = ModContent.ItemType<CruxCardGathicSkeletons>();
                        SoundEngine.PlaySound(SoundID.Chat);
                    }
                    else
                    {
                        Main.npcChatText = "Gimme somethin' to imbue first!";
                        Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                    }
                    ChatNumber = 4;
                }
            }
            else
            {
                ChatNumber++;
                int max = 4;
                if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive && !Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardGathicSkeletons>()))
                    max = 5;
                if (ChatNumber > max)
                    ChatNumber = 0;
            }
        }
        public static string ChitChat()
        {
            return ChatNumber switch
            {
                0 => "They called me the \"Brain Cleaver\" back in ma prime, for reasons I'm sure ya can figure out. My real name was somethin' less intense - Bromal. I was a warrior of Iful's Capital in the ol' kingdom of Kol Duluramodul before the undead took over. Now it's known as the Olden Ruins.",
                1 => "A rotting mass of land home to Gathuram, a domain of Epidotra. During ma time there were no such things as \"domains\", the kingdom was just about all I knew. The folk of it ere the fall came from up north-east, migrating from the sinking Eranmount. They named their new land Odulmount - meaning Great Swamp in your tongue - but after lifetimes of glory, it was renamed to Kol Duluramodul - meaning Immovable Swamp Kingdom. It was indeed an impenetrable land, until the God of Decay appeared.",
                2 => "A disease swept through Kol Duluramodul like an air-borne tsunami. The cause was the False God of Decay who roamed Tethuram - Gathuram's name in ancient times. It rotted and corroded flesh and turned humans into undead. The swamps, we discovered, were where the infection thrived most, so the folk who yet lived abandoned the kingdom and travelled further west. Such hard times they were, I only look back in anguish for my people.",
                3 => "Be thankful for the present ya live in now. The Ancient World was an unforgiving age, made worse by the False Gods. We only knew of one at the time - after becoming a spirit I learnt a lot more about them from others of the realm. Apparently there was also one in Anglon, Blight I believe. For all the knowledge a thousand spirits can give, not one knows of the False Gods' origins. They're all dead though, the last of 'em was felled by ye olde Light. And I'm not talking about the Light you may know.",
                4 => "Aye, I know of the artifact you carry, in fact it was created in Iful's Capital! 'Tis designed to call upon the spirits of corpses. Originally, the purpose was for gainin' knowledge of the past, but overtime we used it for more personal reasons - such as lettin' the dead talk to their family an' friends. I am unsure why it was found beneath this island though.",
                _ => "...",
            };
        }
        public override bool CanChat() => true;
        public override string GetChat()
        {
            return "RAAAARRGGGHH! Haha! I saw ya comin' with ya fancy artifact, knew ya might call me with it, so I was ready to give ya a jump! No reaction? Come on, I yelled pretty loud. Not even a slight shiver in ya boots? Whatever, what is it?";
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