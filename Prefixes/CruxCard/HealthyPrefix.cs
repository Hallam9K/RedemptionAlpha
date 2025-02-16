using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Summon;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Prefixes
{
    public abstract class ModPrefixCrux : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.Custom;
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (item.Redemption().CruxHealthPrefix != 1f)
            {
                yield return new TooltipLine(Mod, "PrefixHealth", CruxHealthTooltip.Format((int)(item.Redemption().CruxHealthPrefix * 100f) - 100))
                {
                    IsModifier = true,
                    IsModifierBad = item.Redemption().CruxHealthPrefix < 1f
                };
            }
            if (item.Redemption().CruxDefensePrefix != 1f)
            {
                yield return new TooltipLine(Mod, "PrefixDefense", CruxDefenseTooltip.Format((int)(item.Redemption().CruxDefensePrefix * 100f) - 100))
                {
                    IsModifier = true,
                    IsModifierBad = item.Redemption().CruxDefensePrefix < 1f
                };
            }
        }
        public static LocalizedText CruxHealthTooltip { get; private set; }
        public static LocalizedText CruxDefenseTooltip { get; private set; }
        public override void SetStaticDefaults()
        {
            CruxHealthTooltip = Language.GetOrRegister(Mod.GetLocalizationKey($"{LocalizationCategory}.{nameof(CruxHealthTooltip)}"));
            CruxDefenseTooltip = Language.GetOrRegister(Mod.GetLocalizationKey($"{LocalizationCategory}.{nameof(CruxDefenseTooltip)}"));
        }
    }
    public class HealthyPrefix : ModPrefixCrux
    {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult *= 1f + 0.06f;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.2f;
        }
        public override void Apply(Item item)
        {
            float health = .15f;
            if (item.ModItem is BaseCruxCard card && card.BossCard)
                health /= 2;
            item.Redemption().CruxHealthPrefix = 1f + health;
        }
    }
}