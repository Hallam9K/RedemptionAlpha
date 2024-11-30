using BetterDialogue.UI;
using Redemption.BaseExtension;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Friendly.TownNPCs;
using Redemption.UI.Dialect;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;

namespace Redemption.NPCs
{
    public class TalkButton : ChatButton
    {
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Talk");
        public override bool IsActive(NPC npc, Player player) => !RedeGlobalButton.talkActive && npc.IsRedeNPC(ref RedeGlobalButton.redeNPC) && RedeGlobalButton.redeNPC.HasTalkButton();
        public override double Priority => 3.0;
        public override void OnClick(NPC npc, Player player)
        {
            Main.npcChatCornerItem = 0;
            RedeGlobalButton.talkActive = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
    public class ReviveButton : ChatButton
    {
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.RevivialPotion");
        public override bool IsActive(NPC npc, Player player) => !RedeGlobalButton.talkActive && npc.IsRedeNPC(ref RedeGlobalButton.redeNPC) && RedeGlobalButton.redeNPC.HasReviveButton();
        public override double Priority => 50.0;

        public override Color? OverrideColor(NPC npc, Player player)
        {
            if (!player.HasItemInInventoryOrOpenVoidBag(ItemType<RevivalPotion>()))
                return Color.Gray;

            return null;
        }
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (player.ConsumeItem(ItemType<RevivalPotion>(), false, true))
            {
                SoundEngine.PlaySound(SoundID.Item3, npc.position);
                Main.npcChatCornerItem = 0;

                WeightedRandom<string> chat;
                switch (npc.ModNPC)
                {
                    case Daerel daerel:
                        daerel.Revived();
                        chat = new(Main.rand);
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Revived1"));
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Revived2"));
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Daerel.Revived3"));
                        Main.npcChatText = chat;
                        break;
                    case Zephos zephos:
                        zephos.Revived();
                        chat = new(Main.rand);
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Revived1"));
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Revived2"));
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Zephos.Revived3"));
                        Main.npcChatText = chat;
                        break;
                    case TBot tbot:
                        tbot.Revived();
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.TBot.Revived");
                        break;
                }
                return;
            }
            Main.npcChatCornerItem = ItemType<RevivalPotion>();
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.General.NoRevivialPotion");
        }
    }
    public class RequestCruxButton : ChatButton
    {
        public override double Priority => 90.0;
        public override string Text(NPC npc, Player player)
        {
            if (!player.RedemptionAbility().SpiritwalkerActive)
                return "???";

            if (npc.IsRedeNPC(ref RedeGlobalButton.redeNPC))
                return RedeGlobalButton.redeNPC.CruxButtonText(player);
            return Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        }
        public override string Description(NPC npc, Player player) => player.RedemptionAbility().SpiritwalkerActive ? string.Empty : Language.GetTextValue("Mods.Redemption.DialogueBox.RequestCruxDescription");
        public override bool IsActive(NPC npc, Player player) => !RedeGlobalButton.talkActive && npc.IsRedeNPC(ref RedeGlobalButton.redeNPC) && RedeGlobalButton.redeNPC.HasCruxButton(player);
        public override Color? OverrideColor(NPC npc, Player player)
        {
            if (!player.RedemptionAbility().SpiritwalkerActive)
                return Color.Gray;

            float mouseTextColorFactor = Main.mouseTextColor / 255f;
            return new((byte)(180 * mouseTextColorFactor), (byte)(255 * mouseTextColorFactor), (byte)(255 * mouseTextColorFactor), Main.mouseTextColor);
        }
        public override void OnClick(NPC npc, Player player)
        {
            if (!player.RedemptionAbility().SpiritwalkerActive)
                return;

            SoundEngine.PlaySound(SoundID.Chat);
            if (npc.IsRedeNPC(ref RedeGlobalButton.redeNPC))
                RedeGlobalButton.redeNPC.CruxButton(player);
        }

        public static void RequestCrux(NPC npc, Player player, int cruxCard, string noCrux, string cruxGiven)
        {
            if (player.ConsumeItem(ItemType<EmptyCruxCard>()))
            {
                player.QuickSpawnItem(npc.GetSource_Loot(), cruxCard);
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + cruxGiven);
                Main.npcChatCornerItem = cruxCard;
            }
            else
            {
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + noCrux);
                Main.npcChatCornerItem = ItemType<EmptyCruxCard>();
            }
        }
        public static void RequestCrux(NPC npc, Player player, int cruxCard, string noCrux, string cruxGiven, string noOffering, ref bool request, int offeringItem, int stack = 1)
        {
            int offering = player.FindItem(offeringItem);
            if (request && offering >= 0 && player.inventory[offering].stack >= stack)
            {
                if (player.ConsumeItem(ItemType<EmptyCruxCard>()))
                {
                    player.inventory[offering].stack -= stack;
                    if (player.inventory[offering].stack <= 0)
                        player.inventory[offering] = new Item();

                    player.QuickSpawnItem(npc.GetSource_Loot(), cruxCard);
                    if (npc.type == NPCType<SpiritGathicMan>())
                    {
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + cruxGiven, player.Male ? Language.GetTextValue("Mods.Redemption.Dialogue.General.Lad") : Language.GetTextValue("Mods.Redemption.Dialogue.General.Lass"));
                    }
                    else
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + cruxGiven);

                    Main.npcChatCornerItem = cruxCard;
                }
                else
                {
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + noCrux);
                    Main.npcChatCornerItem = ItemType<EmptyCruxCard>();
                }
            }
            else
            {
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + noOffering);
                Main.npcChatCornerItem = offeringItem;
            }
            request = true;
        }
    }
}