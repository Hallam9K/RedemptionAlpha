using Redemption.Globals.NPC;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class BileDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burning Acid");
			Description.SetDefault("Reduced defence");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().bileDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().bileDebuff = true;
        }
    }
}