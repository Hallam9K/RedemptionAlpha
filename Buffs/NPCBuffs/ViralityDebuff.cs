using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.NPCBuffs
{
    public class ViralityDebuff : ModBuff
	{
		public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Virality");
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.RedemptionNPCBuff().infected = true;
		}
	}
}
