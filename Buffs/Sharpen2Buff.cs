using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class Sharpen2Buff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sharpened II");
            // Description.SetDefault("Melee weapons have high armor penetration");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 16;
        }
    }
}