using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Buffs
{
    public class VendettaPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Poison Thorns");
            // Description.SetDefault("Attackers also take damage, and get inflicted by poison");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().vendetta = true;
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Poison] += .1f;
            player.thorns += 0.4f;
        }
    }
}