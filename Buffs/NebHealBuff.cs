using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class NebHealBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vigorous Aura");
            Description.SetDefault("A strange aura is healing you...");
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 5;
        }
    }
}