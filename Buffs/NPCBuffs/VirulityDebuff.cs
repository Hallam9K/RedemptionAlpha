using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
	public class VirulityDebuff : ModBuff
	{
		public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Virulity");
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<BuffNPC>().infected = true;
		}
	}
}
