using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class VasaraHealBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shaman's Aura");
            // Description.SetDefault("Greatly increased life regeneration");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 20;
        }
    }
}