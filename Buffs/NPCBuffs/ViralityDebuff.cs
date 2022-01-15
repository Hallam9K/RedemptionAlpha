using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
	public class ViralityDebuff : ModBuff
	{
		public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Virality");
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<BuffNPC>().infected = true;
		}
	}
}
