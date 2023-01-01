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

namespace Redemption.Globals.NPC
{
    public class ElementBestiaryNPC : GlobalNPC // There needs to be a better way to do this
    {
        public override void SetBestiary(Terraria.NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            string entry = "Elemental Multipliers";
            for (int i = 0; i < 14; i++)
            {
                float elementDmg = 1;
                if (npc.type == ModContent.NPCType<Akka>())
                {
                    if (i == 11 || i == 4 || i == 9)
                        elementDmg *= 0.75f;

                    if (i == 10 || i == 2)
                        elementDmg *= 0.9f;

                    if (i == 1)
                        elementDmg *= 1.25f;

                    if (i == 5)
                        elementDmg *= 1.1f;
                }
                if (npc.type == ModContent.NPCType<Ukko>())
                {
                    if (i == 11 || i == 4 || i == 6)
                        elementDmg *= 0.75f;

                    if (i == 10 || i == 2 || i == 5)
                        elementDmg *= 0.9f;

                    if (i == 3)
                        elementDmg *= 1.25f;

                    if (i == 8)
                        elementDmg *= 1.1f;
                }
                if (npc.type == ModContent.NPCType<Erhan>() || npc.type == ModContent.NPCType<ErhanSpirit>())
                {
                    if (i == 12)
                        elementDmg *= 0.9f;
                }
                if (npc.type == ModContent.NPCType<Nebuleus>() || npc.type == ModContent.NPCType<Nebuleus2>() || npc.type == ModContent.NPCType<Nebuleus_Clone>() || npc.type == ModContent.NPCType<Nebuleus2_Clone>())
                {
                    if (i == 13)
                        elementDmg *= 0.75f;

                    if (i == 9)
                        elementDmg *= 0.9f;

                    if (i == 12)
                        elementDmg *= 1.25f;

                    if (i == 8)
                        elementDmg *= 1.1f;
                }
                if (npc.type == ModContent.NPCType<EaglecrestGolem>() || npc.type == ModContent.NPCType<EaglecrestGolem2>() || npc.type == ModContent.NPCType<EaglecrestRockPile>() || npc.type == ModContent.NPCType<EaglecrestRockPile2>())
                {
                    if (i == 4)
                        elementDmg *= 0.75f;
                }
                if (npc.type == ModContent.NPCType<JollyMadman>())
                {
                    if (i == 7)
                        elementDmg *= 2f;
                }
                if (NPCLists.Plantlike.Contains(npc.type))
                {
                    if (i == 1 || i == 5)
                        elementDmg *= 1.25f;

                    if (i == 9)
                        elementDmg *= 0.75f;

                    if (i == 10)
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
                {
                    if (i == 7)
                        elementDmg *= 1.25f;

                    if (i == 8)
                        elementDmg *= 0.8f;
                }
                if (NPCLists.Demon.Contains(npc.type))
                {
                    if (i == 7 || i == 13)
                        elementDmg *= 1.3f;

                    if (i == 1)
                        elementDmg *= 0.5f;

                    if (i == 2 || i == 3)
                        elementDmg *= 1.15f;
                }
                if (NPCLists.Spirit.Contains(npc.type))
                {
                    if (i == 7 || i == 13 || i == 0)
                        elementDmg *= 1.15f;
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (i == 1)
                        elementDmg *= 1.25f;

                    if (i == 3)
                        elementDmg *= 0.75f;

                    if (i == 2)
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Cold.Contains(npc.type))
                {
                    if (i == 1)
                        elementDmg *= 1.25f;

                    if (i == 3)
                        elementDmg *= 0.75f;

                    if (i == 6 || i == 5)
                        elementDmg *= 1.1f;

                    if (i == 10)
                        elementDmg *= 0.9f;
                }
                if (NPCLists.Hot.Contains(npc.type))
                {
                    if (i == 1)
                        elementDmg *= 0.5f;

                    if (i == 3)
                        elementDmg *= 1.25f;

                    if (i == 2 || i == 5 || i == 10)
                        elementDmg *= 1.1f;
                }
                if (NPCLists.Wet.Contains(npc.type))
                {
                    if (i == 1)
                        elementDmg *= 0.75f;

                    if (i == 3 || i == 10)
                        elementDmg *= 1.25f;

                    if (i == 2)
                        elementDmg *= 0.5f;
                }
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (i == 1)
                        elementDmg *= 1.15f;

                    if (i == 3)
                        elementDmg *= 0.7f;

                    if (i == 11)
                        elementDmg *= 1.25f;

                    if (i == 10)
                        elementDmg *= 0.25f;
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (i == 11 || i == 10)
                        elementDmg *= 0.75f;

                    if (i == 6)
                        elementDmg *= 1.1f;

                    if (i == 2)
                        elementDmg *= 1.35f;
                }
                if (!NPCLists.Inorganic.Contains(npc.type))
                {
                    if (i == 11)
                        elementDmg *= 1.1f;

                    if (i == 10)
                        elementDmg *= 1.05f;
                }
                if (NPCLists.Hallowed.Contains(npc.type))
                {
                    if (i == 13)
                        elementDmg *= 0.9f;

                    if (i == 7)
                        elementDmg *= 0.5f;

                    if (i == 8)
                        elementDmg *= 1.25f;
                }
                if (NPCLists.Dark.Contains(npc.type))
                {
                    if (i == 7)
                        elementDmg *= 1.15f;

                    if (i == 9)
                        elementDmg *= 0.9f;

                    if (i == 8)
                        elementDmg *= 0.75f;
                }
                if (NPCLists.Blood.Contains(npc.type))
                {
                    if (i == 7 || i == 3 || i == 10)
                        elementDmg *= 1.1f;

                    if (i == 8)
                        elementDmg *= 0.9f;

                    if (i == 11)
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