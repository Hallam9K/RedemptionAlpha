using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class ShineArmour3Buff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shined Armor III");
            // Description.SetDefault("Greatly increased defense");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 8;
        }
    }
}