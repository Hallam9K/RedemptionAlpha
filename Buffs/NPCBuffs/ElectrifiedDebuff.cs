using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.ID;

namespace Redemption.Buffs.NPCBuffs
{
    public class ElectrifiedDebuff : ModBuff
	{
		public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Electrified");
			Main.debuff[Type] = true;
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Electrified);
        }

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.RedemptionNPCBuff().electrified = true;
		}
	}
}
