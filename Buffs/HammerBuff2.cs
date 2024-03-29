using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class HammerBuff2 : ModBuff
    {
        public override string Texture => "Redemption/Buffs/HammerBuff";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heavy");
            // Description.SetDefault("Increased fall speed");
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.maxFallSpeed += 50;
        }
    }
}