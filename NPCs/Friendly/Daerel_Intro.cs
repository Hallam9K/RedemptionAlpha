using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Terraria.GameContent.UI;
using Terraria.GameContent;
using Redemption.Items.Usable;
using Redemption.UI.ChatUI;
using Redemption.Textures;

namespace Redemption.NPCs.Friendly
{
    public class Daerel_Intro : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/Daerel";
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Daerel");
            Main.npcFrameCount[Type] = 26;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
        }
        private int Look;
        private static Texture2D Bubble => CommonTextures.TextBubble_Epidotra.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice4 with { Pitch = 0.6f };
        public override void AI()
        {
            NPC.DiscourageDespawn(60);
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            NPC portal = Main.npc[(int)NPC.ai[3]];

            if (NPC.alpha > 0 && TimerRand < 3)
                NPC.alpha -= 10;
            switch (TimerRand)
            {
                case 0:
                    if (AITimer++ == 0)
                    {
                        NPC.spriteDirection = 1;
                        NPC.velocity.X = 4;
                        NPC.velocity.Y = -3;
                        for (int i = 0; i < 30; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 1, 0, 0, default, 0.5f);
                            Main.dust[dust].noGravity = true;
                            Color dustColor = new(Color.DarkOliveGreen.R, Color.DarkOliveGreen.G, Color.DarkOliveGreen.B) { A = 0 };
                            Main.dust[dust].color = dustColor;
                            Main.dust[dust].velocity *= 3f;
                        }
                        if (!Main.dedServ)
                        {
                            Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.1"), Color.White, Color.Gray, voice, 0.01f, .5f, .5f, true, bubble: Bubble); // 65

                            ChatUI.Visible = true;
                            ChatUI.Add(d1);
                        }
                    }
                    NPC.rotation += 0.1f;
                    NPC.velocity.X *= 0.99f;
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        NPC.rotation = 0;
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
                    if (AITimer++ >= 60)
                    {
                        ExtraFrames = 0;
                        NPC.velocity.Y = -5;
                        AITimer = 0;
                        TimerRand = 2;
                    }
                    break;
                case 2:
                    if (AITimer++ == 40 && !Main.dedServ)
                    {
                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 187);
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.2"), Color.White, Color.Gray, voice, .05f, 2, .5f, true, bubble: Bubble, endID: 1)); // 187
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 1000)
                    {
                        ExtraFrames = 0;
                        ExtraTexs = 0;
                        NPC.spriteDirection = -1;
                        AITimer = 0;
                        TimerRand = 3;
                    }
                    break;
                case 3:
                    if (AITimer++ == 5 && !Main.dedServ)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.3"), Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble)) // 221
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.4"), Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble)) // 166
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.5"), Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble)) // 206
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.6"), Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble)) // 257
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.7"), Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble)) // 320
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.DaerelIntro.8"), Color.White, Color.Gray, voice, .05f, 2, .5f, true, bubble: Bubble, endID: 1)); // 382
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (Look == 1 && AITimer % 30 == 0)
                        NPC.spriteDirection *= -1;
                    if (Look == 2)
                        NPC.LookAtEntity(player);
                    if (AITimer == 3000)
                    {
                        NPC.velocity.Y = -8;
                        NPC.velocity.X = -3;
                    }
                    if (AITimer >= 3060)
                    {
                        NPC.noTileCollide = true;
                        NPC.Move(portal.Center, 20, 30);
                        NPC.alpha += 5;
                    }
                    if (AITimer >= 3000)
                    {
                        NPC.rotation -= 0.1f;
                        NPC.velocity.X *= 0.99f;

                        if (NPC.DistanceSQ(portal.Center) < 80 * 80)
                            NPC.alpha += 10;
                        if (NPC.alpha >= 255)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, NPC.position);
                            for (int i = 0; i < 30; i++)
                            {
                                int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 1, 0, 0, default, 0.5f);
                                Main.dust[dust].noGravity = true;
                                Color dustColor = new(Color.DarkOliveGreen.R, Color.DarkOliveGreen.G, Color.DarkOliveGreen.B) { A = 0 };
                                Main.dust[dust].color = dustColor;
                                Main.dust[dust].velocity *= 3f;
                            }
                            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<ChaliceFragments>());
                            NPC.active = false;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeQuest.wayfarerVars[0] = 2;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                        }
                    }
                    break;
            }
            ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium);
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            switch (signature)
            {
                case "a":
                    Look = 1;
                    EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 166);
                    break;
                case "b":
                    Look = 2;
                    EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 120);
                    NPC.velocity.Y = -3;
                    break;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 2999;
        }
        private int ExtraFrames;
        private int ExtraTexs;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            NPC.frame.X = 0;

            switch (TimerRand)
            {
                case 0:
                    NPC.frame.Y = frameHeight;
                    break;
                case 1:
                    ExtraTexs = 2;
                    if (NPC.frameCounter++ >= 6)
                    {
                        NPC.frameCounter = 0;
                        if (++ExtraFrames > 3)
                            ExtraFrames = 0;
                    }
                    break;
                case 2:
                    if (NPC.collideY || NPC.velocity.Y == 0)
                    {
                        ExtraTexs = 1;
                        if (NPC.frameCounter++ >= 10)
                        {
                            NPC.frameCounter = 0;
                            if (++ExtraFrames > 1)
                                ExtraFrames = 0;
                        }
                    }
                    else
                    {
                        ExtraTexs = 0;
                        ExtraFrames = 0;
                        NPC.frame.Y = frameHeight;
                    }
                    break;
                case 3:
                    if (NPC.collideY || NPC.velocity.Y == 0)
                    {
                        NPC.frame.Y = 0;
                    }
                    else
                        NPC.frame.Y = frameHeight;
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D extra = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Daerel_Extra").Value;
            Texture2D unconscious = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/DaerelUnconscious").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            switch (ExtraTexs)
            {
                case 0:
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    break;
                case 1:
                    int Height = extra.Height / 2;
                    int y = Height * ExtraFrames;
                    Rectangle rect = new(0, y, extra.Width, Height);
                    Vector2 origin = new(extra.Width / 2f, Height / 2f);
                    spriteBatch.Draw(extra, NPC.Center + new Vector2(2, 2) - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
                    break;
                case 2:
                    int Height2 = unconscious.Height / 4;
                    int y2 = Height2 * ExtraFrames;
                    Rectangle rect2 = new(0, y2, unconscious.Width / 3, Height2);
                    Vector2 origin2 = new(unconscious.Width / 2f, Height2 / 2f);
                    spriteBatch.Draw(unconscious, NPC.Center + new Vector2(40, 6) - screenPos, new Rectangle?(rect2), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
                    break;
            }
            return false;
        }
    }
}
