using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class DreambinderBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dreambound");
            Description.SetDefault("\"Increases length of invincibility\"");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.longInvince = true;
        }
    }
}