using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class sansDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("sans");
			// Description.SetDefault("Bad time time");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.confused = true;
            player.RedemptionPlayerBuff().badtime = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.confused = true;
            npc.RedemptionNPCBuff().badtime = true;
        }
    }
}