using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class XeniumGasDebuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Virality");
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().xeniumGas = true;
        }
    }
}
