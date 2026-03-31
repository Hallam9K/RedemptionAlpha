using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Redemption.Globals.NPCs
{
    internal class CustomCollectionInfoProvider : IBestiaryUICollectionInfoProvider
    {
        private readonly string _persistentIdentifierToCheck;
        private readonly bool _quickUnlock;
        private readonly float _maxKills;

        public CustomCollectionInfoProvider(string persistentId, bool quickUnlock, float maxKills = 50)
        {
            _persistentIdentifierToCheck = persistentId;
            _quickUnlock = quickUnlock;
            _maxKills = maxKills;
        }

        public BestiaryUICollectionInfo GetEntryUICollectionInfo()
        {
            BestiaryEntryUnlockState unlockStateByKillCount = GetUnlockStateByKillCount(Main.BestiaryTracker.Kills.GetKillCount(_persistentIdentifierToCheck), _quickUnlock, _maxKills);
            BestiaryUICollectionInfo result = default;
            result.UnlockState = unlockStateByKillCount;
            return result;
        }

        public static BestiaryEntryUnlockState GetUnlockStateByKillCount(int killCount, bool quickUnlock, float maxKills)
        {
            if ((quickUnlock && killCount > 0) || killCount >= maxKills)
                return BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
            else if (killCount >= maxKills / 2)
                return BestiaryEntryUnlockState.CanShowDropsWithoutDropRates_3;
            else if (killCount >= maxKills / 5)
                return BestiaryEntryUnlockState.CanShowStats_2;
            else if (killCount >= maxKills / 50)
                return BestiaryEntryUnlockState.CanShowPortraitOnly_1;

            return BestiaryEntryUnlockState.NotKnownAtAll_0;
        }
    }
    public class BestiaryNPC : GlobalNPC
    {
        private static readonly List<int> _wasSeen = new();
        public static void ScanWorldForFinds(Terraria.NPC npc)
        {
            if (_wasSeen.Contains(npc.netID))
                return;

            Rectangle hitbox = npc.Hitbox;
            Rectangle playerHitbox = Main.LocalPlayer.HitboxForBestiaryNearbyCheck;
            if (hitbox.Intersects(playerHitbox))
            {
                _wasSeen.Add(npc.netID);
                Main.BestiaryTracker.Sights.SetWasSeenDirectly(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npc.type]);
            }
        }

        private class ElementalFlavorTextElement(NPC npc) : IBestiaryInfoElement, IBestiaryPrioritizedElement, ICategorizedBestiaryInfoElement
        {
            private NPC _npc = npc;

            public float OrderPriority => 0.9f;

            public UIBestiaryEntryInfoPage.BestiaryInfoCategory ElementCategory => UIBestiaryEntryInfoPage.BestiaryInfoCategory.Stats;

            public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
            {
                if (info.UnlockState == BestiaryEntryUnlockState.NotKnownAtAll_0)
                    return null;

                if (RedeConfigServer.Instance.ElementDisable)
                    return null;

                ElementalNPC.SetElementalMultipliers(_npc, ref _npc.GetGlobalNPC<ElementalNPC>().elementDmg);

                string entry = "";
                float[] elementDmg = _npc.GetGlobalNPC<ElementalNPC>().elementDmg;
                bool sameColumn = false;
                for (int i = 0; i < elementDmg.Length - 1; i++)
                {
                    if (elementDmg[i] is 1)
                        continue;

                    elementDmg[i] = (int)Math.Round(elementDmg[i] * 100);
                    if (_npc.boss && !_npc.GetGlobalNPC<ElementalNPC>().uncappedBossMultiplier)
                        elementDmg[i] = MathHelper.Clamp(elementDmg[i], 75, 125);

                    if (elementDmg[i] > 90 && elementDmg[i] < 110)
                        continue;

                    string s = i switch
                    {
                        2 => "[i:Redemption/Fire] ",
                        3 => "[i:Redemption/Water] ",
                        4 => "[i:Redemption/Ice] ",
                        5 => "[i:Redemption/Earth] ",
                        6 => "[i:Redemption/Wind] ",
                        7 => "[i:Redemption/Thunder] ",
                        8 => "[i:Redemption/Holy] ",
                        9 => "[i:Redemption/Shadow] ",
                        10 => "[i:Redemption/Nature] ",
                        11 => "[i:Redemption/Poison] ",
                        12 => "[i:Redemption/Blood] ",
                        13 => "[i:Redemption/Psychic] ",
                        14 => "[i:Redemption/Cosmic] ",
                        _ => "[i:Redemption/Arcane] ",
                    };
                    elementDmg[i] -= 100;
                    elementDmg[i] *= -1;
                    string plus = elementDmg[i] < 0 ? "" : "+";

                    string colorCodeR = "[c/808080:";
                    if (elementDmg[i] > 0)
                        colorCodeR = "[c/BC7777:";
                    else if (elementDmg[i] < 0)
                        colorCodeR = "[c/74B874:";

                    if (sameColumn)
                        entry += "     " + s + colorCodeR + plus + ((int)(elementDmg[i])).ToString() + "%]";
                    else
                        entry += "\n" + s + colorCodeR + plus + ((int)(elementDmg[i])).ToString() + "%]";
                    sameColumn = !sameColumn;
                }
                if (string.IsNullOrEmpty(entry) || entry == "\n")
                    return null;

                int textHeight = (int)(FontAssets.MouseText.Value.MeasureString(entry).Y);

                UIPanel backPanel = new(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Panel"), null, customBarSize: 7)
                {
                    IgnoresMouseInteraction = true,
                    Width = StyleDimension.FromPixelsAndPercent(-11f, 1f),
                    Height = StyleDimension.FromPixels((textHeight * .8f) + 10),
                    BackgroundColor = new Color(43, 56, 101),
                    BorderColor = Color.Transparent,
                    Left = StyleDimension.FromPixels(-8f),
                    HAlign = 1f
                };
                backPanel.SetPadding(0f);

                UIText elementalFlavorTextElement = new(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Multiplier"), .9f)
                {
                    HAlign = 0.5f,
                    VAlign = 0f,
                    MarginTop = 10,
                    TextColor = Color.LightGoldenrodYellow
                };
                backPanel.Append(elementalFlavorTextElement);
                UIText elementalFlavorTextElement2 = new(entry, .8f)
                {
                    HAlign = 0.5f,
                    VAlign = 0f,
                    MarginTop = 10,
                    TextColor = Color.White
                };
                backPanel.Append(elementalFlavorTextElement2);

                return backPanel;
            }
        }

        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.Add(new ElementalFlavorTextElement(npc));
        }
    }
}