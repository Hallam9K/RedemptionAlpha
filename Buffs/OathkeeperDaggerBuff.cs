using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class OathkeeperDaggerBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stabbed");
			Description.SetDefault("Your damage is increased at the cost of decreased max life");
			Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
            player.statLifeMax2 -= (int)(player.statLifeMax2 * 0.4f);
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
            player.bleed = true;
        }
    }
}