using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.ID;

namespace Redemption.Buffs.NPCBuffs
{
    public class BrokenArmorDebuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BrokenArmor);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().brokenArmor = true;
        }
    }
}