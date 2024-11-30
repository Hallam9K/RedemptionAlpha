using BetterDialogue.UI;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.UI.Dialect;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Redemption.NPCs.Friendly.TownNPCs
{
    public class AdviceButton_Encounters : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Advice.CatergoryEncounters");
        public override bool IsActive(NPC npc, Player player) => (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>()) && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 1;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
    public class AdviceButton_Locations : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Advice.CatergoryLocations");
        public override bool IsActive(NPC npc, Player player) => (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>()) && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 2;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
    public class AdviceButton_Elements : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + (46 * 2);
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Advice.CatergoryElements");
        public override bool IsActive(NPC npc, Player player) => (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>()) && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 3;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
    public class AdviceButton_Tips : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + (46 * 3);
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Advice.CatergoryTips");
        public override bool IsActive(NPC npc, Player player) => (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>()) && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            RedeGlobalButton.talkID = 4;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
    public abstract class AdviceButtonBase : ChatButton
    {
        protected abstract int YOffset { get; }
        protected abstract int AdviceType { get; }
        protected abstract string AdviceName { get; }
        protected abstract int TalkID { get; }
        protected abstract bool ForceHidden { get; }
        protected abstract int ArgsID { get; }

        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * YOffset);
        }
        public override string Text(NPC npc, Player player)
        {
            bool locked;
            if (AdviceType >= RedeQuest.adviceUnlocked.Length)
                locked = false;
            else
                locked = !RedeQuest.adviceUnlocked[AdviceType];
            return locked || ForceHidden ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.Advice." + AdviceName);
        }
        public override bool IsActive(NPC npc, Player player)
        {
            return (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>()) && RedeGlobalButton.talkActive && RedeGlobalButton.talkID == TalkID;
        }
        public override Color? OverrideColor(NPC npc, Player player)
        {
            if (ForceHidden)
                return Color.Gray;

            bool locked;
            if (AdviceType >= RedeQuest.adviceUnlocked.Length)
                locked = false;
            else
                locked = !RedeQuest.adviceUnlocked[AdviceType];

            return locked || RedeQuest.adviceSeen[AdviceType] ? Color.Gray : null;
        }
        public override void OnClick(NPC npc, Player player)
        {
            bool locked;
            if (AdviceType >= RedeQuest.adviceUnlocked.Length)
                locked = false;
            else
                locked = !RedeQuest.adviceUnlocked[AdviceType];

            if (locked || ForceHidden)
                return;
            RedeQuest.adviceSeen[AdviceType] = true;
            SoundEngine.PlaySound(SoundID.Chat);

            bool isDaerel = npc.type == NPCType<Daerel>();
            object arg0 = null;
            object arg1 = null;
            switch (ArgsID)
            {
                case 1:
                    int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
                    if (DryadID >= 0)
                        arg0 = Main.npc[DryadID].GivenName;
                    else
                        arg0 = "whats-her-name";
                    break;
                case 2:
                    int FallenID = NPC.FindFirstNPC(NPCType<Fallen>());
                    if (FallenID >= 0)
                        arg0 = Main.npc[FallenID].GivenName;
                    else
                        arg0 = "Fallen";
                    break;
                case 3:
                    arg0 = ElementID.ArcaneS;
                    arg1 = ElementID.HolyS;
                    break;
                case 4:
                    arg0 = ElementID.HolyS;
                    arg1 = ElementID.ShadowS;
                    break;
                case 5:
                    if (isDaerel)
                        arg0 = ElementID.IceS;
                    else
                    {
                        arg0 = Main.LocalPlayer.Male ? "" : " (wink wink)";
                        arg1 = ElementID.IceS;
                    }
                    break;
                case 6:
                    if (isDaerel)
                    {
                        arg0 = ElementID.NatureS;
                        arg1 = ElementID.ShadowS;
                    }
                    else
                    {
                        arg0 = ElementID.FireS;
                        arg1 = ElementID.IceS;
                    }
                    break;
            }

            string wayfarer = isDaerel ? "Daerel" : "Zephos";
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue." + wayfarer + ".Advice." + AdviceName, arg0, arg1);
        }
    }
    public class AdviceButton_Erhan : AdviceButtonBase
    {
        protected override int YOffset => 0;
        protected override int AdviceType => (int)RedeQuest.Advice.Erhan;
        protected override string AdviceName => "Erhan";
        protected override int TalkID => 1;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_ForestNymph : AdviceButtonBase
    {
        protected override int YOffset => 1;
        protected override int AdviceType => (int)RedeQuest.Advice.ForestNymph;
        protected override string AdviceName => "ForestNymph";
        protected override int TalkID => 1;
        protected override int ArgsID => 1;
        protected override bool ForceHidden => Hidden();
        static bool Hidden()
        {
            int DryadID = NPC.FindFirstNPC(NPCID.Dryad);
            return !RedeQuest.adviceSeen[(int)RedeQuest.Advice.ForestNymph] && DryadID == -1;
        }
    }
    public class AdviceButton_EaglecrestGolem : AdviceButtonBase
    {
        protected override int YOffset => 2;
        protected override int AdviceType => (int)RedeQuest.Advice.EaglecrestGolem;
        protected override string AdviceName => "EaglecrestGolem";
        protected override int TalkID => 1;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_Androids : AdviceButtonBase
    {
        protected override int YOffset => 3;
        protected override int AdviceType => (int)RedeQuest.Advice.Androids;
        protected override string AdviceName => "Androids";
        protected override int TalkID => 1;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_UkkoEye : AdviceButtonBase
    {
        protected override int YOffset => 4;
        protected override int AdviceType => (int)RedeQuest.Advice.UkkoEye;
        protected override string AdviceName => "UkkoEye";
        protected override int TalkID => 1;
        protected override int ArgsID => 2;
        protected override bool ForceHidden => Hidden();
        static bool Hidden()
        {
            int FallenID = NPC.FindFirstNPC(NPCType<Fallen>());
            return !RedeQuest.adviceSeen[(int)RedeQuest.Advice.UkkoEye] && (FallenID == -1 || !Main.LocalPlayer.HasItemInAnyInventory(ItemType<GolemEye>()));
        }
    }
    public class AdviceButton_StarSerpent : AdviceButtonBase
    {
        protected override int YOffset => 5;
        protected override int AdviceType => (int)RedeQuest.Advice.StarSerpent;
        protected override string AdviceName => "StarSerpent";
        protected override int TalkID => 1;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }

    public class AdviceButton_Chalice : AdviceButtonBase
    {
        protected override int YOffset => 0;
        protected override int AdviceType => (int)RedeQuest.Advice.Chalice;
        protected override string AdviceName => "Chalice";
        protected override int TalkID => 2;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_Fool : AdviceButtonBase
    {
        protected override int YOffset => 1;
        protected override int AdviceType => (int)RedeQuest.Advice.Fool;
        protected override string AdviceName => "Fool";
        protected override int TalkID => 2;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_UGPortal : AdviceButtonBase
    {
        protected override int YOffset => 2;
        protected override int AdviceType => (int)RedeQuest.Advice.UGPortal;
        protected override string AdviceName => "UGPortal";
        protected override int TalkID => 2;
        protected override int ArgsID => 2;
        protected override bool ForceHidden => Hidden();
        static bool Hidden()
        {
            int FallenID = NPC.FindFirstNPC(NPCType<Fallen>());
            return !RedeQuest.adviceSeen[(int)RedeQuest.Advice.UGPortal] && FallenID == -1;
        }
    }

    public class AdviceButton_Elements2 : AdviceButtonBase
    {
        protected override int YOffset => 0;
        protected override int AdviceType => (int)RedeQuest.Advice.Elements;
        protected override string AdviceName => "Elements";
        protected override int TalkID => 3;
        protected override int ArgsID => 6;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_Spirits : AdviceButtonBase
    {
        protected override int YOffset => 1;
        protected override int AdviceType => (int)RedeQuest.Advice.Spirits;
        protected override string AdviceName => "Spirits";
        protected override int TalkID => 3;
        protected override int ArgsID => 3;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_Undead : AdviceButtonBase
    {
        protected override int YOffset => 2;
        protected override int AdviceType => (int)RedeQuest.Advice.Undead;
        protected override string AdviceName => "Undead";
        protected override int TalkID => 3;
        protected override int ArgsID => 4;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_Slimes : AdviceButtonBase
    {
        protected override int YOffset => 3;
        protected override int AdviceType => (int)RedeQuest.Advice.Slimes;
        protected override string AdviceName => "Slimes";
        protected override int TalkID => 3;
        protected override int ArgsID => 5;
        protected override bool ForceHidden => false;
    }

    public class AdviceButton_Insects : AdviceButtonBase
    {
        protected override int YOffset => 0;
        protected override int AdviceType => (int)RedeQuest.Advice.Insects;
        protected override string AdviceName => "Insects";
        protected override int TalkID => 4;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_Invisibility : AdviceButtonBase
    {
        protected override int YOffset => 1;
        protected override int AdviceType => (int)RedeQuest.Advice.Invisibility;
        protected override string AdviceName => "Invisibility";
        protected override int TalkID => 4;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_GuardPoints : AdviceButtonBase
    {
        protected override int YOffset => 2;
        protected override int AdviceType => (int)RedeQuest.Advice.GuardPoints;
        protected override string AdviceName => "GuardPoints";
        protected override int TalkID => 4;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
    public class AdviceButton_DirtyWound : AdviceButtonBase
    {
        protected override int YOffset => 3;
        protected override int AdviceType => (int)RedeQuest.Advice.DirtyWound;
        protected override string AdviceName => "DirtyWound";
        protected override int TalkID => 4;
        protected override int ArgsID => 0;
        protected override bool ForceHidden => false;
    }
}