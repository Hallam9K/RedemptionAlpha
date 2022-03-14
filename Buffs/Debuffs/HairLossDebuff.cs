using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Buffs.Debuffs
{
    public class HairLossDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().hairLoss = true;
        }
    }
}