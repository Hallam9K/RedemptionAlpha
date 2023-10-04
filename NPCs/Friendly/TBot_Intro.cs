using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Redemption.Base;
using Redemption.Dusts;
using Terraria.GameContent.UI;
using Redemption.UI.ChatUI;

namespace Redemption.NPCs.Friendly
{
    public class TBot_Intro : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Adam");
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
        private int Look;
        public override void AI()
        {
            NPC.DiscourageDespawn(60);
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
                        if (AITimer++ == 18 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Robot1"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false)) // 112
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Robot2"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true, endID: 1)); // 183
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer >= 1000)
                        {
                            AITimer = 0;
                            TimerRand = 4;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer++ == 18 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Normal1"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false)) // 109
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Normal2"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true, endID: 1)); // 335
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer >= 1000)
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
                        if (Look == 0)
                            NPC.LookAtEntity(player);
                        if (AITimer++ == 5 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Robot3"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false)) // 272
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Robot4"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true)) // 473
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Robot5"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true, endID: 1)); // 307
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);

                            EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 272);
                        }
                        if (AITimer >= 3000)
                        {
                            NPC.SetDefaults(ModContent.NPCType<TBot>());
                            NPC.GivenName = Language.GetTextValue("Mods.Redemption.NPCs.TBot_Intro.DisplayName");
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (Look == 0)
                            NPC.LookAtEntity(player);
                        if (AITimer++ == 5 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Normal3"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false)) // 414
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Normal4"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true, endID: 1)); // 560
                            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer == 3000 && !Main.dedServ)
                        {
                            Dialogue d4 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Normal5"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true); // 265

                            EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 265);
                            NPC.spriteDirection = -NPC.spriteDirection;
                            ChatUI.Visible = true;
                            ChatUI.Add(d4);
                        }
                        if (AITimer >= 3265)
                        {
                            NPC.SetDefaults(ModContent.NPCType<TBot>());
                            NPC.GivenName = Language.GetTextValue("Mods.Redemption.NPCs.TBot_Intro.DisplayName");
                            NPC.netUpdate = true;
                        }
                    }
                    break;
            }

            if (!playerTBot && AITimer >= 3000 && TimerRand == 4)
                return;
            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 1200);
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            switch (signature)
            {
                case "a":
                    Look = 1;
                    EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 307);
                    NPC.spriteDirection = -NPC.spriteDirection;
                    break;
                case "b":
                    EmoteBubble.NewBubble(98, new WorldUIAnchor(NPC), 204);
                    break;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 2999;
            if (TimerRand == 4)
                Look = 1;
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
