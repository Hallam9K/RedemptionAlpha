using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.NPCBuffs
{
    public class SilverwoodArrowDebuff : ModBuff
	{
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Silverwood Arrow");
			Description.SetDefault("yowie!");
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.RedemptionNPCBuff().silverwoodArrow = true;
		}
	}
}
