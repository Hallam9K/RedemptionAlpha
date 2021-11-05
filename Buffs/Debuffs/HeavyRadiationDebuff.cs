using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class HeavyRadiationDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heavy Radiation");
			Description.SetDefault("Stats greatly decreased due to radioactivity");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;

            player.lifeRegen = -4;
            player.GetDamage(DamageClass.Generic) *= 0.75f;
            player.statDefense -= 18;
            player.moveSpeed *= 0.5f;
        }
	}
}