using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

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
        }

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.RedemptionNPCBuff().contagionShard = true;
		}
	}
}
