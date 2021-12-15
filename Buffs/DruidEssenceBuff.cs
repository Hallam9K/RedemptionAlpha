using Redemption.DamageClasses;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class DruidEssenceBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Druidic Essence");
			Description.SetDefault("Increased druidic damage and crit");
            Main.buffNoTimeDisplay[Type] = false;
        }
		public override void Update(Player player, ref int buffIndex)
		{
            player.GetDamage<DruidClass>() += 1.05f;
            player.GetCritChance<DruidClass>() += 5;
        }
	}
}
