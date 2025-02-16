using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Summon;
using Terraria;

namespace Redemption.Prefixes
{
    public class StalwartPrefix : ModPrefixCrux
    {
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.082f;
        }
        public override void Apply(Item item)
        {
            float health = .15f;
            if (item.ModItem is BaseCruxCard card && card.BossCard)
                health /= 2;
            item.Redemption().CruxHealthPrefix = 1f + health;
            item.Redemption().CruxDefensePrefix = 1f + .15f;
        }
    }
}