using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class FeverDebuff : ModBuff
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

            player.manaSick = true;
            player.blind = true;
            player.bleed = true;
            player.chilled = true;
        }
    }
}