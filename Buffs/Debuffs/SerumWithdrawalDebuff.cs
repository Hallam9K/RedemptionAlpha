using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Buffs.Debuffs
{
    public class SerumWithdrawalDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Serum Withdrawal");
            // Description.SetDefault("Slowed and cannot take more serums");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed *= 0.8f;
        }
    }
}