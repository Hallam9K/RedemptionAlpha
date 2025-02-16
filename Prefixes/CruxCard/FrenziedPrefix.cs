using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Summon;
using Terraria;

namespace Redemption.Prefixes
{
    public class FrenziedPrefix : ModPrefixCrux
    {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult *= 1f + 0.5f;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.083f;
        }
        public override void Apply(Item item)
        {
            float health = .3f;
            if (item.ModItem is BaseCruxCard card && card.BossCard)
                health = .2f;
            item.Redemption().CruxHealthPrefix = 1f - health;
            item.Redemption().CruxDefensePrefix = 1f - .2f;
        }
    }
}