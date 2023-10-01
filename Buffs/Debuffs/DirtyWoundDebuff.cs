using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.ID;

namespace Redemption.Buffs.Debuffs
{
    public class DirtyWoundDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Bleeding);
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BloodButcherer);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().dirtyWound = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().dirtyWound = true;
        }
    }
}