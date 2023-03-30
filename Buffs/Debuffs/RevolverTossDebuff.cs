using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class RevolverTossDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Epic Fail!");
            // Description.SetDefault("You fool! You idiot! You dropped your gun!");
            Main.debuff[Type] = true;
        }
    }
}