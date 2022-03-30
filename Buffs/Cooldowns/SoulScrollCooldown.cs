using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Cooldowns
{
	public class SoulScrollCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Scroll Cooldown");
			Description.SetDefault("You cannot use the Soul Scroll");
			Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
	}
}