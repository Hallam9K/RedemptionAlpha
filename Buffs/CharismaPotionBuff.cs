using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class CharismaPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charisma");
            Description.SetDefault("\"Shops have lower prices and enemies drop more gold\"");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.discount = true;
            player.GetModPlayer<BuffPlayer>().charisma = true;
        }
    }
}