using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.NPCBuffs
{
    public class UkonArrowDebuff : ModBuff
	{
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ukonvasara Arrow");
			Description.SetDefault("yowie!");
            Main.debuff[Type] = true;
        }

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.RedemptionNPCBuff().ukonArrow = true;
		}
	}
}
