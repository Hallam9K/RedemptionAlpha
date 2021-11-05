using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class PillSickness : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pill Sickness");
			Description.SetDefault("You feel sick");
			Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
            CanBeCleared = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.blind = true;
            player.chilled = true;
        }
    }
}