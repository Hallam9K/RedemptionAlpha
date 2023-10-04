using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Terraria.GameContent.UI;
using Terraria.GameContent;
using Redemption.UI.ChatUI;
using System;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Textures;

namespace Redemption.NPCs.Minibosses.Calavia
{
    public class Calavia_Intro : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/Calavia/Calavia";
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Calavia");
            Main.npcFrameCount[Type] = 20;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.friendly = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.npcSlots = 0;
            NPC.dontTakeDamage = true;
        }
        public override bool CheckActive() => false;
        private static Texture2D Bubble => CommonTextures.TextBubble_Epidotra.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice1 with { Pitch = 0.6f };
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            switch (TimerRand)
            {
                case 0:
                    if (!player.active || player.dead || !NPC.Sight(player, 400, false, true))
                        return;
                    TimerRand = 1;
                    break;
                case 1:
                    if (RedeQuest.calaviaVar is 2)
                    {
                        if (AITimer++ == 0)
                        {
                            NPC.LookAtEntity(player);
                            EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 60);
                            if (!Main.dedServ)
                            {
                                string s1 = "Mur sium'roro?[0.2] Bidu'oque, khru!"; // Fight again? Get lost, undead! (oque = get/obtain, Bidu = lost)

                                DialogueChain chain = new();
                                chain.Add(new(NPC, s1, Color.White, Color.Gray, voice, .05f, 2f, .5f, true, bubble: Bubble, endID: 1));
                                chain.OnEndTrigger += Chain_OnEndTrigger;
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);
                            }
                        }
                        if (AITimer >= 600)
                        {
                            BodyFrame = 2 * 56;
                            CustomBodyAni = true;
                            SoundEngine.PlaySound(SoundID.Grab, NPC.position);
                            ShieldOut = true;
                            NPC.velocity.X = 0;
                            AITimer = 0;
                            TimerRand = 3;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer++ == 0)
                        {
                            NPC.LookAtEntity(player);
                            NPC.velocity.X = 3 * -NPC.spriteDirection;
                            NPC.velocity.Y = -3;
                            EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 60);
                            if (!Main.dedServ)
                            {
                                Dialogue d1 = new(NPC, "Gor'on!", Color.White, Color.Gray, voice, 0.01f, 1f, .5f, true, bubble: Bubble);

                                ChatUI.Visible = true;
                                ChatUI.Add(d1);
                            }
                        }
                        NPC.velocity.X *= 0.99f;
                        if (BaseAI.HitTileOnSide(NPC, 3))
                        {
                            SoundEngine.PlaySound(SoundID.Grab, NPC.position);
                            ShieldOut = true;
                            NPC.velocity.X = 0;
                            AITimer = 0;
                            TimerRand = 2;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case 2:
                    NPC.LookAtEntity(player);
                    if (AITimer++ == 80 && !Main.dedServ)
                    {

                        string s1 = "[@d]Mubirok abo,[0.2][@c] lo tasium ye!";
                        string s2 = ".^0.3^..^0.05^ [@e]Ta baadurlo ye.";

                        DialogueChain chain = new();
                        chain.Add(new(NPC, s1, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, s2, Color.White, Color.Gray, voice, .05f, 2f, .5f, true, bubble: Bubble, endID: 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 3000)
                    {
                        BodyFrame = 2 * 56;
                        CustomBodyAni = true;
                        AITimer = 0;
                        TimerRand = 3;
                    }
                    break;
                case 3:
                    HoldIcefall = true;
                    if (AITimer++ == 0)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Calavia_IcefallArena>(), 0, Vector2.Zero, CustomSounds.IceMist, NPC.whoAmI);
                    if (AITimer % 2 == 0 && AITimer <= 80)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.IceMist with { Pitch = AITimer / 80 }, NPC.position);
                        for (int k = 0; k < 10; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * (60 + (AITimer * 6)));
                            vector.Y = (float)(Math.Cos(angle) * (60 + (AITimer * 6)));
                            Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, ModContent.DustType<GlowDust>(), 0f, 0f, 100, default, .2f)];
                            dust2.noGravity = true;
                            Color dustColor = new(Color.LightCyan.R, Color.LightCyan.G, Color.LightCyan.B) { A = 0 };
                            dust2.color = dustColor;
                            dust2.velocity = NPC.DirectionTo(dust2.position) * Main.rand.NextFloat(2f, 4f);
                        }
                    }
                    if (AITimer >= 120)
                    {
                        if (RedeQuest.calaviaVar < 2 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeQuest.calaviaVar = 2;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        NPC.SetDefaults(ModContent.NPCType<Calavia>());
                        NPC.netUpdate = true;
                    }
                    break;
            }
            if (TimerRand < 3)
            {
                ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Low);
            }
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            switch (signature)
            {
                case "c":
                    CustomBodyAni = true;
                    BodyFrame = 56;
                    break;
                case "d":
                    CustomBodyAni = true;
                    BodyFrame = 3 * 56;
                    break;
                case "e":
                    CustomBodyAni = false;
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                    break;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 2999;
        }
        private int BodyFrame;
        private bool CustomBodyAni;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 19 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
            else
                NPC.frame.Y = 5 * frameHeight;

            if (!CustomBodyAni)
                BodyFrame = NPC.frame.Y;
        }
        private bool ShieldOut;
        private bool HoldIcefall;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rect = new(0, BodyFrame, NPC.frame.Width, NPC.frame.Height);

            spriteBatch.Draw(Calavia.CloakTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.LegsTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (ShieldOut)
                spriteBatch.Draw(Calavia.ShieldTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HoldIcefall)
                spriteBatch.Draw(TextureAssets.Item[ModContent.ItemType<Icefall>()].Value, NPC.Center + new Vector2(14 * NPC.spriteDirection, 0) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.ArmTex.Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}
