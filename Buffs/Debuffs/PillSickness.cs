using Terraria;
using Terraria.ID;
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
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.blind = true;
            player.chilled = true;
        }
    }
}