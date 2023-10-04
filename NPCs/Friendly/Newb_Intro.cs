using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI;
using Terraria.GameContent;
using Redemption.UI.ChatUI;
using Redemption.Textures;

namespace Redemption.NPCs.Friendly
{
    public class Newb_Intro : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/Newb";

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Newb");
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.dontTakeDamage = true;
        }
        private static Texture2D Bubble => CommonTextures.TextBubble_Epidotra.Value;
        private static readonly SoundStyle voice1 = CustomSounds.Voice3 with { Pitch = -0.8f };
        private static readonly SoundStyle voice2 = CustomSounds.Voice3 with { Pitch = -0.1f };
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            switch (TimerRand)
            {
                case 0:
                    if (AITimer++ == 0)
                    {
                        for (int i = 0; i < 60; i++)
                            Dust.NewDust(NPC.position - new Vector2(20, 0), NPC.width + 40, NPC.height, DustID.Dirt, Scale: 2);
                        for (int i = 0; i < 20; i++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Dirt, 0, -6, Scale: 1.5f);

                        NPC.spriteDirection = 1;
                        if (!Main.dedServ)
                        {
                            Dialogue d1 = new(NPC, "...", Color.White, Color.Gray, voice1, .01f, .5f, 1, true, bubble: Bubble);

                            ChatUI.Visible = true;
                            ChatUI.Add(d1);
                        }
                    }
                    if (AITimer >= 60)
                    {
                        AITimer = 0;
                        TimerRand = 1;
                    }
                    break;
                case 1:
                    if (AITimer++ == 30 && !Main.dedServ)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.NewbIntro.1"), Color.White, Color.Gray, voice1, .05f, .5f, 2f, true, bubble: Bubble, endID: 1)); // 187
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 1000)
                    {
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 60);
                        AITimer = 0;
                        TimerRand = 2;
                    }
                    break;
                case 2:
                    if (AITimer++ == 60 && !Main.dedServ)
                    {
                        EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 120);
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.NewbIntro.2"), Color.White, Color.Gray, voice2, .05f, 2f, 0, false, bubble: Bubble)) // 166
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.NewbIntro.3"), Color.White, Color.Gray, voice2, .05f, 2f, 0, false, bubble: Bubble)) // 166
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.NewbIntro.4"), Color.White, Color.Gray, voice2, .05f, 2f, .5f, true, bubble: Bubble, endID: 1)); // 196
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 30)
                        NPC.LookAtEntity(player);

                    if (AITimer >= 2000)
                    {
                        NPC.SetDefaults(ModContent.NPCType<Newb>());
                        NPC.GivenName = Language.GetTextValue("Mods.Redemption.NPCs.Newb_Intro.DisplayName");
                        NPC.netUpdate = true;
                    }
                    break;
            }
            if (RedeConfigClient.Instance.CameraLockDisable)
                return;
            if (NPC.DistanceSQ(player.Center) <= 600 * 600)
            {
                player.RedemptionScreen().ScreenFocusPosition = Vector2.Lerp(NPC.Center, player.Center, player.DistanceSQ(NPC.Center) / (1200 * 1200));
                player.RedemptionScreen().lockScreen = true;
                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(MathHelper.Lerp(1, 0, player.DistanceSQ(NPC.Center) / (600 * 600))).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 2999;
        }
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
                switch (TimerRand)
                {
                    case 0:
                        NPC.frame.X = NPC.frame.Width;
                        break;
                    case 1:
                        NPC.frame.X = NPC.frame.Width;
                        break;
                    case 2:
                        NPC.frame.X = 0;
                        break;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 2) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

    }
}
