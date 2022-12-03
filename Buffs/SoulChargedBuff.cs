using Redemption.DamageClasses;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class SoulChargedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Charged");
            Description.SetDefault("Increased ritual damage");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<RitualistClass>().Flat += 4;
        }
    }
}