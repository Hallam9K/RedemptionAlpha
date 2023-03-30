using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Minions
{
	public class NestWandBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Chikcen");
			// Description.SetDefault("Chikcen");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<NestWand_Proj>()] > 0)
				player.buffTime[buffIndex] = 18000;
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}