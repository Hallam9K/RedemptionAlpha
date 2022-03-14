using Terraria;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Buffs.NPCBuffs
{
    public class ElectrifiedDebuff : ModBuff
	{
		public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electrified");
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.RedemptionNPCBuff().electrified = true;
		}
	}
}
