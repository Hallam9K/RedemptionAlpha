using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class RoosterAuraBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rooster Boost");
            // Description.SetDefault("You feel bucked up... and annoyed");
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.2f;
            player.jumpSpeedBoost += 2f;
        }
    }
}
