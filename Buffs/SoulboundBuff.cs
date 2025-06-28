using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class SoulboundBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}