using BetterDialogue.UI;
using Redemption.UI.Dialect;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Redemption.NPCs
{
    public abstract class TalkButtonBase : ChatButton
    {
        protected abstract int YOffset { get; }
        protected abstract bool LeftSide { get; }
        protected abstract string DialogueType { get; }
        protected abstract bool VisibleRequirement { get; }
        protected abstract int NPCType { get; }

        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + (LeftSide ? 0 : 300);
            position.Y += 56 + (46 * YOffset);
        }
        public override string Text(NPC npc, Player player) => VisibleRequirement ? Language.GetTextValue("Mods.Redemption.DialogueBox." + DialogueType) : "???";
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType && RedeGlobalButton.talkActive;

        public override Color? OverrideColor(NPC npc, Player player) => VisibleRequirement ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            if (!VisibleRequirement)
                return;
            OnSafeClick(npc, player);
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + DialogueType);
        }
        public virtual void OnSafeClick(NPC npc, Player player) { }
    }
}