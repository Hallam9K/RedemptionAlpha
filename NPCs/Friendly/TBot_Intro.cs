using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
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
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("HUH?", 150, 1, 0.5f, "Adam:", 0.5f, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        if (AITimer == 168)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("How- Wh- I-", 180, 1, 0.5f, "Adam:", 0.4f, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        if (AITimer >= 348)
                        {
                            AITimer = 0;
                            TimerRand = 4;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer++ == 18)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("AH!", 150, 1, 0.5f, "Adam:", 0.5f, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        if (AITimer == 168)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...I-I thought you were someone else. Sorry about that.", 240, 1, 0.5f, "Adam:", 0.2f, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        if (AITimer >= 408)
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
                        if (AITimer < 700)
                            NPC.LookAtEntity(player);
                        if (AITimer++ == 5)
                        {
                            EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 240);
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("*Sigh* I wasn't expecting to see the likes of us here.", 240, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        }
                        if (AITimer == 245)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Not going back there now. Since you're here and not there,\nI can safely assume you're not with Her..?", 360, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        if (AITimer == 605)
                            EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 335);
                        if (AITimer == 700)
                        {
                            NPC.spriteDirection = -NPC.spriteDirection;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...Alright. We can safely ignore each other then.", 240, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        }
                        if (AITimer >= 940)
                        {
                            NPC.SetDefaults(ModContent.NPCType<TBot>());
                            NPC.GivenName = "Adam";
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer < 830)
                            NPC.LookAtEntity(player);
                        if (AITimer++ == 5)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Anyways, hello. I'm Adam, and I'm an Android originating from a faraway snowy wasteland.", 300, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        if (AITimer == 305)
                        {
                            EmoteBubble.NewBubble(98, new WorldUIAnchor(NPC), 240);
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Seeing as I won't be returning to that frozen hell for a good while,", 240, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        }
                        if (AITimer == 545)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("mind if I stay here? I assume you've some shelter to stay at.", 240, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        if (AITimer == 830)
                        {
                            EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 180);
                            NPC.spriteDirection = -NPC.spriteDirection;
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...The resemblance between them is uncanny...", 180, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                        }
                        if (AITimer >= 1010)
                        {
                            NPC.SetDefaults(ModContent.NPCType<TBot>());
                            NPC.GivenName = "Adam";
                            NPC.netUpdate = true;
                        }
                    }
                    break;
            }
            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextPos = NPC.Center + new Vector2(0, -80) - Main.screenPosition;

            if (!playerTBot && AITimer >= 785 && TimerRand == 4)
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