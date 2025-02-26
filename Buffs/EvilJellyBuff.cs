using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class EvilJellyBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Devilishly Delicious");
            // Description.SetDefault("Increased drop rate of Shadow Fuel");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Shadow] += 0.08f;
        }
    }
}