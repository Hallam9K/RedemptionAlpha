using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class NauseaDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nausea");
			Description.SetDefault("You feel sick.");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.manaSick = true;
            player.confused = true;
            player.moveSpeed *= 0.5f;
            player.blind = true;
            player.stinky = true;
        }
	}
}