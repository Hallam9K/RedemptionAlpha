using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class HolyFireDebuff : ModBuff
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Incandesence");
            // Description.SetDefault("NO! TOO GLORIOUS! STOP!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().holyFire = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().holyFire = true;
        }
    }
}