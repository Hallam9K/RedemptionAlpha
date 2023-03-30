using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class RevolverTossBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cool Bonus");
            // Description.SetDefault("Boosted firerate for being cool!");
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffType[buffIndex] = ModContent.BuffType<RevolverTossBuff2>();
            player.buffTime[buffIndex] = 360;
            return true;
        }
    }
    public class RevolverTossBuff2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Extra Cool Bonus");
            // Description.SetDefault("Boosted firerate for being cool!");
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffType[buffIndex] = ModContent.BuffType<RevolverTossBuff3>();
            player.buffTime[buffIndex] = 300;
            return true;
        }
    }
    public class RevolverTossBuff3 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Extremely Cool Bonus");
            // Description.SetDefault("Boosted firerate for being cool!");
        }
    }
}