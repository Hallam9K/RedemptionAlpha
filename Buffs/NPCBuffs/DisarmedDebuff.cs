using Terraria;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Buffs.NPCBuffs
{
    public class DisarmedDebuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().disarmed = true;
        }
    }
}