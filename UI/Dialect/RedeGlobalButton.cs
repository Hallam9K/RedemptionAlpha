using BetterDialogue.UI;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Friendly.TownNPCs;
using Redemption.NPCs.Minibosses.Calavia;
using Redemption.UI.ChatUI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Redemption.UI.Dialect
{
    public class RedeGlobalButton : GlobalChatButton
    {
        public static ModRedeNPC redeNPC = null;
        public static bool talkActive;
        public static int talkID;
        public override void ModifyText(ChatButton chatButton, NPC npc, Player player, ref string buttonText)
        {
            if (npc is null)
                return;

            if (chatButton == ChatButton.Exit && talkActive)
                buttonText = Language.GetTextValue("Mods.Redemption.DialogueBox.Back");
            if (npc.type == NPCType<KS3Sitting>() && chatButton == ChatButton.Shop)
                buttonText = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.9");
        }
        public override void ModifyColor(ChatButton chatButton, NPC npc, Player player, ref Color buttonTextColor)
        {
            if (npc is null)
                return;

            if (npc.type == NPCType<KS3Sitting>() && chatButton == ChatButton.Shop && RedeQuest.slayerRep < 4)
                buttonTextColor = Color.Gray;
        }
        public override bool PreClick(ChatButton chatButton, NPC npc, Player player)
        {
            if (npc is null)
                return base.PreClick(chatButton, npc, player);

            if (chatButton == ChatButton.Exit && talkActive)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                talkID = 0;
                talkActive = false;
                return false;
            }
            if (npc.type == NPCType<KS3Sitting>() && chatButton == ChatButton.Shop && RedeQuest.slayerRep < 4)
            {
                Main.npcChatCornerItem = 0;
                SoundEngine.PlaySound(SoundID.Chat);
                if (RedeQuest.slayerRep == 2)
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat9B");
                else if (RedeQuest.slayerRep == 3)
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat9C");
                else
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat9");
                return false;
            }
            return base.PreClick(chatButton, npc, player);
        }
        public override void OnClick(ChatButton chatButton, NPC npc, Player player)
        {
            if (npc is null)
                return;

            if (npc.type == NPCType<TBot>() && chatButton == ChatButton.Shop)
            {
                if (Main.hardMode && !TBot.warheadKnown)
                {
                    TBot.warheadKnown = true;
                    DialogueChain chain = new();
                    chain.Add(new(npc, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Warhead1"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                         .Add(new(npc, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Warhead2"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                         .Add(new(npc, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Warhead3"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                         .Add(new(npc, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Warhead4"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                         .Add(new(npc, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Warhead5"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                         .Add(new(npc, Language.GetTextValue("Mods.Redemption.Cutscene.TBotIntro.Warhead6"), Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true));
                    ChatUI.ChatUI.Visible = true;
                    ChatUI.ChatUI.Add(chain);

                    npc.netUpdate = true;
                }
            }
            else if (npc.type == NPCID.Nurse && chatButton == ChatButton.NurseHeal && player.RedemptionRad().radiationLevel >= 1)
            {
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Nurse.Radiation");
                int AdamID = NPC.FindFirstNPC(NPCType<TBot>());
                if (AdamID >= 0)
                    Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Nurse.Radiation2", Main.npc[AdamID].GivenName);
            }
        }
        public override bool? IsActive(ChatButton chatButton, NPC npc, Player player)
        {
            if (chatButton != ChatButton.Exit && npc.Redemption().HideAllButtons)
                return false;
            if (chatButton == ChatButton.TownNPCHappiness && Main.LocalPlayer.currentShoppingSettings.HappinessReport == string.Empty)
                return false;
            if (chatButton == ChatButton.Shop || chatButton == ChatButton.TownNPCHappiness)
            {
                if (talkActive)
                    return false;
                if ((npc.type == NPCType<Zephos>() || npc.type == NPCType<Daerel>()) && RedeQuest.wayfarerVars[0] < 4)
                    return false;
                if (npc.type == NPCType<ForestNymph_Friendly>() || npc.type == NPCType<Calavia_NPC>())
                    return false;
                if (npc.type == NPCType<Newb>() && RedeBossDowned.downedNebuleus)
                    return false;
            }
            if (chatButton == ChatButton.Shop && npc.ModNPC is TreebarkDryad && RedeBossDowned.downedTreebark)
                return false;
            if (chatButton != ChatButton.Exit && chatButton is not ReviveButton && npc.IsRedeNPC(ref redeNPC) && redeNPC.HasReviveButton())
                return false;
            return null;
        }
    }
}