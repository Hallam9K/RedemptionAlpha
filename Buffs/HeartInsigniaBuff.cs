using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class HeartInsigniaBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heart Rush");
            // Description.SetDefault("Life regen greatly increased");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 6;
        }
    }
}