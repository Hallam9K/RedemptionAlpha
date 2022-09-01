using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Cooldowns
{
	public class RevolverTossCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
	}
}