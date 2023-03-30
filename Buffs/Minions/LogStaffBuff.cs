using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Minions
{
	public class LogStaffBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("LOG");
			// Description.SetDefault("A small log, perhaps a tiny squirrel is hiding inside it");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<LogStaff_Proj>()] > 0)
				player.buffTime[buffIndex] = 18000;
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}