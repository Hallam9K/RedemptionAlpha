using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class HKStatueBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("...");
            // Description.SetDefault("You feel like you're being watched...");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}