using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class RadioactiveFalloutDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Radioactive Fallout");
			// Description.SetDefault("Stats greatly decreased due to radioactivity");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;

            player.GetDamage(DamageClass.Generic) *= 0.85f;
            player.statDefense -= 6;
            player.moveSpeed *= 0.7f;
        }
	}
}