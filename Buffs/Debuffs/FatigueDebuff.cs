using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class FatigueDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            Main.buffNoTimeDisplay[Type] = player.RedemptionRad().radiationLevel >= 1;

            player.blackout = true;
            player.blind = true;
            player.moveSpeed *= 0.4f;
        }
    }
}