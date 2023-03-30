using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Buffs
{
    public class ShadowFuelBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Fuel");
            // Description.SetDefault("Increased Shadow damage");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Shadow] += 0.1f;
        }
    }
}