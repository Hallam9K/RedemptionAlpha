using Terraria;
using Terraria.ModLoader;

namespace Redemption.DamageClasses
{
    public class RitualistClass : DamageClass
    {
        public override void SetStaticDefaults()
        {
            ClassName.SetDefault("ritual damage");
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
}