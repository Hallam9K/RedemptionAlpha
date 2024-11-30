using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.UI.Dialect;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Lab.Volt
{
    public class ProtectorVolt_NPC : ModRedeNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Protector Volt");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 70;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0;

            DialogueBoxStyle = LIDEN;
        }
        public override bool HasTalkButton() => true;
        public override bool HasLeftHangingButton(Player player) => RedeGlobalButton.talkActive;
        public override bool HasRightHangingButton(Player player) => RedeGlobalButton.talkActive && RedeGlobalButton.talkID != 0;
        public override HangingButtonParams LeftHangingButton(Player player) => new(4, true, -2);
        public override HangingButtonParams RightHangingButton(Player player)
        {
            int boxNum = RedeGlobalButton.talkID switch
            {
                2 => 5,
                3 or 4 => 4,
                _ => 2,
            };
            return new(boxNum, false, -2);
        }

        public override bool UsesPartyHat() => false;
        public override bool CanChat() => true;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ >= 20)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public override void AI()
        {
            NPC.spriteDirection = 1;
        }
        private readonly float gunRot = 4.9742f;
        public override string GetChat()
        {
            Player player = Main.LocalPlayer;
            WeightedRandom<string> chat = new(Main.rand);

            if (BasePlayer.HasChestplate(player, ItemType<AndroidArmour>(), true) && BasePlayer.HasLeggings(player, ItemType<AndroidPants>(), true))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Bot1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Bot2"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Bot3"));
            }
            else
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Normal1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Normal2"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Normal3"));
            }
            if (BasePlayer.HasHelmet(player, ItemType<VoltHead>(), true))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Volt1"), 3);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Volt2"), 3);
            }
            if (BasePlayer.HasHelmet(player, ItemType<AdamHead>(), true))
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Adam1"), 3);
            if (BasePlayer.HasChestplate(player, ItemType<JanitorOutfit>(), true) && BasePlayer.HasLeggings(player, ItemType<JanitorPants>(), true))
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Janitor1"), 3);
            if (player.IsFullTBot())
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Volt.TBot1"), 2);
            return chat;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D GunTex = Request<Texture2D>(Texture + "_Extra").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(GunTex, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, GunTex.Height)), NPC.GetAlpha(drawColor), gunRot, new Vector2(GunTex.Width / 2f, GunTex.Height / 2f), NPC.scale, effects, 0f);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
    public class ChallengeButton_Volt : ChatButton
    {
        public override double Priority => 100.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.20");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && !RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => new((byte)(255f * Main.mouseTextColor / 255f), (byte)(150f * Main.mouseTextColor / 255f), (byte)(0f * Main.mouseTextColor / 255f), Main.mouseTextColor);
        public override void OnClick(NPC npc, Player player)
        {
            Main.CloseNPCChatOrSign();
            npc.Transform(NPCType<ProtectorVolt>());
        }
    }
    public class WhoAreYouButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.1");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 1;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Dialogue1");
        }
    }
    public class OtherTBotsButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.4");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 2;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Dialogue2");
        }
    }
    public class GirusButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + (46 * 2);
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.10");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 3;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Dialogue3");
        }
    }
    public class ThisPlaceButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + (46 * 3);
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.15");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 4;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.Dialogue4");
        }
    }
    public class WhatDoYouDoHereButton_Volt : ChatButton
    {
        public static bool clicked;
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.2");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 1;
        public override void OnClick(NPC npc, Player player)
        {
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue1");
        }
    }
    public class CoworkersButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => !WhatDoYouDoHereButton_Volt.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.3");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 1;
        public override Color? OverrideColor(NPC npc, Player player) => !WhatDoYouDoHereButton_Volt.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!WhatDoYouDoHereButton_Volt.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue2");
        }
    }
    public class TheAlphaButton_Volt : ChatButton
    {
        public static bool clicked;
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.5");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 2;
        public override void OnClick(NPC npc, Player player)
        {
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue3");
        }
    }
    public class AdamButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => !TheAlphaButton_Volt.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.6");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 2;
        public override Color? OverrideColor(NPC npc, Player player) => !TheAlphaButton_Volt.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!TheAlphaButton_Volt.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            string s2 = player.IsFullTBot() ? Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue4C") : Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue4B");
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue4") + s2 + Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue4D");
        }
    }
    public class DeadlyGroupButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 2);
        }
        public override string Text(NPC npc, Player player) => !TheAlphaButton_Volt.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.7");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 2;
        public override Color? OverrideColor(NPC npc, Player player) => !TheAlphaButton_Volt.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!TheAlphaButton_Volt.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue5");
        }
    }
    public class HumanButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 3);
        }
        public override string Text(NPC npc, Player player) => !TheAlphaButton_Volt.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.8");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 2;
        public override Color? OverrideColor(NPC npc, Player player) => !TheAlphaButton_Volt.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!TheAlphaButton_Volt.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue6");
        }
    }
    public class ScavengersButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 4);
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.9");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 2;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue7");
        }
    }
    public class WhyFollowGirusButton_Volt : ChatButton
    {
        public static bool clicked;
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.11");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 3;
        public override void OnClick(NPC npc, Player player)
        {
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue8");
        }
    }
    public class UnluckyOnesButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => !WhyFollowGirusButton_Volt.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.12");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 3;
        public override Color? OverrideColor(NPC npc, Player player) => !WhyFollowGirusButton_Volt.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!WhyFollowGirusButton_Volt.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue9");
        }
    }
    public class WhoCreatedYouButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 2);
        }
        public override string Text(NPC npc, Player player) => !WhyFollowGirusButton_Volt.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.13");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 3;
        public override Color? OverrideColor(NPC npc, Player player) => !WhyFollowGirusButton_Volt.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!WhyFollowGirusButton_Volt.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue10");
        }
    }
    public class HowDidGirusFreeYouButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 3);
        }
        public override string Text(NPC npc, Player player) => !WhyFollowGirusButton_Volt.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.14");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 3;
        public override Color? OverrideColor(NPC npc, Player player) => !WhyFollowGirusButton_Volt.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!WhyFollowGirusButton_Volt.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue11");
        }
    }
    public class WhySoManyInfectedButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.16");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 4;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue12");
        }
    }
    public class JanitorButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.17");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 4;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue13");
        }
    }
    public class MACEButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 2);
        }
        public override string Text(NPC npc, Player player) => !RedeBossDowned.downedMACE ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.18");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 4;
        public override Color? OverrideColor(NPC npc, Player player) => !RedeBossDowned.downedMACE ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!RedeBossDowned.downedMACE)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue14");
        }
    }
    public class BottomOfLabButton_Volt : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 3);
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Volt.19");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<ProtectorVolt_NPC>() && RedeGlobalButton.talkActive && RedeGlobalButton.talkID is 4;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Volt.SubDialogue15");
        }
    }
}