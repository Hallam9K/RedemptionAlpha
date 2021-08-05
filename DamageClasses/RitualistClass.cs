using Terraria.ModLoader;

namespace Redemption.DamageClasses
{
    public class RitualistClass : DamageClass
    {
        public override void SetStaticDefaults()
        {
            ClassName.SetDefault("ritual damage");
        }
        protected override float GetBenefitFrom(DamageClass damageClass)
        {
            
            if (damageClass == Generic)
                return 1f;
            return 0f;
        }
    }
}