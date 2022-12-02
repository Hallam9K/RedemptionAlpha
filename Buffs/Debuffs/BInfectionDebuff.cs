using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class BInfectionDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().bInfection = true;
        }
    }
}