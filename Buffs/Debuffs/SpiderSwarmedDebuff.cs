using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Buffs.Debuffs
{
    public class SpiderSwarmedDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().spiderSwarmed = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().spiderSwarmed = true;
        }
    }
}