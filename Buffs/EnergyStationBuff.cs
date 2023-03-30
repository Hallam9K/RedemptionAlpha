using Terraria;
using Terraria.ModLoader;
using Redemption.Globals.Player;

namespace Redemption.Buffs
{
    public class EnergyStationBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Regeneration");
            // Description.SetDefault("Increased Energy regen");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<EnergyPlayer>().energyRegen += 50;
        }
    }
}