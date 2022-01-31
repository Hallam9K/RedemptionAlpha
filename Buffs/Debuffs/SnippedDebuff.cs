using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class SnippedDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snipped");
            Description.SetDefault("Halved wing time");
            Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
        }
		public override void Update(Player player, ref int buffIndex)
		{
			player.RedemptionPlayerBuff().snipped = true;
		}
	}
}