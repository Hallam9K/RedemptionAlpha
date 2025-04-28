using Redemption.BaseExtension;
using Redemption.Globals.Player;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Globals.Items
{
    public class Accessories_GlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
            return lateInstantiation && item.accessory;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            if (tooltipLocation != -1)
            {
                switch (item.type)
                {
                    case ItemID.AnkletoftheWind:
                        TooltipLine elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 15, ElementID.WindS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.CelestialStone:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 8, ElementID.CelestialS));
                        TooltipLine elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 8, ElementID.CelestialS));
                        tooltips.Insert(tooltipLocation + 3, elemLine);
                        tooltips.Insert(tooltipLocation + 3, elemLine2);
                        break;
                    case ItemID.CelestialShell:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 10, ElementID.CelestialS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.CelestialS));
                        tooltips.Insert(tooltipLocation + 5, elemLine);
                        tooltips.Insert(tooltipLocation + 5, elemLine2);
                        break;
                    case ItemID.ArcaneFlower:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 12, ElementID.ArcaneS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.ManaCloak:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 12, ElementID.ArcaneS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.Bezoar:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 8, ElementID.PoisonS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.CountercurseMantra:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.PsychicS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.CrossNecklace:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 5, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FireGauntlet:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 8, ElementID.FireS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FleshKnuckles:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 8, ElementID.BloodS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FrozenTurtleShell:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.IceS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FrozenShield:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.IceS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 5, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 3, elemLine);
                        tooltips.Insert(tooltipLocation + 3, elemLine2);
                        break;
                    case ItemID.HeroShield:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 5, ElementID.BloodS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 5, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 3, elemLine);
                        tooltips.Insert(tooltipLocation + 3, elemLine2);
                        break;
                    case ItemID.PaladinsShield:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 8, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.MedicatedBandage:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.PoisonS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.StarVeil:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 5, ElementID.CelestialS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 5, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.SporeSac:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 12, ElementID.NatureS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Damage", 12, ElementID.PoisonS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.ObsidianSkull:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.FireS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                }
            }
        }
        public override void UpdateEquip(Item item, Terraria.Player player)
        {
            base.UpdateEquip(item, player);

            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            switch (item.type)
            {
                case ItemID.AnkletoftheWind:
                    modPlayer.ElementalDamage[ElementID.Wind] += 0.15f;
                    break;
                case ItemID.CelestialStone:
                    modPlayer.ElementalDamage[ElementID.Celestial] += 0.08f;
                    modPlayer.ElementalResistance[ElementID.Celestial] += 0.08f;
                    break;
                case ItemID.CelestialShell:
                    modPlayer.ElementalDamage[ElementID.Celestial] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Celestial] += 0.1f;
                    break;
                case ItemID.ArcaneFlower:
                    modPlayer.ElementalDamage[ElementID.Arcane] += 0.12f;
                    break;
                case ItemID.ManaCloak:
                    modPlayer.ElementalResistance[ElementID.Arcane] += 0.12f;
                    break;
                case ItemID.Bezoar:
                    modPlayer.ElementalResistance[ElementID.Poison] += 0.08f;
                    break;
                case ItemID.CountercurseMantra:
                    modPlayer.ElementalResistance[ElementID.Psychic] += 0.15f;
                    break;
                case ItemID.CrossNecklace:
                    modPlayer.ElementalDamage[ElementID.Holy] += 0.05f;
                    break;
                case ItemID.FireGauntlet:
                    modPlayer.ElementalDamage[ElementID.Fire] += 0.08f;
                    break;
                case ItemID.FleshKnuckles:
                    modPlayer.ElementalDamage[ElementID.Blood] += 0.08f;
                    break;
                case ItemID.FrozenTurtleShell:
                    modPlayer.ElementalResistance[ElementID.Ice] += 0.1f;
                    break;
                case ItemID.FrozenShield:
                    modPlayer.ElementalResistance[ElementID.Ice] += 0.15f;
                    modPlayer.ElementalResistance[ElementID.Holy] += 0.05f;
                    break;
                case ItemID.HeroShield:
                    modPlayer.ElementalDamage[ElementID.Blood] += 0.05f;
                    modPlayer.ElementalResistance[ElementID.Holy] += 0.05f;
                    break;
                case ItemID.PaladinsShield:
                    modPlayer.ElementalResistance[ElementID.Holy] += 0.08f;
                    break;
                case ItemID.MedicatedBandage:
                    modPlayer.ElementalResistance[ElementID.Poison] += 0.1f;
                    break;
                case ItemID.StarVeil:
                    modPlayer.ElementalDamage[ElementID.Celestial] += 0.05f;
                    modPlayer.ElementalDamage[ElementID.Holy] += 0.05f;
                    break;
                case ItemID.SporeSac:
                    modPlayer.ElementalDamage[ElementID.Nature] += 0.12f;
                    modPlayer.ElementalDamage[ElementID.Poison] += 0.12f;
                    break;
                case ItemID.ObsidianSkull:
                    modPlayer.ElementalResistance[ElementID.Fire] += 0.1f;
                    break;
            }
        }
    }
}