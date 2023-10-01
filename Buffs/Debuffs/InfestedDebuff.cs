using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class InfestedDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Bleeding);
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BloodButcherer);

        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.RedemptionPlayerBuff().infestedTime += 60;
            return false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().infested = true;
        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.RedemptionNPCBuff().infestedTime += 60;
            return false;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().infested = true;
        }
    }
}