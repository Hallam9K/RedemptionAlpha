using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class ShineArmour2Buff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shined Armor II");
            // Description.SetDefault("Increased defense");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 6;
        }
    }
}