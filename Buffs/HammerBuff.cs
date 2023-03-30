using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class HammerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heavy");
            // Description.SetDefault("Increased fall speed");
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.maxFallSpeed += 10;
        }
    }
}