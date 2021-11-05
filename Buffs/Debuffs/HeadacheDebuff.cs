using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class HeadacheDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Headache");
			Description.SetDefault("Sudden and pounding pain from the inside of your skull.");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.blackout = true;
            player.confused = true;
            player.dazed = true;
        }
	}
}