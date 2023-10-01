using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.ID;

namespace Redemption.Buffs.NPCBuffs
{
    public class ContagionShardDebuff : ModBuff
	{
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Contagion Shard");
			// Description.SetDefault("yowie!");
            Main.debuff[Type] = true;
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BoneJavelin);
        }

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.RedemptionNPCBuff().contagionShard = true;
		}
	}
}
