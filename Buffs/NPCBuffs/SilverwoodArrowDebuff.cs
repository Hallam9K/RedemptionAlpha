using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class SilverwoodArrowDebuff : ModBuff
	{
        public override string Texture => "_DebuffTemplate";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Silverwood Arrow");
			Description.SetDefault("yowie!");
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<BuffNPC>().silverwoodArrow = true;
		}
	}
}
