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
    public class Newb_Intro : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/Newb";

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Newb");
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

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            Texture2D bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Kingdom").Value;
            SoundStyle voice1 = CustomSounds.Voice3 with { Pitch = -0.8f };
            SoundStyle voice2 = CustomSounds.Voice3 with { Pitch = -0.1f };

            switch (TimerRand)
            {
                case 0:
                    if (AITimer++ == 0)
                    {
                        NPC.spriteDirection = 1;
                        Dialogue d1 = new(NPC, "...", Color.White, Color.Gray, voice1, 1, 120, 60, true, bubble: bubble);

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(d1);
                    }
                    if (AITimer >= 60)
                    {
                        AITimer = 0;
                        TimerRand = 1;
                    }
                    break;
                case 1:
                    if (AITimer++ == 30)
                    {
                        Dialogue d1 = new(NPC, "What do you think, Jo-", Color.White, Color.Gray, voice1, 3, 30, 100, true, bubble: bubble);
                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(d1);
                    }
                    if (AITimer >= 209)
                    {
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 60);
                        AITimer = 0;
                        TimerRand = 2;
                    }
                    break;
                case 2:
                    if (AITimer++ == 60)
                    {
                        EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 120);
                        DialogueChain chain = new();
                        chain.Add(new(NPC, "Who you?!", Color.White, Color.Gray, voice2, 3, 100, 0, false, bubble: bubble)) // 166
                             .Add(new(NPC, "Where am I?", Color.White, Color.Gray, voice2, 3, 100, 0, false, bubble: bubble)) // 166
                             .Add(new(NPC, "Heyo, I'm Newb![60] Want to be friends?", Color.White, Color.Gray, voice2, 3, 100, 30, true, bubble: bubble)); // 196

                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(chain);
                    }
                    if (AITimer >= 30)
                        NPC.LookAtEntity(player);

                    if (AITimer >= 530)
                    {
                        NPC.SetDefaults(ModContent.NPCType<Newb>());
                        NPC.GivenName = "Newb";
                        NPC.netUpdate = true;
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