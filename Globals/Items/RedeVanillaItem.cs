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
                    case ItemID.AngelWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.DemonWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.ShadowS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FairyWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FinWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.WaterS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FrozenWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.IceS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.HarpyWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.WindS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.LeafWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.NatureS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.BatWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.BloodS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.BeeWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.PoisonS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.ButterflyWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.NatureS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FlameWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.FireS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.BoneWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.EarthS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.GhostWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.ArcaneS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.MothronWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.ShadowS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.CelestialS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.BeetleWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.EarthS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.FestiveWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.NatureS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.IceS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.SpookyWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.ShadowS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.TatteredFairyWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.ShadowS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.HolyS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.SteampunkWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 15, ElementID.WindS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        break;
                    case ItemID.BetsyWings:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.WindS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.FireS));
                        tooltips.Insert(tooltipLocation + 3, elemLine);
                        tooltips.Insert(tooltipLocation + 3, elemLine2);
                        break;
                    case ItemID.WingsNebula:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.CelestialS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.ArcaneS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.WingsVortex:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.CelestialS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.ThunderS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.WingsSolar:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.CelestialS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.FireS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
                        break;
                    case ItemID.WingsStardust:
                        elemLine = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.CelestialS));
                        elemLine2 = new(Mod, "TooltipElem", Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance", 10, ElementID.ArcaneS));
                        tooltips.Insert(tooltipLocation + 1, elemLine);
                        tooltips.Insert(tooltipLocation + 1, elemLine2);
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
                case ItemID.AngelWings:
                    modPlayer.ElementalResistance[ElementID.Holy] += 0.15f;
                    break;
                case ItemID.DemonWings:
                    modPlayer.ElementalResistance[ElementID.Shadow] += 0.15f;
                    break;
                case ItemID.FairyWings:
                    modPlayer.ElementalResistance[ElementID.Holy] += 0.15f;
                    break;
                case ItemID.FinWings:
                    modPlayer.ElementalResistance[ElementID.Water] += 0.15f;
                    break;
                case ItemID.FrozenWings:
                    modPlayer.ElementalResistance[ElementID.Ice] += 0.15f;
                    break;
                case ItemID.HarpyWings:
                    modPlayer.ElementalResistance[ElementID.Wind] += 0.15f;
                    break;
                case ItemID.LeafWings:
                    modPlayer.ElementalResistance[ElementID.Nature] += 0.15f;
                    break;
                case ItemID.BatWings:
                    modPlayer.ElementalResistance[ElementID.Blood] += 0.15f;
                    break;
                case ItemID.BeeWings:
                    modPlayer.ElementalResistance[ElementID.Poison] += 0.15f;
                    break;
                case ItemID.ButterflyWings:
                    modPlayer.ElementalResistance[ElementID.Nature] += 0.15f;
                    break;
                case ItemID.FlameWings:
                    modPlayer.ElementalResistance[ElementID.Fire] += 0.15f;
                    break;
                case ItemID.BoneWings:
                    modPlayer.ElementalResistance[ElementID.Earth] += 0.15f;
                    break;
                case ItemID.GhostWings:
                    modPlayer.ElementalResistance[ElementID.Arcane] += 0.15f;
                    break;
                case ItemID.MothronWings:
                    modPlayer.ElementalResistance[ElementID.Shadow] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Celestial] += 0.1f;
                    break;
                case ItemID.BeetleWings:
                    modPlayer.ElementalResistance[ElementID.Earth] += 0.15f;
                    break;
                case ItemID.FestiveWings:
                    modPlayer.ElementalResistance[ElementID.Nature] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Ice] += 0.1f;
                    break;
                case ItemID.SpookyWings:
                    modPlayer.ElementalResistance[ElementID.Shadow] += 0.15f;
                    break;
                case ItemID.TatteredFairyWings:
                    modPlayer.ElementalResistance[ElementID.Holy] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Shadow] += 0.1f;
                    break;
                case ItemID.SteampunkWings:
                    modPlayer.ElementalResistance[ElementID.Wind] += 0.15f;
                    break;
                case ItemID.BetsyWings:
                    modPlayer.ElementalResistance[ElementID.Wind] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Fire] += 0.1f;
                    break;
                case ItemID.WingsNebula:
                    modPlayer.ElementalResistance[ElementID.Celestial] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Arcane] += 0.1f;
                    break;
                case ItemID.WingsVortex:
                    modPlayer.ElementalResistance[ElementID.Celestial] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Thunder] += 0.1f;
                    break;
                case ItemID.WingsSolar:
                    modPlayer.ElementalResistance[ElementID.Celestial] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Fire] += 0.1f;
                    break;
                case ItemID.WingsStardust:
                    modPlayer.ElementalResistance[ElementID.Celestial] += 0.1f;
                    modPlayer.ElementalResistance[ElementID.Arcane] += 0.1f;
                    break;
            }
        }
    }
}