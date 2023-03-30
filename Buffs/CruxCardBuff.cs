using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
	public class CruxCardBuff : ModBuff
	{
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crux Card");
            // Description.SetDefault("Right-click to despawn spirit summons");
            Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			bool spiritActive = false;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.active || !npc.friendly || !npc.Redemption().spiritSummon || npc.ai[3] != player.whoAmI)
					continue;
				spiritActive = true;
            }
			if (spiritActive)
				player.buffTime[buffIndex] = 18000;
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}