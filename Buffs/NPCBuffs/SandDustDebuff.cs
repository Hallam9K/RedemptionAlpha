using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class SandDustDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dusted");
			// Description.SetDefault("Defense slightly decreased");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.confused = true;
            player.RedemptionPlayerBuff().sandDust = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.confused = true;
            npc.RedemptionNPCBuff().sandDust = true;
        }
    }
}