using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Summon;
using Terraria;

namespace Redemption.Prefixes
{
    public class GladPrefix : ModPrefixCrux
    {
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.02f;
        }
        public override void Apply(Item item)
        {
            float health = .1f;
            if (item.ModItem is BaseCruxCard card && card.BossCard)
                health /= 2;
            item.Redemption().CruxHealthPrefix = 1f + health;
        }
    }
}