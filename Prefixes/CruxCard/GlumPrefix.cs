using Redemption.BaseExtension;
using Terraria;

namespace Redemption.Prefixes
{
    public class GlumPrefix : ModPrefixCrux
    {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult *= 1f - .04f;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f - 0.23f;
        }
        public override void Apply(Item item)
        {
            item.Redemption().CruxDefensePrefix = 1f - .06f;
        }
    }
}