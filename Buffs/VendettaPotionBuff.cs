using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class VendettaPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Thorns");
            Description.SetDefault("\"Attackers also take damage, and get inflicted by poison\"");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().vendetta = true;
            player.thorns += 0.4f;
        }
    }
}