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
using Terraria.GameContent;

namespace Redemption.NPCs.Friendly
{
    public class Zephos_Intro : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/Zephos";
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zephos");
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 46;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            NPC portal = Main.npc[(int)NPC.ai[3]];
            Texture2D bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra").Value;

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
                        Dialogue d1 = new(NPC, null, bubble, null, Color.White, Color.Gray, null, "WAH!", 1, 30, 30, true); // 64

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.AddDialogue(d1);
                    }
                    NPC.rotation += 0.1f;
                    NPC.velocity.X *= 0.99f;
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        NPC.rotation = 0;
                        SoundEngine.PlaySound(SoundID.Dig, NPC.position);
                        player.RedemptionScreen().ScreenShakeIntensity = 3;
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
                    if (AITimer++ == 40)
                    {
                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                        Dialogue d1 = new(NPC, null, bubble, null, Color.White, Color.Gray, null, "Jeez,[10] bad landing.", 3, 100, 30, true); // 194

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.AddDialogue(d1);
                    }
                    if (AITimer >= 234)
                    {
                        ExtraFrames = 0;
                        ExtraTexs = 0;
                        NPC.spriteDirection = -1;
                        AITimer = 0;
                        TimerRand = 3;
                    }
                    break;
                case 3:
                    if (AITimer++ == 5)
                    {
                        Dialogue d1 = new(NPC, null, bubble, null, Color.White, Color.Gray, null, "Ey Daerel,[10] know where we are?", 3, 100, 0, false); // 197
                        Dialogue d2 = new(NPC, null, bubble, null, Color.White, Color.Gray, d1, "Uh..[30] Daerel?", 3, 100, 0, false); // 166
                        Dialogue d3 = new(NPC, null, bubble, null, Color.White, Color.Gray, d2, "Oh,[10] hey there![10] Didn't notice you.", 3, 100, 0, false); // 219
                        Dialogue d4 = new(NPC, null, bubble, null, Color.White, Color.Gray, d3, "You haven't happened upon a boy cloaked in black,[10] have you?", 3, 100, 0, false); // 287
                        Dialogue d5 = new(NPC, null, bubble, null, Color.White, Color.Gray, d4, "Guess he didn't jump in.[30] Oh well![10] I'll head back to get him.", 3, 100, 0, false); // 320
                        Dialogue d6 = new(NPC, null, bubble, null, Color.White, Color.Gray, d5, "I'll come back once I find him,[10] so see ya later!", 3, 100, 30, true); // 284

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.AddDialogue(d1);
                        TextBubbleUI.AddDialogue(d2);
                        TextBubbleUI.AddDialogue(d3);
                        TextBubbleUI.AddDialogue(d4);
                        TextBubbleUI.AddDialogue(d5);
                        TextBubbleUI.AddDialogue(d6);
                    }
                    if (AITimer >= 202 && AITimer % 30 == 0 && AITimer < 368)
                        NPC.spriteDirection *= -1;
                    if (AITimer == 202)
                        EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 197);
                    if (AITimer >= 368)
                        NPC.LookAtEntity(player);
                    if (AITimer == 368)
                    {
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 120);
                        NPC.velocity.Y = -3;
                    }
                    if (AITimer == 1478)
                    {
                        NPC.velocity.Y = -8;
                        NPC.velocity.X = -3;
                    }
                    if (AITimer >= 1538)
                    {
                        NPC.noTileCollide = true;
                        NPC.Move(portal.Center, 20, 30);
                        NPC.alpha += 5;
                    }
                    if (AITimer >= 1478)
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
                            RedeQuest.wayfarerVars[0] = 2;
                            NPC.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                    }
                    break;
            }

            player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
            player.RedemptionScreen().lockScreen = true;
            player.RedemptionScreen().cutscene = true;
            NPC.LockMoveRadius(player);
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1f).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
        }
        private int ExtraFrames;
        private int ExtraTexs;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
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
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D extra = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Zephos_Extra").Value;
            Texture2D unconscious = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/ZephosUnconscious").Value;
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
                    spriteBatch.Draw(extra, NPC.Center - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
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