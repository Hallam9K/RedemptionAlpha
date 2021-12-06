using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Cooldowns
{
	public class HardlightCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SoS Cooldown");
			Description.SetDefault("Hey, the ship can't help you all the time.");
			Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
	}
}