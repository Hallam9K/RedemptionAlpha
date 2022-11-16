using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class SunAuraBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun Aura");
            Description.SetDefault("Empowered by ancient lihzahrd powers");
            Main.buffNoTimeDisplay[Type] = true;
        }
        // TODO: look up whip buffs
        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}