using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Redemption.Dusts;
using Terraria.GameContent.UI;
using Redemption.UI;

namespace Redemption.NPCs.Friendly
{
    public class TBot_Intro : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adam");
            Main.npcFrameCount[Type] = 15;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 42;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
        }
        private bool playerTBot;
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (NPC.alpha > 0)
                NPC.alpha -= 10;
            switch (TimerRand)
            {
                case 0:
                    if (AITimer++ == 0)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 1, 0, 0, default, 0.5f);
                            Main.dust[dust].noGravity = true;
                            Color dustColor = new(Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B) { A = 0 };
                            Main.dust[dust].color = dustColor;
                            Main.dust[dust].velocity *= 3f;
                        }
                        NPC.velocity.X = 3;
                    }
                    NPC.velocity.X *= 0.99f;
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        if (player.IsFullTBot())
                            playerTBot = true;
                        SoundEngine.PlaySound(SoundID.Dig, NPC.position);
                        player.RedemptionScreen().ScreenShakeIntensity += 3;
                        for (int i = 0; i < 30; i++)
                            Dust.NewDust(NPC.BottomLeft, Main.rand.Next(NPC.width), 1, DustID.Smoke, 0, 0, 0, default, 1);
                        NPC.velocity.X = 0;
                        AITimer = 0;
                        TimerRand = 1;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    if (AITimer++ >= 50)
                    {
                        AITimer = 0;
                        TimerRand = 2;
                    }
                    break;
                case 2:
                    if (AITimer++ >= 50)
                    {
                        AITimer = 0;
                        TimerRand = 3;
                    }
                    break;
                case 3:
                    if (NPC.frame.Y >= 8 * 68)
                        NPC.LookAtEntity(player);
                    if (playerTBot)
                    {
                        if (AITimer++ == 18)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, "HUH?", Color.LightGreen, Color.DarkGreen, null, 3, 100, 0, false)) // 112
                                 .Add(new(NPC, "How-[10] Wh-[10] I-", Color.LightGreen, Color.DarkGreen, boxFade: true)); // 183

                            TextBubbleUI.Visible = true;
                            TextBubbleUI.Add(chain);
                        }
                        if (AITimer >= 295)
                        {
                            AITimer = 0;
                            TimerRand = 4;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer++ == 18)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, "AH!", Color.LightGreen, Color.DarkGreen, null, 3, 100, 0, false)) // 109
                                 .Add(new(NPC, "...I-[10]I thought you were someone else.[30] Sorry about that.", Color.LightGreen, Color.DarkGreen, null, boxFade: true)); // 335

                            TextBubbleUI.Visible = true;
                            TextBubbleUI.Add(chain);
                        }
                        if (AITimer >= 444)
                        {
                            AITimer = 0;
                            TimerRand = 4;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case 4:
                    if (playerTBot)
                    {
                        if (AITimer < 750)
                            NPC.LookAtEntity(player);
                        if (AITimer++ == 5)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, "*Sigh*[10] I wasn't expecting to see the likes of us here.", Color.LightGreen, Color.DarkGreen, null, 3, 100, 0, false)) // 272
                                 .Add(new(NPC, "Not going back there now.[30] Since you're here and not there,[10] I can safely assume you're not with Her..?", Color.LightGreen, Color.DarkGreen, boxFade: true)) // 473
                                 .Add(new(NPC, "...Alright.[30] We can safely ignore each other then.", Color.LightGreen, Color.DarkGreen, boxFade: true)); // 307

                            TextBubbleUI.Visible = true;
                            TextBubbleUI.Add(chain);

                            EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 272);
                        }
                        if (AITimer == 750)
                        {
                            EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 307);
                            NPC.spriteDirection = -NPC.spriteDirection;
                        }
                        if (AITimer >= 1057)
                        {
                            NPC.SetDefaults(ModContent.NPCType<TBot>());
                            NPC.GivenName = "Adam";
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer < 979)
                            NPC.LookAtEntity(player);
                        if (AITimer++ == 5)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, "Anyways,[10] hello.[30] I'm Adam,[10] and I'm an Android originating from a faraway snowy wasteland.", Color.LightGreen, Color.DarkGreen, null, 3, 100, 0, false)) // 414
                                 .Add(new(NPC, "Seeing as I won't be returning to that frozen hell for a good while,[10] mind if I stay here?[30] I assume you've some shelter to stay at.", Color.LightGreen, Color.DarkGreen, boxFade: true)); // 560

                            TextBubbleUI.Visible = true;
                            TextBubbleUI.Add(chain);
                        }
                        if (AITimer == 419)
                            EmoteBubble.NewBubble(98, new WorldUIAnchor(NPC), 204);
                        if (AITimer == 979)
                        {
                            Dialogue d4 = new(NPC, "...The resemblance between them is uncanny...", Color.LightGreen, Color.DarkGreen, boxFade: true); // 265

                            EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 265);
                            NPC.spriteDirection = -NPC.spriteDirection;
                            TextBubbleUI.Visible = true;
                            TextBubbleUI.Add(d4);
                        }
                        if (AITimer >= 1244)
                        {
                            NPC.SetDefaults(ModContent.NPCType<TBot>());
                            NPC.GivenName = "Adam";
                            NPC.netUpdate = true;
                        }
                    }
                    break;
            }

            if (!playerTBot && AITimer >= 979 && TimerRand == 4)
                return;
            player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
            player.RedemptionScreen().lockScreen = true;
            player.RedemptionScreen().cutscene = true;
            NPC.LockMoveRadius(player);
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1f).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
        }

        public override void FindFrame(int frameHeight)
        {
            if (TimerRand == 0)
            {
                NPC.frame.Y = 0;
                return;
            }
            if (NPC.frameCounter++ >= 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                switch (TimerRand)
                {
                    case 1:
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 3 * frameHeight;
                        break;
                    case 2:
                        if (NPC.frame.Y > 6 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                        break;
                    case 3:
                        if (NPC.frame.Y > 12 * frameHeight)
                            NPC.frame.Y = 12 * frameHeight;
                        break;
                    case 4:
                        if (NPC.frame.Y > 14 * frameHeight)
                            NPC.frame.Y = 14 * frameHeight;
                        break;
                }
            }
        }
    }
}