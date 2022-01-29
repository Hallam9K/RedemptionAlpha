using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.Items.Placeable.Containers;
using Redemption.Items.Placeable.Furniture.Lab;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Tools.PostML;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.HM.Melee;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Noza, Tamer of Evil");
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 72;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0;
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
        public override bool? CanHitNPC(NPC target) => false;

        public static int ChatNumber = 0;

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = "Cycle Dialogue";
            switch (ChatNumber)
            {
                case 0:
                    button = "AWOOGA";
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
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
                0 => "Wat.",
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
                chat.Add("Nice horns, did your MOM make them for you? WHEHEHEHEHE!");
            }
            if (player.HeldItem.type == ModContent.ItemType<BlindJustice>())
                return "EEK! Get that holy weapon AWAY from me!";
            if (BasePlayer.HasAccessory(player, ItemID.AngelHalo, true, true))
                return "Yuck! What's with that ring over your head? No goody-two-shoes in MY bastion!";

            if (player.IsFullTBot())
            {
                EmoteState = EmotionState.Laugh;
                chat.Add("What is a SILLY LITTLE METAL BUCKET doing in MY bastion!?");
                chat.Add("What even are you? Some sort of bucket cosplayer!? WEHEHEHE!");
            }
            else
            {
                EmoteState = EmotionState.Laugh;
                chat.Add("What is a SILLY LITTLE HUMAN doing in MY bastion!?");
                chat.Add("Look at ITTY BITTY you! WHEHEHEHE! Humans look so STUPID!");
            }
            return chat;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D EyesTex = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Noza_NPC_Blink").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 3) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            int Height = EyesTex.Height / 3;
            int y = Height * EyeFrameY;
            Rectangle rect = new(0, y, EyesTex.Width, Height);
            Vector2 origin = new(EyesTex.Width / 2f, Height / 2f);

            if (NPC.frame.Y < 492)
            {
                spriteBatch.Draw(EyesTex, NPC.Center - screenPos - new Vector2(4 * -NPC.spriteDirection, NPC.frame.Y >= 164 && NPC.frame.Y < 328 ? 18 : 20), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            return false;
        }
    }
}