using System.Collections.Generic;
using Terraria.ModLoader;

namespace Redemption.DamageClasses
{
    public class DruidClass : DamageClass
    {
        public override void SetStaticDefaults()
        {
            ClassName.SetDefault("druidic damage");
        }
        protected override float GetBenefitFrom(DamageClass damageClass)
        {

            if (damageClass == DamageClass.Generic)
                return 1f;
            return 0f;
        }
    }
}