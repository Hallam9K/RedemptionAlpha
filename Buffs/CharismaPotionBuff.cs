using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs
{
    public class CharismaPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charisma");
            // Description.SetDefault("Shops have lower prices and enemies drop more gold");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.discountAvailable = true;
            player.RedemptionPlayerBuff().charisma = true;
        }
    }
}