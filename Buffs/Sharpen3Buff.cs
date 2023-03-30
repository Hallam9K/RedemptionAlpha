using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class Sharpen3Buff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sharpened III");
            // Description.SetDefault("Melee weapons have extreme armor penetration");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 32;
        }
    }
}