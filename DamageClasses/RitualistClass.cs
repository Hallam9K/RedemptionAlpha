using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.DamageClasses
{
    public class RitualistClass : DamageClass
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ritual damage");
        }
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass) => false;

        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<RitualistClass>() += 4;
        }
        public override bool UseStandardCritCalcs => true;
    }
    public class RitItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!item.CountsAsClass<RitualistClass>())
                return;

            TooltipLine ritLine = new(Mod, "RitLine", "NOTE: This class is not yet complete") { OverrideColor = Colors.RarityAmber };
            tooltips.Add(ritLine);
        }
    }
}