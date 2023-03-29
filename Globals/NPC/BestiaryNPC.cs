using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using Terraria;

namespace Redemption.Globals.NPC
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
        public override void SetBestiary(Terraria.NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            string entry = "Elemental Multipliers";
            float[] elementDmg = npc.GetGlobalNPC<ElementalNPC>().elementDmg;
            for (int i = 0; i < elementDmg.Length - 1; i++)
            {
                if (elementDmg[i] is 1)
                    continue;

                elementDmg[i] = (int)Math.Round(elementDmg[i] * 100);
                elementDmg[i] /= 100;
                if (npc.boss)
                    elementDmg[i] = MathHelper.Clamp(elementDmg[i], .75f, 1.25f);

                if (elementDmg[i] > .9f && elementDmg[i] < 1.1f)
                    continue;

                string s = i switch
                {
                    2 => "Fire: ",
                    3 => "Water: ",
                    4 => "Ice: ",
                    5 => "Earth: ",
                    6 => "Wind: ",
                    7 => "Thunder: ",
                    8 => "Holy: ",
                    9 => "Shadow: ",
                    10 => "Nature: ",
                    11 => "Poison: ",
                    12 => "Blood: ",
                    13 => "Psychic: ",
                    14 => "Celestial: ",
                    _ => "Arcane: ",
                };
                entry += "\n" + s + (elementDmg[i] * 100).ToString() + "%";
            }
            if (entry != "Elemental Multipliers")
                bestiaryEntry.Info.Add(new ElementBestiaryText(entry));
        }
    }
    public class ElementBestiaryText : FlavorTextBestiaryInfoElement, IBestiaryInfoElement
    {
        public ElementBestiaryText(string languageKey) : base(languageKey)
        {
        }

        public new UIElement ProvideUIElement(BestiaryUICollectionInfo info)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
                return base.ProvideUIElement(info);
            return null;
        }
    }
}