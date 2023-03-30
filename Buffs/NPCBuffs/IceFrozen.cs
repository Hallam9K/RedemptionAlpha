using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.NPCBuffs
{
    public class IceFrozen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Frozen");
            // Description.SetDefault("brrr");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().iceFrozen = true;
        }
    }
}