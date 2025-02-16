using Redemption.BaseExtension;
using Terraria;

namespace Redemption.Prefixes
{
    public class CrestfallenPrefix : ModPrefixCrux
    {
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f - 0.076f;
        }
        public override void Apply(Item item)
        {
            item.Redemption().CruxHealthPrefix = 1f - .2f;
            item.Redemption().CruxDefensePrefix = 1f - .2f;
        }
    }
}