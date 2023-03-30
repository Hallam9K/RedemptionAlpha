using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs
{
    public class SoulBaubleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magic Absorption");
            // Description.SetDefault("Increased magic damage from draining enemies");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<MagicDamageClass>() += player.RedemptionPlayerBuff().trappedSoulBoost;
        }
    }
}