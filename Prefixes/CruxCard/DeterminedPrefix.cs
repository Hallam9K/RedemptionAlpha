using Redemption.BaseExtension;
using Terraria;

namespace Redemption.Prefixes
{
    public class DeterminedPrefix : ModPrefixCrux
    {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult *= 1f + .08f;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.04f;
        }
        public override void Apply(Item item)
        {
            item.Redemption().CruxDefensePrefix = 1f + .16f;
        }
    }
}