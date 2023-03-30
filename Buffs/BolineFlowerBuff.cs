using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class BolineFlowerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flower Power");
            // Description.SetDefault("Increased life regen and knockback immunity");
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 2;
            player.noKnockback = true;
        }
    }
}