using Redemption.BaseExtension;
using Terraria;

namespace Redemption.Prefixes
{
    public class HardyPrefix : ModPrefixCrux
    {
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.02f;
        }
        public override void Apply(Item item)
        {
            item.Redemption().CruxDefensePrefix = 1f + .1f;
        }
    }
}