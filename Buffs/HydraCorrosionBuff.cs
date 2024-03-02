using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class HydraCorrosionBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().hydraCorrosion = true;
        }
    }
}
