using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class FeverDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fever");
			Description.SetDefault("You feel extremely ill.");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.manaSick = true;
            player.blind = true;
            player.bleed = true;
            player.chilled = true;
        }
	}
}