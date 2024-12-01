using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Weapons.HM.Melee;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly
{
    public class Noza_NPC : ModNPC
    {
        public enum EmotionState
        {
            Normal, Blah, Hmm, Laugh
        }

        public EmotionState EmoteState
        {
            get => (EmotionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AITimer => ref NPC.ai[1];
        public ref float Force => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Noza, Tamer of Evil");
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 76;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0;
            tailChain = new TailScarfPhys();
        }

        private static IPhysChain tailChain;
        public override void AI()
        {
            NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChain[0] = tailChain;
            NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChainOffset[0] = new Vector2(-4f, 6f);
            NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChainDir[0] = -NPC.spriteDirection;
        }
        public override bool UsesPartyHat() => false;
        public override bool CanChat() => true;
        private int EyeFrameY;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ % 10 == 0)
            {
                if (EyeFrameY >= 2)
                    EyeFrameY = 0;
                if (EyeFrameY == 1)
                    EyeFrameY++;
                if (EyeFrameY == 0 && Main.rand.NextBool(16))
                    EyeFrameY = 1;
            }
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].whoAmI != NPC.whoAmI)
                EmoteState = EmotionState.Normal;

            switch (EmoteState)
            {
                case EmotionState.Normal:
                    if (NPC.frameCounter % 20 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                    break;
                case EmotionState.Blah:
                    if (NPC.frame.Y < 4 * frameHeight)
                        NPC.frame.Y = 4 * frameHeight;

                    if (NPC.frameCounter % 10 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 5 * frameHeight)
                            NPC.frame.Y = 4 * frameHeight;
                    }
                    break;
                case EmotionState.Hmm:
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;

                    if (NPC.frameCounter % 20 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 7 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                    break;
                case EmotionState.Laugh:
                    if (NPC.frame.Y < 8 * frameHeight)
                        NPC.frame.Y = 8 * frameHeight;

                    if (NPC.frameCounter % 10 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 9 * frameHeight)
                            NPC.frame.Y = 8 * frameHeight;
                    }
                    break;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public static int ChatNumber = 0;

        public override void SetChatButtons(ref string button, ref string button2)
        {
            switch (ChatNumber)
            {
                case 0:
                    button = "Who are you?";
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                EmoteState = EmotionState.Hmm;
                Main.npcChatText = ChitChat();
            }
            else
            {
                ChatNumber++;
                if (ChatNumber > 0)
                    ChatNumber = 0;
            }
        }
        public static string ChitChat()
        {
            return ChatNumber switch
            {
                0 => Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Chat1"),
                _ => "...",
            };
        }
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new();
            EmoteState = EmotionState.Blah;
            if (BasePlayer.HasHelmet(player, ItemID.DevilHorns, true) || BasePlayer.HasHelmet(player, ItemID.DemonHorns, true))
            {
                EmoteState = EmotionState.Laugh;
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue1"));
            }
            if (player.HeldItem.type == ModContent.ItemType<BlindJustice>())
                return Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue2");
            if (BasePlayer.HasAccessory(player, ItemID.AngelHalo, true, true))
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue3"), 2);

            if (player.IsFullTBot())
            {
                EmoteState = EmotionState.Laugh;
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue4TBot"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue5TBot"));
            }
            else
            {
                EmoteState = EmotionState.Laugh;
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue4"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue5"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Noza.Dialogue6"));
            }
            return chat;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D EyesTex = ModContent.Request<Texture2D>(Texture + "_Blink").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 3) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            int Height = EyesTex.Height / 3;
            int y = Height * EyeFrameY;
            Rectangle rect = new(0, y, EyesTex.Width, Height);
            Vector2 origin = new(EyesTex.Width / 2f, Height / 2f);

            if (NPC.frame.Y < 540)
            {
                spriteBatch.Draw(EyesTex, NPC.Center - screenPos - new Vector2(3 * -NPC.spriteDirection, NPC.frame.Y >= 180 && NPC.frame.Y < 360 ? 18 : 20), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            return false;
        }
    }
    internal class TailScarfPhys : IPhysChain
    {
        public Texture2D GetTexture(Mod mod)
        {
            return ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Noza_NPC_Tail").Value;
        }
        public Texture2D GetGlowmaskTexture(Mod mod) => null;

        public int NumberOfSegments => 6;
        public int MaxFrames => 1;
        public int FrameCounterMax => 0;
        public bool Glow => false;
        public bool HasGlowmask => false;
        public int Shader => 0;
        public int GlowmaskShader => 0;

        public Color GetColor(PlayerDrawSet drawInfo, Color baseColour)
        {
            return baseColour;
        }

        public Vector2 AnchorOffset => new(-10, -12);

        public Vector2 OriginOffset(int index) //padding
        {
            switch (index)
            {
                case 0:
                    return new Vector2(0, 0);
                case 1:
                    return new Vector2(0, 0);
                case 2:
                    return new Vector2(-2, 0);
                case 3:
                    return new Vector2(-4, 0);
                case 4:
                    return new Vector2(-6, 0);
                default:
                    return new Vector2(-20, 0);
            }
        }

        public int Length(int index)
        {
            switch (index)
            {
                case 0:
                    return 20;
                case 1:
                    return 14;
                case 2:
                    return 12;
                case 3:
                    return 12;
                case 4:
                    return 12;
                default:
                    return 26;
            }
        }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(NumberOfSegments, 1, NumberOfSegments - 1 - index, 0);
        }

        public Vector2 Force(Player player, int index, int dir, float gravDir, float time, NPC npc = null, Projectile proj = null)
        {
            Vector2 force = new(
                -dir * 0.5f,
                Player.defaultGravity * (0.5f + NumberOfSegments * NumberOfSegments * 0.5f / (1 + index)) // Apply scaling gravity that weakens at the tip of the chain
                );

            if (!Main.gameMenu)
            {
                float windPower = 0.7f * dir * -10;

                // Wave in the wind
                force.X += 25f;
                force.Y -= 2;
                force.Y += (float)(Math.Sin(time * 0.5f * windPower - index * Math.Sign(force.X)) * 0.25f * windPower) * 6f * dir;
            }
            return force;
        }
    }
}