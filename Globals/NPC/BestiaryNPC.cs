using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
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
    public class BestiaryNPC : GlobalNPC // There needs to be a better way to do this
    {
        public override void SetBestiary(Terraria.NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            string entry = "Elemental Multipliers";
            for (int i = 0; i < 14; i++)
            {
                float elementDmg = 1;
                if (npc.type == ModContent.NPCType<Akka>())
                {
                    if (i is 11 or 4 or 9)
                        elementDmg *= 0.75f;

                    if (i is 10 or 2)
                        elementDmg *= 0.9f;

                    if (i is 1)
                        elementDmg *= 1.25f;

                    if (i is 5)
                        elementDmg *= 1.1f;
                }
                if (npc.type == ModContent.NPCType<Ukko>())
                {
                    if (i is 11 or 4 or 6)
                        elementDmg *= 0.75f;

                    if (i is 10 or 2 or 5)
                        elementDmg *= 0.9f;

                    if (i is 3)
                        elementDmg *= 1.25f;

                    if (i is 8)
                        elementDmg *= 1.1f;
                }
                if (npc.type == ModContent.NPCType<Erhan>() || npc.type == ModContent.NPCType<ErhanSpirit>())
                {
                    if (i is 12)
                        elementDmg *= 0.9f;
                }
                if (npc.type == ModContent.NPCType<Nebuleus>() || npc.type == ModContent.NPCType<Nebuleus2>() || npc.type == ModContent.NPCType<Nebuleus_Clone>() || npc.type == ModContent.NPCType<Nebuleus2_Clone>())
                {
                    if (i is 13)
                        elementDmg *= 0.75f;

                    if (i is 9)
                        elementDmg *= 0.9f;

                    if (i is 12)
                        elementDmg *= 1.25f;

                    if (i is 8)
                        elementDmg *= 1.1f;
                }
                if (npc.type == ModContent.NPCType<EaglecrestGolem>() || npc.type == ModContent.NPCType<EaglecrestGolem2>() || npc.type == ModContent.NPCType<EaglecrestRockPile>() || npc.type == ModContent.NPCType<EaglecrestRockPile2>())
                {
                    if (i is 4)
                        elementDmg *= 0.75f;
                }
                if (npc.type == ModContent.NPCType<JollyMadman>())
                {
                    if (i is 7)
                        elementDmg *= 2f;
                }
                if (NPCLists.Plantlike.Contains(npc.type))
                {
                    if (i is 1 or 5)
                        elementDmg *= 1.25f;

                    if (i is 9)
                        elementDmg *= 0.75f;

                    if (i is 10)
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
                {
                    if (i is 7)
                        elementDmg *= 1.25f;

                    if (i is 8)
                        elementDmg *= 0.8f;
                }
                if (NPCLists.Demon.Contains(npc.type))
                {
                    if (i is 7 or 13)
                        elementDmg *= 1.3f;

                    if (i is 1)
                        elementDmg *= 0.5f;

                    if (i is 2 or 3)
                        elementDmg *= 1.15f;
                }
                if (NPCLists.Spirit.Contains(npc.type))
                {
                    if (i is 7 or 13 or 0)
                        elementDmg *= 1.15f;
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (i is 1)
                        elementDmg *= 1.25f;

                    if (i is 3)
                        elementDmg *= 0.75f;

                    if (i is 2)
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Cold.Contains(npc.type))
                {
                    if (i is 1)
                        elementDmg *= 1.25f;

                    if (i is 3)
                        elementDmg *= 0.75f;

                    if (i is 6 or 5)
                        elementDmg *= 1.1f;

                    if (i is 10)
                        elementDmg *= 0.9f;
                }
                if (NPCLists.Hot.Contains(npc.type))
                {
                    if (i is 1)
                        elementDmg *= 0.5f;

                    if (i is 3)
                        elementDmg *= 1.25f;

                    if (i is 2 or 5 or 10)
                        elementDmg *= 1.1f;
                }
                if (NPCLists.Wet.Contains(npc.type))
                {
                    if (i is 1)
                        elementDmg *= 0.75f;

                    if (i is 3 or 10)
                        elementDmg *= 1.25f;

                    if (i is 2)
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (i is 1)
                        elementDmg *= 1.15f;

                    if (i is 3)
                        elementDmg *= 0.7f;

                    if (i is 11)
                        elementDmg *= 1.25f;

                    if (i is 10)
                        elementDmg *= 0.25f;
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (i is 11 or 10)
                        elementDmg *= 0.75f;

                    if (i is 6)
                        elementDmg *= 1.1f;

                    if (i is 2)
                        elementDmg *= 1.35f;
                }
                if (!NPCLists.Inorganic.Contains(npc.type))
                {
                    if (i is 11)
                        elementDmg *= 1.1f;

                    if (i is 10)
                        elementDmg *= 1.05f;
                }
                if (NPCLists.Hallowed.Contains(npc.type))
                {
                    if (i is 13)
                        elementDmg *= 0.9f;

                    if (i is 7)
                        elementDmg *= 0.5f;

                    if (i is 8)
                        elementDmg *= 1.25f;
                }
                if (NPCLists.Dark.Contains(npc.type))
                {
                    if (i is 7)
                        elementDmg *= 1.15f;

                    if (i is 9)
                        elementDmg *= 0.9f;

                    if (i is 8)
                        elementDmg *= 0.75f;
                }
                if (NPCLists.Blood.Contains(npc.type))
                {
                    if (i is 7 or 3 or 10)
                        elementDmg *= 1.1f;

                    if (i is 8)
                        elementDmg *= 0.9f;

                    if (i is 11)
                        elementDmg *= 0.75f;
                }

                elementDmg = (int)Math.Round(elementDmg * 100);
                elementDmg /= 100;
                if (npc.boss)
                    elementDmg = MathHelper.Clamp(elementDmg, .75f, 1.25f);

                if (elementDmg > .9f && elementDmg < 1.1f)
                    continue;

                string s = i switch
                {
                    1 => "Fire: ",
                    2 => "Water: ",
                    3 => "Ice: ",
                    4 => "Earth: ",
                    5 => "Wind: ",
                    6 => "Thunder: ",
                    7 => "Holy: ",
                    8 => "Shadow: ",
                    9 => "Nature: ",
                    10 => "Poison: ",
                    11 => "Blood: ",
                    12 => "Psychic: ",
                    13 => "Celestial: ",
                    _ => "Arcane: ",
                };
                entry += "\n" + s + (elementDmg * 100).ToString() + "%";
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