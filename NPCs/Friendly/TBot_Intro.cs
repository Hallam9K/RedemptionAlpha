using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Redemption.Base;

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

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
            player.RedemptionScreen().lockScreen = true;
            player.RedemptionScreen().cutscene = true;
            if (NPC.alpha > 0)
                NPC.alpha -= 10;
            switch (TimerRand)
            {
                case 0:
                    if (AITimer++ == 0)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenTorch,
                                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                            Main.dust[dust].velocity *= 5f;
                            Main.dust[dust].noGravity = true;
                        }
                        NPC.velocity.X = 3;
                    }
                    NPC.velocity.X *= 0.99f;
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
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
                    if (AITimer++ == 18)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Woah, you look really familiar.", 180, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                    }
                    if (AITimer >= 200)
                    {
                        AITimer = 0;
                        TimerRand = 4;
                        NPC.netUpdate = true;
                    }
                    break;
                case 4:
                    NPC.LookAtEntity(player);
                    if (AITimer++ == 5)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Nevermind. My name is Adam.", 180, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                    }
                    if (AITimer == 185)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Mind if I stay for a while? I assume you have an accommodation to spare.", 240, 1, 0.5f, "Adam:", 0, null, null, NPC.Center + new Vector2(0, -80) - Main.screenPosition, null, 0, sound: true);
                    }
                    if (AITimer >= 435)
                    {
                        NPC.SetDefaults(ModContent.NPCType<TBot>());
                        NPC.GivenName = "Adam";
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            switch (TimerRand)
            {
                case 0:
                    NPC.frame.Y = 0;
                    break;
                case 1:
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 3 * frameHeight;
                    }
                    break;
                case 2:
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 6 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                    break;
                case 3:
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 12 * frameHeight)
                            NPC.frame.Y = 12 * frameHeight;
                    }
                    break;
                case 4:
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 14 * frameHeight)
                            NPC.frame.Y = 14 * frameHeight;
                    }
                    break;
            }
        }
    }
}